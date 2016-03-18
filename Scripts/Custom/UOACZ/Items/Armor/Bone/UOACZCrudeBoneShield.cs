using System;
using Server;

namespace Server.Items
{
    public class UOACZCrudeBoneShield : BaseShield
    {
        public override int BasePhysicalResistance { get { return 0; } }
        public override int BaseFireResistance { get { return 1; } }
        public override int BaseColdResistance { get { return 0; } }
        public override int BasePoisonResistance { get { return 0; } }
        public override int BaseEnergyResistance { get { return 0; } }

        public override int InitMinHits { get { return 30; } }
        public override int InitMaxHits { get { return 40; } }

        public override int AosStrReq { get { return 45; } }

        public override int ArmorBase { get { return 11; } }
        public override int OldDexBonus { get { return -3; } }

        [Constructable]
        public UOACZCrudeBoneShield(): base(0x1B7B)
        {
            Name = "crude bone shield";

            Weight = 6.0;
            Hue = 2955;
        }

        public UOACZCrudeBoneShield(Serial serial): base(serial)
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
