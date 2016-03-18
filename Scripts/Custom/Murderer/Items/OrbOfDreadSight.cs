using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Targeting;
using Server.ContextMenus;
using Server.Network;
using Server.Regions;
using System.Text;

namespace Server.Items
{
    public class OrbOfDreadSight : BaseOrb
    {
        public override int PlayerClassCurrencyValue { get { return 25; } }

        [Constructable]
        public OrbOfDreadSight()
        {
            Name = "Orb of Dread Sight";

            ItemID = 0xE2D;
            Weight = 1.0;
            SetDisplay(0, 0, 5119, 4095, 400, 400);
            Protected = true;

            LootType = LootType.Blessed;

            PlayerClass = PlayerClass.Murderer;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.MurdererCurrencyItemHue;
        }

        public OrbOfDreadSight(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            PlayerClassPersistance.PlayerClassSingleClick(this, from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile pm_From = from as PlayerMobile;

            if (pm_From != null)
            {
                if (!pm_From.Murderer)
                {
                    pm_From.SendMessage("Only murderers may use this item.");
                    return;
                }
            }

            if (!PlayerClassPersistance.PlayerClassDoubleClick(this, from))
                return;

            base.OnDoubleClick(from);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }
}


