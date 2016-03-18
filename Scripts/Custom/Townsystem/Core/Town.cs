/***************************************************************************
 *                                 Town.cs
 *                            ------------------
 *   begin                : August 2010
 *   author               : Sean Stavropoulos
 *   email                : sean.stavro@gmail.com
 *
 *
 ***************************************************************************/

using System;
using System.Collections;
using Server;
using Server.Targeting;
using Server.Mobiles;
using Server.Commands;
using System.Collections.Generic;
using Server.Prompts;
using Server.Items;
using Server.Regions;
using Server.Custom.Townsystem.Core;
using Server.Achievements;
using System.Text;

namespace Server.Custom.Townsystem
{
    public enum GuardStates
    {
        None=0,
        Lax=1,
        Standard=2,
        Strong=3,
        Exceptional=4
    };

    public enum TreasuryWallTypes 
    { 
        None, 
        Iron, 
        Magical 
    };

    [Parsable]
    [CustomEnum(new string[] { "Britain", "Cove", "Jhelom", "Magincia", "Minoc", "Moonglow", "Nujel'm", "Ocllo", "Serpent's Hold", "Skara Brae", "Trinsic", "Vesper", "Yew" })]
	public abstract class Town : IComparable
    {
        public static void Initialize()
        {
            EventSink.Login += new LoginEventHandler(EventSink_Login);

            CommandSystem.Register("ResetTowns", AccessLevel.Administrator, new CommandEventHandler(ResetTowns_OnCommand));
            CommandSystem.Register("FixBraziers", AccessLevel.Administrator, new CommandEventHandler(FixBraziers_OnCommand));
            CommandSystem.Register("InitializeElections", AccessLevel.Administrator, new CommandEventHandler(InitElections_OnCommand));
            CommandSystem.Register("ResetTreasuries", AccessLevel.Administrator, new CommandEventHandler(ResetTreasuries_OnCommand));
            CommandSystem.Register("DouseBraziers", AccessLevel.Administrator, new CommandEventHandler(DouseBraziers_OnCommand));
            CommandSystem.Register("InitCaptureBraziers", AccessLevel.Administrator, new CommandEventHandler(InitCaptureBraziers_OnCommand));
        }

        public static readonly TimeSpan VendorContractLength = TimeSpan.FromDays(14);
        public static readonly TimeSpan TaxChangePeriod = TimeSpan.FromDays(3.0);
        private static readonly double MaximumSalesTax = 3.0;
        private static readonly double MaximumPropertyTax = 0.1;
        public static readonly TimeSpan LeavePeriod = TimeSpan.FromDays(5); //CHANGE THIS
        public static readonly TimeSpan WithdrawFrequency = TimeSpan.FromDays(7);
        public static readonly double WithdrawPercent = 0.1;
        public static readonly TimeSpan ExileFrequency = TimeSpan.FromHours(12);
        public static readonly TimeSpan UpkeepPeriod = TimeSpan.FromDays(1.0);
        public static readonly TimeSpan BuffUpkeepPeriod = TimeSpan.FromDays(7);
        public static readonly TimeSpan TimeBetweenSecondaryBuffUpdates = TimeSpan.FromDays(1);
        public static readonly int BuffUpkeepCost = 50000;
        public int ActiveCitizens = 0;
        public int ActiveMilitia = 0;

        public static readonly Type[] m_SpawnTypes = new Type[]
        {
            typeof( Orc ), 
            typeof( OrcishLord ), 
            typeof( OrcishMage), 
            typeof( Skeleton ),
            typeof( SkeletalMage ), 
            typeof( SkeletalKnight ), 
            typeof( Troll )
        };

        public TownUpkeepTimer UpkeepTimer { get; set; }

        private KingOfTheHillTimer m_KingOfTheHillTimer;
        public KingOfTheHillTimer KingOfTheHillTimer { get { return m_KingOfTheHillTimer; } set { m_KingOfTheHillTimer = value; } }

        private static GuardedRegionTimer m_GuardTimer;
        private TownDefinition m_Definition;
		private TownState m_State;
        private Region m_Region;
        private int m_CapturePoints;
        private List<GuardList> m_GuardLists;
        private DateTime lastFacBroadcast;

        public static List<Town> Towns 
        {
            get { return Reflector.Towns; } 
        }

        public bool CanUpdateSecondaryBuffs { get { return m_State.LastBuffUpdate + TimeBetweenSecondaryBuffUpdates < DateTime.UtcNow; } }

        public bool TaxChangeReady
        {
			get { return (m_State.LastTaxChange + TaxChangePeriod) < DateTime.Now; }
        }
        public static GuardedRegionTimer GuardTimer
        {
            get { return m_GuardTimer; }
            set { m_GuardTimer = value; }
        }
        public Region Region
        {
            get { 
                if (m_Region != null)
                    return m_Region;

                m_Region = Server.Region.Find(Definition.ElectionStoneLocation, Map.Felucca);
                return m_Region;
            }
        }
		public TownDefinition Definition
		{
			get{ return m_Definition; }
			set{ m_Definition = value; }
		}
		public TownState State
		{
			get{ return m_State; }
			set{ m_State = value; ConstructGuardLists(); }
		}
        public List<TownBrazier> Braziers
        {
            get { return m_State.Braziers; }
            set { m_State.Braziers = value; }
        }

        public CaptureBrazier CaptureBrazier
        {
            get { return m_State.CaptureBrazier; }
            set
            {
                if (m_State.CaptureBrazier != null)
                    m_State.CaptureBrazier.Delete();

                m_State.CaptureBrazier = value;
            }
        }
        public List<TownCrystal> Crystals
        {
            get { return m_State.Crystals; }
            set { m_State.Crystals = value; }
        }
        public List<TownFlag> Flags
        {
            get { return m_State.Flags; }
            set { m_State.Flags = value; }
        }
        public List<OutcastEntry> OutcastEntries
        {
            get { return m_State.OutcastEntries; }
            set { m_State.OutcastEntries = value; }
        }
        public List<BaseCreature> SpawnedCreatures
        {
            get { return m_State.SpawnedCreatures; }
            set { m_State.SpawnedCreatures = value; }
        }
        public TownOutcastBoard OutcastBoard
		{
			get{ return m_State.OutcastBoard; }
			set{ m_State.OutcastBoard = value; }
		}
        public GuardStates GuardState
        {
            get { return m_State.GuardState; }
            set { m_State.GuardState = value; }
        }
        public Election Election
        {
            get { return m_State.Election; }
            set { m_State.Election = value; }
        }
        public KingStone KingStone
        {
            get { return m_State.KingStone; }
            set { m_State.KingStone = value; }
        }
        public MilitiaStone MilitiaStone
        {
            get { return m_State.MilitiaStone; }
            set { m_State.MilitiaStone = value; }
        }
        public DateTime LastSpawnCreatures
        {
            get { return m_State.LastSpawnCreatures; }
            set { m_State.LastSpawnCreatures = value; }
        }
		public Town ControllingTown
		{
			get { return m_State.ControllingTown; }
            set { m_State.ControllingTown = value; }
		}

