using System;
using Server;

namespace Server.Items
{
    public class UOACZFruitCake : UOACZConsumptionItem
    {
        public override ConsumptionMode ConsumptionType { get { return ConsumptionMode.Food; } }

        public override int HitsChange { get { return 0; } }
        public override int StamChange { get { return 0; } }
        public override int ManaChange { get { return 0; } }

        public override int HungerChange { get { return Utility.RandomMinMax(7, 9); } }
        public override int ThirstChange { get { return 0; } }
        public override ConsumptionQuality ConsumptionQualityType { get { return ConsumptionQuality.Normal; } }    

        [Constructable]
        public UOACZFruitCake(): base(2537)
        {
            Name = "fruit cake";
            Hue = 2516;
            Weight = 1;

            Charges = 5;
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);            
        }

        public UOACZFruitCake(Serial serial): base(serial)
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