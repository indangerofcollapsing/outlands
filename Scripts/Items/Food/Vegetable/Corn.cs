using System;

namespace Server.Items
{
    public class Corn : Food
    {
        public override string DisplayName { get { return "corn"; } }
        public override SatisfactionLevelType Satisfaction { get { return SatisfactionLevelType.Paltry; } }

        public override int IconItemId { get { return 3199; } }
        public override int IconItemHue { get { return Hue; } }
        public override int IconOffsetX { get { return -17; } }
        public override int IconOffsetY { get { return 8; } }

        public override int FillFactor { get { return 3; } }
        public override bool IsStackable { get { return true; } }
        public override int MaxCharges { get { return 1; } }
        public override double WeightPerCharge { get { return .2; } }

        public override bool Decays { get { return false; } }
        public override TimeSpan DecayDuration { get { return TimeSpan.FromDays(3); } }

        public override int MinStaminaRegained { get { return 5; } }
        public override int MaxStaminaRegained { get { return 10; } }

        [Constructable]
        public Corn(): this(1)
        {
        }

        [Constructable]
        public Corn(int amount): base(3199)
        {
            Name = "corn";

            Amount = amount;
        }

        public Corn(Serial serial): base(serial)
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