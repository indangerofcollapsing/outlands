using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using Server.Multis;
using Server.Engines.Craft;
using Server.ContextMenus;

namespace Server.Items
{
    public class TomeOfInfiniteWisdom : Runebook
    {
        [Constructable]
        public TomeOfInfiniteWisdom() : base(3)
        {
            LootType = LootType.Blessed;
            Hue = 0x97E;
        }

        public TomeOfInfiniteWisdom(Serial serial) : base(serial)
		{
		}

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            int maxSlots = 3;
            if (dropped is RecallRune)
            {
                return Entries.Count < maxSlots ? base.OnDragDrop(from, dropped) : false;
            }
            else
            {
                return base.OnDragDrop(from, dropped);
            }
        }

        public override string DefaultName
        {
            get
            {
                return "a tome of infinite wisdom";
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            LootType = LootType.Blessed;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
        }
    }
}
