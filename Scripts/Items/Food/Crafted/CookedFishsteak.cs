using System;
using Server;

namespace Server.Items
{
    public class CookedFishsteak : Food
    {
        public override string DisplayName { get { return "cooked fishsteak"; } }
        public override SatisfactionLevelType Satisfaction { get { return SatisfactionLevelType.Meagre; } }

        public override int IconItemId { get { return 2427; } }
        public override int IconItemHue { get { return 1846; } }
        public override int IconOffsetX { get { return -7; } }
        public override int IconOffsetY { get { return 11; } }

        public override int FillFactor { get { return 6; } }
        public override bool IsStackable { get { return true; } }
        public override int MaxCharges { get { return 1; } }
        public override double WeightPerCharge { get { return 1; } }

        public override bool Decays { get { return false; } }
        public override TimeSpan DecayDuration { get { return TimeSpan.FromDays(3); } }

        public override int MinStaminaRegained { get { return 10; } }
        public override int MaxStaminaRegained { get { return 20; } }

        [Constructable]
        public CookedFishsteak(): this(1)
        {
        }    

        [Constructable]
        public CookedFishsteak(int amount): base(2427)
        {
            Name = "cooked fishsteak";
            Hue = 1846;

            Amount = amount;
        }

        public CookedFishsteak(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}