using System;
using Server;

namespace Server.Items
{
    public class CookedBacon : Food
    {
        public override string DisplayName { get { return "cooked bacon"; } }
        public override SatisfactionLevelType Satisfaction { get { return SatisfactionLevelType.Adequate; } }

        public override int IconItemId { get { return 2422; } }
        public override int IconItemHue { get { return 1848; } }
        public override int IconOffsetX { get { return -3; } }
        public override int IconOffsetY { get { return 7; } }

        public override int FillFactor { get { return 10; } }
        public override bool IsStackable { get { return true; } }
        public override int MaxCharges { get { return 1; } }
        public override double WeightPerCharge { get { return 1; } }

        public override bool Decays { get { return false; } }
        public override TimeSpan DecayDuration { get { return TimeSpan.FromDays(3); } }

        public override int MinStaminaRegained { get { return 20; } }
        public override int MaxStaminaRegained { get { return 40; } }

        [Constructable]
        public CookedBacon(): this(1)
        {
        } 

        [Constructable]
        public CookedBacon(int amount): base(2422)
        {
            Name = "cooked bacon";
            Hue = 1848;

            Amount = amount;

            //TEST: Not Stackable
        }

        public CookedBacon(Serial serial): base(serial)
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