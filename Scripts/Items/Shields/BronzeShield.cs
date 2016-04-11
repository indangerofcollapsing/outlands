using System;
using Server;

namespace Server.Items
{
    public class BronzeShield : BaseShield
    {
        public override int BasePhysicalResistance { get { return 0; } }
        public override int BaseFireResistance { get { return 0; } }
        public override int BaseColdResistance { get { return 1; } }
        public override int BasePoisonResistance { get { return 0; } }
        public override int BaseEnergyResistance { get { return 0; } }

        public override int InitMinHits { get { return 85; } }
        public override int InitMaxHits { get { return 85; } }

        public override int AosStrReq { get { return 35; } }

        public override int ArmorBase { get { return 18; } }
        public override int OldDexBonus { get { return -4; } }

        public override int IconItemId { get { return 7026; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 1; } }
        public override int IconOffsetY { get { return 5; } }

        [Constructable]
        public BronzeShield() : base(7026)
        {
            Weight = 6.0;
        }

        public BronzeShield(Serial serial)
            : base(serial)
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
