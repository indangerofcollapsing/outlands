using System;
using System.Collections.Generic;
using Server.Custom.Townsystem;
using Server.Regions;
using Server.Mobiles;
using Server.Network;
using Server.Items;
using Server.Commands;
using Server.Achievements;
using System.Linq;

namespace Server.Custom
{
    public enum WindState
    {
        Pending,
        Announced,
        Active
    }

    public static class WindBattleground
    {
        private static bool m_Enabled = true;
        
        public static void Initialize()
        {
            Timer.DelayCall(TimeSpan.FromTicks(1), delegate
            {
                if (PersistanceItem == null)
                    PersistanceItem = new WindBattlegroundPersistance();
            });

            if (IsActive())
            {
                AddFragment();
            }

            if (m_Enabled)
                StartTimer();
            else
            {
                Utility.PushColor(ConsoleColor.Yellow);
                Console.WriteLine("Wind Battleground is not currently active.");
                Utility.PopColor();
            }
        }

        private static WindState m_State;
        private static bool m_Active;

        // morning time slot at 7 AM CST for koreans
        private static int m_KoreanSlot = 7;
        // afternoon time slot at 1 PM CST for euros
        private static int m_EuropeanSlot = 13;
        // evening time slot at 7 PM CST for east coast
        private static int m_EastCoastSlot = 19;

        private const int MinMaxPerTeam = 3;
        private const int MaxMaxPerTeam = 8;
        private const int WinPoints = 500;
        private const int PointsPerTick = 1;
        public const int CarrierHue = 148;
        private static bool m_Ended = false;

        public static Point3D[] FragmentSpawnLocations = new Point3D[] { new Point3D(5214, 136, 0), new Point3D(5350, 92, 15), new Point3D(5204, 74, 17) };
        public static Rectangle2D EntireWindRect = new Rectangle2D(new Point2D(5110, 0), new Point2D(5372, 250));
        public static Point3D KickoutLocation = new Point3D(1362, 896, 0);

        public static List<WindBrazier> Braziers = new List<WindBrazier>();
        public static List<WindTrackArrow> Arrows = new List<WindTrackArrow>();
        public static Dictionary<Town, int> Participants = new Dictionary<Town, int>();
        public static Dictionary<Town, int> Scores = new Dictionary<Town, int>();
        public static WindBattlegroundPersistance PersistanceItem;

        public static Town FragmentController;
        public static WindDungeonRegion Region;
        public static WindFragment Fragment;
        public static Mobile FragmentHolder;
        private static Timer m_Timer;
        public static Town Owner { get; set; }

        public static void ResetOwner()
        {
            Owner = null;
        }

        public static TimeSpan UntilNextActive()
        {
            DateTime now = DateTime.Now;
            DateTime today = DateTime.Today;

            if (IsActive())
            {
                return TimeSpan.Zero;
            }
            else
            {
                if (now.Hour > m_EastCoastSlot)
                {
                    return today + TimeSpan.FromDays(1) + TimeSpan.FromHours(m_KoreanSlot) - now;
                }
                else if (now.Hour > m_EuropeanSlot)
                {
                    return today + TimeSpan.FromHours(m_EastCoastSlot) - now;
                }
                else if (now.Hour > m_KoreanSlot)
                {
                    return today + TimeSpan.FromHours(m_EuropeanSlot) - now;
                }
                else
                {
                    return today + TimeSpan.FromHours(m_KoreanSlot) - now;
                }
            }

        }

        public static void StartTimer()
        {
            TimeSpan start;

            if (IsActive())
            {
                m_Active = true;
                m_Ended = false;
                m_State = WindState.Active;
                start = TimeSpan.Zero;
            }

            else
            {
                start = UntilNextActive();
                m_State = WindState.Pending;
            }
            
            m_Timer = Timer.DelayCall(TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(5), Slice);
        }

        public static bool IsActive()
        {
            if (!m_Enabled)
                return false;

            DateTime now = DateTime.Now;

            return now.Hour == m_KoreanSlot || now.Hour == m_EuropeanSlot || now.Hour == m_EastCoastSlot;

        }

