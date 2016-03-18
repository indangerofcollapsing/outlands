using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using Server.Network;
using Server.Gumps;
using Server.ContextMenus;

namespace Server.Items
{
    public class SunkenOrnamentalVase : AquariumItem
    {
        public override string DescriptionA { get { return "A Sunken Ornamental Vase"; } }
        public override string DescriptionB { get { return ""; } }

        public override Rarity ItemRarity { get { return Rarity.UltraRare; } }

        public override Type ItemType { get { return Type.Decoration; } }

        public override int OffsetX { get { return -15; } }
        public override int OffsetY { get { return 5; } }

        public override int ItemId { get { return 11739; } }
        public override int ItemHue { get { return 2600; } }

        [Constructable]
        public SunkenOrnamentalVase(): base()
        {
            Name = "a sunken ornamental vase";    

            Weight = 10;
        }

        public SunkenOrnamentalVase(Serial serial): base(serial)
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
