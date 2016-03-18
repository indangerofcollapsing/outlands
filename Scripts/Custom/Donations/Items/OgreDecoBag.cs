using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class OgreDecoBag : Bag
    {
        [Constructable]
        public OgreDecoBag()
        {
            Name = "Ogre's Bag";
            PackItems();
        }

        public void PackItems()
        {
            DropItem(new GruesomeStandardArtifact());
            DropItem(new SkullPole());
            DropItem(new Item(4338)); // garbage
            DropItem(new Item(541)); // palisade
            DropItem(new Item(2420)); // hanging cauldron

        }

        public OgreDecoBag(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }
}
