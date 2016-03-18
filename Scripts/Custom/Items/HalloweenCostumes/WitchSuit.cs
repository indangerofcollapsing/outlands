using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Custom.Townsystem;

namespace Server.Items
{

    public class WitchHat : WizardsHat
    {
        [Constructable]
        public WitchHat()
        {
            Name = "Witch Hat [1/3]";
            Hue = 988;

            DecorativeEquipment = true;
        }

        public WitchHat(Serial serial) : base(serial)
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

    [FlipableAttribute(0x170d, 0x170e)]
    public class WitchSandals : Sandals
    {
        public override CraftResource DefaultResource { get { return CraftResource.RegularLeather; } }

        [Constructable]
        public WitchSandals()
        {
            Name = "Witch Sandals [2/3]";
            Hue = 2424;

            DecorativeEquipment = true;
        }

        public WitchSandals(Serial serial)
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

    public class WitchSetRobe : Robe
    {
        [Constructable]
        public WitchSetRobe()
        {
            Name = "Witch Robe [3/3]";
            Hue = 988;

            DecorativeEquipment = true;
        }

        public WitchSetRobe(Serial serial) : base(serial)
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

    public class BagOfWitchSuit: Bag
    {
        [Constructable]
        public BagOfWitchSuit()
        {
            Name = "a bag of Witch Suit";

            DropItem(new WitchSetRobe());
            DropItem(new WitchSandals());
            DropItem(new WitchHat());
        }

        public BagOfWitchSuit(Serial serial): base(serial)
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
