using System;

namespace Server.Items
{
    public class CookedCrab : Food
    {
        public override string DisplayName { get { return "cooked crab"; } }
        public override SatisfactionLevelType Satisfaction { get { return SatisfactionLevelType.Appetizing; } }

        public override int IconItemId { get { return ItemID; } }
        public override int IconItemHue { get { return Hue; } }
        public override int IconOffsetX { get { return 0; } }
        public override int IconOffsetY { get { return 0; } }

        public override int FillFactor { get { return 12; } }
        public override bool IsStackable { get { return true; } }
        public override int MaxCharges { get { return 1; } }
        public override double WeightPerCharge { get { return 1; } }

        public override bool Decays { get { return false; } }
        public override TimeSpan DecayDuration { get { return TimeSpan.FromDays(7); } }

        public override int MinStaminaRegained { get { return 20; } }
        public override int MaxStaminaRegained { get { return 40; } }

        [Constructable]
        public CookedCrab(): this(1)
        {
        }

        [Constructable]
        public CookedCrab(int amount): base( 0x44D1)
        {
            Amount = amount;
        }

        public CookedCrab(Serial serial): base(serial)
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