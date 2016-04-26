using System;
using Server;
using Server.Items;
using System.Collections;
using System.Collections.Generic;

namespace Server.Multis
{
    public class LargeFishermansBoat : LargeBoat
    {        
        public override BaseBoatDeed BoatDeed { get { return new LargeBoatDeed(); } }
        
        [Constructable]
        public LargeFishermansBoat()
        {
            Name = "a large fisherman's boat";
        }

        public LargeFishermansBoat(Serial serial): base(serial)
        {
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }
    }

    public class LargeFishermansBoatDeed : BaseBoatDeed
    {
        public override BaseBoat Boat { get { return new LargeFishermansBoat(); } }
        private static int m_BoatId = 0x4010;

        public override int DoubloonCost { get { return 250; } }
        public override double DoubloonMultiplier { get { return 2; } }

        [Constructable]
        public LargeFishermansBoatDeed(): base(m_BoatId, new Point3D(0, -1, 0))
        {
            Name = "a large fisherman's boat deed";
        }

        public LargeFishermansBoatDeed(Serial serial): base(serial)
        {
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }
    }
}