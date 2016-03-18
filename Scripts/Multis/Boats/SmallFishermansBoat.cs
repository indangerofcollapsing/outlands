using System;
using Server;
using Server.Items;
using System.Collections;
using System.Collections.Generic;

namespace Server.Multis
{
    public class SmallFishermansBoat : SmallBoat
    {
        public override BaseBoatDeed BoatDeed { get { return new SmallFishermansBoatDeed(); } }
        
        public override List<Point3D> m_CannonLocations()
        {
            List<Point3D> list = new List<Point3D>();

            return list;
        }

        [Constructable]
        public SmallFishermansBoat()
        {
            Name = "a small fisherman's boat";
        }

        public SmallFishermansBoat(Serial serial): base(serial)
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

    public class SmallFishermansBoatDeed : BaseBoatDeed
    {
        public override BaseBoat Boat { get { return new SmallFishermansBoat(); } }
        private static int m_BoatId = 0x4000;

        public override int DoubloonCost { get { return 0; } }
        public override double DoubloonMultiplier { get { return 1; } }

        [Constructable]
        public SmallFishermansBoatDeed(): base(m_BoatId, Point3D.Zero)
        {
            Name = " a small fisherman's boat deed";
            PlayerClassRestricted = false;
        }

        public SmallFishermansBoatDeed(Serial serial): base(serial)
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