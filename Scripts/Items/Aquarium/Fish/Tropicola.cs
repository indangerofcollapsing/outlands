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
    public class Tropicola : AquariumItem
    {        
        public override string DescriptionA { get { return "A Tropicola"; } }
        public override string DescriptionB { get { return ""; } }

        public override Rarity ItemRarity { get { return Rarity.Uncommon; } }

        public override Type ItemType { get { return Type.Fish; } }

        public override int ItemId { get { return 17606; } }
        public override int ItemHue { get { return 2563; } }

        public override int OffsetX { get { return 0; } }
        public override int OffsetY { get { return -5; } }

        public override int MinWeight { get { return 30; } }
        public override int MaxWeight { get { return 60; } }

        [Constructable]
        public Tropicola(): base()
        {
            Name = "a Tropicola";
        }

        public Tropicola(Serial serial): base(serial)
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
