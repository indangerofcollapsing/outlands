using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Misc;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
    public class Prevalia : Town
    {
        public override string TownName { get { return "Prevalia"; } }
        public override TownIDValue TownID { get { return TownIDValue.Prevalia; } }
        public override IndexedRegionName RegionName { get { return IndexedRegionName.Prevalia; } }

        public override int TownHue { get { return 2500; } }

        public override int TownIconItemId { get { return 0x13B9; } }
        public override int TownIconHue { get { return 0; } }

        [Constructable]
        public Prevalia(): base(TownIDValue.Prevalia, Map.Felucca)
        {
        }

        public Prevalia(Serial serial): base(serial)
        {
        }

        public override void CreateVendors()
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