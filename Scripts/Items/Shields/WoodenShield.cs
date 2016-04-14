using System;
using Server;

namespace Server.Items
{
    public class WoodenShield : BaseShield
    {
        public override int BasePhysicalResistance { get { return 0; } }
        public override int BaseFireResistance { get { return 0; } }
        public override int BaseColdResistance { get { return 0; } }
        public override int BasePoisonResistance { get { return 0; } }
        public override int BaseEnergyResistance { get { return 1; } }

        public override int InitMinHits { get { return 70; } }
        public override int InitMaxHits { get { return 70; } }

        public override int AosStrReq { get { return 20; } }

        public override int ArmorBase { get { return 10; } }
        public override int OldDexBonus { get { return 0; } }

        public override int IconItemId { get { return 7034; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return -1; } }
        public override int IconOffsetY { get { return 8; } }

        public override CraftResource DefaultResource { get { return CraftResource.RegularWood; } }

        [Constructable]
        public WoodenShield(): base(7034)
        {
            Name = "wooden shield";
            Weight = 5.0;
        }

        public WoodenShield(Serial serial): base(serial)
        {
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);//version
        }
    }
}
