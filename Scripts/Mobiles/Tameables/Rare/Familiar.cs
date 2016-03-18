using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Mobiles
{
    [CorpseName("a familiar corpse")]
    public class Familiar : Imp
    {
        public override bool RareTamable { get { return true; } }

        [Constructable]
        public Familiar(): base()
        {
            Name = "a familiar";
            Body = 201;
            BaseSoundID = 105;
            Hue = Utility.RandomList(1175, 1150, 1752);
        }

        public Familiar(Serial serial): base(serial)
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

        public override int TamedItemId { get { return 8475; } }
        public override int TamedItemHue { get { return Hue; } }
        public override int TamedItemXOffset { get { return 0; } }
        public override int TamedItemYOffset { get { return 20; } }
    }
}
