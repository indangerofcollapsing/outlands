using System;
using Server;
using Server.Items;
using System.Collections;
using System.Collections.Generic;

namespace Server.Multis
{
    public class GalleonFishermansBoat : GalleonBoat
    {
        public override BaseBoatDeed BoatDeed { get { return new GalleonFishermansBoatDeed(); } }
        
        [Constructable]
        public GalleonFishermansBoat()
        {
            Name = "a fisherman's galleon";
        }

        public GalleonFishermansBoat(Serial serial): base(serial)
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

    public class GalleonFishermansBoatDeed : BaseBoatDeed
    {
        public override BaseBoat Boat { get { return new GalleonFishermansBoat(); } }

        public override int DoubloonCost { get { return 2500; } }
        public override double DoubloonMultiplier { get { return 5; } }

        [Constructable]
        public GalleonFishermansBoatDeed(): base(0x404C, new Point3D(0, -1, 0))
        {
            Name = "a fisherman's galleon deed";
        }

        public GalleonFishermansBoatDeed(Serial serial): base(serial)
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