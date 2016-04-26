using System;
using Server;
using Server.Items;
using System.Collections;
using System.Collections.Generic;

namespace Server.Multis
{
    public class CarrackFishermansBoat : CarrackBoat
    {
        public override BaseBoatDeed BoatDeed { get { return new CarrackFishermansBoatDeed(); } }
        
        [Constructable]
        public CarrackFishermansBoat()
        {
            Name = "a fisherman's carrack";
        }

        public CarrackFishermansBoat(Serial serial): base(serial)
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

    public class CarrackFishermansBoatDeed : BaseBoatDeed
    {
        public override BaseBoat Boat { get { return new CarrackFishermansBoat(); } }

        public override int DoubloonCost { get { return 1000; } }
        public override double DoubloonMultiplier { get { return 4; } }

        [Constructable]
        public CarrackFishermansBoatDeed(): base(0x4052, new Point3D(0, -1, 0))
        {
            Name = "a fisherman's carrack deed";
        }

        public CarrackFishermansBoatDeed(Serial serial): base(serial)
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