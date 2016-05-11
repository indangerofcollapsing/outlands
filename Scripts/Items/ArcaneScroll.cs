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
    public class ArcaneScroll : Item
    {
        [Constructable]
        public ArcaneScroll(int amount): base(3827)
        {
            Name = "an arcane scroll";
            Hue = 2609;

            Stackable = true;
            Weight = .1;

            Amount = amount;
        }

        [Constructable]
        public ArcaneScroll(): base(3827)
        {
            Name = "an arcane scroll";
            Hue = 2609;

            Stackable = true;
            Weight = .1;
        }
        
        public ArcaneScroll(Serial serial): base(serial)
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