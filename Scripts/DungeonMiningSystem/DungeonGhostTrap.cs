using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Server.Mobiles;
using Server.Gumps;
using Server.Network;
using Server.Items;

namespace Server.PortalSystem
{
    public class GhostTrapGump : Gump
    {
        private GhostTrap m_trap;
        private Mobile m_from;

        enum GumpActions
        {
            GA_Confirm,
            GA_Cancel,
            GA_Help
        };

        public GhostTrapGump(Mobile from, GhostTrap trap)
            : base(300, 300)
        {
            m_from = from;
            m_trap = trap;
            Closable = true;

            AddPage(0);
            AddBackground(0, 0, 430, 217, 83);
            AddBackground(21, 23, 39, 173, 2620); // Inventory patch
            AddBackground(21, 23, 387, 173, 2620); // Inventory

            // Radio options to enable/disable spawning.
            AddPage(1);
            AddLabel(110, 40, 0xFF, "Ghost trap configuration (spawner)");
            AddLabel(67, 65, 0xFF, "Spawning:");
            AddLabel(70, 85, 0xFF, "No/Yes");
            AddGroup(1);
            AddRadio(60, 105, 9720, 9723, trap.m_spawningEnabled ? false : true, 1);
            AddRadio(100, 105, 9720, 9723, trap.m_spawningEnabled ? true : false, 2);

            AddLabel(177, 65, 0xFF, "Delay (min):");
            AddLabel(200, 110, 0xFF, " to ");
            AddImageTiled(150, 110, 50, 20, 0xBBC);
            AddTextEntry(150, 110, 50, 20, 0, 1, trap.m_minRespawnMinutes.ToString());
            AddImageTiled(235, 110, 50, 20, 0xBBC);
            AddTextEntry(235, 110, 50, 20, 0, 2, trap.m_maxRespawnMinutes.ToString());

            // Radio option to enable loot pool contribution.
            AddLabel(302, 65, 0xFF, "Drop Loot:");
            AddLabel(315, 85, 0xFF, "No/Yes");
            AddGroup(2);
            AddRadio(300, 105, 9720, 9723, trap.m_lootEnabled ? false : true, 3);
            AddRadio(340, 105, 9720, 9723, trap.m_lootEnabled ? true : false, 4);

            AddButton(40, 160, 2031, 2032, (int)GumpActions.GA_Help, GumpButtonType.Reply, 0);
            AddButton(275, 160, 2453, 2454, (int)GumpActions.GA_Cancel, GumpButtonType.Reply, 0);
            AddButton(345, 160, 2450, 2451, (int)GumpActions.GA_Confirm, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            // 11/11/13 - There is an unknown crash on this response.
            if (m_trap == null || info == null)
            {
                m_from.SendMessage("An error (202) occured with the trap. Notify an admin.");
                return;
            }

            switch (info.ButtonID)
            {
                case (int)GumpActions.GA_Help:
                    {
                        m_from.SendGump(new GhostTrapHelpGump(m_from, m_trap));
                        break;
                    }
                case (int)GumpActions.GA_Cancel:
                    {
                        break;
                    }
                case (int)GumpActions.GA_Confirm:
                    {
                        // Enabled
                        if (info.IsSwitched(1))
                        {
                            // Spawning has been disabled.
                            m_trap.SetSpawningEnabled(false);
                        }
                        else if (info.IsSwitched(2))
                        {
                            // Spawning has been enabled.
                            m_trap.SetSpawningEnabled(true);
                        }

                        // Respawn
                        TextRelay minRespawnRelay = info.GetTextEntry(1);
                        TextRelay maxRespawnRelay = info.GetTextEntry(2);
                        if (minRespawnRelay != null && maxRespawnRelay != null)
                        {
                            int minRespawnSec = 0;
                            int maxRespawnSec = 0;
                            if (!Int32.TryParse(minRespawnRelay.Text, out minRespawnSec) ||
                                !Int32.TryParse(maxRespawnRelay.Text, out maxRespawnSec))
                            {
                                m_from.SendMessage("You entered invalid parameters in the respawn time fields. Try again.");
                                return;
                            } 
                            if (minRespawnSec < GhostTrap.m_defaultMinRespawnMinutes)
                            {
                                m_from.SendMessage("The minimum respawn time cannot be less than {0} minutes.", GhostTrap.m_defaultMinRespawnMinutes);
                                minRespawnSec = GhostTrap.m_defaultMinRespawnMinutes;
                            }
                            if (minRespawnSec > GhostTrap.m_defaultMaxRespawnMinutes)
                            {
                                m_from.SendMessage("The minimum respawn time cannot be more than {0} minutes.", GhostTrap.m_defaultMaxRespawnMinutes);
                                minRespawnSec = GhostTrap.m_defaultMaxRespawnMinutes;
                            }

                            if (maxRespawnSec > GhostTrap.m_defaultMaxRespawnMinutes)
                            {
                                m_from.SendMessage("The maximum respawn time cannot be more than {0} minutes.", GhostTrap.m_defaultMaxRespawnMinutes);
                                maxRespawnSec = GhostTrap.m_defaultMaxRespawnMinutes;
                            }
                            if (maxRespawnSec < GhostTrap.m_defaultMinRespawnMinutes)
                            {
                                m_from.SendMessage("The maximum respawn time cannot be less than {0} minutes.", GhostTrap.m_defaultMinRespawnMinutes);
                                maxRespawnSec = GhostTrap.m_defaultMinRespawnMinutes;
                            }

                            if (minRespawnSec > maxRespawnSec)
                            {
                                m_from.SendMessage("The minimum respawn ({0} minutes) can not be greater than the maximum respawn time ({1} minutes)", 
                                    GhostTrap.m_defaultMinRespawnMinutes, GhostTrap.m_defaultMaxRespawnMinutes);
                                minRespawnSec = maxRespawnSec;
                            }

                            m_trap.SetSpawnTimeRange(minRespawnSec, maxRespawnSec);
                        }

                        // Loot
                        if (info.IsSwitched(3))
                        {
                            // Loot has been disabled.
                            m_trap.SetLootEnabled(false);
                        }
                        else if (info.IsSwitched(4))
                        {
                            // Loot has been enabled.
                            m_trap.SetLootEnabled(true);
                        }
                        break;
                    }
            }
        }
    }
    public class GhostTrapHelpGump : Gump
    {
        private GhostTrap m_trap;
        private Mobile m_from;

