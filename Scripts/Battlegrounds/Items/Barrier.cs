using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Battlegrounds.Items
{
    public class Barrier : Item
    {
        public Barrier(int itemId)
            : base(itemId)
        {
            Movable = false;
            Name = "Spawn Barrier - Game starting shortly.";
            Hue = 1;
        }

        public Barrier()
            : this(16669)
        {

        }

        public Barrier(Serial serial)
            : base(serial)
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
            this.Delete(); // no need to keep these around
        }
    }
}
