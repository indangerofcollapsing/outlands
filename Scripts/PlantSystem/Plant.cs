using System;
using Server;
using Server.Items;
using Server.Targeting;
using Server.Network;

namespace Server.Items
{
    public class Plant : Item
    {
        public virtual string PlantName { get { return "Plant Name"; } }
        public virtual string PlantDescription { get { return "Plant Description"; } }

        public virtual int PlantWeight { get { return 5; } }

        public virtual int PlantItemID { get { return 3238; } }
        public virtual int PlantItemHue { get { return 0; } }
        public virtual int PlantItemOffsetX { get { return 0; } }
        public virtual int PlantItemOffsetY { get { return 0; } }
        
        [Constructable]
        public Plant(): base(0x15FD)
        {
            Name = PlantName;
            ItemID = PlantItemID;
            Hue = PlantItemHue;

            Weight = PlantWeight;
        }

        public Plant(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, PlantName);
            LabelTo(from, "(" + PlantDescription + ")");
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}