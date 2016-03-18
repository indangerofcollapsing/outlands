using System;
using Server.Network;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Targeting;
using Server.Spells;

namespace Server.Custom
{
    public class FaintLure : Lure
    {
        public override int AggroBonus { get { return 3; } }
        public override TimeSpan AggroExpiration { get { return TimeSpan.FromMinutes(20); } }
        public override int LureHue { get { return 2635; } }

        [Constructable]
        public FaintLure(): base()
        {
            Name = "a faint lure";
        }

        public FaintLure(Serial serial): base(serial)
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
