﻿using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Misc;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Targeting;
using Server.Achievements;
using Server.Accounting;
using System.Linq;
using Server.Custom;
using System.Text;

namespace Server
{
    public enum UOACZCustomRegion
    {
        Malas1,
        Terminus1,
    }

    public static class UOACZPersistance
    {
        public static UOACZPersistanceItem PersistanceItem;

        public static List<UOACZAccountEntry> m_UOACZAccountEntries = new List<UOACZAccountEntry>();
        public static List<UOACZCharacterSnapshot> m_UOACZCharacterSnapshots = new List<UOACZCharacterSnapshot>();

        public static Point3D DefaultBritainLocation = new Point3D(1496, 1630, 10);
        public static Map DefaultBritainMap = Map.Felucca;

        public static Point3D DefaultHumanLocation = new Point3D(2033, 1333, -88);
        public static Point3D DefaultUndeadLocation = new Point3D(1823, 1300, -90);
        public static Map DefaultMap = Map.Malas;

        private static UOACZCustomRegion m_CustomRegion = UOACZCustomRegion.Malas1;
        [CommandProperty(AccessLevel.GameMaster)]
        public static UOACZCustomRegion CustomRegion
        {
            get { return m_CustomRegion; }
            set
            {
                if (m_CustomRegion != value)
                {
                    m_CustomRegion = value;

                    ChangeRegion();
                }
            }
        }

        private static bool m_Active = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public static bool Active
        {
            get { return m_Active; }
            set { m_Active = value; }
        }

        public static TimeSpan ScheduledSessionInterval = TimeSpan.FromDays(7);
        public static TimeSpan ScheduledSessionAnnouncementLeadtime = TimeSpan.FromMinutes(10);

        public static TimeSpan SessionDuration = TimeSpan.FromHours(6);

        public static DateTime m_NextScheduledSessionStartTime = DateTime.MaxValue;
        public static bool m_AnnouncementMade = false;

        public static DateTime m_CurrentSessionStartTime = DateTime.MaxValue;
        public static DateTime m_CurrentSessionExpiration = DateTime.MaxValue;

        public static bool m_SessionEndOccuring = false;

        public static DateTime m_MinuteOccurred = DateTime.MaxValue;

        public static int m_MinutesActive = 0;

        public static int m_ThreatLevelProgress = 0;
        public static int m_ThreatLevel = UOACZSystem.StartingThreatLevel;

        public static int m_SpawnWaveProgress = 0;

        public static double HumanBalanceScalar = 1.0;
        public static double UndeadBalanceScalar = 1.0;

        public static int HumanObjectivesComplete = 0;
        public static int UndeadObjectivesComplete = 0;

        public static double m_HumanObjective1 = 0; //Destroy Source of Corruption
        public static double m_HumanObjective2 = 0; //Build the Outpost
        public static double m_HumanObjective3 = 0; //Gather Bones
        public static double m_HumanObjective4 = 0; //Kill Undead Champion
        public static double m_HumanObjective5 = 0; //Kill Undead Boss        

        public static double m_UndeadObjective1 = 0; //Destroy All Stockpiles
        public static double m_UndeadObjective2 = 0; //Kill All Civilians
        public static double m_UndeadObjective3 = 0; //Corrupt Wildlife
        public static double m_UndeadObjective4 = 0; //Kill Human Champion
        public static double m_UndeadObjective5 = 0; //Kill Human Boss

        public static double m_HumanObjective1Target = 1;
        public static double m_HumanObjective2Target = 1;
        public static double m_HumanObjective3Target = 1;
        public static double m_HumanObjective4Target = 1;
        public static double m_HumanObjective5Target = 1;

        public static double m_UndeadObjective1Target = 1;
        public static double m_UndeadObjective2Target = 1;
        public static double m_UndeadObjective3Target = 1;
        public static double m_UndeadObjective4Target = 1;
        public static double m_UndeadObjective5Target = 1;

        public static List<UOACZBreakableStatic> m_OutpostComponents = new List<UOACZBreakableStatic>();

        public static BaseCreature HumanChampion;
        public static BaseCreature HumanBoss;

        public static BaseCreature UndeadChampion;
        public static BaseCreature UndeadBoss;

        public static Point3D HumanChampionLocation;
        public static Point3D HumanBossLocation;

        public static Point3D UndeadChampionLocation;
        public static Point3D UndeadBossLocation;

        public static Double UndeadSentToPlayerDelayChangePerPlayerOnline = 3;
        public static Double UndeadSentToPlayerDelayMaximumSeconds = 60;
        public static Double UndeadSentToPlayerDelayMinimumSeconds = 15;

        public static Dictionary<UOACZSystem.StockpileContributionType, double> m_StockpileProgress = new Dictionary<UOACZSystem.StockpileContributionType, double>();

        private static bool m_Enabled = true;
        [CommandProperty(AccessLevel.GameMaster)]
        public static bool Enabled
        {
            get { return m_Enabled; }
            set
            {
                m_Enabled = value;

                if (!m_Enabled && Active)
                {
                    EndSession();
                }
            }
        }

        public static List<UOACZAccountEntry> m_PreviousHumanScores = new List<UOACZAccountEntry>();
        public static List<UOACZAccountEntry> m_PreviousUndeadScores = new List<UOACZAccountEntry>();
        public static List<UOACZAccountEntry> m_PreviousTotalScores = new List<UOACZAccountEntry>();

        public static List<UOACZAccountEntry> m_BestHumanScores = new List<UOACZAccountEntry>();
        public static List<UOACZAccountEntry> m_BestUndeadScores = new List<UOACZAccountEntry>();
        public static List<UOACZAccountEntry> m_BestTotalScores = new List<UOACZAccountEntry>();

        public static List<UOACZAccountEntry> m_LifetimeHumanScores = new List<UOACZAccountEntry>();
        public static List<UOACZAccountEntry> m_LifetimeUndeadScores = new List<UOACZAccountEntry>();
        public static List<UOACZAccountEntry> m_LifetimeTotalScores = new List<UOACZAccountEntry>();

        public static Timer m_Timer;

        public static void Initialize()
        {
            CommandSystem.Register("UOACZDeleteAllAccounts", AccessLevel.GameMaster, new CommandEventHandler(UOACZDeleteAllAccounts));

            CommandSystem.Register("UOACZEnable", AccessLevel.GameMaster, new CommandEventHandler(UOACZEnable));
            CommandSystem.Register("UOACZDisable", AccessLevel.GameMaster, new CommandEventHandler(UOACZDisable));

            CommandSystem.Register("UOACZStartScheduleNow", AccessLevel.GameMaster, new CommandEventHandler(UOACZStartScheduleNow));
            CommandSystem.Register("UOACZStartScheduleInXMinutes", AccessLevel.GameMaster, new CommandEventHandler(UOACZStartScheduleInXMinutes));
            CommandSystem.Register("UOACZResetSchedule", AccessLevel.GameMaster, new CommandEventHandler(UOACZResetSchedule));

            CommandSystem.Register("UOACZClearWorld", AccessLevel.GameMaster, new CommandEventHandler(UOACZClearWorld));
            CommandSystem.Register("UOACZBuildWorld", AccessLevel.GameMaster, new CommandEventHandler(UOACZBuildWorld));

            CommandSystem.Register("UOACZStartSession", AccessLevel.GameMaster, new CommandEventHandler(UOACZStartSession));
            CommandSystem.Register("UOACZEndSession", AccessLevel.GameMaster, new CommandEventHandler(UOACZEndSession));

            CommandSystem.Register("UOACZMap1", AccessLevel.GameMaster, new CommandEventHandler(UOACZMap1));
            CommandSystem.Register("UOACZMap2", AccessLevel.GameMaster, new CommandEventHandler(UOACZMap2));

            CommandSystem.Register("UOACZSpawnAllWildernessUndead", AccessLevel.GameMaster, new CommandEventHandler(UOACZSpawnAllWildernessUndead));
            CommandSystem.Register("UOACZSpawnAllTownUndead", AccessLevel.GameMaster, new CommandEventHandler(UOACZSpawnAllTownUndead));

            CommandSystem.Register("H", AccessLevel.Player, new CommandEventHandler(UOACZHumanChat));
            CommandSystem.Register("U", AccessLevel.Player, new CommandEventHandler(UOACZUndeadChat));

            CommandSystem.Register("UOACZShowHumanChat", AccessLevel.Player, new CommandEventHandler(UOACZShowHumanChat));
            CommandSystem.Register("UOACZShowUndeadChat", AccessLevel.Player, new CommandEventHandler(UOACZShowUndeadChat));

            CommandSystem.Register("UOACZShowSurvivalTome", AccessLevel.Player, new CommandEventHandler(UOACZShowSurvivalTome));
            CommandSystem.Register("UOACZShowCorruptionTome", AccessLevel.Player, new CommandEventHandler(UOACZShowCorruptionTome));

            CommandSystem.Register("UOACZShowHumanStats", AccessLevel.Player, new CommandEventHandler(UOACZShowHumanStats));
            CommandSystem.Register("UOACZShowUndeadStats", AccessLevel.Player, new CommandEventHandler(UOACZShowUndeadStats));

            CommandSystem.Register("UOACZShowHumanHotbar", AccessLevel.Player, new CommandEventHandler(UOACZShowHumanHotbar));
            CommandSystem.Register("UOACZShowUndeadHotbar", AccessLevel.Player, new CommandEventHandler(UOACZShowUndeadHotbar));

            CommandSystem.Register("UOACZShowObjectives", AccessLevel.Player, new CommandEventHandler(UOACZShowObjectives));

            CommandSystem.Register("UOACZAbility1", AccessLevel.Player, new CommandEventHandler(UOACZAbility1));
            CommandSystem.Register("UOACZAbility2", AccessLevel.Player, new CommandEventHandler(UOACZAbility2));
            CommandSystem.Register("UOACZAbility3", AccessLevel.Player, new CommandEventHandler(UOACZAbility3));
            CommandSystem.Register("UOACZAbility4", AccessLevel.Player, new CommandEventHandler(UOACZAbility4));
            CommandSystem.Register("UOACZAbility5", AccessLevel.Player, new CommandEventHandler(UOACZAbility5));

            CommandSystem.Register("UOACZHotbarLeft", AccessLevel.Player, new CommandEventHandler(UOACZHotbarLeft));
            CommandSystem.Register("UOACZHotbarRight", AccessLevel.Player, new CommandEventHandler(UOACZHotbarRight));
                         
            //CommandSystem.Register("UOACZResetCooldowns", AccessLevel.Player, new CommandEventHandler(UOACZResetCooldowns));
            //CommandSystem.Register("UOACZFullStats", AccessLevel.Player, new CommandEventHandler(UOACZFullStats));
            //CommandSystem.Register("UOACZBoost", AccessLevel.Player, new CommandEventHandler(UOACZBoost));

            Timer.DelayCall(TimeSpan.FromTicks(1), delegate
            {
                if (PersistanceItem == null)
                    PersistanceItem = new UOACZPersistanceItem();

                else if (PersistanceItem.Deleted)
                    PersistanceItem = new UOACZPersistanceItem();

                Timer.DelayCall(TimeSpan.FromSeconds(1), delegate
                {
                    if (Enabled)
                    {
                        if (m_Timer != null)
                        {
                            m_Timer.Stop();
                            m_Timer = null;
                        }

                        m_Timer = new UOACZTimer();
                        m_Timer.Start();
                    }
                });
            });
        }

        #region Commmands

        [Usage("UOACZDeleteAllAccounts")]
        [Description("Deletes all UOACZ Accounts")]
        public static void UOACZDeleteAllAccounts(CommandEventArgs arg)
        {
            PlayerMobile pm_Mobile = arg.Mobile as PlayerMobile;

            if (pm_Mobile == null)
                return;

            Queue m_Queue = new Queue();

            //Accounts
            foreach (UOACZAccountEntry entry in m_UOACZAccountEntries)
            {
                if (entry == null) continue;
                if (entry.Deleted) continue;

                m_Queue.Enqueue(entry);
            }

            while (m_Queue.Count > 0)
            {
                UOACZAccountEntry entry = (UOACZAccountEntry)m_Queue.Dequeue();
                entry.Delete();
            }

            m_UOACZAccountEntries = new List<UOACZAccountEntry>();

            //Snapshots
            foreach (UOACZCharacterSnapshot snapshot in m_UOACZCharacterSnapshots)
            {
                if (snapshot == null) continue;
                if (snapshot.Deleted) continue;

                m_Queue.Enqueue(snapshot);
            }

            while (m_Queue.Count > 0)
            {
                UOACZCharacterSnapshot snapshot = (UOACZCharacterSnapshot)m_Queue.Dequeue();
                snapshot.Delete();
            }

            m_UOACZCharacterSnapshots = new List<UOACZCharacterSnapshot>();

            pm_Mobile.SendMessage("Deleted All UOACZ Accounts.");
        }

        [Usage("UOACZEnable")]
        [Description("Enables UOACZ System")]
        public static void UOACZEnable(CommandEventArgs arg)
        {
            PlayerMobile pm_Mobile = arg.Mobile as PlayerMobile;

            if (pm_Mobile == null)
                return;

            Enabled = true;

            pm_Mobile.SendMessage("Enabling UOACZ system.");
        }

        [Usage("UOACZDisable")]
        [Description("Disables UOACZ System")]
        public static void UOACZDisable(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            if (Active)
            {
                foreach (NetState state in NetState.Instances)
                {
                    Mobile mobile = state.Mobile;
                    PlayerMobile pm_Mobile = mobile as PlayerMobile;

                    if (pm_Mobile == null)
                        continue;

                    pm_Mobile.SendMessage(UOACZSystem.purpleTextHue, "UOACZ has ended.");
                }
            }

            Enabled = false;

            player.SendMessage("Disabling UOACZ system.");
        }

        [Usage("UOACZStartScheduleNow")]
        [Description("Resets Scheduled Timer To Now As Starting Point")]
        public static void UOACZStartScheduleNow(CommandEventArgs arg)
        {
            PlayerMobile pm_Mobile = arg.Mobile as PlayerMobile;

            if (pm_Mobile == null)
                return;

            m_NextScheduledSessionStartTime = DateTime.UtcNow;


            pm_Mobile.SendMessage("UOACZ Schedule Started To Now.");
        }

        [Usage("UOACZStartScheduleInXMinutes")]
        [Description("Resets Scheduled Timer To Now + X Minutes As Starting Point")]
        public static void UOACZStartScheduleInXMinutes(CommandEventArgs arg)
        {
            PlayerMobile pm_Mobile = arg.Mobile as PlayerMobile;

            if (pm_Mobile == null)
                return;

            if (arg.Length == 1)
            {
                try
                {
                    int minutes = arg.GetInt32(0);

                    if (minutes <= 0)
                    {
                        pm_Mobile.SendMessage("Minutes must be more than 0.");
                        return;
                    }

                    m_NextScheduledSessionStartTime = DateTime.UtcNow + TimeSpan.FromMinutes(minutes);
                }

                catch
                {
                    pm_Mobile.SendMessage("Error in arguments. Usage: [UOACZStartScheduleInXMinutes minutes");
                    return;
                }
            }

            pm_Mobile.SendMessage("Current DateTime: " + DateTime.UtcNow.ToString());
            pm_Mobile.SendMessage("UOACZ Schedule set to start at: " + m_NextScheduledSessionStartTime.ToString());
        }

