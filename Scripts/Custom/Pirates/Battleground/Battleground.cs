using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Guilds;
using Server.Multis;
using Server.Items;
using Server.Targeting;
using Server.Spells;
using Server.Commands;
using Server.Network;
using System.Collections;
using Server.Regions;

namespace Server.Custom.Pirates.Battleground
{
    public static class Battleground
    {
        public static bool Enabled = false; //Enable or Disable the Buccs Battleground

        #region Spawn Commands
        public static void Initialize()
        {
            if (Enabled)
            {
                CommandSystem.Register("SpawnBuccsBattleground", AccessLevel.GameMaster, new CommandEventHandler(SpawnBuccsBattleground_OnCommand));
                CommandSystem.Register("ClearBuccsBattleground", AccessLevel.GameMaster, new CommandEventHandler(ClearBuccsBattleground_OnCommand));

                if (m_Region == null)
                {
                    m_Region = new BuccsBayRegion("BuccsBayRegion", Map.Felucca, 55, m_Bay);
                    m_Region.Register();
                }
            }
        }

        [Usage("SpawnBuccsBattleground")]
        [Description("SpawnBuccsBattleground")]
        public static void SpawnBuccsBattleground_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            new BattlegroundPersistance();
            Reset(true, true);
            from.SendMessage("The battleground has been spawned.");
        }

        [Usage("ClearBuccsBattleground")]
        [Description("ClearBuccsBattleground")]
        public static void ClearBuccsBattleground_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            Reset(false, true);

            if (BattlegroundPersistance.Instance != null)
                BattlegroundPersistance.Instance.Delete();

            while(m_Cannoneers.Count > 0)
            {
                Mobile m = m_Cannoneers[0];
                m_Cannoneers.RemoveAt(0);
                m.Delete();
            }

