using System;
using Server;
using Server.Items;
using Server.Targeting;
using Server.Network;

namespace Server.Items
{
    public class BasicFertilizer : SoilItem
    {
        public override double MaxSoilQuality { get { return 30.0; } }
        public override double SoilQualityIncrease { get { return 10.0; } }

        public override string SoilItemName { get { return "basic fertilizer"; } }
        public override int SoilItemId { get { return 4154; } }
        public override int SoilItemHue { get { return 0; } }

        public override int MaxCharges { get { return 5; } }
        
        [Constructable]
        public BasicFertilizer(): base()
        {
        }

        public BasicFertilizer(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}