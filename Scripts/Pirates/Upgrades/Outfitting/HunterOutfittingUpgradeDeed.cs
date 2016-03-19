using System;
using Server;
using Server.Regions;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Multis;
using Server.Gumps;
using System.Collections;
using System.Collections.Generic;

namespace Server.Items
{
    public class HunterOutfittingUpgradeDeed : BaseBoatOutfittingUpgradeDeed
    {
        public override string DisplayName { get { return "Hunter"; } }
        public override OutfittingType Outfitting { get { return OutfittingType.Hunter; } } 

        [Constructable]
        public HunterOutfittingUpgradeDeed(): base()
        {
            Name = "a ship outfitting upgrade: Hunter";
        }

        public HunterOutfittingUpgradeDeed(Serial serial): base(serial)
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