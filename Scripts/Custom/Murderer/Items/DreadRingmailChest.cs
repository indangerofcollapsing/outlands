using System;
using Server.Items;
using Server.Mobiles;
using Server;
using Server.Engines.Quests;

namespace Server.Items
{
    [FlipableAttribute(0x13ec, 0x13ed)]
    public class DreadRingmailChest : BaseArmor
    {
        public override int PlayerClassCurrencyValue { get { return 200; } }

        public override int BasePhysicalResistance { get { return 3; } }
        public override int BaseFireResistance { get { return 3; } }
        public override int BaseColdResistance { get { return 1; } }
        public override int BasePoisonResistance { get { return 5; } }
        public override int BaseEnergyResistance { get { return 3; } }

        public override int InitMinHits { get { return 80; } }
        public override int InitMaxHits { get { return 100; } }

        public override int AosStrReq { get { return 40; } }
        public override int OldStrReq { get { return 20; } }

        public override int RevertArmorBase { get { return 9; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Ringmail; } }

        public override int ArmorBase { get { return 35; } }
        public override int OldDexBonus { get { return -2; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorMeditationAllowance.Half; } }

        [Constructable]
        public DreadRingmailChest(): base(0x13EC)
        {
            Weight = 8.0;            
            Name = "Dread Ringmail Chest";

            PlayerClass = PlayerClass.Murderer;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.MurdererItemHue;
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            PlayerClassPersistance.PlayerClassSingleClick(this, from);
        }

        public DreadRingmailChest(Serial serial): base(serial)
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