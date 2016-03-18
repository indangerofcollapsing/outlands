using System;
using Server;

namespace Server.Items
{
    public class UOACZMeatPie : UOACZConsumptionItem
    {
        public override ConsumptionMode ConsumptionType { get { return ConsumptionMode.Food; } }

        public override int HitsChange { get { return 0; } }
        public override int StamChange { get { return 0; } }
        public override int ManaChange { get { return 0; } }

        public override int HungerChange { get { return Utility.RandomMinMax(8, 10); } }
        public override int ThirstChange { get { return 0; } }
        public override ConsumptionQuality ConsumptionQualityType { get { return ConsumptionQuality.Normal; } } 

        public override Type DropContainer { get { return typeof(UOACZBowl); } }  

        [Constructable]
        public UOACZMeatPie(): base(4161)
        {
            Name = "meat pie";
            Hue = 0;
            Weight = 1;

            Charges = 2;
        }

        public UOACZMeatPie(Serial serial): base(serial)
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