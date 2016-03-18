using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
    public class UOACZCorruptionBox : WoodenBox
    {
        [Constructable]
        public UOACZCorruptionBox(): base()
        {
            Name = "corruption box";
            Hue = 1104;

            Weight = 1;

            LootType = Server.LootType.Blessed;
        }

        public UOACZCorruptionBox(Serial serial): base(serial)
        {
        }        

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version          
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}