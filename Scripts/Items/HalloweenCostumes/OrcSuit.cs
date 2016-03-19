using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Items
{
    public class OrcChest : RingmailChest
    {
        [Constructable]
        public OrcChest()
        {
            Name = "Orc Chest [1/4]";

            DecorativeEquipment = true;

        }

        public OrcChest(Serial serial) : base(serial)
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

    public class OrcBoots: Boots
    {
        [Constructable]
        public OrcBoots()
        {
            Name = "Orc Boots [2/4]";

            DecorativeEquipment = true;

        }

        public OrcBoots(Serial serial) : base(serial)
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
            Name = "Orc Boots [2/4]";

            DecorativeEquipment = true;
        }
    }

    public class OrcAxe : LargeBattleAxe
    {
        [Constructable]
        public OrcAxe()
        {
            Name = "Orc Axe [3/4]";

            DecorativeEquipment = true;
        }

        public OrcAxe(Serial serial) : base(serial)
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

    public class OrcHolloweenMask : OrcMask
    {
        [Constructable]
        public OrcHolloweenMask()
        {
            Name = "Orc Mask [4/4]";

            DecorativeEquipment = true;

        }

        public OrcHolloweenMask(Serial serial) : base(serial)
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

    public class BagOfOrcSuit : Bag
    {
        [Constructable]
        public BagOfOrcSuit()
        {
            Name = "a bag of Orc Suit";

            DropItem(new OrcHolloweenMask());
            DropItem(new OrcChest());
            DropItem(new OrcBoots());
            DropItem(new OrcAxe());
            //DropItem(new OrcHalloweenHelm());


        }

        public BagOfOrcSuit(Serial serial) : base(serial)
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
