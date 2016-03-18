using System;
using Server;
using Server.Items;
using System.Collections;
using System.Collections.Generic;

namespace Server.Multis
{
    public class MediumFishermansBoat : MediumBoat
    {        
        public override BaseBoatDeed BoatDeed { get { return new MediumFishermansBoatDeed(); } }
        
        public override List<Point3D> m_CannonLocations()
        {
            List<Point3D> list = new List<Point3D>();

            return list;
        }

        [Constructable]
        public MediumFishermansBoat()
        {
        }

        public MediumFishermansBoat(Serial serial): base(serial)
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

    public class MediumFishermansBoatDeed : BaseBoatDeed
    {
        public override BaseBoat Boat { get { return new MediumFishermansBoat(); } }
        private static int m_BoatId = 0x4008;

        public override int DoubloonCost { get { return 50; } }
        public override double DoubloonMultiplier { get { return 2; } }

        [Constructable]
        public MediumFishermansBoatDeed(): base(m_BoatId, Point3D.Zero)
        {
            Name = "a medium fisherman's boat deed";
        }

        public MediumFishermansBoatDeed(Serial serial): base(serial)
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