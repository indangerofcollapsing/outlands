using System;
using Server;
using Server.Items;
using Server.Targeting;
using Server.Network;

namespace Server.Items
{
    public enum PlantGroupType
    {
        Crop,
        Fern,
        Flower,
        Grass,
        Tree,
        Vine
    }

    public class PlantSeed : Item
    {
        public virtual string SeedName { get { return "Seed Name"; } }
        public virtual string SeedGumpName { get { return "Seed"; } }
        public virtual string SeedDescription { get { return "Seed Description"; } }

        public virtual int SeedItemID { get { return 22326; } }
        public virtual int SeedHue { get { return 0; } }
        public virtual int SeedGumpOffsetX { get { return 0; } }
        public virtual int SeedGumpOffsetY { get { return 0; } }

        public virtual string PlantName { get { return "Cyprus Tree"; } }
        public virtual Type PlantType { get { return typeof(CyprusTree); } }
        public virtual PlantGroupType PlantGroup { get { return PlantGroupType.Tree; } }
        public virtual int PlantCount { get { return 1; } }

        public virtual double TargetGrowth { get { return 70.0; } }
        public virtual double TargetWater { get { return 25.0; } }
        public virtual double TargetSoilQuality { get { return 25.0; } }
        public virtual double TargetHeat { get { return 25.0; } }

        [Constructable]
        public PlantSeed(): base(22326)
        {
            Name = SeedName;
            ItemID = SeedItemID;
            Hue = SeedHue;

            Weight = 0.1;
        }

        public PlantSeed(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, SeedName);
            LabelTo(from, "(place in plant bowl to grow)");
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.SendMessage(SeedDescription);
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