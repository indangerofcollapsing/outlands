using System;
using Server;
using Server.Guilds;
using Server.Mobiles;
using Server.Spells;
using System.Collections;
using System.Collections.Generic;

namespace Server.Items
{
    public class PirateHeavyShield : BaseShield
    {
        public override int PlayerClassCurrencyValue { get { return 150; } }

        public override int BasePhysicalResistance { get { return 1; } }
        public override int BaseFireResistance { get { return 0; } }
        public override int BaseColdResistance { get { return 0; } }
        public override int BasePoisonResistance { get { return 0; } }
        public override int BaseEnergyResistance { get { return 0; } }

        public override int InitMinHits { get { return 200; } }
        public override int InitMaxHits { get { return 250; } }

        public override int AosStrReq { get { return 90; } }

        public override int ArmorBase { get { return 34; } }
        public override int OldDexBonus { get { return -6; } }

        [Constructable]
        public PirateHeavyShield(): base(0x1B7B)
        {
            Weight = 6.0;
            Name = "Pirate Heavy Shield";

            PlayerClass = PlayerClass.Pirate;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.PirateItemHue;
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            PlayerClassPersistance.PlayerClassSingleClick(this, from);
        }

        public PirateHeavyShield(Serial serial): base(serial)
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