        enum GumpActions
        {
            GA_Done,
        }

        public GhostTrapHelpGump(Mobile from, GhostTrap trap)
            : base(300, 300)
        {
            m_from = from;
            m_trap = trap;
            Closable = true;

            AddPage(0);
            AddBackground(0, 0, 430, 217, 83);
            //AddBackground(21, 23, 39, 173, 2620); // Inventory patch
            //AddBackground(21, 23, 387, 173, 2620); // Inventory

            StringBuilder sb = new StringBuilder();
            sb.Append("Spawning: Toggles the state of spawning. One creature will be released at a time.\n");
            sb.Append("Delay: Sets the min and max time in seconds for a respawn to occur.\n");
            sb.Append("Drop Loot: Toggles whether the creature will produce a corpse once killed.\n");
            AddHtml(21, 23, 385, 130, sb.ToString(), true, true);

            AddButton(345, 160, 2450, 2451, (int)GumpActions.GA_Done, GumpButtonType.Reply, 0);
        }
        public override void OnResponse(NetState sender, RelayInfo info)
        {
            switch (info.ButtonID)
            {
                case (int)GumpActions.GA_Done:
                    {
                        m_from.SendGump(new GhostTrapGump(m_from, m_trap));
                        break;
                    }
            }
        }
    }

