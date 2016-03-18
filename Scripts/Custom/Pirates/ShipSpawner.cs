using System;
using Server;
using System.Collections;
using System.Collections.Generic;
using Server.Network;
using Server.Prompts;
using Server.Multis;
using Server.Mobiles;
using Server.Targeting;
using Server.Gumps;

namespace Server.Items
{
    public class ShipSpawner : Item
    {
        private bool m_Activated = true;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Activated
        {
            get { return m_Activated; }
            set { m_Activated = value; }
        }

        private string m_ShipTypes = "";
        [CommandProperty(AccessLevel.GameMaster)]
        public string ShipTypes
        {
            get { return m_ShipTypes; }
            set { m_ShipTypes = value; }
        }

        private int m_ShipCount = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int ShipCount
        {
            get { return m_ShipCount; }
            set { m_ShipCount = value; }
        }

        private bool m_SpawnAllAvailable = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool SpawnAllAvailable
        {
            get { return m_SpawnAllAvailable; }
            set { m_SpawnAllAvailable = value; }
        }

        private int m_HomeRange = 10;
        [CommandProperty(AccessLevel.GameMaster)]
        public int HomeRange
        {
            get { return m_HomeRange; }
            set { m_HomeRange = value; }
        }

        private int m_SpawnRange = 10;
        [CommandProperty(AccessLevel.GameMaster)]
        public int SpawnRange
        {
            get { return m_SpawnRange; }
            set { m_SpawnRange = value; }
        }

        private int m_MinSpawnTime = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int MinSpawnTime
        {
            get { return m_MinSpawnTime; }
            set { m_MinSpawnTime = value; }
        }

        private int m_MaxSpawnTime = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxSpawnTime
        {
            get { return m_MaxSpawnTime; }
            set { m_MaxSpawnTime = value; }
        }

        private Direction m_PreferredDirection = Direction.North;
        [CommandProperty(AccessLevel.GameMaster)]
        public Direction PreferredDirection
        {
            get { return m_PreferredDirection; }
            set { m_PreferredDirection = value; }
        }

        public List<BaseBoat> m_Boats = new List<BaseBoat>();
        public DateTime m_LastActivity = DateTime.UtcNow;
        public TimeSpan m_NextActivity;

        private Timer m_SpawnTimer;

        [Constructable]
        public ShipSpawner(): base(0x14F4)
        {
            Name = "Ship Spawner";

            Visible = false;
            Movable = false;

            m_NextActivity = TimeSpan.FromMinutes(Utility.RandomMinMax(m_MinSpawnTime, m_MaxSpawnTime));

            m_SpawnTimer = new SpawnTimer(this);
            m_SpawnTimer.Start();
        }

        public ShipSpawner(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, "Ships Active: " + m_Boats.Count.ToString() + " / " + m_ShipCount.ToString());
            LabelTo(from, "[Double Click to Delete All Ships]");
        }

        public override void OnDoubleClick(Mobile from)
        {
            int boatCount = m_Boats.Count;

            for (int a = 0; a < boatCount; a++)
            {
                if (m_Boats[0] != null)
                    m_Boats[0].Delete();
            }
        }