        [Usage("UOACZResetSchedule")]
        [Description("Resets Scheduled Timer To Now + Interval As Starting Point")]
        public static void UOACZResetSchedule(CommandEventArgs arg)
        {
            PlayerMobile pm_Mobile = arg.Mobile as PlayerMobile;

            if (pm_Mobile == null)
                return;

            m_NextScheduledSessionStartTime = DateTime.UtcNow + ScheduledSessionInterval;


            pm_Mobile.SendMessage("UOACZ Schedule Restarted.");
        }

        [Usage("UOACZClearWorld")]
        [Description("Clears All World Objects for current UOACZ Region")]
        public static void UOACZClearWorld(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            UOACZSystem.ClearWorld();
        }

        [Usage("UOACZBuildWorld")]
        [Description("Builds All World Objects for current UOACZ Region")]
        public static void UOACZBuildWorld(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            UOACZSystem.BuildWorld();
        }

        [Usage("UOACZStartSession")]
        [Description("Starts the current UOACZ Session")]
        public static void UOACZStartSession(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            if (!UOACZPersistance.Enabled)
            {
                player.SendMessage("UOACZ is currently disabled.");
                return;
            }

            if (UOACZPersistance.Active)
            {
                player.SendMessage("A UOACZ session is already in progress.");
                return;
            }

            else
            {
                UOACZPersistance.StartSession();
                return;
            }
        }

        [Usage("UOACZEndSession")]
        [Description("Ends the current UOACZ Session")]
        public static void UOACZEndSession(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            if (!UOACZPersistance.Active)
            {
                player.SendMessage("There is no UOACZ session in progress.");
                return;
            }

            else
            {
                foreach (NetState state in NetState.Instances)
                {
                    Mobile mobile = state.Mobile;
                    PlayerMobile pm_Mobile = mobile as PlayerMobile;

                    if (pm_Mobile == null)
                        continue;

                    pm_Mobile.SendMessage(UOACZSystem.purpleTextHue, "UOACZ has ended.");
                }

                UOACZPersistance.EndSession();
                return;
            }
        }

        [Usage("UOACZMap1")]
        [Description("Set UOACZ Map 1 Active")]
        public static void UOACZMap1(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            UOACZPersistance.CustomRegion = UOACZCustomRegion.Malas1;

            player.Say("Coords: " + UOACZRegion.UOACZRegionRectangle.ToString());
            player.Say("Facet: " + UOACZRegion.Facet.ToString());
        }

        [Usage("UOACZMap2")]
        [Description("Set UOACZ Map 2 Active")]
        public static void UOACZMap2(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            UOACZPersistance.CustomRegion = UOACZCustomRegion.Terminus1;

            player.Say("Coords: " + UOACZRegion.UOACZRegionRectangle.ToString());
            player.Say("Facet: " + UOACZRegion.Facet.ToString());
        }

        [Usage("UOACZSpawnAllWildernessUndead")]
        [Description("Spawns All Available Undead")]
        public static void UOACZSpawnAllWildernessUndead(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            if (!UOACZPersistance.Active)
            {
                player.SendMessage("UOACZ is not currently active");
                return;
            }

            UOACZSpawnAllUndead(true);

            player.SendMessage("All available wilderness undead spawned.");
        }

        [Usage("UOACZSpawnAllTownUndead")]
        [Description("Spawns All Available Undead")]
        public static void UOACZSpawnAllTownUndead(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            if (!UOACZPersistance.Active)
            {
                player.SendMessage("UOACZ is not currently active");
                return;
            }

            UOACZSpawnAllUndead(false);

            player.SendMessage("All available town undead spawned.");
        }

        public static void UOACZSpawnAllUndead(bool wilderness)
        {
            foreach (UOACZUndeadSpawner undeadSpawner in UOACZUndeadSpawner.m_Spawners)
            {
                if (!UOACZRegion.ContainsItem(undeadSpawner))
                    continue;

                if (wilderness != undeadSpawner.Wilderness)
                    continue;

                int amountToSpawn = 0;

                double spawnDelay = 0;

                amountToSpawn = undeadSpawner.GetAvailableSpawnAmount();

                if (amountToSpawn > 0)
                {
                    undeadSpawner.PerformSpawns(amountToSpawn);

                    undeadSpawner.m_LastActivity = DateTime.UtcNow;
                    spawnDelay = undeadSpawner.MinSpawnTime + (Utility.RandomDouble() * (undeadSpawner.MaxSpawnTime - undeadSpawner.MinSpawnTime));
                    undeadSpawner.m_NextActivity = TimeSpan.FromMinutes(spawnDelay);
                }
            }
        }

        [Usage("H <text>")]
        [Description("UOACZ Human-Only Chat.")]
        public static void UOACZHumanChat(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            if (player == null) return;
            if (!UOACZPersistance.Active)
            {
                player.SendMessage("UOACZ is not currently in progress.");
                return;
            }

            if (!(player.IsUOACZHuman || player.AccessLevel > AccessLevel.Player))
            {
                player.SendMessage("Only Human players may use this.");
                return;
            }

            string message = "[UOACZ Human: " + player.Name + "] " + e.ArgString;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            foreach (NetState state in NetState.Instances)
            {
                PlayerMobile pm_Mobile = state.Mobile as PlayerMobile;

                if (pm_Mobile == null)
                    continue;

                UOACZPersistance.CheckAndCreateUOACZAccountEntry(pm_Mobile);

                if (pm_Mobile.IsUOACZHuman || pm_Mobile.AccessLevel > AccessLevel.Player)
                {
                    if (pm_Mobile.m_UOACZAccountEntry.ShowHumanChat)
                        pm_Mobile.SendMessage(UOACZSystem.orangeTextHue, message);
                }
            }
        }

        [Usage("U <text>")]
        [Description("UOACZ Undead-Only Chat.")]
        public static void UOACZUndeadChat(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            if (player == null) return;
            if (!UOACZPersistance.Active)
            {
                player.SendMessage("UOACZ is not currently in progress.");
                return;
            }

            if (!(player.IsUOACZUndead || player.AccessLevel > AccessLevel.Player))
            {
                player.SendMessage("Only Undead players may use this.");
                return;
            }

            string message = "[UOACZ Undead: " + player.Name + "] " + e.ArgString;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            foreach (NetState state in NetState.Instances)
            {
                PlayerMobile pm_Mobile = state.Mobile as PlayerMobile;

                if (pm_Mobile == null)
                    continue;

                UOACZPersistance.CheckAndCreateUOACZAccountEntry(pm_Mobile);

                if (pm_Mobile.IsUOACZUndead || pm_Mobile.AccessLevel > AccessLevel.Player)
                {
                    if (pm_Mobile.m_UOACZAccountEntry.ShowUndeadChat)
                        pm_Mobile.SendMessage(UOACZSystem.orangeTextHue, message);
                }
            }
        }

        [Usage("UOACZShowHumanChat")]
        [Description("Toggles displaying of UOACZ Human Chat Channel messages")]
        public static void UOACZShowHumanChat(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            if (player.m_UOACZAccountEntry.ShowHumanChat)
            {
                player.SendMessage("Display UOACZ Human Chat Channel Messages: Disabled.");
                player.m_UOACZAccountEntry.ShowHumanChat = false;
            }

            else
            {
                player.SendMessage("Display UOACZ Human Chat Channel Messages: Enabled.");
                player.m_UOACZAccountEntry.ShowHumanChat = true;
            }
        }

        [Usage("UOACZShowUndeadChat")]
        [Description("Toggles displaying of UOACZ Undead Chat Channel messages")]
        public static void UOACZShowUndeadChat(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            if (player.m_UOACZAccountEntry.ShowUndeadChat)
            {
                player.SendMessage("Display UOACZ Undead Chat Channel Messages: Disabled.");
                player.m_UOACZAccountEntry.ShowUndeadChat = false;
            }

            else
            {
                player.SendMessage("Display UOACZ Undead Chat Channel Messages: Enabled.");
                player.m_UOACZAccountEntry.ShowUndeadChat = true;
            }

        }

        [Usage("UOACZShowSurvivalTome")]
        [Description("Opens the UOACZ Survival Tome Gump")]
        public static void UOACZShowSurvivalTome(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            player.CloseGump(typeof(HumanProfileGump));
            player.SendGump(new HumanProfileGump(player));
        }

        [Usage("UOACZShowCorruptionTome")]
        [Description("Opens the UOACZ Corruption Tome Gump")]
        public static void UOACZShowCorruptionTome(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            player.CloseGump(typeof(UndeadProfileGump));
            player.SendGump(new UndeadProfileGump(player));
        }

        [Usage("UOACZShowHumanStats")]
        [Description("Opens the UOACZ Human Stats Gump")]
        public static void UOACZShowHumanStats(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            player.CloseGump(typeof(HumanProfileStatsHotbarGump));
            player.SendGump(new HumanProfileStatsHotbarGump(player));
        }

        [Usage("UOACZShowUndeadStats")]
        [Description("Opens the UOACZ Undead Stats Gump")]
        public static void UOACZShowUndeadStats(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            player.CloseGump(typeof(UndeadProfileStatsHotbarGump));
            player.SendGump(new UndeadProfileStatsHotbarGump(player));
        }

        [Usage("UOACZShowHumanHotbar")]
        [Description("Opens the UOACZ Human Abilities Hotbar")]
        public static void UOACZShowHumanHotbar(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            player.CloseGump(typeof(HumanProfileAbilitiesHotbarGump));
            player.SendGump(new HumanProfileAbilitiesHotbarGump(player));
        }

        [Usage("UOACZShowUndeadHotbar")]
        [Description("Opens the UOACZ Undead Stats Gump")]
        public static void UOACZShowUndeadHotbar(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            player.CloseGump(typeof(UndeadProfileAbilitiesHotbarGump));
            player.SendGump(new UndeadProfileAbilitiesHotbarGump(player));
        }

        [Usage("UOACZShowObjectives")]
        [Description("Shows the UOACZ Objectives Gump")]
        public static void UOACZShowObjectives(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            player.CloseGump(typeof(ObjectivesHotbarGump));
            player.SendGump(new ObjectivesHotbarGump(player));
        }

        [Usage("UOACZAbility1")]
        [Description("Uses the UOACZ Ability in 1st Hotbar Slot")]
        public static void UOACZAbility1(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            UOACZSystem.CommandActivateAbility(player, 0);
        }

        [Usage("UOACZAbility2")]
        [Description("Uses the UOACZ Ability in 2nd Hotbar Slot")]
        public static void UOACZAbility2(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            UOACZSystem.CommandActivateAbility(player, 1);
        }

        [Usage("UOACZAbility3")]
        [Description("Uses the UOACZ Ability in 3rd Hotbar Slot")]
        public static void UOACZAbility3(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            UOACZSystem.CommandActivateAbility(player, 2);
        }

        [Usage("UOACZAbility4")]
        [Description("Uses the UOACZ Ability in 4th Hotbar Slot")]
        public static void UOACZAbility4(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            UOACZSystem.CommandActivateAbility(player, 3);
        }

        [Usage("UOACZAbility5")]
        [Description("Uses the UOACZ Ability in 5th Hotbar Slot")]
        public static void UOACZAbility5(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            UOACZSystem.CommandActivateAbility(player, 4);
        }

        [Usage("UOACZHotbarLeft")]
        [Description("Navigates to the previous page of the UOACZ Hotbar")]
        public static void UOACZHotbarLeft(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            UOACZSystem.NavigateHotbar(player, -1);
        }

        [Usage("UOACZHotbarRight")]
        [Description("Navigates to the next page of the UOACZ Hotbar")]
        public static void UOACZHotbarRight(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            UOACZSystem.NavigateHotbar(player, 1);
        }

        [Usage("UOACZResetCooldowns")]
        [Description("Resets all ability cooldowns")]
        public static void UOACZResetCooldowns(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            if (player.m_UOACZAccountEntry.ActiveProfile == UOACZAccountEntry.ActiveProfileType.Human)
            {
                foreach (UOACZHumanAbilityEntry entry in player.m_UOACZAccountEntry.HumanProfile.m_Abilities)
                {
                    entry.m_NextUsageAllowed = DateTime.UtcNow;
                }

                UOACZSystem.RefreshAllGumps(player);

                player.SendMessage("Human ability cooldowns reset.");
            }

            if (player.m_UOACZAccountEntry.ActiveProfile == UOACZAccountEntry.ActiveProfileType.Undead)
            {
                foreach (UOACZUndeadAbilityEntry entry in player.m_UOACZAccountEntry.UndeadProfile.m_Abilities)
                {
                    entry.m_NextUsageAllowed = DateTime.UtcNow;
                }

                UOACZSystem.RefreshAllGumps(player);

                player.SendMessage("Undead ability cooldowns reset.");
            }
        }

        [Usage("UOACZFullStats")]
        [Description("Sets Hits, Stam, Mana to Full Values")]
        public static void UOACZFullStats(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            player.Hits = player.HitsMax;
            player.Stam = player.StamMax;
            player.Mana = player.ManaMax;

            player.SendMessage("Stats set to full values.");
        }

        [Usage("UOACZBoost")]
        [Description("Grants 1000 Human Upgrade / Undead Upgrade / Survival / Corruption Points")]
        public static void UOACZBoost(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            player.m_UOACZAccountEntry.HumanProfile.UpgradePoints = 1000;
            player.m_UOACZAccountEntry.HumanProfile.SurvivalPoints = 1000;

            player.m_UOACZAccountEntry.UndeadProfile.UpgradePoints = 1000;
            player.m_UOACZAccountEntry.UndeadProfile.CorruptionPoints = 1000;

            player.SendMessage("UOACZ upgrades / shuffles maxed");
        }

        #endregion

        public static void ChangeRegion()
        {
            Region targetRegion = null;

            foreach (Region region in Region.Regions)
            {
                if (region is UOACZRegion)
                    targetRegion = region;
            }

            if (targetRegion != null)
                targetRegion.Unregister();

            switch (m_CustomRegion)
            {
                case UOACZCustomRegion.Malas1:
                    UOACZRegion.UOACZRegionRectangle = new Rectangle2D(new Point2D(525, 0), new Point2D(2550, 2040));
                    UOACZRegion.Facet = Map.Malas;

                    DefaultHumanLocation = new Point3D(2033, 1333, -88);
                    DefaultUndeadLocation = new Point3D(1823, 1300, -90);
                    DefaultMap = Map.Malas;
                    break;

                case UOACZCustomRegion.Terminus1:
                    UOACZRegion.UOACZRegionRectangle = new Rectangle2D(new Point2D(6198, 1585), new Point2D(6621, 2000));
                    UOACZRegion.Facet = Map.Felucca;

                    DefaultHumanLocation = new Point3D(6428, 1923, 15);
                    DefaultUndeadLocation = new Point3D(6279, 1625, 0);
                    DefaultMap = Map.Felucca;
                    break;
            }

            UOACZRegion m_UOACZRegion = new UOACZRegion("UOACZRegion", UOACZRegion.Facet, 10, new Rectangle2D[] { UOACZRegion.UOACZRegionRectangle });
            m_UOACZRegion.Register();
        }

