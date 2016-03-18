/***************************************************************************
 *                              Dockmaster.cs
 *                            ------------------
 *   begin                : August 2010
 *   author               : Sean Stavropoulos
 *   email                : sean.stavro@gmail.com
 *
 *
 ***************************************************************************/
using System;
using Server;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Targeting;
using Server.ContextMenus;
using Server.Mobiles;
using Server.Custom.Pirates.Battleground;

namespace Server.Mobiles
{
    public class BattlegroundDockmaster : Dockmaster
    {
        [Constructable]
        public BattlegroundDockmaster()
            : base()
        {
        }

        public BattlegroundDockmaster(Serial serial)
            : base(serial)
        {
        }

        public override void VendorBuy(Mobile from)
        {
            BattlegroundDefense.PurchaseMenuRequest(from, this);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }
}