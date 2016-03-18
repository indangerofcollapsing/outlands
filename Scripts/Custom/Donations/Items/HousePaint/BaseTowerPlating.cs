using System;
using System.Collections.Generic;
using Server.Targeting;
using Server.Mobiles;
using Server.Multis;

namespace Server.Items
{
    public class BaseTowerPlating : BaseHousePaint
    {
        public BaseTowerPlating()
            : base(7153)
        {
        }

        public BaseTowerPlating(Serial serial)
            : base(serial)
        {
        }

        public override bool CanPaint(BaseHouse house) 
        {
            return ((house is SmallTower || house is Tower) &&
                    base.CanPaint(house));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); //version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }
}
