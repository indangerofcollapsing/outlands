using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public class BagOfCraftingComponents : Bag
    {
        [Constructable]
        public BagOfCraftingComponents(): this(50)
        {
        }

        [Constructable]
        public BagOfCraftingComponents(int amount)
        {
            DropItem(new BluecapMushroom(amount));
            DropItem(new CockatriceEgg(amount));
            DropItem(new Creepervine(amount));
            DropItem(new FireEssence(amount));
            DropItem(new Ghostweed(amount));
            DropItem(new GhoulHide(amount));
            DropItem(new LuniteHeart(amount));
            DropItem(new ObsidianShard(amount));
            DropItem(new Quartzstone(amount));
            DropItem(new ShatteredCrystal(amount));
            DropItem(new Snakeskin(amount));
            DropItem(new TrollFat(amount));
        }

        public BagOfCraftingComponents(Serial serial): base(serial)
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