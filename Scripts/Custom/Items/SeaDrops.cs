using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Items;

namespace Server.Items
{
    [FlipableAttribute(0x13BB, 0x13C0)]
    public class SeaScaleCoif : BaseArmor
    {
        public override int BasePhysicalResistance { get { return 4; } }
        public override int BaseFireResistance { get { return 4; } }
        public override int BaseColdResistance { get { return 4; } }
        public override int BasePoisonResistance { get { return 1; } }
        public override int BaseEnergyResistance { get { return 2; } }

        public override int InitMinHits { get { return 35; } }
        public override int InitMaxHits { get { return 60; } }

        public override int AosStrReq { get { return 60; } }
        public override int OldStrReq { get { return 20; } }

        //Changed to IPY Value
        public override int ArmorBase { get { return 23; } }
        //Added by IPY
        public override int RevertArmorBase { get { return 3; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Chainmail; } }

        [Constructable]
        public SeaScaleCoif()
            : base(0x13BB)
        {
            Weight = 1.0;
            Hue = 2124;
            Name = "Sea Serpent Scale Coif";
        }

        public SeaScaleCoif(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [FlipableAttribute(0x13bf, 0x13c4)]
    public class SeaScaleChest : BaseArmor
    {
        public override int BasePhysicalResistance { get { return 4; } }
        public override int BaseFireResistance { get { return 4; } }
        public override int BaseColdResistance { get { return 4; } }
        public override int BasePoisonResistance { get { return 1; } }
        public override int BaseEnergyResistance { get { return 2; } }

        public override int InitMinHits { get { return 45; } }
        public override int InitMaxHits { get { return 60; } }

        public override int AosStrReq { get { return 60; } }
        public override int OldStrReq { get { return 20; } }

        //IPY values
        public override int OldDexBonus { get { return 0; } }

        //IPY Value
        public override int ArmorBase { get { return 23; } }
        //Added by IPY
        public override int RevertArmorBase { get { return 10; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Chainmail; } }

        [Constructable]
        public SeaScaleChest()
            : base(0x13BF)
        {
            Weight = 7.0;
            Hue = 2124;
            Name = "Sea Serpent Scale Tunic";
        }

        public SeaScaleChest(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [FlipableAttribute(0x13be, 0x13c3)]
    public class SeaScaleLegs : BaseArmor
    {
        public override int BasePhysicalResistance { get { return 4; } }
        public override int BaseFireResistance { get { return 4; } }
        public override int BaseColdResistance { get { return 4; } }
        public override int BasePoisonResistance { get { return 1; } }
        public override int BaseEnergyResistance { get { return 2; } }

        public override int InitMinHits { get { return 45; } }
        public override int InitMaxHits { get { return 60; } }

        public override int AosStrReq { get { return 60; } }
        public override int OldStrReq { get { return 20; } }

        //IPY Value
        public override int OldDexBonus { get { return 0; } }
        //IpY Value
        public override int ArmorBase { get { return 23; } }
        //Added by IPY
        public override int RevertArmorBase { get { return 6; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Chainmail; } }

        [Constructable]
        public SeaScaleLegs()
            : base(0x13BE)
        {
            Weight = 7.0;
            Hue = 2124;
            Name = "Sea Serpent Scale Leggings";
        }

        public SeaScaleLegs(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

        
}
