using System;
using Server;
using Server.Items;
using Server.Targeting;
using Server.Network;

namespace Server.Items
{
    public class CyprusTree : Plant
    {
        public override string PlantName { get { return "Cyprus Tree"; } }
        public override string PlantDescription { get { return "Cyprus Description"; } }

        public override int PlantWeight { get { return 50; } }

        public override int PlantItemID { get { return 3320; } }
        public override int PlantItemHue { get { return 0; } }
        public override int PlantItemOffsetX { get { return -38; } }
        public override int PlantItemOffsetY { get { return -123; } }

        [Constructable]
        public CyprusTree(): base()
        {
        }

        public CyprusTree(Serial serial): base(serial)
        {
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