using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Items;

namespace Server.Items
{
    public class PlumbersHat : Cap
    {
        [Constructable]
        public PlumbersHat()
        {
            Name = "Plumbers Hat [1/4]";
            Hue = 2118;

            DecorativeEquipment = true;
        }

        public PlumbersHat(Serial serial) : base(serial)
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

    public class PlumbersShirt : FancyShirt
    {
        [Constructable]
        public PlumbersShirt()
        {
            Name = "Plumbers Shirt [2/4]";
            Hue = 2118;

            DecorativeEquipment = true;
        }

        public PlumbersShirt(Serial serial) : base(serial)
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

    public class PlumbersOverallsPt1 : FullApron
    {
        [Constructable]
        public PlumbersOverallsPt1()
        {
            Name = "Plumbers Overall Pt1 [3/4]";
            Hue = 2123;

            DecorativeEquipment = true;
        }

        public PlumbersOverallsPt1(Serial serial) : base(serial)
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

    public class PlumbersOverallsPt2 : LongPants
    {
        [Constructable]
        public PlumbersOverallsPt2()
        {
            Name = "Plumbers Overall Pt2 [4/4]";
            Hue = 2123;

            DecorativeEquipment = true;
        }

        public PlumbersOverallsPt2(Serial serial) : base(serial)
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

    public class BagOfPlumberSuit : Bag
    {
        [Constructable]
        public BagOfPlumberSuit()
        {
            Name = "a bag of Plumbers Suit";

            DropItem(new PlumbersHat());
            DropItem(new PlumbersShirt());
            DropItem(new PlumbersOverallsPt1());
            DropItem(new PlumbersOverallsPt2());
        }

        public BagOfPlumberSuit(Serial serial) : base(serial)
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
