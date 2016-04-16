using System;
using Server;
using Server.Network;
using Server.Mobiles;
using Server.Custom;
using Server.Multis;
using System.Collections.Generic;

namespace Server.Items
{   
    public class TransportContractCargo : Item
    {
        private ShipTransportContract m_Contract;
        [CommandProperty(AccessLevel.GameMaster)]
        public ShipTransportContract Contract
        {
            get { return m_Contract; }
            set { m_Contract = value; }
        }

        [Constructable]
        public TransportContractCargo(): base()
        {
            Name = "cargo";
            ItemID = Utility.RandomList(4014, 2473, 3703, 3715, 3711, 3645, 3644, 3647, 3648, 2475, 3710, 7808, 7809);

            Weight = 0.0;           
        }

        public TransportContractCargo(Serial serial): base(serial)
        {
        }        

        public override void OnSingleClick(Mobile from)
        {            
            if (m_Contract == null)
                return;

            m_Contract.CheckExpiration();

            string contractType = "Daily Transport Cargo: ";

            switch (m_Contract.ContractType)
            {
                case ShipTransportContract.TransportContractType.DailyEasy: contractType = "Easy"; break;
                case ShipTransportContract.TransportContractType.DailyChallenging: contractType = "Challenging"; break;
                case ShipTransportContract.TransportContractType.DailyDangerous: contractType = "Dangerous"; break;
            }

            LabelTo(from, contractType);

            Point3D m_Start = ShipTransportContract.GetLocationPoint(m_Contract.PickupLocation);
            Point3D m_End = ShipTransportContract.GetLocationPoint(m_Contract.Destination);

            LabelTo(from, "[" + m_Start.X.ToString() + "," + m_Start.Y.ToString() + " to " + m_End.X.ToString() + "," + m_End.Y.ToString() + "]");

            if (m_Contract.Resolved)
            {
                if (m_Contract.Successful)
                    LabelTo(from, "(Completed)");

                else
                    LabelTo(from, "(Failed)");
            }

            else
            {
                string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, m_Contract.ExpirationDate, true, true, true, true, true);

                LabelTo(from, "(Expires in " + timeRemaining + ")");
            }     
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            if (m_Contract == null)
                return;

            m_Contract.CheckExpiration();

            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            player.SendGump(new ShipTransportContractGump(player, m_Contract));
        }

        public override bool OnDroppedOnto(Mobile from, Item target)
        {
            return false;
        }

        public override bool OnDroppedToMobile(Mobile from, Mobile target)
        {
            return false;
        }

        public override bool OnDroppedToWorld(Mobile from, Point3D p)
        {
            return false;
        }

        public override void OnLocationChange(Point3D oldLocation)
        {
            base.OnLocationChange(oldLocation);

            if (m_Contract == null) return;
            if (m_Contract.Deleted) return;

            m_Contract.CheckExpiration();

            if (m_Contract.Resolved)
                return;

            Point3D pickupLocation = ShipTransportContract.GetLocationPoint(m_Contract.PickupLocation);
            Point3D destination = ShipTransportContract.GetLocationPoint(m_Contract.Destination);

            if (Utility.GetDistance(Location, destination) <= ShipTransportContract.DropOffProximity)
            {
                BaseBoat boat = BaseBoat.FindBoatAt(Location, Map);

                if (boat != null)
                {
                    List<Mobile> boatMobiles = boat.GetMobilesOnBoat(true, true);

                    foreach (Mobile mobile in boatMobiles)
                    {
                        mobile.SendMessage("You have reached your cargo's destination. Rewards have been place in your ship's hold.");
                    }

                    m_Contract.Successful = true;
                    m_Contract.Resolved = true;

                    Delete();

                    return;
                }
            }

            int distanceFromPickup = Utility.GetDistance(Location, pickupLocation);
            int distanceFromDestination = Utility.GetDistance(Location, destination);
            int totalDistance = Utility.GetDistance(pickupLocation, destination);
            
            if (m_Contract.OceanEventsOccured < m_Contract.MaxOceanEvents)
            {
                if (distanceFromPickup < ShipTransportContract.OceanEventMinimumDistanceFromPickupLocation)
                    return;

                if (distanceFromDestination < ShipTransportContract.OceanEventMinimumDistanceFromDestination)
                    return;

                if (m_Contract.LastOceanEvent + ShipTransportContract.OceanEventMinimumDelayBetweenEvents > DateTime.UtcNow)
                    return;

                BaseBoat boat = BaseBoat.FindBoatAt(Location, Map);

                if (boat == null)
                    return;

                m_Contract.OceanEventsOccured++;
                m_Contract.LastOceanEvent = DateTime.UtcNow;
                m_Contract.LastOceanEventLocation = Location;
                
                int eventDifficulty = ((int)m_Contract.ContractType) + 1;

                OceanEvent.StartOceanEvent(Location, Map, eventDifficulty, boat);
            }           
        }

        public override void OnDelete()
        {
            if (m_Contract != null)
            {
                if (!m_Contract.Resolved)
                {
                    m_Contract.Resolved = true;
                    m_Contract.Successful = false;

                    if (m_Contract.Creator != null)
                        m_Contract.Creator.SendMessage("You abandon a transport contract in progress.");
                }                
            }

            base.OnDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version

            //Version 0
            writer.Write(m_Contract);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version
            if (version >= 0)
            {
                m_Contract = (ShipTransportContract)reader.ReadItem();
            }
        } 
    }
}