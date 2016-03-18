using System;

namespace Server.Items
{
    public class PirateSkullcap : BaseHat
    {
        public override int PlayerClassCurrencyValue { get { return 2500; } }

        [Constructable]
        public PirateSkullcap()
            : this(0)
        {
        }

        [Constructable]
        public PirateSkullcap(int hue)
            : base(0x1544, hue)
        {
            Weight = 1.0;
            Name = "Pirate Skullcap";

            PlayerClass = PlayerClass.Pirate;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.PirateItemHue;

            LootType = Server.LootType.Blessed;
        }

        public PirateSkullcap(Serial serial)
            : base(serial)
        {
        }

        public override bool Dye(Mobile from, DyeTub sender)
        {
            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class ArmoredPirateSkullcap : BaseArmoredHat
    {
        [Constructable]
        public ArmoredPirateSkullcap()
            : base(0x1544)
        {
            Weight = 1.0;
            Name = "Pirate Skullcap";

            PlayerClass = PlayerClass.Pirate;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.PirateItemHue;

            LootType = Server.LootType.Blessed;
        }

        public ArmoredPirateSkullcap(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}