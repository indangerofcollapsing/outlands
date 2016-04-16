using System;
using System.Collections;
using System.Collections.Generic;
using Server.Guilds;
using Server.Multis;
using Server.Mobiles;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public class ShipTransportContract : Item
    {
        public static List<ShipTransportContract> m_Instances = new List<ShipTransportContract>();

        public static TimeSpan EasyDuration = TimeSpan.FromHours(3);
        public static TimeSpan ChallengingDuration = TimeSpan.FromHours(2);
        public static TimeSpan DangerousDuration = TimeSpan.FromHours(1);

        public static int EasyOceanEvents = 1;
        public static int ChallengingOceanEvents = 2;
        public static int DangerousOceanEvents = 3;

        public static int PickupProximity = 20;
        public static int DropOffProximity = 20;

        public static int OceanEventMinimumDistanceFromPickupLocation = 75;
        public static int OceanEventMinimumDistanceFromDestination = 75;

        public static TimeSpan OceanEventMinimumDelayBetweenEvents = TimeSpan.FromMinutes(5);

        public enum TransportContractType
        {
            DailyEasy,
            DailyChallenging,
            DailyDangerous
        }

        public enum LocationType
        {
            BritainDocks,
            CoveDocks,
            VesperDocks,
            TrinsicDocks,
            SkaraBraeDocks,
            YewDocks,
            MinocDocks,
            JhelomDocks,
            NujelmDocks,
            MoonglowDocks,
            MaginciaDocks,
            OccloDocks,
            SerpentsHoldDocks,
            BuccaneersDen
        }        

        public static string GetLocationName(LocationType locationType)
        {
            string locationName = "";

            switch (locationType)
            {
                case LocationType.BritainDocks: return "Britain"; break;
                case LocationType.CoveDocks: return "Cove"; break;
                case LocationType.VesperDocks: return "Vesper"; break;
                case LocationType.TrinsicDocks: return "Trinsic"; break;
                case LocationType.SkaraBraeDocks: return "Skara Brae"; break;
                case LocationType.YewDocks: return "Yew"; break;
                case LocationType.MinocDocks: return "Minoc"; break;
                case LocationType.JhelomDocks: return "Jhelom"; break;
                case LocationType.NujelmDocks: return "Nujel'm"; break;
                case LocationType.MoonglowDocks: return "MoonglowDocks"; break;
                case LocationType.MaginciaDocks: return "Magincia"; break;
                case LocationType.OccloDocks: return "Occlo"; break;
                case LocationType.SerpentsHoldDocks: return "Serpent's Hold"; break;
                case LocationType.BuccaneersDen: return "Buccaneer's Den"; break;
            }

            return locationName;
        }

        public static Point3D GetLocationPoint(LocationType locationType)
        {
            Point3D point = new Point3D();
 
            switch (locationType)
            {
                case LocationType.BritainDocks: return new Point3D(1500, 1500, 0); break;
                case LocationType.CoveDocks: return new Point3D(1500, 1500, 0); break;
                case LocationType.VesperDocks: return new Point3D(1500, 1500, 0); break;
                case LocationType.TrinsicDocks: return new Point3D(1500, 1500, 0); break;
                case LocationType.SkaraBraeDocks: return new Point3D(1500, 1500, 0); break;
                case LocationType.YewDocks: return new Point3D(1500, 1500, 0); break;
                case LocationType.MinocDocks: return new Point3D(1500, 1500, 0); break;
                case LocationType.JhelomDocks: return new Point3D(1500, 1500, 0); break;
                case LocationType.NujelmDocks: return new Point3D(1500, 1500, 0); break;
                case LocationType.MoonglowDocks: return new Point3D(1500, 1500, 0); break;
                case LocationType.MaginciaDocks: return new Point3D(1500, 1500, 0); break;
                case LocationType.OccloDocks: return new Point3D(1500, 1500, 0); break;
                case LocationType.SerpentsHoldDocks: return new Point3D(1500, 1500, 0); break;
                case LocationType.BuccaneersDen: return new Point3D(1500, 1500, 0); break;
            }

            return point;
        }

        private TransportContractType m_ContractType = TransportContractType.DailyEasy;
        [CommandProperty(AccessLevel.GameMaster)]
        public TransportContractType ContractType
        {
            get { return m_ContractType; }
            set 
            {
                m_ContractType = value;

                GuildDockUpgradeDetail upgradeDetail;

                switch (m_ContractType)
                {
                    case TransportContractType.DailyEasy:
                        upgradeDetail = GuildDockUpgrades.GetUpgradeDetail(GuildDockUpgradeType.DailyTransportContractEasy);
                        Hue = upgradeDetail.m_IconHue;

                        MaxOceanEvents = EasyOceanEvents;
                        ExpirationDate = DateTime.UtcNow + EasyDuration;
                    break;
                    case TransportContractType.DailyChallenging: 
                        upgradeDetail = GuildDockUpgrades.GetUpgradeDetail(GuildDockUpgradeType.DailyTransportContractChallenging);
                        Hue = upgradeDetail.m_IconHue;

                        MaxOceanEvents = ChallengingOceanEvents;
                        ExpirationDate = DateTime.UtcNow + ChallengingDuration;
                    break;
                    case TransportContractType.DailyDangerous: 
                        upgradeDetail = GuildDockUpgrades.GetUpgradeDetail(GuildDockUpgradeType.DailyTransportContractDangerous);
                        Hue = upgradeDetail.m_IconHue;

                        MaxOceanEvents = DangerousOceanEvents;
                        ExpirationDate = DateTime.UtcNow + DangerousDuration;
                    break;
                }
            }
        }  
      
        private LocationType m_PickupLocation;
        [CommandProperty(AccessLevel.GameMaster)]
        public LocationType PickupLocation
        {
            get { return m_PickupLocation; }
            set { m_PickupLocation = value; }
        }

        private LocationType m_Destination;
        [CommandProperty(AccessLevel.GameMaster)]
        public LocationType Destination
        {
            get { return m_Destination; }
            set { m_Destination = value; }
        }

        private PlayerMobile m_Creator;
        [CommandProperty(AccessLevel.GameMaster)]
        public PlayerMobile Creator
        {
            get { return m_Creator; }
            set { m_Creator = value; }
        }

        private Guild m_Guild;
        [CommandProperty(AccessLevel.GameMaster)]
        public Guild Guild
        {
            get { return m_Guild; }
            set { m_Guild = value; }
        }

        private DateTime m_CreationDate = DateTime.UtcNow;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime CreationDate
        {
            get { return m_CreationDate; }
            set { m_CreationDate = value; }
        }

        private DateTime m_ExpirationDate = DateTime.MaxValue;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime ExpirationDate
        {
            get { return m_ExpirationDate; }
            set { m_ExpirationDate = value; }
        }

        private TransportContractCargo m_Cargo;
        [CommandProperty(AccessLevel.GameMaster)]
        public TransportContractCargo Cargo
        {
            get { return m_Cargo; }
            set { m_Cargo = value; }
        }

        private bool m_MadePickup = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool MadePickup
        {
            get { return m_MadePickup; }
            set { m_MadePickup = value; }
        }

        private bool m_Resolved = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Resolved
        {
            get { return m_Resolved; }
            set
            {
                m_Resolved = value;

                if (m_Resolved)
                {
                    if (m_Instances.Contains(this))
                        m_Instances.Remove(this);
                }

                else
                {
                    if (!m_Instances.Contains(this))
                        m_Instances.Add(this);
                }            
            }
        }

        private int m_OceanEventsOccured = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int OceanEventsOccured
        {
            get { return m_OceanEventsOccured; }
            set { m_OceanEventsOccured = value; }
        }

        private int m_MaxOceanEvents = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxOceanEvents
        {
            get { return m_MaxOceanEvents; }
            set { m_MaxOceanEvents = value; }
        }

        private DateTime m_LastOceanEvent;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime LastOceanEvent
        {
            get { return m_LastOceanEvent; }
            set { m_LastOceanEvent = value; }
        }

        private Point3D m_LastOceanEventLocation;
        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D LastOceanEventLocation
        {
            get { return m_LastOceanEventLocation; }
            set { m_LastOceanEventLocation = value; }
        }

        private bool m_Successful = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Successful
        {
            get { return m_Successful; }
            set { m_Successful = value; }
        }

        [Constructable]
        public ShipTransportContract(TransportContractType contractType, PlayerMobile player, Guild guild): base(0x14F0)
        {
            Name = "transport contract";
            Weight = 1.0;

            ContractType = contractType;
            Creator = player;
            Guild = guild;
            CreationDate = DateTime.UtcNow;

            SetRoute();
            SetTimeout();            
        }

        public void SetRoute()
        {
            switch (ContractType)
            {
                #region Routes

                case TransportContractType.DailyEasy:
                    switch (Utility.RandomMinMax(1, 30))
                    {
                        case 1: PickupLocation = LocationType.JhelomDocks; Destination = LocationType.SerpentsHoldDocks; break;
                        case 2: PickupLocation = LocationType.SerpentsHoldDocks; Destination = LocationType.JhelomDocks; break;

                        case 3: PickupLocation = LocationType.SerpentsHoldDocks; Destination = LocationType.TrinsicDocks; break;
                        case 4: PickupLocation = LocationType.TrinsicDocks; Destination = LocationType.SerpentsHoldDocks; break;

                        case 5: PickupLocation = LocationType.SerpentsHoldDocks; Destination = LocationType.OccloDocks; break;
                        case 6: PickupLocation = LocationType.OccloDocks; Destination = LocationType.SerpentsHoldDocks; break;

                        case 7: PickupLocation = LocationType.TrinsicDocks; Destination = LocationType.BuccaneersDen; break;
                        case 8: PickupLocation = LocationType.BuccaneersDen; Destination = LocationType.TrinsicDocks; break;

                        case 9: PickupLocation = LocationType.OccloDocks; Destination = LocationType.MaginciaDocks; break;
                        case 10: PickupLocation = LocationType.MaginciaDocks; Destination = LocationType.OccloDocks; break;

                        case 11: PickupLocation = LocationType.OccloDocks; Destination = LocationType.BuccaneersDen; break;
                        case 12: PickupLocation = LocationType.BuccaneersDen; Destination = LocationType.OccloDocks; break;

                        case 13: PickupLocation = LocationType.OccloDocks; Destination = LocationType.MoonglowDocks; break;
                        case 14: PickupLocation = LocationType.MoonglowDocks; Destination = LocationType.OccloDocks; break;

                        case 15: PickupLocation = LocationType.MaginciaDocks; Destination = LocationType.BuccaneersDen; break;
                        case 16: PickupLocation = LocationType.BuccaneersDen; Destination = LocationType.MaginciaDocks; break;

                        case 17: PickupLocation = LocationType.MaginciaDocks; Destination = LocationType.NujelmDocks; break;
                        case 18: PickupLocation = LocationType.NujelmDocks; Destination = LocationType.MaginciaDocks; break;

                        case 19: PickupLocation = LocationType.MaginciaDocks; Destination = LocationType.MoonglowDocks; break;
                        case 20: PickupLocation = LocationType.MoonglowDocks; Destination = LocationType.MaginciaDocks; break;

                        case 21: PickupLocation = LocationType.MoonglowDocks; Destination = LocationType.BuccaneersDen; break;
                        case 22: PickupLocation = LocationType.BuccaneersDen; Destination = LocationType.MoonglowDocks; break;

                        case 23: PickupLocation = LocationType.MoonglowDocks; Destination = LocationType.NujelmDocks; break;
                        case 24: PickupLocation = LocationType.NujelmDocks; Destination = LocationType.MoonglowDocks; break;

                        case 25: PickupLocation = LocationType.NujelmDocks; Destination = LocationType.BuccaneersDen; break;
                        case 26: PickupLocation = LocationType.BuccaneersDen; Destination = LocationType.NujelmDocks; break;

                        case 27: PickupLocation = LocationType.NujelmDocks; Destination = LocationType.VesperDocks; break;
                        case 28: PickupLocation = LocationType.VesperDocks; Destination = LocationType.NujelmDocks; break;

                        case 29: PickupLocation = LocationType.BritainDocks; Destination = LocationType.CoveDocks; break;
                        case 30: PickupLocation = LocationType.CoveDocks; Destination = LocationType.BritainDocks; break;
                    }
                break;             

                case TransportContractType.DailyChallenging:
                    switch (Utility.RandomMinMax(1, 28))
                    {
                        case 1: PickupLocation = LocationType.YewDocks; Destination = LocationType.SkaraBraeDocks; break;
                        case 2: PickupLocation = LocationType.SkaraBraeDocks; Destination = LocationType.YewDocks; break;

                        case 3: PickupLocation = LocationType.SkaraBraeDocks; Destination = LocationType.JhelomDocks; break;
                        case 4: PickupLocation = LocationType.JhelomDocks; Destination = LocationType.SkaraBraeDocks; break;

                        case 5: PickupLocation = LocationType.JhelomDocks; Destination = LocationType.TrinsicDocks; break;
                        case 6: PickupLocation = LocationType.TrinsicDocks; Destination = LocationType.JhelomDocks; break;

                        case 7: PickupLocation = LocationType.JhelomDocks; Destination = LocationType.OccloDocks; break;
                        case 8: PickupLocation = LocationType.OccloDocks; Destination = LocationType.JhelomDocks; break;

                        case 9: PickupLocation = LocationType.SerpentsHoldDocks; Destination = LocationType.BritainDocks; break;
                        case 10: PickupLocation = LocationType.BritainDocks; Destination = LocationType.SerpentsHoldDocks; break;

                        case 11: PickupLocation = LocationType.SerpentsHoldDocks; Destination = LocationType.MoonglowDocks; break;
                        case 12: PickupLocation = LocationType.MoonglowDocks; Destination = LocationType.SerpentsHoldDocks; break;

                        case 13: PickupLocation = LocationType.OccloDocks; Destination = LocationType.BritainDocks; break;
                        case 14: PickupLocation = LocationType.BritainDocks; Destination = LocationType.OccloDocks; break;

                        case 15: PickupLocation = LocationType.OccloDocks; Destination = LocationType.VesperDocks; break;
                        case 16: PickupLocation = LocationType.VesperDocks; Destination = LocationType.OccloDocks; break;

                        case 17: PickupLocation = LocationType.TrinsicDocks; Destination = LocationType.CoveDocks; break;
                        case 18: PickupLocation = LocationType.CoveDocks; Destination = LocationType.TrinsicDocks; break;

                        case 19: PickupLocation = LocationType.TrinsicDocks; Destination = LocationType.MoonglowDocks; break;
                        case 20: PickupLocation = LocationType.MoonglowDocks; Destination = LocationType.TrinsicDocks; break;

                        case 21: PickupLocation = LocationType.BritainDocks; Destination = LocationType.MaginciaDocks; break;
                        case 22: PickupLocation = LocationType.MaginciaDocks; Destination = LocationType.BritainDocks; break;

                        case 23: PickupLocation = LocationType.BritainDocks; Destination = LocationType.VesperDocks; break;
                        case 24: PickupLocation = LocationType.VesperDocks; Destination = LocationType.BritainDocks; break;

                        case 25: PickupLocation = LocationType.CoveDocks; Destination = LocationType.NujelmDocks; break;
                        case 26: PickupLocation = LocationType.NujelmDocks; Destination = LocationType.CoveDocks; break;

                        case 27: PickupLocation = LocationType.MinocDocks; Destination = LocationType.VesperDocks; break;
                        case 28: PickupLocation = LocationType.VesperDocks; Destination = LocationType.MinocDocks; break;
                    }
                break;

                case TransportContractType.DailyDangerous:
                    switch (Utility.RandomMinMax(1, 26))
                    {
                        case 1: PickupLocation = LocationType.MinocDocks; Destination = LocationType.YewDocks; break;
                        case 2: PickupLocation = LocationType.YewDocks; Destination = LocationType.MinocDocks; break;

                        case 3: PickupLocation = LocationType.MinocDocks; Destination = LocationType.MoonglowDocks; break;
                        case 4: PickupLocation = LocationType.MoonglowDocks; Destination = LocationType.MinocDocks; break;

                        case 5: PickupLocation = LocationType.YewDocks; Destination = LocationType.JhelomDocks; break;
                        case 6: PickupLocation = LocationType.JhelomDocks; Destination = LocationType.YewDocks; break;

                        case 7: PickupLocation = LocationType.SkaraBraeDocks; Destination = LocationType.SerpentsHoldDocks; break;
                        case 8: PickupLocation = LocationType.SerpentsHoldDocks; Destination = LocationType.SkaraBraeDocks; break;

                        case 9: PickupLocation = LocationType.JhelomDocks; Destination = LocationType.BritainDocks; break;
                        case 10: PickupLocation = LocationType.BritainDocks; Destination = LocationType.JhelomDocks; break;

                        case 11: PickupLocation = LocationType.SerpentsHoldDocks; Destination = LocationType.VesperDocks; break;
                        case 12: PickupLocation = LocationType.VesperDocks; Destination = LocationType.SerpentsHoldDocks; break;

                        case 13: PickupLocation = LocationType.TrinsicDocks; Destination = LocationType.MoonglowDocks; break;
                        case 14: PickupLocation = LocationType.MoonglowDocks; Destination = LocationType.TrinsicDocks; break;

                        case 15: PickupLocation = LocationType.SkaraBraeDocks; Destination = LocationType.BuccaneersDen; break;
                        case 16: PickupLocation = LocationType.BuccaneersDen; Destination = LocationType.SkaraBraeDocks; break;

                        case 17: PickupLocation = LocationType.CoveDocks; Destination = LocationType.OccloDocks; break;
                        case 18: PickupLocation = LocationType.OccloDocks; Destination = LocationType.CoveDocks; break;

                        case 19: PickupLocation = LocationType.MinocDocks; Destination = LocationType.BuccaneersDen; break;
                        case 20: PickupLocation = LocationType.BuccaneersDen; Destination = LocationType.MinocDocks; break;

                        case 21: PickupLocation = LocationType.CoveDocks; Destination = LocationType.SerpentsHoldDocks; break;
                        case 22: PickupLocation = LocationType.SerpentsHoldDocks; Destination = LocationType.CoveDocks; break;

                        case 23: PickupLocation = LocationType.VesperDocks; Destination = LocationType.JhelomDocks; break;
                        case 24: PickupLocation = LocationType.JhelomDocks; Destination = LocationType.VesperDocks; break;

                        case 25: PickupLocation = LocationType.MoonglowDocks; Destination = LocationType.SerpentsHoldDocks; break;
                        case 26: PickupLocation = LocationType.SerpentsHoldDocks; Destination = LocationType.MoonglowDocks; break;
                    }
                break;

                #endregion
            }
        }       

        public ShipTransportContract(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            CheckExpiration();

            string contractType = "Daily Transport Contract: ";

            switch(m_ContractType)
            {
                case TransportContractType.DailyEasy: contractType = "Easy"; break;
                case TransportContractType.DailyChallenging: contractType = "Challenging"; break;
                case TransportContractType.DailyDangerous: contractType = "Dangerous"; break;
            }

            LabelTo(from, contractType);

            Point3D m_Start = ShipTransportContract.GetLocationPoint(m_PickupLocation);
            Point3D m_End = ShipTransportContract.GetLocationPoint(m_Destination);

            LabelTo(from, "[" + m_Start.X.ToString() + "," + m_Start.Y.ToString() + " to " + m_End.X.ToString() + "," + m_End.Y.ToString() + "]");

            if (m_Resolved)
            {
                if (m_Successful)
                    LabelTo(from, "(Completed)");

                else
                    LabelTo(from, "(Failed)");
            }

            else
            {
                string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, m_ExpirationDate, true, true, true, true, true);

                LabelTo(from, "(Expires in " + timeRemaining + ")");
            }            
        }

        public override void OnDoubleClick(Mobile from)
        {
            CheckExpiration();

            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;
            
            player.SendGump(new ShipTransportContractGump(player, this));
        }

        public override void OnLocationChange(Point3D oldLocation)
        {
            base.OnLocationChange(oldLocation);

            CheckExpiration();

            if (Resolved)
                return;

            Point3D pickupLocation = GetLocationPoint(PickupLocation);            

            if (!MadePickup)
            {
                if (Utility.GetDistance(Location, pickupLocation) <= ShipTransportContract.PickupProximity)
                {
                    BaseBoat boat = BaseBoat.FindBoatAt(Location, Map);

                    if (boat != null)
                    {
                        MadePickup = true;

                        List<Mobile> boatMobiles = boat.GetMobilesOnBoat(true, true);

                        foreach (Mobile mobile in boatMobiles)
                        {
                            mobile.SendMessage("You pickup cargo from a transport contract and place it in your ship's hold.");
                        }
                    }
                }

                return;
            }
        }

        public void SetTimeout()
        {
            if (!m_Resolved)
            {
                if (DateTime.UtcNow >= m_ExpirationDate)
                {
                    if (Creator != null)
                        Creator.SendMessage("One of your transport contracts has expired.");

                    Resolved = true;
                    Successful = false;                    
                }

                else
                {
                    if (!m_Instances.Contains(this))
                        m_Instances.Add(this);

                    double timeoutSeconds = (m_ExpirationDate - DateTime.UtcNow).TotalSeconds;

                    Timer.DelayCall(TimeSpan.FromSeconds(timeoutSeconds), delegate
                    {
                        if (this == null) return;
                        if (Deleted) return;

                        if (!m_Resolved)
                        {
                            if (Creator != null)
                                Creator.SendMessage("One of your transport contracts has expired.");

                            Resolved = true;
                            Successful = false;
                        }
                    });
                }
            }
        }

        public void CheckExpiration()
        {
            if (!m_Resolved)
            {
                if (DateTime.UtcNow >= m_ExpirationDate)
                {
                    Resolved = true;
                    Successful = false;
                }
            }
        }

        public override void OnDelete()
        {
            if (m_Instances.Contains(this))
                m_Instances.Remove(this);

            if (!m_Resolved)
            {
                if (Creator != null)
                    Creator.SendMessage("You abandon a transport contract in progress.");
            }

            if (m_Cargo != null)
                m_Cargo.Delete();

            base.OnDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            //Version 0
            writer.Write((int)m_ContractType);
            writer.Write((int)m_PickupLocation);
            writer.Write((int)m_Destination);
            writer.Write(m_Creator);
            writer.Write(m_Guild);
            writer.Write(m_CreationDate);
            writer.Write(m_ExpirationDate);
            writer.Write(m_Cargo);
            writer.Write(m_MadePickup);
            writer.Write(m_Resolved);
            writer.Write(m_OceanEventsOccured);
            writer.Write(m_MaxOceanEvents);
            writer.Write(m_LastOceanEvent);
            writer.Write(m_LastOceanEventLocation);
            writer.Write(m_Successful);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_ContractType = (TransportContractType)reader.ReadInt();
               
                m_PickupLocation = (LocationType)reader.ReadInt();
                m_Destination = (LocationType)reader.ReadInt();
                m_Creator = (PlayerMobile)reader.ReadMobile();
                m_Guild = (Guild)reader.ReadGuild();
                m_CreationDate = reader.ReadDateTime();
                m_ExpirationDate = reader.ReadDateTime();
                m_Cargo = (TransportContractCargo)reader.ReadItem();
                m_MadePickup = reader.ReadBool();
                m_Resolved = reader.ReadBool();
                m_OceanEventsOccured = reader.ReadInt();
                m_MaxOceanEvents = reader.ReadInt();
                m_LastOceanEvent = reader.ReadDateTime();
                m_LastOceanEventLocation = reader.ReadPoint3D();
                m_Successful = reader.ReadBool();
            }

            //-----

            SetTimeout();
        }
    }

    public class ShipTransportContractGump : Gump
    {
        PlayerMobile m_Player;
        ShipTransportContract m_TransportContract;

        public ShipTransportContractGump(PlayerMobile player, ShipTransportContract transportContract): base(50, 50)
        {
            if (player == null || transportContract == null)
                return;

            m_Player = player;
            m_TransportContract = transportContract;           

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddImage(0, -1, 1249);

            int WhiteTextHue = 2655;
            int GreenTextHue = 0x3F;
            int RedTextHue = 2115;

            string contractName = "Daily Transport Contract: ";

            GuildDockUpgradeType upgradeType = GuildDockUpgradeType.DailyTransportContractEasy;
            GuildDockUpgradeDetail upgradeDetail = GuildDockUpgrades.GetUpgradeDetail(upgradeType);

            switch (m_TransportContract.ContractType)
            {
                case ShipTransportContract.TransportContractType.DailyEasy:
                    upgradeType = GuildDockUpgradeType.DailyTransportContractEasy;
                    contractName += "Easy"; 
                break;

                case ShipTransportContract.TransportContractType.DailyChallenging:
                    upgradeType = GuildDockUpgradeType.DailyTransportContractChallenging;
                    contractName += "Challenging";
                break;
                case ShipTransportContract.TransportContractType.DailyDangerous:
                    upgradeType = GuildDockUpgradeType.DailyTransportContractDangerous;
                    contractName += "Dangerous";
                break;
            }

            string guildName = "";

            if (transportContract.Guild != null)
                guildName = transportContract.Guild.Name + " (" + transportContract.Guild.Abbreviation + ")";            

            AddLabel(108, 60, WhiteTextHue, contractName);
            AddLabel(129, 80, 2599, guildName);

            AddItem(169 + upgradeDetail.m_IconOffsetX, 108 + upgradeDetail.m_IconOffsetY, upgradeDetail.m_IconItemID, upgradeDetail.m_IconHue);

            if (transportContract.Resolved)
            {
                if (transportContract.Successful)
                    AddLabel(54, 216, GreenTextHue, "Completed");

                else
                    AddLabel(54, 216, RedTextHue, "Failed");
            }

            else
            {
                string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, transportContract.ExpirationDate, true, true, true, true, false);

                AddLabel(54, 216, 2603, "Time Remaining");
                AddLabel(156, 216, WhiteTextHue, timeRemaining);
            }

            string originText = "";
            string originName = ShipTransportContract.GetLocationName(transportContract.PickupLocation);
            Point3D originPoint = ShipTransportContract.GetLocationPoint(transportContract.PickupLocation);

            originText = originName + "(" + originPoint.X + "," + originPoint.Y + ")";
            
            string destinationText = "";
            string destinationName = ShipTransportContract.GetLocationName(transportContract.Destination);
            Point3D destinationPoint = ShipTransportContract.GetLocationPoint(transportContract.Destination);

            originText = destinationName + "(" + destinationPoint.X + "," + destinationPoint.Y + ")";

            AddLabel(110, 236, 2603, "Origin");
            AddLabel(156, 236, WhiteTextHue, originText);

            AddLabel(74, 256, 2603, "Destination");
            AddLabel(156, 256, WhiteTextHue, destinationText);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            switch (info.ButtonID)
            {                
            }
        }
    }
}