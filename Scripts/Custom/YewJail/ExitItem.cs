using System;
using System.Collections;
using Server.Network;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Items;
using Server.Mobiles;

namespace Server.Items
{
    public class ExitItem : Item
    {
        public ExitItem()
            : base(0xE1E)
        {
            //this.Hue = 1131;
            this.Name = "JailExitDetector";
            Visible = false;
        }

        public ExitItem(Serial serial) : base(serial) { }

        public override bool OnMoveOver(Mobile m)
        {
            if (m.Player)
            {
                if (((PlayerMobile)m).YewJailed)
                {
                    ((PlayerMobile)m).m_YewJailItem.EscapedJail();
                }
            }

            return base.OnMoveOver(m);
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            YewJail.YewJailControl.RegisterExitItem(this);
        }
    }
}
