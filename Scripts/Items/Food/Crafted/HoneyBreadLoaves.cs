using System;
using Server;

namespace Server.Items
{
    public class HoneyBreadLoaves : Food
    {
        public override string DisplayName { get { return "honey bread loaves"; } }
        public override SatisfactionLevelType Satisfaction { get { return SatisfactionLevelType.Adequate; } }

        public override int IconItemId { get { return 2444; } }
        public override int IconItemHue { get { return 2214; } }
        public override int IconOffsetX { get { return 0; } }
        public override int IconOffsetY { get { return 6; } }

        public override int FillFactor { get { return 10; } }
        public override bool IsStackable { get { return true; } }
        public override int MaxCharges { get { return 1; } }
        public override double WeightPerCharge { get { return 1; } }

        public override bool Decays { get { return false; } }
        public override TimeSpan DecayDuration { get { return TimeSpan.FromDays(3); } }

        public override int MinStaminaRegained { get { return 20; } }
        public override int MaxStaminaRegained { get { return 40; } }

        [Constructable]
        public HoneyBreadLoaves() : this(1)
        {
        } 

        [Constructable]
        public HoneyBreadLoaves(int amount): base(2444)
        {
            Name = "honey bread loaves";
            Hue = 2214;

            Amount = amount;
        }

        public HoneyBreadLoaves(Serial serial): base(serial)
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