    public class GhostTrap : Item, IDungeonItem
    {
        public class GhostTrapTimer : Timer
        {
            GhostTrap m_trap;
            DateTime m_LastSpawn;

            public GhostTrapTimer(GhostTrap trap, int minTimeRange, int maxTimeRange)
                : base(TimeSpan.FromSeconds(GetTime(minTimeRange, maxTimeRange)))
            {
                m_trap = trap;
                Priority = TimerPriority.OneSecond;
                // allow initial spawn
                m_LastSpawn = DateTime.MinValue;

            }
            static private int GetTime(int minTimeRange, int maxTimeRange)
            {
                Random r = new Random();
                return r.Next(minTimeRange, maxTimeRange+1);
            }
            protected override void OnTick()
            {
                // ensure the minimum default time has passed
                if (m_LastSpawn + TimeSpan.FromMinutes(GhostTrap.m_defaultMinRespawnMinutes) < DateTime.UtcNow)
                {
                    m_trap.CreateCreature();
                    m_LastSpawn = DateTime.UtcNow;
                }
            }
        }

        public enum ETrapType
        {
            eTT_Unused,
            eTT_Creature,
            eTT_Demon,
        }

        public static readonly int m_cost = 150;
        // Description from vendor
        public static readonly string m_nameEx = "ghost trap";
        // Description from world
        public static readonly string m_name = "A ghost trap";
        public static readonly int m_gidActivated = 0x0A15;
        public static readonly int m_gidDeactivated = 0x0A18;
        public static int m_defaultMinRespawnMinutes = 10;
        public static int m_defaultMaxRespawnMinutes = 2880;
        //public static string[] m_demonList =
        //{
        //    "daemon",
        //    "balron",
        //};

        public bool m_activated { get; private set; }
        public ETrapType m_trapType;
        public string m_trappedTypeName;
        public Queue<TrapRecord> m_trappedList;
        public bool m_dungeonActive;
        public string m_visibleTo;
        public bool m_lootEnabled;

        // Spawner settings visible to gump
        public bool m_spawningEnabled;
        public int m_minRespawnMinutes;
        public int m_maxRespawnMinutes;

        public GhostTrapTimer m_timer;
        private BaseCreature m_spawn;
        private DateTime m_lastSpawnTimer;

        public struct TrapRecord
        {
            public string m_creatureType;
            public TrapRecord(string creatureType)
            {
                m_creatureType = creatureType;
            }
        }
        
        [Constructable]
        public GhostTrap()
            : base(m_gidDeactivated)
        {
            Name = m_nameEx;

            m_activated = false;
            m_trappedList = new Queue<TrapRecord>();
            m_dungeonActive = false;
            m_trappedTypeName = String.Empty;
            m_visibleTo = String.Empty;
            m_spawningEnabled = false;
            m_minRespawnMinutes = m_defaultMinRespawnMinutes;
            m_maxRespawnMinutes = m_defaultMaxRespawnMinutes;
            m_lootEnabled = true;
            m_lastSpawnTimer = DateTime.UtcNow;
        }
        public GhostTrap(Serial serial)
            : base(serial)
        {
            m_trappedList = new Queue<TrapRecord>();
            m_lastSpawnTimer = DateTime.UtcNow;
        }
        public override bool Decays
        {
            get
            {
                return this.Map != PortalsSystem.s_map;
            }
        }

        public override string DefaultName
        {
            get { return m_name; }
        }

