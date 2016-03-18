using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Items
{
    public class SkeletonHead : BoneHelm, IScissorable
    {
        [Constructable]
        public SkeletonHead()
        {
            Weight = 3.0;
            Name = "Skeleton Head [1/4]";
            Hue = 2407;

            DecorativeEquipment = true;
        }

        public SkeletonHead(Serial serial) : base(serial)
        {
        }

        public new bool Scissor(Mobile from, Scissors scissors) { return Helpers.CutLeather(this, from); }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            DecorativeEquipment = true;
        }
    }

    public class SkeletonChest : BoneChest, IScissorable
    {
        [Constructable]
        public SkeletonChest()
        {
            Weight = 6.0;
            Hue = 2407;
            Name = "Skeleton Chest [2/4]";

            DecorativeEquipment = true;
        }

        public SkeletonChest(Serial serial) : base(serial)
        {
        }

        public new bool Scissor(Mobile from, Scissors scissors) { return Helpers.CutLeather(this, from); }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            DecorativeEquipment = true;
        }
    }

    public class SkeletonLegs : BoneLegs, IScissorable
    {
        [Constructable]
        public SkeletonLegs()
        {
            Weight = 3.0;
            Hue = 2407;
            Name = "Skeleton Legs [3/4]";

            DecorativeEquipment = true;
        }

        public SkeletonLegs(Serial serial) : base(serial)
        {
        }

        public new bool Scissor(Mobile from, Scissors scissors) { return Helpers.CutLeather(this, from); }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            DecorativeEquipment = true;
        }
    }

    public class SkeletonArms : BoneArms, IScissorable
    {
        [Constructable]
        public SkeletonArms()
        {
            //IPY weight
            Weight = 1.0;
            Hue = 2407;
            Name = "Skeleton Arms [4/4]";

            DecorativeEquipment = true;
        }

        public SkeletonArms(Serial serial) : base(serial)
        {
        }

        public new bool Scissor(Mobile from, Scissors scissors) { return Helpers.CutLeather(this, from); }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            DecorativeEquipment = true;
        }
    }

    public class BagOfSkeletonSuit : Bag
    {
        [Constructable]
        public BagOfSkeletonSuit()
        {
            Name = "a bag of Skeleton Suit";

            DropItem(new SkeletonHead());
            DropItem(new SkeletonArms());
            DropItem(new SkeletonChest());
            DropItem(new SkeletonLegs());
        }

        public BagOfSkeletonSuit(Serial serial) : base(serial)
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
