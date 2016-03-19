using System;
using Server;

namespace Server.Items
{
    public class UOACZGlassOfWine : UOACZConsumptionItem
    {
        public override ConsumptionMode ConsumptionType { get { return ConsumptionMode.Drink; } }

        public override int HitsChange { get { return 0; } }
        public override int StamChange { get { return 0; } }
        public override int ManaChange { get { return 0; } }

        public override int HungerChange { get { return 0; } }
        public override int ThirstChange { get { return Utility.RandomMinMax(7, 9); } }
        public override ConsumptionQuality ConsumptionQualityType { get { return ConsumptionQuality.Normal; } } 

        public override int MaxCharges { get { return 1; } }

        public override Type DropContainer { get { return typeof(UOACZGlass); } }   

        [Constructable]
        public UOACZGlassOfWine(): base(8077)
        {
            Name = "glass of wine";
            Hue = 0;
            Weight = 1;
        }

        public UOACZGlassOfWine(Serial serial): base(serial)
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