        private static Type[] m_AllowedTypes = new Type[] 
        {
            // Covetous mobs
            typeof( Aboleth ),          typeof( DrowBlackguard ),   typeof( Beholder ),
            typeof( DrowBlademaster ),  typeof( DrowSpellsinger ),
            typeof( DriderHarbinger ),  typeof( DriderSentinel ),   typeof( DriderWarrior ),
            typeof( Gauth ),            typeof( Glowvines ),        typeof( HookHorror ), 
            typeof( MindFlayer ),       typeof( Myconid ),          typeof( MyconidTallstalk ),
            typeof( NightbarkTree ),    typeof( SvirfneblinIllusionist ), 
            typeof( SvirfneblinRogue ), typeof( Umberhulk ),        typeof( KuoToa ),
            // Deceit mobs
            typeof( CorruptReaver ),    typeof( BloodyZombie ),     typeof( Creeper ),
            typeof( CorruptRunecaster), typeof( CorruptWarmage ),
            typeof( DecayedZombie ),    typeof( HollowOne ),        typeof( UndeadOgreLord ),
            // Despise mobs
            typeof( DespiseOrc ),       typeof( ElderMojoka ),      typeof( OrcishExecutioner ),
            typeof( OrcishGrunt ),      typeof( OrcishMaurk ),      typeof( OrcishPeon ),
            typeof( OrcishScout ),      typeof( OrcishSurjin ),     typeof( OrcMojoka ),
            // Ice mobs
            typeof( OrghereimBeastmaster ),
            typeof( OrghereimBoneMender ),
            typeof( OrghereimBowMaiden ),
            typeof( OrghereimCrone ),   typeof( OrghereimIceCarl ), typeof( OrghereimSage ),
            typeof( OrghereimShieldMaiden ),
            typeof( OrghereimShieldMother ),
            typeof( OrghereimSwordThane ),
            typeof( OrghereimTracker ), typeof( Valkyrie ),
            // Shame mobs
            typeof( AncientFlame ),     typeof( ArmoredTitan ),     typeof( AtlanteanBattleMage ),
            typeof( AtlanteanWarden ),  typeof( EarthOreling ),     typeof( ElderFireElemental ),
            typeof(ElderWaterElemental),typeof( ElementalSeer ),    typeof( EnergyElementalist ),
            typeof( FlameElementalist ),typeof( FountainOfEvil ),   typeof( Gust ),
            typeof( Puddle ),           typeof( RockElemental ),    typeof( RockOreling ),
            // Wrong mobs
            typeof( SanguinAlchemist ), typeof( SanguinAssassin ),  typeof( SanguinConscript ),
            typeof( SanguinDefender ),  typeof( SanguinHealer ),    typeof( SanguinHunter ),
            typeof( SanguinKnight ),    typeof( SanguinMage ),      typeof( SanguinMedic ),
            typeof( SanguinMender ),    typeof( SanguinProtector ), typeof( SanguinScout ),
            typeof( SanguinWizard ),
            // Misc mobs
            typeof( FrostOrc ),         typeof( FrostOrcLord ),     typeof( FrostOrcMage ),
            typeof( BoneKnight ),       typeof( EarthElemental ),   typeof( Ettin ),
            typeof( Ghoul ),            typeof( Golem ),            typeof( HeadlessOne ),
            typeof( Mummy ),            typeof( Ogre ),             typeof( OgreLord ),
            typeof( Rat ),              typeof( RottingCorpse ),    typeof( Sewerrat ),
            typeof( Skeleton ),         typeof( Slime ),            typeof( Zombie ),
            typeof( Walrus ),           typeof( RestlessSoul ),     typeof( AirElemental ),
            typeof( AncientWyrm ),      typeof( Balron ),           
            typeof( Lich ),             typeof( PoisonElemental ),  typeof( Revenant ),
            typeof( WhiteWyrm ),        typeof( Wisp ),
            typeof( SilverSerpent ),
            typeof( Alligator ),
            typeof( AncientLich ),      typeof( Bird ),
            typeof( BlackBear ),        
            typeof( BloodElemental ),   typeof( Boar ),             
            typeof( BoneMagi ),         typeof( Brigand ),          
            typeof( BrownBear ),        typeof( Bull ),             typeof( BullFrog ),
            typeof( Cat ),              
            typeof( Chicken ),          
            typeof( Cougar ),           typeof( Cow ),
            typeof( Cyclops ),          typeof( Daemon ),           typeof( DeepSeaSerpent ),
            typeof( DesertOstard ),     typeof( DireWolf ),         typeof( Dog ),
            typeof( Dolphin ),          typeof( Dragon ),           typeof( Drake ),
            typeof( Eagle ),            typeof( ElderGazer ),
            typeof( EvilMage ),         typeof( EvilMageLord ),     typeof( Executioner ),
            typeof( Savage ),           typeof( FireElemental ),    
            typeof( FrostSpider ),      typeof( Gargoyle ),         typeof( Gazer ),
            typeof( GiantRat ),         typeof( GiantSerpent ),
            typeof( GiantSpider ),      typeof( GiantToad ),        typeof( Goat ),
            typeof( Gorilla ),          typeof( GreatHart ),
            typeof( GreyWolf ),         typeof( GrizzlyBear ),      
            typeof( Harpy ),            
            typeof( Hind ),             typeof( Horse ),
            typeof( Imp ),              typeof( Kraken ),
            typeof( Lizardman ),        typeof( Mongbat ),
            typeof( MountainGoat ),     typeof( Orc ),
            typeof( OrcBomber ),        typeof( OrcBrute ),         typeof( OrcCaptain ),
            typeof( OrcishLord ),       typeof( OrcishMage ),       
            typeof( Panther ),          typeof( Pig ),
            typeof( PolarBear ),        typeof( Rabbit ),
            typeof( Ratman ),           typeof( RatmanArcher ),     typeof( RatmanMage ),
            typeof( Scorpion ),         typeof( SeaSerpent ),
            typeof( Shade ),            typeof( Sheep ),
            typeof( SkeletalMage ),
            typeof( Snake ),
            typeof( SnowLeopard ),      typeof( Spectre ),
            typeof( SwampTentacle ),    
            typeof( TimberWolf ),       typeof( Titan ),            typeof( Troll ),
            typeof( WaterElemental ),   
            typeof( WhiteWolf ),        typeof( Wraith ),           typeof( Wyvern ),
            typeof( LichLord ),         typeof( SkeletalKnight ),
        };