        public static void Slice()
        {
            switch (m_State)
            {
                case (WindState.Pending):
                    {
                        if (UntilNextActive() > TimeSpan.FromHours(1))
                            break;

                        DateTime now = DateTime.Now;
                        Town.GlobalTownCrierBroadcast(new string[] { "To arms! Wind is vulnerable to capture by militias at the top of the hour!" }, UntilNextActive());
                        m_State = WindState.Announced;
                    } break;

                case (WindState.Announced):
                    {
                        if (IsActive())
                        {
                            DateTime now = DateTime.Now;
                            Faction.BroadcastAll("Wind Battleground has now begun.");
                            Begin();
                            Town.GlobalTownCrierBroadcast(new string[] { "To arms! Wind is vulnerable to capture by enemy militias!" }, TimeSpan.FromHours(1));
                            m_State = WindState.Active;
                            m_Active = true;
                            m_Ended = false;
                        }
                    } break;

                case (WindState.Active):
                    {
                        if (!IsActive())
                        {
                            m_Active = false;
                            m_State = WindState.Pending;
                            m_Timer.Stop();
                            StartTimer();
                            if (!m_Ended)
                                End(null);
                        } 
                        else
                        {
                            AwardPoints();
                        }
                    } break;
            }
        }

        public static void AwardPoints()
        {
            if (FragmentController != null)
            {
                if (!Scores.ContainsKey(FragmentController))
                    Scores.Add(FragmentController, 0);

                Scores[FragmentController] += PointsPerTick;

                foreach (WindBrazier wb in Braziers)
                {
                    if (!wb.Captured || wb.CapTown != FragmentController) continue;

                    Scores[FragmentController] += PointsPerTick;

                    wb.PublicOverheadMessage(Network.MessageType.Regular, FragmentController.HomeFaction.Definition.HuePrimary, false, String.Format("[{0}] {2}", wb.CapTown, Braziers.Count, Scores[wb.CapTown]));
                }

                if (Scores[FragmentController] > WinPoints)
                {
                    End(FragmentController);
                }

                if (Fragment != null)
                {
                    var owner = Fragment.FindOwner();
                    if (owner != null)
                        owner.PublicOverheadMessage(MessageType.Regular, CarrierHue, true, Scores[FragmentController].ToString());
                }
            }
        }

        public static void AddFragment()
        {
            if (Fragment == null || Fragment.Deleted)
                Fragment = new WindFragment();

            Fragment.MoveToWorld(FragmentSpawnLocations[Utility.Random(FragmentSpawnLocations.Length - 1)], Map.Felucca);
        }

        public static void Begin()
        {
            if (!m_Enabled)
                return;

            ResetOwner();

            AddFragment();

            if (Region == null)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine("WindBattleground: ERROR - Region is null.");
                Utility.PopColor();
                return;
            }

            m_Ended = false;
            SetupParticipants();
            Scores.Clear();
        }

        public static void SetupParticipants()
        {
            Participants.Clear();

            var players = Region.GetPlayers();
            foreach (Mobile m in players)
            {
                var pm = m as PlayerMobile;

                if (pm == null)
                    continue;

                var town = pm.Citizenship;

                if (!pm.IsInMilitia || pm.NetState == null || town == null)
                {
                    Kickout(m);
                    continue;
                }

                if (!Participants.ContainsKey(town))
                    Participants.Add(town, 0);

                if (Participants[town] >= MinMaxPerTeam)
                {
                    Kickout(m);
                    continue;
                }
                Participants[town]++;
            }
        }

        public static void End(Town winner = null)
        {
            m_Ended = true;
            ClearArrows();

            Participants.Clear();

            WindFragment.ClearInstances();

            foreach (WindBrazier wb in Braziers)
                wb.Captured = false;

            if (winner == null && Scores.Count == 0)
            {
                Broadcast("The wind battleground has ended!");
            }
            else
            {
                if (winner == null)
                    winner = Scores.OrderByDescending(key => key.Value).First().Key;

                string winnerName = "";
                if (winner.HomeFaction.Definition.FriendlyName != null)
                {
                    winnerName = winner.HomeFaction.Definition.FriendlyName;
                }
                else
                {
                    winnerName = winner.ToString();
                }

                Broadcast(String.Format("{0} has captured Wind!", winnerName));
                RewardWinners(winner);
                Owner = winner;
            }
        }

        private static void RewardWinners(Town winner)
        {
            var players = Region.GetPlayers();
            foreach (PlayerMobile player in players)
            {
                if (player == null)
                    continue;
                if (player.IsInMilitia && player.Citizenship == winner)
                {
                    var reward = new TreasuryKeyGoodiebag(player, 2000);
                    Item item = Loot.RandomWeapon();
                    ((BaseWeapon)item).DamageLevel = (WeaponDamageLevel)Utility.Random(6); // up to vanq
                    ((BaseWeapon)item).AccuracyLevel = (WeaponAccuracyLevel)Utility.Random(6);
                    ((BaseWeapon)item).DurabilityLevel = (WeaponDurabilityLevel)Utility.Random(6);
                    if (Utility.Random(3000) == 0) // 1 in 3000 silver vanq
                        ((BaseWeapon)item).Slayer = SlayerName.Silver;
                    reward.DropItem(item);
                    player.AddToBackpack(reward);
                    player.TreasuryKeys += 1000;
                }
            }
        }

