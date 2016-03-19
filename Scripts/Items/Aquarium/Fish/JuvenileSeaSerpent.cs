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
    public class JuvenileSeaSerpent : AquariumItem
    {        
        public override string DescriptionA { get { return "A Juvenile Sea Serpent"; } }
        public override string DescriptionB { get { return ""; } }

        public override Rarity ItemRarity { get { return Rarity.UltraRare; } }

        public override Type ItemType { get { return Type.Fish; } }

        public override int ItemId { get { return 9665; } }
        public override int ItemHue { get { return 0; } }

        public override int OffsetX { get { return -5; } }
        public override int OffsetY { get { return 0; } }

        public override int MinWeight { get { return 75; } }
        public override int MaxWeight { get { return 150; } }

        [Constructable]
        public JuvenileSeaSerpent(): base()
        {
            Name = "a juvenile sea serpent";
        }

        public JuvenileSeaSerpent(Serial serial): base(serial)
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