        public class UOACZTimer : Timer
        {
            public UOACZTimer(): base(TimeSpan.Zero, TimeSpan.FromSeconds(5))
            {
                m_MinuteOccurred = DateTime.UtcNow + TimeSpan.FromMinutes(1);

                Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                if (!UOACZPersistance.Enabled)
                {
                    return;
                }

                if (UOACZPersistance.Active)
                {
                    if (DateTime.UtcNow >= UOACZPersistance.m_CurrentSessionExpiration)
                    {
                        if (!m_SessionEndOccuring)
                        {
                            m_SessionEndOccuring = true;

                            foreach (NetState state in NetState.Instances)
                            {
                                Mobile mobile = state.Mobile;
                                PlayerMobile player = mobile as PlayerMobile;

                                if (player == null)
                                    continue;

                                player.SendMessage(UOACZSystem.purpleTextHue, "UOACZ has ended. The result is a tie.");
                            }

                            Timer.DelayCall(TimeSpan.FromSeconds(5), delegate
                            {
                                if (Active)
                                {
                                    EndSession();
                                    return;
                                }
                            });
                        }

                        return;
                    }

                    bool minuteOccurred = false;

                    if (DateTime.UtcNow >= m_MinuteOccurred)
                    {
                        minuteOccurred = true;
                        m_MinuteOccurred = DateTime.UtcNow + TimeSpan.FromMinutes(1);

                        m_MinutesActive++;
                        m_ThreatLevelProgress++;

                        m_SpawnWaveProgress++;

                        if (m_ThreatLevelProgress >= UOACZSystem.MinutesPerThreatLevelIncrease)
                        {
                            m_ThreatLevelProgress = 0;
                            m_ThreatLevel++;
                        }

                        UOACZEvents.MinuteTick();
                    }

                    UOACZSystem.TimerTick(minuteOccurred);
                }

                else
                {
                    if (DateTime.UtcNow >= UOACZPersistance.m_NextScheduledSessionStartTime)
                    {
                        UOACZPersistance.m_NextScheduledSessionStartTime = UOACZPersistance.m_NextScheduledSessionStartTime + ScheduledSessionInterval;
                        UOACZPersistance.m_AnnouncementMade = false;

                        StartSession();
                        return;
                    }

                    if (!UOACZPersistance.m_AnnouncementMade && DateTime.UtcNow >= UOACZPersistance.m_NextScheduledSessionStartTime - UOACZPersistance.ScheduledSessionAnnouncementLeadtime)
                    {
                        UOACZPersistance.m_AnnouncementMade = true;

                        foreach (NetState state in NetState.Instances)
                        {
                            PlayerMobile player = state.Mobile as PlayerMobile;

                            if (player == null)
                                continue;

                            string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, DateTime.UtcNow + UOACZPersistance.ScheduledSessionAnnouncementLeadtime, false, false, true, true, true);

                            player.SendMessage(UOACZSystem.purpleTextHue, "UOACZ will begin in " + timeRemaining + ".");
                        }
                    }
                }
            }
        }

        public static void StartSession()
        {
            if (Active)
                return;

            Active = true;

            m_CurrentSessionStartTime = DateTime.UtcNow;
            m_CurrentSessionExpiration = DateTime.UtcNow + SessionDuration;
            m_SessionEndOccuring = false;

            m_MinutesActive = 0;

            m_ThreatLevelProgress = 0;
            m_ThreatLevel = UOACZSystem.StartingThreatLevel;

            m_SpawnWaveProgress = 0;

            HumanBalanceScalar = 1.0;
            UndeadBalanceScalar = 1.0;

            HumanObjectivesComplete = 0;

            m_HumanObjective1 = 0;
            m_HumanObjective2 = 0;
            m_HumanObjective3 = 0;
            m_HumanObjective4 = 0;
            m_HumanObjective5 = 0;

            UndeadObjectivesComplete = 0;

            m_UndeadObjective1 = 0;
            m_UndeadObjective2 = 0;
            m_UndeadObjective3 = 0;
            m_UndeadObjective4 = 0;
            m_UndeadObjective5 = 0;

            m_HumanObjective1Target = 1;
            m_HumanObjective2Target = 1;
            m_HumanObjective3Target = 1;
            m_HumanObjective4Target = 1;
            m_HumanObjective5Target = 1;

            m_UndeadObjective1Target = 1;
            m_UndeadObjective2Target = 1;
            m_UndeadObjective3Target = 1;
            m_UndeadObjective4Target = 1;
            m_UndeadObjective5Target = 1;

            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }

            m_Timer = new UOACZTimer();
            m_Timer.Start();

