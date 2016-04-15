using System;
using Server;

namespace Server.Items
{
    public class CookedFishFillet : Food
    {
        public override string DisplayName { get { return "cooked fish fillet"; } }
        public override SatisfactionLevelType Satisfaction { get { return SatisfactionLevelType.Adequate; } }

        public override int IconItemId { get { return 7708; } }
        public override int IconItemHue { get { return 2968; } }
        public override int IconOffsetX { get { return 3; } }
        public override int IconOffsetY { get { return -4; } }

        public override int FillFactor { get { return 10; } }
        public override bool IsStackable { get { return true; } }
        public override int MaxCharges { get { return 1; } }
        public override double WeightPerCharge { get { return 1; } }

        public override bool Decays { get { return false; } }
        public override TimeSpan DecayDuration { get { return TimeSpan.FromDays(3); } }

        public override int MinStaminaRegained { get { return 20; } }
        public override int MaxStaminaRegained { get { return 40; } }

        [Constructable]
        public CookedFishFillet(): this(1)
        {
        } 

        [Constructable]
        public CookedFishFillet(int amount): base(7708)
        {
            Name = "cooked fish fillet";
            Hue = 2968;

            Amount = amount;
        }

        public CookedFishFillet(Serial serial): base(serial)
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