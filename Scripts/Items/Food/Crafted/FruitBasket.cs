using System;
using Server;

namespace Server.Items
{
    public class FruitDish : Food
    {
        public override string DisplayName { get { return "fruit dish"; } }
        public override SatisfactionLevelType Satisfaction { get { return SatisfactionLevelType.Meagre; } }

        public override int IconItemId { get { return 2523; } }
        public override int IconItemHue { get { return 0; } }
        public override int IconOffsetX { get { return -4; } }
        public override int IconOffsetY { get { return -1; } }

        public override int FillFactor { get { return 6; } }
        public override bool IsStackable { get { return true; } }
        public override int MaxCharges { get { return 1; } }
        public override double WeightPerCharge { get { return 1; } }

        public override bool Decays { get { return false; } }
        public override TimeSpan DecayDuration { get { return TimeSpan.FromDays(7); } }

        public override int MinStaminaRegained { get { return 10; } }
        public override int MaxStaminaRegained { get { return 20; } }

        [Constructable]
        public FruitDish(): this(1)
        {
        }

        [Constructable]
        public FruitDish(int amount): base(2523)
        {
            Name = "fruit dish";
            Hue = 0;

            Amount = amount;
        }

        public FruitDish(Serial serial): base(serial)
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