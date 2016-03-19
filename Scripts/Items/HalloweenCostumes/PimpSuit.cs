using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Items
{
    public class PimpNecklace : Necklace
    {
        [Constructable]
        public PimpNecklace()
        {
            Name = "Pimp Necklace [1/5]";

            DecorativeEquipment = true;
        }

        public PimpNecklace(Serial serial) : base(serial)
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

    public class PimpWatch : GoldBracelet
    {
        [Constructable]
        public PimpWatch()
        {
            Weight = 0.1;
            Name = "Pimp Watch [2/5]";

            DecorativeEquipment = true;
        }

        public PimpWatch(Serial serial) : base(serial)
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

    public class PimpCape : Cloak
    {
        [Constructable]
        public PimpCape()
        {
            Name = "Pimp Cape [3/5]";
            Hue = 13;

            DecorativeEquipment = true;
        }

        public PimpCape(Serial serial)
                : base(serial)
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

    public class PimpShoes : Boots
    {
        [Constructable]
        public PimpShoes()

        {
            Name = "Pimp Shoes [4/5]";
            Hue = 13;

            DecorativeEquipment = true;
        }

        public PimpShoes(Serial serial)
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

    public class PimpHat : FeatheredHat
    {

        [Constructable]
        public PimpHat()
        {
            Name = "Pimp Hat [5/5]";
            Hue = 13;

            DecorativeEquipment = true;
        }

        public PimpHat(Serial serial)
                : base(serial)
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

    public class BagOfPimpSuit : Bag
    {
        [Constructable]
        public BagOfPimpSuit()
        {
            Name = "a bag of Pimp Suit";

            DropItem(new PimpWatch());
            DropItem(new PimpNecklace());
            DropItem(new PimpCape());
            DropItem(new PimpShoes());
            DropItem(new PimpHat());
        }

        public BagOfPimpSuit(Serial serial) : base(serial)
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
