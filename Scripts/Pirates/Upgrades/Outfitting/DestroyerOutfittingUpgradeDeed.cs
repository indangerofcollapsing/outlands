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
    public class DestroyerOutfittingUpgradeDeed : BaseBoatOutfittingUpgradeDeed
    {
        public override string DisplayName { get { return "Destroyer"; } }
        public override OutfittingType Outfitting { get { return OutfittingType.Destroyer; } } 

        [Constructable]
        public DestroyerOutfittingUpgradeDeed(): base()
        {
            Name = "a ship outfitting upgrade: Destroyer";
        }

        public DestroyerOutfittingUpgradeDeed(Serial serial): base(serial)
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