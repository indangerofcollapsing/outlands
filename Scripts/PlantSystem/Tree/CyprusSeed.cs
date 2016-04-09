using System;
using Server;
using Server.Items;
using Server.Targeting;
using Server.Network;

namespace Server.Items
{
    public class CyprusTreeSeed : PlantSeed
    {
        public override string SeedName { get { return "Cyprus Seed"; } }
        public override string SeedGumpName { get { return "Cyprus"; } }
        public override string SeedDescription { get { return "A Swampy-Smelling Seed"; } }

        public override int SeedItemID { get { return 22326; } }
        public override int SeedHue { get { return 2210; } }
        public override int SeedGumpOffsetX { get { return 0; } }
        public override int SeedGumpOffsetY { get { return 0; } }

        public override Type PlantType { get { return typeof(CyprusTree); } }
        public override PlantGroupType PlantGroup { get { return PlantGroupType.Tree; } }
        public override int PlantCount { get { return 1; } }

        public override double TargetGrowth { get { return 125.0; } }
        public override double TargetWater { get { return 80.0; } }
        public override double TargetSoilQuality { get { return 60.0; } }
        public override double TargetHeat { get { return 40.0; } }

        [Constructable]
        public CyprusTreeSeed(): base()
        {
        }

        public CyprusTreeSeed(Serial serial): base(serial)
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