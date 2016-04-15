using System;
using Server;

namespace Server.Items
{
    public class CookedDrumstick : Food
    {
        public override string DisplayName { get { return "cooked drumstick"; } }
        public override SatisfactionLevelType Satisfaction { get { return SatisfactionLevelType.Adequate; } }

        public override int IconItemId { get { return 5640; } }
        public override int IconItemHue { get { return 1850; } }
        public override int IconOffsetX { get { return 0; } }
        public override int IconOffsetY { get { return 10; } }

        public override int FillFactor { get { return 10; } }
        public override bool IsStackable { get { return true; } }
        public override int MaxCharges { get { return 1; } }
        public override double WeightPerCharge { get { return 1; } }

        public override bool Decays { get { return false; } }
        public override TimeSpan DecayDuration { get { return TimeSpan.FromDays(3); } }

        public override int MinStaminaRegained { get { return 20; } }
        public override int MaxStaminaRegained { get { return 40; } }

        [Constructable]
        public CookedDrumstick(): this(1)
        {
        } 

        [Constructable]
        public CookedDrumstick(int amount): base(5640)
        {
            Name = "cooked drumstick";
            Hue = 1848;

            Amount = amount;
        }

        public CookedDrumstick(Serial serial): base(serial)
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