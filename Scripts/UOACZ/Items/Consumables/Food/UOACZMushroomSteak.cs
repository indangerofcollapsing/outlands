using System;
using Server;

namespace Server.Items
{
    public class UOACZMushroomSteak : UOACZConsumptionItem
    {
        public override ConsumptionMode ConsumptionType { get { return ConsumptionMode.Food; } }

        public override int HitsChange { get { return 0; } }
        public override int StamChange { get { return 0; } }
        public override int ManaChange { get { return 0; } }

        public override int HungerChange { get { return Utility.RandomMinMax(9, 11); } }
        public override int ThirstChange { get { return 0; } }
        public override ConsumptionQuality ConsumptionQualityType { get { return ConsumptionQuality.Normal; } }    

        [Constructable]
        public UOACZMushroomSteak(): base(2416)
        {
            Name = "mushroom steak";
            Hue = 2709;

            Charges = 2;
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
        }

        public UOACZMushroomSteak(Serial serial): base(serial)
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