using System;
using System.Collections;
using Server.Network;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Items;

namespace Server.Items
{
    public class Flint : Item
    {
        public int m_JailNo;

        [Constructable]
        public Flint() : base(0xE1E)
        {
            this.Weight = 1.0;
            //this.Hue = 1131;
            this.Name = "flint";
        }

        public Flint(Serial serial) : base(serial) { }

        public void TrySteal(Mobile from)
        {
            //if (m_JailNo != null)
            YewJail.YewJailControl.m_YewJailFlintGuards[m_JailNo].TrySteal(from);
        }

        public override bool OnDroppedInto(Mobile from, Container target, Point3D p)
        {
            if (from.Player)
            {
                if (((Server.Mobiles.PlayerMobile)from).YewJailed && target.FindItemByType(typeof(HideableSilverWire)) != null && target.FindItemByType(typeof(Manure)) != null)
                {
                    from.SendGump(new YewJail.YewJailBombGump(((Mobiles.PlayerMobile)from).m_YewJailItem));
                }
            }
            return base.OnDroppedInto(from, target, p);
        }

        public override void OnAdded(object parent)
        {
            if (parent is Mobiles.PlayerMobile)
            {
                Container target = ((Mobiles.PlayerMobile)parent).Backpack;
                Mobile from = parent as Mobile;
                if (from.Player)
                {
                    if (((Server.Mobiles.PlayerMobile)from).YewJailed && target.FindItemByType(typeof(HideableSilverWire)) != null && target.FindItemByType(typeof(Manure)) != null)
                    {
                        from.SendGump(new YewJail.YewJailBombGump(((Mobiles.PlayerMobile)from).m_YewJailItem));
                    }
                }
            }
        
            base.OnAdded(parent);
        }

        public override bool OnDroppedToMobile(Mobile from, Mobile target)
        {
            if (target.Player)
            {
                Container target2 = target.Backpack;

                if (((Server.Mobiles.PlayerMobile)from).YewJailed && target2.FindItemByType(typeof(HideableSilverWire)) != null && target2.FindItemByType(typeof(Manure)) != null)
                {
                    from.SendGump(new YewJail.YewJailBombGump(((Mobiles.PlayerMobile)from).m_YewJailItem));
                }
            }
            return base.OnDroppedToMobile(from, target);
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
        }
    }
}