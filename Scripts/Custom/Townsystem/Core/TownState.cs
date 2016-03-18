/***************************************************************************
 *                               TownState.cs
 *                            ------------------
 *   begin                : August 2010
 *   author               : Sean Stavropoulos
 *   email                : sean.stavro@gmail.com
 *
 *
 ***************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Regions;
using Server.Custom.Townsystem.Core;

namespace Server.Custom.Townsystem
{
    public class TownState
    {
        private const int BROADCASTS_PER_PERIOD = 2;
        private static readonly TimeSpan BROADCAST_PERIOD = TimeSpan.FromHours(12.0);

        private Town m_Town;
        private Mobile m_King;
        private Mobile m_Commander;
        private Town m_ControllingTown;
        private Faction m_HomeFaction;
        private List<CitizenshipState> m_Members;
        private List<Mobile> m_MilitiaMembers;
        private List<Mobile> m_Exiles;
        private List<TownBrazier> m_Braziers;
        private CaptureBrazier m_CaptureBrazier;
        private List<TownCrystal> m_Crystals;
        private List<TownFlag> m_Flags;
        private List<OutcastEntry> m_OutcastEntries;
        private KingStone m_KingStone;
        private MilitiaStone m_MilitiaStone;
        private TownOutcastBoard m_OutcastBoard;
        private Election m_Election;
        private DateTime m_LastSpawnCreatures;
        private List<BaseCreature> m_SpawnedCreatures;
        private int m_TotalVoted;
        private Revolt.RevoltStone m_TownRevoltStone;
        private double m_Treasury;
        private TreasuryWallTypes m_TreasuryWallType;
        private List<TreasuryWall> m_TreasuryWalls;
        private double m_SalesTax;
        private double m_PropertyTax;
        private DateTime m_LastTaxChange;
        private DateTime m_ControlStartTime;
        private GuardStates m_GuardState;
        private List<TownCrier> m_TownCriers;
        private DateTime[] m_LastBroadcasts = new DateTime[BROADCASTS_PER_PERIOD];
        private CitizenshipBuffs m_PrimaryCitizenshipBuff;
        private List<CitizenshipBuffs> m_SecondaryCitizenshipBuffs;
        private DateTime m_LastBuffUpkeep = DateTime.MinValue;
        private DateTime m_LastBuffUpdate = DateTime.MinValue;
        private bool m_AllowNewCitizenshipBuffs;
        private List<MarketFloor> m_MarketTiles;
        private DateTime m_LastUpkeep;
        private TreasuryLog m_WithdrawLog;
        private DateTime m_LastTreasuryWithdraw;
        private DateTime m_LastExile;
        private Withdraws m_Withdraws;
        private DateTime m_LastTheft;
        private DateTime m_LastGuardStateChange;
        private int m_AvailableBombPlans;
        //private TownCaptureBanner m_CaptureBanner;
        private List<AllianceFlag> m_AllianceFlags;
        private DateTime m_LastBombPlanPurchase;

        public Mobile King
        {
            get { return m_King; }
            set
            {
                if (m_King != null)
                {
                    PlayerState pl = PlayerState.Find(m_King);

                    if (pl != null)
                        pl.King = null;
                }

                m_King = value;

                if (m_King != null)
                {
                    PlayerState pl = PlayerState.Find(m_King);

                    if (pl != null)
                        pl.King = m_Town;
                }
            }
        }
        public Mobile Commander
        {
            get { return m_Commander; }
            set { m_Commander = value; }
        }
        public Town Town
        {
            get { return m_Town; }
            set { m_Town = value; }
        }
        // factions no longer control towns, towns do.
        public Town ControllingTown
        {
            get { return m_ControllingTown == null ? Town : m_ControllingTown; }
            set
            {
                m_ControllingTown = value;
                TownDecorationItem.OnTownChanged(Town); // controlling faction changed
            }
        }

        public Faction HomeFaction
        {
            get { return m_HomeFaction; }
            set { m_HomeFaction = value; }
        }
        public GuardStates GuardState
        {
            get { return m_GuardState; }
            set { m_GuardState = value; }
        }
        public List<TownCrier> TownCriers
        {
            get 
            {
                m_TownCriers.RemoveAll(tc => tc == null);
                return m_TownCriers; 
            }
            set { m_TownCriers = value; }
        }
        public int TotalVoted
        {
            get { return m_TotalVoted; }
            set { m_TotalVoted = value; }
        }
        public Revolt.RevoltStone TownRevoltStone
        {
            get { return m_TownRevoltStone; }
            set { m_TownRevoltStone = value; }
        }
        public List<CitizenshipState> Members
        {
            get { return m_Members; }
            set { m_Members = value; }
        }
        public List<Mobile> MilitiaMembers
        {
            get { return m_MilitiaMembers; }
            set { m_MilitiaMembers = value; }
        }
        public List<Mobile> Exiles
        {
            get { return m_Exiles; }
            set { m_Exiles = value; }
        }
        public List<TownBrazier> Braziers
        {
            get { return m_Braziers; }
            set { m_Braziers = value; }
        }
        public CaptureBrazier CaptureBrazier
        {
            get { return m_CaptureBrazier; }
            set { m_CaptureBrazier = value; }
        }
        public List<TownCrystal> Crystals
        {
            get { return m_Crystals; }
            set { m_Crystals = value; }
        }
        public List<TownFlag> Flags
        {
            get { return m_Flags; }
            set { m_Flags = value; }
        }
        public List<OutcastEntry> OutcastEntries
        {
            get { return m_OutcastEntries; }
            set { m_OutcastEntries = value; }
        }
        public List<BaseCreature> SpawnedCreatures
        {
            get { return m_SpawnedCreatures; }
            set { m_SpawnedCreatures = value; }
        }
        public DateTime LastSpawnCreatures
        {
            get { return m_LastSpawnCreatures; }
            set { m_LastSpawnCreatures = value; }
        }
        public TownOutcastBoard OutcastBoard
        {
            get { return m_OutcastBoard; }
            set { m_OutcastBoard = value; }
        }
        public KingStone KingStone
        {
            get { return m_KingStone; }
            set { m_KingStone = value; }
        }
        public MilitiaStone MilitiaStone
        {
            get { return m_MilitiaStone; }
            set { m_MilitiaStone = value; }
        }
        public Election Election
        {
            get { return m_Election; }
            set { m_Election = value; }
        }
        public double SalesTax
        {
            get { return m_SalesTax; }
            set { m_SalesTax = value; }
        }
        public double PropertyTax
        {
            get { return m_PropertyTax; }
            set { m_PropertyTax = value; }
        }
        public double Treasury
        {
            get { return m_Treasury; }
            set { m_Treasury = value; }
        }
        public TreasuryWallTypes TreasuryWallType
        {
            get { return m_TreasuryWallType; }
            set { m_TreasuryWallType = value; }
        }
        public List<TreasuryWall> TreasuryWalls
        {
            get { return m_TreasuryWalls; }
            set { m_TreasuryWalls = value; }
        }
        public DateTime LastTaxChange
        {
            get { return m_LastTaxChange; }
            set { m_LastTaxChange = value; }
        }
        public DateTime ControlStartTime
        {
            get { return m_ControlStartTime; }
            set { m_ControlStartTime = value; }
        }
        public CitizenshipBuffs PrimaryCitizenshipBuff
        {
            get { return m_PrimaryCitizenshipBuff; }
            set { m_PrimaryCitizenshipBuff = value; }
        }

        public DateTime LastBuffUpkeep
        {
            get { return m_LastBuffUpkeep; }
            set { m_LastBuffUpkeep = value; }
        }

        public DateTime LastBuffUpdate
        {
            get { return m_LastBuffUpdate; }
            set { m_LastBuffUpdate = value; }
        }

        public List<CitizenshipBuffs> SecondaryCitizenshipBuffs
        {
            get { return m_SecondaryCitizenshipBuffs; }
        }

        public bool AllowNewCitizenshipBuffs
        {
            get { return m_AllowNewCitizenshipBuffs; }
            set { m_AllowNewCitizenshipBuffs = value; }
        }
        public List<MarketFloor> MarketTiles
        {
            get { return m_MarketTiles; }
            set { m_MarketTiles = value; }
        }
        public DateTime LastUpkeep
        {
            get { return m_LastUpkeep; }
            set { m_LastUpkeep = value; }
        }
        public TreasuryLog WithdrawLog
        {
            get { return m_WithdrawLog; }
            set { m_WithdrawLog = value; }
        }
        public DateTime LastTreasuryWithdraw
        {
            get { return m_LastTreasuryWithdraw; }
            set { m_LastTreasuryWithdraw = value; }
        }
        public DateTime LastExile
        {
            get { return m_LastExile; }
            set { m_LastExile = value; }
        }
        public Withdraws Withdraws
        {
            get { return m_Withdraws; }
            set { m_Withdraws = value; }
        }
        public DateTime LastTheft
        {
            get { return m_LastTheft; }
            set { m_LastTheft = value; }
        }
        public DateTime LastGuardStateChange
        {
            get { return m_LastGuardStateChange; }
            set { m_LastGuardStateChange = value; }
        }
        public int AvailableBombPlans
        {
            get { return m_AvailableBombPlans; ; }
            set { m_AvailableBombPlans = value; }
        }
        public List<AllianceFlag> AllianceFlags
        {
            get { return m_AllianceFlags; }
            set { m_AllianceFlags = value; }
        }
        public DateTime LastBombPlanPurchase
        {
            get { return m_LastBombPlanPurchase; }
            set { m_LastBombPlanPurchase = value; }
        }
        public bool ExilesDisabled { get; set; }

        //public TownCaptureBanner CaptureBanner { get { return m_CaptureBanner; } set { m_CaptureBanner = value; } }

        public bool FactionMessageReady
        {
            get
            {
                for (int i = 0; i < m_LastBroadcasts.Length; ++i)
                {
                    if (DateTime.Now >= (m_LastBroadcasts[i] + BROADCAST_PERIOD))
                        return true;
                }

                return false;
            }
        }

        public void RegisterBroadcast()
        {
            for (int i = 0; i < m_LastBroadcasts.Length; ++i)
            {
                if (DateTime.Now >= (m_LastBroadcasts[i] + BROADCAST_PERIOD))
                {
                    m_LastBroadcasts[i] = DateTime.Now;
                    break;
                }
            }
        }

        public TownState(Town town)
        {
            m_Town = town;
            m_Members = new List<CitizenshipState>();
            m_MilitiaMembers = new List<Mobile>();
            m_Exiles = new List<Mobile>();
            m_Crystals = new List<TownCrystal>();
            m_Braziers = new List<TownBrazier>();
            m_Flags = new List<TownFlag>();
            m_SpawnedCreatures = new List<BaseCreature>();
            m_Treasury = 0;
            m_SalesTax = 0.25;
            m_PropertyTax = 0.25;
            m_ControlStartTime = DateTime.MinValue;
            m_LastSpawnCreatures = DateTime.MinValue;
            m_TreasuryWalls = new List<TreasuryWall>();
            m_TreasuryWallType = TreasuryWallTypes.None;
            m_Election = new Election(town);
            m_OutcastEntries = new List<OutcastEntry>();
            m_TownCriers = new List<TownCrier>();
            m_TotalVoted = 0;
            m_AllowNewCitizenshipBuffs = true;
            m_MarketTiles = new List<MarketFloor>();
            m_LastUpkeep = DateTime.Now;
            m_WithdrawLog = new TreasuryLog();
            m_LastTreasuryWithdraw = DateTime.Now;
            m_Withdraws = new Withdraws(m_Town);
            m_AllianceFlags = new List<AllianceFlag>();
            m_SecondaryCitizenshipBuffs = new List<CitizenshipBuffs>();
        }

        public TownState(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();
            Dictionary<Town, int> CaptureDict = null;

            if (version == 0)
                m_TownCriers = new List<TownCrier>();
            else if (version == 5)
                m_WithdrawLog = new TreasuryLog();

            if (version < 13)
                m_AllianceFlags = new List<AllianceFlag>();


            m_SecondaryCitizenshipBuffs = new List<CitizenshipBuffs>();

            switch (version)
            {
                case 23:
                    {
                        m_CaptureBrazier = reader.ReadItem() as CaptureBrazier;
                        goto case 22;
                    }
                case 22:
                    {
                        m_LastBuffUpdate = reader.ReadDateTime();
                        goto case 21;
                    }
                case 21:
                    {
                        int count = reader.ReadInt();
                        for (int i = 0; i < count; i++)
                        {
                            int buff = reader.ReadInt();
                            if (Enum.IsDefined(typeof(CitizenshipBuffs), buff))
                            {
                                m_SecondaryCitizenshipBuffs.Add((CitizenshipBuffs)buff);
                            }
                        }
                        m_LastBuffUpkeep = reader.ReadDateTime();
                        goto case 20;
                    }
                case 20:
                    {
                        reader.ReadBool();
                        goto case 19;
                    }
                case 19:
                    {
                        m_Town = Town.ReadReference(reader);
                        m_ControllingTown = Town.ReadReference(reader);

                        if (reader.ReadBool())
                        {
                            int count = reader.ReadInt();
                            CaptureDict = new Dictionary<Town, int>(Town.Towns.Count);
                            for (int i = 0; i < count; ++i)
                            {
                                var town = Town.ReadReference(reader);
                                int points = reader.ReadInt();
                                if (town != null)
                                    CaptureDict.Add(town, points);
                            }
                        }
                        goto case 18;
                    }
                case 18:
                    {
                        goto case 17;
                    }
                case 17:
                    {
                        goto case 16;
                    }
                case 16:
                    {
                        m_Commander = reader.ReadMobile();
                        goto case 15;
                    }
                case 15:
                    {
                        ExilesDisabled = reader.ReadBool();
                        goto case 14;
                    }
                case 14:
                    {
                        m_LastBombPlanPurchase = reader.ReadDateTime();
                        goto case 13;
                    }
                case 13:
                    {
                        int count = reader.ReadInt();
                        m_AllianceFlags = new List<AllianceFlag>();
                        for (int i = 0; i < count; i++)
                        {
                            var flag = reader.ReadItem() as AllianceFlag;
                            if (flag != null)
                                m_AllianceFlags.Add(flag);
                        }
                        goto case 12;
                    }
                case 12:
                    {
                        if (17 > version)
                            reader.ReadItem(); // old towncapture banner
                        goto case 11;
                    }
                case 11:
                    {
                        // moved up to 19
                        if (version < 19)
                        {
                            if (reader.ReadBool())
                            {
                                // still read old data, do nothing with it
                                int count = reader.ReadInt();
                                Dictionary<int, int> oldData = new Dictionary<int, int>(Town.Towns.Count);
                                for (int i = 0; i < count; ++i)
                                {
                                    var faction = Faction.ReadReference(reader);
                                    int points = reader.ReadInt();
                                }
                            }
                        }
                        goto case 10;
                    }
                case 10:
                    {
                        m_AvailableBombPlans = reader.ReadInt();
                        goto case 9;
                    }
                case 9:
                    {
                        m_LastGuardStateChange = reader.ReadDateTime();
                        goto case 8;
                    }
                case 8:
                    {
                        m_LastExile = reader.ReadDateTime();
                        m_LastTheft = reader.ReadDateTime();
                        goto case 7;
                    }
                case 7:
                    {
                        m_LastTreasuryWithdraw = reader.ReadDateTime();
                        goto case 6;
                    }
                case 6:
                    {
                        m_WithdrawLog = new TreasuryLog(reader);
                        goto case 5;
                    }
                case 5:
                    {
                        m_LastUpkeep = reader.ReadDateTime();
                        goto case 4;
                    }
                case 4:
                    {
                        m_MarketTiles = new List<MarketFloor>();
                        int count = reader.ReadInt();
                        for (int i = 0; i < count; i++)
                            m_MarketTiles.Add(reader.ReadItem() as MarketFloor);

                        goto case 3;
                    }
                case 3:
                    {
                        m_AllowNewCitizenshipBuffs = reader.ReadBool();

                        int buff = reader.ReadInt();
                        if (buff > ((CitizenshipBuffs[])Enum.GetValues(typeof(CitizenshipBuffs))).Length - 1)
                            buff = 1; //set to first valid buff

                        m_PrimaryCitizenshipBuff = (CitizenshipBuffs)buff;

                        goto case 2;
                    }
                case 2:
                    {
                        m_TotalVoted = reader.ReadInt();
                        m_TownRevoltStone = reader.ReadItem() as Revolt.RevoltStone;

                        goto case 1;
                    }
                case 1:
                    {
                        int count = reader.ReadEncodedInt();
                        m_TownCriers = new List<TownCrier>();
                        for (int i = 0; i < count; i++)
                            m_TownCriers.Add((TownCrier)reader.ReadMobile());

                        goto case 0;
                    }
                case 0:
                    {
                        int count = reader.ReadEncodedInt();

                        for (int i = 0; i < count; ++i)
                        {
                            DateTime time = reader.ReadDateTime();

                            if (i < m_LastBroadcasts.Length)
                                m_LastBroadcasts[i] = time;
                        }

                        m_Treasury = reader.ReadLong();
                        m_SalesTax = reader.ReadDouble();
                        m_PropertyTax = reader.ReadDouble();
                        m_LastTaxChange = reader.ReadDateTime();

                        m_ControlStartTime = reader.ReadDateTime();
                        m_LastSpawnCreatures = reader.ReadDateTime();

                        // replaced as of version 19
                        if (version < 19)
                        {
                            m_Town = Town.ReadReference(reader);
                            Faction.ReadReference(reader);
                        }

                        m_King = reader.ReadMobile();

                        int memberCount = reader.ReadEncodedInt();
                        m_Members = new List<CitizenshipState>();
                        for (int i = 0; i < memberCount; ++i)
                        {
                            CitizenshipState cs = new CitizenshipState(reader, m_Town);

                            if (cs.Mobile != null && cs.Town == m_Town && ((PlayerMobile)cs.Mobile).CitizenshipPlayerState == cs)
                                m_Members.Add(cs);
                        }

                        m_MilitiaMembers = new List<Mobile>();
                        int militiamembercount = reader.ReadEncodedInt();
                        for (int i = 0; i < militiamembercount; ++i)
                        {
                            Mobile m = reader.ReadMobile();
                            if (m != null)
                                m_MilitiaMembers.Add(m);
                        }

                        m_Exiles = new List<Mobile>();
                        int exilesCount = reader.ReadEncodedInt();
                        for (int i = 0; i < exilesCount; ++i)
                        {
                            Mobile m = reader.ReadMobile();
                            if (m != null)
                                m_Exiles.Add(m);
                        }

                        m_Crystals = new List<TownCrystal>();
                        int crystalCount = reader.ReadEncodedInt();
                        for (int i = 0; i < crystalCount; ++i)
                            m_Crystals.Add((TownCrystal)reader.ReadItem());

                        m_Braziers = new List<TownBrazier>();
                        int brazierCount = reader.ReadEncodedInt();
                        for (int i = 0; i < brazierCount; ++i)
                        {
                            TownBrazier tb = (TownBrazier)reader.ReadItem();
                            if (tb != null)
                                m_Braziers.Add(tb);
                        }

                        m_Flags = new List<TownFlag>();
                        int flagCount = reader.ReadEncodedInt();
                        for (int i = 0; i < flagCount; ++i)
                            m_Flags.Add((TownFlag)reader.ReadItem());

                        m_SpawnedCreatures = new List<BaseCreature>();
                        int spawnedCount = reader.ReadEncodedInt();
                        for (int i = 0; i < spawnedCount; ++i)
                            m_SpawnedCreatures.Add((BaseCreature)reader.ReadMobile());

                        m_TreasuryWalls = new List<TreasuryWall>();
                        int wallCount = reader.ReadEncodedInt();
                        for (int i = 0; i < wallCount; ++i)
                            m_TreasuryWalls.Add((TreasuryWall)reader.ReadItem());

                        m_OutcastEntries = new List<OutcastEntry>();
                        int outcastCount = reader.ReadEncodedInt();
                        for (int i = 0; i < outcastCount; ++i)
                            m_OutcastEntries.Add(new OutcastEntry(reader));

                        bool dummy = reader.ReadBool(); // UNUSED! legacy, tictactoe capture
                        dummy = reader.ReadBool(); // UNUSED! legacy, allow new decreed

                        m_KingStone = (KingStone)reader.ReadItem();
                        m_MilitiaStone = (MilitiaStone)reader.ReadItem();

                        m_TreasuryWallType = (TreasuryWallTypes)reader.ReadEncodedInt();
                        m_TreasuryWallType = TreasuryWallTypes.None; //removing this shit

                        m_GuardState = (GuardStates)reader.ReadEncodedInt();

                        m_OutcastBoard = (TownOutcastBoard)reader.ReadItem();

                        if (version < 18)
                            reader.ReadItem(); // pre version 18 this was the serialized TreasuryChest

                        m_Election = new Election(reader);

                        m_HomeFaction = Faction.ReadReference(reader);

                        if (version > 7)
                            m_Withdraws = Withdraws.Deserialize(m_Town, reader);
                        else
                        {
                            m_Withdraws = new Townsystem.Withdraws(m_Town);
                            m_Treasury -= Members.Count * 6 * 25; //retroactive guards
                            if (m_Treasury < 0)
                                m_Treasury = 0;
                        }

                        m_Town.State = this;
                    } break;
            }

            if (version < 10)
            {
                m_HomeFaction = Faction.Parse(m_Town.Definition.FriendlyName);
                //ControllingFaction = m_HomeFaction;

                bool rejoin = true;

                for (int i = 0; i < MilitiaMembers.Count; ++i)
                {
                    Mobile m = MilitiaMembers[i];
                    PlayerMobile pm = m as PlayerMobile;
                    rejoin = true;

                    if (pm.TownsystemPlayerState != null && pm.TownsystemPlayerState.IsLeaving)
                        rejoin = false;

                    pm.IsInMilitia = false;
                    if (rejoin)
                        pm.IsInMilitia = true;
                }
            }

            if (CaptureDict != null && CaptureDict.Count > 0)
            {
                m_Town.KingOfTheHillTimer = new KingOfTheHillTimer(m_Town);
                m_Town.KingOfTheHillTimer.TownCapturePoints = CaptureDict;
                m_Town.KingOfTheHillTimer.Start();
            }

            m_Town.UpkeepTimer = new Town.TownUpkeepTimer(m_Town);
            m_Town.UpkeepTimer.Start();
        }

        public void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt((int)23); // version

            // version 23 capture brazier
            writer.Write(m_CaptureBrazier);

            // version 22 time between secondary buff changes
            writer.Write(m_LastBuffUpdate);

            // version 21 secondary buffs
            writer.Write(m_SecondaryCitizenshipBuffs.Count);
            foreach (var buff in m_SecondaryCitizenshipBuffs)
                writer.Write((int)buff);

            writer.Write(m_LastBuffUpkeep);

            // version 20 KOTH timer
            if (m_Town.KingOfTheHillTimer != null && m_Town.KingOfTheHillTimer.Running)
            {
                writer.Write(true);
            }
            else
            {
                writer.Write(false);
            }
            // version 19
            // serialize hometown instead of homefaction
            Town.WriteReference(writer, m_Town);

            if (m_ControllingTown == null)
                Town.WriteReference(writer, m_Town);
            else
                Town.WriteReference(writer, m_ControllingTown);
            // moved and updated from version 11
            if (m_Town.KingOfTheHillTimer != null && m_Town.KingOfTheHillTimer.Running)
            {
                writer.Write(true);
                writer.Write(m_Town.KingOfTheHillTimer.TownCapturePoints.Count);
                foreach (KeyValuePair<Town, int> entry in m_Town.KingOfTheHillTimer.TownCapturePoints)
                {
                    Town.WriteReference(writer, entry.Key);
                    writer.Write(entry.Value);
                }
            }
            else
            {
                writer.Write(false);
            }

            // version 18
            // removed serialization of treasurychest

            // version 17
            // removed serialization of towncapturebanner. 

            //version 16
            writer.Write(m_Commander);

            //version 15
            writer.Write(ExilesDisabled);

            //version 14
            writer.Write(m_LastBombPlanPurchase);

            //version 13
            writer.Write(m_AllianceFlags.Count);
            foreach (AllianceFlag flag in m_AllianceFlags)
                writer.Write(flag);

            //version 12
            //writer.Write(m_CaptureBanner);

            // moved up changed as of 19
            //version 11

            //version 10
            writer.Write(m_AvailableBombPlans);

            //version 9
            writer.Write(m_LastGuardStateChange);

            //version 8
            writer.Write(m_LastExile);
            writer.Write(m_LastTheft);

            //version 7
            writer.Write(m_LastTreasuryWithdraw);

            //version 6
            m_WithdrawLog.Serialize(writer);

            //version 5
            writer.Write(m_LastUpkeep);

            //version 4
            if (m_MarketTiles == null)
                m_MarketTiles = new List<MarketFloor>();

            writer.Write(m_MarketTiles.Count);
            foreach (MarketFloor mf in MarketTiles)
                writer.WriteItem(mf);

            //version 3
            writer.Write(m_AllowNewCitizenshipBuffs);
            writer.Write((int)m_PrimaryCitizenshipBuff);

            //version 2
            writer.Write(m_TotalVoted);
            writer.Write(m_TownRevoltStone);

            writer.WriteEncodedInt(m_TownCriers.Count);
            foreach (TownCrier c in m_TownCriers)
                writer.Write(c);

            writer.WriteEncodedInt((int)m_LastBroadcasts.Length);

            for (int i = 0; i < m_LastBroadcasts.Length; ++i)
                writer.Write((DateTime)m_LastBroadcasts[i]);

            writer.Write((long)m_Treasury);
            writer.Write((double)m_SalesTax);
            writer.Write((double)m_PropertyTax);
            writer.Write((DateTime)m_LastTaxChange);

            writer.Write((DateTime)m_ControlStartTime);
            writer.Write((DateTime)m_LastSpawnCreatures);

            //Faction.WriteReference(writer, m_ControllingFaction);

            writer.Write((Mobile)m_King);

            //Members
            writer.WriteEncodedInt((int)m_Members.Count);
            for (int i = 0; i < m_Members.Count; ++i)
                m_Members[i].Serialize(writer);

            //Militia Members
            writer.WriteEncodedInt((int)m_MilitiaMembers.Count);
            for (int i = 0; i < m_MilitiaMembers.Count; ++i)
                writer.Write((Mobile)m_MilitiaMembers[i]);

            //Exiles
            writer.WriteEncodedInt((int)m_Exiles.Count);
            for (int i = 0; i < m_Exiles.Count; ++i)
                writer.Write((Mobile)m_Exiles[i]);

            //Crystals
            writer.WriteEncodedInt((int)m_Crystals.Count);
            for (int i = 0; i < m_Crystals.Count; ++i)
                writer.Write((TownCrystal)m_Crystals[i]);

            //Braziers
            writer.WriteEncodedInt((int)m_Braziers.Count);
            for (int i = 0; i < m_Braziers.Count; ++i)
                writer.Write((TownBrazier)m_Braziers[i]);

            //Flags
            writer.WriteEncodedInt((int)m_Flags.Count);
            for (int i = 0; i < m_Flags.Count; ++i)
                writer.Write((TownFlag)m_Flags[i]);

            //Spawned Creatures
            writer.WriteEncodedInt((int)m_SpawnedCreatures.Count);
            for (int i = 0; i < m_SpawnedCreatures.Count; ++i)
                writer.Write((BaseCreature)m_SpawnedCreatures[i]);

            //Walls
            writer.WriteEncodedInt((int)m_TreasuryWalls.Count);
            for (int i = 0; i < m_TreasuryWalls.Count; ++i)
                writer.Write((TreasuryWall)m_TreasuryWalls[i]);

            //Outcast Entries
            writer.WriteEncodedInt((int)m_OutcastEntries.Count);
            for (int i = 0; i < m_OutcastEntries.Count; ++i)
                m_OutcastEntries[i].Serialize(writer);


            writer.Write((bool)false); // UNUSED! legacy, tictactoe capture
            writer.Write((bool)false); // UNUSED! legacy, allow new decreed

            writer.Write((Item)m_KingStone);
            writer.Write((Item)m_MilitiaStone);

            writer.WriteEncodedInt((int)m_TreasuryWallType);
            writer.WriteEncodedInt((int)m_GuardState);

            writer.WriteItem((TownOutcastBoard)m_OutcastBoard);

            m_Election.Serialize(writer);

            Faction.WriteReference(writer, m_HomeFaction);

            m_Withdraws.Serialize(writer);
        }

    }
}