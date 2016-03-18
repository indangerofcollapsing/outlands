using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Items;

namespace Server.Items
{

    public class KantmissBow : Bow
    {
        [Constructable]
        public KantmissBow()
        {
            Name = "Kantmiss Averdyne Bow [1/5]";
            Hue = 2406;

            DecorativeEquipment = true;
        }

        public KantmissBow(Serial serial) : base(serial)
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

    public class KantmissChest : LeatherChest, IScissorable
    {
        [Constructable]
        public KantmissChest()
        {
            Name = "Kantmiss Averdyne Chest [2/5]";
            Hue = 2406;

            DecorativeEquipment = true;
        }

        public KantmissChest(Serial serial) : base(serial)
        {
        }

        public new bool Scissor(Mobile from, Scissors scissors) { return Helpers.CutLeather(this, from); }

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

    public class KantmissLegs : LeatherLegs, IScissorable
    {
        [Constructable]
        public KantmissLegs()
        {
            Name = "Kantmiss Averdyne Legs [3/5]";
            Hue = 2406;

            DecorativeEquipment = true;
        }

        public KantmissLegs(Serial serial) : base(serial)
        {
        }

        public new bool Scissor(Mobile from, Scissors scissors) { return Helpers.CutLeather(this, from); }

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

    public class KantmissArms : LeatherArms, IScissorable
    {
        [Constructable]
        public KantmissArms()
        {
            Name = "Kantmiss Averdyne Arms [4/5]";
            Hue = 2406;

            DecorativeEquipment = true;

        }

        public KantmissArms(Serial serial) : base(serial)
        {
        }

        public new bool Scissor(Mobile from, Scissors scissors) { return Helpers.CutLeather(this, from); }
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

    public class KantmissBoots : Boots
    {
        [Constructable]
        public KantmissBoots()
        {
            Name = "Kantmiss Averdyne Boots [5/5]";
            Hue = 2406;

            DecorativeEquipment = true;
        }

        public KantmissBoots(Serial serial) : base(serial)
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


    public class BagOfKantmissSuit : Bag
    {
        [Constructable]
        public BagOfKantmissSuit()
        {
            Name = "a bag of Kantmiss Averdyne Suit";

            DropItem(new KantmissBow());
            DropItem(new KantmissChest());
            DropItem(new KantmissLegs());
            DropItem(new KantmissArms());
            DropItem(new KantmissBoots());
        }

        public BagOfKantmissSuit(Serial serial) : base(serial)
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
