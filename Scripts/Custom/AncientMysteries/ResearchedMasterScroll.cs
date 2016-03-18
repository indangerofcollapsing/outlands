using System;
using Server.Network;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Targeting;
using Server.Spells;
using Server.Gumps;

namespace Server.Custom
{
    public class ResearchedMasterScroll : Item
    {
        [Constructable]
        public ResearchedMasterScroll(): base(8002)
        {
            Name = "a master spell scroll";
            Hue = 2550;

            Weight = 1.0;
        }

        public ResearchedMasterScroll(Serial serial): base(serial)
        {
        }
        
        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
            
            LabelTo(from, "(researched)");
        }        

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
                from.SendMessage("This must be in your pack in order to use it.");
            else
            {
                from.SendMessage("This researched master scroll may now be applied towards an ancient mystery scroll.");
                return;                
            }
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
        }
    }    
}
