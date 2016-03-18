using System;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Custom;
using Server.Gumps;

namespace Server.Items
{
    public class UOACZConsumeFoodGump : Gump
    {
        public PlayerMobile m_Player;
        public UOACZConsumptionItem m_ConsumptionItem;
        public Point3D m_ConsumptionItemLocation;

        public UOACZConsumeFoodGump(UOACZConsumptionItem consumptionItem, PlayerMobile player): base(10, 10)
        {
            if (player == null) return;
            if (player.Deleted) return;
            if (consumptionItem == null) return;
            if (consumptionItem.Deleted) return;

            m_Player = player;
            m_ConsumptionItem = consumptionItem;
            m_ConsumptionItemLocation = consumptionItem.Location;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            int textHue = 2036;

            AddImage(10, 10, 103);
            AddImage(125, 10, 103);
            AddImage(10, 100, 103);
            AddImage(125, 100, 103);
            AddImage(20, 61, 200);
            AddImage(126, 61, 200);
            AddImage(20, 20, 200);
            AddImage(126, 20, 200);
            
            AddItem(116, 56, m_ConsumptionItem.ItemID, m_ConsumptionItem.Hue);
            
            switch (m_ConsumptionItem.ConsumptionQualityType)
            {
                case UOACZConsumptionItem.ConsumptionQuality.Raw:
                    AddLabel(Utility.CenteredTextOffset(150, "Consume Raw Food"), 23, UOACZSystem.yellowTextHue, "Consume Raw Food");
                    AddLabel(Utility.CenteredTextOffset(150, "Consuming raw food will cause"), 87, textHue, "Consuming raw food will cause");
                    AddLabel(Utility.CenteredTextOffset(150, "mild disease damage over"), 107, textHue, "mild disease damage over");
                break;

                case UOACZConsumptionItem.ConsumptionQuality.Corrupted:
                    AddLabel(Utility.CenteredTextOffset(150, "Consume Corrupted Food"), 23, 2208, "Consume Corrupted Food");
                    AddLabel(Utility.CenteredTextOffset(160, "Consuming corrupted food will cause"), 87, textHue, "Consuming corrupted food will cause");
                    AddLabel(Utility.CenteredTextOffset(150, "serious disease damage over"), 107, textHue, "serious disease damage over");
                break;
            }

            AddLabel(102, 127, textHue, UOACZSystem.FoodDiseaseSeconds.ToString() + " seconds"); 

            AddButton(31, 161, 247, 248, 1, GumpButtonType.Reply, 0);
            AddLabel(112, 161, 2603, "Proceed?");   
            AddButton(179, 160, 242, 241, 2, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            PlayerMobile player = sender.Mobile as PlayerMobile;

            if (player == null)
                return;

            if (info.ButtonID == 1)
            {
                if (player == null) return;
                if (player.Deleted) return;
                if (player.Backpack == null) return;
                if (m_ConsumptionItem == null)
                {
                    player.SendMessage("That item is no longer accessible.");
                    return;
                }

                if (m_ConsumptionItem.Deleted)
                {
                    player.SendMessage("That item is no longer accessible.");
                    return;
                }

                if (m_ConsumptionItem.Location != m_ConsumptionItemLocation)
                {
                    player.SendMessage("That item is no longer accessible.");
                    return;
                }

                m_ConsumptionItem.Consume(player);
            }
        }
    }
}
