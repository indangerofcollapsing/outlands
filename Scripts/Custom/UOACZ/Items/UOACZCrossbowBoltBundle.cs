using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
    public class UOACZBoltBundle : Item
    {
        [Constructable]
        public UOACZBoltBundle(): base(7165)
        {
            Name = "a bolt bundle";

            Weight = 3;
        }

        public UOACZBoltBundle(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            LabelTo(from, "(double click to separate)");
        }
        
        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("That item must be in your pack in order to use it.");
                return;
            }

            from.SendMessage("You separate the crossbow bolt bundle and its contents are placed in your backpack.");

            Item[] bolts = from.Backpack.FindItemsByType(typeof(Bolt));

            Bolt largestItem = null;
            int highestCount = 0;

            foreach (Item item in bolts)
            {
                Bolt bolt = item as Bolt;

                if (bolt.Amount > highestCount)
                    largestItem = bolt;
            }

            if (largestItem != null)
                largestItem.Amount += 25;

            else
                from.Backpack.DropItem(new Bolt(25));

            Delete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version          
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}