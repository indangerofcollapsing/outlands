using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Mobiles
{
    [CorpseName("a ravenous ostard corpse")]
    public class RavenousOstard : Bloodcat
    {
        public override bool RareTamable { get { return true; } }

        [Constructable]
        public RavenousOstard(): base()
        {
            Name = "a ravenous ostard";
            Body = 218;
            BaseSoundID = 629;
            Hue = 2644;
        }

        public RavenousOstard(Serial serial): base(serial)
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

        public override int TamedItemId { get { return 8503; } }
        public override int TamedItemHue { get { return Hue; } }
        public override int TamedItemXOffset { get { return 5; } }
        public override int TamedItemYOffset { get { return 0; } }
    }
}
