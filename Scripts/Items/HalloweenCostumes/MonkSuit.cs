using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Server.Items
{
    public class MonkNecklace : Necklace
    {
        [Constructable]
        public MonkNecklace()
        {
            Name = "Monk Necklace [1/3]";
            ItemID = 0x3BB5;
            Hue = 2707;

            DecorativeEquipment = true;

        }

        public MonkNecklace(Serial serial) : base(serial)
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

            DecorativeEquipment = true;
        }
    }

    [FlipableAttribute(0x170d, 0x170e)]
    public class MonkSandals : Sandals
    {
        public override CraftResource DefaultResource { get { return CraftResource.RegularLeather; } }

        [Constructable]
        public MonkSandals()
        {
            Name = "Monk Sandals [2/3]";
            Hue = 343;

            DecorativeEquipment = true;
        }

        public MonkSandals(Serial serial)
                : base(serial)
        {
        }

        public override bool Scissor(Mobile from, Scissors scissors) { return Helpers.CutLeather(this, from); }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            DecorativeEquipment = true;

        }
    }

    public class MonkHalloweenRobe : Robe
    {
        [Constructable]
        public MonkHalloweenRobe()
        {
            Name = "Monk Robe [3/3]";
            Hue = 343;

            DecorativeEquipment = true;
        }

        public MonkHalloweenRobe(Serial serial) : base(serial)
        {
        }

        public override bool Scissor(Mobile from, Scissors scissors) { return Helpers.CutCloth(this, from); }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            DecorativeEquipment = true;
        }
    }


    public class BagOfMonkSuit : Bag
    {
        [Constructable]
        public BagOfMonkSuit()
        {
            Name = "a bag of Monk Suit";

            DropItem(new MonkHalloweenRobe());
            DropItem(new MonkSandals());
            DropItem(new MonkNecklace());
        }

        public BagOfMonkSuit(Serial serial) : base(serial)
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
