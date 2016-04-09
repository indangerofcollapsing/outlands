using System;
using Server;
using Server.Items;
using Server.Targeting;
using Server.Network;

namespace Server.Items
{
    public class RawDirt : SoilEnhancer
    {
        public override double MaxSoilQuality { get { return 20.0; } }
        public override double SoilQualityIncrease { get { return 5.0; } }
        public override SoilEnhancerQualityType QualityType { get { return SoilEnhancerQualityType.Low; } }

        public override string SoilItemName { get { return "raw dirt"; } }
        public override int SoilItemId { get { return 2323; } }
        public override int SoilItemHue { get { return 0; } }

        public override int MaxCharges { get { return 2; } }
        
        [Constructable]
        public RawDirt(): base()
        {
        }

        public RawDirt(Serial serial): base(serial)
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