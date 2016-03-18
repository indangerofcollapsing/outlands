using System;

namespace Server.Items
{
    [FlipableAttribute(0x170d, 0x170e)]
    public class PaladinSandals : BaseShoes
    {
        public override int PlayerClassCurrencyValue { get { return 2500; } }

        public override CraftResource DefaultResource { get { return CraftResource.RegularLeather; } }

        [Constructable]
        public PaladinSandals(): this(0)
        {
        }

        [Constructable]
        public PaladinSandals(int hue): base(0x170D, hue)
        {
            Weight = 1.0;
            Name = "Paladin Sandals";

            PlayerClass = PlayerClass.Paladin;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.PaladinItemHue;

            LootType = Server.LootType.Blessed;
        }

        public PaladinSandals(Serial serial) : base(serial)
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
