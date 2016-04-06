using System;

namespace Server.Items
{
    public class CookedLobster : Food
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
        public CookedLobster(): this(1)
        {
        }

        [Constructable]
        public CookedLobster(int amount): base(0x44D3)
        {
            Name = "cooked lobster";

            Weight = 2.0;
            Amount = amount;
        }

        public CookedLobster(Serial serial): base(serial)
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