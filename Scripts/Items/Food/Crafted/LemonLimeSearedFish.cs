using System;
using Server;

namespace Server.Items
{
    public class LemonLimeSearedFish : Food
    {
        public override string DisplayName { get { return "lemon-lime seared fish"; } }
        public override SatisfactionLevelType Satisfaction { get { return SatisfactionLevelType.Delectable; } }

        public override int IconItemId { get { return 10302; } }
        public override int IconItemHue { get { return Hue; } }
        public override int IconOffsetX { get { return -2; } }
        public override int IconOffsetY { get { return -2; } }

        public override int FillFactor { get { return 30; } }
        public override bool IsStackable { get { return false; } }
        public override int MaxCharges { get { return 5; } }
        public override double WeightPerCharge { get { return 1; } }

        public override bool Decays { get { return true; } }
        public override TimeSpan DecayDuration { get { return TimeSpan.FromDays(3); } }

        public override int MinStaminaRegained { get { return 50; } }
        public override int MaxStaminaRegained { get { return 100; } }
        
        [Constructable]
        public LemonLimeSearedFish(): base(10302)
        {
            Name = "lemon-lime seared fish";
        }

        public LemonLimeSearedFish(Serial serial): base(serial)
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