        public static void Broadcast(string msg)
        {
            if (Region != null)
            {
                var mobiles = Region.GetPlayers();

                Packet p = null;

                foreach (Mobile m in mobiles)
                {
                    var state = m.NetState;

                    if (state != null)
                    {
                        if (p == null)
                            p = Packet.Acquire(new UnicodeMessage(Serial.MinusOne, -1, MessageType.Alliance, 0x0, 3, "ENU", "System", msg));

                        state.Send(p);
                    }
                }

                Packet.Release(p);
            }
        }

        public static void OnEnterDungeon(Mobile m)
        {
            AchievementSystem.Instance.TickProgress(m, AchievementTriggers.Trigger_ExploreWind);
            if (m.AccessLevel == AccessLevel.Player)
            {
                if (IsActive())
                {
                    var town = Town.Find(m);

                    if (town != null)
                    {
                        if (!Participants.ContainsKey(town))
                            Participants.Add(town, 0);

                        Participants[town]++;
                    }
                }

                if (!CanEnter(m))
                {
                    m.SendMessage("You are not allowed entry to Wind at this time.");
                    Kickout(m);
                }
                else
                {
                    if (FragmentHolder != null)
                        TrackFragmentHolder();
                }
            }
        }

        public static void OnExitDungeon(Mobile m)
        {
            if (WindFragment.ExistsOn(m))
            {
                Fragment.ReturnHome();
                m.SendMessage("You have abandonded the Wind Fragment!");
                Broadcast("The Wind Fragment has returned home!");
            }

            if (m.QuestArrow is WindTrackArrow)
                m.QuestArrow.Stop();

            if (m.AccessLevel == AccessLevel.Player)
            {
                var town = Town.Find(m);
                if (town != null)
                {
                    if (Participants.ContainsKey(town))
                    {

                        Participants[town]--;

                        if (Participants[town] < 0)
                        {
                            Participants[town] = 0;
                            Server.Commands.CommandHandlers.BroadcastMessage(AccessLevel.GameMaster, 0x482, String.Format("Error: Participating players would now be negative for ({0}) in the Wind Battleground.", m.Name));
                        }

                        if (!m.Alive && IsActive())
                            Timer.DelayCall(TimeSpan.FromTicks(1), delegate { TeleportHome(m); });

                    }
                }
            }
        }

        private static void TeleportHome(Mobile m)
        {
            if (m.ShortTermMurders > 4)
            {
                Point3D bucs = new Point3D(2725, 2148, 0);
                m.MoveToWorld(bucs, Map.Felucca);
            }
            else if (Town.CheckCitizenship(m) is Cove)
            {
                Point3D cove = new Point3D(2257, 1201, 0);
                m.MoveToWorld(cove, Map.Felucca);
            }
            else if (Town.CheckCitizenship(m) is Jhelom)
            {
                Point3D jhelom = new Point3D(1341, 3763, 0);
                m.MoveToWorld(jhelom, Map.Felucca);
            }
            else if (Town.CheckCitizenship(m) is Minoc)
            {
                Point3D minoc = new Point3D(2501, 572, 0);
                m.MoveToWorld(minoc, Map.Felucca);
            }
            else if (Town.CheckCitizenship(m) is Magincia)
            {
                Point3D magincia = new Point3D(3708, 2140, 20);
                m.MoveToWorld(magincia, Map.Felucca);
            }
            else if (Town.CheckCitizenship(m) is Moonglow)
            {
                Point3D moonglow = new Point3D(4439, 1170, 0);
                m.MoveToWorld(moonglow, Map.Felucca);
            }
            else if (Town.CheckCitizenship(m) is Nujelm)
            {
                Point3D nujelm = new Point3D(3763, 1293, 0);
                m.MoveToWorld(nujelm, Map.Felucca);
            }
            else if (Town.CheckCitizenship(m) is Ocllo)
            {
                Point3D ocllo = new Point3D(3675, 2492, 0);
                m.MoveToWorld(ocllo, Map.Felucca);
            }
            else if (Town.CheckCitizenship(m) is SerpentsHold)
            {
                Point3D serps = new Point3D(3019, 3373, 15);
                m.MoveToWorld(serps, Map.Felucca);
            }
            else if (Town.CheckCitizenship(m) is SkaraBrae)
            {
                Point3D skara = new Point3D(573, 2135, 0);
                m.MoveToWorld(skara, Map.Felucca);
            }
            else if (Town.CheckCitizenship(m) is Vesper)
            {
                Point3D vesper = new Point3D(2889, 711, 0);
                m.MoveToWorld(vesper, Map.Felucca);
            }
            else if (Town.CheckCitizenship(m) is Trinsic)
            {
                Point3D trinsic = new Point3D(1867, 2817, 0);
                m.MoveToWorld(trinsic, Map.Felucca);
            }
            else if (Town.CheckCitizenship(m) is Yew)
            {
                Point3D yew = new Point3D(639, 878, 0);
                m.MoveToWorld(yew, Map.Felucca);
            }
            else
            {
                // default send to brit
                Point3D brit = new Point3D(1545, 1600, 15);
                m.MoveToWorld(brit, Map.Felucca);
            }
        }

