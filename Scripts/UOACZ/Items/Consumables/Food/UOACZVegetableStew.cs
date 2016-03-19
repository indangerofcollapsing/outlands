using System;
using Server;

namespace Server.Items
{
    public class UOACZVegetableStew : UOACZConsumptionItem
    {
        public override ConsumptionMode ConsumptionType { get { return ConsumptionMode.Food; } }

        public override int HitsChange { get { return 0; } }
        public override int StamChange { get { return 0; } }
        public override int ManaChange { get { return 0; } }

        public override int HungerChange { get { return Utility.RandomMinMax(4, 6); } }
        public override int ThirstChange { get { return 0; } }

        public override ConsumptionQuality ConsumptionQualityType { get { return ConsumptionQuality.Normal; } } 

        public override Type DropContainer { get { return typeof(UOACZBowl); } }  

        [Constructable]
        public UOACZVegetableStew(): base(5638)
        {
            Name = "vegetable stew";
            Hue = Utility.RandomList(2562, 2527, 2611, 2213, 2127, 2101);

            Charges = 4;
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            
        }

        public UOACZVegetableStew(Serial serial): base(serial)
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