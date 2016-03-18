using System;

namespace Server.Items
{    
    public class PirateCloak : BaseCloak
    {
        public override int PlayerClassCurrencyValue { get { return 2500; } }

        [Constructable]
        public PirateCloak(): this(0)
        {
        }

        [Constructable]
        public PirateCloak(int hue): base(0x1515, hue)
        {
            Weight = 5.0;
            Name = "Pirate Cloak";

            PlayerClass = PlayerClass.Pirate;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.PirateItemHue;

            LootType = Server.LootType.Blessed;
        }

        public PirateCloak(Serial serial): base(serial)
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