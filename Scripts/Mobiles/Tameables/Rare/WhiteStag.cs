using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Mobiles
{
    [CorpseName("a white stag corpse")]
    public class WhiteStag : Bullvore
    {
        public override bool RareTamable { get { return true; } }

        [Constructable]
        public WhiteStag(): base()
        {
            Name = "a white stag";
            Body = 234;
            Hue = 2498;
        }

        public WhiteStag(Serial serial): base(serial)
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

        public override int GetAttackSound() { return 0x82; }
        public override int GetHurtSound()   { return 0x83; }
        public override int GetDeathSound()  { return 0x84; }
        public override int TamedItemId      { get { return 8404; } }
        public override int TamedItemHue     { get { return Hue; } }
        public override int TamedItemXOffset { get { return 5; } }
        public override int TamedItemYOffset { get { return 5; } }
    }
}