            UOACZSystem.SessionBegin();
        }

        public static void EndSession()
        {
            if (!Active)
                return;

            UOACZPersistance.m_SessionEndOccuring = false;

            UOACZSystem.SessionComplete();
        }

        public static void ResetProfile(PlayerMobile player)
        {
            if (player == null)
                return;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            ResetProfile(player.m_UOACZAccountEntry);
        }

        public static void ResetProfile(UOACZAccountEntry entry)
        {
            if (entry == null)
                return;

            Queue m_Queue = new Queue();

            //Delete Stored UOACZ Items
            CheckCreateBackpacks(entry);

            List<Item> m_HumanBackpackItems = entry.m_UOACZHumanBackpackItems.Items;
            List<Item> m_HumanWornItems = entry.m_UOACZHumanWornItems.Items;

            List<Item> m_UndeadBackpackItems = entry.m_UOACZUndeadBackpackItems.Items;
            List<Item> m_UndeadWornItems = entry.m_UOACZUndeadWornItems.Items;

            foreach (Item item in m_HumanBackpackItems)
            {
                m_Queue.Enqueue(item);
            }

            foreach (Item item in m_HumanWornItems)
            {
                m_Queue.Enqueue(item);
            }

            while (m_Queue.Count > 0)
            {
                Item item = (Item)m_Queue.Dequeue();
                item.Delete();
            }

            foreach (Item item in m_UndeadBackpackItems)
            {
                m_Queue.Enqueue(item);
            }

            foreach (Item item in m_UndeadWornItems)
            {
                m_Queue.Enqueue(item);
            }

            while (m_Queue.Count > 0)
            {
                Item item = (Item)m_Queue.Dequeue();
                item.Delete();
            }

            //Human Profile
            if (entry.HumanProfile.Stockpile == null)
                entry.HumanProfile.Stockpile = new UOACZStockpileContainer(entry.HumanProfile.AccountName);

            else if (entry.HumanProfile.Stockpile.Deleted)
                entry.HumanProfile.Stockpile = new UOACZStockpileContainer(entry.HumanProfile.AccountName);

            else
            {
                //Delete Stockpile Items
                List<Item> m_Items = entry.HumanProfile.Stockpile.Items;

                foreach (Item item in m_Items)
                {
                    m_Queue.Enqueue(item);
                }

                while (m_Queue.Count > 0)
                {
                    Item item = (Item)m_Queue.Dequeue();
                    item.Delete();
                }
            }

            entry.InitialHumanItemsGenerated = false;
            entry.InitialUndeadItemsGenerated = false;
            entry.FatigueExpiration = DateTime.UtcNow;

            entry.HumanProfile.Stockpile.MaxItems = 15;

            entry.HumanProfile.CauseOfDeath = UOACZAccountEntry.HumanProfileEntry.CauseOfDeathType.Misc;
            entry.HumanProfile.ActivePage = UOACZAccountEntry.HumanProfileEntry.HumanActivePageType.Abilities;

            entry.HumanProfile.AbilitiesHotbarPageNumber = 1;
            entry.HumanProfile.LeftPageNumber = 1;
            entry.HumanProfile.RightPageNumber = 1;
            entry.HumanProfile.RadialSelectionNumber = 0;

            entry.HumanProfile.FollowersMax = 0;

            entry.HumanProfile.SurvivalPoints = UOACZSystem.HumanStartingSurvivalPoints;
            entry.HumanProfile.UpgradePoints = UOACZSystem.HumanStartingUpgradePoints;

            entry.HumanProfile.HungerPoints = 100;
            entry.HumanProfile.MaxHungerPoints = 100;

            entry.HumanProfile.ThirstPoints = 100;
            entry.HumanProfile.MaxThirstPoints = 100;

            entry.HumanProfile.HumanityPoints = 100;
            entry.HumanProfile.MaxHumanityPoints = 100;

            entry.HumanProfile.MaxHonorPoints = 100;
            entry.HumanProfile.HonorPoints = 100;

            entry.HumanProfile.TotalMinutesAlive = 0;
            entry.HumanProfile.UpgradeMinutes = 0;
            entry.HumanProfile.UpgradesSpent = 0;
            entry.HumanProfile.ShufflesSpent = 0;
            entry.HumanProfile.Deaths = 0;

            entry.HumanProfile.m_UpgradesAvailable = new List<UOACZHumanUpgradeType>();
            UOACZHumanUpgrades.ShuffleAvailableUpgrades(entry);

            entry.HumanProfile.m_Upgrades = new List<UOACZHumanUpgradeEntry>();
            entry.HumanProfile.m_Abilities = new List<UOACZHumanAbilityEntry>();

            //Escape Ability          
            UOACZHumanAbilityType abilityType = UOACZHumanAbilityType.Escape;
            UOACZHumanAbilityDetail abilityDetail = UOACZHumanAbilities.GetAbilityDetail(abilityType);
            entry.HumanProfile.m_Abilities.Add(new UOACZHumanAbilityEntry(abilityType, 1, abilityDetail.CooldownMinutes, DateTime.UtcNow));

            //Phalanx Ability          
            abilityType = UOACZHumanAbilityType.Phalanx;
            abilityDetail = UOACZHumanAbilities.GetAbilityDetail(abilityType);
            entry.HumanProfile.m_Abilities.Add(new UOACZHumanAbilityEntry(abilityType, 1, abilityDetail.CooldownMinutes, DateTime.UtcNow));

            //Toolkit Ability          
            abilityType = UOACZHumanAbilityType.Toolkit;
            abilityDetail = UOACZHumanAbilities.GetAbilityDetail(abilityType);
            entry.HumanProfile.m_Abilities.Add(new UOACZHumanAbilityEntry(abilityType, 1, abilityDetail.CooldownMinutes, DateTime.UtcNow));

            entry.HumanProfile.m_Stats = new Dictionary<StatType, int>();

            entry.HumanProfile.m_Stats.Add(StatType.Str, 75);
            entry.HumanProfile.m_Stats.Add(StatType.Dex, 50);
            entry.HumanProfile.m_Stats.Add(StatType.Int, 25);

            entry.HumanProfile.m_Skills = new Dictionary<SkillName, double>();

            int skillsCount = Enum.GetNames(typeof(SkillName)).Length;

            for (int a = 0; a < skillsCount; a++)
            {
                entry.HumanProfile.m_Skills.Add((SkillName)a, 0);
            }

            entry.HumanProfile.m_Skills[SkillName.Tactics] = 80;
            entry.HumanProfile.m_Skills[SkillName.Wrestling] = 80;

            entry.HumanProfile.m_Skills[SkillName.Archery] = 60;
            entry.HumanProfile.m_Skills[SkillName.Fencing] = 60;
            entry.HumanProfile.m_Skills[SkillName.Swords] = 60;
            entry.HumanProfile.m_Skills[SkillName.Macing] = 60;            

            entry.HumanProfile.m_Skills[SkillName.Healing] = 40;
            entry.HumanProfile.m_Skills[SkillName.Anatomy] = 40;
            entry.HumanProfile.m_Skills[SkillName.Parry] = 40;
            entry.HumanProfile.m_Skills[SkillName.ArmsLore] = 40;
            entry.HumanProfile.m_Skills[SkillName.MagicResist] = 40;

            entry.HumanProfile.m_Skills[SkillName.Lumberjacking] = 20;
            entry.HumanProfile.m_Skills[SkillName.Mining] = 20;
            entry.HumanProfile.m_Skills[SkillName.Fishing] = 20;
            entry.HumanProfile.m_Skills[SkillName.Camping] = 20;

            entry.HumanProfile.m_GMCraftingSkills = new List<SkillName>();

            //Undead Profile        
            entry.UndeadProfile.ActivePage = UOACZAccountEntry.UndeadProfileEntry.UndeadActivePageType.Abilities;

            entry.UndeadProfile.m_Abilities = new List<UOACZUndeadAbilityEntry>();

            entry.UndeadProfile.AbilitiesHotbarPageNumber = 1;
            entry.UndeadProfile.LeftPageNumber = 1;
            entry.UndeadProfile.RightPageNumber = 1;

            entry.UndeadProfile.MonsterTier = 1;
            entry.UndeadProfile.PostTier5Upgrades = 0;

            entry.UndeadProfile.CorruptionPoints = UOACZSystem.UndeadStartingCorruptionPoints;
            entry.UndeadProfile.UpgradePoints = UOACZSystem.UndeadStartingUpgradePoints;

            entry.UndeadProfile.DyedHueMod = -1;

            UOACZUndeadUpgradeType upgradeType = UOACZUndeadUpgradeType.Zombie;

            switch (Utility.RandomMinMax(1, 7))
            {
                case 1: upgradeType = UOACZUndeadUpgradeType.Zombie; break;
                case 2: upgradeType = UOACZUndeadUpgradeType.Skeleton; break;
                case 3: upgradeType = UOACZUndeadUpgradeType.Ghoul; break;
                case 4: upgradeType = UOACZUndeadUpgradeType.Spectre; break;
                case 5: upgradeType = UOACZUndeadUpgradeType.Necromancer; break;
                case 6: upgradeType = UOACZUndeadUpgradeType.VoidSlime; break;
                case 7: upgradeType = UOACZUndeadUpgradeType.SkeletalCritter; break;
            }

            UOACZUndeadUpgrades.SetActiveForm(entry, upgradeType);

            entry.UndeadProfile.TotalMinutesUndead = 0;
            entry.UndeadProfile.UpgradeMinutes = 0;
            entry.UndeadProfile.UpgradesSpent = 0;
            entry.UndeadProfile.ShufflesSpent = 0;
            entry.UndeadProfile.Deaths = 0;

            entry.UndeadProfile.m_FormsKilledWith = new List<UOACZUndeadUpgradeType>();
        }

        public static void CheckAndCreateUOACZAccountEntry(PlayerMobile player)
        {
            if (player == null) return;
            if (player.Deleted) return;

            if (player.m_UOACZAccountEntry == null)
                CreateUOACZAccountEntry(player);

            if (player.m_UOACZAccountEntry.Deleted)
                CreateUOACZAccountEntry(player);

            Account account = player.Account as Account;

            if (account == null)
                return;

            string accountName = account.Username;

            if (player.m_UOACZAccountEntry.HumanProfile == null)
                player.m_UOACZAccountEntry.HumanProfile = new UOACZAccountEntry.HumanProfileEntry(accountName);

            if (player.m_UOACZAccountEntry.HumanProfile.Deleted)
                player.m_UOACZAccountEntry.HumanProfile = new UOACZAccountEntry.HumanProfileEntry(accountName);

            if (player.m_UOACZAccountEntry.UndeadProfile == null)
                player.m_UOACZAccountEntry.UndeadProfile = new UOACZAccountEntry.UndeadProfileEntry(accountName);

            if (player.m_UOACZAccountEntry.UndeadProfile.Deleted)
                player.m_UOACZAccountEntry.UndeadProfile = new UOACZAccountEntry.UndeadProfileEntry(accountName);

            CheckCreateBackpacks(player.m_UOACZAccountEntry);
        }

        public static void CheckCreateBackpacks(UOACZAccountEntry entry)
        {
            if (entry == null)
                return;

            if (entry.m_UOACZHumanBackpackItems == null)
                entry.m_UOACZHumanBackpackItems = new Backpack();

            else if (entry.m_UOACZHumanBackpackItems.Deleted)
                entry.m_UOACZHumanBackpackItems = new Backpack();

            entry.m_UOACZHumanBackpackItems.MaxItems = 500;

            if (entry.m_UOACZHumanWornItems == null)
                entry.m_UOACZHumanWornItems = new Backpack();

            else if (entry.m_UOACZHumanWornItems.Deleted)
                entry.m_UOACZHumanWornItems = new Backpack();

            entry.m_UOACZHumanWornItems.MaxItems = 500;

            if (entry.m_UOACZUndeadBackpackItems == null)
                entry.m_UOACZUndeadBackpackItems = new Backpack();

            else if (entry.m_UOACZUndeadBackpackItems.Deleted)
                entry.m_UOACZUndeadBackpackItems = new Backpack();

            entry.m_UOACZUndeadBackpackItems.MaxItems = 500;

            if (entry.m_UOACZUndeadWornItems == null)
                entry.m_UOACZUndeadWornItems = new Backpack();

            else if (entry.m_UOACZUndeadWornItems.Deleted)
                entry.m_UOACZUndeadWornItems = new Backpack();

            entry.m_UOACZUndeadWornItems.MaxItems = 500;
        }

        public static void CreateUOACZAccountEntry(PlayerMobile player)
        {
            if (player == null) return;
            if (player.Account == null) return;

            string accountName = player.Account.Username;

            UOACZAccountEntry UOACZAccountEntry = null;

            foreach (UOACZAccountEntry entry in m_UOACZAccountEntries)
            {
                if (entry.m_AccountUsername == accountName && !entry.Deleted)
                {
                    player.m_UOACZAccountEntry = entry;
                    return;
                }
            }

            UOACZAccountEntry newEntry = new UOACZAccountEntry(accountName);

            Account account = player.Account as Account;

            for (int i = 0; i < (account.Length - 1); i++)
            {
                Mobile mobile = account.accountMobiles[i] as Mobile;

                if (mobile != null)
                {
                    PlayerMobile pm_Mobile = mobile as PlayerMobile;

                    if (pm_Mobile != null)
                    {
                        pm_Mobile.m_UOACZAccountEntry = newEntry;
                        ResetProfile(player);
                    }
                }
            }
        }

        public static UOACZAccountEntry FindUOACZAccountEntryByAccountName(string accountName)
        {
            foreach (UOACZAccountEntry entry in m_UOACZAccountEntries)
            {
                if (entry.m_AccountUsername == accountName && !entry.Deleted)
                {
                    return entry;
                }
            }

            return null;
        }

        public static UOACZAccountEntry FindUOACZAccountEntry(PlayerMobile player)
        {
            if (player == null) return null;
            if (player.Account == null) return null;

            string accountName = player.Account.Username;

            UOACZAccountEntry UOACZAccountEntry = null;

            foreach (UOACZAccountEntry entry in m_UOACZAccountEntries)
            {
                if (entry.m_AccountUsername == accountName && !entry.Deleted)
                {
                    UOACZAccountEntry = entry;
                    break;
                }
            }

            return UOACZAccountEntry;
        }

        public static UOACZCharacterSnapshot FindCharacterSnapshot(PlayerMobile player)
        {
            if (player == null)
                return null;

            foreach (UOACZCharacterSnapshot snapshot in m_UOACZCharacterSnapshots)
            {
                if (snapshot == null) continue;
                if (snapshot.Deleted) continue;

                if (snapshot.m_Player == player)
                    return snapshot;
            }

            return null;
        }

        public static void StoreCharacterSnapshot(PlayerMobile player)
        {
            if (player == null) return;
            if (player.Account == null) return;

            UOACZCharacterSnapshot snapshot = FindCharacterSnapshot(player);

            if (snapshot == null)
                snapshot = new UOACZCharacterSnapshot(player.Account.Username, player, player.HueMod, player.FollowersMax);

            else if (snapshot.Deleted)
                snapshot = new UOACZCharacterSnapshot(player.Account.Username, player, player.HueMod, player.FollowersMax);
            
            player.HideSkillUpdates = true;
        }

        public static void RestoreCharacterSnapshot(PlayerMobile player)
        {
            if (player == null)
                return;

            UOACZCharacterSnapshot snapshot = FindCharacterSnapshot(player);

            if (snapshot == null)
                return;

            player.FollowersMax = snapshot.m_FollowersMax;
            player.HueMod = snapshot.m_HueMod;

            player.HideSkillUpdates = false;

            if (!snapshot.Deleted)
                snapshot.Delete();
        }

        public static void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(5); //Version

            //Version 0   
            writer.Write(m_Enabled);
            writer.Write(m_Active);
            writer.Write(m_NextScheduledSessionStartTime);
            writer.Write(m_AnnouncementMade);
            writer.Write(m_CurrentSessionStartTime);
            writer.Write(m_CurrentSessionExpiration);
            writer.Write(m_SessionEndOccuring);
            writer.Write(m_MinutesActive);

            writer.Write(m_StockpileProgress.Count);
            foreach (KeyValuePair<UOACZSystem.StockpileContributionType, double> pair in m_StockpileProgress)
            {
                writer.Write((int)pair.Key);
                writer.Write(pair.Value);
            }

            //Version 1
            writer.Write(m_ThreatLevelProgress);
            writer.Write(m_ThreatLevel);

            //Version 2

            writer.Write((int)m_CustomRegion);

            //Version 3
            writer.Write(m_SpawnWaveProgress);

            //Version 4
            writer.Write(HumanObjectivesComplete);

            writer.Write(m_HumanObjective1);
            writer.Write(m_HumanObjective2);
            writer.Write(m_HumanObjective3);
            writer.Write(m_HumanObjective4);
            writer.Write(m_HumanObjective5);

            writer.Write(UndeadObjectivesComplete);

            writer.Write(m_UndeadObjective1);
            writer.Write(m_UndeadObjective2);
            writer.Write(m_UndeadObjective3);
            writer.Write(m_UndeadObjective4);
            writer.Write(m_UndeadObjective5);

            writer.Write(m_HumanObjective1Target);
            writer.Write(m_HumanObjective2Target);
            writer.Write(m_HumanObjective3Target);
            writer.Write(m_HumanObjective4Target);
            writer.Write(m_HumanObjective5Target);

            writer.Write(m_UndeadObjective1Target);
            writer.Write(m_UndeadObjective2Target);
            writer.Write(m_UndeadObjective3Target);
            writer.Write(m_UndeadObjective4Target);
            writer.Write(m_UndeadObjective5Target);

            writer.Write(m_OutpostComponents.Count);
            for (int a = 0; a < m_OutpostComponents.Count; a++)
            {
                writer.Write(m_OutpostComponents[a]);
            }

            writer.Write(HumanChampion);
            writer.Write(HumanBoss);
            writer.Write(UndeadChampion);
            writer.Write(UndeadBoss);

            writer.Write(HumanChampionLocation);
            writer.Write(HumanBossLocation);
            writer.Write(UndeadChampionLocation);
            writer.Write(UndeadBossLocation);

            writer.Write(m_PreviousHumanScores.Count);
            for (int a = 0; a < m_PreviousHumanScores.Count; a++)
            {
                writer.Write(m_PreviousHumanScores[a]);
            }

            writer.Write(m_PreviousUndeadScores.Count);
            for (int a = 0; a < m_PreviousUndeadScores.Count; a++)
            {
                writer.Write(m_PreviousUndeadScores[a]);
            }

            writer.Write(m_PreviousTotalScores.Count);
            for (int a = 0; a < m_PreviousTotalScores.Count; a++)
            {
                writer.Write(m_PreviousTotalScores[a]);
            }

            writer.Write(m_BestHumanScores.Count);
            for (int a = 0; a < m_BestHumanScores.Count; a++)
            {
                writer.Write(m_BestHumanScores[a]);
            }

            writer.Write(m_BestUndeadScores.Count);
            for (int a = 0; a < m_BestUndeadScores.Count; a++)
            {
                writer.Write(m_BestUndeadScores[a]);
            }

            writer.Write(m_BestTotalScores.Count);
            for (int a = 0; a < m_BestTotalScores.Count; a++)
            {
                writer.Write(m_BestTotalScores[a]);
            }

            writer.Write(m_LifetimeHumanScores.Count);
            for (int a = 0; a < m_LifetimeHumanScores.Count; a++)
            {
                writer.Write(m_LifetimeHumanScores[a]);
            }

            writer.Write(m_LifetimeUndeadScores.Count);
            for (int a = 0; a < m_LifetimeUndeadScores.Count; a++)
            {
                writer.Write(m_LifetimeUndeadScores[a]);
            }

            writer.Write(m_LifetimeTotalScores.Count);
            for (int a = 0; a < m_LifetimeTotalScores.Count; a++)
            {
                writer.Write(m_LifetimeTotalScores[a]);
            }

            //Version 5
            writer.Write(HumanBalanceScalar);
            writer.Write(UndeadBalanceScalar);
        }

        public static void Deserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            //Version 0
            if (version >= 0)
            {
                m_Enabled = reader.ReadBool();
                m_Active = reader.ReadBool();

                m_NextScheduledSessionStartTime = reader.ReadDateTime();
                m_AnnouncementMade = reader.ReadBool();
                m_CurrentSessionStartTime = reader.ReadDateTime();
                m_CurrentSessionExpiration = reader.ReadDateTime();
                m_SessionEndOccuring = reader.ReadBool();

                m_MinutesActive = reader.ReadInt();

                int civilianProgress = reader.ReadInt();
                for (int a = 0; a < civilianProgress; a++)
                {
                    UOACZSystem.StockpileContributionType type = (UOACZSystem.StockpileContributionType)reader.ReadInt();
                    double value = reader.ReadDouble();

                    m_StockpileProgress.Add(type, value);
                }
            }

            //Version 1
            if (version >= 1)
            {
                m_ThreatLevelProgress = reader.ReadInt();
                m_ThreatLevel = reader.ReadInt();
            }

            //Version 2
            if (version >= 2)
            {
                CustomRegion = (UOACZCustomRegion)reader.ReadInt();
            }

            //Version 3
            if (version >= 3)
            {
                m_SpawnWaveProgress = reader.ReadInt();
            }

            //Version 4
            if (version >= 4)
            {
                HumanObjectivesComplete = reader.ReadInt();

                m_HumanObjective1 = reader.ReadDouble();
                m_HumanObjective2 = reader.ReadDouble();
                m_HumanObjective3 = reader.ReadDouble();
                m_HumanObjective4 = reader.ReadDouble();
                m_HumanObjective5 = reader.ReadDouble();

                UndeadObjectivesComplete = reader.ReadInt();

                m_UndeadObjective1 = reader.ReadDouble();
                m_UndeadObjective2 = reader.ReadDouble();
                m_UndeadObjective3 = reader.ReadDouble();
                m_UndeadObjective4 = reader.ReadDouble();
                m_UndeadObjective5 = reader.ReadDouble();

                m_HumanObjective1Target = reader.ReadDouble();
                m_HumanObjective2Target = reader.ReadDouble();
                m_HumanObjective3Target = reader.ReadDouble();
                m_HumanObjective4Target = reader.ReadDouble();
                m_HumanObjective5Target = reader.ReadDouble();

                m_UndeadObjective1Target = reader.ReadDouble();
                m_UndeadObjective2Target = reader.ReadDouble();
                m_UndeadObjective3Target = reader.ReadDouble();
                m_UndeadObjective4Target = reader.ReadDouble();
                m_UndeadObjective5Target = reader.ReadDouble();

                int outpostComponentsCount = reader.ReadInt();
                for (int a = 0; a < outpostComponentsCount; a++)
                {
                    m_OutpostComponents.Add(reader.ReadItem() as UOACZBreakableStatic);
                }

                HumanChampion = (BaseCreature)reader.ReadMobile();
                HumanBoss = (BaseCreature)reader.ReadMobile();
                UndeadChampion = (BaseCreature)reader.ReadMobile();
                UndeadBoss = (BaseCreature)reader.ReadMobile();

                HumanChampionLocation = reader.ReadPoint3D();
                HumanBossLocation = reader.ReadPoint3D();
                UndeadChampionLocation = reader.ReadPoint3D();
                UndeadBossLocation = reader.ReadPoint3D();

                int entries = reader.ReadInt();
                for (int a = 0; a < entries; a++)
                {
                    m_PreviousHumanScores.Add((UOACZAccountEntry)reader.ReadItem());
                }

                entries = reader.ReadInt();
                for (int a = 0; a < entries; a++)
                {
                    m_PreviousUndeadScores.Add((UOACZAccountEntry)reader.ReadItem());
                }

                entries = reader.ReadInt();
                for (int a = 0; a < entries; a++)
                {
                    m_PreviousTotalScores.Add((UOACZAccountEntry)reader.ReadItem());
                }

                entries = reader.ReadInt();
                for (int a = 0; a < entries; a++)
                {
                    m_BestHumanScores.Add((UOACZAccountEntry)reader.ReadItem());
                }

                entries = reader.ReadInt();
                for (int a = 0; a < entries; a++)
                {
                    m_BestUndeadScores.Add((UOACZAccountEntry)reader.ReadItem());
                }

                entries = reader.ReadInt();
                for (int a = 0; a < entries; a++)
                {
                    m_BestTotalScores.Add((UOACZAccountEntry)reader.ReadItem());
                }

                entries = reader.ReadInt();
                for (int a = 0; a < entries; a++)
                {
                    m_LifetimeHumanScores.Add((UOACZAccountEntry)reader.ReadItem());
                }

                entries = reader.ReadInt();
                for (int a = 0; a < entries; a++)
                {
                    m_LifetimeUndeadScores.Add((UOACZAccountEntry)reader.ReadItem());
                }

                entries = reader.ReadInt();
                for (int a = 0; a < entries; a++)
                {
                    m_LifetimeTotalScores.Add((UOACZAccountEntry)reader.ReadItem());
                }
            }

            //Version 5
            if (version >= 5)
            {
                HumanBalanceScalar = reader.ReadDouble();
                UndeadBalanceScalar = reader.ReadDouble();
            }

            //-----            
        }
    }

    public class UOACZPersistanceItem : Item
    {
        public override string DefaultName { get { return "UOACZPersistance"; } }

        public UOACZPersistanceItem(): base(0x0)
        {
            Movable = false;
        }

        public UOACZPersistanceItem(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version

            UOACZPersistance.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Movable = false;

            UOACZPersistance.PersistanceItem = this;
            UOACZPersistance.Deserialize(reader);
        }
    }

    public class UOACZCharacterSnapshot : Item
    {
        public string m_AccountUsername = "";
        public PlayerMobile m_Player;
        public int m_HueMod = -1;
        public int m_FollowersMax = 5;

        public UOACZAccountEntry.ActiveProfileType m_LastActiveProfileType = UOACZAccountEntry.ActiveProfileType.Human;

        public Container m_RealWorldBackpackItems = new Backpack();
        public Container m_RealWorldWornItems = new Backpack();

        [Constructable]
        public UOACZCharacterSnapshot(string accountName, PlayerMobile player, int hueMod, int followersMax): base(0x0)
        {
            Visible = false;
            Movable = false;

            m_AccountUsername = accountName;
            m_Player = player;
            m_HueMod = hueMod;
            m_FollowersMax = followersMax;

            m_RealWorldBackpackItems.MaxItems = 500;
            m_RealWorldWornItems.MaxItems = 500;

            UOACZPersistance.m_UOACZCharacterSnapshots.Add(this);
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            m_RealWorldBackpackItems.Delete();
            m_RealWorldWornItems.Delete();

            UOACZPersistance.m_UOACZCharacterSnapshots.Remove(this);
        }

        public UOACZCharacterSnapshot(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version

            //Version 0            
            writer.Write(m_AccountUsername);
            writer.Write(m_Player);
            writer.Write(m_HueMod);
            writer.Write(m_FollowersMax);

            writer.Write((int)m_LastActiveProfileType);

            writer.Write(m_RealWorldBackpackItems);
            writer.Write(m_RealWorldWornItems);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_AccountUsername = reader.ReadString();
                m_Player = (PlayerMobile)reader.ReadMobile();
                m_HueMod = reader.ReadInt();
                m_FollowersMax = reader.ReadInt();

                m_LastActiveProfileType = (UOACZAccountEntry.ActiveProfileType)reader.ReadInt();

                m_RealWorldBackpackItems = (Backpack)reader.ReadItem();
                m_RealWorldWornItems = (Backpack)reader.ReadItem();
            }

            //--------------

            UOACZPersistance.m_UOACZCharacterSnapshots.Add(this);
        }
    }

    public class UOACZAccountEntry : Item
    {
        public enum ActiveProfileType
        {
            None,
            Human,
            Undead,
        }

        #region Properties

        public string m_AccountUsername = "";
        public ActiveProfileType ActiveProfile = ActiveProfileType.None;
        public DateTime NextEntryAllowed = DateTime.UtcNow;
        public PlayerMobile MostRecentPlayer;
        public string MostRecentPlayerString = "";

        public HumanProfileEntry HumanProfile;
        public UndeadProfileEntry UndeadProfile;

        private bool m_HumanAbilitiesHotbarDisplayed = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool HumanAbilitiesHotbarDisplayed
        {
            get { return m_HumanAbilitiesHotbarDisplayed; }
            set { m_HumanAbilitiesHotbarDisplayed = value; }
        }

        private bool m_HumanStatsHotbarDisplayed = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool HumanStatsHotbarDisplayed
        {
            get { return m_HumanStatsHotbarDisplayed; }
            set { m_HumanStatsHotbarDisplayed = value; }
        }

        private bool m_UndeadAbilitiesHotbarDisplayed = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool UndeadAbilitiesHotbarDisplayed
        {
            get { return m_UndeadAbilitiesHotbarDisplayed; }
            set { m_UndeadAbilitiesHotbarDisplayed = value; }
        }

        private bool m_UndeadStatsHotbarDisplayed = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool UndeadStatsHotbarDisplayed
        {
            get { return m_UndeadStatsHotbarDisplayed; }
            set { m_UndeadStatsHotbarDisplayed = value; }
        }

        private bool m_ObjectivesDisplayed = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool ObjectivesDisplayed
        {
            get { return m_ObjectivesDisplayed; }
            set { m_ObjectivesDisplayed = value; }
        }

        private bool m_HumanObjectivesExpanded = true;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool HumanObjectivesExpanded
        {
            get { return m_HumanObjectivesExpanded; }
            set { m_HumanObjectivesExpanded = value; }
        }

        private bool m_UndeadObjectivesExpanded = true;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool UndeadObjectivesExpanded
        {
            get { return m_UndeadObjectivesExpanded; }
            set { m_UndeadObjectivesExpanded = value; }
        }

        private bool m_InitialHumanItemsGenerated = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool InitialHumanItemsGenerated
        {
            get { return m_InitialHumanItemsGenerated; }
            set { m_InitialHumanItemsGenerated = value; }
        }

        private bool m_InitialUndeadItemsGenerated = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool InitialUndeadItemsGenerated
        {
            get { return m_InitialUndeadItemsGenerated; }
            set { m_InitialUndeadItemsGenerated = value; }
        }

        public Container m_UOACZHumanBackpackItems;
        public Container m_UOACZHumanWornItems;

        public Container m_UOACZUndeadBackpackItems;
        public Container m_UOACZUndeadWornItems;

        //Current Session Scores
        private int m_CurrentSessionHumanScore = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int CurrentSessionHumanScore
        {
            get { return m_CurrentSessionHumanScore; }
            set { m_CurrentSessionHumanScore = value; }
        }

        private int m_CurrentSessionUndeadScore = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int CurrentSessionUndeadScore
        {
            get { return m_CurrentSessionUndeadScore; }
            set { m_CurrentSessionUndeadScore = value; }
        }

        private int m_CurrentSessionTotalScore = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int CurrentSessionTotalScore
        {
            get { return m_CurrentSessionTotalScore; }
            set { m_CurrentSessionTotalScore = value; }
        }

        //Previous Session Scores
        private int m_PreviousSessionHumanScore = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int PreviousSessionHumanScore
        {
            get { return m_PreviousSessionHumanScore; }
            set { m_PreviousSessionHumanScore = value; }
        }

        private int m_PreviousSessionUndeadScore = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int PreviousSessionUndeadScore
        {
            get { return m_PreviousSessionUndeadScore; }
            set { m_PreviousSessionUndeadScore = value; }
        }

        private int m_PreviousSessionTotalScore = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int PreviousSessionTotalScore
        {
            get { return m_PreviousSessionTotalScore; }
            set { m_PreviousSessionTotalScore = value; }
        }

        //Lifetime Scores
        private int m_LifetimeHumanScore = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int LifetimeHumanScore
        {
            get { return m_LifetimeHumanScore; }
            set { m_LifetimeHumanScore = value; }
        }

        private int m_LifetimeUndeadScore = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int LifetimeUndeadScore
        {
            get { return m_LifetimeUndeadScore; }
            set { m_LifetimeUndeadScore = value; }
        }

        private int m_LifetimeTotalScore = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int LifetimeTotalScore
        {
            get { return m_LifetimeTotalScore; }
            set { m_LifetimeTotalScore = value; }
        }

        //Best Scores
        private int m_BestHumanScore = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int BestHumanScore
        {
            get { return m_BestHumanScore; }
            set { m_BestHumanScore = value; }
        }

        private int m_BestUndeadScore = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int BestUndeadScore
        {
            get { return m_BestUndeadScore; }
            set { m_BestUndeadScore = value; }
        }

        private int m_BestTotalScore = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int BestTotalScore
        {
            get { return m_BestTotalScore; }
            set { m_BestTotalScore = value; }
        }

        public enum ScorePageType
        {
            Previous,
            Best,
            Lifetime,
            RewardsTomesUnlocks,
            Admin
        }

        private ScorePageType m_ScorePage = ScorePageType.Previous;
        [CommandProperty(AccessLevel.GameMaster)]
        public ScorePageType ScorePage
        {
            get { return m_ScorePage; }
            set { m_ScorePage = value; }
        }

        private int m_ScorePageNumber = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int ScorePageNumber
        {
            get { return m_ScorePageNumber; }
            set { m_ScorePageNumber = value; }
        }

        #endregion

        #region Rewards / Unlockables

        private int m_RewardPoints = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int RewardPoints
        {
            get { return m_RewardPoints; }
            set { m_RewardPoints = value; }
        }

        private int m_RewardPage = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int RewardPage
        {
            get { return m_RewardPage; }
            set { m_RewardPage = value; }
        }

        private int m_UnlockablesPage = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int UnlockablesPage
        {
            get { return m_UnlockablesPage; }
            set { m_UnlockablesPage = value; }
        }

        private bool m_ShowHumanChat = true;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool ShowHumanChat
        {
            get { return m_ShowHumanChat; }
            set { m_ShowHumanChat = value; }
        }

        private bool m_ShowUndeadChat = true;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool ShowUndeadChat
        {
            get { return m_ShowUndeadChat; }
            set { m_ShowUndeadChat = value; }
        }

        public List<UOACZUnlockableDetailEntry> m_Unlockables = new List<UOACZUnlockableDetailEntry>();

        #endregion

        #region Extra Stats

        public int HumanVictoryCount = 0;
        public int UndeadVictoryCount = 0;
        public int HighestSessionScoreCount = 0;
        public int TotalRewardPointsEarned = 0;
        public int HumanDeaths = 0;
        public int UndeadDeaths = 0;
        public int HumanPlayersKilledAsHuman = 0;
        public int UndeadPlayersKilledAsHuman = 0;
        public int HumanPlayersKilledAsUndead = 0;
        public int UndeadKilledAsHuman = 0;
        public int CiviliansKilledAsUndead = 0;
        public int MilitiaKilledAsUndead = 0;
        public int WildlifeCorrupted = 0;
        public int HumanUpgradePointsSpentTotal = 0;
        public int HumanSurvivalPointsSpentTotal = 0;
        public int MostHumanUpgradesSpentInASession = 0;
        public int UndeadUpgradePointsSpentTotal = 0;
        public int UndeadCorruptionPointsSpentTotal = 0;
        public int LongestTimeAsHuman = 0;
        public int LongestTimeAsHumanWithoutADeath = 0;
        public int TotalTimeSpentAsHuman = 0;
        public int TotalTimeSpentAsUndead = 0;

        public int ScavengeableDebrisItems = 0;
        public int ScavengeableContainerItems = 0;
        public int ScavengeableVegetationItems = 0;
        public int ScavengeableCottonItems = 0;
        public int ScavengeableFishingItems = 0;
        public int ScavengeableOreFormationItems = 0;
        public int ScavengeableWaterItems = 0;
        public int FoodItemsConsumed = 0;
        public int DrinkItemsConsumed = 0;
        public int HumanityLost = 0;
        public int HonorLost = 0;
        public int WildlifeSkinned = 0;
        public int TrapsSprung = 0;
        public int TunnelsUsed = 0;
        public int TrapsPlaced = 0;
        public int NetsThrown = 0;
        public int HumanAbilitiesUsed = 0;
        public int UndeadAbilitiesUsed = 0;
        public int TimesRepaired = 0;
        public int TotalRepairAmount = 0;
        public int TotalObjectDamage = 0;
        public int TotalFollowers = 0;
        public int LargestSwarm = 0;
        public int HighestMonsterTierLevel = 1;

        #endregion
        
        private DateTime m_FatigueExpiration = DateTime.UtcNow;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime FatigueExpiration
        {
            get { return m_FatigueExpiration; }
            set { m_FatigueExpiration = value; }
        }

        private DateTime m_NextTeamSwitchAllowed = DateTime.UtcNow;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextTeamSwitchAllowed
        {
            get { return m_NextTeamSwitchAllowed; }
            set { m_NextTeamSwitchAllowed = value; }
        }

        public ActiveProfileType CurrentTeam = ActiveProfileType.Human;
        public int m_TeamSelectIndex = 0;
        
        [Constructable]
        public UOACZAccountEntry(string accountName): base(0x0)
        {
            Visible = false;
            Movable = false;

            m_AccountUsername = accountName;

            HumanProfile = new HumanProfileEntry(accountName);
            UndeadProfile = new UndeadProfileEntry(accountName);

            Backpack humanBackpack = new Backpack();
            humanBackpack.MaxItems = 500;
            m_UOACZHumanBackpackItems = humanBackpack;

            Backpack undeadBackpack = new Backpack();
            undeadBackpack.MaxItems = 500;
            m_UOACZUndeadBackpackItems = undeadBackpack;

            UOACZPersistance.m_UOACZAccountEntries.Add(this);
        }

        public UOACZAccountEntry(Serial serial): base(serial)
        {
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            HumanProfile.Delete();
            UndeadProfile.Delete();

            if (m_UOACZHumanBackpackItems != null)
            {
                if (!m_UOACZHumanBackpackItems.Deleted)
                    m_UOACZHumanBackpackItems.Delete();
            }

            if (m_UOACZHumanWornItems != null)
            {
                if (!m_UOACZHumanWornItems.Deleted)
                    m_UOACZHumanWornItems.Delete();
            }

            if (m_UOACZUndeadBackpackItems != null)
            {
                if (!m_UOACZUndeadBackpackItems.Deleted)
                    m_UOACZUndeadBackpackItems.Delete();
            }

            if (m_UOACZUndeadWornItems != null)
            {
                if (!m_UOACZUndeadWornItems.Deleted)
                    m_UOACZUndeadWornItems.Delete();
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)5); //Version

            //Version 0
            writer.Write(m_AccountUsername);
            writer.Write((int)ActiveProfile);
            writer.Write(NextEntryAllowed);

            writer.Write((Item)HumanProfile);
            writer.Write((Item)UndeadProfile);

            //Version 1
            writer.Write(MostRecentPlayer);
            writer.Write(MostRecentPlayerString);

            //Version 2
            writer.Write(m_HumanAbilitiesHotbarDisplayed);
            writer.Write(m_HumanStatsHotbarDisplayed);
            writer.Write(m_UndeadAbilitiesHotbarDisplayed);
            writer.Write(m_UndeadStatsHotbarDisplayed);
            writer.Write(m_ObjectivesDisplayed);

            writer.Write(m_HumanObjectivesExpanded);
            writer.Write(m_UndeadObjectivesExpanded);

            //Version 3
            writer.Write(m_InitialHumanItemsGenerated);
            writer.Write(m_UOACZHumanBackpackItems);
            writer.Write(m_UOACZHumanWornItems);

            writer.Write(m_CurrentSessionHumanScore);
            writer.Write(m_CurrentSessionUndeadScore);
            writer.Write(m_CurrentSessionTotalScore);

            writer.Write(m_PreviousSessionHumanScore);
            writer.Write(m_PreviousSessionUndeadScore);
            writer.Write(m_PreviousSessionTotalScore);

            writer.Write(m_LifetimeHumanScore);
            writer.Write(m_LifetimeUndeadScore);
            writer.Write(m_LifetimeTotalScore);

            writer.Write(m_BestHumanScore);
            writer.Write(m_BestUndeadScore);
            writer.Write(m_BestTotalScore);

            writer.Write((int)m_ScorePage);
            writer.Write(m_ScorePageNumber);

            writer.Write(m_RewardPoints);
            writer.Write(m_RewardPage);
            writer.Write(m_UnlockablesPage);
            writer.Write(m_ShowHumanChat);
            writer.Write(m_ShowUndeadChat);

            writer.Write(m_Unlockables.Count);
            for (int a = 0; a < m_Unlockables.Count; a++)
            {
                UOACZUnlockableDetailEntry unlockable = m_Unlockables[a];

                writer.Write((int)unlockable.m_UnlockableType);
                writer.Write(unlockable.m_Unlocked);
                writer.Write(unlockable.m_Active);
            }

            writer.Write(HumanVictoryCount);
            writer.Write(UndeadVictoryCount);
            writer.Write(HighestSessionScoreCount);
            writer.Write(TotalRewardPointsEarned);
            writer.Write(HumanDeaths);
            writer.Write(UndeadDeaths);
            writer.Write(HumanPlayersKilledAsHuman);
            writer.Write(UndeadPlayersKilledAsHuman);
            writer.Write(HumanPlayersKilledAsUndead);
            writer.Write(UndeadKilledAsHuman);
            writer.Write(CiviliansKilledAsUndead);
            writer.Write(MilitiaKilledAsUndead);
            writer.Write(WildlifeCorrupted);
            writer.Write(HumanUpgradePointsSpentTotal);
            writer.Write(HumanSurvivalPointsSpentTotal);
            writer.Write(MostHumanUpgradesSpentInASession);
            writer.Write(UndeadUpgradePointsSpentTotal);
            writer.Write(UndeadCorruptionPointsSpentTotal);
            writer.Write(LongestTimeAsHuman);
            writer.Write(LongestTimeAsHumanWithoutADeath);
            writer.Write(TotalTimeSpentAsHuman);
            writer.Write(TotalTimeSpentAsUndead);

            writer.Write(ScavengeableDebrisItems);
            writer.Write(ScavengeableContainerItems);
            writer.Write(ScavengeableVegetationItems);
            writer.Write(ScavengeableCottonItems);
            writer.Write(ScavengeableFishingItems);
            writer.Write(ScavengeableOreFormationItems);
            writer.Write(ScavengeableWaterItems);
            writer.Write(FoodItemsConsumed);
            writer.Write(DrinkItemsConsumed);
            writer.Write(HumanityLost);
            writer.Write(HonorLost);
            writer.Write(WildlifeSkinned);
            writer.Write(TrapsSprung);
            writer.Write(TunnelsUsed);
            writer.Write(TrapsPlaced);
            writer.Write(NetsThrown);
            writer.Write(HumanAbilitiesUsed);
            writer.Write(UndeadAbilitiesUsed);
            writer.Write(TimesRepaired);
            writer.Write(TotalRepairAmount);
            writer.Write(TotalObjectDamage);
            writer.Write(TotalFollowers);
            writer.Write(LargestSwarm);
            writer.Write(HighestMonsterTierLevel);

            //Version 4
            writer.Write(m_FatigueExpiration);
            writer.Write(m_InitialUndeadItemsGenerated);
            writer.Write(m_NextTeamSwitchAllowed);
            writer.Write((int)CurrentTeam);
            writer.Write(m_TeamSelectIndex);   

            //Version 5
            writer.Write(m_UOACZUndeadBackpackItems);
            writer.Write(m_UOACZUndeadWornItems);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_AccountUsername = reader.ReadString();
                ActiveProfile = (ActiveProfileType)reader.ReadInt();
                NextEntryAllowed = reader.ReadDateTime();

                HumanProfile = (HumanProfileEntry)reader.ReadItem();
                UndeadProfile = (UndeadProfileEntry)reader.ReadItem();
            }

            //Version 1
            if (version >= 1)
            {
                MostRecentPlayer = (PlayerMobile)reader.ReadMobile();
                MostRecentPlayerString = reader.ReadString();
            }

            //Version 2
            if (version >= 2)
            {
                m_HumanAbilitiesHotbarDisplayed = reader.ReadBool();
                m_HumanStatsHotbarDisplayed = reader.ReadBool();
                m_UndeadAbilitiesHotbarDisplayed = reader.ReadBool();
                m_UndeadStatsHotbarDisplayed = reader.ReadBool();
                m_ObjectivesDisplayed = reader.ReadBool();

                m_HumanObjectivesExpanded = reader.ReadBool();
                m_UndeadObjectivesExpanded = reader.ReadBool();
            }

            //Version 3
            if (version >= 3)
            {
                m_InitialHumanItemsGenerated = reader.ReadBool();
                m_UOACZHumanBackpackItems = (Backpack)reader.ReadItem();
                m_UOACZHumanWornItems = (Backpack)reader.ReadItem();

                m_CurrentSessionHumanScore = reader.ReadInt();
                m_CurrentSessionUndeadScore = reader.ReadInt();
                m_CurrentSessionTotalScore = reader.ReadInt();

                m_PreviousSessionHumanScore = reader.ReadInt();
                m_PreviousSessionUndeadScore = reader.ReadInt();
                m_PreviousSessionTotalScore = reader.ReadInt();

                m_LifetimeHumanScore = reader.ReadInt();
                m_LifetimeUndeadScore = reader.ReadInt();
                m_LifetimeTotalScore = reader.ReadInt();

                m_BestHumanScore = reader.ReadInt();
                m_BestUndeadScore = reader.ReadInt();
                m_BestTotalScore = reader.ReadInt();

                m_ScorePage = (ScorePageType)reader.ReadInt();
                m_ScorePageNumber = reader.ReadInt();

                m_RewardPoints = reader.ReadInt();
                m_RewardPage = reader.ReadInt();
                m_UnlockablesPage = reader.ReadInt();
                m_ShowHumanChat = reader.ReadBool();
                m_ShowUndeadChat = reader.ReadBool();

                int unlockablesCount = reader.ReadInt();

                for (int a = 0; a < unlockablesCount; a++)
                {
                    UOACZUnlockableType unlockableType = (UOACZUnlockableType)reader.ReadInt();
                    bool unlocked = reader.ReadBool();
                    bool active = reader.ReadBool();

                    m_Unlockables.Add(new UOACZUnlockableDetailEntry(unlockableType, unlocked, active));
                }

                HumanVictoryCount = reader.ReadInt();
                UndeadVictoryCount = reader.ReadInt();
                HighestSessionScoreCount = reader.ReadInt();
                TotalRewardPointsEarned = reader.ReadInt();
                HumanDeaths = reader.ReadInt();
                UndeadDeaths = reader.ReadInt();
                HumanPlayersKilledAsHuman = reader.ReadInt();
                UndeadPlayersKilledAsHuman = reader.ReadInt();
                HumanPlayersKilledAsUndead = reader.ReadInt();
                UndeadKilledAsHuman = reader.ReadInt();
                CiviliansKilledAsUndead = reader.ReadInt();
                MilitiaKilledAsUndead = reader.ReadInt();
                WildlifeCorrupted = reader.ReadInt();
                HumanUpgradePointsSpentTotal = reader.ReadInt();
                HumanSurvivalPointsSpentTotal = reader.ReadInt();
                MostHumanUpgradesSpentInASession = reader.ReadInt();
                UndeadUpgradePointsSpentTotal = reader.ReadInt();
                UndeadCorruptionPointsSpentTotal = reader.ReadInt();
                LongestTimeAsHuman = reader.ReadInt();
                LongestTimeAsHumanWithoutADeath = reader.ReadInt();
                TotalTimeSpentAsHuman = reader.ReadInt();
                TotalTimeSpentAsUndead = reader.ReadInt();

                ScavengeableDebrisItems = reader.ReadInt();
                ScavengeableContainerItems = reader.ReadInt();
                ScavengeableVegetationItems = reader.ReadInt();
                ScavengeableCottonItems = reader.ReadInt();
                ScavengeableFishingItems = reader.ReadInt();
                ScavengeableOreFormationItems = reader.ReadInt();
                ScavengeableWaterItems = reader.ReadInt();
                FoodItemsConsumed = reader.ReadInt();
                DrinkItemsConsumed = reader.ReadInt();
                HumanityLost = reader.ReadInt();
                HonorLost = reader.ReadInt();

                WildlifeSkinned = reader.ReadInt();
                TrapsSprung = reader.ReadInt();
                TunnelsUsed = reader.ReadInt();
                TrapsPlaced = reader.ReadInt();
                NetsThrown = reader.ReadInt();
                HumanAbilitiesUsed = reader.ReadInt();
                UndeadAbilitiesUsed = reader.ReadInt();
                TimesRepaired = reader.ReadInt();
                TotalRepairAmount = reader.ReadInt();
                TotalObjectDamage = reader.ReadInt();
                TotalFollowers = reader.ReadInt();
                LargestSwarm = reader.ReadInt();
                HighestMonsterTierLevel = reader.ReadInt();
            }

            if (version >= 4)
            {
                m_FatigueExpiration = reader.ReadDateTime();
                m_InitialUndeadItemsGenerated = reader.ReadBool();
                m_NextTeamSwitchAllowed = reader.ReadDateTime();
                CurrentTeam = (ActiveProfileType)reader.ReadInt();
                m_TeamSelectIndex = reader.ReadInt();
            }

            if (version >= 5)
            {  
                m_UOACZUndeadBackpackItems = (Backpack)reader.ReadItem() as Backpack;
                m_UOACZUndeadWornItems = (Backpack)reader.ReadItem() as Backpack;
            }

            UOACZPersistance.m_UOACZAccountEntries.Add(this);
        }

        public class HumanProfileEntry : Item
        {
            #region Properties

            public enum HumanActivePageType
            {
                Abilities,
                StatsAndSkills
            }

            public enum CauseOfDeathType
            {
                Misc,
                Undead,
                PlayerUndead,
                Human,
                PlayerHuman,
                Hunger,
                Thirst,
            }

            private string m_AccountName;
            [CommandProperty(AccessLevel.GameMaster)]
            public string AccountName
            {
                get { return m_AccountName; }
                set { m_AccountName = value; }
            }

            private CauseOfDeathType m_CauseOfDeath = CauseOfDeathType.Misc;
            [CommandProperty(AccessLevel.GameMaster)]
            public CauseOfDeathType CauseOfDeath
            {
                get { return m_CauseOfDeath; }
                set { m_CauseOfDeath = value; }
            }

            private HumanActivePageType m_ActivePage = HumanActivePageType.Abilities;
            [CommandProperty(AccessLevel.GameMaster)]
            public HumanActivePageType ActivePage
            {
                get { return m_ActivePage; }
                set { m_ActivePage = value; }
            }

            private DateTime m_NextAbilityAllowed = DateTime.UtcNow;
            [CommandProperty(AccessLevel.GameMaster)]
            public DateTime NextAbilityAllowed
            {
                get { return m_NextAbilityAllowed; }
                set { m_NextAbilityAllowed = value; }
            }

            private bool m_EnableOverheadStatsText = true;
            [CommandProperty(AccessLevel.GameMaster)]
            public bool EnableOverheadStatsText
            {
                get { return m_EnableOverheadStatsText; }
                set { m_EnableOverheadStatsText = value; }
            }

            private int m_AbilitiesHotbarPageNumber = 1;
            [CommandProperty(AccessLevel.GameMaster)]
            public int AbilitiesHotbarPageNumber
            {
                get { return m_AbilitiesHotbarPageNumber; }
                set { m_AbilitiesHotbarPageNumber = value; }
            }

            private int m_LeftPageNumber = 1;
            [CommandProperty(AccessLevel.GameMaster)]
            public int LeftPageNumber
            {
                get { return m_LeftPageNumber; }
                set { m_LeftPageNumber = value; }
            }

            private int m_RightPageNumber = 1;
            [CommandProperty(AccessLevel.GameMaster)]
            public int RightPageNumber
            {
                get { return m_RightPageNumber; }
                set { m_RightPageNumber = value; }
            }

            private int m_RadialSelectionNumber = 0;
            [CommandProperty(AccessLevel.GameMaster)]
            public int RadialSelectionNumber
            {
                get { return m_RadialSelectionNumber; }
                set { m_RadialSelectionNumber = value; }
            }

            private int m_FollowersMax = 0;
            [CommandProperty(AccessLevel.GameMaster)]
            public int FollowersMax
            {
                get { return m_FollowersMax; }
                set { m_FollowersMax = value; }
            }

            private int m_SurvivalPoints = 5;
            [CommandProperty(AccessLevel.GameMaster)]
            public int SurvivalPoints
            {
                get { return m_SurvivalPoints; }
                set { m_SurvivalPoints = value; }
            }

            private int m_UpgradePoints = 5;
            [CommandProperty(AccessLevel.GameMaster)]
            public int UpgradePoints
            {
                get { return m_UpgradePoints; }
                set { m_UpgradePoints = value; }
            }

            private int m_HungerPoints = 100;
            [CommandProperty(AccessLevel.GameMaster)]
            public int HungerPoints
            {
                get { return m_HungerPoints; }
                set
                {
                    m_HungerPoints = value;

                    if (m_HungerPoints < 0)
                        m_HungerPoints = 0;

                    if (m_HungerPoints > m_MaxHungerPoints)
                        m_HungerPoints = m_MaxHungerPoints;
                }
            }

            private int m_MaxHungerPoints = 100;
            [CommandProperty(AccessLevel.GameMaster)]
            public int MaxHungerPoints
            {
                get { return m_MaxHungerPoints; }
                set { m_MaxHungerPoints = value; }
            }

            private int m_ThirstPoints = 100;
            [CommandProperty(AccessLevel.GameMaster)]
            public int ThirstPoints
            {
                get { return m_ThirstPoints; }
                set
                {
                    m_ThirstPoints = value;

                    if (m_ThirstPoints < 0)
                        m_ThirstPoints = 0;

                    if (m_ThirstPoints > m_MaxThirstPoints)
                        m_ThirstPoints = m_MaxThirstPoints;
                }
            }

            private int m_MaxThirstPoints = 100;
            [CommandProperty(AccessLevel.GameMaster)]
            public int MaxThirstPoints
            {
                get { return m_MaxThirstPoints; }
                set { m_MaxThirstPoints = value; }
            }

            private int m_HumanityPoints = 100;
            [CommandProperty(AccessLevel.GameMaster)]
            public int HumanityPoints
            {
                get { return m_HumanityPoints; }
                set
                {
                    m_HumanityPoints = value;

                    if (m_HumanityPoints < 0)
                        m_HumanityPoints = 0;

                    if (m_HumanityPoints > m_MaxHumanityPoints)
                        m_HumanityPoints = m_MaxHumanityPoints;
                }
            }

            private int m_MaxHumanityPoints = 100;
            [CommandProperty(AccessLevel.GameMaster)]
            public int MaxHumanityPoints
            {
                get { return m_MaxHumanityPoints; }
                set { m_MaxHumanityPoints = value; }
            }

            private int m_HonorPoints = 100;
            [CommandProperty(AccessLevel.GameMaster)]
            public int HonorPoints
            {
                get { return m_HonorPoints; }
                set
                {
                    m_HonorPoints = value;

                    if (m_HonorPoints < 0)
                        m_HonorPoints = 0;

                    if (m_HonorPoints > m_MaxHonorPoints)
                        m_HonorPoints = m_MaxHonorPoints;
                }
            }

            private int m_MaxHonorPoints = 100;
            [CommandProperty(AccessLevel.GameMaster)]
            public int MaxHonorPoints
            {
                get { return m_MaxHonorPoints; }
                set { m_MaxHonorPoints = value; }
            }

            private int m_TotalMinutesAlive = 0;
            [CommandProperty(AccessLevel.GameMaster)]
            public int TotalMinutesAlive
            {
                get { return m_TotalMinutesAlive; }
                set { m_TotalMinutesAlive = value; }
            }

            private bool m_QualifiedForUpgrade = false;
            [CommandProperty(AccessLevel.GameMaster)]
            public bool QualifiedForUpgrade
            {
                get { return m_QualifiedForUpgrade; }
                set { m_QualifiedForUpgrade = value; }
            }

            private int m_UpgradeMinutes = 0;
            [CommandProperty(AccessLevel.GameMaster)]
            public int UpgradeMinutes
            {
                get { return m_UpgradeMinutes; }
                set { m_UpgradeMinutes = value; }
            }

            private int m_UpgradesSpent = 0;
            [CommandProperty(AccessLevel.GameMaster)]
            public int UpgradesSpent
            {
                get { return m_UpgradesSpent; }
                set { m_UpgradesSpent = value; }
            }

            private int m_ShufflesSpent = 0;
            [CommandProperty(AccessLevel.GameMaster)]
            public int ShufflesSpent
            {
                get { return m_ShufflesSpent; }
                set { m_ShufflesSpent = value; }
            }

            private int m_Deaths = 0;
            [CommandProperty(AccessLevel.GameMaster)]
            public int Deaths
            {
                get { return m_Deaths; }
                set { m_Deaths = value; }
            }

            private UOACZStockpileContainer m_Stockpile;
            [CommandProperty(AccessLevel.GameMaster)]
            public UOACZStockpileContainer Stockpile
            {
                get { return m_Stockpile; }
                set { m_Stockpile = value; }
            }

            public DateTime NextHideAllowed = DateTime.UtcNow;

            public List<SkillName> m_GMCraftingSkills = new List<SkillName>();

            public Dictionary<StatType, int> m_Stats = new Dictionary<StatType, int>();
            public Dictionary<SkillName, double> m_Skills = new Dictionary<SkillName, double>();
            public List<UOACZHumanUpgradeType> m_UpgradesAvailable = new List<UOACZHumanUpgradeType>();
            public List<UOACZHumanUpgradeEntry> m_Upgrades = new List<UOACZHumanUpgradeEntry>();
            public List<UOACZHumanAbilityEntry> m_Abilities = new List<UOACZHumanAbilityEntry>();

            #endregion

            [Constructable]
            public HumanProfileEntry(string accountName): base(0x0)
            {
                Visible = false;
                Movable = false;

                m_AccountName = accountName;
                m_Stockpile = new UOACZStockpileContainer(accountName);

                #region Stats and Skills

                m_Stats.Add(StatType.Str, 60);
                m_Stats.Add(StatType.Dex, 40);
                m_Stats.Add(StatType.Int, 20);

                int skillsCount = Enum.GetNames(typeof(SkillName)).Length;

                for (int a = 0; a < skillsCount; a++)
                {
                    m_Skills.Add((SkillName)a, 0);
                }

                #endregion
            }

            public HumanProfileEntry(Serial serial)
                : base(serial)
            {
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);
                writer.Write((int)0); //Version

                //Version 0
                writer.Write((int)m_CauseOfDeath);
                writer.Write((int)m_ActivePage);
                writer.Write(m_EnableOverheadStatsText);
                writer.Write(m_AbilitiesHotbarPageNumber);
                writer.Write(m_LeftPageNumber);
                writer.Write(m_RightPageNumber);
                writer.Write(m_RadialSelectionNumber);
                writer.Write(m_FollowersMax);
                writer.Write(m_SurvivalPoints);
                writer.Write(m_UpgradePoints);
                writer.Write(m_HungerPoints);
                writer.Write(m_MaxHungerPoints);
                writer.Write(m_ThirstPoints);
                writer.Write(m_MaxThirstPoints);
                writer.Write(m_HumanityPoints);
                writer.Write(m_MaxHumanityPoints);
                writer.Write(m_HonorPoints);
                writer.Write(m_MaxHonorPoints);
                writer.Write(m_TotalMinutesAlive);
                writer.Write(m_UpgradeMinutes);
                writer.Write(m_QualifiedForUpgrade);
                writer.Write(m_UpgradesSpent);
                writer.Write(m_ShufflesSpent);
                writer.Write(m_Deaths);
                writer.Write(m_AccountName);
                writer.Write(m_Stockpile);

                writer.Write(m_GMCraftingSkills.Count);
                for (int a = 0; a < m_GMCraftingSkills.Count; a++)
                {
                    writer.Write((int)m_GMCraftingSkills[a]);
                }

                writer.Write(m_Stats.Count);
                foreach (KeyValuePair<StatType, int> stat in m_Stats)
                {
                    writer.Write((int)stat.Key);
                    writer.Write(stat.Value);
                }

                writer.Write(m_Skills.Count);
                foreach (KeyValuePair<SkillName, double> skill in m_Skills)
                {
                    writer.Write((int)skill.Key);
                    writer.Write(skill.Value);
                }

                writer.Write(m_UpgradesAvailable.Count);
                for (int a = 0; a < m_UpgradesAvailable.Count; a++)
                {
                    writer.Write((int)m_UpgradesAvailable[a]);
                }

                writer.Write(m_Upgrades.Count);
                for (int a = 0; a < m_Upgrades.Count; a++)
                {
                    writer.Write((int)m_Upgrades[a].m_UpgradeType);
                    writer.Write(m_Upgrades[a].m_TimesPurchased);
                }

                writer.Write(m_Abilities.Count);
                for (int a = 0; a < m_Abilities.Count; a++)
                {
                    UOACZHumanAbilityEntry abilityEntry = m_Abilities[a];

                    writer.Write((int)abilityEntry.m_AbilityType);
                    writer.Write(abilityEntry.m_TimesAcquired);
                    writer.Write(abilityEntry.m_CooldownMinutes);
                    writer.Write(abilityEntry.m_NextUsageAllowed);
                }
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);
                int version = reader.ReadInt();

                //Version 0
                if (version >= 0)
                {
                    m_CauseOfDeath = (CauseOfDeathType)reader.ReadInt();
                    m_ActivePage = (HumanActivePageType)reader.ReadInt();
                    m_EnableOverheadStatsText = reader.ReadBool();
                    m_AbilitiesHotbarPageNumber = reader.ReadInt();
                    m_LeftPageNumber = reader.ReadInt();
                    m_RightPageNumber = reader.ReadInt();
                    m_RadialSelectionNumber = reader.ReadInt();
                    m_FollowersMax = reader.ReadInt();
                    m_SurvivalPoints = reader.ReadInt();
                    m_UpgradePoints = reader.ReadInt();
                    m_HungerPoints = reader.ReadInt();
                    m_MaxHungerPoints = reader.ReadInt();
                    m_ThirstPoints = reader.ReadInt();
                    m_MaxThirstPoints = reader.ReadInt();
                    m_HumanityPoints = reader.ReadInt();
                    m_MaxHumanityPoints = reader.ReadInt();
                    m_HonorPoints = reader.ReadInt();
                    m_MaxHonorPoints = reader.ReadInt();
                    m_TotalMinutesAlive = reader.ReadInt();
                    m_UpgradeMinutes = reader.ReadInt();
                    m_QualifiedForUpgrade = reader.ReadBool();
                    m_UpgradesSpent = reader.ReadInt();
                    m_ShufflesSpent = reader.ReadInt();
                    m_Deaths = reader.ReadInt();
                    m_AccountName = reader.ReadString();
                    m_Stockpile = (UOACZStockpileContainer)reader.ReadItem();

                    int gmCraftingSkills = reader.ReadInt();
                    for (int a = 0; a < gmCraftingSkills; a++)
                    {
                        m_GMCraftingSkills.Add((SkillName)reader.ReadInt());
                    }

                    int statsCount = reader.ReadInt();
                    for (int a = 0; a < statsCount; a++)
                    {
                        StatType statType = (StatType)reader.ReadInt();
                        int statValue = reader.ReadInt();

                        m_Stats.Add(statType, statValue);
                    }

                    int skillsCount = reader.ReadInt();
                    for (int a = 0; a < skillsCount; a++)
                    {
                        SkillName skillName = (SkillName)reader.ReadInt();
                        double skillValue = reader.ReadDouble();

                        m_Skills.Add(skillName, skillValue);
                    }

                    int upgradesAvailableCount = reader.ReadInt();
                    for (int a = 0; a < upgradesAvailableCount; a++)
                    {
                        m_UpgradesAvailable.Add((UOACZHumanUpgradeType)reader.ReadInt());
                    }

                    int upgradesCount = reader.ReadInt();
                    for (int a = 0; a < upgradesCount; a++)
                    {
                        UOACZHumanUpgradeType upgradeType = (UOACZHumanUpgradeType)reader.ReadInt();
                        int timesPurchased = reader.ReadInt();

                        m_Upgrades.Add(new UOACZHumanUpgradeEntry(upgradeType, timesPurchased));
                    }

                    int abilitiesCount = reader.ReadInt();
                    for (int a = 0; a < abilitiesCount; a++)
                    {
                        UOACZHumanAbilityType abilityType = (UOACZHumanAbilityType)reader.ReadInt();
                        int timesAcquired = reader.ReadInt();
                        double cooldownMinutes = reader.ReadDouble();
                        DateTime nextUsageAllowed = reader.ReadDateTime();

                        UOACZHumanAbilityEntry abilityEntry = new UOACZHumanAbilityEntry(abilityType, timesAcquired, cooldownMinutes, nextUsageAllowed);

                        m_Abilities.Add(abilityEntry);
                    }
                }
            }
        }

        public class UndeadProfileEntry : Item
        {
            #region Properties

            public DateTime m_NextMoveSoundAllowed = DateTime.UtcNow;
            public TimeSpan MoveSoundDelay = TimeSpan.FromSeconds(Utility.RandomMinMax(5, 10));

            public DateTime m_NextFormChangeAllowed = DateTime.UtcNow;

            public enum UndeadActivePageType
            {
                Abilities,
                StatsAndSkills,
            }

            private string m_AccountName;
            [CommandProperty(AccessLevel.GameMaster)]
            public string AccountName
            {
                get { return m_AccountName; }
                set { m_AccountName = value; }
            }

            private UndeadActivePageType m_ActivePage = UndeadActivePageType.Abilities;
            [CommandProperty(AccessLevel.GameMaster)]
            public UndeadActivePageType ActivePage
            {
                get { return m_ActivePage; }
                set { m_ActivePage = value; }
            }

            private DateTime m_NextAbilityAllowed = DateTime.UtcNow;
            [CommandProperty(AccessLevel.GameMaster)]
            public DateTime NextAbilityAllowed
            {
                get { return m_NextAbilityAllowed; }
                set { m_NextAbilityAllowed = value; }
            }

            private bool m_EnableOverheadStatsText = true;
            [CommandProperty(AccessLevel.GameMaster)]
            public bool EnableOverheadStatsText
            {
                get { return m_EnableOverheadStatsText; }
                set { m_EnableOverheadStatsText = value; }
            }

            private int m_AbilitiesHotbarPageNumber = 1;
            [CommandProperty(AccessLevel.GameMaster)]
            public int AbilitiesHotbarPageNumber
            {
                get { return m_AbilitiesHotbarPageNumber; }
                set { m_AbilitiesHotbarPageNumber = value; }
            }

            private int m_LeftPageNumber = 1;
            [CommandProperty(AccessLevel.GameMaster)]
            public int LeftPageNumber
            {
                get { return m_LeftPageNumber; }
                set { m_LeftPageNumber = value; }
            }

            private int m_RightPageNumber = 1;
            [CommandProperty(AccessLevel.GameMaster)]
            public int RightPageNumber
            {
                get { return m_RightPageNumber; }
                set { m_RightPageNumber = value; }
            }

            private int m_MonsterTier = 1;
            [CommandProperty(AccessLevel.GameMaster)]
            public int MonsterTier
            {
                get { return m_MonsterTier; }
                set { m_MonsterTier = value; }
            }

            private int m_FollowersMax = 1;
            [CommandProperty(AccessLevel.GameMaster)]
            public int FollowersMax
            {
                get { return m_FollowersMax; }
                set { m_FollowersMax = value; }
            }

            private int m_DamageMin = 3;
            [CommandProperty(AccessLevel.GameMaster)]
            public int DamageMin
            {
                get { return m_DamageMin; }
                set { m_DamageMin = value; }
            }

            private int m_DamageMax = 5;
            [CommandProperty(AccessLevel.GameMaster)]
            public int DamageMax
            {
                get { return m_DamageMax; }
                set { m_DamageMax = value; }
            }

            private int m_VirtualArmor = 0;
            [CommandProperty(AccessLevel.GameMaster)]
            public int VirtualArmor
            {
                get { return m_VirtualArmor; }
                set { m_VirtualArmor = value; }
            }

            private int m_CorruptionPoints = 3;
            [CommandProperty(AccessLevel.GameMaster)]
            public int CorruptionPoints
            {
                get { return m_CorruptionPoints; }
                set { m_CorruptionPoints = value; }
            }

            private int m_UpgradePoints = 0;
            [CommandProperty(AccessLevel.GameMaster)]
            public int UpgradePoints
            {
                get { return m_UpgradePoints; }
                set { m_UpgradePoints = value; }
            }

            private int m_BodyValue = 3;
            [CommandProperty(AccessLevel.GameMaster)]
            public int BodyValue
            {
                get { return m_BodyValue; }
                set { m_BodyValue = value; }
            }

            private int m_HueMod = 0;
            [CommandProperty(AccessLevel.GameMaster)]
            public int HueMod
            {
                get { return m_HueMod; }
                set { m_HueMod = value; }
            }

            private int m_IconItemID = 0;
            [CommandProperty(AccessLevel.GameMaster)]
            public int IconItemID
            {
                get { return m_IconItemID; }
                set { m_IconItemID = value; }
            }

            private int m_IconHue = 0;
            [CommandProperty(AccessLevel.GameMaster)]
            public int IconHue
            {
                get { return m_IconHue; }
                set { m_IconHue = value; }
            }

            private int m_IconOffsetX = 0;
            [CommandProperty(AccessLevel.GameMaster)]
            public int IconOffsetX
            {
                get { return m_IconOffsetX; }
                set { m_IconOffsetX = value; }
            }

            private int m_IconOffsetY = 0;
            [CommandProperty(AccessLevel.GameMaster)]
            public int IconOffsetY
            {
                get { return m_IconOffsetY; }
                set { m_IconOffsetY = value; }
            }

            private int m_AngerSound = 0x1D7;
            [CommandProperty(AccessLevel.GameMaster)]
            public int AngerSound
            {
                get { return m_AngerSound; }
                set { m_AngerSound = value; }
            }

            private int m_IdleSound = 0x1D8;
            [CommandProperty(AccessLevel.GameMaster)]
            public int IdleSound
            {
                get { return m_IdleSound; }
                set { m_IdleSound = value; }
            }

            private int m_AttackSound = 0x1D9;
            [CommandProperty(AccessLevel.GameMaster)]
            public int AttackSound
            {
                get { return m_AttackSound; }
                set { m_AttackSound = value; }
            }

            private int m_HurtSound = 0x1DA;
            [CommandProperty(AccessLevel.GameMaster)]
            public int HurtSound
            {
                get { return m_HurtSound; }
                set { m_HurtSound = value; }
            }

            private int m_DeathSound = 0x1DB;
            [CommandProperty(AccessLevel.GameMaster)]
            public int DeathSound
            {
                get { return m_DeathSound; }
                set { m_DeathSound = value; }
            }

            private int m_AttackAnimation = -1;
            [CommandProperty(AccessLevel.GameMaster)]
            public int AttackAnimation
            {
                get { return m_AttackAnimation; }
                set { m_AttackAnimation = value; }
            }

            private int m_AttackAnimationFrames = 0;
            [CommandProperty(AccessLevel.GameMaster)]
            public int AttackAnimationFrames
            {
                get { return m_AttackAnimationFrames; }
                set { m_AttackAnimationFrames = value; }
            }

            private int m_IdleAnimation = -1;
            [CommandProperty(AccessLevel.GameMaster)]
            public int IdleAnimation
            {
                get { return m_IdleAnimation; }
                set { m_IdleAnimation = value; }
            }

            private int m_IdleAnimationFrames = 0;
            [CommandProperty(AccessLevel.GameMaster)]
            public int IdleAnimationFrames
            {
                get { return m_IdleAnimationFrames; }
                set { m_IdleAnimationFrames = value; }
            }

            private int m_HurtAnimation = -1;
            [CommandProperty(AccessLevel.GameMaster)]
            public int HurtAnimation
            {
                get { return m_HurtAnimation; }
                set { m_HurtAnimation = value; }
            }

            private int m_HurtAnimationFrames = 0;
            [CommandProperty(AccessLevel.GameMaster)]
            public int HurtAnimationFrames
            {
                get { return m_HurtAnimationFrames; }
                set { m_HurtAnimationFrames = value; }
            }

            private int m_SpecialAnimation = 11;
            [CommandProperty(AccessLevel.GameMaster)]
            public int SpecialAnimation
            {
                get { return m_SpecialAnimation; }
                set { m_SpecialAnimation = value; }
            }

            private int m_SpecialAnimationFrames = 5;
            [CommandProperty(AccessLevel.GameMaster)]
            public int SpecialAnimationFrames
            {
                get { return m_SpecialAnimationFrames; }
                set { m_SpecialAnimationFrames = value; }
            }

            private int m_CastingAnimation = 4;
            [CommandProperty(AccessLevel.GameMaster)]
            public int CastingAnimation
            {
                get { return m_CastingAnimation; }
                set { m_CastingAnimation = value; }
            }

            private int m_CastingAnimationFrames = 4;
            [CommandProperty(AccessLevel.GameMaster)]
            public int CastingAnimationFrames
            {
                get { return m_CastingAnimationFrames; }
                set { m_CastingAnimationFrames = value; }
            }

            private int m_TotalMinutesUndead = 0;
            [CommandProperty(AccessLevel.GameMaster)]
            public int TotalMinutesUndead
            {
                get { return m_TotalMinutesUndead; }
                set { m_TotalMinutesUndead = value; }
            }

            private int m_UpgradeMinutes = 0;
            [CommandProperty(AccessLevel.GameMaster)]
            public int UpgradeMinutes
            {
                get { return m_UpgradeMinutes; }
                set { m_UpgradeMinutes = value; }
            }

            private bool m_QualifiedForUpgrade = true;
            [CommandProperty(AccessLevel.GameMaster)]
            public bool QualifiedForUpgrade
            {
                get { return m_QualifiedForUpgrade; }
                set { m_QualifiedForUpgrade = value; }
            }

            private int m_UpgradesSpent = 0;
            [CommandProperty(AccessLevel.GameMaster)]
            public int UpgradesSpent
            {
                get { return m_UpgradesSpent; }
                set { m_UpgradesSpent = value; }
            }

            private int m_ShufflesSpent = 0;
            [CommandProperty(AccessLevel.GameMaster)]
            public int ShufflesSpent
            {
                get { return m_ShufflesSpent; }
                set { m_ShufflesSpent = value; }
            }

            private int m_Deaths = 0;
            [CommandProperty(AccessLevel.GameMaster)]
            public int Deaths
            {
                get { return m_Deaths; }
                set { m_Deaths = value; }
            }

            private DateTime m_NextUndeadItemAllowed = DateTime.UtcNow;
            [CommandProperty(AccessLevel.GameMaster)]
            public DateTime NextUndeadItemAllowed
            {
                get { return m_NextUndeadItemAllowed; }
                set { m_NextUndeadItemAllowed = value; }
            }

            public List<UOACZUndeadUpgradeType> m_FormsKilledWith = new List<UOACZUndeadUpgradeType>();

            private UOACZUndeadUpgradeType m_ActiveForm = UOACZUndeadUpgradeType.Zombie;
            [CommandProperty(AccessLevel.GameMaster)]
            public UOACZUndeadUpgradeType ActiveForm
            {
                get { return m_ActiveForm; }
                set { m_ActiveForm = value; }
            }

            public Dictionary<StatType, int> m_Stats = new Dictionary<StatType, int>();
            public Dictionary<SkillName, double> m_Skills = new Dictionary<SkillName, double>();
            public List<UOACZUndeadAbilityEntry> m_Abilities = new List<UOACZUndeadAbilityEntry>();

            private int m_DyedHueMod = -1;
            [CommandProperty(AccessLevel.GameMaster)]
            public int DyedHueMod
            {
                get { return m_DyedHueMod; }
                set { m_DyedHueMod = value; }
            }

            private int m_PostTier5Upgrades = 0;
            [CommandProperty(AccessLevel.GameMaster)]
            public int PostTier5Upgrades
            {
                get { return m_PostTier5Upgrades; }
                set { m_PostTier5Upgrades = value; }
            }

            #endregion

            [Constructable]
            public UndeadProfileEntry(string accountName): base(0x0)
            {
                Visible = false;
                Movable = false;

                m_AccountName = accountName;

                #region Stats and Skills

                m_Stats.Add(StatType.Str, 50);
                m_Stats.Add(StatType.Dex, 25);
                m_Stats.Add(StatType.Int, 10);

                int skillsCount = Enum.GetNames(typeof(SkillName)).Length;

                for (int a = 0; a < skillsCount; a++)
                {
                    m_Skills.Add((SkillName)a, 0);
                }

                #endregion
            }

            public UndeadProfileEntry(Serial serial)
                : base(serial)
            {
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);
                writer.Write((int)2); //Version                   

                writer.Write((int)m_ActivePage);
                writer.Write(m_EnableOverheadStatsText);
                writer.Write(m_AbilitiesHotbarPageNumber);
                writer.Write(m_LeftPageNumber);
                writer.Write(m_RightPageNumber);
                writer.Write(m_MonsterTier);
                writer.Write(m_FollowersMax);
                writer.Write(m_DamageMin);
                writer.Write(m_DamageMax);
                writer.Write(m_VirtualArmor);
                writer.Write(m_CorruptionPoints);
                writer.Write(m_UpgradePoints);
                writer.Write(m_BodyValue);
                writer.Write(m_HueMod);
                writer.Write(m_IconItemID);
                writer.Write(m_IconHue);
                writer.Write(m_IconOffsetX);
                writer.Write(m_IconOffsetY);
                writer.Write(m_TotalMinutesUndead);
                writer.Write(m_UpgradeMinutes);
                writer.Write(m_QualifiedForUpgrade);
                writer.Write(m_UpgradesSpent);
                writer.Write(m_ShufflesSpent);
                writer.Write(m_Deaths);
                writer.Write(m_NextUndeadItemAllowed);

                writer.Write(m_FormsKilledWith.Count);
                for (int a = 0; a < m_FormsKilledWith.Count; a++)
                {
                    writer.Write((int)m_FormsKilledWith[a]);
                }

                writer.Write((int)m_ActiveForm);
                writer.Write(m_AngerSound);
                writer.Write(m_IdleSound);
                writer.Write(m_AttackSound);
                writer.Write(m_HurtSound);
                writer.Write(m_DeathSound);
                writer.Write(m_AttackAnimation);
                writer.Write(m_AttackAnimationFrames);
                writer.Write(m_IdleAnimation);
                writer.Write(m_IdleAnimationFrames);
                writer.Write(m_HurtAnimation);
                writer.Write(m_HurtAnimationFrames);
                writer.Write(m_SpecialAnimation);
                writer.Write(m_SpecialAnimationFrames);
                writer.Write(m_CastingAnimation);
                writer.Write(m_CastingAnimationFrames);
                writer.Write(m_AccountName);

                //Version 0
                writer.Write(m_Stats.Count);
                foreach (KeyValuePair<StatType, int> stat in m_Stats)
                {
                    writer.Write((int)stat.Key);
                    writer.Write(stat.Value);
                }

                writer.Write(m_Skills.Count);
                foreach (KeyValuePair<SkillName, double> skill in m_Skills)
                {
                    writer.Write((int)skill.Key);
                    writer.Write(skill.Value);
                }

                writer.Write(m_Abilities.Count);
                for (int a = 0; a < m_Abilities.Count; a++)
                {
                    writer.Write((int)m_Abilities[a].m_AbilityType);
                    writer.Write(m_Abilities[a].m_CooldownMinutes);
                    writer.Write(m_Abilities[a].m_NextUsageAllowed);
                }

                //Version 1
                writer.Write(m_DyedHueMod);

                //Version 2
                writer.Write(m_PostTier5Upgrades);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);
                int version = reader.ReadInt();

                //Version 0
                if (version >= 0)
                {
                    m_ActivePage = (UndeadActivePageType)reader.ReadInt();
                    m_EnableOverheadStatsText = reader.ReadBool();
                    m_AbilitiesHotbarPageNumber = reader.ReadInt();
                    m_LeftPageNumber = reader.ReadInt();
                    m_RightPageNumber = reader.ReadInt();
                    m_MonsterTier = reader.ReadInt();
                    m_FollowersMax = reader.ReadInt();
                    m_DamageMin = reader.ReadInt();
                    m_DamageMax = reader.ReadInt();
                    m_VirtualArmor = reader.ReadInt();
                    m_CorruptionPoints = reader.ReadInt();
                    m_UpgradePoints = reader.ReadInt();
                    m_BodyValue = reader.ReadInt();
                    m_HueMod = reader.ReadInt();
                    m_IconItemID = reader.ReadInt();
                    m_IconHue = reader.ReadInt();
                    m_IconOffsetX = reader.ReadInt();
                    m_IconOffsetY = reader.ReadInt();
                    m_TotalMinutesUndead = reader.ReadInt();
                    m_UpgradeMinutes = reader.ReadInt();
                    m_QualifiedForUpgrade = reader.ReadBool();
                    m_UpgradesSpent = reader.ReadInt();
                    m_ShufflesSpent = reader.ReadInt();
                    m_Deaths = reader.ReadInt();
                    m_NextUndeadItemAllowed = reader.ReadDateTime();

                    int formsKilledWithCounted = reader.ReadInt();
                    for (int a = 0; a < formsKilledWithCounted; a++)
                    {
                        m_FormsKilledWith.Add((UOACZUndeadUpgradeType)reader.ReadInt());
                    }

                    m_ActiveForm = (UOACZUndeadUpgradeType)reader.ReadInt();
                    m_AngerSound = reader.ReadInt();
                    m_IdleSound = reader.ReadInt();
                    m_AttackSound = reader.ReadInt();
                    m_HurtSound = reader.ReadInt();
                    m_DeathSound = reader.ReadInt();

                    m_AttackAnimation = reader.ReadInt();
                    m_AttackAnimationFrames = reader.ReadInt();
                    m_IdleAnimation = reader.ReadInt();
                    m_IdleAnimationFrames = reader.ReadInt();
                    m_HurtAnimation = reader.ReadInt();
                    m_HurtAnimationFrames = reader.ReadInt();

                    m_SpecialAnimation = reader.ReadInt();
                    m_SpecialAnimationFrames = reader.ReadInt();
                    m_CastingAnimation = reader.ReadInt();
                    m_CastingAnimationFrames = reader.ReadInt();

                    m_AccountName = reader.ReadString();

                    int statsCount = reader.ReadInt();
                    for (int a = 0; a < statsCount; a++)
                    {
                        StatType statType = (StatType)reader.ReadInt();
                        int statValue = reader.ReadInt();

                        m_Stats.Add(statType, statValue);
                    }

                    int skillsCount = reader.ReadInt();
                    for (int a = 0; a < skillsCount; a++)
                    {
                        SkillName skillName = (SkillName)reader.ReadInt();
                        double skillValue = reader.ReadDouble();

                        m_Skills.Add(skillName, skillValue);
                    }

                    int abilitiesCount = reader.ReadInt();
                    for (int a = 0; a < abilitiesCount; a++)
                    {
                        UOACZUndeadAbilityType abilityType = (UOACZUndeadAbilityType)reader.ReadInt();
                        double cooldownMinutes = reader.ReadDouble();
                        DateTime nextUsageAllowed = reader.ReadDateTime();

                        m_Abilities.Add(new UOACZUndeadAbilityEntry(abilityType, cooldownMinutes, nextUsageAllowed));
                    }
                }

                //Version 1
                if (version >= 1)
                {
                    m_DyedHueMod = reader.ReadInt();
                }

                //Version 2
                if (version >= 2)
                {
                    m_PostTier5Upgrades = reader.ReadInt();
                }
            }
        }
    }
}
