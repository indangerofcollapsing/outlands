using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Items
{

    public class PilgrimFloppyHat : FloppyHat
    {
        [Constructable]
        public PilgrimFloppyHat()
        {
            Name = "Pilgrim Floppy Hat [1/3]";

            DecorativeEquipment = true;

        }

        public PilgrimFloppyHat(Serial serial) : base(serial)
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

    public class PilgrimHalfApron : HalfApron
    {
        [Constructable]
        public PilgrimHalfApron()
        {
            Name = "Pilgrim Half Apron [2/3]";

            DecorativeEquipment = true;
        }

        public PilgrimHalfApron(Serial serial) : base(serial)
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

    public class PilgrimBoots: Boots
    {
        [Constructable]
        public PilgrimBoots()
        {
            Name = "Pilgrim Boots [3/3]";

            DecorativeEquipment = true;
        }

        public PilgrimBoots(Serial serial) : base(serial)
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

    public class BagOfPilgrimSuit : Bag
    {
        [Constructable]
        public BagOfPilgrimSuit()
        {
            Name = "a bag of Pilgrim Suit";

            DropItem(new PilgrimBoots());
            DropItem(new PilgrimFloppyHat());
            DropItem(new PilgrimHalfApron());
        }

        public BagOfPilgrimSuit(Serial serial) : base(serial)
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
