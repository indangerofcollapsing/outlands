using System;

namespace Server.Items
{
    public class CookedLobster : Food
    {
        public override int FillFactor { get { return 20; } }
        public override SatisfactionLevelType Satisfaction { get { return SatisfactionLevelType.Delectable; } }
        public override TimeSpan SatisfactionDuration { get { return TimeSpan.FromMinutes(60); } }
        public override int MaxCharges { get { return 2; } }
        public override bool Decays { get { return true; } }
        public override TimeSpan DecayDuration { get { return TimeSpan.FromDays(3); } }

        public override int MinStaminaRegained { get { return 40; } }
        public override int MaxStaminaRegained { get { return 80; } }

        [Constructable]
        public CookedLobster(): this(1)
        {
        }

        [Constructable]
        public CookedLobster(int amount): base(amount, 0x44D3)
        {
            Stackable = false;

            Weight = 2.0;
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