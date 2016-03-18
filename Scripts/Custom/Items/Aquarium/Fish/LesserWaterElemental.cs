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
    public class LesserWaterElemental : AquariumItem
    {        
        public override string DescriptionA { get { return "A Lesser Water Elemental"; } }
        public override string DescriptionB { get { return ""; } }

        public override Rarity ItemRarity { get { return Rarity.UltraRare; } }

        public override Type ItemType { get { return Type.Fish; } }

        public override int ItemId { get { return 8459; } }
        public override int ItemHue { get { return 0; } }

        public override int OffsetX { get { return -10; } }
        public override int OffsetY { get { return -10; } }

        public override int MinWeight { get { return 100; } }
        public override int MaxWeight { get { return 200; } }

        [Constructable]
        public LesserWaterElemental(): base()
        {
            Name = "a lesser water elemental";
        }

        public LesserWaterElemental(Serial serial): base(serial)
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
