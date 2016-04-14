using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Items
{
    public class AmazingLadyBracelet : GoldBracelet
    {
        [Constructable]
        public AmazingLadyBracelet()
        {
            Weight = 0.1;
            Name = "Amazing Lady Bracelet [1/6]";

            DecorativeEquipment = true;
        }

        public AmazingLadyBracelet(Serial serial) : base(serial)
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

    [FlipableAttribute(0x170b, 0x170c)]
    public class AmazingLadyBoots : Boots
    {
        public override CraftResource DefaultResource { get { return CraftResource.RegularLeather; } }

        [Constructable]
        public AmazingLadyBoots()
        {
            Weight = 3.0;
            Name = "Amazing Lady Boots [2/6]";
            Hue = 38;

            DecorativeEquipment = true;
        }

        public AmazingLadyBoots(Serial serial)
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

    public class AmazingLadyHeadband : Bandana
    {
        [Constructable]
        public AmazingLadyHeadband()
        {
            Weight = 1.0;
            Name = "Amazing Lady Headband [3/6]";
            Hue = 250;

            DecorativeEquipment = true;
        }

        public AmazingLadyHeadband(Serial serial) : base(serial)
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

    public class AmazingLadySword : VikingSword
    {
        [Constructable]
        public AmazingLadySword()
        {
            Name = "Amazing Lady Sword [4/6]";

            DecorativeEquipment = true;
        }

        public AmazingLadySword(Serial serial) : base(serial)
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

    public class AmazingLadyBustier : LeatherBustier, IScissorable
    {
        public override bool AllowMaleWearer { get { return true; } }

        [Constructable]
        public AmazingLadyBustier()
        {
            Weight = 1.0;
            Name = "Amazing Lady Bustier [5/6]";
            Hue = 2124;

            DecorativeEquipment = true;
        }

        public AmazingLadyBustier(Serial serial) : base(serial)
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

    public class AmazingLadySkirt : LeatherSkirt, IScissorable
    {
        public override bool AllowMaleWearer { get { return false; } }

        [Constructable]
        public AmazingLadySkirt()
        {
            Weight = 1.0;
            Name = "Amazing Lady Skirt [6/6]";
            Hue = 38;

            DecorativeEquipment = true;
        }

        public AmazingLadySkirt(Serial serial) : base(serial)
        {
        }

        public new bool Scissor(Mobile from, Scissors scissors) { return Helpers.CutLeather(this, from); }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);

            if (Weight == 3.0)
                Weight = 1.0;
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            DecorativeEquipment = true;
        }
    }

    public class BagOfAmazingLadySuit: Bag
    {
        [Constructable]
        public BagOfAmazingLadySuit()
        {
            Name = "a bag of Amazing Lady Suit";

            DropItem(new AmazingLadyBracelet());
            DropItem(new AmazingLadySword());
            DropItem(new AmazingLadyHeadband());
            DropItem(new AmazingLadyBustier());
            DropItem(new AmazingLadySkirt());
            DropItem(new AmazingLadyBoots());
        }

        public BagOfAmazingLadySuit(Serial serial): base(serial)
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
