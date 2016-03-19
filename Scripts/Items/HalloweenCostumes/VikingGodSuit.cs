using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Server.Items
{
    [FlipableAttribute(0x1407, 0x1406)]
    public class VikingGodWarMace : WarMace
    {
        [Constructable]
        public VikingGodWarMace()
        {
            Name = "Viking God War Mace [1/5]";

            DecorativeEquipment = true;
        }

        public VikingGodWarMace(Serial serial) : base(serial)
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

    public class VikingGodCloak : Cloak
    {

        [Constructable]
        public VikingGodCloak()
        {
            Name = "Viking God Cloak [2/5]";
            Hue = 38;

            DecorativeEquipment = true;
        }

        public VikingGodCloak(Serial serial)
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

    public class VikingGodChest : RingmailChest
    {
        [Constructable]
        public VikingGodChest()
        {
            Name = "Viking God Chest [3/5]";
            Hue = 2406;

            DecorativeEquipment = true;
        }

        public VikingGodChest(Serial serial) : base(serial)
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

    public class VikingGodHelm : NorseHelm
    {
        [Constructable]
        public VikingGodHelm()
        {
            Name = "Viking God Helm [4/5]";

            DecorativeEquipment = true;
        }

        public VikingGodHelm(Serial serial) : base(serial)
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


    public class VikingGodLegs : RingmailLegs
    {
        [Constructable]
        public VikingGodLegs()
        {
            Name = "Viking God Legs [5/5]";
            Hue = 2406;

            DecorativeEquipment = true;
        }

        public VikingGodLegs(Serial serial) : base(serial)
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

    public class BagOfVikingGodSuit : Bag
    {
        [Constructable]
        public BagOfVikingGodSuit()
        {
            Name = "a bag of Viking God Suit";

            DropItem(new VikingGodWarMace());
            DropItem(new VikingGodCloak());
            DropItem(new VikingGodHelm());
            DropItem(new VikingGodLegs());
            DropItem(new VikingGodChest());
        }

        public BagOfVikingGodSuit(Serial serial) : base(serial)
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

