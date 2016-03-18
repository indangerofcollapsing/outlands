using Server;
using System;
using Server.Items;
using Server.Mobiles;
using System.Collections.Generic;
using System.Collections;
using Server.Custom.Townsystem;

namespace Server.Engines.Quests.Paladin
{
    public static class PaladinSiege
    {
        public enum QuestState
        {
            None,
            PreSiege,
            FirstWave,
            SecondWave,
            ThirdWave,
            FourthWave,
            FifthWave,
            SixthWave
        }

        //private static readonly Point3D[] _deceitLocations = new Point3D[] { new Point3D(5139, 663, 0), new Point3D(5157, 663, 0), new Point3D(5181, 724, 0), new Point3D(5151, 743, 0), new Point3D(5199, 655, 0), new Point3D(5211, 744, -20), new Point3D(5307, 668, 0), new Point3D(5266, 690, 0), new Point3D(5315, 749, -20) };
        //private static readonly Point3D[] _hythlothLocations = new Point3D[] { new Point3D(6122, 218, 22), new Point3D(6122, 159, 0), new Point3D(6053, 157, 0), new Point3D(6028, 197, 22), new Point3D(6049, 227, 44), new Point3D(6083, 90, 22), new Point3D(6111, 84, 0), new Point3D(6104, 44, 22), new Point3D(6049, 45, 0) };
        private static readonly Point3D[] _gateLocations = new Point3D[] { new Point3D(1767, 2778, 0), new Point3D(1769, 2796, 0), new Point3D(1765, 2796, 0), new Point3D(1764, 2822, 0) };
        private static readonly Point3D[] _gateLocationHomes = new Point3D[] { new Point3D(1790, 2759, 0), new Point3D(1830, 2774, 0), new Point3D(2028, 2718, 25), new Point3D(1794, 2798, 0) };
        private static readonly Point3D[] _civilianLocations = new Point3D[] { new Point3D(1857, 2777, 0), new Point3D(1893, 2777, 7), new Point3D(1936, 2776, 10), new Point3D(1923, 2777, 0), new Point3D(1968, 2778, 20) };
        private static readonly Point3D[] _pathLocations = new Point3D[] { new Point3D(1828, 2779, 20), new Point3D(2004, 2779, 20), new Point3D(2012, 2732, 20), new Point3D(2028, 2718, 25) };
        private static readonly Point3D _daemonSpawnLocation = new Point3D(1931,2787,20);
        private static readonly Point3D _daemon2SpawnLocation = new Point3D(2004,2780,20);
        private static readonly TimeSpan _artifactRecaptureTime = TimeSpan.FromSeconds(30); //change this
        private static readonly TimeSpan _siegeStateLength = TimeSpan.FromMinutes(3);
        //private static readonly TimeSpan _questItemRetreivalTime = TimeSpan.FromHours(3);
        private static readonly TimeSpan _finalStageLength = TimeSpan.FromMinutes(45);
        //private static readonly string[] text1 = new string[] { "Greetings, {0}", "Thine interest in joining the ranks of the Paladins is flattering,", "as tales of thy deeds hath certainly been told within the walls of Trinsic.", "The Order of the Shining Serpent hath existed since the dark ages of this world,", "and joining our ranks is no small feat.", "If your bravery you intend to prove,", "seek the great silver weapon of Paladins long past from the depths of Deceit.", "Return to us this stolen artifact and the Paladins shall surely take note of thy heroism..." };
        //private static readonly string[] text2 = new string[] { "Welcome back, {0}", "Your bravery is truly a beacon of light in these treacherous times.", "Return to us the stolen Paladin Shield from the depths of Hythloth,", "and you shall be welcome in our ranks.", "Make haste, for surely the sword's disappearance will not go unnoticed." };
        //private static readonly string[] text3 = new string[] { "{0}! Glory be to the Paladins!", "Thou hast returned the holy artifacts, and have fulfilled thy promises to the Order.", "Now, finally I can dub thee a...", "Wait!", "Evil Stirs!", "The removal of these holy artifacts from the clutches of these monsters has captured their attention.", "Quickly! Defend the city and its citizens!", "Do not let the hand of evil grasp these artifacts once again!" };
        