        public static bool CanTrapType(Type creatureType)
        {
            return m_AllowedTypes.Contains(creatureType);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(m_activated);
            writer.Write((int)m_trapType);
            writer.Write(m_trappedTypeName);
            writer.Write(m_visibleTo);
            writer.Write(m_spawningEnabled);
            writer.Write(m_minRespawnMinutes);
            writer.Write(m_maxRespawnMinutes);
            writer.Write(m_dungeonActive);
            writer.Write(m_lootEnabled);

            writer.Write(m_trappedList.Count);
            foreach (TrapRecord record in m_trappedList)
            {
                writer.Write(record.m_creatureType);
            }

            writer.Write(m_spawn);
        }
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            m_activated = reader.ReadBool();
            m_trapType = (ETrapType)reader.ReadInt();
            m_trappedTypeName = reader.ReadString();
            m_visibleTo = reader.ReadString();
            m_spawningEnabled = reader.ReadBool();

            // Clamp the values in the case of a min/max change during development.
            int min = GhostTrap.m_defaultMinRespawnMinutes;
            int max = GhostTrap.m_defaultMaxRespawnMinutes;
            m_minRespawnMinutes = reader.ReadInt();
            m_minRespawnMinutes = Math.Max(min, Math.Min(max, m_minRespawnMinutes));
            m_maxRespawnMinutes = reader.ReadInt();
            m_maxRespawnMinutes = Math.Max(min, Math.Min(max, m_maxRespawnMinutes));

            m_dungeonActive = reader.ReadBool();
            m_lootEnabled = reader.ReadBool();

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                TrapRecord record = new TrapRecord(reader.ReadString());
                m_trappedList.Enqueue(record);
            }

