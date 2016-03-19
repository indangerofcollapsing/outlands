using System;
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

namespace Server.Items
{
    public static class InfluencePersistance
    {
        public static InfluencePersistanceItem PersistanceItem;

        public static bool Enabled = false;        

        public static List<InfluenceMarker> m_InfluenceMarkers = new List<InfluenceMarker>();

        public static List<InfluenceAccountEntry> m_InfluenceAccountEntries = new List<InfluenceAccountEntry>();
        public static List<InfluenceAccountEntry> m_ActiveInfluenceAccountEntries = new List<InfluenceAccountEntry>();
        public static List<Mobile> m_PlayersGivenFreeRecruitmentDeeds = new List<Mobile>();

        public static IndexedRegionName[] Regions = new IndexedRegionName[]
        {
            IndexedRegionName.Britain,
            IndexedRegionName.BuccaneeersDen,
            IndexedRegionName.Cove,
            IndexedRegionName.Jhelom,
            IndexedRegionName.Magincia,
            IndexedRegionName.Minoc,
            IndexedRegionName.Moonglow,
            IndexedRegionName.Nujelm,
            IndexedRegionName.Occlo,
            IndexedRegionName.SerpentsHold,
            IndexedRegionName.SkaraBrae,
            IndexedRegionName.Trinsic,
            IndexedRegionName.Vesper,
            IndexedRegionName.Yew
        };

        public static int[] lotteryTicketTiers = new int[]
        {
            100, 200, 300, 500, 750,
            1000, 1250, 1500, 1750, 2000,
            2500, 3000, 3500, 4000, 4500,
            5000, 6000, 7000, 8000, 9000,
            10000, 12000, 14000, 16000, 18000,
            20000, 22500, 25000, 27500, 30000,
        };
        
        public static TimeSpan TickDuration = TimeSpan.FromMinutes(30);
        public static TimeSpan IntervalDuration = TimeSpan.FromHours(24);
        public static TimeSpan CycleDuration = TimeSpan.FromDays(7);        

        public static DateTime NextTickCompletion = DateTime.UtcNow + TickDuration;
        public static DateTime NextIntervalCompletion = DateTime.UtcNow + IntervalDuration;
        public static DateTime NextCycleCompletion = DateTime.UtcNow + CycleDuration;

        public static DateTime PreviousCycleCompletion = DateTime.MinValue;  

        public static int StartingTownPrestige = 2;

        private static Timer InfluenceTimer;

         #region Store Current and Previous Region Controllers and Influence Totals

        //Current Standings
        public static InfluenceAccountEntry RealmController;
        public static InfluenceAccountEntry BritainController;
        public static InfluenceAccountEntry BuccaneersDenController;
        public static InfluenceAccountEntry CoveController;
        public static InfluenceAccountEntry JhelomController;
        public static InfluenceAccountEntry MaginciaController;
        public static InfluenceAccountEntry MinocController;
        public static InfluenceAccountEntry MoonglowController;
        public static InfluenceAccountEntry NujelmController;
        public static InfluenceAccountEntry OccloController;
        public static InfluenceAccountEntry SerpentsHoldController;
        public static InfluenceAccountEntry SkaraBraeController;
        public static InfluenceAccountEntry TrinsicController;
        public static InfluenceAccountEntry VesperController;
        public static InfluenceAccountEntry YewController;

        public static double RealmControllerCurrentInfluence = 0;
        public static double BritainControllerCurrentInfluence = 0;
        public static double BuccaneersDenControllerCurrentInfluence = 0;
        public static double CoveControllerCurrentInfluence = 0;
        public static double JhelomControllerCurrentInfluence = 0;
        public static double MaginciaControllerCurrentInfluence = 0;
        public static double MinocControllerCurrentInfluence = 0;
        public static double MoonglowControllerCurrentInfluence = 0;
        public static double NujelmControllerCurrentInfluence = 0;
        public static double OccloControllerCurrentInfluence = 0;
        public static double SerpentsHoldControllerCurrentInfluence = 0;
        public static double SkaraBraeControllerCurrentInfluence = 0;
        public static double TrinsicControllerCurrentInfluence = 0;
        public static double VesperControllerCurrentInfluence = 0;
        public static double YewControllerCurrentInfluence = 0;

        //Last Cycle Results
        public static InfluenceAccountEntry PreviousRealmController;
        public static InfluenceAccountEntry PreviousBritainController;
        public static InfluenceAccountEntry PreviousBuccaneersDenController;
        public static InfluenceAccountEntry PreviousCoveController;
        public static InfluenceAccountEntry PreviousJhelomController;
        public static InfluenceAccountEntry PreviousMaginciaController;
        public static InfluenceAccountEntry PreviousMinocController;
        public static InfluenceAccountEntry PreviousMoonglowController;
        public static InfluenceAccountEntry PreviousNujelmController;
        public static InfluenceAccountEntry PreviousOccloController;
        public static InfluenceAccountEntry PreviousSerpentsHoldController;
        public static InfluenceAccountEntry PreviousSkaraBraeController;
        public static InfluenceAccountEntry PreviousTrinsicController;
        public static InfluenceAccountEntry PreviousVesperController;
        public static InfluenceAccountEntry PreviousYewController;

        public static string PreviousRealmControllerName = "-";
        public static string PreviousBritainControllerName = "-";
        public static string PreviousBuccaneersDenControllerName = "-";
        public static string PreviousCoveControllerName = "-";
        public static string PreviousJhelomControllerName = "-";
        public static string PreviousMaginciaControllerName = "-";
        public static string PreviousMinocControllerName = "-";
        public static string PreviousMoonglowControllerName = "-";
        public static string PreviousNujelmControllerName = "-";
        public static string PreviousOccloControllerName = "-";
        public static string PreviousSerpentsHoldControllerName = "-";
        public static string PreviousSkaraBraeControllerName = "-";
        public static string PreviousTrinsicControllerName = "-";
        public static string PreviousVesperControllerName = "-";
        public static string PreviousYewControllerName = "-";

        public static double PreviousRealmControllerInfluence = 0;
        public static double PreviousBritainControllerInfluence = 0;
        public static double PreviousBuccaneersDenControllerInfluence = 0;
        public static double PreviousCoveControllerInfluence = 0;
        public static double PreviousJhelomControllerInfluence = 0;
        public static double PreviousMaginciaControllerInfluence = 0;
        public static double PreviousMinocControllerInfluence = 0;
        public static double PreviousMoonglowControllerInfluence = 0;
        public static double PreviousNujelmControllerInfluence = 0;
        public static double PreviousOccloControllerInfluence = 0;
        public static double PreviousSerpentsHoldControllerInfluence = 0;
        public static double PreviousSkaraBraeControllerInfluence = 0;
        public static double PreviousTrinsicControllerInfluence = 0;
        public static double PreviousVesperControllerInfluence = 0;
        public static double PreviousYewControllerInfluence = 0;

        #endregion

        public static void Initialize()
        {
            CommandSystem.Register("EnableInfluenceSystem", AccessLevel.GameMaster, new CommandEventHandler(EnableInfluenceSystem));
            CommandSystem.Register("DisableInfluenceSystem", AccessLevel.GameMaster, new CommandEventHandler(DisablesInfluenceSystem));

            CommandSystem.Register("RestartInfluenceCycle", AccessLevel.GameMaster, new CommandEventHandler(RestartInfluenceCycle));
            CommandSystem.Register("StartInfluenceSystem", AccessLevel.GameMaster, new CommandEventHandler(StartInfluenceSystem));
            CommandSystem.Register("StopInfluenceSystem", AccessLevel.GameMaster, new CommandEventHandler(StopInfluenceSystem));

            CommandSystem.Register("ManuallySetCycleEarnedInfluence", AccessLevel.GameMaster, new CommandEventHandler(ManuallySetCycleEarnedInfluence));
            CommandSystem.Register("RecalculateRegionControllersInfluence", AccessLevel.GameMaster, new CommandEventHandler(RecalculateRegionControllersInfluence));

            CommandSystem.Register("ForceCycleCompletion", AccessLevel.GameMaster, new CommandEventHandler(ForceCycleCompletion));
          
            // To Begin Influence System From Scratch:
            // 1) Run [EnableInfluenceSystem
            // 2) Run [RestartInfluenceSystem
            
            CommandSystem.Register("SpawnAllInfluenceMarkers", AccessLevel.GameMaster, new CommandEventHandler(SpawnAllInfluenceMarkers));
            CommandSystem.Register("DeleteAllInfluenceMarkers", AccessLevel.GameMaster, new CommandEventHandler(DeleteAllInfluenceMarkers));
            CommandSystem.Register("WipeInfluenceSystem", AccessLevel.GameMaster, new CommandEventHandler(WipeInfluenceSystem));

            CommandSystem.Register("RecalculateAllMarkers", AccessLevel.GameMaster, new CommandEventHandler(RecalculateAllMarkers));

            Timer.DelayCall(TimeSpan.FromTicks(1), delegate
            {
                if (PersistanceItem == null)                
                    PersistanceItem = new InfluencePersistanceItem();                
                
                if (Enabled)
                {
                    InfluenceTimer = new InternalTimer(PersistanceItem);
                    InfluenceTimer.Start();
                }                
            });
        }

        private class InternalTimer : Timer
        {
            private InfluencePersistanceItem m_PersistanceItem;

            public InternalTimer(InfluencePersistanceItem persistanceItem): base(TimeSpan.Zero, TimeSpan.FromMinutes(1))
            {
                Priority = TimerPriority.FiveSeconds;

                m_PersistanceItem = persistanceItem;
            }

            protected override void OnTick()
            {
                if (DateTime.UtcNow >= NextTickCompletion)
                    InfluenceTick();
            }
        }   

        private static void InfluenceTick()
        {
            if (!Enabled)
                return;

            bool isCycleComplete = false;
            bool isIntervalComplete = false;

            NextTickCompletion = NextTickCompletion + TickDuration;

            //Interval Complete
            if (DateTime.UtcNow >= NextIntervalCompletion)
            {
                isIntervalComplete = true;

                NextIntervalCompletion = NextIntervalCompletion + IntervalDuration;
            }

            //Cycle Complete
            if (DateTime.UtcNow >= NextCycleCompletion)
            {
                isCycleComplete = true;
                
                NextCycleCompletion = NextCycleCompletion + CycleDuration;
            }            

            List<NetState> m_States = NetState.Instances;

            for (int a = 0; a < m_States.Count; a++)
            {
                PlayerMobile player = m_States[a].Mobile as PlayerMobile;

                if (player == null)
                    continue;

                if (player.m_InfluenceAccountEntry == null)
                    CheckCreateInfluenceAccountEntry(player);

                else if (player.m_InfluenceAccountEntry.Deleted)
                    CheckCreateInfluenceAccountEntry(player);

                if (player.m_InfluenceAccountEntry == null)
                    continue;                

                player.m_InfluenceAccountEntry.TicksRecordedThisInterval++;
            }

            foreach (InfluenceAccountEntry entry in InfluencePersistance.m_InfluenceAccountEntries)
            {
                if (entry == null) continue;
                if (entry.Deleted) continue;

                if (isIntervalComplete || isCycleComplete)
                {
                    UpdatePlayerInfluenceScores(entry);

                    entry.TicksRecordedThisInterval = 0;
                }
            }

            if (isCycleComplete)
                ResolveCycle();              
        }

        [Usage("EnableInfluenceSystem")]
        [Description("Enables the Influence System")]
        public static void EnableInfluenceSystem(CommandEventArgs arg)
        {
            Enabled = true;

            arg.Mobile.SendMessage("Influence System Enabled.");
        }

        [Usage("DisableInfluenceSystem")]
        [Description("Disables the Influence System")]
        public static void DisablesInfluenceSystem(CommandEventArgs arg)
        {
            Enabled = false;

            if (InfluenceTimer != null)
            {
                InfluenceTimer.Stop();
                InfluenceTimer = null;
            }            

            arg.Mobile.SendMessage("Influence System Disable.");
        }

        [Usage("RestartInfluenceCycle")]
        [Description("Resets The Current Influence Cycle")]
        public static void RestartInfluenceCycle(CommandEventArgs arg)
        {
            NextTickCompletion = DateTime.UtcNow + TickDuration;
            NextIntervalCompletion = DateTime.UtcNow + IntervalDuration;
            NextCycleCompletion = DateTime.UtcNow + CycleDuration;

            foreach (InfluenceAccountEntry entry in InfluencePersistance.m_InfluenceAccountEntries)
            {
                if (entry == null) continue;
                if (entry.Deleted) continue;

                ClearPlayerInfluenceScores(entry);
            }

            ResetCityControllers();            

            if (InfluenceTimer != null)
            {
                InfluenceTimer.Stop();
                InfluenceTimer = null;
            }
            
            InfluenceTimer = new InternalTimer(PersistanceItem);
            InfluenceTimer.Start();

            arg.Mobile.SendMessage("Current Influence Cycle Reset and Started.");
        }

        [Usage("StartInfluenceSystem")]
        [Description("Starts the Influence System Timer")]
        public static void StartInfluenceSystem(CommandEventArgs arg)
        {
            if (InfluenceTimer != null)
            {
                InfluenceTimer.Stop();
                InfluenceTimer = null;
            }

            InfluenceTimer = new InternalTimer(PersistanceItem);
            InfluenceTimer.Start();

            NextTickCompletion = DateTime.UtcNow + TickDuration;

            arg.Mobile.SendMessage("Influence System Timer Started.");
        }

        [Usage("StopInfluenceSystem")]
        [Description("Disables The Influence System")]
        public static void StopInfluenceSystem(CommandEventArgs arg)
        {
            if (InfluenceTimer != null)
            {
                InfluenceTimer.Stop();
                InfluenceTimer = null;
            }

            arg.Mobile.SendMessage("Influence System Timer Stopped.");
        }

        [Usage("ManuallySetCycleEarnedInfluence")]
        [Description("Manually Sets Each Accounts Earned Influence to Equal Full Amount of Cycle Progress")]
        public static void ManuallySetCycleEarnedInfluence(CommandEventArgs arg)
        {
            TimeSpan timeRemainingInCycle = CycleDuration - (NextCycleCompletion - DateTime.UtcNow);
            TimeSpan timeRemainingInInterval = IntervalDuration - (NextIntervalCompletion - DateTime.UtcNow);

            int intervalsCompleted = (int)(Math.Floor(timeRemainingInCycle.TotalDays));
            int currentTicksThisInterval = (int)(Math.Ceiling((double)timeRemainingInInterval.TotalMinutes / (double)TickDuration.TotalMinutes));

            foreach (InfluenceAccountEntry influenceAccountEntry in InfluencePersistance.m_InfluenceAccountEntries)
            {
                if (influenceAccountEntry == null)
                    continue;

                influenceAccountEntry.TicksRecordedThisInterval = currentTicksThisInterval;

                influenceAccountEntry.RealmCurrentInfluence = influenceAccountEntry.RealmInfluenceEarnedPerDay * intervalsCompleted;

                influenceAccountEntry.BritainCurrentInfluence = influenceAccountEntry.BritainInfluenceEarnedPerDay * intervalsCompleted;
                influenceAccountEntry.BuccaneersDenCurrentInfluence = influenceAccountEntry.BuccaneersDenInfluenceEarnedPerDay * intervalsCompleted;
                influenceAccountEntry.CoveCurrentInfluence = influenceAccountEntry.CoveInfluenceEarnedPerDay * intervalsCompleted;
                influenceAccountEntry.JhelomCurrentInfluence = influenceAccountEntry.JhelomInfluenceEarnedPerDay * intervalsCompleted;
                influenceAccountEntry.MaginciaCurrentInfluence = influenceAccountEntry.MaginciaInfluenceEarnedPerDay * intervalsCompleted;
                influenceAccountEntry.MinocCurrentInfluence = influenceAccountEntry.MinocInfluenceEarnedPerDay * intervalsCompleted;
                influenceAccountEntry.MoonglowCurrentInfluence = influenceAccountEntry.MoonglowInfluenceEarnedPerDay * intervalsCompleted;
                influenceAccountEntry.NujelmCurrentInfluence = influenceAccountEntry.NujelmInfluenceEarnedPerDay * intervalsCompleted;
                influenceAccountEntry.OccloCurrentInfluence = influenceAccountEntry.OccloInfluenceEarnedPerDay * intervalsCompleted;
                influenceAccountEntry.SerpentsHoldCurrentInfluence = influenceAccountEntry.SerpentsHoldInfluenceEarnedPerDay * intervalsCompleted;
                influenceAccountEntry.SkaraBraeCurrentInfluence = influenceAccountEntry.SkaraBraeInfluenceEarnedPerDay * intervalsCompleted;
                influenceAccountEntry.TrinsicCurrentInfluence = influenceAccountEntry.TrinsicInfluenceEarnedPerDay * intervalsCompleted;
                influenceAccountEntry.VesperCurrentInfluence = influenceAccountEntry.VesperInfluenceEarnedPerDay * intervalsCompleted;
                influenceAccountEntry.YewCurrentInfluence = influenceAccountEntry.YewInfluenceEarnedPerDay * intervalsCompleted;
            }

            arg.Mobile.SendMessage("Manually Set All Accounts to Have Earned Current Cycle's Influence Amounts.");
        }

