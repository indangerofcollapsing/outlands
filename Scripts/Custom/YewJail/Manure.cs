/***************************************************************************
 *                                 Manure.cs
 *                            -------------------
 *   begin                : July 2010
 *   author               : Sean Stavropoulos
 *   email                : sean.stavro@gmail.com
 *
 *
 ***************************************************************************/
using System;
using System.Collections;
using Server.Network;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Items;

namespace Server.Items 
{
    public class Manure : Item
    {
        [Constructable]
        public Manure()
            : base(0x1CF0)
        {
            this.Weight = 1.0;
            this.Hue = 1131;
            this.Name = "manure";
        }

       public Manure(Serial serial) : base(serial) { }

       public override bool OnDroppedInto(Mobile from, Container target, Point3D p)
       {
           if (from.Player)
           {
               if (((Server.Mobiles.PlayerMobile)from).YewJailed && target.FindItemByType(typeof(Flint)) != null && target.FindItemByType(typeof(HideableSilverWire)) != null)
               {
                   from.SendGump(new YewJail.YewJailBombGump(((Mobiles.PlayerMobile)from).m_YewJailItem));
               }
           }
           return base.OnDroppedInto(from, target, p);
       }

       public override bool OnDroppedToMobile(Mobile from, Mobile target)
       {
           if (target.Player)
           {
               Container target2 = target.Backpack;

               if (((Server.Mobiles.PlayerMobile)from).YewJailed && target2.FindItemByType(typeof(HideableSilverWire)) != null && target2.FindItemByType(typeof(Flint)) != null)
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