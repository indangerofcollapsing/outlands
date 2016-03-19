using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Items
{
    public class PirateCutlass : Cutlass
    {
        [Constructable]
        public PirateCutlass()
        {
            Name = "Pirate Cutlass [1/6]";

            DecorativeEquipment = true;
        }

        public PirateCutlass(Serial serial) : base(serial)
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

    public class PiratePants : LongPants
    {
        [Constructable]
        public PiratePants()
        {
            Name = "Pirate Pants [2/6]";
            Hue = 48;

            DecorativeEquipment = true;
        }

        public PiratePants(Serial serial) : base(serial)
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

    public class PirateHalloweenBoots : Boots
    {
        [Constructable]
        public PirateHalloweenBoots()
        {
            Name = "Pirate Boots [3/6]";

            DecorativeEquipment = true;
        }

        public PirateHalloweenBoots(Serial serial) : base(serial)
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

    public class PirateHat : TricorneHat
    {
        [Constructable]
        public PirateHat()
        {
            Name = "Pirate Hat [4/6]";
            Hue = 33;

            DecorativeEquipment = true;
        }

        public PirateHat(Serial serial) : base(serial)
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

            Name = "Pirate Hat [4/6]";

            DecorativeEquipment = true;
        }
    }

    public class PirateHalloweenShirt : FancyShirt
    {
        [Constructable]
        public PirateHalloweenShirt()
        {
            Name = "Pirate Shirt [5/6]";
            Hue = 33;

            DecorativeEquipment = true;
        }

        public PirateHalloweenShirt(Serial serial) : base(serial)
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

    public class PirateHalloweenDoublet : Doublet
    {
        [Constructable]
        public PirateHalloweenDoublet()
        {
            Name = "Pirate Doublet [6/6]";
            Hue = 48;

            DecorativeEquipment = true;
        }

        public PirateHalloweenDoublet(Serial serial) : base(serial)
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

    public class BagOfPirateSuit : Bag
    {
        [Constructable]
        public BagOfPirateSuit()
        {
            Name = "a bag of Pirate Suit";

            DropItem(new PirateHalloweenBoots());
            DropItem(new PirateCutlass());
            DropItem(new PirateHat());
            DropItem(new PiratePants());
            DropItem(new PirateHalloweenShirt());
            DropItem(new PirateHalloweenDoublet());


        }

        public BagOfPirateSuit(Serial serial) : base(serial)
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
