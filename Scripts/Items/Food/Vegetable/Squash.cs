using System;

namespace Server.Items
{
    public class Squash : Food
    {
        public override string DisplayName { get { return "squash"; } }
        public override SatisfactionLevelType Satisfaction { get { return SatisfactionLevelType.Paltry; } }

        public override int IconItemId { get { return 3186; } }
        public override int IconItemHue { get { return Hue; } }
        public override int IconOffsetX { get { return -9; } }
        public override int IconOffsetY { get { return 2; } }

        public override int FillFactor { get { return 3; } }
        public override bool IsStackable { get { return true; } }
        public override int MaxCharges { get { return 1; } }
        public override double WeightPerCharge { get { return .2; } }

        public override bool Decays { get { return false; } }
        public override TimeSpan DecayDuration { get { return TimeSpan.FromDays(3); } }

        public override int MinStaminaRegained { get { return 5; } }
        public override int MaxStaminaRegained { get { return 10; } }

        [Constructable]
        public Squash(): this(1)
        {
        }

        [Constructable]
        public Squash(int amount): base(3186)
        {
            Name = "squash";

            Weight = 1.0;
            Amount = amount;
        }

        public Squash(Serial serial): base(serial)
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