        public static void TrackFragmentHolder()
        {
            Timer.DelayCall(TimeSpan.FromTicks(1), () => {
                ClearArrows();
                var players = Region.GetPlayers();

                foreach (var player in players)
                {
                    if (player == FragmentHolder)
                        continue;
                    player.QuestArrow = new WindTrackArrow(player, FragmentHolder);
                }
            });
        }

        public static void ClearArrows()
        {
            while (Arrows.Count > 0)
                Arrows[0].Stop();
        }

        public static void PickupFragment(Mobile m)
        {
            Town fac = Town.Find(m);
            if (fac != null)
            {
                FragmentController = fac;
                FragmentHolder = m;
                TrackFragmentHolder();
                Broadcast(String.Format("{0} has picked up the wind fragment!", m.Name));
            }
        }

        public static void DropFragment(Mobile m)
        {
            ClearArrows();

            if (m != null && Server.Spells.SpellHelper.IsWindLoc(m.Location))
            {
                Broadcast(String.Format("{0} has dropped the Wind Fragment.", m.Name));
            }

            FragmentController = null;
            FragmentHolder = null;
        }

        public static void Kickout(Mobile m)
        {
            if (m.Map == Map.Internal)
            {
                m.LogoutLocation = KickoutLocation;
                m.LogoutMap = Map.Felucca;
            }
            else
            {
                Timer.DelayCall(TimeSpan.FromTicks(1), delegate 
                {
                    BaseCreature.TeleportPets(m, KickoutLocation, Map.Felucca, false, 25);
                    m.MoveToWorld(KickoutLocation, Map.Felucca);
                });
            }
        }

        public static bool MeetsMinimumRequirements(Mobile m)
        {
            return m.Skills.Highest.BaseFixedPoint >= 990;
        }

        public static bool CanEnter(Mobile m)
        {
            if (!(m is PlayerMobile) || m.AccessLevel > AccessLevel.Player)
                return true;

            Town town = Town.Find(m);
            bool militia = ((PlayerMobile)m).IsInMilitia;

            if (militia)
            {
                if (IsActive())
                {
                    if (!MeetsMinimumRequirements(m)) 
                    {
                        m.SendMessage("Sorry, but you cannot enter the Wind battleground with such low skills.");
                        return false;
                    }

                    if (Participants.ContainsKey(town))
                    {
                        if (Participants[town] >= MaxMaxPerTeam)
                        {
                            m.SendMessage("Sorry, but the Wind battleground is currently full.");
                            return false;
                        }
                        else if (Participants[town] >= MinMaxPerTeam)
                        {
                            int min = FindMinimumParticipants();
                            if (Participants[town] > min)
                            {
                                m.SendMessage("Sorry, but the Wind battleground is currently full.");
                                return false;
                            }
                        }
                    }
                }
            }
            else
            {
                if (IsActive())
                {
                    m.SendMessage("You must be in militia to enter during the battleground.");
                    return false;
                }
            }

            return true;
        }

        public static int FindMinimumParticipants()
        {
            // prevent first town in from getting 8 people in right away
            if (Participants.Count < 2)
                return MinMaxPerTeam;

            int min = 0;

            foreach (KeyValuePair<Town, int> entry in Participants)
            {
                if (entry.Value > 0 && entry.Value < min)
                {
                    min = entry.Value;
                }
            }
            min = Math.Max(MinMaxPerTeam, min + 1); // only allow the lowest team + 1 into wind

            return min;
        }

        public static void RegisterBrazier(WindBrazier wb)
        {
            if (!Braziers.Contains(wb))
                Braziers.Add(wb);
        }