            m_spawn = reader.ReadMobile() as BaseCreature;
            if (m_spawn == null && m_dungeonActive && m_spawningEnabled && count > 0)
            {
                m_timer = new GhostTrapTimer(this, m_minRespawnMinutes, m_maxRespawnMinutes);
                m_timer.Start();
            }
        }
        #region Interaction
        public override void OnSingleClick(Mobile from)
        {
            string contents = "";

            int listSize = m_trappedList.Count;
            if (listSize > 0)
            {
                contents = String.Format("{0}: {1}", m_trapType == ETrapType.eTT_Demon ? "demon" : m_trappedTypeName, listSize);
            }

            from.SendMessage("{0} {1}. {2}", m_activated ? "An activated" : "A deactivated", m_nameEx, contents);

        }
        public override void GetProperties(ObjectPropertyList list)
        {
            list.Add("A {0} ", m_nameEx);
        }
        public override void OnDoubleClick(Mobile from)
        {
            if (m_dungeonActive)
            {
                from.SendGump(new GhostTrapGump(from, this));
            }
            else
            {
                m_activated = !m_activated;
                ItemID = m_activated ? m_gidActivated : m_gidDeactivated;
                from.SendMessage("You have {0} the {1}.", m_activated ? "activated" : "deactivated", m_nameEx);

            }
        }
        #endregion

        // IDungeonItem
        public bool IsVisibleTo(Mobile m)
        {
            // If the item isn't a spawner, it's visible to everyone. Otherwise,
            // the item is only visible to the player who dropped it into their dungeon.
            return m.AccessLevel > AccessLevel.Player || !m_dungeonActive || m_visibleTo == m.Name;
        }
        // ~IDungeonItem

        public override bool OnDroppedToWorld(Mobile from, Point3D p)
        {
            // If a dungeon owner drops this on the ground of their dungeon,
            // it becomes a spawner.

            // Determine if the trap is on the ground of the mobile's dungeon.
            PortalPartition partition = PortalsSystem.GetPartitionByUsername(from.Account.Username);
            if (partition != null && partition.InteriorContainsPoint(p) && from.Map == PortalsSystem.s_map)
            {
                SetSpawnReadiness(true, from.Name);
            }
            else
            {
                SetSpawnReadiness(false);
            }

 	        return base.OnDroppedToWorld(from, p);
        }
        private void CloseGumps(Mobile from)
        {
            if (from.HasGump(typeof(GhostTrapGump)))
            {
                from.CloseGump(typeof(GhostTrapGump));
            }
            if (from.HasGump(typeof(GhostTrapHelpGump)))
            {
                from.CloseGump(typeof(GhostTrapHelpGump));
            }
        }
        public override bool OnDroppedInto(Mobile from, Items.Container target, Point3D p)
        {
            CloseGumps(from);
            Disable();
            return base.OnDroppedInto(from, target, p);
        }
        public override bool OnDroppedOnto(Mobile from, Item target, Point3D p)
        {
            CloseGumps(from);
            Disable();
            return base.OnDroppedOnto(from, target, p);
        }
        public override bool OnDroppedToMobile(Mobile from, Mobile target)
        {
            CloseGumps(from);
            Disable();
            return base.OnDroppedToMobile(from, target);
        }
        private void Disable()
        {
            m_dungeonActive = false;
            m_spawningEnabled = false;
            m_visibleTo = String.Empty;

            // Kill any running timers.
            if (m_timer != null)
            {
                m_timer.Stop();
            }
            // Dispose of any creatures.
            if (m_spawn != null)
            {
                m_spawn.Delete();
                m_spawn = null;
            }
        }
        /// <summary>
        /// Add will append a creature type to the trap. If the type is not currently
        /// set on the trap, it will be deduced based on the creatureType passed.
        /// </summary>
        /// <param name="creatureType"></param>
        public void Add(string creatureType)
        {
            if (m_trapType == ETrapType.eTT_Unused)
            {
                // Determine which type of trap this creature belongs in.
                //m_trapType = IsCategorizedByDemon(creatureType) ? ETrapType.eTT_Demon : ETrapType.eTT_Creature;
                m_trapType = ETrapType.eTT_Creature;
                m_trappedTypeName = creatureType;
            }

            TrapRecord record = new TrapRecord(creatureType);
            m_trappedList.Enqueue(record);
        }
        public void SetSpawnTimeRange(int minSeconds, int maxSeconds)
        {
            m_minRespawnMinutes = minSeconds;
            m_maxRespawnMinutes = maxSeconds;
        }

