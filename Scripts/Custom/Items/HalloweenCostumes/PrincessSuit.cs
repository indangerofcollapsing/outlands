using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Items;

namespace Server.Items
{
    public class PrincessBonnet : Bonnet
    {
        [Constructable]
        public PrincessBonnet()
        {
            Name = "Princess Bonnet [1/5]";
            Hue = 2631;

            DecorativeEquipment = true;
        }

        public PrincessBonnet(Serial serial) : base(serial)
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

    public class PrincessDress : FancyDress
    {
        [Constructable]
        public PrincessDress()
        {
            Name = "Princess Dress [2/5]";
            Hue = 2631;

            DecorativeEquipment = true;
        }

        public PrincessDress(Serial serial) : base(serial)
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

    public class PrincessBoots : Boots
    {
        [Constructable]
        public PrincessBoots()
        {
            Name = "Princess Boots [3/5]";
            Hue = 2631;

            DecorativeEquipment = true;
        }

        public PrincessBoots(Serial serial) : base(serial)
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

    public class PrincessNecklace : Necklace
    {
        [Constructable]
        public PrincessNecklace()
        {
            Name = "Princess Family Jewels Neckalce [4/5]";

            DecorativeEquipment = true;
        }

        public PrincessNecklace(Serial serial) : base(serial)
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

    public class PrincessRing : GoldRing
    {
        [Constructable]
        public PrincessRing()
        {
            Name = "Princess Family Jewels Ring [5/5]";

            DecorativeEquipment = true;
        }

        public PrincessRing(Serial serial) : base(serial)
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

    public class BagOfPrincessSuit : Bag
    {
        [Constructable]
        public BagOfPrincessSuit()
        {
            Name = "a bag of Princess Suit";

            DropItem(new PrincessBonnet());
            DropItem(new PrincessDress());
            DropItem(new PrincessBoots());
            DropItem(new PrincessNecklace());
            DropItem(new PrincessRing());


        }

        public BagOfPrincessSuit(Serial serial) : base(serial)
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
