using System;
using Server;

namespace Server.Items
{
    public class FruitPie : Food
    {
        public override string DisplayName { get { return "fruit pie"; } }
        public override SatisfactionLevelType Satisfaction { get { return SatisfactionLevelType.Appetizing; } }

        public override int IconItemId { get { return 4162; } }
        public override int IconItemHue { get { return 2634; } }
        public override int IconOffsetX { get { return 5; } }
        public override int IconOffsetY { get { return 7; } }

        public override int FillFactor { get { return 15; } }
        public override bool IsStackable { get { return false; } }
        public override int MaxCharges { get { return 3; } }
        public override double WeightPerCharge { get { return 1; } }

        public override bool Decays { get { return true; } }
        public override TimeSpan DecayDuration { get { return TimeSpan.FromDays(7); } }

        public override int MinStaminaRegained { get { return 30; } }
        public override int MaxStaminaRegained { get { return 60; } }

        [Constructable]
        public FruitPie(): base(4162)
        {
            Name = "fruit pie";
            Hue = 2634;
        }

        public FruitPie(Serial serial): base(serial)
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