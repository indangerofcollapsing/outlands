using System;
using Server.Items;
using System.Collections;
using System.Collections.Generic;

namespace Server
{
    public class Unity : Faction
    {
        public override bool Active { get { return true; } }

        public override FactionTypeValue FactionType { get { return FactionTypeValue.Unity; } }
        public override string FactionName { get { return "Unity"; } }

        public override int TextHue { get { return 2603; } }

        public override int SymbolIconId { get { return 11009; } }
        public override int SymbolIconHue { get { return 2603; } }
        public override int SymbolIconOffsetX { get { return 0; } }
        public override int SymbolIconOffsetY { get { return 0; } }

        public override int FlagIconId { get { return 17099; } }
        public override int FlagIconHue { get { return 2603; } }
        public override int FlagIconOffsetX { get { return 0; } }
        public override int FlagIconOffsetY { get { return 0; } }

        [Constructable]
        public Unity(): base()
        {
        }

        public Unity(Serial serial): base(serial)
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
    }
}