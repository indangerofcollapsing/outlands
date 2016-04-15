using System;
using Server;

namespace Server.Items
{
    public class Cornbread : Food
    {
        public override string DisplayName { get { return "cornbread"; } }
        public override SatisfactionLevelType Satisfaction { get { return SatisfactionLevelType.Adequate; } }

        public override int IconItemId { get { return 2430; } }
        public override int IconItemHue { get { return 2635; } }
        public override int IconOffsetX { get { return -4; } }
        public override int IconOffsetY { get { return 4; } }

        public override int FillFactor { get { return 10; } }
        public override bool IsStackable { get { return true; } }
        public override int MaxCharges { get { return 1; } }
        public override double WeightPerCharge { get { return 1; } }

        public override bool Decays { get { return false; } }
        public override TimeSpan DecayDuration { get { return TimeSpan.FromDays(3); } }

        public override int MinStaminaRegained { get { return 20; } }
        public override int MaxStaminaRegained { get { return 40; } }

        [Constructable]
        public Cornbread(): this(1)
        {
        } 

        [Constructable]
        public Cornbread(int amount): base(2430)
        {
            Name = "cornbread";
            Hue = 2635;

            Amount = amount;
        }

        public Cornbread(Serial serial): base(serial)
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