        /// <summary>
        /// SetSpawnReadiness toggles interactivity depending based on it's presence
        /// in a player's dungeon. Called as the item is moved about the world.
        /// </summary>
        /// <param name="isDungeonActive"></param>
        /// <param name="visibleTo"></param>
        private void SetSpawnReadiness(bool isDungeonActive, string visibleTo = "")
        {
            // 1 of 2 criteria for the spawner timer to begin.
            m_dungeonActive = isDungeonActive;
            m_visibleTo = visibleTo;

            if (m_dungeonActive && m_spawningEnabled)
            {
                PrepareSpawner();
            }
        }
        /// <summary>
        /// SetSpawningEnabled is flipped manually by the dungeon owner
        /// once it is on their dungeon floor.
        /// </summary>
        /// <param name="isEnabled"></param>
        public void SetSpawningEnabled(bool isEnabled)
        {
            // 1 of 2 criteria for the spawner timer to begin.
            m_spawningEnabled = isEnabled;

            if (m_dungeonActive && m_spawningEnabled)
            {
                PrepareSpawner();
            }

            if (!m_spawningEnabled)
            {
                // Dispose of any creatures.
                if (m_spawn != null)
                {
                    m_spawn.Delete();
                    m_spawn = null;
                }
            }

        }
        public void SetLootEnabled(bool isEnabled)
        {
            m_lootEnabled = isEnabled;

            // The existing spawn can still apply this setting.
            if (m_spawn != null)
            {
                // Removes corpse on death if loot is not enabled.
                m_spawn.m_consumedByGhostTrap = !m_lootEnabled;
            }
        }

        /// <summary>
        /// PrepareSpawner is called when the trap is placed into an enabled state
        /// within a player's dungeon. This begins the timer.
        /// </summary>
        private void PrepareSpawner()
        {
            // The trap must be in the player's dungeon, and enabled.
            if (!(m_dungeonActive && m_spawningEnabled && m_spawn == null))
            {
                // Never continue.
                return;
            }

            // To ensure that there isn't any duping going on, ensure that the time between timers
            // is at least 1 second.
            DateTime currentTime = DateTime.UtcNow;
            bool timeValid = m_lastSpawnTimer < currentTime.Subtract(TimeSpan.FromSeconds(1.0));

            if (m_trappedList.Count > 0 && timeValid)
            {
                m_lastSpawnTimer = currentTime;
                m_timer = new GhostTrapTimer(this, m_minRespawnMinutes, m_maxRespawnMinutes);
                m_timer.Start();
            }
        }
        /// <summary>
        /// Callback from GhostTrapTimer to place a creature at the trap location.
        /// </summary>
        public void CreateCreature()
        {
            if (!(m_trappedList.Count > 0))
            {
                Reset();
                return;
            }

            if (m_spawn == null && CanCreate())
            {
                // Create the next creature in the queue. This will be removed once
                // the creature dies.
                TrapRecord record = m_trappedList.Peek();
                Type type = ScriptCompiler.FindTypeByName(record.m_creatureType);
                if (type != null)
                {
                    m_spawn = ((BaseCreature)XmlSpawner.CreateObject(type, record.m_creatureType));
                    m_spawn.MoveToWorld(this.Location, this.Map);

                    // Removes corpse on death if loot is not enabled.
                    m_spawn.m_consumedByGhostTrap = !m_lootEnabled;
                    m_spawn.Tamable = false;
                    m_spawn.Home = this.Location;
                    m_spawn.RangeHome = 6;

                    // Required callback on death.
                    m_spawn.m_trap = this;
                }
            }
        }