        private static List<WayPoint> _waypoints = new List<WayPoint>();
        private static WayPoint _artifactWayPoint;
        private static WayPoint _entranceWayPoint;
        private static WayPoint _exitWayPoint;
        private static Mobile _QuestGuard;
        private static QuestState _questState;
        public static Mobile CurrentPlayer;
        private static DateTime _lastStateChange;
        private static int _civilianDeaths;
        private static InternalTimer _timer;
        //private static Item _questItem;
        //private static bool _artifactsPlaced;
        private static List<Mobile> SpawnedMobiles = new List<Mobile>();
        private static List<Mobile> ArtifactMobiles = new List<Mobile>();
        private static List<Item> SpawnedArtifacts = new List<Item>();

        public static bool Saved = false;

        public static void Serialize(GenericWriter writer)
        {
            Saved = true;
            Timer.DelayCall(TimeSpan.FromTicks(1), delegate { Saved = false; });

            writer.WriteEncodedInt(0);

            writer.Write(_waypoints.Count);
            foreach(WayPoint wp in _waypoints)
                writer.Write(wp);

            writer.WriteItem(_artifactWayPoint);
            writer.WriteItem(_entranceWayPoint);
            writer.WriteItem(_exitWayPoint);
            writer.WriteMobile(_QuestGuard);
            writer.Write((int)_questState);
            writer.WriteMobile(CurrentPlayer);
            writer.Write(_lastStateChange);
            writer.Write(_civilianDeaths);
            //writer.WriteItem(_questItem);
            //writer.Write(_artifactsPlaced);

            writer.Write(SpawnedArtifacts.Count);
            foreach (Item i in SpawnedArtifacts)
                writer.WriteItem(i);

            writer.Write(SpawnedMobiles.Count);
            foreach (Mobile m in SpawnedMobiles)
                writer.Write(m);

            writer.Write(ArtifactMobiles.Count);
            foreach (Mobile m in ArtifactMobiles)
                writer.Write(m);
        }

        public static void Deserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            switch (version)
            {
                case 0:
                    {
                        int count = reader.ReadInt();
                        for (int i = 0; i < count; i++)
                            _waypoints.Add((WayPoint)reader.ReadItem());

                        _artifactWayPoint = (WayPoint)reader.ReadItem();
                        _entranceWayPoint = (WayPoint)reader.ReadItem();
                        _exitWayPoint = (WayPoint)reader.ReadItem();
                        _QuestGuard = reader.ReadMobile();
                        _questState = (QuestState)reader.ReadInt();
                        CurrentPlayer = reader.ReadMobile();
                        _lastStateChange = reader.ReadDateTime();
                        _civilianDeaths = reader.ReadInt();
                        //_questItem = reader.ReadItem();
                        //_artifactsPlaced = reader.ReadBool();

                        int count2 = reader.ReadInt();
                        for (int i = 0; i < count2; i++)
                            SpawnedArtifacts.Add(reader.ReadItem());

                        int count3 = reader.ReadInt();
                        for (int i = 0; i < count3; i++)
                            SpawnedMobiles.Add(reader.ReadMobile());

                        int count4 = reader.ReadInt();
                        for (int i = 0; i < count4; i++)
                            ArtifactMobiles.Add(reader.ReadMobile());

                        break;
                    }
            }


            if (CurrentState > QuestState.None)
            {
                _timer = new InternalTimer();
                _timer.Start();
            }
        }

        public static QuestState CurrentState
        {
            get { return _questState; }
            set
            {
                _questState = value;
                _lastStateChange = DateTime.UtcNow;
            }
        }

        public static string CheckReqs(Mobile from)
        {
            if (from == null)
                return "";

            string s = "";
            PlayerMobile pm = from as PlayerMobile;

            if (pm.Paladin == true)
            {
                s = "You already serve The Order of the Shining Serpent.";
                return s;
            }

            Accounting.Account acc = from.Account as Accounting.Account;
            foreach (Mobile m in acc.accountMobiles)
                if (m != null && m.ShortTermMurders > 0)
                    return "Murderers are not welcome amongst our ranks.";
            
            if (pm.PaladinRejoinAllowed > DateTime.UtcNow)
                s = "Your unlawful conduct has not been forgotten. You are not welcome here yet.";

            else if (pm.NpcGuild == NpcGuild.DetectivesGuild)
                s = "Thou must resign from thy Detective's duty first.";

            else if (pm.NpcGuild == NpcGuild.ThievesGuild)
                s = "Thieves are not welcome amongst our ranks.";

            else if (Faction.Find(pm) != null)
                s = "Militia members are not welcome amongst our ranks.";
            
            return s;
        }

