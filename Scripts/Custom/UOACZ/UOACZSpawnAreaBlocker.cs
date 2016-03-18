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
    public class UOACZSpawnAreaBlocker : Item
    {
        public static List<UOACZSpawnAreaBlocker> m_Instances = new List<UOACZSpawnAreaBlocker>();
               
        private Rectangle2D m_BlockedArea = new Rectangle2D(0, 0, 0, 0);
        [CommandProperty(AccessLevel.GameMaster)]
        public Rectangle2D BlockedArea
        {
            get { return m_BlockedArea; }
            set { m_BlockedArea = value; }
        }

        [Constructable]
        public UOACZSpawnAreaBlocker(): base(7960)
        {
            Name = "Spawn Area Blocker";

            Hue = 2117;  
            Movable = false;                  

            m_Instances.Add(this);
        }

        public UOACZSpawnAreaBlocker(Serial serial): base(serial)
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
            writer.Write(m_BlockedArea);           
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_BlockedArea = reader.ReadRect2D();              
            }

            //-------

            m_Instances.Add(this);
        }
    }
}