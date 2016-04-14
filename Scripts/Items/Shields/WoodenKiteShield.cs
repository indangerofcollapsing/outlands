using System;
using Server;

namespace Server.Items
{
    public class WoodenKiteShield : BaseShield
    {
        public override int BasePhysicalResistance { get { return 0; } }
        public override int BaseFireResistance { get { return 0; } }
        public override int BaseColdResistance { get { return 0; } }
        public override int BasePoisonResistance { get { return 0; } }
        public override int BaseEnergyResistance { get { return 1; } }

        public override int InitMinHits { get { return 80; } }
        public override int InitMaxHits { get { return 80; } }

        public override int AosStrReq { get { return 20; } }

        public override int ArmorBase { get { return 14; } }
        public override int OldDexBonus { get { return -2; } }

        public override int IconItemId { get { return 7033; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 5; } }
        public override int IconOffsetY { get { return 6; } }

        public override CraftResource DefaultResource { get { return CraftResource.RegularWood; } }

        [Constructable]
        public WoodenKiteShield(): base(7033)
        {
            Name = "wooden kite shield";
            Weight = 5.0;
        }

        public WoodenKiteShield(Serial serial)
            : base(serial)
        {
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (Weight == 7.0)
                Weight = 5.0;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);//version
        }
    }
}
