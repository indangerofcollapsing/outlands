using System;

namespace Server.Items
{
    public class CookedCrab : Food
    {
        public override int FillFactor { get { return 10; } }
        public override SatisfactionLevelType Satisfaction { get { return SatisfactionLevelType.Delectable; } }
        public override TimeSpan SatisfactionDuration { get { return TimeSpan.FromMinutes(60); } }
        public override int MaxCharges { get { return 1; } }
        public override bool Decays { get { return false; } }
        public override TimeSpan DecayDuration { get { return TimeSpan.FromDays(3); } }

        public override int MinStaminaRegained { get { return 20; } }
        public override int MaxStaminaRegained { get { return 40; } }

        [Constructable]
        public CookedCrab(): this(1)
        {
        }

        [Constructable]
        public CookedCrab(int amount): base( 0x44D1)
        {
            Name = "cooked crab";

            Weight = 2.0;
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