        public void StartSpawn(Mobile from)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); //version

            //Version 0
            writer.Write(m_Activated);
            writer.Write(m_ShipTypes);
            writer.Write(m_ShipCount);
            writer.Write(m_SpawnAllAvailable);
            writer.Write(m_HomeRange);
            writer.Write(m_SpawnRange);
            writer.Write(m_MinSpawnTime);
            writer.Write(m_MaxSpawnTime);

            writer.Write(m_Boats.Count);

            foreach (BaseBoat boat in m_Boats)
            {
                writer.Write(boat);
            }

            writer.Write(m_LastActivity);
            writer.Write(m_NextActivity);

            //Version 1
            writer.Write((int)PreferredDirection);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            m_Boats = new List<BaseBoat>();

            m_Activated = reader.ReadBool();
            m_ShipTypes = reader.ReadString();
            m_ShipCount = reader.ReadInt();
            m_SpawnAllAvailable = reader.ReadBool();
            m_HomeRange = reader.ReadInt();
            m_SpawnRange = reader.ReadInt();
            m_MinSpawnTime = reader.ReadInt();
            m_MaxSpawnTime = reader.ReadInt();

            int boatCount = reader.ReadInt();
            for (int a = 0; a < boatCount; a++)
            {
                BaseBoat boat = (BaseBoat)reader.ReadItem();
                m_Boats.Add(boat);
            }

            m_LastActivity = reader.ReadDateTime();
            m_NextActivity = reader.ReadTimeSpan();

            if (m_SpawnTimer == null)
            {
                m_SpawnTimer = new SpawnTimer(this);
                m_SpawnTimer.Start();
            }

            else if (!m_SpawnTimer.Running)
                m_SpawnTimer.Start();

            //Version 1
            if (version >= 1)
            {
                m_PreferredDirection = (Direction)reader.ReadInt();
            }
        }

        public static BaseBoat RandomizeBoat(Dictionary<Type, int> boatChances)
        {
            BaseBoat boat = null;

            int TotalValues = 0;

            foreach (KeyValuePair<Type, int> pair in boatChances)
            {
                TotalValues += pair.Value;
            }

            double ActionCheck = Utility.RandomDouble();
            double CumulativeAmount = 0.0;
            double AdditionalAmount = 0.0;

            bool foundDirection = true;

            foreach (KeyValuePair<Type, int> pair in boatChances)
            {
                AdditionalAmount = (double)pair.Value / (double)TotalValues;

                if (ActionCheck >= CumulativeAmount && ActionCheck < (CumulativeAmount + AdditionalAmount))
                {
                    boat = (BaseBoat)Activator.CreateInstance(pair.Key);
                    break;
                }

                CumulativeAmount += AdditionalAmount;
            }  

            return boat;
        }

        public static BaseBoat GetRandomBoatType(string boatString)
        {
            BaseBoat boat = null;

            int randomBoat;

            string shipString = boatString.Trim();
            boatString = shipString.ToLower();

            string[] boatTypes = boatString.Split(',');

            string newBoat = boatTypes[Utility.RandomMinMax(0, boatTypes.Length - 1)];

            Dictionary<Type, int> DictBoatOptions = new Dictionary<Type, int>();

            switch (newBoat)
            {
                case "smallbritainnavyboat": boat = new SmallBritainNavyBoat(); break;
                case "mediumbritainnavyboat": boat = new MediumBritainNavyBoat(); break;
                case "largebritainnavyboat": boat = new LargeBritainNavyBoat(); break;
                case "carrackbritainnavyboat": boat = new CarrackBritainNavyBoat(); break;
                case "galleonbritainnavyboat": boat = new GalleonBritainNavyBoat(); break;

                case "smallfishingboat": boat = new SmallFishingBoat(); break;
                case "mediumfishingboat": boat = new MediumFishingBoat(); break;
                case "largefishingboat": boat = new LargeFishingBoat(); break;
                case "carrackfishingboat": boat = new CarrackFishingBoat(); break;
                case "galleonfishingboat": boat = new GalleonFishingBoat(); break;

                case "smallpirateboat": boat = new SmallPirateBoat(); break;
                case "mediumpirateboat": boat = new MediumPirateBoat(); break;
                case "largepirateboat": boat = new LargePirateBoat(); break;
                case "carrackpirateboat": boat = new CarrackPirateBoat(); break;
                case "galleonpirateboat": boat = new GalleonPirateBoat(); break;

                case "smallghostshipboat": boat = new SmallGhostShipBoat(); break;
                case "mediumghostshipboat": boat = new MediumGhostShipBoat(); break;
                case "largeghostshipboat": boat = new LargeGhostShipBoat(); break;
                case "carrackghostshipboat": boat = new CarrackGhostShipBoat(); break;
                case "galleonghostshipboat": boat = new GalleonGhostShipBoat(); break;

                case "smallorghereimboat": boat = new SmallOrghereimBoat(); break;
                case "mediumorghereimboat": boat = new MediumOrghereimBoat(); break;
                case "largeorghereimboat": boat = new LargeOrghereimBoat(); break;
                case "carrackorghereimboat": boat = new CarrackOrghereimBoat(); break;
                case "galleonorghereimboat": boat = new GalleonOrghereimBoat(); break;

                case "smallorcboat": boat = new SmallOrcBoat(); break;
                case "mediumorcboat": boat = new MediumOrcBoat(); break;
                case "largeorcboat": boat = new LargeOrcBoat(); break;
                case "carrackorcboat": boat = new CarrackOrcBoat(); break;
                case "galleonorcboat": boat = new GalleonOrcBoat(); break;

                case "anybritainboat":

                    DictBoatOptions.Add(typeof(SmallBritainNavyBoat), 5);
                    DictBoatOptions.Add(typeof(MediumBritainNavyBoat), 4);
                    DictBoatOptions.Add(typeof(LargeBritainNavyBoat), 3);
                    DictBoatOptions.Add(typeof(CarrackBritainNavyBoat), 2);
                    DictBoatOptions.Add(typeof(GalleonBritainNavyBoat), 1);

                    boat = RandomizeBoat(DictBoatOptions);
                break;

                case "anyfishingboat":

                    DictBoatOptions.Add(typeof(SmallFishingBoat), 5);
                    DictBoatOptions.Add(typeof(MediumFishingBoat), 4);
                    DictBoatOptions.Add(typeof(LargeFishingBoat), 3);
                    DictBoatOptions.Add(typeof(CarrackFishingBoat), 2);
                    DictBoatOptions.Add(typeof(GalleonFishingBoat), 1);

                    boat = RandomizeBoat(DictBoatOptions);
                break;

                case "anypirateboat":

                    DictBoatOptions.Add(typeof(SmallPirateBoat), 5);
                    DictBoatOptions.Add(typeof(MediumPirateBoat), 4);
                    DictBoatOptions.Add(typeof(LargePirateBoat), 3);
                    DictBoatOptions.Add(typeof(CarrackPirateBoat), 2);
                    DictBoatOptions.Add(typeof(GalleonPirateBoat), 1);

                    boat = RandomizeBoat(DictBoatOptions);
                break;

                case "anyghostshipboat":

                    DictBoatOptions.Add(typeof(SmallGhostShipBoat), 5);
                    DictBoatOptions.Add(typeof(MediumGhostShipBoat), 4);
                    DictBoatOptions.Add(typeof(LargeGhostShipBoat), 3);
                    DictBoatOptions.Add(typeof(CarrackGhostShipBoat), 2);
                    DictBoatOptions.Add(typeof(GalleonGhostShipBoat), 1);

                    boat = RandomizeBoat(DictBoatOptions);
                break;

                case "anyorghereimboat":

                    DictBoatOptions.Add(typeof(SmallOrghereimBoat), 5);
                    DictBoatOptions.Add(typeof(MediumOrghereimBoat), 4);
                    DictBoatOptions.Add(typeof(LargeOrghereimBoat), 3);
                    DictBoatOptions.Add(typeof(CarrackOrghereimBoat), 2);
                    DictBoatOptions.Add(typeof(GalleonOrghereimBoat), 1);

                    boat = RandomizeBoat(DictBoatOptions);
                break;

                case "anyorcboat":

                    DictBoatOptions.Add(typeof(SmallOrcBoat), 5);
                    DictBoatOptions.Add(typeof(MediumOrcBoat), 4);
                    DictBoatOptions.Add(typeof(LargeOrcBoat), 3);
                    DictBoatOptions.Add(typeof(CarrackOrcBoat), 2);
                    DictBoatOptions.Add(typeof(GalleonOrcBoat), 1);

                    boat = RandomizeBoat(DictBoatOptions);
                break;

                case "anyship":

                    DictBoatOptions.Add(typeof(SmallBritainNavyBoat), 5);
                    DictBoatOptions.Add(typeof(MediumBritainNavyBoat), 4);
                    DictBoatOptions.Add(typeof(LargeBritainNavyBoat), 3);
                    DictBoatOptions.Add(typeof(CarrackBritainNavyBoat), 2);
                    DictBoatOptions.Add(typeof(GalleonBritainNavyBoat), 1);

                    DictBoatOptions.Add(typeof(SmallFishingBoat), 5);
                    DictBoatOptions.Add(typeof(MediumFishingBoat), 4);
                    DictBoatOptions.Add(typeof(LargeFishingBoat), 3);
                    DictBoatOptions.Add(typeof(CarrackFishingBoat), 2);
                    DictBoatOptions.Add(typeof(GalleonFishingBoat), 1);

                    DictBoatOptions.Add(typeof(SmallPirateBoat), 5);
                    DictBoatOptions.Add(typeof(MediumPirateBoat), 4);
                    DictBoatOptions.Add(typeof(LargePirateBoat), 3);
                    DictBoatOptions.Add(typeof(CarrackPirateBoat), 2);
                    DictBoatOptions.Add(typeof(GalleonPirateBoat), 1);

                    DictBoatOptions.Add(typeof(SmallGhostShipBoat), 5);
                    DictBoatOptions.Add(typeof(MediumGhostShipBoat), 4);
                    DictBoatOptions.Add(typeof(LargeGhostShipBoat), 3);
                    DictBoatOptions.Add(typeof(CarrackGhostShipBoat), 2);
                    DictBoatOptions.Add(typeof(GalleonGhostShipBoat), 1);

                    DictBoatOptions.Add(typeof(SmallOrghereimBoat), 5);
                    DictBoatOptions.Add(typeof(MediumOrghereimBoat), 4);
                    DictBoatOptions.Add(typeof(LargeOrghereimBoat), 3);
                    DictBoatOptions.Add(typeof(CarrackOrghereimBoat), 2);
                    DictBoatOptions.Add(typeof(GalleonOrghereimBoat), 1);

                    DictBoatOptions.Add(typeof(SmallOrcBoat), 5);
                    DictBoatOptions.Add(typeof(MediumOrcBoat), 4);
                    DictBoatOptions.Add(typeof(LargeOrcBoat), 3);
                    DictBoatOptions.Add(typeof(CarrackOrcBoat), 2);
                    DictBoatOptions.Add(typeof(GalleonOrcBoat), 1);

                    boat = RandomizeBoat(DictBoatOptions);

                break;
            }

            return boat;
        }

        public override void OnAfterDelete()
        {           
            int boatCount = m_Boats.Count;

            for (int a = 0; a < boatCount; a++)
            {
                m_Boats[0].Delete();
            }

            base.OnAfterDelete();            
        }

        public void ShipSunk(BaseBoat boat)
        {
            m_Boats.Remove(boat);

            foreach (BaseBoat checkboat in BaseBoat.AllBoatInstances)
            {
                if (checkboat != null)
                {
                    if (checkboat.BoatCombatant != null)
                    {
                        if (checkboat.BoatCombatant == boat)
                            checkboat.BoatCombatant = null;
                    }
                }
            }

            m_LastActivity = DateTime.UtcNow;
            m_NextActivity = TimeSpan.FromMinutes(Utility.RandomMinMax(m_MinSpawnTime, m_MaxSpawnTime));
        }

        private class SpawnTimer : Timer
        {
            public ShipSpawner m_ShipSpawner;

            public SpawnTimer(ShipSpawner shipSpawner): base(TimeSpan.Zero, TimeSpan.FromSeconds(10))
            {
                m_ShipSpawner = shipSpawner;
                Priority = TimerPriority.FiveSeconds;
            }

            protected override void OnTick()
            {
                if (!m_ShipSpawner.Activated)
                    return;

                DateTime nextSpawnTime = m_ShipSpawner.m_LastActivity + m_ShipSpawner.m_NextActivity;

                if (nextSpawnTime < DateTime.UtcNow)
                {
                    if (m_ShipSpawner.m_Boats.Count < m_ShipSpawner.ShipCount)
                    {
                        int shipsNeeded = m_ShipSpawner.ShipCount - m_ShipSpawner.m_Boats.Count;
                        int shipsSpawned = 0;

                        if (!m_ShipSpawner.SpawnAllAvailable)
                            shipsNeeded = 1;

                        m_ShipSpawner.m_LastActivity = DateTime.UtcNow;
                        m_ShipSpawner.m_NextActivity = TimeSpan.FromMinutes(Utility.RandomMinMax(m_ShipSpawner.m_MinSpawnTime, m_ShipSpawner.m_MaxSpawnTime));

                        //Ships Needed
                        for (int a = 0; a < shipsNeeded; a++)
                        {
                            BaseBoat boat = GetRandomBoatType(m_ShipSpawner.m_ShipTypes);

                            if (boat == null)
                                continue;

                            bool shipSpawned = false;

                            //Make 50 Attempts to Find Randomized Location for Boat Spawn Point Before Aborting
                            for (int b = 0; b < 50; b++)
                            {
                                if (shipSpawned)                                
                                    break;                                

                                Point3D newLocation = new Point3D();

                                int x = m_ShipSpawner.X;

                                int xOffset = Utility.RandomMinMax(0, m_ShipSpawner.SpawnRange);
                                if (Utility.RandomDouble() >= .5)
                                    xOffset *= -1;

                                x += xOffset;

                                int y = m_ShipSpawner.Y;

                                int yOffset = Utility.RandomMinMax(0, m_ShipSpawner.SpawnRange);
                                if (Utility.RandomDouble() >= .5)
                                    yOffset *= -1;

                                y += yOffset;

                                newLocation.X = x;
                                newLocation.Y = y;
                                newLocation.Z = m_ShipSpawner.Z;

                                Direction newDirection = Direction.North;
                                int shipFacingItemID = -1;                                

                                //Try Preferred Direction first
                                switch (m_ShipSpawner.PreferredDirection)
                                {
                                    case Direction.North:
                                        newDirection = Direction.North;
                                        shipFacingItemID = boat.NorthID;
                                    break;

                                    case Direction.Up:
                                        newDirection = Direction.North;
                                        shipFacingItemID = boat.NorthID;
                                    break;

                                    case Direction.East:
                                        newDirection = Direction.East;
                                        shipFacingItemID = boat.EastID;
                                    break;

                                    case Direction.Right:
                                        newDirection = Direction.East;
                                        shipFacingItemID = boat.EastID;
                                    break;

                                    case Direction.South:
                                        newDirection = Direction.South;
                                        shipFacingItemID = boat.SouthID;
                                    break;

                                    case Direction.Down:
                                        newDirection = Direction.South;
                                        shipFacingItemID = boat.SouthID;
                                    break;

                                    case Direction.West:
                                        newDirection = Direction.West;
                                        shipFacingItemID = boat.WestID;
                                    break;

                                    case Direction.Left:
                                        newDirection = Direction.West;
                                        shipFacingItemID = boat.WestID;
                                    break;

                                    default:
                                        newDirection = Direction.North;
                                        shipFacingItemID = boat.NorthID;
                                    break; 
                                }

                                if (boat.CanFit(newLocation, m_ShipSpawner.Map, shipFacingItemID))
                                {
                                    m_ShipSpawner.m_LastActivity = DateTime.UtcNow;

                                    boat.MoveToWorld(newLocation, m_ShipSpawner.Map);
                                    m_ShipSpawner.m_Boats.Add(boat);
                                    boat.m_ShipSpawner = m_ShipSpawner;

                                    Timer.DelayCall(TimeSpan.FromSeconds(.200), delegate
                                    {
                                        if (boat != null)
                                        {
                                            if (!boat.Deleted && boat.CanFit(newLocation, m_ShipSpawner.Map, shipFacingItemID))                                            
                                                boat.SetFacing(newDirection);                                            
                                        }
                                    });

                                    shipSpawned = true;
                                }

                                //Try Random Direction
                                else
                                {
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

                                    if (boat.CanFit(newLocation, m_ShipSpawner.Map, shipFacingItemID))
                                    {
                                        m_ShipSpawner.m_LastActivity = DateTime.UtcNow;

                                        boat.MoveToWorld(newLocation, m_ShipSpawner.Map);
                                        m_ShipSpawner.m_Boats.Add(boat);
                                        boat.m_ShipSpawner = m_ShipSpawner;

                                        Timer.DelayCall(TimeSpan.FromSeconds(.200), delegate
                                        {
                                            boat.SetFacing(newDirection);
                                        });

                                        shipSpawned = true;
                                    }
                                }
                            }

                            if (!shipSpawned)
                                boat.Delete();
                        }
                    }
                }
            }
        }
    }
}