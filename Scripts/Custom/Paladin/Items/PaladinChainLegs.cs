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
    [FlipableAttribute(0x13be, 0x13c3)]
    public class PaladinChainLegs : BaseArmor
    {
        public override int PlayerClassCurrencyValue { get { return 150; } }

        public override int BasePhysicalResistance { get { return 4; } }
        public override int BaseFireResistance { get { return 4; } }
        public override int BaseColdResistance { get { return 4; } }
        public override int BasePoisonResistance { get { return 1; } }
        public override int BaseEnergyResistance { get { return 2; } }

        public override int InitMinHits { get { return 90; } }
        public override int InitMaxHits { get { return 110; } }

        public override int AosStrReq { get { return 60; } }
        public override int OldStrReq { get { return 20; } }

        public override int RevertArmorBase { get { return 6; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Chainmail; } }

        public override int ArmorBase { get { return 40; } }
        public override int OldDexBonus { get { return -2; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorMeditationAllowance.Quarter; } }

        [Constructable]
        public PaladinChainLegs(): base(0x13BE)
        {
            Weight = 7.0;
            Name = "Paladin Chain Legs";       

            PlayerClass = PlayerClass.Paladin;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.PaladinItemHue;
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            PlayerClassPersistance.PlayerClassSingleClick(this, from);
        }

        public PaladinChainLegs(Serial serial): base(serial)
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