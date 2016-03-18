using System;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Custom;
using Server.Gumps;

namespace Server.Items
{
    public class ConvertPowerscrollDeedGump : Gump
    {
        public static int InfluenceLotteryTicketsGiven = 3;

        public PlayerMobile m_Player;
        public PowerScroll m_Powerscroll;
        public Point3D m_PowerscrollLocation;

        public ConvertPowerscrollDeedGump(PowerScroll powerscroll, PlayerMobile player): base(10, 10)
        {
            if (player == null) return;
            if (player.Deleted) return;
            if (powerscroll == null) return;
            if (powerscroll.Deleted) return;

            m_Player = player;
            m_Powerscroll = powerscroll;
            m_PowerscrollLocation = powerscroll.Location;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddImage(10, 10, 103);
            AddImage(125, 10, 103);
            AddImage(10, 100, 103);
            AddImage(125, 100, 103);
            AddImage(20, 61, 200);
            AddImage(126, 61, 200);
            AddImage(20, 20, 200);
            AddImage(126, 20, 200);

            int textHue = 2036;
            
            AddLabel(72, 23, 149, "Convert Powerscroll");

            AddItem(73, 49, 5360, 1153);
            AddLabel(128, 55, textHue, "->");
            AddItem(149, 55, 5357, 2615);

            AddLabel(30, 87, textHue, "This will destroy the Powerscroll");
            AddLabel(43, 107, textHue, "and create " + InfluenceLotteryTicketsGiven.ToString() + " Influence Lottery");
            AddLabel(102, 127, textHue, "tickets");

            AddButton(31, 161, 247, 248, 1, GumpButtonType.Reply, 0);
            AddLabel(107, 161, 2208, "Proceed?");   
            AddButton(179, 160, 242, 241, 2, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            PlayerMobile player = sender.Mobile as PlayerMobile;

            if (info.ButtonID == 1)
            {
                if (player == null) return;
                if (player.Deleted) return;
                if (player.Backpack == null) return;
                if (m_Powerscroll == null)
                {
                    player.SendMessage("That powerscroll is no longer accessible.");
                    return;
                }

                if (m_Powerscroll.Deleted)
                {
                    player.SendMessage("That powerscroll is no longer accessible.");
                    return;
                }

                if (m_Powerscroll.Location != m_PowerscrollLocation)
                {
                    player.SendMessage("That powerscroll is no longer accessible.");
                    return;
                }

                if (player.Backpack.Items.Count + InfluenceLotteryTicketsGiven - 1 > player.Backpack.MaxItems)
                {
                    player.SendMessage("Your backpack currently has too many items to receive all items that would be created. Please make some space and try again.");
                    return;
                }

                player.SendSound(0x5B5);
                player.SendMessage("You convert the powerscroll into " + InfluenceLotteryTicketsGiven + " influence lottery tickets.");

                m_Powerscroll.Delete();

                for (int a = 0; a < InfluenceLotteryTicketsGiven; a++)
                {
                    player.Backpack.DropItem(new InfluenceLotteryTicket());
                }
            }
        }
    }
}
