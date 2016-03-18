using System;

namespace Server.Items
{
    [FlipableAttribute(0x170d, 0x170e)]
    public class WoodlandSpriteSandals : BaseShoes
    {
        public override CraftResource DefaultResource { get { return CraftResource.RegularLeather; } }

        [Constructable]
        public WoodlandSpriteSandals(): this(0)
        {
        }

        [Constructable]
        public WoodlandSpriteSandals(int hue): base(0x170D, hue)
        {
            Weight = 1.0;
            Name = "Woodland Sprite Sandals";

            Hue = 2003;
        }

        public WoodlandSpriteSandals(Serial serial): base(serial)
        {
        }

        public override bool Dye(Mobile from, DyeTub sender) { return false;}

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
