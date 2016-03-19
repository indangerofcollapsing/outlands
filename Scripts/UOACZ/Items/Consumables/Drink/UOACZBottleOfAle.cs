using System;
using Server;

namespace Server.Items
{
    public class UOACZBottleOfAle : UOACZConsumptionItem
    {
        public override ConsumptionMode ConsumptionType { get { return ConsumptionMode.Drink; } }

        public override int HitsChange { get { return 0; } }
        public override int StamChange { get { return 0; } }
        public override int ManaChange { get { return 0; } }

        public override int HungerChange { get { return 0; } }
        public override int ThirstChange { get { return Utility.RandomMinMax(6, 8); } }
        public override ConsumptionQuality ConsumptionQualityType { get { return ConsumptionQuality.Normal; } } 

        public override int ItemIdFor1Charges { get { return 2463; } }
        public override int ItemIdFor2Charges { get { return 2464; } }
        public override int ItemIdFor3Charges { get { return 2465; } }
        public override int ItemIdFor4Charges { get { return 2466; } }

        public override int MaxCharges { get { return 4; } }

        public override Type DropContainer { get { return typeof(Bottle); } }   

        [Constructable]
        public UOACZBottleOfAle(): base(2463)
        {
            Name = "bottle of ale";
            Hue = 0;
            Weight = 1;
        }

        public UOACZBottleOfAle(Serial serial): base(serial)
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