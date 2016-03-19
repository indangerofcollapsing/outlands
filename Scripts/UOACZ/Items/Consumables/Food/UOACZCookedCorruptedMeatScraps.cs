using System;
using Server;

namespace Server.Items
{
    public class UOACZCookedCorruptedMeatScraps : UOACZConsumptionItem
    {
        public override ConsumptionMode ConsumptionType { get { return ConsumptionMode.Food; } }

        public override int HitsChange { get { return 0; } }
        public override int StamChange { get { return 0; } }
        public override int ManaChange { get { return 0; } }

        public override int HungerChange { get { return Utility.RandomMinMax(6, 8); } }
        public override int ThirstChange { get { return 0; } }

        public override ConsumptionQuality ConsumptionQualityType { get { return ConsumptionQuality.Corrupted; } } 

        [Constructable]
        public UOACZCookedCorruptedMeatScraps(): base(7820)
        {
            Name = "cooked corrupted meat scraps";
            Hue = 2211;
            Weight = 1;
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);            
        }

        public UOACZCookedCorruptedMeatScraps(Serial serial): base(serial)
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