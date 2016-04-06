using Server.Multis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Mobiles;
using Server.Misc;
using Server.Network;

namespace Server.Items
{
    class PlayerEnhancementStone : Item
    {
        [Constructable]
        public PlayerEnhancementStone(): base(3804)
        {
            Name = "a player enhancement stone";
            Hue = 2635;
            
            Movable = false;
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            PlayerMobile pm_From = from as PlayerMobile;

            if (pm_From == null)
                return;

            PlayerEnhancementPersistance.CheckAndCreatePlayerEnhancementAccountEntry(pm_From);

            from.CloseAllGumps();
            from.SendGump(new PlayerEnhancementGump(from));
        }

        public PlayerEnhancementStone(Serial serial): base(serial)
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
