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
    public class UOACZSpawnRedirector : Item
    {
        public static List<UOACZSpawnRedirector> m_Instances = new List<UOACZSpawnRedirector>();

        private int m_TriggerRadius = 10;
        [CommandProperty(AccessLevel.GameMaster)]
        public int TriggerRadius
        {
            get { return m_TriggerRadius; }
            set { m_TriggerRadius = value; }
        }

        private int m_MinRedirection = 12;
        [CommandProperty(AccessLevel.GameMaster)]
        public int MinRedirection
        {
            get { return m_MinRedirection; }
            set { m_MinRedirection = value; }
        }

        private int m_MaxRedirection = 16;
        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxRedirection
        {
            get { return m_MaxRedirection; }
            set { m_MaxRedirection = value; }
        }

        [Constructable]
        public UOACZSpawnRedirector(): base(7960)
        {
            Name = "Spawn Redirector";

            Hue = 2587;  
            Movable = false;                  

            m_Instances.Add(this);
        }

        public UOACZSpawnRedirector(Serial serial): base(serial)
        {
        }

        public override bool CanBeSeenBy(Mobile from)
        {
            if (from.AccessLevel > AccessLevel.Player)
                return true;

            return false;
        }     

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version

            //Version 0
            writer.Write((int)m_TriggerRadius);
            writer.Write((int)m_MinRedirection);
            writer.Write((int)m_MaxRedirection);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_TriggerRadius = reader.ReadInt();
                m_MinRedirection = reader.ReadInt();
                m_MaxRedirection = reader.ReadInt();
            }

            //-------

            m_Instances.Add(this);
        }
    }
}