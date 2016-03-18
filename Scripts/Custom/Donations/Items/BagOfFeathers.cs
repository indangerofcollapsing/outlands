using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class BagOfFeathers : Bag
    {
        public const int FeatherAmount = 15;

        [Constructable]
        public BagOfFeathers()
        {
            PackItems();
        }

        public void PackItems()
        {
            for (int i = 0; i < FeatherAmount; ++i)
                DropItem(new Feather());
        }

        public BagOfFeathers(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }
}
