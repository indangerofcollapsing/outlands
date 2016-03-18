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
    public class MerchantOutfittingUpgradeDeed : BaseBoatOutfittingUpgradeDeed
    {
        public override string DisplayName { get { return "Merchant"; } }
        public override OutfittingType Outfitting { get { return OutfittingType.Merchant; } } 

        [Constructable]
        public MerchantOutfittingUpgradeDeed(): base()
        {
            Name = "a ship outfitting upgrade: Merchant";
        }

        public MerchantOutfittingUpgradeDeed(Serial serial): base(serial)
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