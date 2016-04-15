using System;

namespace Server.Items
{
    public class Watermelon : Food
    {
        public override string DisplayName { get { return "watermelon"; } }
        public override SatisfactionLevelType Satisfaction { get { return SatisfactionLevelType.Paltry; } }

        public override int IconItemId { get { return 3164; } }
        public override int IconItemHue { get { return Hue; } }
        public override int IconOffsetX { get { return 8; } }
        public override int IconOffsetY { get { return 7; } }

        public override int FillFactor { get { return 6; } }
        public override bool IsStackable { get { return true; } }
        public override int MaxCharges { get { return 1; } }
        public override double WeightPerCharge { get { return 1; } }

        public override bool Decays { get { return false; } }
        public override TimeSpan DecayDuration { get { return TimeSpan.FromDays(3); } }

        public override int MinStaminaRegained { get { return 10; } }
        public override int MaxStaminaRegained { get { return 20; } }

        [Constructable]
        public Watermelon(): this(1)
        {
        }

        [Constructable]
        public Watermelon(int amount): base(3164)
        {
            Name = "watermelon";

            Amount = amount;
        }

        public Watermelon(Serial serial): base(serial)
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