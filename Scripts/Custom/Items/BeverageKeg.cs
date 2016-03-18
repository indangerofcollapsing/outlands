using Server.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Items
{
    class BeverageKeg : BaseBeverage
    {
        public override string DefaultName
        {
            get
            {
                return "a beverage keg";
            }
        }

        public override int MaxQuantity
        {
            get { return 100; }
        }

        public override int ComputeItemID()
        {
            return 0x1940;
        }

        [Constructable]
        public BeverageKeg()
            : base()
        {
        }

        public BeverageKeg(Serial serial)
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
        }
    }
}
