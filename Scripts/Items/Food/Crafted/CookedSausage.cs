using System;
using Server;

namespace Server.Items
{
    public class CookedSausage : Food
    {
        public override string DisplayName { get { return "cooked sausage"; } }
        public override SatisfactionLevelType Satisfaction { get { return SatisfactionLevelType.Adequate; } }

        public override int IconItemId { get { return 2496; } }
        public override int IconItemHue { get { return 1848; } }
        public override int IconOffsetX { get { return 2; } }
        public override int IconOffsetY { get { return 8; } }

        public override int FillFactor { get { return 10; } }
        public override bool IsStackable { get { return true; } }
        public override int MaxCharges { get { return 1; } }
        public override double WeightPerCharge { get { return 1; } }

        public override bool Decays { get { return false; } }
        public override TimeSpan DecayDuration { get { return TimeSpan.FromDays(3); } }

        public override int MinStaminaRegained { get { return 20; } }
        public override int MaxStaminaRegained { get { return 40; } }

        [Constructable]
        public CookedSausage(): this(1)
        {
        } 

        [Constructable]
        public CookedSausage(int amount): base(2496)
        {
            Name = "cooked sausage";
            Hue = 1848;

            Amount = amount;
        }
        
        public CookedSausage(Serial serial): base(serial)
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