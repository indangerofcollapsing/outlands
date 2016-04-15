using System;
using Server;

namespace Server.Items
{
    public class GardenSalad : Food
    {
        public override string DisplayName { get { return "garden salad"; } }
        public override SatisfactionLevelType Satisfaction { get { return SatisfactionLevelType.Meagre; } }

        public override int IconItemId { get { return 10316; } }
        public override int IconItemHue { get { return 2209; } }
        public override int IconOffsetX { get { return 0; } }
        public override int IconOffsetY { get { return 1; } }

        public override int FillFactor { get { return 6; } }
        public override bool IsStackable { get { return true; } }
        public override int MaxCharges { get { return 1; } }
        public override double WeightPerCharge { get { return 1; } }

        public override bool Decays { get { return false; } }
        public override TimeSpan DecayDuration { get { return TimeSpan.FromDays(3); } }

        public override int MinStaminaRegained { get { return 10; } }
        public override int MaxStaminaRegained { get { return 20; } }

        [Constructable]
        public GardenSalad(): this(1)
        {
        } 

        [Constructable]
        public GardenSalad(int amount): base(10316)
        {
            Name = "garden salad";
            Hue = 2209;

            Amount = amount;
        }

        public GardenSalad(Serial serial): base(serial)
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