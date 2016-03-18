using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Mobiles
{
    [CorpseName("a crimson recluse corpse")]
    public class CrimsonRecluse : LavaSerpent
    {
        public override bool RareTamable { get { return true; } }

        [Constructable]
        public CrimsonRecluse(): base()
        {
            Name = "a crimson recluse";
            Body = 28;
            BaseSoundID = 904;
            Hue = 1779;
        }

        public CrimsonRecluse(Serial serial): base(serial)
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

        public override int TamedItemId { get { return 8445; } }
        public override int TamedItemHue { get { return Hue; } }
        public override int TamedItemXOffset { get { return 0; } }
        public override int TamedItemYOffset { get { return 0; } }
    }
}