        public Faction HomeFaction // This is the "original town"
        {
			get { return m_State.HomeFaction; }
            set 
            {
				if (value == m_State.HomeFaction || value == null)
                    return;

                foreach (Mobile m in MilitiaMembers)
                {
                    PlayerState ps = PlayerState.Find(m);
                    if (ps != null)
                    {
                        if (ps.RankIndex != -1)
                        {
							while ((ps.RankIndex + 1) < m_State.HomeFaction.ZeroRankOffset)
                            {
								PlayerState pNext = m_State.HomeFaction.Members[ps.RankIndex + 1];
								m_State.HomeFaction.Members[ps.RankIndex + 1] = ps;
								m_State.HomeFaction.Members[ps.RankIndex] = pNext;
                                ps.RankIndex++;
                                pNext.RankIndex--;
                            }
							m_State.HomeFaction.ZeroRankOffset--;
                        }

                        ps.Owner = value.Members;
						m_State.HomeFaction.Members.Remove(ps);
                        value.Members.Add(ps);
                        ps.Faction = value;
                    }
                }

                value.Members.Sort();
                value.ZeroRankOffset = value.Members.Count;

                for (int i = value.Members.Count - 1; i >= 0; i--)
                {
                    PlayerState player = value.Members[i];

                    if (player.KillPoints <= 0)
                        value.ZeroRankOffset = i;
                    else
                        player.RankIndex = i;
                }
				m_State.HomeFaction = value;

                foreach (TownFlag tf in Flags)
                    tf.Invalidate();

                List<GuardList> guardLists = GuardLists;

				for (int i = 0; i < guardLists.Count; ++i)
				{
					GuardList guardList = guardLists[i];
					List<BaseFactionGuard> guards = guardList.Guards;

					for (int j = guards.Count - 1; j >= 0; --j)
						guards[j].Delete();
				}

				ConstructGuardLists();
            }
        }
		public Mobile King
		{
			get{ return m_State.King; }
            set { SetNewKing(m_State.King, value); m_State.King = value; }
		}
        public Mobile Commander
        {
            get { return m_State.Commander; }
            set { SetNewCommander(m_State.Commander, value); m_State.Commander = value; }
        }
        public List<CitizenshipState> Members
        {
            get { return m_State.Members; }
            set { m_State.Members = value; }
        }
        public List<Mobile> MilitiaMembers
        {
            get { return m_State.MilitiaMembers; }
            set { m_State.MilitiaMembers = value; }
        }
        public List<Mobile> Exiles
        {
            get { return m_State.Exiles; }
            set { m_State.Exiles = value; }
        }
		public double SalesTax
		{
			get{ return m_State.SalesTax; }
			set{ m_State.SalesTax = value; }
		}
        public double PropertyTax
        {
            get { return m_State.PropertyTax; }
            set { m_State.PropertyTax = value; }
        }
        public double Treasury
        {
            get { return m_State.Treasury; }
            set 
            { 
                if (value < 0) value = 0;
                m_State.Treasury = value; 
            }
        }
        public TreasuryWallTypes TreasuryWallType
        {
            get { return m_State.TreasuryWallType; }
            set { m_State.TreasuryWallType = value; }
        }
        public List<TreasuryWall> TreasuryWalls
        {
            get { return m_State.TreasuryWalls; }
            set { m_State.TreasuryWalls = value; }
        }
		public DateTime LastTaxChange
		{
			get{ return m_State.LastTaxChange; }
			set{ m_State.LastTaxChange = value; }
		}
        public DateTime ControlStartTime
        {
            get { return m_State.ControlStartTime; }
            set { m_State.ControlStartTime = value; }
        }
        public List<TownCrier> TownCriers
        {
            get { return m_State.TownCriers; }
            set { m_State.TownCriers = value; }
        }
        public int TotalVoted
        {
            get { return m_State.TotalVoted; }
            set { m_State.TotalVoted = value; }
        }
        public Revolt.RevoltStone TownRevoltStone
        {
            get { return m_State.TownRevoltStone; }
            set { m_State.TownRevoltStone = value; }
        }
        public List<GuardList> GuardLists
        {
            get { return m_GuardLists; }
            set { m_GuardLists = value; }
        }
        public CitizenshipBuffs PrimaryCitizenshipBuff
        {
            get { return m_State.PrimaryCitizenshipBuff; }
            set 
            { 
                if (SecondaryCitizenshipBuffs.Contains(value))
                    SecondaryCitizenshipBuffs.Remove(value);

                m_State.PrimaryCitizenshipBuff = value; 
            }
        }

        public List<CitizenshipBuffs> SecondaryCitizenshipBuffs
        {
            get { return m_State.SecondaryCitizenshipBuffs; }
        }

        public bool AllowNewCitizenshipBuffs
        {
            get { return m_State.AllowNewCitizenshipBuffs; }
            set { m_State.AllowNewCitizenshipBuffs = value; }
        }
        public List<MarketFloor> MarketTiles
        {
            get { return m_State.MarketTiles; }
            set { m_State.MarketTiles = value; }
        }
        public DateTime LastUpkeep
        {
            get { return m_State.LastUpkeep; }
            set { m_State.LastUpkeep = value; }
        }
        public TreasuryLog WithdrawLog
        {
            get { return m_State.WithdrawLog; }
            set { m_State.WithdrawLog = value; }
        }
        public DateTime LastTreasuryWithdraw
        {
            get { return m_State.LastTreasuryWithdraw; }
            set { m_State.LastTreasuryWithdraw = value; }
        }
        public DateTime LastExile
        {
            get { return m_State.LastExile; }
            set { m_State.LastExile = value; }
        }
        public Withdraws WithdrawManager
        {
            get { return m_State.Withdraws; }
            set { m_State.Withdraws = value; }
        }
        public DateTime LastGuardStateChange
        {
            get { return m_State.LastGuardStateChange; }
            set { m_State.LastGuardStateChange = value; }
        }

        public List<AllianceFlag> AllianceFlags
        {
            get { return m_State.AllianceFlags; }
            set { m_State.AllianceFlags = value; }
        }
        public DateTime LastBombPlanPurchase
        {
            get { return m_State.LastBombPlanPurchase; }
            set { m_State.LastBombPlanPurchase = value; }
        }
        public bool ExilesDisabled {
            get { return m_State.ExilesDisabled; }
            set { m_State.ExilesDisabled = value; }
        }

        public DateTime LastGuardHire { get; set; }

        public DateTime LastBuffUpkeep
        {
            get { return m_State.LastBuffUpkeep; }
            set { m_State.LastBuffUpkeep = value; }
        }

        public DateTime LastBuffUpdate
        {
            get { return m_State.LastBuffUpdate; }
            set { m_State.LastBuffUpdate = value; }
        }


        public int GuardUpkeep
        {
            get
            {
                List<GuardList> guardLists = GuardLists;
                int upkeep = 0;

                for (int i = 0; i < guardLists.Count; ++i)
                    upkeep += guardLists[i].Guards.Count * guardLists[i].Definition.Upkeep;

                return upkeep;
            }
        }

