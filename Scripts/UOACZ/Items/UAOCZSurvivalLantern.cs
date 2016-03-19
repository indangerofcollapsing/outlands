using System;
using Server;

namespace Server.Items
{
    public class UOACZSurvivalLantern : Lantern
    {
        [Constructable]
        public UOACZSurvivalLantern(): base()
        {
            Name = "a survival lantern";
            Hue = 2575;

            Weight = 1;

            LootType = LootType.Blessed;
        }

        public UOACZSurvivalLantern(Serial serial): base(serial)
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