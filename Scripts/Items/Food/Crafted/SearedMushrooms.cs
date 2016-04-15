using System;

namespace Server.Items
{
    public class SearedMushrooms : Food
    {
        public override string DisplayName { get { return "seared mushrooms"; } }
        public override SatisfactionLevelType Satisfaction { get { return SatisfactionLevelType.Adequate; } }

        public override int IconItemId { get { return 3350; } }
        public override int IconItemHue { get { return 25; } }
        public override int IconOffsetX { get { return -1; } }
        public override int IconOffsetY { get { return 5; } }

        public override int FillFactor { get { return 10; } }
        public override bool IsStackable { get { return true; } }
        public override int MaxCharges { get { return 1; } }
        public override double WeightPerCharge { get { return 1; } }

        public override bool Decays { get { return false; } }
        public override TimeSpan DecayDuration { get { return TimeSpan.FromDays(3); } }

        public override int MinStaminaRegained { get { return 20; } }
        public override int MaxStaminaRegained { get { return 40; } }

        [Constructable]
        public SearedMushrooms(): this(1)
        {
        }

        [Constructable]
        public SearedMushrooms(int amount): base(3350)
        {
            Name = "seared mushrooms";
            Hue = 1847;

            Amount = amount;
        }

        public SearedMushrooms(Serial serial): base(serial)
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