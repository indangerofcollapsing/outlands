using System;
using Server.Items;
using System.Collections;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;

namespace Server
{
    public class UOACZDestination : Item
    {
        public static List<UOACZDestination> m_UOACZDestinations = new List<UOACZDestination>();

        public enum DirectionType
        {
            Entrance,
            Exit
        }

        public enum ProfileType
        {
            Human,
            Undead
        }

        private DirectionType m_GateDirection = DirectionType.Entrance;
        [CommandProperty(AccessLevel.GameMaster)]
        public DirectionType GateDirection
        {
            get { return m_GateDirection; }
            set { m_GateDirection = value; }
        }

        private ProfileType m_ProfileTypeAllowed = ProfileType.Human;
        [CommandProperty(AccessLevel.GameMaster)]
        public ProfileType ProfileTypeAllowed
        {
            get { return m_ProfileTypeAllowed; }
            set { m_ProfileTypeAllowed = value; }
        }

        private bool m_MurdererFriendly = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool MurdererFriendly
        {
            get { return m_MurdererFriendly; }
            set { m_MurdererFriendly = value; }
        }

        [Constructable]
        public UOACZDestination(): base(7973)
        {
            Visible = false;
            Movable = false;

            Hue = 2591;

            m_UOACZDestinations.Add(this);
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, "UOACZ Destination: " + m_GateDirection.ToString());
            LabelTo(from, "(" + m_ProfileTypeAllowed.ToString() + ")");
        }

        public static UOACZDestination GetRandomEntrance(bool isHuman)
        {
            UOACZDestination entranceDestination = null;

            List<UOACZDestination> m_Destinations = new List<UOACZDestination>();

            foreach (UOACZDestination destination in UOACZDestination.m_UOACZDestinations)
            {
                if (destination == null) continue;
                if (destination.Deleted) continue;
                if (!UOACZRegion.ContainsItem(destination)) continue;

                if (isHuman && destination.m_ProfileTypeAllowed == ProfileType.Human && destination.GateDirection == UOACZDestination.DirectionType.Entrance)
                    m_Destinations.Add(destination);

                if (!isHuman && destination.m_ProfileTypeAllowed == ProfileType.Undead && destination.GateDirection == UOACZDestination.DirectionType.Entrance)
                    m_Destinations.Add(destination);
            }

            if (m_Destinations.Count == 0)
                return entranceDestination;

            entranceDestination = m_Destinations[Utility.RandomMinMax(0, m_Destinations.Count - 1)];

            return entranceDestination;
        }

        public static UOACZDestination GetRandomExit(bool murderer)
        {
            UOACZDestination destination = null;

            List<UOACZDestination> m_Destinations = new List<UOACZDestination>();

            foreach (UOACZDestination Destination in UOACZDestination.m_UOACZDestinations)
            {
                if (Destination == null) continue;
                if (Destination.Deleted) continue;
                if (Destination.GateDirection == UOACZDestination.DirectionType.Exit)
                {
                    if (Destination.MurdererFriendly && murderer)
                        m_Destinations.Add(Destination);

                    if (!Destination.MurdererFriendly && !murderer)
                        m_Destinations.Add(Destination);
                }
            }

            if (m_Destinations.Count == 0)
                return destination;

            destination = m_Destinations[Utility.RandomMinMax(0, m_Destinations.Count - 1)];

            return destination;
        }

        public UOACZDestination(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); //version    

            writer.Write((int)m_GateDirection);
            writer.Write((int)m_ProfileTypeAllowed);

            //Version 1
            writer.Write(m_MurdererFriendly);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_GateDirection = (DirectionType)reader.ReadInt();
            m_ProfileTypeAllowed = (ProfileType)reader.ReadInt();

            //Version 1
            if (version >= 1)
            {
                m_MurdererFriendly = reader.ReadBool();
            }

            //------------

            m_UOACZDestinations.Add(this);
        }
    }
}