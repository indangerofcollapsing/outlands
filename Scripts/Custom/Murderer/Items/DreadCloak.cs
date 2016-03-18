using System;

namespace Server.Items
{    
    public class DreadCloak : BaseCloak
    {
        public override int PlayerClassCurrencyValue { get { return 2500; } }

        [Constructable]
        public DreadCloak(): this(0)
        {
        }

        [Constructable]
        public DreadCloak(int hue): base(0x1515, hue)
        {
            Weight = 5.0;
            Name = "Dread Cloak";

            PlayerClass = PlayerClass.Murderer;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.MurdererItemHue;

            LootType = Server.LootType.Blessed;
        }

        public DreadCloak(Serial serial): base(serial)
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