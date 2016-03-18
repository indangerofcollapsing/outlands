using System;
using Server;

namespace Server.Items
{
    public class UOACZBottleOfFruitJuice : UOACZConsumptionItem
    {
        public override ConsumptionMode ConsumptionType { get { return ConsumptionMode.Drink; } }

        public override int HitsChange { get { return 0; } }
        public override int StamChange { get { return 0; } }
        public override int ManaChange { get { return 0; } }

        public override int HungerChange { get { return 0; } }
        public override int ThirstChange { get { return Utility.RandomMinMax(5, 7); } }
        public override ConsumptionQuality ConsumptionQualityType { get { return ConsumptionQuality.Normal; } } 

        public override int ItemIdFor1Charges { get { return 2459; } }
        public override int ItemIdFor2Charges { get { return 2460; } }
        public override int ItemIdFor3Charges { get { return 2461; } }
        public override int ItemIdFor4Charges { get { return 2462; } }

        public override int MaxCharges { get { return 4; } }

        public override Type DropContainer { get { return typeof(Bottle); } }   

        [Constructable]
        public UOACZBottleOfFruitJuice(): base(2459)
        {
            Name = "bottle of fruit juice";
            Hue = 2630;
            Weight = 1;
        }

        public UOACZBottleOfFruitJuice(Serial serial): base(serial)
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