using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Items
{
    class ArchitectsPen : Item
    {

        public override string DefaultName
        {
            get
            {
                return "Architect's Pen";
            }
        }

        [Constructable]
        public ArchitectsPen() : base(0xFBF) 
        {
            Hue = 1774;
        }

        public ArchitectsPen(Serial serial)
            : base(serial)
        {

        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
        }
    }
}
