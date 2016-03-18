using System;
using System.Collections;
using Server.Targeting;
using Server.Items;
using Server.Engines.Harvest;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
    public class OceansBountyFishingPole : FishingPole
    {
        public override int PlayerClassCurrencyValue { get { return 2500; } }

        [Constructable]
        public OceansBountyFishingPole(): base()
        {
            Name = "Ocean's Bounty Fishing Pole";

            Layer = Layer.TwoHanded;
            Weight = 8.0;

            PlayerClass = PlayerClass.Pirate;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.PirateItemHue;

            LootType = Server.LootType.Blessed;
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            PlayerClassPersistance.PlayerClassSingleClick(this, from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            if (!player.Pirate || PlayerClassOwner != player)
            {
                from.SendMessage("Only the Pirate owner of this item may use it");
                return;
            }

            base.OnDoubleClick(from);
        }

        public OceansBountyFishingPole(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);


            //----------------

            LootType = LootType.Blessed;
        }
    }
}