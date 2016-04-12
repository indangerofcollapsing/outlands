using System;
using Server.Items;
using System.Collections;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;
using Server.Mobiles;
using Server.Targeting;

namespace Server
{
    public class SoulOfMaliel : KhaldunLichCrystal
    {
        public override int MainHue { get { return 2587; } }
        public override int AltHue { get { return 2603; } }

        [Constructable]
        public SoulOfMaliel(): base()
        {
            Name = "captured soul of Maliel";

            Hue = MainHue;
        }
        
        public SoulOfMaliel(Serial serial): base(serial)
        {
        }
        
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}