        /*public static void OnPaladinKeywordJoin(Mobile from, PaladinOrderGuard guard)
        {
            String s = CheckReqs(from);
            if (s.Length > 0)
            {
                guard.Say(s);
            }
            else
            {
                SpeakArray(guard, from, text1);
                CurrentState = QuestState.Deceit;
                CurrentPlayer = from;
                GenerateDeceitItem(from);
                _timer = new InternalTimer();
                _timer.Start();
            }
        }*/

        /*public static void SpeakArray(Mobile toSpeak, Mobile from, string[] array)
        {
            int j = 0;
            toSpeak.Say(array[j++], from.Name);
            TimeSpan t = TimeSpan.Zero;
            for (int i = 1; i < array.Length-1; i++)
            {
                t += Time(array[i - 1]);
                Timer.DelayCall(t, delegate { toSpeak.Say(array[j++]); });
            }
        }*/

        /*private static TimeSpan Time(string text)
        {
            return TimeSpan.FromSeconds(2 + .075 * text.Length);
        }*/

        /*public static bool OnDragDrop(Mobile guard, Mobile from, Item dropped)
        {
            if ((CurrentState != QuestState.Deceit && CurrentState != QuestState.Hythloth) || CurrentPlayer != from)
                return false;

            if (dropped is PaladinQuestItem)
            {
                if (_questItem != dropped)
                {
                    dropped.Delete();
                    return true;
                }

                if (CurrentState == QuestState.Deceit)
                {
                    CurrentState = QuestState.Hythloth;
                    GenerateHythlothItem(from);
                    SpeakArray(guard, from, text2);
                    dropped.Delete();
                    return true;
                }
                else if (CurrentState == QuestState.Hythloth)
                {
                    ((BaseVendor)guard).CurrentSpeed = 0.2;
                    Timer.DelayCall(TimeSpan.FromSeconds(30), delegate { ((BaseVendor)guard).CurrentSpeed = 2; });

                    CurrentState = QuestState.PreSiege;
                    dropped.Delete();
                    _QuestGuard = guard;g
                    BeginSiege();
                    var bc = (BaseCreature)guard;
                    _artifactWayPoint = new WayPoint();
                    _artifactWayPoint.MoveToWorld(_pathLocations[_pathLocations.Length - 1], Map.Felucca);

                    _exitWayPoint = new WayPoint();
                    _exitWayPoint.MoveToWorld(new Point3D(2019, 2719, 24), Map.Felucca);

                    _entranceWayPoint = new WayPoint();
                    _entranceWayPoint.MoveToWorld(new Point3D(2019, 2719, 24), Map.Felucca);
                    
                    _entranceWayPoint.NextPoint = _artifactWayPoint;
                    _artifactWayPoint.NextPoint = _exitWayPoint;

                    bc.CurrentWayPoint = _entranceWayPoint;

                    //SpeakArray(_QuestGuard, CurrentPlayer, text3);
                    return true;
                }
            }

            return false;
        }*/

        public static void OnPaladinKeywordQuit(Mobile from, PaladinOrderGuard guard)
        {
            PlayerMobile pm = (PlayerMobile)from;

            if (!pm.Paladin)
            {
                guard.SayTo(from, @"Thou dost not serve us!");
            }

            else
            {
                guard.SayTo(from, @"I accept thy resignation from The Order of the Shining Serpent."); // I accept thy resignation.
                PaladinEvents.RemovePaladinStatus(from, 3);
            }
        }

        public static void PlaceArtifacts()
        {
            Item i = new Item(5049);
            i.Movable = false;
            i.MoveToWorld(new Point3D(2028, 2717, 25),Map.Felucca);
            i.Hue = 2101;
            i.Name = "Paladin Silver Sword";
            SpawnedArtifacts.Add(i);

            Item i2 = new Item(7108);
            i2.Name = "Paladin Shield";
            i2.Hue = 150;
            i2.Movable = false;
            i2.MoveToWorld(new Point3D(2029, 2717, 25),Map.Felucca);
            SpawnedArtifacts.Add(i2);
        }

