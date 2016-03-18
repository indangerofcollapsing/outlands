using System;
using Server;

namespace Server.Items
{
    public class UOACZJugOfCider : UOACZConsumptionItem
    {
        public override ConsumptionMode ConsumptionType { get { return ConsumptionMode.Drink; } }

        public override int HitsChange { get { return 0; } }
        public override int StamChange { get { return 0; } }
        public override int ManaChange { get { return 0; } }

        public override int HungerChange { get { return 0; } }
        public override int ThirstChange { get { return Utility.RandomMinMax(5, 7); } }
        public override ConsumptionQuality ConsumptionQualityType { get { return ConsumptionQuality.Normal; } } 

        public override int ItemIdFor1Charges { get { return 2504; } }
        public override int ItemIdFor2Charges { get { return 2446; } }
        public override int ItemIdFor3Charges { get { return 2445; } }

        public override int MaxCharges { get { return 3; } }

        public override Type DropContainer { get { return typeof(Bottle); } }

        [Constructable]
        public UOACZJugOfCider(): base(2504)
        {
            Name = "jug of cider";
            Hue = 0;
            Weight = 1;
        }

        public UOACZJugOfCider(Serial serial): base(serial)
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