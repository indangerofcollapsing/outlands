using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public class BagOfCannonShot : Bag
    {
        [Constructable]
        public BagOfCannonShot(): this(100)
        {
            Hue = 1102;
        }

        [Constructable]
        public BagOfCannonShot(int amount)
        {
            Hue = 1102;

            DropItem(new CannonShot(amount));
        }

        public BagOfCannonShot(Serial serial): base(serial)
        {
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