        public int MaximumNumberOfGuards
        {
            get
            {
                switch (GuardState)
                {
                    case GuardStates.None: return 0;
                    case GuardStates.Lax: return 1;
                    case GuardStates.Standard: return 2;
                    case GuardStates.Strong: return 3;
                    case GuardStates.Exceptional: return 4;
                    default: return 0;
                }
            }
        }

        public int NumberOfGuards
        {
            get
            {
                List<GuardList> guardLists = GuardLists;
                int guards = 0;

                for (int i = 0; i < guardLists.Count; ++i)
                    guards += guardLists[i].Guards.Count;

                return guards;
            }
        }

        public bool AllowGuardType(Type type)
        {
            if (type == typeof(FactionWizard)) 
                return GuardState >= GuardStates.Strong;

            return true;
        }

        public int AvailableBombPlans
        {
            get { return m_State.AvailableBombPlans; }
            set { m_State.AvailableBombPlans = value; }
        }

        private Timer m_IncomeTimer;

        public void StartIncomeTimer()
        {
            UpdateActiveCitizens();
            UpdateActiveMilitia();

            if (m_IncomeTimer != null)
                m_IncomeTimer.Stop();

            m_IncomeTimer = Timer.DelayCall(TimeSpan.FromMinutes(1.0), TimeSpan.FromMinutes(1.0), new TimerCallback(CheckIncome));
        }

        public void StopIncomeTimer()
        {
            if (m_IncomeTimer != null)
                m_IncomeTimer.Stop();

            m_IncomeTimer = null;
        }

        public void CheckIncome()
        {
			if ((LastUpkeep + UpkeepPeriod) > DateTime.UtcNow)
                return;

            ProcessIncome();
        }

        public bool HasActiveBuff(CitizenshipBuffs buff)
        {
            return PrimaryCitizenshipBuff == buff || SecondaryCitizenshipBuffs.Contains(buff);
        }

        public void UpdateActiveCitizens()
        {
            int count = 0;
            foreach (CitizenshipState state in Members)
                if (CitizenshipState.MeetsDispursementRequirements(state.Mobile))
                    count++;

            ActiveCitizens = count;
        }

        public void UpdateActiveMilitia()
        {
            int count = 0;
            foreach (Mobile mob in MilitiaMembers)
                if (CitizenshipState.MeetsDispursementRequirements(mob))
                    count++;

            ActiveMilitia = count;
        }

        public int GetGuardCost(GuardStates state)
        {
            switch (state)
            {
                case GuardStates.None: return 0; break;
                case GuardStates.Lax: return 3; break;
                case GuardStates.Standard: return 7; break;
                case GuardStates.Strong: return 10; break;
                case GuardStates.Exceptional: return 17; break;
                default: return 0; break;
            }
        }

        public void AdventurerTreasuryDeposit(int goldWorth)
        {
            Treasury += goldWorth;
        }

        public void ProcessIncome()
        {
			LastUpkeep = DateTime.UtcNow;
            UpdateActiveCitizens();
            UpdateActiveMilitia();

            int passiveGuardCost = GetGuardCost(GuardState);

            if (passiveGuardCost > 0)
            {
                int cost = ActiveCitizens * passiveGuardCost;
                if (cost > Treasury)
                {
                    int state = (int)GuardState - 1;
                    for (int i = state; i >= 0; i--)
                    {
                        var newState = (GuardStates)i;
                        int newCost = GetGuardCost(newState);
                        
                        newCost *= ActiveCitizens;

                        if (newCost > Treasury)
                            continue;

                        Treasury -= newCost;

                        GuardState = newState;

                        AddTownCrierEntry(new string[] { "The town cannot afford the current level of guards anymore!", "Guards have been laid off!" }, TimeSpan.FromHours(6));
                    }
                }
                else
                {
                    Treasury -= cost;
                }
            }

            int flow = -1*GuardUpkeep;

            if ((Treasury + flow) < 0)
            {
                ArrayList toDelete = BuildFinanceList();

                while ((Treasury + flow) < 0 && toDelete.Count > 0)
                {
                    int index = Utility.Random(toDelete.Count);
                    Mobile mob = (Mobile)toDelete[index];

                    mob.Delete();

                    toDelete.RemoveAt(index);
                    flow = -1*GuardUpkeep;
                }
            }

            if (flow != 0)
            {
                Treasury += flow;
                WithdrawLog.AddLogEntry(King, -1 * flow, TreasuryLogType.GuardUpkeep);
            }
        }

        public ArrayList BuildFinanceList()
        {
            ArrayList list = new ArrayList();

            List<GuardList> guardLists = GuardLists;

            for (int i = 0; i < guardLists.Count; ++i)
                list.AddRange(guardLists[i].Guards);

            return list;
        }

        public void BeginCommanderAppointing(Mobile from)
        {
            if (!IsKing(from))
                return;

            from.SendMessage("Target the player you wish to appoint as Commander.");
            from.BeginTarget(12, false, TargetFlags.None, new TargetCallback(EndCommanderAppointing));
        }

        public void EndCommanderAppointing(Mobile from, object obj)
        {
            if (!IsKing(from))
                return;

            if (obj is PlayerMobile)
            {
                PlayerMobile player = (PlayerMobile)obj;
                Commander = player;
                from.SendMessage(String.Format("You have appointed {0} to the position of Commander.", player.RawName));
            }
            else
            {
                from.SendMessage("That is not a player!");
            }
        }

        public void BeginOrderFiring(Mobile from)
        {
            if (!IsKing(from))
                return;

            from.SendMessage("Target the guard you wish to dismiss.");
            from.BeginTarget(12, false, TargetFlags.None, new TargetCallback(EndOrderFiring));
        }

        public void EndOrderFiring(Mobile from, object obj)
        {
            if (!IsKing(from))
                return;

            if (obj is BaseFactionGuard)
            {
                BaseFactionGuard guard = (BaseFactionGuard)obj;

                if (guard.Town == this)
                    guard.Delete();
            }
            else
            {
                from.SendMessage("That is not a guard!");
            }
        }

        public void TreasuryWithdrawRequest(Mobile from, int amount, Withdraws.WithdrawType type)
        {
            if (!IsKing(from))
            {
                from.SendMessage("You are no longer the king of {0}", Definition.FriendlyName);
                return;
            }
            else if (!WithdrawManager.CanWithdraw(amount))
            {
                from.SendMessage("You cannot withdraw that much gold at this time.");
            }
            else if (Treasury < amount)
            {
                from.SendMessage(0x0, "The {0} treasury only contains {1} gold.", Definition.FriendlyName, (int)Treasury);
            }
            else if (Treasury * WithdrawPercent < amount)
            {
                from.SendMessage(0x0, "The maximum withdraw at this time is limited to {0} gold.", (int)(Treasury * WithdrawPercent));
            }
            else if (WithdrawManager.IsPendingTransaction)
            {
                from.SendMessage(0x0, "You may only have one pending transaction at at time.");
            }
            else
            {
                if (type == Withdraws.WithdrawType.Personal)
                {
                    from.SendMessage(String.Format("You have chosen to withdraw {0} gold coin from the treasury. You will receive the gold coin in 36 hours.", (int)amount));
                    //AddTownCrierEntry(new string[] { String.Format("The King sees fit to tax the treasury of this fair city to the amount of {0}!", amount), "Bankers are currently processing this royal order, and the gold shall be awarded to the King.", String.Format("We're quite certain {0} intentions are entirely benevolent! Long live the King!",King != null && King.Female ? "her" : "his") }, TimeSpan.FromHours(24));
                    WithdrawLog.AddLogEntry(from, amount, TreasuryLogType.PersonalRequestDispursment);
                }
                else
                {
                    from.SendMessage(String.Format("You have chosen to dispurse {0} gold coin from the treasury. The gold will be dispursed in three hours.", amount));
                }

                WithdrawManager.Withdraw(from, amount, type);
                
            }
        }

