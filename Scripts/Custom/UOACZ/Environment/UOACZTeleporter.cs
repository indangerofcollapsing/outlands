using System;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;

namespace Server.Items
{
    public class UOACZTeleporter : Teleporter
    {
        private bool m_CreatureOnly = false;
        [CommandProperty(AccessLevel.Counselor)]
        public bool CreatureOnly
        {
            get { return m_CreatureOnly; }
            set
            { 
                m_CreatureOnly = value;

                if (m_CreatureOnly)
                    Hue = 0;

                else
                    Hue = 2115;
            }
        }

        [Constructable]
        public UOACZTeleporter(): base()
        {
            Visible = true;
        }

        public override bool CanBeSeenBy(Mobile from)
        {
            if (from.AccessLevel > AccessLevel.Player)
                return true;

            return false;
        }

        public override bool CanTeleport(Mobile m)
        {
            if (m_CreatureOnly)
            {
                if (m is PlayerMobile)
                    return false;
            }

            return true;
        }

        public UOACZTeleporter(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version  
        
            //Version 0
            writer.Write(m_CreatureOnly);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 1)
            {
                m_CreatureOnly = reader.ReadBool();
            }
        }
    }
}
