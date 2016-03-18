using System;
using Server;
using Server.Engines.Craft;

namespace Server.Items
{   
    public class UOACZTinkersTools : BaseTool
    {
        public override CraftSystem CraftSystem { get { return UOACZDefTinkering.CraftSystem; } }

        public override int MaxUses { get { return 50; } }

        [Constructable]
        public UOACZTinkersTools(): base(0x1EB8)
        {
            Name = "tinker's tools";

            Weight = 1.0;

            UsesRemaining = MaxUses;
        }

        [Constructable]
        public UOACZTinkersTools(int uses): base(uses, 0x1EB8)
        {
            Name = "tinker's tools";

            Weight = 1.0;

            UsesRemaining = MaxUses;
        }

        public UOACZTinkersTools(Serial serial): base(serial)
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