        /*private static Point3D GenerateDeceitItem(Mobile player)
        {
            if (_questItem != null && !_questItem.Deleted)
                _questItem.Delete();

            Point3D loc = _deceitLocations[Utility.Random(_deceitLocations.Length - 1)];
            _questItem = new PaladinQuestItem("Paladin Silver Sword", 5049, 2101);
            _questItem.MoveToWorld(loc, Map.Felucca);

            return loc;
        }

        private static Point3D GenerateHythlothItem(Mobile player)
        {
            if (_questItem != null && !_questItem.Deleted)
                _questItem.Delete();

            Point3D loc = _hythlothLocations[Utility.Random(_hythlothLocations.Length - 1)];
            _questItem = new PaladinQuestItem("Paladin Shield", 7108, 150);
            _questItem.MoveToWorld(loc, Map.Felucca);
            return loc;
        }*/

        public static bool CanBeginSiege()
        {
            return (_timer == null);
        }

        public static void BeginSiege(Mobile from)
        {
            if (_timer != null)
            {
                Console.WriteLine("WARNING: Paladin Quest BeginSiege method called at bad state.");
                return;
            }

            CurrentState = QuestState.PreSiege;
            CurrentPlayer = from;
            _timer = new InternalTimer();
            _timer.Start();

            _lastStateChange = DateTime.UtcNow - TimeSpan.FromMinutes(2.5);

            PlaceArtifacts();
            GenerateBlackGates();
            GenerateCivilians();
            GenerateWayPoints();
        }

        public static void EndQuest(bool success)
        {
            CurrentState = QuestState.None;

            //if (_questItem != null && !_questItem.Deleted)
            //    _questItem.Delete();

            while (SpawnedMobiles.Count > 0)
            {
                Mobile m = SpawnedMobiles[0];
                SpawnedMobiles.RemoveAt(0);

                if (m != null)
                    m.Delete();
            }

            while (SpawnedArtifacts.Count > 0)
            {
                Item i = SpawnedArtifacts[0];
                SpawnedArtifacts.RemoveAt(0);

                if (i != null)
                    i.Delete();
            }

            while (_waypoints.Count > 0)
            {
                WayPoint wp = _waypoints[0];
                _waypoints.RemoveAt(0);

                if (wp != null)
                    wp.Delete();
            }

            if (_artifactWayPoint != null)
                _artifactWayPoint.Delete();

            if (_entranceWayPoint != null)
                _entranceWayPoint.Delete();

            if (_exitWayPoint != null)
                _exitWayPoint.Delete();

            //_artifactsPlaced = false;

            _civilianDeaths = 0;

            if (_timer != null)
            {
                _timer.Stop();
                _timer = null;
            }

            if (CurrentPlayer == null)
                return;
            
            QuestSystem qs = ((PlayerMobile)CurrentPlayer).Quest;

            if (qs != null && qs is PaladinInitiationQuest)
            {
                if (success)
                {
                    qs.AddConversation(new SuccessMessage());
                    qs.Complete();
                }
                else
                {
                    qs.AddConversation(new FailMessage());
                    qs.Cancel();
                }
            }

            if (!success)
            {
                CurrentPlayer = null;
                return;
            }

            String s = CheckReqs(CurrentPlayer);

            if (s.Length > 0)
                CurrentPlayer.SendMessage(s);
            else
            {
                ((PlayerMobile)CurrentPlayer).Paladin = true;
                CurrentPlayer.FixedEffect(0x373A, 10, 30);
                CurrentPlayer.PlaySound(0x209);
            }

            CurrentPlayer = null;
        }

        private static void GenerateBlackGates()
        {
            for (int i = 0; i < _gateLocations.Length; i++)
            {
                Item gate = new Item(3948);
                gate.Movable = false;
                gate.Hue = 1175;
                gate.MoveToWorld(_gateLocations[i], Map.Felucca);
                SpawnedArtifacts.Add(gate);
            }
        }

