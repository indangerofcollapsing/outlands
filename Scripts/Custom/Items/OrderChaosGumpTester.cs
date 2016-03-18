using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;

namespace Server.Items
{
    public class OrderChaosGumpTester : Item
    {   
        [Constructable]
        public OrderChaosGumpTester(): base(0x26BC)
        {
            Name = "Order Chaos gump tester";
        }

        public OrderChaosGumpTester(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);            
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            player.CloseGump(typeof(OrderChaosTestGump));
            player.SendGump(new OrderChaosTestGump(player));
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

    public class OrderChaosTestGump : Gump
    {
        PlayerMobile player;

        public OrderChaosTestGump(PlayerMobile player): base(10, 10)
        {
            if (player == null) return;
            if (player.Deleted) return;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            int GreenTextHue = 0x3F;
            int WhiteTextHue = 2655;
            int PinkTextHue = 2606;

            int ContestingTextHue = 2114;
            
            AddItem(6, 12, 7108);
            AddItem(119, 13, 7107);

            /*
            AddLabel(55, 5, 1164, "Order");
            AddLabel(55, 25, GreenTextHue, "125 [+1.0]");

            AddLabel(159, 5, 2115, "Chaos");
            AddLabel(160, 25, WhiteTextHue, "85 [+0]");

            AddLabel(20, 50, 149, "Capture Event");
            AddLabel(118, 50, 2615, "Wrong Dungeon");

            AddLabel(20, 70, 149, "Zone A");
            AddButton(76, 73, 2118, 2118, 0, GumpButtonType.Reply, 0);
            AddLabel(100, 70, GreenTextHue, "Order [+1.0]");

            AddLabel(20, 90, 149, "Zone B");
            AddButton(76, 93, 2118, 2118, 0, GumpButtonType.Reply, 0);
            AddLabel(100, 90, 2114, "Contested");

            AddLabel(19, 110, 149, "Time Left");
            AddLabel(100, 110, 2550, "29m 30s");

            AddLabel(20, 130, 149, "My Status");
            AddLabel(100, 130, GreenTextHue, "Controlling Zone A");
            */

            AddLabel(55, 5, 1164, "Order");
            AddLabel(55, 25, GreenTextHue, "40 [+0]");

            AddLabel(159, 5, 2115, "Chaos");
            AddLabel(160, 25, WhiteTextHue, "20 [+0]");

            AddLabel(20, 50, 149, "Capture Event");
            AddLabel(118, 50, 2615, "Wrong Dungeon");

            AddLabel(20, 70, 149, "Zone A");
            AddButton(76, 73, 2118, 2118, 0, GumpButtonType.Reply, 0);
            AddLabel(100, 70, WhiteTextHue, "Uncontrolled");

            AddLabel(20, 90, 149, "Zone B");
            AddButton(76, 93, 2118, 2118, 0, GumpButtonType.Reply, 0);
            AddLabel(100, 90, WhiteTextHue, "Uncontrolled");

            AddLabel(19, 110, 149, "Time Left");
            AddLabel(100, 110, 2550, "47m 57s");

            AddLabel(20, 130, 149, "My Status");
            AddLabel(100, 130, PinkTextHue, "Statloss 9m 53s");
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
        }
    }
}