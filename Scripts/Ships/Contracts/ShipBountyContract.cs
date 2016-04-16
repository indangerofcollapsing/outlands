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
    public class ShipBountyContract : Item
    {
        public static List<ShipBountyContract> m_Instances = new List<ShipBountyContract>();
        
        public enum BountyContractType
        {
            DailyEasy,
            DailyChallenging,
            DailyDangerous
        }

        public static Point3D[] SpawnLocations = new Point3D[]
        {
            //Ocean Spawns
            new Point3D(2763, 1532, 0),
            new Point3D(2951, 2533, 0),
            new Point3D(2812, 2173, 0),
            new Point3D(4059, 655, 0),
            new Point3D(4144, 275, 0),
            new Point3D(2195, 1360, 0),
            new Point3D(4725, 380, 0),
            new Point3D(4561, 2351, 0),
            new Point3D(4300, 3500, 0),
            new Point3D(500, 3100, 0),
            new Point3D(50, 1600, 0),
            new Point3D(2350, 4000, 0),
            new Point3D(3900, 1600, 0),

            //Fishing Boat Spawns
            new Point3D(1873, 2027, 0),
            new Point3D(2118, 2418, 0),
            new Point3D(2197, 3245, 0),
            new Point3D(2152, 3698, 0),
            new Point3D(1362, 3364, 0),
            new Point3D(853, 2681, 0),
            new Point3D(560, 1812, 0),
            new Point3D(93, 1430, 0),
            new Point3D(750, 470, 0),
            new Point3D(1393, 479, 0),
            new Point3D(2223, 165, 0),
            new Point3D(2951, 235, 0),
            new Point3D(3413, 576, 0),
            new Point3D(2637, 1289, 0),
        };

        private BountyContractType m_ContractType = BountyContractType.DailyEasy;
        [CommandProperty(AccessLevel.GameMaster)]
        public BountyContractType ContractType
        {
            get { return m_ContractType; }
            set 
            {
                m_ContractType = value;

                GuildDockUpgradeDetail upgradeDetail;

                switch (m_ContractType)
                {
                    case BountyContractType.DailyEasy:
                        upgradeDetail = GuildDockUpgrades.GetUpgradeDetail(GuildDockUpgradeType.DailyBountyContractEasy);
                        Hue = upgradeDetail.m_IconHue;                       
                    break;
                    case BountyContractType.DailyChallenging: 
                        upgradeDetail = GuildDockUpgrades.GetUpgradeDetail(GuildDockUpgradeType.DailyBountyContractChallenging);
                        Hue = upgradeDetail.m_IconHue;
                    break;
                    case BountyContractType.DailyDangerous: 
                        upgradeDetail = GuildDockUpgrades.GetUpgradeDetail(GuildDockUpgradeType.DailyBountyContractDangerous);
                        Hue = upgradeDetail.m_IconHue;
                    break;
                }
            }
        }

        private Point3D m_Destination;
        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D Destination
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

        private BaseBoat m_Boat;
        [CommandProperty(AccessLevel.GameMaster)]
        public BaseBoat Boat
        {
            get { return m_Boat; }
            set { m_Boat = value; }
        }

        private bool m_Completed = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Completed
        {
            get { return m_Completed; }
            set
            {
                m_Completed = value;

                if (m_Completed)
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

        [Constructable]
        public ShipBountyContract(BountyContractType contractType, PlayerMobile player, Guild guild): base(0x14F0)
        {
            Name = "ship bounty contract";
            Weight = 1.0;

            ContractType = contractType;
            Creator = player;
            Guild = guild;
            CreationDate = DateTime.UtcNow;           

            CreateBoat();

            if (!m_Completed)
                m_Instances.Add(this);
        }

        public ShipBountyContract(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            string contractType = "Daily Bounty Contract: ";

            switch(m_ContractType)
            {
                case BountyContractType.DailyEasy: contractType = "Easy"; break;
                case BountyContractType.DailyChallenging: contractType = "Challenging"; break;
                case BountyContractType.DailyDangerous: contractType = "Dangerous"; break;
            }

            LabelTo(from, contractType);

            LabelTo(from, "[at " + m_Destination.X.ToString() + "," + m_Destination.Y.ToString() + "]");

            if (m_Completed)
                LabelTo(from, "(Completed)");          
        }

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            player.CloseGump(typeof(ShipBountyContract));
            player.SendGump(new ShipBountyContractGump(player, this));
        }

        public void CreateBoat()
        {
            List<Point3D> m_PossibleLocations = new List<Point3D>();

            foreach (Point3D point in SpawnLocations)
            {
                m_PossibleLocations.Add(point);
            }

            int locationChecks = m_PossibleLocations.Count;

            bool shipSpawned = false;

            int spawnRange = 10;

            BaseBoat boat = null;

            switch (m_ContractType)
            {
                case BountyContractType.DailyEasy:
                    boat = new CarrackBoat();
                break;

                case BountyContractType.DailyChallenging:
                    boat = new CarrackBoat();
                break;

                case BountyContractType.DailyDangerous:
                    boat = new CarrackBoat();
                break;
            }

            if (boat == null)
                return;

            for (int a = 0; a < locationChecks; a++)
            {
                Point3D locationToCheck = m_PossibleLocations[Utility.RandomMinMax(0, m_PossibleLocations.Count - 1)];

                //Check Count
                for (int b = 0; b < 25; b++)
                {
                    if (shipSpawned)
                        break;

                    Point3D newLocation = new Point3D();

                    int x = locationToCheck.X;

                    int xOffset = Utility.RandomMinMax(0, spawnRange);
                    if (Utility.RandomDouble() >= .5)
                        xOffset *= -1;

                    x += xOffset;

                    int y = locationToCheck.Y;

                    int yOffset = Utility.RandomMinMax(0, spawnRange);
                    if (Utility.RandomDouble() >= .5)
                        yOffset *= -1;

                    y += yOffset;

                    newLocation.X = x;
                    newLocation.Y = y;
                    newLocation.Z = locationToCheck.Z;

                    Direction newDirection = Direction.North;
                    int shipFacingItemID = -1;
                    
                    int randomDirection = Utility.RandomList(1, 2, 3, 4);

                    switch (randomDirection)
                    {
                        case 1:
                            newDirection = Direction.North;
                            shipFacingItemID = boat.NorthID;
                        break;

                        case 2:
                            newDirection = Direction.East;
                            shipFacingItemID = boat.EastID;
                        break;

                        case 3:
                            newDirection = Direction.South;
                            shipFacingItemID = boat.SouthID;
                        break;

                        case 4:
                            newDirection = Direction.West;
                            shipFacingItemID = boat.WestID;
                        break;
                    }

                    if (boat.CanFit(newLocation, Map, shipFacingItemID))
                    {
                        boat.MoveToWorld(newLocation, Map);
                        boat.m_ShipSpawner = null;

                        shipSpawned = true;

                        Timer.DelayCall(TimeSpan.FromSeconds(.200), delegate
                        {
                            boat.SetFacing(newDirection);
                        });                        
                    }
                }

                if (shipSpawned)
                {
                    m_Boat = boat;
                    m_Destination = boat.Location;

                    m_Creator.SendMessage("You have received a bounty contract on a ship located at " + m_Destination.ToString() + ".");

                    return;
                }

                else
                    m_PossibleLocations.Remove(locationToCheck);
            }

            if (!shipSpawned)
                boat.Delete();
        }

        public override void OnDelete()
        {
            if (m_Instances.Contains(this))
                m_Instances.Remove(this);

            base.OnDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            //Version 0
            writer.Write((int)m_ContractType);
            writer.Write(m_Destination);
            writer.Write(m_Creator);
            writer.Write(m_Guild);
            writer.Write(m_CreationDate);
            writer.Write(m_ExpirationDate);
            writer.Write(m_Completed);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_ContractType = (BountyContractType)reader.ReadInt();
                m_Destination = reader.ReadPoint3D();
                m_Creator = (PlayerMobile)reader.ReadMobile();
                m_Guild = (Guild)reader.ReadGuild();
                m_CreationDate = reader.ReadDateTime();
                m_ExpirationDate = reader.ReadDateTime();
                m_Completed = reader.ReadBool();
            }

            //-----

            if (!m_Completed)
                m_Instances.Add(this);
        }
    }

    public class ShipBountyContractGump : Gump
    {
        PlayerMobile m_Player;
        ShipBountyContract m_BountyContract;

        public ShipBountyContractGump(PlayerMobile player, ShipBountyContract bountyContract): base(50, 50)
        {
            if (player == null || bountyContract == null)
                return;

            m_Player = player;
            m_BountyContract = bountyContract;           

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddImage(0, -1, 1249);

            int WhiteTextHue = 2655;
            int GreenTextHue = 0x3F;

            string contractName = "Daily Bounty Contract: ";

            GuildDockUpgradeType upgradeType = GuildDockUpgradeType.DailyTransportContractEasy;
            GuildDockUpgradeDetail upgradeDetail = GuildDockUpgrades.GetUpgradeDetail(upgradeType);

            switch (m_BountyContract.ContractType)
            {
                case ShipBountyContract.BountyContractType.DailyEasy:
                    upgradeType = GuildDockUpgradeType.DailyBountyContractEasy;
                    contractName += "Easy"; 
                break;
                case ShipBountyContract.BountyContractType.DailyChallenging:
                    upgradeType = GuildDockUpgradeType.DailyBountyContractChallenging;
                    contractName += "Challenging";
                break;
                case ShipBountyContract.BountyContractType.DailyDangerous:
                    upgradeType = GuildDockUpgradeType.DailyBountyContractDangerous;
                    contractName += "Dangerous";
                break;
            }

            string guildName = "";

            if (bountyContract.Guild != null)            
                guildName = bountyContract.Guild.Name + " (" + bountyContract.Guild.Abbreviation + ")";            

            AddLabel(108, 60, WhiteTextHue, contractName);
            AddLabel(129, 80, 2599, guildName);

            AddItem(169 + upgradeDetail.m_IconOffsetX, 108 + upgradeDetail.m_IconOffsetY, upgradeDetail.m_IconItemID, upgradeDetail.m_IconHue);

            if (bountyContract.Completed)
                AddLabel(91, 237, GreenTextHue, "Completed");

            AddLabel(132, 257, 2603, "Location");
            AddLabel(195, 257, WhiteTextHue, bountyContract.Destination.X + "," + bountyContract.Destination.Y);           
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            switch (info.ButtonID)
            {                
            }
        }
    }
}