        private static void GenerateMobs(Type type, int no = 1, bool onlygate3 = false)
        {
            if (!typeof(BaseCreature).IsAssignableFrom(type))
            {
                Console.WriteLine("WARNING: PaladinQuest GenerateMobs requesting invalid type.");
                return;
            }

            for (int n = 0; n < no; n++)
            {
                for (int i = 0; i < _gateLocations.Length; i++)
                {
                    if (onlygate3 && i != 2)
                        continue;

                    var skele = (BaseCreature)Activator.CreateInstance(type);
                    skele.MoveToWorld(_gateLocations[i], Map.Felucca);
                    skele.RangeHome = 2;

                    SpawnedMobiles.Add(skele);

                    if (i == 2)
                    {
                        skele.Home = _gateLocationHomes[2];
                        ArtifactMobiles.Add(skele);
                        skele.CurrentWaypoint = _waypoints.Count > 0 ? _waypoints[0] : null;
                    }
                    else
                        skele.Home = new Point3D(_gateLocationHomes[i].X, _gateLocationHomes[i].Y + Utility.RandomMinMax(-5, 5), 0);

                    skele.AIObject.Activate();
                }
            }
        }

        private static void GenerateWayPoints()
        {
            foreach (Point3D p in _pathLocations)
            {
                WayPoint wp = new WayPoint();
                wp.MoveToWorld(p, Map.Felucca);
                _waypoints.Add(wp);
            }

            int c = _waypoints.Count - 1;
            for (int i = 0; i < c; i++)
                _waypoints[i].NextWaypoint = _waypoints[i + 1];
        }

        public static void RegisterDaemonDeath(BaseCreature daemon)
        {
                EndQuest(true);
        }

        public static void RegisterCivilianDeath()
        {
            if (++_civilianDeaths > 1)
            {
                EndQuest(false);

                if (CurrentPlayer != null)
                    CurrentPlayer.SendMessage(72, "You have let too many civilians die, and have failed your quest to become a Paladin.");
            }
        }

        private static void GenerateCivilians()
        {
            for (int i = 0; i < _civilianLocations.Length; i++)
            {
                var civ = new PaladinQuestCivilian();
                civ.MoveToWorld(_civilianLocations[i], Map.Felucca);
                civ.Home = _civilianLocations[i];
                civ.RangeHome = 1;
                SpawnedMobiles.Add(civ);
            }
        }

        private static void GenerateDaemons()
        {
            if (CurrentPlayer != null)
                CurrentPlayer.SendMessage(72, "A daemon crashes into the town, attacking the civilians and attempting to take back the artifacts!");
                        
            var d1 = new PaladinQuestDaemon();
            d1.MoveToWorld(_daemonSpawnLocation, Map.Felucca);
            SpawnedMobiles.Add(d1);

            d1.AIObject.Activate();
        }

        private static void SpawnStateMobiles(int state)
        {
            switch (state)
            {
                case 1:
                    {
                        GenerateMobs(typeof(PaladinQuestSkeleton));
                        break;
                    }
                case 2:
                    {
                        GenerateMobs(typeof(PaladinQuestSkeleton),2);
                        break;
                    }
                case 3:
                    {
                        GenerateMobs(typeof(PaladinQuestSkeleton),3);
                        GenerateMobs(typeof(PaladinQuestLich), 1, true);
                        break;
                    }
                case 4:
                    {
                        GenerateMobs(typeof(PaladinQuestSkeleton), 3);
                        GenerateMobs(typeof(PaladinQuestLich));
                        break;
                    }
                case 5:
                    {
                        GenerateMobs(typeof(PaladinQuestSkeleton));
                        GenerateMobs(typeof(PaladinQuestBoneKnight));
                        GenerateMobs(typeof(PaladinQuestLich));
                        GenerateDaemons();
                        break;
                    }
                case 6:
                    {
                        GenerateMobs(typeof(PaladinQuestSkeleton),5);
                        GenerateMobs(typeof(PaladinQuestBoneMagi));
                        break;
                    }
            }

        }

        public class InternalTimer : Timer
        {
            private Mobile _stealing;
            private DateTime _startTheft;

            public InternalTimer()
                : base(TimeSpan.Zero, TimeSpan.FromSeconds(10))
            {
                Priority = TimerPriority.FiveSeconds;
            }

