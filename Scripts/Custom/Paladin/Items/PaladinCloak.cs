using System;

namespace Server.Items
{    
    public class PaladinCloak : BaseCloak
    {
        public override int PlayerClassCurrencyValue { get { return 2500; } }

        [Constructable]
        public PaladinCloak(): this(0)
        {
        }

        [Constructable]
        public PaladinCloak(int hue): base(0x1515, hue)
        {
            Weight = 5.0;
            Name = "Paladin Cloak";

            PlayerClass = PlayerClass.Paladin;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.PaladinItemHue;

            LootType = Server.LootType.Blessed;
        }

        public PaladinCloak(Serial serial): base(serial)
        {
        }

        public override bool Dye(Mobile from, DyeTub sender)
        {
            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}