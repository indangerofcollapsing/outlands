using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Items;

namespace Server.Items
{
    [Flipable(0x1537, 0x1538)]
    public class GirlScoutKilt : Kilt
    {
        [Constructable]
        public GirlScoutKilt()
        {
            Name = "Girl Scout Kilt [1/5]";
            Hue = 2967;

            DecorativeEquipment = true;
        }

        public GirlScoutKilt(Serial serial) : base(serial)
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

    public class GirlScoutShirt : FancyShirt
    {
        [Constructable]
        public GirlScoutShirt()
        {
            Name = "Girl Scout Shirt [2/5]";
            Hue = 549;

            DecorativeEquipment = true;
        }

        public GirlScoutShirt(Serial serial) : base(serial)
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

    public class GirlScoutSash : BodySash
    {
        [Constructable]
        public GirlScoutSash()
        {
            Name = "Girl Scout Sash [3/5]";
            Hue = 2967;

            DecorativeEquipment = true;
        }

        public GirlScoutSash(Serial serial) : base(serial)
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

    public class GirlScoutBonnet : Bonnet
    {
        [Constructable]
        public GirlScoutBonnet()
        {
            Name = "Girl Scout Cap [4/5]";
            Hue = 2967;

            DecorativeEquipment = true;
        }

        public GirlScoutBonnet(Serial serial) : base(serial)
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

    public class GirlScoutBoots : Boots
    {
        [Constructable]
        public GirlScoutBoots()
        {
            Name = "Girl Scout Boots [5/5]";
            Hue = 549;

            DecorativeEquipment = true;
        }

        public GirlScoutBoots(Serial serial) : base(serial)
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

    public class BagOfGirlScoutSuit : Bag
    {
        [Constructable]
        public BagOfGirlScoutSuit()
        {
            Name = "a bag of Girl Scout Suit";

            DropItem(new GirlScoutBonnet());
            DropItem(new GirlScoutSash());
            DropItem(new GirlScoutBoots());
            DropItem(new GirlScoutShirt());
            DropItem(new GirlScoutKilt());

        }

        public BagOfGirlScoutSuit(Serial serial) : base(serial)
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