        /// <summary>
        /// OnDeath called from BaseCreature::OnBeforeDeath. Once the creature has been reported
        /// dead, we can dequeue the entry. This is the only time we can dequeue, as the creature
        /// could be removed for a number of other reasons, including removing the trap from the
        /// ground.
        /// </summary>
        public void OnDeath()
        {
            // The creature was genuinely killed, remove it from the list.
            m_trappedList.Dequeue();
            m_spawn = null;

            if (!(m_trappedList.Count > 0))
            {
                Reset();
                return;
            }

            PrepareSpawner();
        }
        private bool CanCreate()
        {
            return m_dungeonActive && m_spawningEnabled && m_trappedList.Count > 0;
        }
        private void Reset()
        {
            m_trapType = ETrapType.eTT_Unused;
            m_trappedTypeName = String.Empty;
            m_spawningEnabled = false;
        }
        public bool CanTrapCreature(string creatureType)
        {
            bool result = (m_trapType == ETrapType.eTT_Creature &&
                String.Compare(m_trappedTypeName, creatureType, true) == 0);
            return result;
        }
        //public bool CanTrapDemon(string creatureType)
        //{
        //    bool result = (m_trapType == ETrapType.eTT_Demon &&
        //        IsCategorizedByDemon(creatureType));
        //    return result;
        //}
        //public bool IsCategorizedByDemon(string creatureType)
        //{
        //    return m_demonList.Contains(creatureType.ToLower());
        //}

    }

    public class GhostCoins : Item
    {
        public override double DefaultWeight
        {
            get { return (Core.ML ? (0.02 / 3) : 0.02); }
        }

        [Constructable]
        public GhostCoins()
            : this(1, true)
        {
        }

        [Constructable]
        public GhostCoins(int amountFrom, int amountTo)
            : this(Utility.RandomMinMax(amountFrom, amountTo), false)
        {
        }

        [Constructable]
        public GhostCoins(int amount)
            : this(amount, false)
        {
        }

        [Constructable]
        public GhostCoins(int amount, bool exact)
            : base(0xEED)
        {
            Hue = 1900; // Grey
            Stackable = true;
            Amount = exact ? amount : (int)Math.Ceiling(amount / 3.0);
            Server.Custom.CurrencyTracking.RegisterGhostCoins(Amount);
        }
        public override string DefaultName
        {
            get { return "ghost coins"; }
        }
        public override bool StackWith(Mobile from, Item dropped, bool playSound)
        {
            return dropped.Movable && Movable && base.StackWith(from, dropped, playSound);
        }

        public GhostCoins(Serial serial)
            : base(serial)
        {
        }

        public override int GetDropSound()
        {
            if (Amount <= 1)
                return 0x2E4;
            else if (Amount <= 5)
                return 0x2E5;
            else
                return 0x2E6;
        }

        protected override void OnAmountChange(int oldValue)
        {
            int newValue = this.Amount;

            UpdateTotal(this, TotalType.GhostCoins, newValue - oldValue);

            Server.Custom.CurrencyTracking.RegisterGhostCoins(newValue - oldValue);
        }

        public override int GetTotal(TotalType type)
        {
            int baseTotal = base.GetTotal(type);

            if (type == TotalType.GhostCoins)
                baseTotal += this.Amount;

            return baseTotal;
        }

        public override void OnDelete()
        {
            Server.Custom.CurrencyTracking.DeleteGhostCoins(this.Amount);
            base.OnDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            Server.Custom.CurrencyTracking.RegisterGhostCoins(this.Amount);
        }
    }

}
