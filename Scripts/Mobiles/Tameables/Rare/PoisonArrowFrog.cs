using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Mobiles
{
    [CorpseName("a poison arrow frog corpse")]
    public class PoisonArrowFrog : WyvernHatchling
    {
        public override bool RareTamable { get { return true; } }

        [Constructable]
        public PoisonArrowFrog(): base()
        {
            Name = "a poison arrow frog";
            Body = 81;
            BaseSoundID = 614;
            Hue = 1270;
        }

        public PoisonArrowFrog(Serial serial): base(serial)
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

        public override int TamedItemId { get { return 8496; } }
        public override int TamedItemHue { get { return Hue; } }
        public override int TamedItemXOffset { get { return 0; } }
        public override int TamedItemYOffset { get { return 15; } }
    }
}
