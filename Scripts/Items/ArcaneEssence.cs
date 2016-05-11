using System;
using Server.Items;
using System.Collections;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;

namespace Server
{
    public class ArcaneEssence : Item
    {
        [Constructable]
        public ArcaneEssence(int amount): base(3844)
        {
            Name = "arcane essence";
            Hue = 2609;

            Stackable = true;
            Weight = .01;

            Amount = amount;
        }

        [Constructable]
        public ArcaneEssence(): base(3844)
        {
            Name = "arcane essence";
            Hue = 2609;

            Stackable = true;
            Weight = .01;
        }
        
        public ArcaneEssence(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
            }
        }
    }
}