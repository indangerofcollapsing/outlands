using System;
using Server;
using Server.Regions;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;
using Server.Multis;
using System.Collections;
using System.Collections.Generic;
using Server.Items;

namespace Server.Custom
{
    public class UOACZBoilingOilCauldronDeed : UOACZBaseConstructionDeed
    {
        public override UOACZConstructionTile.ConstructionObjectType ConstructionType { get { return UOACZConstructionTile.ConstructionObjectType.Fortification; } }
        public override Type ConstructableObject { get { return typeof(UOACZBoilingOilCauldron); } }
        public override string DisplayName { get { return "boiling oil cauldron"; } }

        [Constructable]
        public UOACZBoilingOilCauldronDeed(): base()
        {            
        }

        public UOACZBoilingOilCauldronDeed(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}