        public static void RegisterFragment(WindFragment wf)
        {
            if (Fragment != null && !Fragment.Deleted && !(Fragment == wf))
            {
                Fragment.Delete();
            }

            Fragment = wf;
        }

        public static bool TryCapture(WindBrazier wb, Mobile m)
        {
            Town town = Town.Find(m);

            if (town == null)
            {
                m.SendMessage("You must be in a town militia to light this brazier.");
            }
            else if (!IsActive())
            {
                m.SendMessage("The Wind braziers are not active at this time.");
            }
            else if (wb.Captured)
            {
                m.SendMessage("This brazier is already captured.");
            }
            else
            {

                wb.CapTown = town;
                wb.Captured = true;
                Effects.PlaySound(wb.Location, wb.Map, 0x225);
                Broadcast(String.Format("The {0} brazier has been captured by {1}!", wb.BrazierLocationName.ToLower(), town.HomeFaction.Definition.FriendlyName));

                return true;
            }
            return false;
        }

        public static bool OnDouse(WindBrazier wb)
        {
            Broadcast(String.Format("The {0} brazier has been doused!", wb.BrazierLocationName.ToLower()));
            return true;
        }

        public static void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(0); //version

            Town.WriteReference(writer, Owner);

            bool toWrite = IsActive();
            writer.Write(toWrite);

            if (toWrite)
            {
                writer.Write(Scores.Count);
                foreach (KeyValuePair<Town, int> entry in Scores)
                {
                    Town.WriteReference(writer, entry.Key);
                    writer.Write(entry.Value);
                }
            }

        }

        public static void Deserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();
            switch (version)
            {
                case 0:
                    {
                        Owner = Town.ReadReference(reader);

                        if (reader.ReadBool())
                        {
                            int count = reader.ReadInt();
                            for (int i = 0; i < count; i++)
                            {
                                Town town = Town.ReadReference(reader);
                                int val = reader.ReadInt();
                                if (town != null)
                                    Scores.Add(town, val);
                            }
                        }
                    } break;
            }
        }

        public class WindBattlegroundPersistance : Item
        {
            public override string DefaultName { get { return "Wind Battleground Persistance"; } }

            public WindBattlegroundPersistance()
                : base(0x0)
            {
            }

            public WindBattlegroundPersistance(Serial serial)
                : base(serial)
            {
            }


            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);
                writer.Write((int)0); // version
                WindBattleground.Serialize(writer);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);
                int version = reader.ReadInt();

                WindBattleground.PersistanceItem = this;
                WindBattleground.Deserialize(reader);
            }
        }

        public class WindTrackArrow : QuestArrow
        {
            private Mobile m_From;
            private Timer m_Timer;

            public WindTrackArrow(Mobile from, Mobile target)
                : base(from, target)
            {
                m_From = from;
                m_Timer = new WindTrackTimer(from, this);
                m_Timer.Start();

                from.LocalOverheadMessage(MessageType.Emote, from.EmoteHue, true, "You begin to sense the Wind Fragment");

                WindBattleground.Arrows.Add(this);
            }

            public override void OnClick(bool rightClick)
            {
                if (rightClick)
                {
                    if (m_From != null)
                        m_From.SendMessage("The force of the Wind Fragment is too strong to ignore.");
                }
            }

            public override void OnStop()
            {
                WindBattleground.Arrows.Remove(this);
                m_Timer.Stop();

                if (m_From != null)
                {
                    m_From.LocalOverheadMessage(MessageType.Emote, m_From.EmoteHue, true, "You can no longer sense the Wind Fragment.");
                }
            }
        }

        private class WindTrackTimer : Timer
        {
            private Mobile m_From;
            private WindFragment m_Target;
            private int m_LastX, m_LastY;
            private WindTrackArrow m_Arrow;

            public WindTrackTimer(Mobile from, WindTrackArrow arrow)
                : base(TimeSpan.FromSeconds(0.25), TimeSpan.FromSeconds(2.5))
            {
                m_From = from;
                m_Target = WindBattleground.Fragment;

                m_Arrow = arrow;
            }

            protected override void OnTick()
            {
                if (!m_Arrow.Running)
                {
                    Stop();
                    return;
                }
                else if (m_From.NetState == null || m_From.Deleted || m_Target.Deleted || m_From.Map != m_Target.Map)
                {
                    m_Arrow.Stop();
                    Stop();
                    return;
                }

                Point3D location = WindBattleground.Fragment.GetWorldLocation();

                if (m_LastX != location.X || m_LastY != location.Y)
                {
                    m_LastX = location.X;
                    m_LastY = location.Y;

                    m_Arrow.Update();
                }
            }
        }
    }
}
