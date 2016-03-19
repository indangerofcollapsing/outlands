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
    public class Ghostfin : AquariumItem
    {        
        public override string DescriptionA { get { return "A Ghostfin"; } }
        public override string DescriptionB { get { return ""; } }

        public override Rarity ItemRarity { get { return Rarity.UltraRare; } }

        public override Type ItemType { get { return Type.Fish; } }

        public override int ItemId { get { return 17159; } }
        public override int ItemHue { get { return 2658; } }

        public override int MinWeight { get { return 20; } }
        public override int MaxWeight { get { return 40; } }

        [Constructable]
        public Ghostfin(): base()
        {
            Name = "a ghostfin";
        }

        public Ghostfin(Serial serial): base(serial)
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