        /*
        public List<VendorList> VendorLists
        {
            get{ return m_VendorLists; }
            set{ m_VendorLists = value; }
        }
        */

        /*
		public int FinanceUpkeep
		{
			get
			{
				List<VendorList> vendorLists = VendorLists;
				int upkeep = 0;

				for ( int i = 0; i < vendorLists.Count; ++i )
					upkeep += vendorLists[i].Vendors.Count * vendorLists[i].Definition.Upkeep;

				return upkeep;
			}
		}

		public int DailyIncome
		{
			get{ return (10000 * (100 + m_State.Tax)) / 100; }
		}

		public int NetCashFlow
		{
			get{ return DailyIncome - FinanceUpkeep - KingUpkeep; }
		}

		private List<VendorList> m_VendorLists;
        */

        public bool IsKing(Mobile mob)
        {
            if (mob == null || mob.Deleted)
                return false;

            return (mob.AccessLevel >= AccessLevel.GameMaster || mob == King);
        }

        public bool IsCommander(Mobile mob)
        {
            if (mob == null || mob.Deleted)
                return false;

            return (mob.AccessLevel >= AccessLevel.GameMaster || mob == Commander);
        }

        public bool TryCapture(TownBrazier b, Mobile m)
        {
            var pm = m as PlayerMobile;
            Town capTown = pm.Citizenship;
            bool militia = pm.IsInMilitia;

            if (!OCTimeSlots.IsActiveTown(this))
            {
				m.SendMessage("This town is not currently active for town militia wars.");
            }
            else if (!militia || capTown == null)
            {
                m.SendMessage("You must join a militia to control a town.");
            }
			else if (ControllingTown == null)
            {
                m.SendMessage("This town currently has no controlling town.  Please report this to a GM.");
            }
            else if (b.Captured)
            {
                m.SendMessage("This brazier is already controlled.");
            }
            else
            {
                b.CapTown = capTown;
                b.Captured = true;
                Effects.PlaySound(b.Location, b.Map, 0x225);
                BroadcastFactions(String.Format("The {0} brazier has been captured by {1}!", b.BrazierLocationName.ToLower(), capTown.Definition.FriendlyName));

                if (lastFacBroadcast < DateTime.Now - TimeSpan.FromMinutes(5))
                {
                    lastFacBroadcast = DateTime.Now;
                    Faction.BroadcastAll(String.Format("{0} is under siege by {1}!", Definition.FriendlyName, capTown.Definition.FriendlyName));
                }

                if (m_KingOfTheHillTimer == null || !m_KingOfTheHillTimer.Running)
                {
                    m_KingOfTheHillTimer = new KingOfTheHillTimer(m_State.Town);
                    m_KingOfTheHillTimer.Start();
                }

                // IPY ACHIEVEMENT
                AchievementSystem.Instance.TickProgress(m, AchievementTriggers.Trigger_LightBrazier);
                DailyAchievement.TickProgress(Category.PvP, (PlayerMobile)m, PvPCategory.LightBraziers);
                // IPY ACHIEVEMENT

                OCLeaderboard.RegisterBrazier(m);

                return true;
            }

            return false;
        }

        public void AddMilitiaMember(Mobile m)
        {
            if (!MilitiaMembers.Contains(m))
                MilitiaMembers.Add(m);
        }

        public void RemoveMilitiaMember(Mobile m)
        {
            if (MilitiaMembers.Contains(m))
                MilitiaMembers.Remove(m);
        }

        public void SalesTaxChangeRequest(Mobile from, double newVal)
        {
            if (TaxChangeReady)
            {
                newVal = Math.Max(0.0, newVal);

                if (newVal == SalesTax)
                    return;

                if (newVal > MaximumSalesTax)
                {
                    newVal = MaximumSalesTax;
                    from.SendMessage("Sales tax has reached it's maximum value.");
                }

                AddTownCrierEntry(new string[] { String.Format("Sales tax has {0} to {1:0.0}%!", newVal > SalesTax ? "increased" : "decreased", (newVal * 100.0).ToString())  }, TimeSpan.FromHours(3.0));

				LastTaxChange = DateTime.Now;
                SalesTax = newVal;
                
            }
            else
                from.SendMessage("You cannot change the sales tax at this time.");

        }

        public void PropertyTaxChangeRequest(Mobile from, double newVal)
        {
            if (TaxChangeReady)
            {
                if (newVal > MaximumPropertyTax)
                {
                    newVal = MaximumPropertyTax;
                    from.SendMessage("Property tax has reached it's maximum value.");
                }

				LastTaxChange = DateTime.Now;
                PropertyTax = newVal;
            }
            else
                from.SendMessage("You cannot change the property tax at this time.");
        }

        public void SetNewKing(Mobile oldKing, Mobile newKing)
        {
            Commander = null;

            if (oldKing != null)
            {
                oldKing.SendMessage("Your term of service as King has ended.");
                ((PlayerMobile)oldKing).RemovePrefixTitle = "King";
            }

            if (newKing == null)
                return;

            ((PlayerMobile)newKing).AddPrefixTitle = "King";
            newKing.SendMessage(String.Format("You are now King of {0}!", m_State.Town));
            AllowNewCitizenshipBuffs = true;

            // allow new kings to withdraw immediately
            if (WithdrawManager != null && WithdrawManager.Entries != null)
            {
                WithdrawManager.Entries.Clear();
                WithdrawManager.StopTimer();
            }

            ReplaceKingCrown(newKing, this);
        }

        public void SetNewCommander(Mobile oldCommander, Mobile newCommander)
        {
            if (oldCommander != null)
            {
                oldCommander.SendMessage("Your term of service as Commander has ended.");
                ((PlayerMobile)oldCommander).RemovePrefixTitle = "Commander";
            }

            if (newCommander == null)
                return;

            ((PlayerMobile)newCommander).AddPrefixTitle = "Commander";
            newCommander.SendMessage(String.Format("You are now Commander of {0}!", m_State.Town));
        }

        public static void DouseAllBraziers()
        {
            foreach (Town town in Towns)
                town.DouseBraziers();
        }

