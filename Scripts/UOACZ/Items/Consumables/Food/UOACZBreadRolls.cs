using System;
using Server;

namespace Server.Items
{
    public class UOACZBreadRolls : UOACZConsumptionItem
    {
        public override ConsumptionMode ConsumptionType { get { return ConsumptionMode.Food; } }

        public override int HitsChange { get { return 0; } }
        public override int StamChange { get { return 0; } }
        public override int ManaChange { get { return 0; } }

        public override int HungerChange { get { return Utility.RandomMinMax(4, 6); } }
        public override int ThirstChange { get { return 0; } }
        public override ConsumptionQuality ConsumptionQualityType { get { return ConsumptionQuality.Normal; } } 

        public override int ItemIdFor1Charges { get { return 2538; } }
        public override int ItemIdFor2Charges { get { return 2554; } }
        public override int ItemIdFor3Charges { get { return 2539; } }
        public override int ItemIdFor4Charges { get { return 2539; } }

        [Constructable]
        public UOACZBreadRolls(): base(2539)
        {
            Name = "bread rolls";
            Hue = 0;
            Weight = 1;

            Charges = 3;
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);            
        }

        public UOACZBreadRolls(Serial serial): base(serial)
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