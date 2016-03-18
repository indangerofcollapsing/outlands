using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Mobiles
{
    [CorpseName("a thunder bird corpse")]
    public class ThunderBird : Wisp
    {
        public override bool RareTamable { get { return true; } }

        [Constructable]
        public ThunderBird(): base()
        {
            Name = "a thunder bird";
            Body = 5;
            BaseSoundID = 750;
            Hue = 2580;
        }

        public ThunderBird(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public override int TamedItemId { get { return 8434; } }
        public override int TamedItemHue { get { return Hue; } }
        public override int TamedItemXOffset { get { return 0; } }
        public override int TamedItemYOffset { get { return 0; } }
    }
}