            protected override void OnTick()
            {
                if (CurrentState == QuestState.None)
                {
                    Stop();
                    EndQuest(false);
                    return;
                }
                /*else if (CurrentState <= QuestState.Hythloth)
                {
                    if (_lastStateChange + _questItemRetreivalTime < DateTime.UtcNow)
                    {
                        EndQuest(false);
                        Stop();
                    }
                }*/
                else if ((CurrentState < QuestState.SixthWave) && _lastStateChange + _siegeStateLength < DateTime.UtcNow)
                {
                    //if (!_artifactsPlaced)
                     //   PlaceArtifacts();

                    SpawnStateMobiles((int)++CurrentState - (int)QuestState.FirstWave);
                }
                else if (_lastStateChange + _finalStageLength < DateTime.UtcNow)
                {
                    CurrentPlayer.SendMessage("You have failed the quest. The monsters have once again stolen the artifacts!");
                    EndQuest(false);
                }
                else if (CurrentState == QuestState.PreSiege) //walking to place artifacts
                {
                    if (_QuestGuard == null)
                        return;

                    if (_QuestGuard.InRange(_pathLocations[_pathLocations.Length - 1], 5))
                    {
                        _QuestGuard.Frozen = true;
                        Timer.DelayCall(TimeSpan.FromSeconds(15), delegate { if (_QuestGuard == null) return; _QuestGuard.Frozen = false; });
                        _lastStateChange = DateTime.MinValue;
                        PlaceArtifacts();
                    }
                }
                else //Siege
                {
                    if (_stealing != null)
                    {
                        if (_stealing.Combatant != null)
                        {
                            _stealing.PublicOverheadMessage(Network.MessageType.Emote, _stealing.EmoteHue, true, "*The creature has become distracted from stealing the artifacts.");
                            _stealing = null;
                        }
                        else if (_startTheft + _artifactRecaptureTime < DateTime.UtcNow)
                        {
                            CurrentPlayer.SendMessage("You have failed the quest. The monsters have once again stolen the artifacts!");
                            EndQuest(false);
                        }
                    }
                    else
                    {
                        IPooledEnumerable eable = Map.Felucca.GetMobilesInRange(new Point3D(2028, 2718, 25), 4);
                        foreach (Mobile m in eable)
                        {
                            if (ArtifactMobiles.Contains(m) && m.Combatant == null)
                            {
                                string s;

                                if (Utility.RandomBool())
                                    s = "*The creature attempts to take the artifacts.*";
                                else
                                    s = "*The creature claws furiously at the artifacts.*";

                                m.PublicOverheadMessage(Network.MessageType.Emote, m.EmoteHue, true, s);
                                _stealing = m;
                                _startTheft = DateTime.UtcNow;
                                break;
                            }
                        }
                        eable.Free();
                    }
                }
            }
        }
    }

    public class PaladinQuestCivilian : BaseCreature
    {
        private static string[] _texts = new string[] { "Help! What's going on!?", "Don't let them kill us!", "Protect us, Paladins!" };
        public override bool InitialInnocent { get { return true; } }
        public override bool AlwaysAttackable { get { return false; } }

        private static int GetRandomHue()
        {
            switch (Utility.Random(6))
            {
                default:
                case 0: return 0;
                case 1: return Utility.RandomBlueHue();
                case 2: return Utility.RandomGreenHue();
                case 3: return Utility.RandomRedHue();
                case 4: return Utility.RandomYellowHue();
                case 5: return Utility.RandomNeutralHue();
            }
        }

        [Constructable]
        public PaladinQuestCivilian()
            : base(AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4) 
        {
            Title = "the civilian";
            SetHits(100);
            SetStr(90, 100);
            SetDex(90, 100);
            SetInt(15, 25);
            VirtualArmor = 15;
            SetSkill(SkillName.Wrestling, 90.0);
            SetSkill(SkillName.MagicResist, 100.0);
                
            Hue = Utility.RandomSkinHue();

            if (Female = Utility.RandomBool())
            {
                Body = 401;
                Name = NameList.RandomName("female");
            }
            else
            {
                Body = 400;
                Name = NameList.RandomName("male");
            }

            if (Female)
                AddItem(new PlainDress());
            else
                AddItem(new Shirt(GetRandomHue()));

            int lowHue = GetRandomHue();

            AddItem(new ShortPants(lowHue));

            if (Female)
                AddItem(new Boots(lowHue));
            else
                AddItem(new Shoes(lowHue));

            Utility.AssignRandomHair(this);
        }

        public PaladinQuestCivilian(Serial serial) : base(serial) { }

        public override bool IsEnemy(Mobile m)
        {
            return Aggressors.Find(ai => ai.Defender == this) != null;
        }

        private DateTime _lastSpoke;
        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (_lastSpoke + TimeSpan.FromSeconds(10) < DateTime.UtcNow)
            {
                _lastSpoke = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.Random(10));
                Say(_texts[Utility.Random(_texts.Length)]);
            }
            base.OnMovement(m, oldLocation);
        }

        public override bool IsHarmfulCriminal(Mobile target)
        {
            if (target.Player)
                return false;

            return base.IsHarmfulCriminal(target);
        }

        public override void OnDeath(Container c)
        {
            PaladinSiege.RegisterCivilianDeath();
            base.OnDeath(c);
        }

        public override void AggressiveAction(Mobile aggressor, bool criminal, bool causeCombat)
        {
            if (aggressor is PlayerMobile)
                aggressor.CriminalAction(true);

            base.AggressiveAction(aggressor, criminal, causeCombat);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }

    public class PaladinQuestSkeleton : Skeleton
    {
        public PaladinQuestSkeleton() : base() { PassiveSpeed = 0.2; }

        public PaladinQuestSkeleton(Serial serial) : base(serial) { }

        public override bool IsEnemy(Mobile m)
        {
            if (m is PaladinQuestCivilian)
            {
                foreach (AggressorInfo ai in m.Aggressors)
                {
                    if (!ai.Attacker.Player)
                        return false;
                }
                return true;
            }

            return Aggressors.Find(ai => ai.Attacker == m) != null;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }

    public class PaladinQuestBoneKnight : BoneKnight
    {
        public PaladinQuestBoneKnight() : base() { PassiveSpeed = 0.2; }

        public PaladinQuestBoneKnight(Serial serial) : base(serial) { }

        public override bool IsEnemy(Mobile m)
        {
            if (m is PaladinQuestCivilian)
            {
                foreach (AggressorInfo ai in m.Aggressors)
                {
                    if (!ai.Attacker.Player)
                        return false;
                }
                return true;
            }

            return Aggressors.Find(ai => ai.Attacker == m) != null;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }

    public class PaladinQuestLich : Lich
    {
        public PaladinQuestLich() : base() { PassiveSpeed = 0.2; }

        public PaladinQuestLich(Serial serial) : base(serial) { }

        public override bool IsEnemy(Mobile m)
        {
            if (m is PaladinQuestCivilian)
            {
                foreach (AggressorInfo ai in m.Aggressors)
                {
                    if (!ai.Attacker.Player)
                        return false;
                }
                return true;
            }

            return Aggressors.Find(ai => ai.Attacker == m) != null;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }

    public class PaladinQuestDaemon : Daemon
    {
        public PaladinQuestDaemon() : base() { PassiveSpeed = 0.2; }

        public PaladinQuestDaemon(Serial serial) : base(serial) { }

        public override bool IsEnemy(Mobile m)
        {
            if (m is PaladinQuestCivilian)
            {
                foreach (AggressorInfo ai in m.Aggressors)
                {
                    if (!ai.Attacker.Player)
                        return false;
                }
                return true;
            }

            return Aggressors.Find(ai => ai.Attacker == m) != null;
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            PaladinSiege.RegisterDaemonDeath(this);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }

    public class PaladinQuestBoneMagi : BoneMagi
    {
        public PaladinQuestBoneMagi() : base() { PassiveSpeed = 0.2; }

        public PaladinQuestBoneMagi(Serial serial) : base(serial) { }

        public override bool IsEnemy(Mobile m)
        {
            if (m is PaladinQuestCivilian)
            {
                foreach (AggressorInfo ai in m.Aggressors)
                {
                    if (!ai.Attacker.Player)
                        return false;
                }
                return true;
            }

            return Aggressors.Find(ai => ai.Attacker == m) != null;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }

}