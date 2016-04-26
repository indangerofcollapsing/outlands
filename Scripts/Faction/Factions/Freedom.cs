using System;
using Server.Items;
using System.Collections;
using System.Collections.Generic;

namespace Server
{
    public class Freedom : Faction
    {
        public override bool Active { get { return true; } }

        public override string FactionName { get { return "Freedom"; } }

        public override int TextHue { get { return 2550; } }

        public override int SymbolIconId { get { return 16140; } }
        public override int SymbolIconHue { get { return 2550; } }
        public override int SymbolIconOffsetX { get { return 0; } }
        public override int SymbolIconOffsetY { get { return 0; } }

        public override int FlagIconId { get { return 6326; } }
        public override int FlagIconHue { get { return 2550; } }
        public override int FlagIconOffsetX { get { return 0; } }
        public override int FlagIconOffsetY { get { return 0; } }

        public Freedom()
        {
        }
    }
}