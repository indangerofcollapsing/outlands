using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Server.Items;

namespace Server.Items
{
    public class AlizzaHeadband : Bandana
    {
        [Constructable]
        public AlizzaHeadband()
        {
            Name = "Alizza in Lalaland Headband [1/5]";
            Hue = 101;

            DecorativeEquipment = true;
        }

        public AlizzaHeadband(Serial serial) : base(serial)
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

    public class AlizzaShoes : Shoes
    {
        [Constructable]
        public AlizzaShoes()
        {
            Name = "Alizza in Lalaland Shoes [2/5]";
            Hue = 797;

            DecorativeEquipment = true;
        }

        public AlizzaShoes(Serial serial) : base(serial)
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

    public class AlizzaDress : PlainDress
    {
        [Constructable]
        public AlizzaDress()
        {
            Name = "Alizza in Lalaland Dress [3/5]";
            Hue = 101;

            DecorativeEquipment = true;
        }

        public AlizzaDress(Serial serial) : base(serial)
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

    public class AlizzaApron : HalfApron
    {
        [Constructable]
        public AlizzaApron()
        {
            Name = "Alizza in Lalaland Apron [4/5]";
            Hue = 1001;

            DecorativeEquipment = true;
        }

        public AlizzaApron(Serial serial) : base(serial)
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

    public class AlizzaCatLantern : CatLantern
    {
        [Constructable]
        public AlizzaCatLantern() : base(1154, 11)
        {
            Name = "Alizza in Lalaland Cat Lantern [5/5]";

            DecorativeEquipment = true;
        }

        public AlizzaCatLantern(Serial serial) : base(serial)
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

    public class BagOfAlizzaSuit : Bag
    {
        [Constructable]
        public BagOfAlizzaSuit()
        {
            Name = "a bag of Alizza in Lalaland Suit";

            DropItem(new AlizzaHeadband());
            DropItem(new AlizzaDress());
            DropItem(new AlizzaShoes());
            DropItem(new AlizzaApron());
            DropItem(new AlizzaCatLantern());

        }

        public BagOfAlizzaSuit(Serial serial) : base(serial)
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
