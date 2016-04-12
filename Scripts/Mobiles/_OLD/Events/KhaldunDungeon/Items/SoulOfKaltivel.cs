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
    public class SoulOfKaltivel : KhaldunLichCrystal
    {
        public override int MainHue { get { return 2614; } }
        public override int AltHue { get { return 2630; } }

        [Constructable]
        public SoulOfKaltivel(): base()
        {
            Name = "captured soul of Kaltivel";

            Hue = MainHue;
        }
        
        public SoulOfKaltivel(Serial serial): base(serial)
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