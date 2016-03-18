using System;
using Server.Items;
using Server.Mobiles;
using Server;
using Server.Engines.Quests;

namespace Server.Items
{
    [FlipableAttribute(0x1c04, 0x1c05)]
    public class PirateFemalePlateChest : BaseArmor
    {
        public override int PlayerClassCurrencyValue { get { return 200; } }

        public override int BasePhysicalResistance { get { return 5; } }
        public override int BaseFireResistance { get { return 3; } }
        public override int BaseColdResistance { get { return 2; } }
        public override int BasePoisonResistance { get { return 3; } }
        public override int BaseEnergyResistance { get { return 2; } }
       
        public override int InitMinHits { get { return 100; } }
        public override int InitMaxHits { get { return 120; } }

        public override int AosStrReq { get { return 95; } }
        public override int OldStrReq { get { return 60; } }
        
        public override int OldDexBonus { get { return -4; } }
        
        public override int ArmorBase { get { return 50; } }       
        public override int RevertArmorBase { get { return 16; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Plate; } }

        [Constructable]
        public PirateFemalePlateChest(): base(0x1c04)
        {
            Weight = 10.0;            
            Name = "Pirate Female Plate Chest";

            PlayerClass = PlayerClass.Pirate;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.PirateItemHue;
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            PlayerClassPersistance.PlayerClassSingleClick(this, from);
        }

        public PirateFemalePlateChest(Serial serial): base(serial)
        {
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