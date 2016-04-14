using System;

namespace Server.Items
{
    public class RoastPig : Food
    {
        public override string DisplayName { get { return "roast pig"; } }
        public override SatisfactionLevelType Satisfaction { get { return SatisfactionLevelType.Delectable; } }

        public override int IconItemId { get { return ItemID; } }
        public override int IconItemHue { get { return Hue; } }
        public override int IconOffsetX { get { return 0; } }
        public override int IconOffsetY { get { return 0; } }

        public override int FillFactor { get { return 30; } }
        public override bool IsStackable { get { return false; } }
        public override int MaxCharges { get { return 5; } }
        public override double WeightPerCharge { get { return 1; } }

        public override bool Decays { get { return true; } }
        public override TimeSpan DecayDuration { get { return TimeSpan.FromDays(3); } }

        public override int MinStaminaRegained { get { return 50; } }
        public override int MaxStaminaRegained { get { return 100; } }
        
        [Constructable]
        public RoastPig(): base(2491)
        {
        }

        public RoastPig(Serial serial): base(serial)
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