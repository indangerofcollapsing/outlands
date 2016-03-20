using Server.Multis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Mobiles;

namespace Server.Items
{
    class DecoyEasterEgg : Item
    {
        [Constructable]
        public DecoyEasterEgg(): base(0x9B5)
        {
            Name = "easter eggs";

            Weight = 1;
            Hue = Utility.RandomMinMax(2, 362);
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);
            
            if (from.InRange(this.GetWorldLocation(), 1))
            {
                Effects.PlaySound(Location, Map, 0x134);

                from.SendMessage("These appear to be some sort of decoy, and they crumble in your hands...");

                Delete();
            }
            else
                from.SendMessage("You are too far away to use that.");
        }

        public DecoyEasterEgg(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);          
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