        [Usage("RecalculateRegionControllersInfluence")]
        [Description("Recalculates the influence total of each region to determine controller")]
        public static void RecalculateRegionControllersInfluence(CommandEventArgs arg)
        {
            ResetCityControllers();

            foreach (InfluenceAccountEntry influenceAccountEntry in InfluencePersistance.m_InfluenceAccountEntries)
            {
                if (influenceAccountEntry.RealmCurrentInfluence > RealmControllerCurrentInfluence)
                {
                    RealmControllerCurrentInfluence = influenceAccountEntry.RealmCurrentInfluence;
                    RealmController = influenceAccountEntry;
                }

                if (influenceAccountEntry.BritainCurrentInfluence > BritainControllerCurrentInfluence)
                {
                    BritainControllerCurrentInfluence = influenceAccountEntry.BritainCurrentInfluence;
                    BritainController = influenceAccountEntry;
                }

                if (influenceAccountEntry.BuccaneersDenCurrentInfluence > BuccaneersDenControllerCurrentInfluence)
                {
                    BuccaneersDenControllerCurrentInfluence = influenceAccountEntry.BuccaneersDenCurrentInfluence;
                    BuccaneersDenController = influenceAccountEntry;
                }

                if (influenceAccountEntry.CoveCurrentInfluence > CoveControllerCurrentInfluence)
                {
                    CoveControllerCurrentInfluence = influenceAccountEntry.CoveCurrentInfluence;
                    CoveController = influenceAccountEntry;
                }

                if (influenceAccountEntry.JhelomCurrentInfluence > JhelomControllerCurrentInfluence)
                {
                    JhelomControllerCurrentInfluence = influenceAccountEntry.JhelomCurrentInfluence;
                    JhelomController = influenceAccountEntry;
                }

                if (influenceAccountEntry.MaginciaCurrentInfluence > MaginciaControllerCurrentInfluence)
                {
                    MaginciaControllerCurrentInfluence = influenceAccountEntry.MaginciaCurrentInfluence;
                    MaginciaController = influenceAccountEntry;
                }

                if (influenceAccountEntry.MinocCurrentInfluence > MinocControllerCurrentInfluence)
                {
                    MinocControllerCurrentInfluence = influenceAccountEntry.MinocCurrentInfluence;
                    MinocController = influenceAccountEntry;
                }

                if (influenceAccountEntry.MoonglowCurrentInfluence > MoonglowControllerCurrentInfluence)
                {
                    MoonglowControllerCurrentInfluence = influenceAccountEntry.MoonglowCurrentInfluence;
                    MoonglowController = influenceAccountEntry;
                }

                if (influenceAccountEntry.NujelmCurrentInfluence > NujelmControllerCurrentInfluence)
                {
                    NujelmControllerCurrentInfluence = influenceAccountEntry.NujelmCurrentInfluence;
                    NujelmController = influenceAccountEntry;
                }

                if (influenceAccountEntry.OccloCurrentInfluence > OccloControllerCurrentInfluence)
                {
                    OccloControllerCurrentInfluence = influenceAccountEntry.OccloCurrentInfluence;
                    OccloController = influenceAccountEntry;
                }

                if (influenceAccountEntry.SerpentsHoldCurrentInfluence > SerpentsHoldControllerCurrentInfluence)
                {
                    SerpentsHoldControllerCurrentInfluence = influenceAccountEntry.SerpentsHoldCurrentInfluence;
                    SerpentsHoldController = influenceAccountEntry;
                }

                if (influenceAccountEntry.SkaraBraeCurrentInfluence > SkaraBraeControllerCurrentInfluence)
                {
                    SkaraBraeControllerCurrentInfluence = influenceAccountEntry.SkaraBraeCurrentInfluence;
                    SkaraBraeController = influenceAccountEntry;
                }

                if (influenceAccountEntry.TrinsicCurrentInfluence > TrinsicControllerCurrentInfluence)
                {
                    TrinsicControllerCurrentInfluence = influenceAccountEntry.TrinsicCurrentInfluence;
                    TrinsicController = influenceAccountEntry;
                }

                if (influenceAccountEntry.VesperCurrentInfluence > VesperControllerCurrentInfluence)
                {
                    VesperControllerCurrentInfluence = influenceAccountEntry.VesperCurrentInfluence;
                    VesperController = influenceAccountEntry;
                }

                if (influenceAccountEntry.YewCurrentInfluence > YewControllerCurrentInfluence)
                {
                    YewControllerCurrentInfluence = influenceAccountEntry.YewCurrentInfluence;
                    YewController = influenceAccountEntry;
                }
            }

            arg.Mobile.SendMessage("Manually recalculated region controllers total influence.");
        }

        [Usage("ForceCycleCompletion")]
        [Description("Manually Forces a Cycle Complete and Begins the Next Cycle")]
        public static void ForceCycleCompletion(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            if (!Enabled)
            {
                player.SendMessage("The Influence System is currently disabled.");
                return;
            }

            NextTickCompletion = DateTime.UtcNow + TickDuration;
            NextCycleCompletion = DateTime.UtcNow + CycleDuration;
            NextIntervalCompletion = DateTime.UtcNow + IntervalDuration;

            ResolveCycle();

            player.SendMessage("Influence System cycle resolved and new cycle started.");    
        }

        [Usage("WipeInfluenceSystem")]
        [Description("Completely Wipes All Account Data for Influence System")]
        public static void WipeInfluenceSystem(CommandEventArgs arg)
        {
            //WARNING: THIS WILL WIPE ALL ACCOUNT DATA FOR ALL PLAYERS ON ALL MARKERS IN ALL TOWNS

            arg.Mobile.SendMessage("Wiping Influence System....");

            if (InfluenceTimer != null)
            {
                InfluenceTimer.Stop();
                InfluenceTimer = null;
            }

            Enabled = false;  
            
            foreach (InfluenceMarker marker in m_InfluenceMarkers)
            {
                marker.m_Controller = null;
                marker.m_ControllerInfluence = 0;
                marker.m_InfluenceEntries.Clear();
            }

            foreach (InfluenceAccountEntry entry in m_InfluenceAccountEntries)
            {
                entry.Delete();                
            }            

            m_InfluenceAccountEntries.Clear();

            ResetCityControllers();
            ClearPreviousControllers();                      

            arg.Mobile.SendMessage("Influence System Wiped and Disabled.");
        }
        
