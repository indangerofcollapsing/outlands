using System;
using Server.Items;
using Server.Mobiles;
using Server;
using Server.Engines.Quests;

namespace Server.Items
{
    [FlipableAttribute(0x1414, 0x1418)]
    public class PiratePlateGloves : BaseArmor
    {
        public override int PlayerClassCurrencyValue { get { return 100; } }

        public override int BasePhysicalResistance { get { return 5; } }
        public override int BaseFireResistance { get { return 3; } }
        public override int BaseColdResistance { get { return 2; } }
        public override int BasePoisonResistance { get { return 3; } }
        public override int BaseEnergyResistance { get { return 2; } }
        
        public override int InitMinHits { get { return 100; } }
        public override int InitMaxHits { get { return 120; } }

        public override int AosStrReq { get { return 70; } }
        public override int OldStrReq { get { return 30; } }
       
        public override int OldDexBonus { get { return 0; } }
       
        public override int ArmorBase { get { return 50; } }      
        public override int RevertArmorBase { get { return 3; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Plate; } }

        [Constructable]
        public PiratePlateGloves(): base(0x1414)
        {
            Weight = 2.0;
            Name = "Pirate Plate Gloves";

            PlayerClass = PlayerClass.Pirate;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.PirateItemHue;
        }

        public PiratePlateGloves(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            PlayerClassPersistance.PlayerClassSingleClick(this, from);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}