            from.SendMessage("The battleground has been removed.");
        }
        #endregion
        
        public class BattlegroundControlInfo
        {
            public BaseGuild ControllingGuild { get; set; }
            public DateTime ControlStart { get; set; }
            public DateTime LastAward { get; set; }
            public DateTime LastSeen { get; set; }
            public DateTime LastCaptureDisplay { get; set; }
            public TimeSpan CaptureElapse { get; set; }
            public bool Captured { get; set; }
            public bool Contested { get; set; }

            public BattlegroundControlInfo(BaseGuild guild)
            {
                ControllingGuild = guild;
                ControlStart = LastSeen = DateTime.UtcNow;
                Captured = Contested = false;
                CaptureElapse = TimeSpan.Zero;
            }
        }

        private static readonly int DoubloonsToDispurse = 10;
        private static readonly TimeSpan TimerFrequency = TimeSpan.FromSeconds(10);
        private static readonly TimeSpan DisplayCaptureFrequency = TimeSpan.FromSeconds(10);
        private static readonly TimeSpan GuildAbandonTime = TimeSpan.FromHours(2);
        private static readonly TimeSpan DoubloonAwardTime = TimeSpan.FromMinutes(10);
        private static readonly TimeSpan CaptureTime = TimeSpan.FromMinutes(30);
        private static readonly Point3D[] CannoneerSpawnLocations = new Point3D[3] { new Point3D(2759, 2166, -2), new Point3D(2751, 2157, -2), new Point3D(2751, 2176, -2) };
        private static readonly Rectangle2D[] m_Bay = new Rectangle2D[] { new Rectangle2D(new Point2D(2750, 2150), new Point2D(2843, 2186)), new Rectangle2D(new Point2D(2760, 2186), new Point2D(2843, 2208)) };
        private static BuccsBayRegion m_Region;

        private static List<BuccsCannoneer> m_Cannoneers = new List<BuccsCannoneer> { };
        private static ControlTimer m_Timer;
        private static BattlegroundControlInfo m_Info;
        public static BattlegroundControlInfo ControlInfo { get { return m_Info; } set { m_Info = value; } }

        private static void SpawnCannoneers()
        {
            foreach (Point3D p in CannoneerSpawnLocations)
                if (!CannoneerExistsAt(p))
                {
                    BuccsCannoneer dg = new BuccsCannoneer();
                    dg.MoveToWorld(p, Map.Felucca);
                    m_Cannoneers.Add(dg);
                }
        }

        private static bool CannoneerExistsAt(Point3D p)
        {
            IPooledEnumerable eable = Map.Felucca.GetMobilesInRange(p, 0);

            foreach (Mobile m in eable)
            {
                if (m is DoubloonDockGuard)
                {
                    eable.Free();
                    return true;
                }
            }

            eable.Free();

            return false;
        }

        public static void RegisterDeath(BuccsCannoneer killed)
        {
            if (m_Cannoneers.Contains(killed))
            {
                m_Cannoneers.Remove(killed);

                if (m_Cannoneers.Count > 0)
                    return;

                StartTimer();
            }
            else
            {
                BattlegroundDefense.RegisterCannoneerDeath(killed);
            }
        }

        public static bool IsInBay(Map map, Point3D p)
        {
            if (map != Map.Felucca)
                return false;

            for (int i = 0; i < m_Bay.Length; i++)
            {
                Rectangle2D rect = m_Bay[i];
                if (rect.Contains(p))
                    return true;
            }

            return false;
        }

        public static bool CheckControl(Mobile from)
        {
            return m_Info != null && m_Info.Captured == true && from != null && from.Guild != null && from.Guild == m_Info.ControllingGuild;
        }

        public static void OnEnterBay(Mobile from)
        {
            if (m_Info == null)
            {
                from.SendMessage(72, "The Buccaneer's Den Bay is currently uncontrolled.");
                return;
            }
            
            if (!m_Info.Captured)
                from.SendMessage(72, "The Buccaneer's Den Bay is currently being captured by {0}!", m_Info.ControllingGuild.Name);
            else
                from.SendMessage(72, "The Buccaneer's Den Bay is currently controlled by {0}!", m_Info.ControllingGuild.Name);

            if (m_Info.Contested)
                from.SendMessage(72, "The Buccaneer's Den Bay is currently being being contested!");
        }

        private static List<Mobile> GetPlayersInBay()
        {
            return m_Region.GetPlayers();
        }

        private static List<BaseGuild> FindGuilds()
        {
            var mobs = GetPlayersInBay();
            var guilds = new List<BaseGuild> { };

            foreach (Mobile m in mobs)
                if (m.Alive && m.Guild != null && !guilds.Contains(m.Guild))
                    guilds.Add(m.Guild);

            return guilds;
        }

        private static List<BaseBoat> GetGuildBoatsInBay()
        {
            var boats = new List<BaseBoat> { };

            for (int i = 0; i < m_Bay.Length; i++)
            {
                IPooledEnumerable eable = Map.Felucca.GetItemsInBounds(m_Bay[i]);

                foreach (Item item in eable)
                    if (item is BaseBoat)
                    {
                        var boat = (BaseBoat)item;
                        if (!boat.Deleted && boat.Owner != null && boat.Owner.Guild != null && boat.Owner.Guild == m_Info.ControllingGuild && !boats.Contains(boat))
                            boats.Add(boat);
                    }

                eable.Free();
            }

            return boats;
        }

        private static void CheckAward()
        {
            if (m_Info.LastAward + DoubloonAwardTime < DateTime.UtcNow)
                AwardDoubloons();
        }

        private static void StartCapture(BaseGuild guild)
        {
            if (m_Info != null)
                Reset(false, false);

            m_Info = new BattlegroundControlInfo(guild);
        }

        private static void CheckCapture()
        {
            m_Info.CaptureElapse += TimerFrequency;

            if (m_Info.CaptureElapse > CaptureTime)
                FinishCapture();
            else
                CheckCaptureDisplay();
        }

        private static void CheckCaptureDisplay()
        {
            if (m_Info.LastCaptureDisplay + DisplayCaptureFrequency < DateTime.UtcNow)
            {
                TimeSpan toCap = CaptureTime - m_Info.CaptureElapse;
                int minutes = (int)toCap.TotalMinutes;
                int seconds = (int)(toCap.TotalSeconds - minutes * 60);

                string msg = String.Format("Bay Control Countdown: {0:00}:{1:00}", minutes, seconds);

                SendGuildDualMessage(msg);

                m_Info.LastCaptureDisplay = DateTime.UtcNow;
            }
        }

        private static void SendGuildOverheadMessage(string msg)
        {
            var guild = (Guild)m_Info.ControllingGuild;
            var members = guild.Members;
            int count = members.Count;

            for (int i = 0; i < count; ++i)
            {
                Mobile m = members[i];

                if (m.NetState != null)
                    m.PrivateOverheadMessage(Network.MessageType.Regular, 72, false, msg, m.NetState);
            }
        }

        private static void SendGuildDualMessage(string msg) //overhead for those in the bay, regular for those not
        {
            var guild = (Guild)m_Info.ControllingGuild;
            var members = guild.Members;
            int count = members.Count;
            //Packet p = null;

            for (int i = 0; i < count; ++i)
            {
                Mobile m = members[i];
                NetState state = m.NetState;

                if (state != null)
                {
                    if (IsInBay(m.Map, m.Location))
                    {
                        m.PrivateOverheadMessage(Network.MessageType.Regular, 72, false, msg, m.NetState);
                    }
                    else
                    {
                        //if (p == null)
                            //p = Packet.Acquire(new UnicodeMessage( Serial.MinusOne, -1, MessageType.Regular, 72, 3, "ENU", "System", msg ));

                        //state.Send(p);
                    }
                }                    
            }
        }

        private static void SendBayGuildMessage(string msg)
        {
            var players = m_Region.GetPlayers();
            Packet p = null;

            foreach (Mobile m in players)
            {
                NetState state = m.NetState;

                if (state != null && m.Guild != null)
                {
                    if (p == null)
                        p = Packet.Acquire(new UnicodeMessage(Serial.MinusOne, -1, MessageType.Regular, 72, 3, "ENU", "System", msg));

                    state.Send(p);
                }
            }
        }

        private static void SendBayNonControllingGuildMessage(string msg)
        {
            var players = m_Region.GetPlayers();
            //Packet p = null;

            foreach (Mobile m in players)
            {
                /*NetState state = m.NetState;

                if (state != null && m.Guild != null && (m_Info != null || m.Guild != m_Info.ControllingGuild))
                {
                    if (p == null)
                        p = Packet.Acquire(new UnicodeMessage(Serial.MinusOne, -1, MessageType.Regular, 72, 3, "ENU", "System", msg));

                    state.Send(p);
                }*/
                m.PrivateOverheadMessage(Network.MessageType.Regular, 72, false, msg, m.NetState);
            }
        }

        private static void StartTimer()
        {
            if (m_Info != null)
                Reset(false, true);

            m_Timer = new ControlTimer();
            m_Timer.Start();
        }

        private static void FinishCapture()
        {
            if (m_Info.ControllingGuild == null)
                Reset(true, true);

            m_Info.Captured = true;
            m_Info.LastAward = DateTime.UtcNow;

            SendGuildDualMessage("Buccaneer's bay has been secured! You will begin receiving doubloons in ten minutes.");

        }

        private static void AwardDoubloons()
        {
            m_Info.LastAward = DateTime.UtcNow;

            var boats = GetGuildBoatsInBay();

            if (boats.Count == 0)
                return;

            int toGive = DoubloonsToDispurse;
            BattlegroundDefense.DefenseOffset(ref toGive);

            var awards = new Dictionary<BaseBoat, int> { };

            while (toGive > 0)
            {
                var boat = boats[Utility.Random(boats.Count)];

                if (awards.ContainsKey(boat))
                    awards[boat]++;
                else
                    awards.Add(boat, 1);

                toGive--;
            }

            foreach (KeyValuePair<BaseBoat, int> entry in awards)
            {
                var boat = entry.Key;
                int amount = entry.Value;

                if (boat.Hold != null)
                {
                    var doubloon = boat.Hold.FindItemByType(typeof(Doubloon));

                    if (doubloon != null)
                        doubloon.Amount += amount;
                    else
                        boat.Hold.DropItem(new Doubloon(amount));
                    
                    boat.Hold.PublicOverheadMessage(Network.MessageType.Regular, 32, false, String.Format("+{0} doubloon{1}!", amount, amount > 1 ? "s" : ""));
                }
            }
        }

        private static void CheckAbandoned()
        {
            //If the guild has placed any defenses, it will not be abandoned until the cannoneers are gone
            if (BattlegroundDefense.HasDefense)
                return;

            //If it was abandoned while capturing, then start reducing the timer
            if (!m_Info.Captured)
            {
                if (m_Info.CaptureElapse < TimerFrequency)
                    Reset(true, true);
                else
                {
                    m_Info.CaptureElapse -= TimerFrequency;

                    if (m_Info.Captured)
                        m_Info.Captured = false;
                }
            }
            //else look for abandonment time
            else if (m_Info.LastSeen + GuildAbandonTime < DateTime.UtcNow)
                Reset(true, true);
        }

        private static void Contested(List<BaseGuild> guilds)
        {
            //If the guild has placed any defenses, it will not be contested until the cannoneers are gone
            if (BattlegroundDefense.HasDefense)
                return;

            if (!m_Info.Contested)
            {
                m_Info.Contested = true;
                SendGuildDualMessage("Buccaneer's Bay is now contested!");
                SendBayNonControllingGuildMessage("You are now contesting the bay!");
            }

            //Contested makes the timer frequency drop twice as fast as the capture
            if (m_Info.CaptureElapse < (TimerFrequency + TimerFrequency))
                Reset(false, false);
            else
            {
                m_Info.CaptureElapse -= (TimerFrequency + TimerFrequency);

                if (m_Info.Captured)
                    m_Info.Captured = false;
            }
        }

        private static void EndContested()
        {
            m_Info.Contested = false;
        }

        private static void CheckResetNoAction()
        {
            if (m_Timer.LastAction + GuildAbandonTime < DateTime.UtcNow)
                Reset(true, true);
        }

        private static void Reset(bool spawn, bool killTimer)
        {
            if (killTimer && m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }

            if (spawn)
                SpawnCannoneers();

            Timer.DelayCall(TimeSpan.FromTicks(1), BattlegroundDefense.Reset);

            m_Info = null;
        }

        public static void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(0);

            writer.Write(m_Cannoneers.Count);
            foreach (BuccsCannoneer bc in m_Cannoneers)
                writer.Write(bc);

            bool infoWrite = m_Info != null;

            writer.Write(infoWrite);

            if (infoWrite)
            {
                writer.Write(m_Info.ControllingGuild);
                writer.Write(m_Info.ControlStart);
                writer.Write(m_Info.LastAward);
                writer.Write(m_Info.LastSeen);
                writer.Write(m_Info.LastCaptureDisplay);
                writer.Write(m_Info.CaptureElapse);
                writer.Write(m_Info.Captured);
                writer.Write(m_Info.Contested);
            }

            writer.Write(m_Timer != null && m_Timer.Running);
        }

        public static void Deserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                var bc = (BuccsCannoneer)reader.ReadMobile();
                if (bc != null)
                    m_Cannoneers.Add(bc);
            }

            bool infoRead = reader.ReadBool();

            if (infoRead)
            {
                BaseGuild guild = reader.ReadGuild();
                DateTime controlStart = reader.ReadDateTime();
                DateTime lastAward = reader.ReadDateTime();
                DateTime lastSeen = reader.ReadDateTime();
                DateTime lastDisplay = reader.ReadDateTime();
                TimeSpan elapse = reader.ReadTimeSpan();
                bool captured = reader.ReadBool();
                bool contested = reader.ReadBool();

                if (guild != null)
                {
                    m_Info = new BattlegroundControlInfo(guild);
                    m_Info.ControlStart = controlStart;
                    m_Info.LastAward = lastAward;
                    m_Info.LastSeen = lastSeen;
                    m_Info.LastCaptureDisplay = lastDisplay;
                    m_Info.CaptureElapse = elapse;
                    m_Info.Captured = captured;
                    m_Info.Contested = contested;
                }
            }

            //Should the timer be running?
            if (reader.ReadBool())
            {
                m_Timer = new ControlTimer();
                m_Timer.Start();
            }
        }

        private class ControlTimer : Timer
        {
            public DateTime LastAction;

            public ControlTimer()
                : base(TimeSpan.Zero, TimerFrequency)
            {
                Priority = TimerPriority.OneSecond;
                LastAction = DateTime.UtcNow;
            }

            protected override void OnTick()
            {
                var guilds = FindGuilds();

                if (m_Info == null) //fighting over it, or cannoneers present
                {
                    //If there is only one guild, start capturing, else check for reset due to no action.
                    if (guilds.Count == 1)
                        StartCapture(guilds[0]);
                    else
                        CheckResetNoAction();

                    //return so the LastAction does not get updated
                    return;
                }
                else if (guilds.Count == 1 && guilds[0] == m_Info.ControllingGuild)
                {
                    //Present and alone
                    m_Info.LastSeen = DateTime.UtcNow;

                    //Was it previously contested?
                    if (m_Info.Contested)
                        EndContested();

                    //Has it been captured yet?
                    if (!m_Info.Captured)
                    {
                        CheckCapture();
                        return;
                    }

                    //Check for Doubloon Dispursement
                    CheckAward();

                    return;
                }
                else if (guilds.Count == 0)
                {
                    //Abandoned
                    CheckAbandoned();
                    return;
                }
                else
                {
                    //Contested
                    Contested(guilds);
                }

                LastAction = DateTime.UtcNow;
            }
        }
    }

    public static class BattlegroundDefense
    {
        public class BattlegroundDefenseInfo
        {
            public List<DepthCharge> Charges { get; set; }
            public List<BuccsCannoneer> Cannoneers { get; set; }

            public BattlegroundDefenseInfo()
            {
                Charges = new List<DepthCharge> { };
                Cannoneers = new List<BuccsCannoneer> { };
            }
        }

        public static readonly int MaximumDepthCharges = 10;
        public static readonly int MaximumHiredCannoneers = 5;
        public static readonly int DepthChargeCost = 25;
        public static readonly int CannoneerPurchaseCost = 50;
        public static readonly int CannoneerRadius = 2;
        private static BattlegroundDefenseInfo m_Info =  new BattlegroundDefenseInfo();
        public static BattlegroundDefenseInfo Info { get { return m_Info; } set { m_Info = value; } }
        public static bool HasDefense { get { return m_Info != null && (m_Info.Cannoneers.Count > 0); } }
        public static int DepthChargeCount { get { return m_Info == null? 0 : m_Info.Charges.Count; } }
        public static int CannoneerCount { get { return m_Info == null? 0 : m_Info.Cannoneers.Count; } }

        public static void Reset()
        {
            while (m_Info.Charges.Count > 0) {
                var charge = m_Info.Charges[0];
                m_Info.Charges.RemoveAt(0);
                charge.Delete();
            }

            while (m_Info.Cannoneers.Count > 0)
            {
                var cannoneer = m_Info.Cannoneers[0];
                m_Info.Cannoneers.RemoveAt(0);
                cannoneer.Delete();
            }
        }

        public static void PurchaseMenuRequest(Mobile from, Mobile vendor)
        {
            if (!Battleground.CheckControl(from))
            {
                vendor.Say("Your guild must control Buccaneer's Bay in order to purchase defenses.");
            }
            else
            {
                if (from.HasGump(typeof(DefenseGump)))
                    from.CloseGump(typeof(DefenseGump));

                from.SendGump(new DefenseGump());
            }
        }

        public static void DefenseOffset(ref int offset)
        {
            if (m_Info == null)
                return;

            offset -= m_Info.Cannoneers.Count * 2; //-2 doubloon offset per cannoneer
        }

        public static void PurchaseDepthChargeRequest(Mobile from)
        {
            if (!Battleground.CheckControl(from))
            {
                from.SendMessage(72, "Your guild must control Buccaneer's Bay in order to place a depth charge.");
            }
            else if (GetAvailableDoubloons(from) < DepthChargeCost)
            {
                from.SendMessage(72, "You must be in a boat with doubloons to purchase a depth charge.");
                from.SendGump(new DefenseGump());
            }
            else if (DepthChargeCount >= MaximumDepthCharges)
            {
                from.SendMessage(72, "The bay is fully mined!");
                from.SendGump(new DefenseGump());
            }
            else
            {
                from.SendMessage(72, "Where would you like to sink a depth charge?");
                from.Target = new DepthChargeTarget();
            }
        }

        public static void DepthChargePlacement(Mobile from, Point3D p)
        {
            if (from.Map != Map.Felucca)
                return;

            BaseBoat b = BaseBoat.FindBoatAt(p, from.Map);

            if (!Battleground.CheckControl(from))
            {
                from.SendMessage(72, "Your guild must control Buccaneer's Bay in order to place a depth charge.");
            }
            else if (GetAvailableDoubloons(from) < DepthChargeCost)
            {
                from.SendMessage(72, "You must be in boat owned by a member of your guild with at least {0} doubloons to purchase a depth charge.", DepthChargeCost);
                from.SendGump(new DefenseGump());
            }
            else if (DepthChargeCount >= MaximumDepthCharges)
            {
                from.SendMessage(72, "The bay is fully mined!");
                from.SendGump(new DefenseGump());
            }
            else if (!Battleground.IsInBay(from.Map, p))
            {
                from.SendMessage(72, "Depth charges must be placed in the Buccaneer's Den Bay.");
                from.SendGump(new DefenseGump());
            }
            else if (b != null && b.IsMoving)
            {
                from.SendMessage(72, "You may not place depth charges while the boat is moving.");
                from.SendGump(new DefenseGump());
            }
            else
            {

                IPooledEnumerable eable = from.Map.GetItemsInRange(p, 5);
                foreach (Item i in eable)
                {
                    if (i is DepthCharge)
                    {
                        from.SendMessage("You may not place depth charges so close together.");
                        from.SendGump(new DefenseGump());
                        eable.Free();
                        return;
                    }
                }

                eable.Free();

                if (m_Info == null)
                    m_Info = new BattlegroundDefenseInfo();

                from.RevealingAction();
                Effects.SendLocationEffect(p, from.Map, 0x352D, 16, 4);
                Effects.PlaySound(p, from.Map, 0x364);

                ConsumeDoubloons(from, DepthChargeCost);
                var depthCharge = new DepthCharge();
                depthCharge.ActiveGuild = Battleground.ControlInfo.ControllingGuild;
                depthCharge.MoveToWorld(new Point3D(p, -6), Map.Felucca);
                m_Info.Charges.Add(depthCharge);

                ((Guild)from.Guild).GuildTextMessage("{0} has placed a depth charge in Buccaneer's Bay!", from.RawName);
                from.SendGump(new DefenseGump());
            }
            
        }

        public static void OnDepthChargeExplode(DepthCharge dc)
        {
            if (m_Info != null && m_Info.Charges.Contains(dc))
                m_Info.Charges.Remove(dc);
        }

        public static void RegisterCannoneerDeath(BuccsCannoneer c)
        {
            if (m_Info != null && m_Info.Cannoneers.Contains(c))
                m_Info.Cannoneers.Remove(c);
        }

        public static bool CheckWaterTarget(object targeted, Mobile from, out Point3D loc)
        {
            int tileID;
            Map map;

            if (targeted is Static && !((Static)targeted).Movable)
            {
                Static obj = (Static)targeted;

                tileID = (obj.ItemID & 0x3FFF) | 0x4000;
                map = obj.Map;
                loc = obj.GetWorldLocation();
            }
            else if (targeted is StaticTarget)
            {
                StaticTarget obj = (StaticTarget)targeted;

                tileID = (obj.ItemID & 0x3FFF) | 0x4000;
                map = from.Map;
                loc = obj.Location;
            }
            else if (targeted is LandTarget)
            {
                LandTarget obj = (LandTarget)targeted;

                tileID = obj.TileID & 0x3FFF;
                map = from.Map;
                loc = obj.Location;
            }
            else
            {
                tileID = 0;
                map = null;
                loc = Point3D.Zero;
                return false;
            }

            return (map != null && map != Map.Internal) && Validate(tileID);
        }

        public static bool ConsumeDoubloons(Mobile from, int amount)
        {
            var boat = BaseBoat.FindBoatAt(from.Location, from.Map);

            return boat.Hold.ConsumeTotal(typeof(Doubloon), amount);
        }

        public static void PurchaseCannoneerRequest(Mobile from)
        {
            if (!Battleground.CheckControl(from))
            {
                from.SendMessage(72, "Your guild must control Buccaneer's Bay in order to place a cannoneer.");
            }
            else if (GetAvailableDoubloons(from) < CannoneerPurchaseCost)
            {
                from.SendMessage(72, "You must be in a guild boat with doubloons to purchase a cannoneer.");
                from.SendGump(new DefenseGump());
            }
            else if (CannoneerCount >= MaximumHiredCannoneers)
            {
                from.SendMessage(72, "The bay is fully protected!");
                from.SendGump(new DefenseGump());
            }
            else
            {
                from.SendMessage(72, "Where would you like to hire a cannoneer?");
                from.Target = new CannoneerTarget();
            }
        }

        public static void CannoneerPlacement(Mobile from, Point3D p)
        {
            if (from == null || from.Map == null || from.Map == Map.Internal)
                return;

            int count = CannoneerRadius*2;
            for (int x = -1 * CannoneerRadius; x < count; x++)
            {
                for (int y = -1 * CannoneerRadius; y < count; y++)
                {
                    LandTile landTile = from.Map.Tiles.GetLandTile(p.X, p.Y);
                    if (Array.IndexOf(m_WaterTiles, landTile.ID) != -1 || !from.Map.CanFit(p, 10))
                    {
                        from.SendMessage(72, "There is not enough room for a cannoneer at that location.");
                        from.SendGump(new DefenseGump());
                        return;
                    }
                }
            }

            var newCannon = new BuccsCannoneer();

            if (Battleground.ControlInfo != null)
                newCannon.Guild = Battleground.ControlInfo.ControllingGuild;

            newCannon.MoveToWorld(p, from.Map);
            m_Info.Cannoneers.Add(newCannon);
            ((Guild)from.Guild).GuildTextMessage("{0} has placed a cannoneer in Buccaneer's Bay!", from.RawName);

            from.SendGump(new DefenseGump());
        }

        public static bool Validate(int tileID)
        {
            bool contains = false;

            for (int i = 0; !contains && i < m_WaterTiles.Length; i += 2)
                contains = (tileID >= m_WaterTiles[i] && tileID <= m_WaterTiles[i + 1]);

            return contains;
        }

        public static int GetAvailableDoubloons(Mobile from)
        {
            if (from == null || from.Map == null || from.Guild == null || from.Map == Map.Internal)
                return 0;

            var boat = BaseBoat.FindBoatAt(from.Location, from.Map);
            
            if (boat == null || boat.Hold == null || boat.Owner == null || boat.Owner.Guild == null || boat.Owner.Guild != from.Guild )
                return 0;

            Item[] doubloons = boat.Hold.FindItemsByType(typeof(Doubloon));

            int amount = 0;

            for (int i = 0; i < doubloons.Length; i++)
                amount += doubloons[i].Amount;

            return amount;
        }

        public static void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(0);

            writer.Write(m_Info.Cannoneers.Count);
            foreach (BuccsCannoneer bc in m_Info.Cannoneers)
                writer.Write(bc);

            writer.Write(m_Info.Charges.Count);
            foreach (DepthCharge dc in m_Info.Charges)
                writer.Write(dc);
        }

        public static void Deserialize(GenericReader reader)
        {
            reader.ReadEncodedInt();

            int cCount = reader.ReadInt();
            for (int i = 0; i < cCount; i++)
            {
                BuccsCannoneer bc = (BuccsCannoneer)reader.ReadMobile();
                if (bc != null)
                    m_Info.Cannoneers.Add(bc);
            }

            int dcCount = reader.ReadInt();
            for (int i = 0; i < dcCount; i++)
            {
                DepthCharge dc = (DepthCharge)reader.ReadItem();
                if (dc != null)
                    m_Info.Charges.Add(dc);
            }
        }

        private class DepthChargeTarget : Target
        {
            public DepthChargeTarget()
                : base(3, true, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                Point3D p;

                if (CheckWaterTarget(targeted, from, out p))
                    DepthChargePlacement(from, p);
                else
                    from.SendMessage("Invalid water target.");
            }
        }

        private class CannoneerTarget : Target
        {
            public CannoneerTarget()
                : base(12, true, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                IPoint3D ip = targeted as IPoint3D;

                if (ip == null)
                    return;

                Map map = from.Map;

                if (map == null)
                    return;

                SpellHelper.GetSurfaceTop(ref ip);

                Point3D p = new Point3D(ip.X, ip.Y, -2);
                CannoneerPlacement(from, p);
            }
        }

        private static int[] m_WaterTiles = new int[]
			{
				0x00A8, 0x00AB,
				0x0136, 0x0137,
				0x5797, 0x579C,
				0x746E, 0x7485,
				0x7490, 0x74AB,
				0x74B5, 0x75D5
			};

    }

    public class BattlegroundPersistance : Item
    {
        private static BattlegroundPersistance m_Instance;

        public static BattlegroundPersistance Instance { get { return m_Instance; } }

        public override string DefaultName
        {
            get { return "Battleground Persistance - Internal"; }
        }

        public BattlegroundPersistance()
            : base(1)
        {
            Movable = false;

            if (m_Instance == null || m_Instance.Deleted)
                m_Instance = this;
            else
                base.Delete();
        }

        public BattlegroundPersistance(Serial serial)
            : base(serial)
		{
			m_Instance = this;
		}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0);

            Battleground.Serialize(writer);
            BattlegroundDefense.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();

            Battleground.Deserialize(reader);
            BattlegroundDefense.Deserialize(reader);
        }
    }
}