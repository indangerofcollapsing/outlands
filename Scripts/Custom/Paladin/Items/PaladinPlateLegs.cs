using System;
using Server.Items;
using Server.Mobiles;
using Server;
using Server.Engines.Quests;
using Server.Prompts;
using Server.Network;
using System.Collections;

namespace Server.Items
{
    [FlipableAttribute(0x1411, 0x141a)]
    public class PaladinPlateLegs : BaseArmor
    {
        public override int PlayerClassCurrencyValue { get { return 150; } }

        public override int BasePhysicalResistance { get { return 5; } }
        public override int BaseFireResistance { get { return 3; } }
        public override int BaseColdResistance { get { return 2; } }
        public override int BasePoisonResistance { get { return 3; } }
        public override int BaseEnergyResistance { get { return 2; } }
        
        public override int InitMinHits { get { return 100; } }
        public override int InitMaxHits { get { return 120; } }

        public override int AosStrReq { get { return 90; } }
        public override int OldStrReq { get { return 60; } }
        
        public override int OldDexBonus { get { return -3; } }    
    
        public override int ArmorBase { get { return 50; } }       
        public override int RevertArmorBase { get { return 6; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Plate; } }

        [Constructable]
        public PaladinPlateLegs()
            : base(0x1411)
        {
            Weight = 7.0;
            Name = "Paladin Plate Legs";

            PlayerClass = PlayerClass.Paladin;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.PaladinItemHue;
        }

        public PaladinPlateLegs(Serial serial): base(serial)
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