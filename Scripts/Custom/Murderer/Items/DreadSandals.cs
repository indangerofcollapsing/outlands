using System;

namespace Server.Items
{
    [FlipableAttribute(0x170d, 0x170e)]
    public class DreadSandals : BaseShoes
    {
        public override int PlayerClassCurrencyValue { get { return 2500; } }

        public override CraftResource DefaultResource { get { return CraftResource.RegularLeather; } }

        [Constructable]
        public DreadSandals(): this(0)
        {
        }

        [Constructable]
        public DreadSandals(int hue): base(0x170D, hue)
        {
            Weight = 1.0;
            Name = "Dread Sandals";

            PlayerClass = PlayerClass.Murderer;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.MurdererItemHue;

            LootType = Server.LootType.Blessed;
        }

        public DreadSandals(Serial serial): base(serial)
        {
        }

        public override bool Dye(Mobile from, DyeTub sender)
        {
            return false;
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