        public static void ReplaceKingCrown(Mobile king, Town town)
        {
            if (town == null || king == null)
                return;

            foreach (Crown crown in Crown.Instances)
                if (crown.Town == town)
                {
                    king.Backpack.DropItem(crown);
                    king.SendMessage("You have been given a King's Crown!");
                    return;
                }

            var c = new Crown(town);
            king.Backpack.DropItem(c);
                    king.SendMessage("You have been given a King's Crown!");
        }


        public void ClearTreasuryWalls()
        {
            while (TreasuryWalls.Count > 0)
            {
                var wall = TreasuryWalls[0];
                TreasuryWalls.RemoveAt(0);
                wall.Delete();
            }
        }

        public void SetTreasuryWalls(TreasuryWallTypes type)
        {
            if (type == TreasuryWallType)
                return;

            switch (type)
            {
                case (TreasuryWallTypes.None):
                    {
                        ClearTreasuryWalls();
                        TreasuryWallType = TreasuryWallTypes.None;

                    }break;
                case (TreasuryWallTypes.Iron):
                    {
                        ClearTreasuryWalls();

                        for (int i = 0; i < Definition.TreasuryWallEastLocations.Length; i++)
                        {
                            TreasuryWall w = new TreasuryWall(this, type, Direction.East);
                            w.MoveToWorld(Definition.TreasuryWallEastLocations[i], Map.Felucca);
                            TreasuryWalls.Add(w);
                        }

                        for (int i = 0; i < Definition.TreasuryWallNorthLocations.Length; i++)
                        {
                            
                            TreasuryWall w = new TreasuryWall(this, type, Direction.North);
                            w.MoveToWorld(Definition.TreasuryWallNorthLocations[i], Map.Felucca);
                            TreasuryWalls.Add(w);
                        }

                        TreasuryWallType = TreasuryWallTypes.Iron;
                    } break;
                case (TreasuryWallTypes.Magical):
                    {
                        ClearTreasuryWalls();

                        for (int i = 0; i < Definition.TreasuryWallEastLocations.Length; i++)
                        {
                            TreasuryWall w = new TreasuryWall(this, type, Direction.East);
                            w.MoveToWorld(Definition.TreasuryWallEastLocations[i], Map.Felucca);
                            TreasuryWalls.Add(w);
                        }

                        for (int i = 0; i < Definition.TreasuryWallNorthLocations.Length; i++)
                        {
                            TreasuryWall w = new TreasuryWall(this, type, Direction.North);
                            w.MoveToWorld(Definition.TreasuryWallNorthLocations[i], Map.Felucca);
                            TreasuryWalls.Add(w);
                        }

                        TreasuryWallType = TreasuryWallTypes.Magical;
                    } break;
            }
        }

        public static void GlobalTownCrierBroadcast(string[] entry, TimeSpan time)
        {
            foreach (Town t in Towns)
                t.AddTownCrierEntry(entry, time);
        }

        public void AddTownCrierEntry(string[] entry, TimeSpan time)
        {
            foreach (TownCrier c in TownCriers)
                c.AddEntry(entry, time);
        }

        private static Item CheckExistance(Point3D loc, Map facet, Type type)
        {
            foreach (Item item in facet.GetItemsInRange(loc, 0))
            {
                if (type.IsAssignableFrom(item.GetType()))
                    return item;
            }

            return null;
        }

        public bool CanExile
        {
			get { return (LastExile + ExileFrequency < DateTime.Now); }
        }

        public void Exile(Mobile from, Mobile toExile)
        {
            if (toExile.AccessLevel > AccessLevel.Player)
            {
                from.SendMessage(String.Format("You will need a bigger boot to remove them from {0}.", Definition.FriendlyName));
                return;
            }
            else if (Exiles.Contains(toExile))
            {
                from.SendMessage(String.Format("'{0}' has already been exiled from {1}.", toExile.Name, Definition.FriendlyName));
                return;
            }
            else if (!CanExile && from.AccessLevel == AccessLevel.Player)
            {
                from.SendMessage("You cannot exile again so soon.");
                return;
            }

			for (int i = 0; i < OutcastEntries.Count; ++i )
			{
				OutcastEntry oe = OutcastEntries[i];
				if (oe.Outcast == toExile)
				{
					if (oe.Count >= 5)
					{
						from.SendMessage(String.Format("{0} is now exiled from {1}.", toExile.Name, Definition.FriendlyName));
						Exiles.Add(toExile);

						if (from.AccessLevel == AccessLevel.Player)
							LastExile = DateTime.Now;
					}
					else
					{
						from.SendMessage(String.Format("{0} must have more votes on the outcast board to be exiled from {1}.", toExile.Name, Definition.FriendlyName));
					}

					return;
				}
			}

            from.SendMessage(String.Format("{0} must have more votes on the outcast board to be exiled from {1}.", toExile.Name, Definition.FriendlyName));
        }

		public static Town FromRegion( Region reg )
		{
			if ( reg.Map != Faction.Facet )
				return null;

			List<Town> towns = Towns;

			for ( int i = 0; i < towns.Count; ++i )
			{
				Town town = towns[i];
                
				if ( reg.IsPartOf( town.Definition.Region ) )
					return town;
			}

			return null;
		}

        public static Town FromLocation(Point3D loc, Map map)
        {
            Region reg = Region.Find(loc, map);
            return FromRegion(reg);
        }

