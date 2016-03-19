using System;
using Server;

namespace Server.Items
{
    public class UOACZRawSteak : UOACZConsumptionItem
    {
        public override ConsumptionMode ConsumptionType { get { return ConsumptionMode.Food; } }

        public override int HitsChange { get { return 0; } }
        public override int StamChange { get { return 0; } }
        public override int ManaChange { get { return 0; } }

        public override int HungerChange { get { return Utility.RandomMinMax(6, 8); } }
        public override int ThirstChange { get { return 0; } }

        public override ConsumptionQuality ConsumptionQualityType { get { return ConsumptionQuality.Raw; } }    

        [Constructable]
        public UOACZRawSteak(): base(2545)
        {
            Name = "raw steak";
            Hue = 0;
            Weight = 1;
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);           
        }

        public UOACZRawSteak(Serial serial): base(serial)
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