        [Usage("SpawnAllInfluenceMarkers")]
        [Description("Spawns All Pre-Determined Influence Markers")]
        public static void SpawnAllInfluenceMarkers(CommandEventArgs arg)
        {
            PlayerMobile pm_Mobile = arg.Mobile as PlayerMobile;

            if (pm_Mobile == null)
                return;

            int influenceMarkers = InfluencePersistance.m_InfluenceMarkers.Count;

            for (int a = 0; a < influenceMarkers; a++)
            {
                if (InfluencePersistance.m_InfluenceMarkers[0] != null)
                    InfluencePersistance.m_InfluenceMarkers[0].Delete();
            }

            #region Pre-Determined Influence Markers
            
            InfluenceMarker marker;
            XmlSpawner xmlSpawner;
            Point3D markerLocation;

            //Britain
            markerLocation = new Point3D(1431, 1693, 0);
            marker = new InfluenceMarker();   
            marker.BuildingName = "Britain Bank";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.Britain;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(1512, 1552, 25);
            marker = new InfluenceMarker();           
            marker.BuildingName = "Britain Stables";            
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.Britain;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(1467, 1765, -2);
            marker = new InfluenceMarker();           
            marker.BuildingName = "Britain Docks";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.Britain;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(1499, 1627, 10);
            marker = new InfluenceMarker();           
            marker.BuildingName = "Britain Inn";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.Britain;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(1427, 1552, 30);
            marker = new InfluenceMarker();            
            marker.BuildingName = "Britain Smith";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.Britain;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(1504, 1664, 20);
            marker = new InfluenceMarker();            
            marker.BuildingName = "Britain Alchemist";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.Britain;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(1476, 1692, 0);
            marker = new InfluenceMarker();            
            marker.BuildingName = "Britain Tailor";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.Britain;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(1432, 1727, 20);
            marker = new InfluenceMarker();            
            marker.BuildingName = "Britain Tavern";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.Britain;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(1553, 1688, 30);
            marker = new InfluenceMarker();            
            marker.BuildingName = "Britain Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.Britain;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(1547, 1720, 21);
            marker = new InfluenceMarker();            
            marker.BuildingName = "Britain Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.Britain;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(1546, 1752, 15);
            marker = new InfluenceMarker();            
            marker.BuildingName = "Britain Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.Britain;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(1622, 1768, 60);
            marker = new InfluenceMarker();            
            marker.BuildingName = "Britain Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.Britain;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            //Buccaneer's Den
            markerLocation = new Point3D(2734, 2192, 0);       
            marker = new InfluenceMarker();  
            marker.BuildingName = "Buccaneer's Den Bank";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.BuccaneeersDen;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(2678, 2169, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Buccaneer's Den Stables";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.BuccaneeersDen;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;
                        
            markerLocation = new Point3D(2744, 2163, -2);
            marker = new InfluenceMarker();
            marker.BuildingName = "Buccaneer's Den Docks";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.BuccaneeersDen;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;
            
            markerLocation = new Point3D(2713, 2104, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Buccaneer's Den Inn";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.BuccaneeersDen;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(2632, 2088, 10);
            marker = new InfluenceMarker();
            marker.BuildingName = "Buccaneer's Den Smith";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.BuccaneeersDen;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(2783, 2136, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Buccaneer's Den Alchemist";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.BuccaneeersDen;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(2680, 2157, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Buccaneer's Den Tailor";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.BuccaneeersDen;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(2688, 2233, 2);
            marker = new InfluenceMarker();
            marker.BuildingName = "Buccaneer's Den Tavern";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.BuccaneeersDen;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(2681, 2243, 2);
            marker = new InfluenceMarker();
            marker.BuildingName = "Buccaneer's Den Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.BuccaneeersDen;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(2672, 2233, 2);
            marker = new InfluenceMarker();
            marker.BuildingName = "Buccaneer's Den Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.BuccaneeersDen;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(2664, 2197, 4);
            marker = new InfluenceMarker();
            marker.BuildingName = "Buccaneer's Den Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.BuccaneeersDen;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(2664, 2096, 5);
            marker = new InfluenceMarker();
            marker.BuildingName = "Buccaneer's Den Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.BuccaneeersDen;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            //Cove
            markerLocation = new Point3D(2233, 1195, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Cove Bank";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.Cove;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(2239, 1166, -1);
            marker = new InfluenceMarker();
            marker.BuildingName = "Cove Stables";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.Cove;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(2251, 1191, -2);
            marker = new InfluenceMarker();
            marker.BuildingName = "Cove Docks";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.Cove;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(2232, 1216, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Cove Inn";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.Cove;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(2221, 1172, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Cove Smith";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.Cove;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(2248, 1233, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Cove Alchemist";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.Cove;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(2240, 1188, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Cove Tailor";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.Cove;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(2277, 1204, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Cove Tavern";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.Cove;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(2282, 1224, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Cove Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.Cove;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(2290, 1220, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Cove Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.Cove;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(2280, 1195, 20);
            marker = new InfluenceMarker();
            marker.BuildingName = "Cove Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.Cove;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(2280, 1203, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Cove Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.Cove;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            //Jhelom
            markerLocation = new Point3D(1320, 3780, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Jhelom Bank";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.Jhelom;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(1311, 3758, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Jhelom Stables";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.Jhelom;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(1369, 3882, -2);
            marker = new InfluenceMarker();
            marker.BuildingName = "Jhelom Docks";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.Jhelom;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(1376, 3820, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Jhelom Inn";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.Jhelom;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(1425, 3856, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Jhelom Smith";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.Jhelom;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(1432, 3990, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Jhelom Alchemist";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.Jhelom;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(1365, 3784, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Jhelom Tailor";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.Jhelom;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(1434, 3776, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Jhelom Tavern";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.Jhelom;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(1369, 3856, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Jhelom Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.Jhelom;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(1456, 3832, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Jhelom Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.Jhelom;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(1457, 3808, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Jhelom Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.Jhelom;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(1383, 3800, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Jhelom Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.Jhelom;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            //Magincia
            markerLocation = new Point3D(3733, 2160, 20);
            marker = new InfluenceMarker();
            marker.BuildingName = "Magincia Bank";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.Magincia;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(3728, 2107, 20);
            marker = new InfluenceMarker();
            marker.BuildingName = "Magincia Stables";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.Magincia;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(3669, 2270, 21);
            marker = new InfluenceMarker();
            marker.BuildingName = "Magincia Docks";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.Magincia;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(3704, 2170, 20);
            marker = new InfluenceMarker();
            marker.BuildingName = "Magincia Inn";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.Magincia;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(3672, 2129, 20);
            marker = new InfluenceMarker();
            marker.BuildingName = "Magincia Smith";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.Magincia;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(3710, 2216, 20);
            marker = new InfluenceMarker();
            marker.BuildingName = "Magincia Alchemist";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.Magincia;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(3672, 2236, 20);
            marker = new InfluenceMarker();
            marker.BuildingName = "Magincia Tailor";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.Magincia;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(3736, 2231, 20);
            marker = new InfluenceMarker();
            marker.BuildingName = "Magincia Tavern";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.Magincia;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(3718, 2096, 5);
            marker = new InfluenceMarker();
            marker.BuildingName = "Magincia Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.Magincia;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(3714, 2184, 20);
            marker = new InfluenceMarker();
            marker.BuildingName = "Magincia Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.Magincia;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(3795, 2232, 20);
            marker = new InfluenceMarker();
            marker.BuildingName = "Magincia Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.Magincia;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(3738, 2075, 5);
            marker = new InfluenceMarker();
            marker.BuildingName = "Magincia Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.Magincia;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            //Minoc
            markerLocation = new Point3D(2500, 560, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Minoc Bank";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.Minoc;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(2524, 379, 23);
            marker = new InfluenceMarker();
            marker.BuildingName = "Minoc Stables";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.Minoc;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(2455, 576, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Minoc Docks";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.Minoc;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(2486, 413, 15);
            marker = new InfluenceMarker();
            marker.BuildingName = "Minoc Inn";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.Minoc;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(2466, 569, 5);
            marker = new InfluenceMarker();
            marker.BuildingName = "Minoc Smith";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.Minoc;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(2502, 600, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Minoc Alchemist";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.Minoc;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(2480, 440, 15);
            marker = new InfluenceMarker();
            marker.BuildingName = "Minoc Tailor";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.Minoc;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(2473, 409, 15);
            marker = new InfluenceMarker();
            marker.BuildingName = "Minoc Tavern";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.Minoc;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(2569, 522, 15);
            marker = new InfluenceMarker();
            marker.BuildingName = "Minoc Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.Minoc;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(2426, 500, 15);
            marker = new InfluenceMarker();
            marker.BuildingName = "Minoc Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.Minoc;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(2484, 600, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Minoc Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.Minoc;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(2512, 633, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Minoc Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.Minoc;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            //Moonglow
            markerLocation = new Point3D(4476, 1174, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Moonglow Bank";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.Moonglow;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(4425, 1162, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Moonglow Stables";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.Moonglow;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(4414, 1045, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Moonglow Docks";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.Moonglow;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(new Point3D(4414, 1045, -2), pm_Mobile.Map);

            markerLocation = new Point3D(4400, 1168, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Moonglow Inn";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.Moonglow;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(4443, 1168, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Moonglow Smith";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.Moonglow;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(4419, 1120, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Moonglow Alchemist";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.Moonglow;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(4452, 1072, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Moonglow Tailor";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.Moonglow;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(4480, 1072, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Moonglow Inn";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.Moonglow;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(4479, 1230, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Moonglow Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.Moonglow;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(4400, 1207, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Moonglow Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.Moonglow;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(4416, 1211, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Moonglow Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.Moonglow;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(4529, 1064, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Moonglow Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.Moonglow;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            //Nujelm
            markerLocation = new Point3D(3768, 1318, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Nujel'm Bank";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.Nujelm;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(3796, 1258, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Nujel'm Stables";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.Nujelm;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(3805, 1273, 6);
            marker = new InfluenceMarker();
            marker.BuildingName = "Nujel'm Docks";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.Nujelm;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(3744, 1320, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Nujel'm Inn";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.Nujelm;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(3552, 1191, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Nujel'm Smith";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.Nujelm;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(3719, 1232, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Nujel'm Alchemist";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.Nujelm;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(3779, 1272, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Nujel'm Tailor";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.Nujelm;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(3783, 1232, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Nujel'm Tavern";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.Nujelm;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(3772, 1207, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Nujel'm Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.Nujelm;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(3732, 1392, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Nujel'm Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.Nujelm;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(3600, 1224, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Nujel'm Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.Nujelm;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(3632, 1225, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Nujel'm Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.Nujelm;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            //Occlo
            markerLocation = new Point3D(3688, 2520, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Occlo Bank";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.Occlo;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(3680, 2529, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Occlo Stables";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.Occlo;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(3644, 2650, -2);
            marker = new InfluenceMarker();
            marker.BuildingName = "Occlo Docks";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.Occlo;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(3672, 2624, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Occlo Inn";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.Occlo;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(3648, 2593, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Occlo Smith";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.Occlo;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(3635, 2536, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Occlo Alchemist";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.Occlo;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(3661, 2592, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Occlo Tailor";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.Occlo;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(3666, 2656, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Occlo Tavern";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.Occlo;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(new Point3D(3666, 2656, -2), pm_Mobile.Map);

            markerLocation = new Point3D(3654, 2480, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Occlo Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.Occlo;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(3640, 2512, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Occlo Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.Occlo;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(3704, 2592, 20);
            marker = new InfluenceMarker();
            marker.BuildingName = "Occlo Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.Occlo;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(3733, 2640, 40);
            marker = new InfluenceMarker();
            marker.BuildingName = "Occlo Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.Occlo;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            //Serpent's Hold
            markerLocation = new Point3D(2890, 3481, 15);
            marker = new InfluenceMarker();
            marker.BuildingName = "Serpent's Hold Bank";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.SerpentsHold;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(2912, 3519, 10);
            marker = new InfluenceMarker();
            marker.BuildingName = "Serpent's Hold Stables";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.SerpentsHold;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(2946, 3400, 1);
            marker = new InfluenceMarker();
            marker.BuildingName = "Serpent's Hold Docks";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.SerpentsHold;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(2976, 3412, 15);
            marker = new InfluenceMarker();
            marker.BuildingName = "Serpent's Hold Inn";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.SerpentsHold;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(3016, 3350, 15);
            marker = new InfluenceMarker();
            marker.BuildingName = "Serpent's Hold Smith";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.SerpentsHold;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(3014, 3360, 15);
            marker = new InfluenceMarker();
            marker.BuildingName = "Serpent's Hold Alchemist";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.SerpentsHold;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(2888, 3499, 10);
            marker = new InfluenceMarker();
            marker.BuildingName = "Serpent's Hold Tailor";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.SerpentsHold;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(2975, 3432, 15);
            marker = new InfluenceMarker();
            marker.BuildingName = "Serpent's Hold Tavern";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.SerpentsHold;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(2948, 3368, 15);
            marker = new InfluenceMarker();
            marker.BuildingName = "Serpent's Hold Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.SerpentsHold;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(3049, 3352, 15);
            marker = new InfluenceMarker();
            marker.BuildingName = "Serpent's Hold Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.SerpentsHold;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(3054, 3424, 15);
            marker = new InfluenceMarker();
            marker.BuildingName = "Serpent's Hold Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.SerpentsHold;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(3052, 3408, 15);
            marker = new InfluenceMarker();
            marker.BuildingName = "Serpent's Hold Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.SerpentsHold;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            //Skara Brae
            markerLocation = new Point3D(591, 2152, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Skara Brae Bank";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.SkaraBrae;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(574, 2128, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Skara Brae Stables";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.SkaraBrae;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(640, 2226, -2);
            marker = new InfluenceMarker();
            marker.BuildingName = "Skara Brae Docks";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.SkaraBrae;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(595, 2168, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Skara Brae Smith";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.SkaraBrae;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(616, 2237, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Skara Brae Inn";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.SkaraBrae;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(613, 2184, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Skara Brae Alchemist";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.SkaraBrae;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(653, 2192, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Skara Brae Tailor";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.SkaraBrae;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(568, 2183, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Skara Brae Tavern";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.SkaraBrae;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(576, 2214, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Skara Brae Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.SkaraBrae;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(752, 2264, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Skara Brae Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.SkaraBrae;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(789, 2224, 10);
            marker = new InfluenceMarker();
            marker.BuildingName = "Skara Brae Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.SkaraBrae;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(809, 2200, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Skara Brae Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.SkaraBrae;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            //Trinsic
            markerLocation = new Point3D(1819, 2828, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Trinsic Bank";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.Trinsic;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(1828, 2739, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Trinsic Stables";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.Trinsic;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(2060, 2848, -2);
            marker = new InfluenceMarker();
            marker.BuildingName = "Trinsic Docks";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.Trinsic;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(1848, 2744, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Trinsic Inn";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.Trinsic;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(1892, 2664, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Trinsic Smith";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.Trinsic;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(1852, 2714, 10);
            marker = new InfluenceMarker();
            marker.BuildingName = "Trinsic Alchemist";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.Trinsic;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(1990, 2846, 15);
            marker = new InfluenceMarker();
            marker.BuildingName = "Trinsic Tailor";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.Trinsic;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            marker = new InfluenceMarker();
            markerLocation = new Point3D(1942, 2796, 0);
            marker.BuildingName = "Trinsic Tavern";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.Trinsic;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(1855, 2837, 10);
            marker = new InfluenceMarker();
            marker.BuildingName = "Trinsic Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.Trinsic;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(1889, 2842, 20);
            marker = new InfluenceMarker();
            marker.BuildingName = "Trinsic Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.Trinsic;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(1944, 2733, 10);
            marker = new InfluenceMarker();
            marker.BuildingName = "Trinsic Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.Trinsic;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(1954, 2695, 20);
            marker = new InfluenceMarker();
            marker.BuildingName = "Trinsic Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.Trinsic;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            //Vesper
            markerLocation = new Point3D(2892, 680, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Vesper Bank";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.Vesper;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(2978, 610, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Vesper Stables";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.Vesper;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(3015, 821, -2);
            marker = new InfluenceMarker();
            marker.BuildingName = "Vesper Docks";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.Vesper;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(2976, 882, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Vesper Inn";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.Vesper;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(2840, 805, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Vesper Smith";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.Vesper;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(2881, 656, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Vesper Alchemist";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.Vesper;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(2968, 629, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Vesper Tailor";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.Vesper;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker; ;

            markerLocation = new Point3D(2916, 910, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Vesper Tavern";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.Vesper;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(2909, 720, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Vesper Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.Vesper;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(2856, 905, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Vesper Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.Vesper;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(2848, 928, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Vesper Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.Vesper;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(2752, 987, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Vesper Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.Vesper;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            //Yew
            markerLocation = new Point3D(649, 816, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Yew Bank";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.Yew;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(627, 858, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Yew Stables";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.Yew;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(591, 772, 1);
            marker = new InfluenceMarker();
            marker.BuildingName = "Yew Docks";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.Yew;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(608, 817, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Yew Inn";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.City;
            marker.MarkerRegion = IndexedRegionName.Yew;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(645, 857, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Yew Smith";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.Yew;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(638, 848, 20);
            marker = new InfluenceMarker();
            marker.BuildingName = "Yew Alchemist";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.Yew;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(452, 968, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Yew Tailor";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.Yew;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(624, 880, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Yew Tavern";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Vendor;
            marker.MarkerRegion = IndexedRegionName.Yew;
            marker.Facing = InfluenceMarker.MarkerFacing.East;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(641, 944, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Yew Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.Yew;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(657, 1008, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Yew Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.Yew;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(573, 1128, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Yew Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.Yew;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;

            markerLocation = new Point3D(608, 1185, 0);
            marker = new InfluenceMarker();
            marker.BuildingName = "Yew Residential";
            marker.BuildingType = InfluenceMarker.MarkerBuildingType.Basic;
            marker.MarkerRegion = IndexedRegionName.Yew;
            marker.Facing = InfluenceMarker.MarkerFacing.South;
            marker.Enabled = true;
            marker.MoveToWorld(markerLocation, pm_Mobile.Map);
            xmlSpawner = new XmlSpawner();
            xmlSpawner.MaxCount = 1; xmlSpawner.SpawnRange = 3;
            xmlSpawner.MoveToWorld(markerLocation, pm_Mobile.Map);
            marker.XMLSpawner = xmlSpawner;
            xmlSpawner.InfluenceMarker = marker;
            
            #endregion
            
            pm_Mobile.SendMessage("Spawned All Influence Markers.");
        }

        [Usage("DeleteAllInfluenceMarkers")]
        [Description("Deletes All Influence Markers")]
        public static void DeleteAllInfluenceMarkers(CommandEventArgs arg)
        {
            PlayerMobile pm_Mobile = arg.Mobile as PlayerMobile;

            if (pm_Mobile == null)
                return;

            int influenceMarkers = InfluencePersistance.m_InfluenceMarkers.Count;

            for (int a = 0; a < influenceMarkers; a++)
            {
                if (InfluencePersistance.m_InfluenceMarkers[0] != null)
                    InfluencePersistance.m_InfluenceMarkers[0].Delete();
            }
           
            pm_Mobile.SendMessage("Deleted All Influence Markers.");
        }

        [Usage("RecalculateAllMarkers")]
        [Description("Clears Players Town Daily Totals and Recalculates All Marker Values")]
        public static void RecalculateAllMarkers(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            foreach (InfluenceAccountEntry accountEntry in m_InfluenceAccountEntries)
            {
                if (accountEntry == null) continue;
                if (accountEntry.Deleted) continue;

                ClearPlayerInfluencePerDayScores(accountEntry);

                if (accountEntry.m_CurrentCharacter == null)
                {
                    List<PlayerMobile> m_AlternateCharacters = new List<PlayerMobile>();

                    if (accountEntry.m_AccountUsername == "" || accountEntry.m_AccountUsername == null)
                        continue;

                    Account account = Accounts.GetAccount(accountEntry.m_AccountUsername) as Account;

                    if (account == null) return;
                    if (account.accountMobiles == null) return;

                    for (int a = 0; a < account.accountMobiles.Length; a++)
                    {
                        PlayerMobile pm_Mobile = account.accountMobiles[a] as PlayerMobile;

                        if (pm_Mobile != null)
                        {
                            if (!pm_Mobile.Deleted)
                                m_AlternateCharacters.Add(pm_Mobile);
                        }
                    }
                   
                    if (m_AlternateCharacters.Count > 0)
                        accountEntry.m_CurrentCharacter = m_AlternateCharacters[0];
                }                
            }

            foreach (InfluenceMarker marker in m_InfluenceMarkers)
            {
                if (marker == null) continue;
                if (marker.Deleted) continue;

                marker.m_Controller = null;
                marker.m_ControllerInfluence = 0;               

                marker.DetermineMarkerController();

                foreach (InfluenceEntry entry in marker.m_InfluenceEntries)
                {
                    if (entry == null) continue;
                    if (entry.m_InfluenceAccountEntry == null) continue;

                    int influenceAtMarker = entry.m_PrestigeLevel + entry.m_InvestmentLevel;

                    if (marker.m_Controller == entry.m_InfluenceAccountEntry)                    
                        influenceAtMarker += InfluenceMarker.GetControllerInfluenceBonus(marker);

                    InfluenceMarker.UpdatePlayerInfluence(entry.m_InfluenceAccountEntry, influenceAtMarker, marker);
                }
            }

            player.SendMessage("Recalculated All Influence Markers.");
        }

        public static void UpdatePlayerInfluenceScores(InfluenceAccountEntry influenceAccountEntry)
        {
            if (influenceAccountEntry == null)
                return;

            double ticksPerInterval = InfluencePersistance.IntervalDuration.TotalMinutes / InfluencePersistance.TickDuration.TotalMinutes;
            double accountIntervalActivity = (double)influenceAccountEntry.TicksRecordedThisInterval / ticksPerInterval;

            //Override: If Player is Online for At Least Half of a Day, They Qualify as Receiving Their Full Day's Influence
            if (accountIntervalActivity >= .5)
                accountIntervalActivity = 1.0;
                        
            //Realm
            influenceAccountEntry.RealmCurrentInfluence += influenceAccountEntry.RealmInfluenceEarnedPerDay * accountIntervalActivity;

            if (influenceAccountEntry.RealmCurrentInfluence > RealmControllerCurrentInfluence)
            {
                RealmController = influenceAccountEntry;
                RealmControllerCurrentInfluence = influenceAccountEntry.RealmCurrentInfluence;
            }

            #region PlayerCityInfluenceUpdates

            //Britain
            influenceAccountEntry.BritainCurrentInfluence += influenceAccountEntry.BritainInfluenceEarnedPerDay * accountIntervalActivity;

            if (influenceAccountEntry.BritainCurrentInfluence > BritainControllerCurrentInfluence)
                SetRegionController(IndexedRegionName.Britain, influenceAccountEntry);            

            //Buccaneer's Den
            influenceAccountEntry.BuccaneersDenCurrentInfluence += influenceAccountEntry.BuccaneersDenInfluenceEarnedPerDay * accountIntervalActivity;

            if (influenceAccountEntry.BuccaneersDenCurrentInfluence > BuccaneersDenControllerCurrentInfluence)
                SetRegionController(IndexedRegionName.BuccaneeersDen, influenceAccountEntry);
            
            //Cove
            influenceAccountEntry.CoveCurrentInfluence += influenceAccountEntry.CoveInfluenceEarnedPerDay * accountIntervalActivity;

            if (influenceAccountEntry.CoveCurrentInfluence > CoveControllerCurrentInfluence)
                SetRegionController(IndexedRegionName.Cove, influenceAccountEntry);            

            //Jhelom
            influenceAccountEntry.JhelomCurrentInfluence += influenceAccountEntry.JhelomInfluenceEarnedPerDay * accountIntervalActivity;

            if (influenceAccountEntry.JhelomCurrentInfluence > JhelomControllerCurrentInfluence)
                SetRegionController(IndexedRegionName.Jhelom, influenceAccountEntry);            

            //Magincia
            influenceAccountEntry.MaginciaCurrentInfluence += influenceAccountEntry.MaginciaInfluenceEarnedPerDay * accountIntervalActivity;

            if (influenceAccountEntry.MaginciaCurrentInfluence > MaginciaControllerCurrentInfluence)
                SetRegionController(IndexedRegionName.Magincia, influenceAccountEntry);            

            //Minoc
            influenceAccountEntry.MinocCurrentInfluence += influenceAccountEntry.MinocInfluenceEarnedPerDay * accountIntervalActivity;

            if (influenceAccountEntry.MinocCurrentInfluence > MinocControllerCurrentInfluence)
                SetRegionController(IndexedRegionName.Minoc, influenceAccountEntry);            

            //Moonglow
            influenceAccountEntry.MoonglowCurrentInfluence += influenceAccountEntry.MoonglowInfluenceEarnedPerDay * accountIntervalActivity;

            if (influenceAccountEntry.MoonglowCurrentInfluence > MoonglowControllerCurrentInfluence)
                SetRegionController(IndexedRegionName.Moonglow, influenceAccountEntry);            

            //Nujelm
            influenceAccountEntry.NujelmCurrentInfluence += influenceAccountEntry.NujelmInfluenceEarnedPerDay * accountIntervalActivity;

            if (influenceAccountEntry.NujelmCurrentInfluence > NujelmControllerCurrentInfluence)
                SetRegionController(IndexedRegionName.Nujelm, influenceAccountEntry);            

            //Occlo
            influenceAccountEntry.OccloCurrentInfluence += influenceAccountEntry.OccloInfluenceEarnedPerDay * accountIntervalActivity;

            if (influenceAccountEntry.OccloCurrentInfluence > OccloControllerCurrentInfluence)
                SetRegionController(IndexedRegionName.Occlo, influenceAccountEntry);            

            //Serpent's Hold
            influenceAccountEntry.SerpentsHoldCurrentInfluence += influenceAccountEntry.SerpentsHoldInfluenceEarnedPerDay * accountIntervalActivity;

            if (influenceAccountEntry.SerpentsHoldCurrentInfluence > SerpentsHoldControllerCurrentInfluence)
                SetRegionController(IndexedRegionName.SerpentsHold, influenceAccountEntry);            

            //Skara Brae
            influenceAccountEntry.SkaraBraeCurrentInfluence += influenceAccountEntry.SkaraBraeInfluenceEarnedPerDay * accountIntervalActivity;

            if (influenceAccountEntry.SkaraBraeCurrentInfluence > SkaraBraeControllerCurrentInfluence)
                SetRegionController(IndexedRegionName.SkaraBrae, influenceAccountEntry);            

            //Trinsic
            influenceAccountEntry.TrinsicCurrentInfluence += influenceAccountEntry.TrinsicInfluenceEarnedPerDay * accountIntervalActivity;

            if (influenceAccountEntry.TrinsicCurrentInfluence > TrinsicControllerCurrentInfluence)
                SetRegionController(IndexedRegionName.Trinsic, influenceAccountEntry);            

            //Vesper
            influenceAccountEntry.VesperCurrentInfluence += influenceAccountEntry.VesperInfluenceEarnedPerDay * accountIntervalActivity;

            if (influenceAccountEntry.VesperCurrentInfluence > VesperControllerCurrentInfluence)
                SetRegionController(IndexedRegionName.Vesper, influenceAccountEntry);            

            //Yew
            influenceAccountEntry.YewCurrentInfluence += influenceAccountEntry.YewInfluenceEarnedPerDay * accountIntervalActivity;

            if (influenceAccountEntry.YewCurrentInfluence > YewControllerCurrentInfluence)
                SetRegionController(IndexedRegionName.Yew, influenceAccountEntry);

            #endregion
        }

        public static void ResolveCycle()
        {
            PreviousCycleCompletion = DateTime.UtcNow;

            #region Region Controllers Assigned

            //Realm
            if (PreviousRealmController != RealmController && PreviousRealmController != null)            
                ChangeRealmController(PreviousRealmController, false);   

            if (PreviousRealmController != RealmController && RealmController != null)
                ChangeRealmController(RealmController, true);  

            PreviousRealmController = RealmController;
            PreviousRealmControllerName = "-";

            if (RealmController != null)
            {
                if (RealmController.m_CurrentCharacter != null)
                    PreviousRealmControllerName = RealmController.m_CurrentCharacter.RawName;
            }

            PreviousRealmControllerInfluence = RealmControllerCurrentInfluence;

            //Britain
            if (PreviousBritainController != BritainController && PreviousBritainController != null)
                ChangeRegionController(IndexedRegionName.Britain, PreviousBritainController, false);

            if (PreviousBritainController != BritainController && BritainController != null)
                ChangeRegionController(IndexedRegionName.Britain, BritainController, true);  

            PreviousBritainController = BritainController;
            PreviousBritainControllerName = "-";

            if (BritainController != null)
            {
                if (BritainController.m_CurrentCharacter != null)
                    PreviousBritainControllerName = BritainController.m_CurrentCharacter.RawName;
            }               

            PreviousBritainControllerInfluence = BritainControllerCurrentInfluence;

            //Buccaneer's Den
            if (PreviousBuccaneersDenController != BuccaneersDenController && PreviousBuccaneersDenController != null)
                ChangeRegionController(IndexedRegionName.BuccaneeersDen, PreviousBuccaneersDenController, false);

            if (PreviousBuccaneersDenController != BuccaneersDenController && BuccaneersDenController != null)
                ChangeRegionController(IndexedRegionName.BuccaneeersDen, BuccaneersDenController, true);  

            PreviousBuccaneersDenController = BuccaneersDenController;
            PreviousBuccaneersDenControllerName = "-";

            if (BuccaneersDenController != null)
            {
                if (BuccaneersDenController.m_CurrentCharacter != null)
                    PreviousBuccaneersDenControllerName = BuccaneersDenController.m_CurrentCharacter.RawName;
            }   

            PreviousBuccaneersDenControllerInfluence = BuccaneersDenControllerCurrentInfluence;

            //Cove
            if (PreviousCoveController != CoveController && PreviousCoveController != null)
                ChangeRegionController(IndexedRegionName.Cove, PreviousCoveController, false);

            if (PreviousCoveController != CoveController && CoveController != null)
                ChangeRegionController(IndexedRegionName.Cove, CoveController, true); 

            PreviousCoveController = CoveController;
            PreviousCoveControllerName = "-";

            if (CoveController != null)
            {
                if (CoveController.m_CurrentCharacter != null)
                    PreviousCoveControllerName = CoveController.m_CurrentCharacter.RawName;
            }                

            PreviousCoveControllerInfluence = CoveControllerCurrentInfluence;

            //Jhelom
            if (PreviousJhelomController != JhelomController && PreviousJhelomController != null)
                ChangeRegionController(IndexedRegionName.Jhelom, PreviousJhelomController, false);

            if (PreviousJhelomController != JhelomController && JhelomController != null)
                ChangeRegionController(IndexedRegionName.Jhelom, JhelomController, true); 

            PreviousJhelomController = JhelomController;
            PreviousJhelomControllerName = "-";

            if (JhelomController != null)
            {
                if (JhelomController.m_CurrentCharacter != null)
                    PreviousJhelomControllerName = JhelomController.m_CurrentCharacter.RawName;
            }                

            PreviousJhelomControllerInfluence = JhelomControllerCurrentInfluence;

            //Magincia
            if (PreviousMaginciaController != MaginciaController && PreviousMaginciaController != null)
                ChangeRegionController(IndexedRegionName.Magincia, PreviousMaginciaController, false);

            if (PreviousMaginciaController != MaginciaController && MaginciaController != null)
                ChangeRegionController(IndexedRegionName.Magincia, MaginciaController, true); 
           
            PreviousMaginciaController = MaginciaController;
            PreviousMaginciaControllerName = "-";

            if (MaginciaController != null)
            {
                if (MaginciaController.m_CurrentCharacter != null)
                    PreviousMaginciaControllerName = MaginciaController.m_CurrentCharacter.RawName;
            }                

            PreviousMaginciaControllerInfluence = MaginciaControllerCurrentInfluence;

            //Minoc
            if (PreviousMinocController != MinocController && PreviousMinocController != null)
                ChangeRegionController(IndexedRegionName.Minoc, PreviousMinocController, false);

            if (PreviousMinocController != MinocController && MinocController != null)
                ChangeRegionController(IndexedRegionName.Minoc, MinocController, true); 

            PreviousMinocController = MinocController;
            PreviousMinocControllerName = "-";

            if (MinocController != null)
            {
                if (MinocController.m_CurrentCharacter != null)
                    PreviousMinocControllerName = MinocController.m_CurrentCharacter.RawName;
            }

            PreviousMinocControllerInfluence = MinocControllerCurrentInfluence;

            //Moonglow
            if (PreviousMoonglowController != MoonglowController && PreviousMoonglowController != null)
                ChangeRegionController(IndexedRegionName.Moonglow, PreviousMoonglowController, false);

            if (PreviousMoonglowController != MoonglowController && MoonglowController != null)
                ChangeRegionController(IndexedRegionName.Moonglow, MoonglowController, true);

            PreviousMoonglowController = MoonglowController;
            PreviousMoonglowControllerName = "-";

            if (MoonglowController != null)
            {
                if (MoonglowController.m_CurrentCharacter != null)
                    PreviousMoonglowControllerName = MoonglowController.m_CurrentCharacter.RawName;
            }

            PreviousMoonglowControllerInfluence = MoonglowControllerCurrentInfluence;

            //Nujelm
            if (PreviousNujelmController != NujelmController && PreviousNujelmController != null)
                ChangeRegionController(IndexedRegionName.Nujelm, PreviousNujelmController, false);

            if (PreviousNujelmController != NujelmController && NujelmController != null)
                ChangeRegionController(IndexedRegionName.Nujelm, NujelmController, true); 

            PreviousNujelmController = NujelmController;
            PreviousNujelmControllerName = "-";

            if (NujelmController != null)
            {
                if (NujelmController.m_CurrentCharacter != null)
                    PreviousNujelmControllerName = NujelmController.m_CurrentCharacter.RawName;
            }                

            PreviousNujelmControllerInfluence = NujelmControllerCurrentInfluence;

            //Occlo
            if (PreviousOccloController != OccloController && PreviousOccloController != null)
                ChangeRegionController(IndexedRegionName.Occlo, PreviousOccloController, false);

            if (PreviousOccloController != OccloController && OccloController != null)
                ChangeRegionController(IndexedRegionName.Occlo, OccloController, true); 

            PreviousOccloController = OccloController;
            PreviousOccloControllerName = "-";

            if (OccloController != null)
            {
                if (OccloController.m_CurrentCharacter != null)
                    PreviousOccloControllerName = OccloController.m_CurrentCharacter.RawName;
            }                

            PreviousOccloControllerInfluence = OccloControllerCurrentInfluence;

            //Serpent's Hold
            if (PreviousSerpentsHoldController != SerpentsHoldController && PreviousSerpentsHoldController != null)
                ChangeRegionController(IndexedRegionName.SerpentsHold, PreviousSerpentsHoldController, false);

            if (PreviousSerpentsHoldController != SerpentsHoldController && SerpentsHoldController != null)
                ChangeRegionController(IndexedRegionName.SerpentsHold, SerpentsHoldController, true); 

            PreviousSerpentsHoldController = SerpentsHoldController;
            PreviousSerpentsHoldControllerName = "-";

            if (SerpentsHoldController != null)
            {
                if (SerpentsHoldController.m_CurrentCharacter != null)
                    PreviousSerpentsHoldControllerName = SerpentsHoldController.m_CurrentCharacter.RawName;
            }                

            PreviousSerpentsHoldControllerInfluence = SerpentsHoldControllerCurrentInfluence;

            //Skara Brae
            if (PreviousSkaraBraeController != SkaraBraeController && PreviousSkaraBraeController != null)
                ChangeRegionController(IndexedRegionName.SkaraBrae, PreviousSkaraBraeController, false);

            if (PreviousSkaraBraeController != SkaraBraeController && SkaraBraeController != null)
                ChangeRegionController(IndexedRegionName.SkaraBrae, SkaraBraeController, true); 

            PreviousSkaraBraeController = SkaraBraeController;
            PreviousSkaraBraeControllerName = "-";

            if (SkaraBraeController != null)
            {
                if (SkaraBraeController.m_CurrentCharacter != null)
                    PreviousSkaraBraeControllerName = SkaraBraeController.m_CurrentCharacter.RawName;
            }                

            PreviousSkaraBraeControllerInfluence = SkaraBraeControllerCurrentInfluence;

            //Trinsic
            if (PreviousTrinsicController != TrinsicController && PreviousTrinsicController != null)
                ChangeRegionController(IndexedRegionName.Trinsic, PreviousTrinsicController, false);

            if (PreviousTrinsicController != TrinsicController && TrinsicController != null)
                ChangeRegionController(IndexedRegionName.Trinsic, TrinsicController, true); 

            PreviousTrinsicController = TrinsicController;
            PreviousTrinsicControllerName = "-";

            if (TrinsicController != null)
            {
                if (TrinsicController.m_CurrentCharacter != null)
                    PreviousTrinsicControllerName = TrinsicController.m_CurrentCharacter.RawName;
            }                

            PreviousTrinsicControllerInfluence = TrinsicControllerCurrentInfluence;

            //Vesper
            if (PreviousVesperController != VesperController && PreviousVesperController != null)
                ChangeRegionController(IndexedRegionName.Vesper, PreviousVesperController, false);

            if (PreviousVesperController != VesperController && VesperController != null)
                ChangeRegionController(IndexedRegionName.Vesper, VesperController, true); 

            PreviousVesperController = VesperController;
            PreviousVesperControllerName = "-";

            if (VesperController != null)
            {
                if (VesperController.m_CurrentCharacter != null)
                    PreviousVesperControllerName = VesperController.m_CurrentCharacter.RawName;
            }                

            PreviousVesperControllerInfluence = VesperControllerCurrentInfluence;

            //Yew
            if (PreviousYewController != YewController && PreviousYewController != null)
                ChangeRegionController(IndexedRegionName.Yew, PreviousYewController, false);

            if (PreviousYewController != YewController && YewController != null)
                ChangeRegionController(IndexedRegionName.Yew, YewController, true); 

            PreviousYewController = YewController;
            PreviousYewControllerName = "-";

            if (YewController != null)
            {
                if (YewController.m_CurrentCharacter != null)
                    PreviousYewControllerName = YewController.m_CurrentCharacter.RawName;
            }                

            PreviousYewControllerInfluence = YewControllerCurrentInfluence;

            #endregion

            foreach (InfluenceAccountEntry entry in m_InfluenceAccountEntries)
            {
                if (entry == null) continue;
                if (entry.Deleted) continue;

                int totalInfluence = (int)(Math.Ceiling(entry.RealmCurrentInfluence));
                int lotteryTickets = DetermineLotteryTicketsAwarded(totalInfluence);

                entry.InfluenceLotteryTickets += lotteryTickets;

                if (entry.m_CurrentCharacter != null && lotteryTickets > 0)
                {
                    Account account = entry.m_CurrentCharacter.Account as Account;

                    if (account == null)
                        continue;

                    for (int a = 0; a < (account.Length - 1); a++)
                    {
                        Mobile mobile = account.accountMobiles[a] as Mobile;

                        if (mobile == null) continue;
                        if (mobile.Deleted) continue;

                        PlayerMobile player = mobile as PlayerMobile;

                        if (player != null)
                        {
                            player.SendSound(0x5BC);
                            player.SendMessage(0x3F, "You have earned " + lotteryTickets.ToString() + " Influence Lottery Tickets.");
                        }                        
                    }
                }

                UpdatePlayerPreviousInfluenceScores(entry);
                ClearPlayerInfluenceScores(entry);
            }
            
            ResetCityControllers();
        }

        public static void ChangeRealmController(InfluenceAccountEntry influenceAccountEntry, bool isNewController)
        {
            if (influenceAccountEntry == null)
                return;

            string maleTitle = "Merchant Prince of the Realm";
            string femaleTitle = "Merchant Princess of the Realm";

            Account account = Accounts.GetAccount(influenceAccountEntry.m_AccountUsername) as Account;

            for (int i = 0; i < (account.Length - 1); i++)
            {
                Mobile mobile = account.accountMobiles[i] as Mobile;

                if (mobile != null)
                {
                    PlayerMobile pm_Mobile = mobile as PlayerMobile;

                    if (pm_Mobile != null)
                    {
                        string title = maleTitle;

                        if (pm_Mobile.Female)
                            title = femaleTitle;

                        if (isNewController)
                        {  
                        }

                        else
                        {                            
                        }
                    }
                }
            }
        }

        public static void ChangeRegionController(IndexedRegionName region, InfluenceAccountEntry influenceAccountEntry, bool isNewController)
        {
            if (influenceAccountEntry == null)
                return;

            string maleTitle = "Merchant Prince of " + GetRegionName(region);
            string femaleTitle = "Merchant Princess of " + GetRegionName(region);
            
            Account account = Accounts.GetAccount(influenceAccountEntry.m_AccountUsername) as Account;

            for (int i = 0; i < (account.Length - 1); i++)
            {
                Mobile mobile = account.accountMobiles[i] as Mobile;

                if (mobile != null)
                {
                    PlayerMobile pm_Mobile = mobile as PlayerMobile;

                    if (pm_Mobile != null)
                    {
                        string title = maleTitle;

                        if (pm_Mobile.Female)
                            title = femaleTitle;

                        if (isNewController)
                        {                                                
                        }

                        else
                        {                            
                        }
                    }
                }
            }
        }

        public static void ResetCityControllers()
        {
            RealmController = null;
            RealmControllerCurrentInfluence = 0;

            BritainController = null;
            BritainControllerCurrentInfluence = 0;
            
            BuccaneersDenController = null;
            BuccaneersDenControllerCurrentInfluence = 0;
            
            CoveController = null;
            CoveControllerCurrentInfluence = 0;
            
            JhelomController = null;
            JhelomControllerCurrentInfluence = 0;            
                        
            MaginciaController = null;
            MaginciaControllerCurrentInfluence = 0;
            
            MinocController = null;
            MinocControllerCurrentInfluence = 0;
            
            MoonglowController = null;
            MoonglowControllerCurrentInfluence = 0;
            
            NujelmController = null;
            NujelmControllerCurrentInfluence = 0;            

            OccloController = null;
            OccloControllerCurrentInfluence = 0;
            
            SerpentsHoldController = null;
            SerpentsHoldControllerCurrentInfluence = 0;            

            SkaraBraeController = null;
            SkaraBraeControllerCurrentInfluence = 0;            

            TrinsicController = null;
            TrinsicControllerCurrentInfluence = 0;            

            VesperController = null;
            VesperControllerCurrentInfluence = 0;            

            YewController = null;
            YewControllerCurrentInfluence = 0;            
        }

        public static void ClearPreviousControllers()
        {
            PreviousRealmController = null;
            PreviousRealmControllerName = "-";
            PreviousRealmControllerInfluence = 0;

            PreviousBritainController = null;
            PreviousBritainControllerName = "-";
            PreviousBritainControllerInfluence = 0;

            PreviousBuccaneersDenController = null;
            PreviousBuccaneersDenControllerName = "-";
            PreviousBuccaneersDenControllerInfluence = 0;

            PreviousCoveController = null;
            PreviousCoveControllerName = "-";
            PreviousCoveControllerInfluence = 0;

            PreviousJhelomController = null;
            PreviousJhelomControllerName = "-";
            PreviousJhelomControllerInfluence = 0;

            PreviousMaginciaController = null;
            PreviousMaginciaControllerName = "-";
            PreviousMaginciaControllerInfluence = 0;

            PreviousMinocController = null;
            PreviousMinocControllerName = "-";
            PreviousMinocControllerInfluence = 0;

            PreviousMoonglowController = null;
            PreviousMoonglowControllerName = "-";
            PreviousMoonglowControllerInfluence = 0;

            PreviousNujelmController = null;
            PreviousNujelmControllerName = "-";
            PreviousNujelmControllerInfluence = 0;

            PreviousOccloController = null;
            PreviousOccloControllerName = "-";
            PreviousOccloControllerInfluence = 0;

            PreviousSerpentsHoldController = null;
            PreviousSerpentsHoldControllerName = "-";
            PreviousSerpentsHoldControllerInfluence = 0;

            PreviousSkaraBraeController = null;
            PreviousSkaraBraeControllerName = "-";
            PreviousSkaraBraeControllerInfluence = 0;

            PreviousTrinsicController = null;
            PreviousTrinsicControllerName = "-";
            PreviousTrinsicControllerInfluence = 0;

            PreviousVesperController = null;
            PreviousVesperControllerName = "-";
            PreviousVesperControllerInfluence = 0;

            PreviousYewController = null;
            PreviousYewControllerName = "-";
            PreviousYewControllerInfluence = 0;
        }

        public static void SetRegionController(IndexedRegionName region, InfluenceAccountEntry influenceAccountEntry)
        {
            if (influenceAccountEntry == null)
                return;

            switch (region)
            {
                case IndexedRegionName.Britain: InfluencePersistance.BritainController = influenceAccountEntry; InfluencePersistance.BritainControllerCurrentInfluence = influenceAccountEntry.BritainCurrentInfluence; break;
                case IndexedRegionName.BuccaneeersDen: InfluencePersistance.BuccaneersDenController = influenceAccountEntry; InfluencePersistance.BuccaneersDenControllerCurrentInfluence = influenceAccountEntry.BuccaneersDenCurrentInfluence; break;
                case IndexedRegionName.Cove: InfluencePersistance.CoveController = influenceAccountEntry; InfluencePersistance.CoveControllerCurrentInfluence = influenceAccountEntry.CoveCurrentInfluence; break;
                case IndexedRegionName.Jhelom: InfluencePersistance.JhelomController = influenceAccountEntry; InfluencePersistance.JhelomControllerCurrentInfluence = influenceAccountEntry.JhelomCurrentInfluence; break;
                case IndexedRegionName.Magincia: InfluencePersistance.MaginciaController = influenceAccountEntry; InfluencePersistance.MaginciaControllerCurrentInfluence = influenceAccountEntry.MaginciaCurrentInfluence; break;
                case IndexedRegionName.Minoc: InfluencePersistance.MinocController = influenceAccountEntry; InfluencePersistance.MinocControllerCurrentInfluence = influenceAccountEntry.MinocCurrentInfluence; break;
                case IndexedRegionName.Moonglow: InfluencePersistance.MoonglowController = influenceAccountEntry; InfluencePersistance.MoonglowControllerCurrentInfluence = influenceAccountEntry.MoonglowCurrentInfluence; break;
                case IndexedRegionName.Nujelm: InfluencePersistance.NujelmController = influenceAccountEntry; InfluencePersistance.NujelmControllerCurrentInfluence = influenceAccountEntry.NujelmCurrentInfluence; break;
                case IndexedRegionName.Occlo: InfluencePersistance.OccloController = influenceAccountEntry; InfluencePersistance.OccloControllerCurrentInfluence = influenceAccountEntry.OccloCurrentInfluence; break;
                case IndexedRegionName.SerpentsHold: InfluencePersistance.SerpentsHoldController = influenceAccountEntry; InfluencePersistance.SerpentsHoldControllerCurrentInfluence = influenceAccountEntry.SerpentsHoldCurrentInfluence; break;
                case IndexedRegionName.SkaraBrae: InfluencePersistance.SkaraBraeController = influenceAccountEntry; InfluencePersistance.SkaraBraeControllerCurrentInfluence = influenceAccountEntry.SkaraBraeCurrentInfluence; break;
                case IndexedRegionName.Trinsic: InfluencePersistance.TrinsicController = influenceAccountEntry; InfluencePersistance.TrinsicControllerCurrentInfluence = influenceAccountEntry.TrinsicCurrentInfluence; break;
                case IndexedRegionName.Vesper: InfluencePersistance.VesperController = influenceAccountEntry; InfluencePersistance.VesperControllerCurrentInfluence = influenceAccountEntry.VesperCurrentInfluence; break;
                case IndexedRegionName.Yew: InfluencePersistance.YewController = influenceAccountEntry; InfluencePersistance.YewControllerCurrentInfluence = influenceAccountEntry.YewCurrentInfluence; break;
            }
        }

        public static void UpdatePlayerPrestigeAvailable(PlayerMobile player, IndexedRegionName region, int influenceAmount)
        {
            if (player.m_InfluenceAccountEntry == null)
                InfluencePersistance.CheckCreateInfluenceAccountEntry(player);

            if (player.m_InfluenceAccountEntry == null)
                return;

            switch (region)
            {
                case IndexedRegionName.Britain: player.m_InfluenceAccountEntry.BritainPrestigeAvailable += influenceAmount; break;
                case IndexedRegionName.BuccaneeersDen: player.m_InfluenceAccountEntry.BuccaneersDenPrestigeAvailable += influenceAmount; break;
                case IndexedRegionName.Cove: player.m_InfluenceAccountEntry.CovePrestigeAvailable += influenceAmount; break;
                case IndexedRegionName.Jhelom: player.m_InfluenceAccountEntry.JhelomPrestigeAvailable += influenceAmount; break;
                case IndexedRegionName.Magincia: player.m_InfluenceAccountEntry.MaginciaPrestigeAvailable += influenceAmount; break;
                case IndexedRegionName.Minoc: player.m_InfluenceAccountEntry.MinocPrestigeAvailable += influenceAmount; break;
                case IndexedRegionName.Moonglow: player.m_InfluenceAccountEntry.MoonglowPrestigeAvailable += influenceAmount; break;
                case IndexedRegionName.Nujelm: player.m_InfluenceAccountEntry.NujelmPrestigeAvailable += influenceAmount; break;
                case IndexedRegionName.Occlo: player.m_InfluenceAccountEntry.OccloPrestigeAvailable += influenceAmount; break;
                case IndexedRegionName.SerpentsHold: player.m_InfluenceAccountEntry.SerpentsHoldPrestigeAvailable += influenceAmount; break;
                case IndexedRegionName.SkaraBrae: player.m_InfluenceAccountEntry.SkaraBraePrestigeAvailable += influenceAmount; break;
                case IndexedRegionName.Trinsic: player.m_InfluenceAccountEntry.TrinsicPrestigeAvailable += influenceAmount; break;
                case IndexedRegionName.Vesper: player.m_InfluenceAccountEntry.VesperPrestigeAvailable += influenceAmount; break;
                case IndexedRegionName.Yew: player.m_InfluenceAccountEntry.YewPrestigeAvailable += influenceAmount; break;
            }
        }

        public static int GetPlayerPrestigeAvailable(PlayerMobile player, IndexedRegionName region)
        {
            int prestigeAvailable = 0;

            if (player.m_InfluenceAccountEntry == null)
                InfluencePersistance.CheckCreateInfluenceAccountEntry(player);

            if (player.m_InfluenceAccountEntry == null)
                return 0;

            switch (region)
            {
                case IndexedRegionName.Britain: prestigeAvailable = player.m_InfluenceAccountEntry.BritainPrestigeAvailable; break;
                case IndexedRegionName.BuccaneeersDen: prestigeAvailable = player.m_InfluenceAccountEntry.BuccaneersDenPrestigeAvailable; break;
                case IndexedRegionName.Cove: prestigeAvailable = player.m_InfluenceAccountEntry.CovePrestigeAvailable; break;
                case IndexedRegionName.Jhelom: prestigeAvailable = player.m_InfluenceAccountEntry.JhelomPrestigeAvailable; break;
                case IndexedRegionName.Magincia: prestigeAvailable = player.m_InfluenceAccountEntry.MaginciaPrestigeAvailable; break;
                case IndexedRegionName.Minoc: prestigeAvailable = player.m_InfluenceAccountEntry.MinocPrestigeAvailable; break;
                case IndexedRegionName.Moonglow: prestigeAvailable = player.m_InfluenceAccountEntry.MoonglowPrestigeAvailable; break;
                case IndexedRegionName.Nujelm: prestigeAvailable = player.m_InfluenceAccountEntry.NujelmPrestigeAvailable; break;
                case IndexedRegionName.Occlo: prestigeAvailable = player.m_InfluenceAccountEntry.OccloPrestigeAvailable; break;
                case IndexedRegionName.SerpentsHold: prestigeAvailable = player.m_InfluenceAccountEntry.SerpentsHoldPrestigeAvailable; break;
                case IndexedRegionName.SkaraBrae: prestigeAvailable = player.m_InfluenceAccountEntry.SkaraBraePrestigeAvailable; break;
                case IndexedRegionName.Trinsic: prestigeAvailable = player.m_InfluenceAccountEntry.TrinsicPrestigeAvailable; break;
                case IndexedRegionName.Vesper: prestigeAvailable = player.m_InfluenceAccountEntry.VesperPrestigeAvailable; break;
                case IndexedRegionName.Yew: prestigeAvailable = player.m_InfluenceAccountEntry.YewPrestigeAvailable; break;
            }

            return prestigeAvailable;
        }

        public static void OnLogin(PlayerMobile player)
        {
            InfluencePersistance.CheckCreateInfluenceAccountEntry(player);

            if (player == null)
                return;

            if (player.m_InfluenceAccountEntry.m_CurrentCharacter == null)
                player.m_InfluenceAccountEntry.m_CurrentCharacter = player;

            else if (player.m_InfluenceAccountEntry.m_CurrentCharacter.Deleted)
                player.m_InfluenceAccountEntry.m_CurrentCharacter = player;
        }

        public static void PlayerDeleted(PlayerMobile player)
        {
            if (player == null) return;
            if (player.m_InfluenceAccountEntry == null) return;

            List<PlayerMobile> m_AlternateCharacters = new List<PlayerMobile>();

            Account account = player.Account as Account;

            if (account == null) return;
            if (account.accountMobiles == null) return;

            for (int a = 0; a < account.accountMobiles.Length; a++)
            {
                PlayerMobile pm_Mobile = account.accountMobiles[a] as PlayerMobile;
                
                if (pm_Mobile != null)
                {
                    if (!pm_Mobile.Deleted)
                        m_AlternateCharacters.Add(pm_Mobile);
                }                
            }
           
            if (m_AlternateCharacters.Count > 0)
                player.m_InfluenceAccountEntry.m_CurrentCharacter = m_AlternateCharacters[0];            
        }

        public static void UpdatePlayerPreviousInfluenceScores(InfluenceAccountEntry influenceAccountEntry)
        {
            if (influenceAccountEntry == null)
                return;

            influenceAccountEntry.RealmPreviousInfluence = influenceAccountEntry.RealmCurrentInfluence;

            influenceAccountEntry.BritainPreviousInfluence = influenceAccountEntry.BritainCurrentInfluence;
            influenceAccountEntry.BuccaneersDenPreviousInfluence = influenceAccountEntry.BuccaneersDenCurrentInfluence;
            influenceAccountEntry.CovePreviousInfluence = influenceAccountEntry.CoveCurrentInfluence;
            influenceAccountEntry.JhelomPreviousInfluence = influenceAccountEntry.JhelomCurrentInfluence;
            influenceAccountEntry.MaginciaPreviousInfluence = influenceAccountEntry.MaginciaCurrentInfluence;
            influenceAccountEntry.MinocPreviousInfluence = influenceAccountEntry.MinocCurrentInfluence;
            influenceAccountEntry.MoonglowPreviousInfluence = influenceAccountEntry.MoonglowCurrentInfluence;
            influenceAccountEntry.NujelmPreviousInfluence = influenceAccountEntry.NujelmCurrentInfluence;
            influenceAccountEntry.OccloPreviousInfluence = influenceAccountEntry.OccloCurrentInfluence;
            influenceAccountEntry.SerpentsHoldPreviousInfluence = influenceAccountEntry.SerpentsHoldCurrentInfluence;
            influenceAccountEntry.SkaraBraePreviousInfluence = influenceAccountEntry.SkaraBraeCurrentInfluence;
            influenceAccountEntry.TrinsicPreviousInfluence = influenceAccountEntry.TrinsicCurrentInfluence;
            influenceAccountEntry.VesperPreviousInfluence = influenceAccountEntry.VesperCurrentInfluence;
            influenceAccountEntry.YewPreviousInfluence = influenceAccountEntry.YewCurrentInfluence;
        }
        
        public static void ClearPlayerInfluenceScores(InfluenceAccountEntry influenceAccountEntry)
        {
            if (influenceAccountEntry == null)
                return;

            influenceAccountEntry.TicksRecordedThisInterval = 0;

            influenceAccountEntry.RealmCurrentInfluence = 0;

            influenceAccountEntry.BritainCurrentInfluence = 0;
            influenceAccountEntry.BuccaneersDenCurrentInfluence = 0;
            influenceAccountEntry.CoveCurrentInfluence = 0;
            influenceAccountEntry.JhelomCurrentInfluence = 0;
            influenceAccountEntry.MaginciaCurrentInfluence = 0;
            influenceAccountEntry.MinocCurrentInfluence = 0;
            influenceAccountEntry.MoonglowCurrentInfluence = 0;
            influenceAccountEntry.NujelmCurrentInfluence = 0;
            influenceAccountEntry.OccloCurrentInfluence = 0;
            influenceAccountEntry.SerpentsHoldCurrentInfluence = 0;
            influenceAccountEntry.SkaraBraeCurrentInfluence = 0;
            influenceAccountEntry.TrinsicCurrentInfluence = 0;
            influenceAccountEntry.VesperCurrentInfluence = 0;
            influenceAccountEntry.YewCurrentInfluence = 0;
        }

        public static void ClearPlayerInfluencePerDayScores(InfluenceAccountEntry influenceAccountEntry)
        {
            if (influenceAccountEntry == null)
                return;

            influenceAccountEntry.RealmInfluenceEarnedPerDay = 0;

            influenceAccountEntry.BritainInfluenceEarnedPerDay = 0;
            influenceAccountEntry.BuccaneersDenInfluenceEarnedPerDay = 0;
            influenceAccountEntry.CoveInfluenceEarnedPerDay = 0;
            influenceAccountEntry.JhelomInfluenceEarnedPerDay = 0;
            influenceAccountEntry.MaginciaInfluenceEarnedPerDay = 0;
            influenceAccountEntry.MinocInfluenceEarnedPerDay = 0;
            influenceAccountEntry.MoonglowInfluenceEarnedPerDay = 0;
            influenceAccountEntry.NujelmInfluenceEarnedPerDay = 0;
            influenceAccountEntry.OccloInfluenceEarnedPerDay = 0;
            influenceAccountEntry.SerpentsHoldInfluenceEarnedPerDay = 0;
            influenceAccountEntry.SkaraBraeInfluenceEarnedPerDay = 0;
            influenceAccountEntry.TrinsicInfluenceEarnedPerDay = 0;
            influenceAccountEntry.VesperInfluenceEarnedPerDay = 0;
            influenceAccountEntry.YewInfluenceEarnedPerDay = 0;
        }

        public static double GetPlayerRegionCurrentInfluence(PlayerMobile player, IndexedRegionName markerRegion)
        {
            if (player.m_InfluenceAccountEntry == null)
                InfluencePersistance.CheckCreateInfluenceAccountEntry(player);

            if (player.m_InfluenceAccountEntry == null)
                return 0;

            double regionCurrentInfluence = 0;

            switch (markerRegion)
            {
                case IndexedRegionName.Britain: regionCurrentInfluence = player.m_InfluenceAccountEntry.BritainCurrentInfluence; break;
                case IndexedRegionName.BuccaneeersDen: regionCurrentInfluence = player.m_InfluenceAccountEntry.BuccaneersDenCurrentInfluence; break;
                case IndexedRegionName.Cove: regionCurrentInfluence = player.m_InfluenceAccountEntry.CoveCurrentInfluence; break;
                case IndexedRegionName.Jhelom: regionCurrentInfluence = player.m_InfluenceAccountEntry.JhelomCurrentInfluence; break;
                case IndexedRegionName.Magincia: regionCurrentInfluence = player.m_InfluenceAccountEntry.MaginciaCurrentInfluence; break;
                case IndexedRegionName.Minoc: regionCurrentInfluence = player.m_InfluenceAccountEntry.MinocCurrentInfluence; break;
                case IndexedRegionName.Moonglow: regionCurrentInfluence = player.m_InfluenceAccountEntry.MoonglowCurrentInfluence; break;
                case IndexedRegionName.Nujelm: regionCurrentInfluence = player.m_InfluenceAccountEntry.NujelmCurrentInfluence; break;
                case IndexedRegionName.Occlo: regionCurrentInfluence = player.m_InfluenceAccountEntry.OccloCurrentInfluence; break;
                case IndexedRegionName.SerpentsHold: regionCurrentInfluence = player.m_InfluenceAccountEntry.SerpentsHoldCurrentInfluence; break;
                case IndexedRegionName.SkaraBrae: regionCurrentInfluence = player.m_InfluenceAccountEntry.SkaraBraeCurrentInfluence; break;
                case IndexedRegionName.Trinsic: regionCurrentInfluence = player.m_InfluenceAccountEntry.TrinsicCurrentInfluence; break;
                case IndexedRegionName.Vesper: regionCurrentInfluence = player.m_InfluenceAccountEntry.VesperCurrentInfluence; break;
                case IndexedRegionName.Yew: regionCurrentInfluence = player.m_InfluenceAccountEntry.YewCurrentInfluence; break;
            }

            return regionCurrentInfluence;
        }

        public static double GetPlayerInfluenceEarnedPerDay(PlayerMobile player, IndexedRegionName markerRegion)
        {
            if (player.m_InfluenceAccountEntry == null)
                InfluencePersistance.CheckCreateInfluenceAccountEntry(player);

            if (player.m_InfluenceAccountEntry == null)
                return 0;

            double regionInfluencePerDay = 0;

            switch (markerRegion)
            {
                case IndexedRegionName.Britain: regionInfluencePerDay = player.m_InfluenceAccountEntry.BritainInfluenceEarnedPerDay; break;
                case IndexedRegionName.BuccaneeersDen: regionInfluencePerDay = player.m_InfluenceAccountEntry.BuccaneersDenInfluenceEarnedPerDay; break;
                case IndexedRegionName.Cove: regionInfluencePerDay = player.m_InfluenceAccountEntry.CoveInfluenceEarnedPerDay; break;
                case IndexedRegionName.Jhelom: regionInfluencePerDay = player.m_InfluenceAccountEntry.JhelomInfluenceEarnedPerDay; break;
                case IndexedRegionName.Magincia: regionInfluencePerDay = player.m_InfluenceAccountEntry.MaginciaInfluenceEarnedPerDay; break;
                case IndexedRegionName.Minoc: regionInfluencePerDay = player.m_InfluenceAccountEntry.MinocInfluenceEarnedPerDay; break;
                case IndexedRegionName.Moonglow: regionInfluencePerDay = player.m_InfluenceAccountEntry.MoonglowInfluenceEarnedPerDay; break;
                case IndexedRegionName.Nujelm: regionInfluencePerDay = player.m_InfluenceAccountEntry.NujelmInfluenceEarnedPerDay; break;
                case IndexedRegionName.Occlo: regionInfluencePerDay = player.m_InfluenceAccountEntry.OccloInfluenceEarnedPerDay; break;
                case IndexedRegionName.SerpentsHold: regionInfluencePerDay = player.m_InfluenceAccountEntry.SerpentsHoldInfluenceEarnedPerDay; break;
                case IndexedRegionName.SkaraBrae: regionInfluencePerDay = player.m_InfluenceAccountEntry.SkaraBraeInfluenceEarnedPerDay; break;
                case IndexedRegionName.Trinsic: regionInfluencePerDay = player.m_InfluenceAccountEntry.TrinsicInfluenceEarnedPerDay; break;
                case IndexedRegionName.Vesper: regionInfluencePerDay = player.m_InfluenceAccountEntry.VesperInfluenceEarnedPerDay; break;
                case IndexedRegionName.Yew: regionInfluencePerDay = player.m_InfluenceAccountEntry.YewInfluenceEarnedPerDay; break;
            }

            return regionInfluencePerDay;
        }

        public static string GetRegionName(IndexedRegionName region)
        {
            string regionName = "";

            switch (region)
            {
                case IndexedRegionName.Britain: regionName = "Britain"; break;
                case IndexedRegionName.BuccaneeersDen: regionName = "Buccaneer's Den"; break;
                case IndexedRegionName.Cove: regionName = "Cove"; break;
                case IndexedRegionName.Jhelom: regionName = "Jhelom"; break;
                case IndexedRegionName.Magincia: regionName = "Magincia"; break;
                case IndexedRegionName.Minoc: regionName = "Minoc"; break;
                case IndexedRegionName.Moonglow: regionName = "Moonglow"; break;
                case IndexedRegionName.Nujelm: regionName = "Nujel'm"; break;
                case IndexedRegionName.Occlo: regionName = "Occlo"; break;
                case IndexedRegionName.SerpentsHold: regionName = "Serpent's Hold"; break;
                case IndexedRegionName.SkaraBrae: regionName = "Skara Brae"; break;
                case IndexedRegionName.Trinsic: regionName = "Trinsic"; break;
                case IndexedRegionName.Vesper: regionName = "Vesper"; break;
                case IndexedRegionName.Yew: regionName = "Yew"; break;
            }

            return regionName;
        }

        public static IndexedRegionName GetRandomRegion()
        {
            IndexedRegionName region = Regions[Utility.RandomMinMax(0, Regions.Length - 1)];

            return region;
        }
        
        public static InfluenceAccountEntry GetCityController(IndexedRegionName region) 
        {
            InfluenceAccountEntry controller = null;

            switch (region)
            {
                case IndexedRegionName.Britain: controller = BritainController; break;
                case IndexedRegionName.BuccaneeersDen: controller = BuccaneersDenController; break;
                case IndexedRegionName.Cove: controller = CoveController; break;
                case IndexedRegionName.Jhelom: controller = JhelomController; break;
                case IndexedRegionName.Magincia: controller = MaginciaController; break;
                case IndexedRegionName.Minoc: controller = MinocController; break;
                case IndexedRegionName.Moonglow: controller = MoonglowController; break;
                case IndexedRegionName.Nujelm: controller = NujelmController; break;
                case IndexedRegionName.Occlo: controller = OccloController; break;
                case IndexedRegionName.SerpentsHold: controller = SerpentsHoldController; break;
                case IndexedRegionName.SkaraBrae: controller = SkaraBraeController; break;
                case IndexedRegionName.Trinsic: controller = TrinsicController; break;
                case IndexedRegionName.Vesper: controller = VesperController; break;
                case IndexedRegionName.Yew: controller = YewController; break;
            }

            return controller;
        }

        public static InfluenceAccountEntry GetPreviousCityController(IndexedRegionName region)
        {
            InfluenceAccountEntry controller = null;

            switch (region)
            {
                case IndexedRegionName.Britain: controller = PreviousBritainController; break;
                case IndexedRegionName.BuccaneeersDen: controller = PreviousBuccaneersDenController; break;
                case IndexedRegionName.Cove: controller = PreviousCoveController; break;
                case IndexedRegionName.Jhelom: controller = PreviousJhelomController; break;
                case IndexedRegionName.Magincia: controller = PreviousMaginciaController; break;
                case IndexedRegionName.Minoc: controller = PreviousMinocController; break;
                case IndexedRegionName.Moonglow: controller = PreviousMoonglowController; break;
                case IndexedRegionName.Nujelm: controller = PreviousNujelmController; break;
                case IndexedRegionName.Occlo: controller = PreviousOccloController; break;
                case IndexedRegionName.SerpentsHold: controller = PreviousSerpentsHoldController; break;
                case IndexedRegionName.SkaraBrae: controller = PreviousSkaraBraeController; break;
                case IndexedRegionName.Trinsic: controller = PreviousTrinsicController; break;
                case IndexedRegionName.Vesper: controller = PreviousVesperController; break;
                case IndexedRegionName.Yew: controller = PreviousYewController; break;
            }

            return controller;
        }

        public static string GetPreviousCityControllerName(IndexedRegionName region)
        {
            string controllerName = "-";

            switch (region)
            {
                case IndexedRegionName.Britain: controllerName = PreviousBritainControllerName; break;
                case IndexedRegionName.BuccaneeersDen: controllerName = PreviousBuccaneersDenControllerName; break;
                case IndexedRegionName.Cove: controllerName = PreviousCoveControllerName; break;
                case IndexedRegionName.Jhelom: controllerName = PreviousJhelomControllerName; break;
                case IndexedRegionName.Magincia: controllerName = PreviousMaginciaControllerName; break;
                case IndexedRegionName.Minoc: controllerName = PreviousMinocControllerName; break;
                case IndexedRegionName.Moonglow: controllerName = PreviousMoonglowControllerName; break;
                case IndexedRegionName.Nujelm: controllerName = PreviousNujelmControllerName; break;
                case IndexedRegionName.Occlo: controllerName = PreviousOccloControllerName; break;
                case IndexedRegionName.SerpentsHold: controllerName = PreviousSerpentsHoldControllerName; break;
                case IndexedRegionName.SkaraBrae: controllerName = PreviousSkaraBraeControllerName; break;
                case IndexedRegionName.Trinsic: controllerName = PreviousTrinsicControllerName; break;
                case IndexedRegionName.Vesper: controllerName = PreviousVesperControllerName; break;
                case IndexedRegionName.Yew: controllerName = PreviousYewControllerName; break;
            }

            return controllerName;
        }

        public static double GetCityControllerInfluence(IndexedRegionName region)
        {
            double controllerInfluence = 0;

            switch (region)
            {
                case IndexedRegionName.Britain: controllerInfluence = BritainControllerCurrentInfluence; break;
                case IndexedRegionName.BuccaneeersDen: controllerInfluence = BuccaneersDenControllerCurrentInfluence; break;
                case IndexedRegionName.Cove: controllerInfluence = CoveControllerCurrentInfluence; break;
                case IndexedRegionName.Jhelom: controllerInfluence = JhelomControllerCurrentInfluence; break;
                case IndexedRegionName.Magincia: controllerInfluence = MaginciaControllerCurrentInfluence; break;
                case IndexedRegionName.Minoc: controllerInfluence = MinocControllerCurrentInfluence; break;
                case IndexedRegionName.Moonglow: controllerInfluence = MoonglowControllerCurrentInfluence; break;
                case IndexedRegionName.Nujelm: controllerInfluence = NujelmControllerCurrentInfluence; break;
                case IndexedRegionName.Occlo: controllerInfluence = OccloControllerCurrentInfluence; break;
                case IndexedRegionName.SerpentsHold: controllerInfluence = SerpentsHoldControllerCurrentInfluence; break;
                case IndexedRegionName.SkaraBrae: controllerInfluence = SkaraBraeControllerCurrentInfluence; break;
                case IndexedRegionName.Trinsic: controllerInfluence = TrinsicControllerCurrentInfluence; break;
                case IndexedRegionName.Vesper: controllerInfluence = VesperControllerCurrentInfluence; break;
                case IndexedRegionName.Yew: controllerInfluence = YewControllerCurrentInfluence; break;
            }

            return controllerInfluence;
        }

        public static double GetPreviousCityControllerInfluence(IndexedRegionName region)
        {
            double controllerInfluence = 0;

            switch (region)
            {
                case IndexedRegionName.Britain: controllerInfluence = PreviousBritainControllerInfluence; break;
                case IndexedRegionName.BuccaneeersDen: controllerInfluence = PreviousBuccaneersDenControllerInfluence; break;
                case IndexedRegionName.Cove: controllerInfluence = PreviousCoveControllerInfluence; break;
                case IndexedRegionName.Jhelom: controllerInfluence = PreviousJhelomControllerInfluence; break;
                case IndexedRegionName.Magincia: controllerInfluence = PreviousMaginciaControllerInfluence; break;
                case IndexedRegionName.Minoc: controllerInfluence = PreviousMinocControllerInfluence; break;
                case IndexedRegionName.Moonglow: controllerInfluence = PreviousMoonglowControllerInfluence; break;
                case IndexedRegionName.Nujelm: controllerInfluence = PreviousNujelmControllerInfluence; break;
                case IndexedRegionName.Occlo: controllerInfluence = PreviousOccloControllerInfluence; break;
                case IndexedRegionName.SerpentsHold: controllerInfluence = PreviousSerpentsHoldControllerInfluence; break;
                case IndexedRegionName.SkaraBrae: controllerInfluence = PreviousSkaraBraeControllerInfluence; break;
                case IndexedRegionName.Trinsic: controllerInfluence = PreviousTrinsicControllerInfluence; break;
                case IndexedRegionName.Vesper: controllerInfluence = PreviousVesperControllerInfluence; break;
                case IndexedRegionName.Yew: controllerInfluence = PreviousYewControllerInfluence; break;
            }

            return controllerInfluence;
        }
        
        public static double GetPlayerCityInfluence(PlayerMobile player, IndexedRegionName region)
        {
            double influenceAmount = 0;

            switch (region)
            {
                case IndexedRegionName.Britain: influenceAmount = player.m_InfluenceAccountEntry.BritainCurrentInfluence; break;
                case IndexedRegionName.BuccaneeersDen: influenceAmount = player.m_InfluenceAccountEntry.BuccaneersDenCurrentInfluence; break;
                case IndexedRegionName.Cove: influenceAmount = player.m_InfluenceAccountEntry.CoveCurrentInfluence; break;
                case IndexedRegionName.Jhelom: influenceAmount = player.m_InfluenceAccountEntry.JhelomCurrentInfluence; break;
                case IndexedRegionName.Magincia: influenceAmount = player.m_InfluenceAccountEntry.MaginciaCurrentInfluence; break;
                case IndexedRegionName.Minoc: influenceAmount = player.m_InfluenceAccountEntry.MinocCurrentInfluence; break;
                case IndexedRegionName.Moonglow: influenceAmount = player.m_InfluenceAccountEntry.MoonglowCurrentInfluence; break;
                case IndexedRegionName.Nujelm: influenceAmount = player.m_InfluenceAccountEntry.NujelmCurrentInfluence; break;
                case IndexedRegionName.Occlo: influenceAmount = player.m_InfluenceAccountEntry.OccloCurrentInfluence; break;
                case IndexedRegionName.SerpentsHold: influenceAmount = player.m_InfluenceAccountEntry.SerpentsHoldCurrentInfluence; break;
                case IndexedRegionName.SkaraBrae: influenceAmount = player.m_InfluenceAccountEntry.SkaraBraeCurrentInfluence; break;
                case IndexedRegionName.Trinsic: influenceAmount = player.m_InfluenceAccountEntry.TrinsicCurrentInfluence; break;
                case IndexedRegionName.Vesper: influenceAmount = player.m_InfluenceAccountEntry.VesperCurrentInfluence; break;
                case IndexedRegionName.Yew: influenceAmount = player.m_InfluenceAccountEntry.YewCurrentInfluence; break;
            }

            return influenceAmount;
        }

        public static double GetPreviousPlayerCityInfluence(PlayerMobile player, IndexedRegionName region)
        {
            double influenceAmount = 0;

            switch (region)
            {
                case IndexedRegionName.Britain: influenceAmount = player.m_InfluenceAccountEntry.BritainPreviousInfluence; break;
                case IndexedRegionName.BuccaneeersDen: influenceAmount = player.m_InfluenceAccountEntry.BuccaneersDenPreviousInfluence; break;
                case IndexedRegionName.Cove: influenceAmount = player.m_InfluenceAccountEntry.CovePreviousInfluence; break;
                case IndexedRegionName.Jhelom: influenceAmount = player.m_InfluenceAccountEntry.JhelomPreviousInfluence; break;
                case IndexedRegionName.Magincia: influenceAmount = player.m_InfluenceAccountEntry.MaginciaPreviousInfluence; break;
                case IndexedRegionName.Minoc: influenceAmount = player.m_InfluenceAccountEntry.MinocPreviousInfluence; break;
                case IndexedRegionName.Moonglow: influenceAmount = player.m_InfluenceAccountEntry.MoonglowPreviousInfluence; break;
                case IndexedRegionName.Nujelm: influenceAmount = player.m_InfluenceAccountEntry.NujelmPreviousInfluence; break;
                case IndexedRegionName.Occlo: influenceAmount = player.m_InfluenceAccountEntry.OccloPreviousInfluence; break;
                case IndexedRegionName.SerpentsHold: influenceAmount = player.m_InfluenceAccountEntry.SerpentsHoldPreviousInfluence; break;
                case IndexedRegionName.SkaraBrae: influenceAmount = player.m_InfluenceAccountEntry.SkaraBraePreviousInfluence; break;
                case IndexedRegionName.Trinsic: influenceAmount = player.m_InfluenceAccountEntry.TrinsicPreviousInfluence; break;
                case IndexedRegionName.Vesper: influenceAmount = player.m_InfluenceAccountEntry.VesperPreviousInfluence; break;
                case IndexedRegionName.Yew: influenceAmount = player.m_InfluenceAccountEntry.YewPreviousInfluence; break;
            }

            return influenceAmount;
        }
        
        public static int GetPlayerCityInfluencePerDay(PlayerMobile player, IndexedRegionName region)
        {
            int influenceAmount = 0;

            switch (region)
            {
                case IndexedRegionName.Britain: influenceAmount = player.m_InfluenceAccountEntry.BritainInfluenceEarnedPerDay; break;
                case IndexedRegionName.BuccaneeersDen: influenceAmount = player.m_InfluenceAccountEntry.BuccaneersDenInfluenceEarnedPerDay; break;
                case IndexedRegionName.Cove: influenceAmount = player.m_InfluenceAccountEntry.CoveInfluenceEarnedPerDay; break;
                case IndexedRegionName.Jhelom: influenceAmount = player.m_InfluenceAccountEntry.JhelomInfluenceEarnedPerDay; break;
                case IndexedRegionName.Magincia: influenceAmount = player.m_InfluenceAccountEntry.MaginciaInfluenceEarnedPerDay; break;
                case IndexedRegionName.Minoc: influenceAmount = player.m_InfluenceAccountEntry.MinocInfluenceEarnedPerDay; break;
                case IndexedRegionName.Moonglow: influenceAmount = player.m_InfluenceAccountEntry.MoonglowInfluenceEarnedPerDay; break;
                case IndexedRegionName.Nujelm: influenceAmount = player.m_InfluenceAccountEntry.NujelmInfluenceEarnedPerDay; break;
                case IndexedRegionName.Occlo: influenceAmount = player.m_InfluenceAccountEntry.OccloInfluenceEarnedPerDay; break;
                case IndexedRegionName.SerpentsHold: influenceAmount = player.m_InfluenceAccountEntry.SerpentsHoldInfluenceEarnedPerDay; break;
                case IndexedRegionName.SkaraBrae: influenceAmount = player.m_InfluenceAccountEntry.SkaraBraeInfluenceEarnedPerDay; break;
                case IndexedRegionName.Trinsic: influenceAmount = player.m_InfluenceAccountEntry.TrinsicInfluenceEarnedPerDay; break;
                case IndexedRegionName.Vesper: influenceAmount = player.m_InfluenceAccountEntry.VesperInfluenceEarnedPerDay; break;
                case IndexedRegionName.Yew: influenceAmount = player.m_InfluenceAccountEntry.YewInfluenceEarnedPerDay; break;
            }

            return influenceAmount;
        }

        public static int DetermineLotteryTicketsAwarded(int influenceTotal)
        {   
            int ticketsAwarded = 0;

            for (int a = 0; a < lotteryTicketTiers.Length; a++)
            {
                if (influenceTotal >= lotteryTicketTiers[a])
                    ticketsAwarded = a + 1;
            }
            
            return ticketsAwarded; 
        }

        public static void CheckCreateInfluenceAccountEntry(PlayerMobile player)
        {
            if (player == null) return;
            if (player.Account == null) return;

            string accountName = player.Account.Username;

            InfluenceAccountEntry influenceAccountEntry = null;

            bool foundInfluenceEntry = false;

            foreach (InfluenceAccountEntry entry in m_InfluenceAccountEntries)
            {
                if (entry == null) continue;
                if (entry.m_AccountUsername == accountName)
                {
                    player.m_InfluenceAccountEntry = entry;
                    return;
                }
            }

            if (!foundInfluenceEntry)
            {
                InfluenceAccountEntry newEntry = new InfluenceAccountEntry(accountName, player);
                m_InfluenceAccountEntries.Add(newEntry);

                Account account = player.Account as Account;

                for (int a = 0; a < account.accountMobiles.Length; a++)
                {
                    Mobile mobile = account.accountMobiles[a] as Mobile;

                    if (mobile == null)
                        continue;
                    
                    PlayerMobile pm_Mobile = mobile as PlayerMobile;

                    if (pm_Mobile != null)
                        pm_Mobile.m_InfluenceAccountEntry = newEntry;                    
                }   
            }
        }
        
        public static InfluenceAccountEntry FindInfluenceAccountEntry(PlayerMobile player)
        {
            if (player == null) return null;
            if (player.Account == null) return null;

            string accountName = player.Account.Username;

            InfluenceAccountEntry influenceAccountEntry = null;

            foreach (InfluenceAccountEntry entry in m_InfluenceAccountEntries)
            {
                if (entry.m_AccountUsername == accountName)
                {
                    influenceAccountEntry = entry;
                    break;
                }
            }

            return influenceAccountEntry;
        }

        public static void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(0); //Version
            
            //Version 0   
            writer.Write(Enabled);

            writer.Write(NextIntervalCompletion);
            writer.Write(NextCycleCompletion);
            writer.Write(PreviousCycleCompletion);

            writer.Write((int)m_InfluenceMarkers.Count);
            foreach (InfluenceMarker influenceMarker in m_InfluenceMarkers)
            {
                writer.Write((Item)influenceMarker);
            }

            writer.Write((int)m_InfluenceAccountEntries.Count);
            foreach (InfluenceAccountEntry entry in m_InfluenceAccountEntries)
            {
                writer.Write(entry);
            }

            writer.Write((int)m_ActiveInfluenceAccountEntries.Count);
            foreach (InfluenceAccountEntry entry in m_ActiveInfluenceAccountEntries)
            {
                writer.Write(entry);
            }

            writer.Write(RealmController);
            writer.Write(BritainController);
            writer.Write(BuccaneersDenController);
            writer.Write(CoveController);
            writer.Write(JhelomController);
            writer.Write(MaginciaController);
            writer.Write(MinocController);
            writer.Write(MoonglowController);
            writer.Write(NujelmController);
            writer.Write(OccloController);
            writer.Write(SerpentsHoldController);
            writer.Write(SkaraBraeController);
            writer.Write(TrinsicController);
            writer.Write(VesperController);
            writer.Write(YewController);

            writer.Write(RealmControllerCurrentInfluence);
            writer.Write(BritainControllerCurrentInfluence);
            writer.Write(BuccaneersDenControllerCurrentInfluence);
            writer.Write(CoveControllerCurrentInfluence);
            writer.Write(JhelomControllerCurrentInfluence);
            writer.Write(MaginciaControllerCurrentInfluence);
            writer.Write(MinocControllerCurrentInfluence);
            writer.Write(MoonglowControllerCurrentInfluence);
            writer.Write(NujelmControllerCurrentInfluence);
            writer.Write(OccloControllerCurrentInfluence);
            writer.Write(SerpentsHoldControllerCurrentInfluence);
            writer.Write(SkaraBraeControllerCurrentInfluence);
            writer.Write(TrinsicControllerCurrentInfluence);
            writer.Write(VesperControllerCurrentInfluence);
            writer.Write(YewControllerCurrentInfluence);

            writer.Write(PreviousRealmController);
            writer.Write(PreviousBritainController);
            writer.Write(PreviousBuccaneersDenController);
            writer.Write(PreviousCoveController);
            writer.Write(PreviousJhelomController);
            writer.Write(PreviousMaginciaController);
            writer.Write(PreviousMinocController);
            writer.Write(PreviousMoonglowController);
            writer.Write(PreviousNujelmController);
            writer.Write(PreviousOccloController);
            writer.Write(PreviousSerpentsHoldController);
            writer.Write(PreviousSkaraBraeController);
            writer.Write(PreviousTrinsicController);
            writer.Write(PreviousVesperController);
            writer.Write(PreviousYewController);

            writer.Write(PreviousRealmControllerName);
            writer.Write(PreviousBritainControllerName);
            writer.Write(PreviousBuccaneersDenControllerName);
            writer.Write(PreviousCoveControllerName);
            writer.Write(PreviousJhelomControllerName);
            writer.Write(PreviousMaginciaControllerName);
            writer.Write(PreviousMinocControllerName);
            writer.Write(PreviousMoonglowControllerName);
            writer.Write(PreviousNujelmControllerName);
            writer.Write(PreviousOccloControllerName);
            writer.Write(PreviousSerpentsHoldControllerName);
            writer.Write(PreviousSkaraBraeControllerName);
            writer.Write(PreviousTrinsicControllerName);
            writer.Write(PreviousVesperControllerName);
            writer.Write(PreviousYewControllerName);

            writer.Write(PreviousRealmControllerInfluence);
            writer.Write(PreviousBritainControllerInfluence);
            writer.Write(PreviousBuccaneersDenControllerInfluence);
            writer.Write(PreviousCoveControllerInfluence);
            writer.Write(PreviousJhelomControllerInfluence);
            writer.Write(PreviousMaginciaControllerInfluence);
            writer.Write(PreviousMinocControllerInfluence);
            writer.Write(PreviousMoonglowControllerInfluence);
            writer.Write(PreviousNujelmControllerInfluence);
            writer.Write(PreviousOccloControllerInfluence);
            writer.Write(PreviousSerpentsHoldControllerInfluence);
            writer.Write(PreviousSkaraBraeControllerInfluence);
            writer.Write(PreviousTrinsicControllerInfluence);
            writer.Write(PreviousVesperControllerInfluence);
            writer.Write(PreviousYewControllerInfluence);

            writer.Write((int)m_PlayersGivenFreeRecruitmentDeeds.Count);
            foreach (Mobile mobile in m_PlayersGivenFreeRecruitmentDeeds)
            {
                writer.Write(mobile);
            }
        }

        public static void Deserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();
            
            m_InfluenceMarkers = new List<InfluenceMarker>();
            m_InfluenceAccountEntries = new List<InfluenceAccountEntry>();
            m_ActiveInfluenceAccountEntries = new List<InfluenceAccountEntry>();
            m_PlayersGivenFreeRecruitmentDeeds = new List<Mobile>();

            if (version >= 0)
            {
                Enabled = reader.ReadBool();
                                
                NextIntervalCompletion = reader.ReadDateTime();
                NextCycleCompletion = reader.ReadDateTime();
                PreviousCycleCompletion = reader.ReadDateTime();

                int influenceMarkers = reader.ReadInt();
                for (int a = 0; a < influenceMarkers; ++a)
                {
                    InfluenceMarker marker = reader.ReadItem() as InfluenceMarker;

                    if (marker != null)
                        m_InfluenceMarkers.Add(marker);
                }

                int influenceAccountEntries = reader.ReadInt();
                for (int a = 0; a < influenceAccountEntries; ++a)
                {
                    InfluenceAccountEntry entry = reader.ReadItem() as InfluenceAccountEntry;

                    if (entry != null)
                        m_InfluenceAccountEntries.Add(entry);                    
                }

                int activeInfluenceAccountEntries = reader.ReadInt();
                for (int a = 0; a < activeInfluenceAccountEntries; ++a)
                {
                    InfluenceAccountEntry entry = reader.ReadItem() as InfluenceAccountEntry;

                    if (entry != null)
                        m_ActiveInfluenceAccountEntries.Add(entry);
                }

                RealmController = reader.ReadItem() as InfluenceAccountEntry;
                BritainController = reader.ReadItem() as InfluenceAccountEntry;
                BuccaneersDenController = reader.ReadItem() as InfluenceAccountEntry;
                CoveController = reader.ReadItem() as InfluenceAccountEntry;
                JhelomController = reader.ReadItem() as InfluenceAccountEntry;
                MaginciaController = reader.ReadItem() as InfluenceAccountEntry;
                MinocController = reader.ReadItem() as InfluenceAccountEntry;
                MoonglowController = reader.ReadItem() as InfluenceAccountEntry;
                NujelmController = reader.ReadItem() as InfluenceAccountEntry;
                OccloController = reader.ReadItem() as InfluenceAccountEntry;
                SerpentsHoldController = reader.ReadItem() as InfluenceAccountEntry;
                SkaraBraeController = reader.ReadItem() as InfluenceAccountEntry;
                TrinsicController = reader.ReadItem() as InfluenceAccountEntry;
                VesperController = reader.ReadItem() as InfluenceAccountEntry;
                YewController = reader.ReadItem() as InfluenceAccountEntry;

                RealmControllerCurrentInfluence = reader.ReadDouble();
                BritainControllerCurrentInfluence = reader.ReadDouble();
                BuccaneersDenControllerCurrentInfluence = reader.ReadDouble();
                CoveControllerCurrentInfluence = reader.ReadDouble();
                JhelomControllerCurrentInfluence = reader.ReadDouble();
                MaginciaControllerCurrentInfluence = reader.ReadDouble();
                MinocControllerCurrentInfluence = reader.ReadDouble();
                MoonglowControllerCurrentInfluence = reader.ReadDouble();
                NujelmControllerCurrentInfluence = reader.ReadDouble();
                OccloControllerCurrentInfluence = reader.ReadDouble();
                SerpentsHoldControllerCurrentInfluence = reader.ReadDouble();
                SkaraBraeControllerCurrentInfluence = reader.ReadDouble();
                TrinsicControllerCurrentInfluence = reader.ReadDouble();
                VesperControllerCurrentInfluence = reader.ReadDouble();
                YewControllerCurrentInfluence = reader.ReadDouble();

                PreviousRealmController = reader.ReadItem() as InfluenceAccountEntry;
                PreviousBritainController = reader.ReadItem() as InfluenceAccountEntry;
                PreviousBuccaneersDenController = reader.ReadItem() as InfluenceAccountEntry;
                PreviousCoveController = reader.ReadItem() as InfluenceAccountEntry;
                PreviousJhelomController = reader.ReadItem() as InfluenceAccountEntry;
                PreviousMaginciaController = reader.ReadItem() as InfluenceAccountEntry;
                PreviousMinocController = reader.ReadItem() as InfluenceAccountEntry;
                PreviousMoonglowController = reader.ReadItem() as InfluenceAccountEntry;
                PreviousNujelmController = reader.ReadItem() as InfluenceAccountEntry;
                PreviousOccloController = reader.ReadItem() as InfluenceAccountEntry;
                PreviousSerpentsHoldController = reader.ReadItem() as InfluenceAccountEntry;
                PreviousSkaraBraeController = reader.ReadItem() as InfluenceAccountEntry;
                PreviousTrinsicController = reader.ReadItem() as InfluenceAccountEntry;
                PreviousVesperController = reader.ReadItem() as InfluenceAccountEntry;
                PreviousYewController = reader.ReadItem() as InfluenceAccountEntry;

                PreviousRealmControllerName = reader.ReadString();
                PreviousBritainControllerName = reader.ReadString();
                PreviousBuccaneersDenControllerName = reader.ReadString();
                PreviousCoveControllerName = reader.ReadString();
                PreviousJhelomControllerName = reader.ReadString();
                PreviousMaginciaControllerName = reader.ReadString();
                PreviousMinocControllerName = reader.ReadString();
                PreviousMoonglowControllerName = reader.ReadString();
                PreviousNujelmControllerName = reader.ReadString();
                PreviousOccloControllerName = reader.ReadString();
                PreviousSerpentsHoldControllerName = reader.ReadString();
                PreviousSkaraBraeControllerName = reader.ReadString();
                PreviousTrinsicControllerName = reader.ReadString();
                PreviousVesperControllerName = reader.ReadString();
                PreviousYewControllerName = reader.ReadString();

                PreviousRealmControllerInfluence = reader.ReadDouble();
                PreviousBritainControllerInfluence = reader.ReadDouble();
                PreviousBuccaneersDenControllerInfluence = reader.ReadDouble();
                PreviousCoveControllerInfluence = reader.ReadDouble();
                PreviousJhelomControllerInfluence = reader.ReadDouble();
                PreviousMaginciaControllerInfluence = reader.ReadDouble();
                PreviousMinocControllerInfluence = reader.ReadDouble();
                PreviousMoonglowControllerInfluence = reader.ReadDouble();
                PreviousNujelmControllerInfluence = reader.ReadDouble();
                PreviousOccloControllerInfluence = reader.ReadDouble();
                PreviousSerpentsHoldControllerInfluence = reader.ReadDouble();
                PreviousSkaraBraeControllerInfluence = reader.ReadDouble();
                PreviousTrinsicControllerInfluence = reader.ReadDouble();
                PreviousVesperControllerInfluence = reader.ReadDouble();
                PreviousYewControllerInfluence = reader.ReadDouble();

                int playersGivenFreeRecruitmentDeedsCount = reader.ReadInt();
                for (int a = 0; a < playersGivenFreeRecruitmentDeedsCount; ++a)
                {
                    Mobile mobile = reader.ReadMobile();

                    if (mobile != null)
                        m_PlayersGivenFreeRecruitmentDeeds.Add(mobile);
                }
            }
        }
    }

    public class InfluenceAccountEntry : Item
    {
        public string m_AccountUsername = "";
        public PlayerMobile m_CurrentCharacter;

        public int InfluenceLotteryTickets = 0;
        public int TicksRecordedThisInterval = 0;

        public int RealmInfluenceEarnedPerDay = 0;
        public int BritainInfluenceEarnedPerDay = 0;
        public int BuccaneersDenInfluenceEarnedPerDay = 0;
        public int CoveInfluenceEarnedPerDay = 0;
        public int JhelomInfluenceEarnedPerDay = 0;
        public int MaginciaInfluenceEarnedPerDay = 0;
        public int MinocInfluenceEarnedPerDay = 0;
        public int MoonglowInfluenceEarnedPerDay = 0;
        public int NujelmInfluenceEarnedPerDay = 0;
        public int OccloInfluenceEarnedPerDay = 0;
        public int SerpentsHoldInfluenceEarnedPerDay = 0;
        public int SkaraBraeInfluenceEarnedPerDay = 0;
        public int TrinsicInfluenceEarnedPerDay = 0;
        public int VesperInfluenceEarnedPerDay = 0;
        public int YewInfluenceEarnedPerDay = 0;

        public double RealmCurrentInfluence = 0;
        public double BritainCurrentInfluence = 0;
        public double BuccaneersDenCurrentInfluence = 0;
        public double CoveCurrentInfluence = 0;
        public double JhelomCurrentInfluence = 0;
        public double MaginciaCurrentInfluence = 0;
        public double MinocCurrentInfluence = 0;
        public double MoonglowCurrentInfluence = 0;
        public double NujelmCurrentInfluence = 0;
        public double OccloCurrentInfluence = 0;
        public double SerpentsHoldCurrentInfluence = 0;
        public double SkaraBraeCurrentInfluence = 0;
        public double TrinsicCurrentInfluence = 0;
        public double VesperCurrentInfluence = 0;
        public double YewCurrentInfluence = 0;

        public double RealmPreviousInfluence = 0;
        public double BritainPreviousInfluence = 0;
        public double BuccaneersDenPreviousInfluence = 0;
        public double CovePreviousInfluence = 0;
        public double JhelomPreviousInfluence = 0;
        public double MaginciaPreviousInfluence = 0;
        public double MinocPreviousInfluence = 0;
        public double MoonglowPreviousInfluence = 0;
        public double NujelmPreviousInfluence = 0;
        public double OccloPreviousInfluence = 0;
        public double SerpentsHoldPreviousInfluence = 0;
        public double SkaraBraePreviousInfluence = 0;
        public double TrinsicPreviousInfluence = 0;
        public double VesperPreviousInfluence = 0;
        public double YewPreviousInfluence = 0;

        public int BritainPrestigeAvailable = InfluencePersistance.StartingTownPrestige;
        public int BuccaneersDenPrestigeAvailable = InfluencePersistance.StartingTownPrestige;
        public int CovePrestigeAvailable = InfluencePersistance.StartingTownPrestige;
        public int JhelomPrestigeAvailable = InfluencePersistance.StartingTownPrestige;
        public int MaginciaPrestigeAvailable = InfluencePersistance.StartingTownPrestige;
        public int MinocPrestigeAvailable = InfluencePersistance.StartingTownPrestige;
        public int MoonglowPrestigeAvailable = InfluencePersistance.StartingTownPrestige;
        public int NujelmPrestigeAvailable = InfluencePersistance.StartingTownPrestige;
        public int OccloPrestigeAvailable = InfluencePersistance.StartingTownPrestige;
        public int SerpentsHoldPrestigeAvailable = InfluencePersistance.StartingTownPrestige;
        public int SkaraBraePrestigeAvailable = InfluencePersistance.StartingTownPrestige;
        public int TrinsicPrestigeAvailable = InfluencePersistance.StartingTownPrestige;
        public int VesperPrestigeAvailable = InfluencePersistance.StartingTownPrestige;
        public int YewPrestigeAvailable = InfluencePersistance.StartingTownPrestige;

        [Constructable]
        public InfluenceAccountEntry(string accountName, PlayerMobile currentCharacter): base(0x0)
        {
            Visible = false;
            Movable = false;

            m_AccountUsername = accountName;
            m_CurrentCharacter = currentCharacter;
        }

        public InfluenceAccountEntry(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version
            
            //Version 0
            writer.Write(m_AccountUsername);
            writer.Write(m_CurrentCharacter);

            writer.Write(InfluenceLotteryTickets);
            writer.Write(TicksRecordedThisInterval);

            writer.Write(RealmInfluenceEarnedPerDay);
            writer.Write(BritainInfluenceEarnedPerDay);
            writer.Write(BuccaneersDenInfluenceEarnedPerDay);
            writer.Write(CoveInfluenceEarnedPerDay);
            writer.Write(JhelomInfluenceEarnedPerDay);
            writer.Write(MaginciaInfluenceEarnedPerDay);
            writer.Write(MinocInfluenceEarnedPerDay);
            writer.Write(MoonglowInfluenceEarnedPerDay);
            writer.Write(NujelmInfluenceEarnedPerDay);
            writer.Write(OccloInfluenceEarnedPerDay);
            writer.Write(SerpentsHoldInfluenceEarnedPerDay);
            writer.Write(SkaraBraeInfluenceEarnedPerDay);
            writer.Write(TrinsicInfluenceEarnedPerDay);
            writer.Write(VesperInfluenceEarnedPerDay);
            writer.Write(YewInfluenceEarnedPerDay);

            writer.Write(RealmCurrentInfluence);
            writer.Write(BritainCurrentInfluence);
            writer.Write(BuccaneersDenCurrentInfluence);
            writer.Write(CoveCurrentInfluence);
            writer.Write(JhelomCurrentInfluence);
            writer.Write(MaginciaCurrentInfluence);
            writer.Write(MinocCurrentInfluence);
            writer.Write(MoonglowCurrentInfluence);
            writer.Write(NujelmCurrentInfluence);
            writer.Write(OccloCurrentInfluence);
            writer.Write(SerpentsHoldCurrentInfluence);
            writer.Write(SkaraBraeCurrentInfluence);
            writer.Write(TrinsicCurrentInfluence);
            writer.Write(VesperCurrentInfluence);
            writer.Write(YewCurrentInfluence);

            writer.Write(RealmPreviousInfluence);
            writer.Write(BritainPreviousInfluence);
            writer.Write(BuccaneersDenPreviousInfluence);
            writer.Write(CovePreviousInfluence);
            writer.Write(JhelomPreviousInfluence);
            writer.Write(MaginciaPreviousInfluence);
            writer.Write(MinocPreviousInfluence);
            writer.Write(MoonglowPreviousInfluence);
            writer.Write(NujelmPreviousInfluence);
            writer.Write(OccloPreviousInfluence);
            writer.Write(SerpentsHoldPreviousInfluence);
            writer.Write(SkaraBraePreviousInfluence);
            writer.Write(TrinsicPreviousInfluence);
            writer.Write(VesperPreviousInfluence);
            writer.Write(YewPreviousInfluence);

            writer.Write(BritainPrestigeAvailable);
            writer.Write(BuccaneersDenPrestigeAvailable);
            writer.Write(CovePrestigeAvailable);
            writer.Write(JhelomPrestigeAvailable);
            writer.Write(MaginciaPrestigeAvailable);
            writer.Write(MinocPrestigeAvailable);
            writer.Write(MoonglowPrestigeAvailable);
            writer.Write(NujelmPrestigeAvailable);
            writer.Write(OccloPrestigeAvailable);
            writer.Write(SerpentsHoldPrestigeAvailable);
            writer.Write(SkaraBraePrestigeAvailable);
            writer.Write(TrinsicPrestigeAvailable);
            writer.Write(VesperPrestigeAvailable);
            writer.Write(YewPrestigeAvailable);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            
            //Version 0
            if (version >= 0)
            {
                m_AccountUsername = reader.ReadString();
                m_CurrentCharacter = reader.ReadMobile() as PlayerMobile;

                InfluenceLotteryTickets = reader.ReadInt();
                TicksRecordedThisInterval = reader.ReadInt();

                RealmInfluenceEarnedPerDay = reader.ReadInt();
                BritainInfluenceEarnedPerDay = reader.ReadInt();
                BuccaneersDenInfluenceEarnedPerDay = reader.ReadInt();
                CoveInfluenceEarnedPerDay = reader.ReadInt();
                JhelomInfluenceEarnedPerDay = reader.ReadInt();
                MaginciaInfluenceEarnedPerDay = reader.ReadInt();
                MinocInfluenceEarnedPerDay = reader.ReadInt();
                MoonglowInfluenceEarnedPerDay = reader.ReadInt();
                NujelmInfluenceEarnedPerDay = reader.ReadInt();
                OccloInfluenceEarnedPerDay = reader.ReadInt();
                SerpentsHoldInfluenceEarnedPerDay = reader.ReadInt();
                SkaraBraeInfluenceEarnedPerDay = reader.ReadInt();
                TrinsicInfluenceEarnedPerDay = reader.ReadInt();
                VesperInfluenceEarnedPerDay = reader.ReadInt();
                YewInfluenceEarnedPerDay = reader.ReadInt();

                RealmCurrentInfluence = reader.ReadDouble();
                BritainCurrentInfluence = reader.ReadDouble();
                BuccaneersDenCurrentInfluence = reader.ReadDouble();
                CoveCurrentInfluence = reader.ReadDouble();
                JhelomCurrentInfluence = reader.ReadDouble();
                MaginciaCurrentInfluence = reader.ReadDouble();
                MinocCurrentInfluence = reader.ReadDouble();
                MoonglowCurrentInfluence = reader.ReadDouble();
                NujelmCurrentInfluence = reader.ReadDouble();
                OccloCurrentInfluence = reader.ReadDouble();
                SerpentsHoldCurrentInfluence = reader.ReadDouble();
                SkaraBraeCurrentInfluence = reader.ReadDouble();
                TrinsicCurrentInfluence = reader.ReadDouble();
                VesperCurrentInfluence = reader.ReadDouble();
                YewCurrentInfluence = reader.ReadDouble();

                RealmPreviousInfluence = reader.ReadDouble();
                BritainPreviousInfluence = reader.ReadDouble();
                BuccaneersDenPreviousInfluence = reader.ReadDouble();
                CovePreviousInfluence = reader.ReadDouble();
                JhelomPreviousInfluence = reader.ReadDouble();
                MaginciaPreviousInfluence = reader.ReadDouble();
                MinocPreviousInfluence = reader.ReadDouble();
                MoonglowPreviousInfluence = reader.ReadDouble();
                NujelmPreviousInfluence = reader.ReadDouble();
                OccloPreviousInfluence = reader.ReadDouble();
                SerpentsHoldPreviousInfluence = reader.ReadDouble();
                SkaraBraePreviousInfluence = reader.ReadDouble();
                TrinsicPreviousInfluence = reader.ReadDouble();
                VesperPreviousInfluence = reader.ReadDouble();
                YewPreviousInfluence = reader.ReadDouble();

                BritainPrestigeAvailable = reader.ReadInt();
                BuccaneersDenPrestigeAvailable = reader.ReadInt();
                CovePrestigeAvailable = reader.ReadInt();
                JhelomPrestigeAvailable = reader.ReadInt();
                MaginciaPrestigeAvailable = reader.ReadInt();
                MinocPrestigeAvailable = reader.ReadInt();
                MoonglowPrestigeAvailable = reader.ReadInt();
                NujelmPrestigeAvailable = reader.ReadInt();
                OccloPrestigeAvailable = reader.ReadInt();
                SerpentsHoldPrestigeAvailable = reader.ReadInt();
                SkaraBraePrestigeAvailable = reader.ReadInt();
                TrinsicPrestigeAvailable = reader.ReadInt();
                VesperPrestigeAvailable = reader.ReadInt();
                YewPrestigeAvailable = reader.ReadInt();
            }
        }
    }
    
    public class InfluencePersistanceItem : Item
    {
        public override string DefaultName { get { return "InfluencePersistance"; } }

        public InfluencePersistanceItem(): base(0x0)
        {
            Movable = false;
        }

        public InfluencePersistanceItem(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version

            InfluencePersistance.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Movable = false;

            InfluencePersistance.PersistanceItem = this;
            InfluencePersistance.Deserialize(reader);
        }
    }    
}