        public static bool HasHouseInTownRegion(Mobile m)
        {
            if (!(m is PlayerMobile))
                return false;

            Town citizenship = ((PlayerMobile)m).Citizenship;

            if (citizenship == null)
                return false;

            List<Multis.BaseHouse> houses = Multis.BaseHouse.GetHouses(m);


            foreach (Multis.BaseHouse bh in houses)
            {
                Rectangle3D[] areas = citizenship.Region.Area;
                for (int j = 0; j < areas.Length; j++)
                {
                    Rectangle3D rect = areas[j];
                    Rectangle3D expandedArea = new Rectangle3D(new Point3D(rect.Start.X - 100, rect.Start.Y - 100, rect.Start.Z), new Point3D(rect.End.X + 100, rect.End.Y + 100, rect.End.Z));
                    if (expandedArea.Contains(bh.Location))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool ValidHousePlacement(Mobile m, Point3D p)
        {
            foreach (Town town in Towns) 
            {
                Rectangle3D[] areas = town.Region.Area;
                for (int j = 0; j < areas.Length; j++ )
                {
                    Rectangle3D rect = areas[j];
                    Rectangle3D expandedArea = new Rectangle3D(new Point3D(rect.Start.X - 100, rect.Start.Y - 100, rect.Start.Z), new Point3D(rect.End.X + 100, rect.End.Y + 100, rect.End.Z));
                    if (expandedArea.Contains(p))
                        return (CheckCitizenship(m) == town);
                }
            }
            return true;
        }
        
		public void ConstructGuardLists()
		{
			GuardDefinition[] defs = (HomeFaction == null ? new GuardDefinition[0] : HomeFaction.Definition.Guards);

			m_GuardLists = new List<GuardList>();

			for ( int i = 0; i < defs.Length; ++i )
				m_GuardLists.Add( new GuardList( defs[i] ) );
		}

		public GuardList FindGuardList( Type type )
		{
			List<GuardList> guardLists = GuardLists;

			for ( int i = 0; i < guardLists.Count; ++i )
			{
				GuardList guardList = guardLists[i];

				if ( guardList.Definition.Type == type )
					return guardList;
			}

			return null;
		}
        /*
		public void ConstructVendorLists()
		{
			VendorDefinition[] defs = VendorDefinition.Definitions;

			m_VendorLists = new List<VendorList>();

			for ( int i = 0; i < defs.Length; ++i )
				m_VendorLists.Add( new VendorList( defs[i] ) );
		}

		public VendorList FindVendorList( Type type )
		{
			List<VendorList> vendorLists = VendorLists;

			for ( int i = 0; i < vendorLists.Count; ++i )
			{
				VendorList vendorList = vendorLists[i];

				if ( vendorList.Definition.Type == type )
					return vendorList;
			}

			return null;
		}
        */
		public bool RegisterGuard( BaseFactionGuard guard )
		{
			if ( guard == null )
				return false;

			GuardList guardList = FindGuardList( guard.GetType() );

			if ( guardList == null )
				return false;

			guardList.Guards.Add( guard );
			return true;
		}

		public bool UnregisterGuard( BaseFactionGuard guard )
		{
			if ( guard == null )
				return false;

			GuardList guardList = FindGuardList( guard.GetType() );

			if ( guardList == null )
				return false;

			if ( !guardList.Guards.Contains( guard ) )
				return false;

			guardList.Guards.Remove( guard );
			return true;
		}

        public static void InitElections_OnCommand(CommandEventArgs e)
        {
            foreach (Town town in Towns)
            {
                if (town.Election == null)
                    continue;

                Election election = town.Election;
                election.State = ElectionState.Pending;
                election.NextStateTime = Election.NextKing;
            }
        }

        public static void ResetTreasuries_OnCommand(CommandEventArgs e)
        {
            foreach (Town town in Towns)
            {
                town.Treasury = 100000;
                town.SecondaryCitizenshipBuffs.Clear();
                town.SalesTax = 0.24;
                town.GuardState = GuardStates.Standard;
            }
        }

        public static void DouseBraziers_OnCommand(CommandEventArgs e)
        {
            Town.DouseAllBraziers();
        }

        public static void InitCaptureBraziers_OnCommand(CommandEventArgs e)
        {
            new CaptureBrazier().MoveToWorld(new Point3D(4471, 1125, 0), Map.Felucca);  // moonglow
            new CaptureBrazier().MoveToWorld(new Point3D(1905, 2732, 20), Map.Felucca); // trinsic
            new CaptureBrazier().MoveToWorld(new Point3D(2420, 523, 0), Map.Felucca);   // minoc
            new CaptureBrazier().MoveToWorld(new Point3D(1414, 3709, 0), Map.Felucca);  // jhelom
            new CaptureBrazier().MoveToWorld(new Point3D(2213, 1117, 40), Map.Felucca); // cove
            new CaptureBrazier().MoveToWorld(new Point3D(3688, 2472, 0), Map.Felucca);  // ocllo
            new CaptureBrazier().MoveToWorld(new Point3D(613, 2126, 0), Map.Felucca);   // skara brae
            new CaptureBrazier().MoveToWorld(new Point3D(2836, 947, 0), Map.Felucca);   // vesper
            new CaptureBrazier().MoveToWorld(new Point3D(627, 848, 0), Map.Felucca);    // yew
            new CaptureBrazier().MoveToWorld(new Point3D(2944, 3344, 15), Map.Felucca); // serps
            new CaptureBrazier().MoveToWorld(new Point3D(3763, 1189, 30), Map.Felucca); // nujelm
            new CaptureBrazier().MoveToWorld(new Point3D(3783, 2259, 20), Map.Felucca); // magincia
            new CaptureBrazier().MoveToWorld(new Point3D(1529, 1416, 35), Map.Felucca); // britain
        }

        public static void FixBraziers_OnCommand(CommandEventArgs e)
        {
            foreach (Town town in Towns)
                town.Braziers.Clear();

            Queue q = new Queue();

            foreach (Item item in World.Items.Values)
            {
                if (!(item is TownBrazier))
                    continue;

                TownBrazier tb = item as TownBrazier;

                if (tb.Town == null)
                {
                    q.Enqueue(tb);
                    continue;
                }

                tb.Town.Braziers.Add(tb);
            }

            while (q.Count > 0)
                ((Item)q.Dequeue()).Delete();

            e.Mobile.SendMessage("Done.");
        }

        public static void ResetTowns_OnCommand(CommandEventArgs e)
        {
            foreach (Town town in Towns)
            {
                town.ControllingTown = town;
            }
        }

		public Town()
		{
			m_State = new TownState( this );
			//ConstructVendorLists();
            Timer.DelayCall(TimeSpan.FromTicks(1), ConstructGuardLists);
            Timer.DelayCall(TimeSpan.FromTicks(1), StartIncomeTimer);
		}

        public void BroadcastFactions(string text)
        {
            List<Mobile> townMobiles = Region.GetPlayers();

            foreach (Mobile m in townMobiles)
                if (Faction.Find(m) != null)
                    m.SendMessage(text);
        }
        public void AllianceChat(Mobile from, int hue, string text)
        {
            foreach (var playerState in Members)
            {
                if (playerState != null && playerState.Mobile != null && ((PlayerMobile)playerState.Mobile).ShowTownChat)
                    ((PlayerMobile)playerState.Mobile).SendAllianceMessage(from, hue, text);
            }
        }

        public void DouseBraziers()
        {
            foreach (TownBrazier b in Braziers)
                b.Captured = false;

            if (CaptureBrazier != null)
                CaptureBrazier.Captured = false;
        }

		public void Capture( Town t, bool message = true )
		{
			if (m_State.ControllingTown == t)
				return;

            DouseBraziers();

			Town oldOwner = m_State.ControllingTown;
			m_State.ControllingTown = t;
            if (message)
            {
				World.Broadcast(0x35, true, String.Format("{0} has taken control of {1}!", t.Definition.FriendlyName, Definition.FriendlyName));

				string gain = String.Format("Your town has gained control of {0}!", Definition.FriendlyName);
                string lose = String.Format("Your town has lost control of {0}!",   Definition.FriendlyName);

                foreach (Network.NetState ns in Network.NetState.Instances)
                {
                    Mobile mob = ns.Mobile;
                    Town mobTown = Town.Find(mob);

                    if (mobTown == null)
                        continue;

                    if (mobTown == oldOwner)
                    {
                        mob.LocalOverheadMessage(Network.MessageType.Regular, 0x0, true, lose);
                        mob.PlaySound(0x1E1);
                        mob.FixedParticles(0x374A, 10, 15, 5028, EffectLayer.Waist);
                    }
                    else if (mobTown == t)
                    {
                        mob.LocalOverheadMessage(Network.MessageType.Regular, 0x0, true, gain);
                        mob.PlaySound(0x1E7);
                        mob.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);
                    }
                }
            }
            
            foreach (TownCrystal c in Crystals)
                c.Invalidate();
        }

        public static Town Find(Mobile mob)
        {
            return Find(mob, false);
        }

        // only returns town for a militia member
        public static Town Find(Mobile mob, bool inherit)
        {
            PlayerState pl = PlayerState.Find(mob);

            if (pl != null)
                return pl.Town;

            if (inherit && mob is BaseCreature)
            {
                BaseCreature bc = (BaseCreature)mob;

                if (bc.Controlled)
                    return Find(bc.ControlMaster, false);
                else if (bc.Summoned)
                    return Find(bc.SummonMaster, false);
                else if (mob is BaseFactionGuard)
                    return ((BaseFactionGuard)mob).Town;
            }

            return null;
        }

        public bool FactionMessageReady
        {
            get { return m_State.FactionMessageReady; }
        }

		public int CompareTo( object obj )
		{
			return m_Definition.Sort - ((Town)obj).m_Definition.Sort;
		}

		public override string ToString()
		{
			return m_Definition.FriendlyName;
		}

		public static void WriteReference( GenericWriter writer, Town town )
		{
			int idx = Towns.IndexOf( town );

			writer.WriteEncodedInt( (int) (idx + 1) );
		}

		public static Town ReadReference( GenericReader reader )
		{
			int idx = reader.ReadEncodedInt() - 1;

			if ( idx >= 0 && idx < Towns.Count )
				return Towns[idx];

			return null;
		}

		public static Town Parse( string name )
		{
			List<Town> towns = Towns;

			for ( int i = 0; i < towns.Count; ++i )
			{
				Town town = towns[i];

				if ( Insensitive.Equals( town.Definition.FriendlyName, name ) )
					return town;
			}

			return null;
		}

        public static void AddCitizen(Mobile m, Town town)
        {
            if (!(m is PlayerMobile) || town == null)
                return;

            PlayerMobile pm = (PlayerMobile)m;

            if (pm.Citizenship != null)
            {
                if (pm.Citizenship == town)
                    m.SendMessage("You are already a member of this town.");
                else
                    m.SendMessage("You must revoke your current citizenship to join a new town.");

                return;
            }
            else if (town == null)
            {
                m.SendMessage("You must be in a town to request citizenship.");
                return;
            }
            else
            {
                Accounting.Account acc = m.Account as Accounting.Account;

                foreach (Mobile mob in acc.accountMobiles)
                {
                    if (mob == null || ((PlayerMobile)mob).Citizenship != null)
                        continue;

                    CitizenshipState state = new CitizenshipState(mob, town);
                    town.Members.Add(state);
                }

                m.SendMessage(String.Format("You are now a citizen of {0}!", town.Definition.FriendlyName));
            }

        }

        public static void RemoveCitizen(Mobile m, bool deleting)
        {
            if (!(m is PlayerMobile))
                return;

            PlayerMobile pm = (PlayerMobile)m;
            Town town = pm.Citizenship;

            if (town == null)
            {
                m.SendMessage("You are not currently a citizen of any town.");
            }
            else if (deleting)
            {
                CitizenshipState cs = pm.CitizenshipPlayerState;

                if (town.Members.Contains(cs))
                    town.Members.Remove(cs);
            }
            else
            {
                Accounting.Account acc = m.Account as Accounting.Account;

                foreach (Mobile mob in acc.accountMobiles)
                {
                    if (mob == null)
                        continue;

                    if (town.King == mob)
                    {
                        town.King = null;
                        town.AddTownCrierEntry(new string[] { "The king has abandonded his duties!", "The town has no king!", "Prepare for a new election!" }, TimeSpan.FromHours(4));
                        //town.Election.ResetNextElection();
                    }

                    if (town.Election.IsCandidate(mob))
                        town.Election.RemoveCandidate(mob);

                    town.Election.RemoveVoter(mob);

                    mob.SendMessage(String.Format("You are no longer a citizen of {0}.", town.Definition.FriendlyName));

                    Faction f = Faction.Find(mob);
                    if (f != null)
                        f.RemoveMember(mob);

                    if (town.MilitiaMembers.Contains(mob))
                        town.RemoveMilitiaMember(mob);

                    CitizenshipState cs = ((PlayerMobile)mob).CitizenshipPlayerState;

                    if (town.Members.Contains(cs))
                        town.Members.Remove(cs);

                    ((PlayerMobile)mob).CitizenshipPlayerState = null;
                }
            }
        }

        //FIX THIS AREA:

        public static bool CanLeave(Mobile mob)
        {
            if (mob.AccessLevel > AccessLevel.Player)
                return true;

            CitizenshipState cs = CitizenshipState.Find(mob);

            if (cs == null)
                return false;

			if ((cs.StartDate + LeavePeriod) >= DateTime.Now)
            {
				TimeSpan left = (cs.StartDate + LeavePeriod) - DateTime.Now;
                mob.SendMessage(String.Format("You must wait {0} days, {1} hours, and {2} minutes longer until you can revoke your citizenship.", left.Days, left.Hours, left.Minutes));
                return false;
            }
          
            Accounting.Account acc = mob.Account as Accounting.Account;

            if (acc == null)
                return false;

            foreach (Mobile m in acc.accountMobiles)
            {
                if (m == null)
                    continue;

                if (HasHouseInTownRegion(m))
                {
                    mob.SendMessage("You must move your house out of the restricted housing area before you can revoke your citizenship.");
                    return false;
                }
            }

            foreach (MarketFloor mf in cs.Town.MarketTiles)
            {
                IPooledEnumerable eable = Map.Felucca.GetMobilesInRange(mf.Location, 0);
                foreach (Mobile m in eable)
                {
                    if (m is PlayerVendor && ((PlayerVendor)m).Owner == mob)
                    {
                        mob.SendMessage("You cannot revoke your citizenship with an active Vendor in town.");
                        eable.Free();
                        return false;
                    }
                }
                eable.Free();
            }

            return true;
        }

        public static bool CheckLeaveTimer(Mobile mob)
        {
            CitizenshipState cs = CitizenshipState.Find(mob);

            if (cs == null || !cs.IsLeaving)
                return false;

			if ((cs.Leaving + TimeSpan.FromHours(1)) >= DateTime.Now)
                return false;

            if (!CanLeave(mob))
                return false;

            Accounting.Account acc = mob.Account as Accounting.Account;

            foreach (Mobile m in acc.accountMobiles)
            {
                if (m == null)
                    continue;

                Town.RemoveCitizen(m, false);
            }

            mob.SendMessage("You have now left the town.");
            return true;
        }

        public static Town CheckAccountCitizenship(Mobile m)
        {
            Accounting.Account acc = m.Account as Accounting.Account;

            foreach (Mobile mob in acc.accountMobiles)
            {
                if (mob == null)
                    continue;

                Town t = ((PlayerMobile)mob).Citizenship;
                if (t != null)
                    return t;
            }
            return null;
        }

        public static Town CheckCitizenship(Mobile m)
        {
            if (!(m is PlayerMobile))
                return null;

            return ((PlayerMobile)m).Citizenship;
        }

        private static void EventSink_Login(LoginEventArgs e)
        {
            Mobile mob = e.Mobile;

            CheckLeaveTimer(mob);
        }

        public void ProcessUpkeep()
        {
            if (LastBuffUpkeep + BuffUpkeepPeriod > DateTime.UtcNow)
                return;

            var removals = new List<CitizenshipBuffs>();
            int spent = 0;
            foreach (var buff in SecondaryCitizenshipBuffs)
            {
                if (Treasury >= BuffUpkeepCost)
                {
                    Treasury -= BuffUpkeepCost;
                    spent += BuffUpkeepCost;
                }
                else
                {
                    removals.Add(buff);
                }
            }

            foreach (var buff in removals)
                SecondaryCitizenshipBuffs.Remove(buff);

            if (removals.Count > 0)
            {
                var output = new StringBuilder("The following buffs have been removed due to insufficient funds: ");
                removals.ForEach(r => output.Append(string.Format("{0} ", TownBuff.GetBuffName(r))));
                Broadcast(output.ToString());
            }

            if (spent > 0)
            {
                WithdrawLog.AddLogEntry(King, spent, TreasuryLogType.CitizenBuffs);
            }

            LastBuffUpkeep = DateTime.UtcNow;
        }

        public void SpawnCreatures(int Min, int Max)
        {
            Queue Q = new Queue();
            foreach (BaseCreature b in SpawnedCreatures)
            {
                if (b.Hits == b.HitsMax)
                    Q.Enqueue(b);
            }
            while (Q.Count > 0)
                ((BaseCreature)Q.Dequeue()).Delete();

            Point3D centerLoc = Region.GoLocation;
            Point3D spawnLoc;
            int maxtries = 10;
            int tries = 0;
            bool foundLoc = false;
            do
            {
                spawnLoc = new Point3D(RandomRange(centerLoc.X), RandomRange(centerLoc.Y), centerLoc.Z);
                spawnLoc.Z = Map.Felucca.GetAverageZ(spawnLoc.X, spawnLoc.Y);
                if (Map.Felucca.CanFit(spawnLoc, 1))
                    foundLoc = true;
                tries++;
            }
            while (foundLoc == false && tries < maxtries);

            for (int i = 0; i < Utility.RandomMinMax(Min, Max); i++)
            {
                BaseCreature b = (BaseCreature)Activator.CreateInstance((Type)m_SpawnTypes[Utility.Random(m_SpawnTypes.Length)]);
                b.MoveToWorld(spawnLoc, Map.Felucca);
                b.Home = centerLoc;
                b.RangeHome = 50;
                SpawnedCreatures.Add(b);
                //Console.Write("\nSpawned Creature {0} at {1}", b.Name, b.Location);
            }

			LastSpawnCreatures = DateTime.Now;
        } //TODO: CHANGE THIS

        private int RandomRange(int value)
        {
            return (value - Utility.Random(100) + 50);
        }

        public void Broadcast(string text)
        {
            Broadcast(0x3B2, text);
        }

        public void Broadcast(int hue, string text)
        {
            for (int i = 0; i < Members.Count; ++i)
            {
                Mobile m = Members[i].Mobile;
                if (m != null)
                    m.SendMessage(hue, text);
            }
        }

        public void Broadcast(string format, params object[] args)
        {
            Broadcast(String.Format(format, args));
        }

        public void Broadcast(int hue, string format, params object[] args)
        {
            Broadcast(hue, String.Format(format, args));
        }

        public void BeginBroadcast(Mobile from)
        {
            from.SendMessage("Enter Town Message");
            from.Prompt = new BroadcastPrompt(this);
        }

        public void EndBroadcast(Mobile from, string text)
        {
            Broadcast(0, "{0} [King] {1} : {2}", from.Name, Definition.FriendlyName, text);
        }

        private class BroadcastPrompt : Prompt
        {
            private Town m_Town;

            public BroadcastPrompt(Town town)
            {
                m_Town = town;
            }

            public override void OnResponse(Mobile from, string text)
            {
                m_Town.EndBroadcast(from, text);
            }
        }

        public class GuardedRegionTimer : Timer
        {
            public GuardedRegionTimer()
                : base(TimeSpan.Zero, TimeSpan.FromMinutes(5))
            {
                Priority = TimerPriority.OneMinute;
            }

            protected override void OnTick()
            {
                foreach (Town town in Towns)
                {
                    if (town.GuardState <= GuardStates.Lax) //Spawn Creatures
                    {
                        if (town.GuardState == GuardStates.Lax)
                        {
							if (town.LastSpawnCreatures + TimeSpan.FromHours(6) < DateTime.Now)
                                town.SpawnCreatures(2, 3);
                        }
                        else //None
							if (town.LastSpawnCreatures + TimeSpan.FromHours(1) < DateTime.Now)
                                town.SpawnCreatures(2, 5);
                    }
                }
            }
        }

        public class TownUpkeepTimer : Timer
        {
            private Town m_Town;

            public TownUpkeepTimer(Town town)
                : base(TimeSpan.Zero, TimeSpan.FromHours(1))
            {
                Priority = TimerPriority.OneMinute;
                m_Town = town;
            }

            protected override void OnTick()
            {
                base.OnTick();
                if (m_Town == null)
                {
                    Stop();
                    return;
                }

                m_Town.ProcessUpkeep();
            }
        }

        public int ProrateSecondaryBuffAmount
        {
            get
            {
                var distance = ((LastBuffUpkeep + BuffUpkeepPeriod) - DateTime.UtcNow).Days;

                var prorate = (Town.BuffUpkeepCost / BuffUpkeepPeriod.Days) * distance;

                if (distance == 0)
                    return Town.BuffUpkeepCost;
                else
                    return prorate;
            }
        }

        public void AdjustSecondaryBuffs(Mobile from, List<CitizenshipBuffs> added, List<CitizenshipBuffs> removals)
        {
            if (LastBuffUpkeep == DateTime.MinValue)
                LastBuffUpkeep = DateTime.UtcNow - BuffUpkeepPeriod;

            int prorate = ProrateSecondaryBuffAmount;
            int totalCost = (prorate * added.Count) - (prorate * removals.Count);

            double adjusted = Treasury - totalCost;

            if (adjusted < 0)
            {
                from.SendMessage("Your town lacks sufficient funds to purchase those buffs.");
            }
            else
            {
                Treasury -= totalCost;

                foreach (var buff in removals)
                    SecondaryCitizenshipBuffs.Remove(buff);
                foreach (var buff in added)
                    SecondaryCitizenshipBuffs.Add(buff);

                if (removals.Count > 0 || added.Count > 0)
                {
                    from.SendMessage("Your town seconday buffs have been updated!");
                    Broadcast(string.Format("{0} buffs have been updated!", Definition.FriendlyName));
                    LastBuffUpdate = DateTime.UtcNow;
                }

            }
        }

    }
}