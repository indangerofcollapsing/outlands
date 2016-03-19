using Server.Gumps;
using Server.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Battlegrounds.Gumps
{
    public class BattlegroundAdminGump : Gump
    {
        public BattlegroundAdminGump()
            : base(50, 50)
        {
            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            AddPage(0);
            AddBackground(53, 75, 344, 331, 9200);
            AddLabel(148, 90, 0, @"Battleground Admin (Not Persisted)");
            AddLabel(196, 117, 0, @"Enabled");
            AddLabel(276, 118, 0, @"Free Consume");

            int y = 145;
            for (int i = 0; i < Battleground.Instances.Count; i++)
            {
                var bg = Battleground.Instances[i];

                int buttonID = (i + 1) * 10;

                AddLabel(70, y, 0, bg.Name);
                if (bg.Enabled)
                    AddButton(210, y, 1895, 1896, buttonID, GumpButtonType.Reply, 0);
                else
                    AddButton(210, y, 1896, 1895, buttonID, GumpButtonType.Reply, 0);

                if (bg.FreeConsume)
                    AddButton(290, y, 1895, 1896, buttonID + 1, GumpButtonType.Reply, 0);
                else
                    AddButton(290, y, 1896, 1895, buttonID + 1, GumpButtonType.Reply, 0);
                y += 20;
            }

        }


        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            switch (info.ButtonID)
            {
                case 0:
                    break;
                default:
                    if (info.ButtonID % 10 == 0)
                    {
                        int index = (info.ButtonID / 10) - 1;
                        if (index >= Battleground.Instances.Count)
                            return;

                        var battleground = Battleground.Instances[index];
                        battleground.Enabled = !battleground.Enabled;

                        from.SendMessage(string.Format("{0} has been updated.", battleground.Name));
                        from.SendGump(new BattlegroundAdminGump());
                    }
                    else
                    {
                        int index = (int)Math.Ceiling(info.ButtonID / 10.0) - 2;

                        if (index >= Battleground.Instances.Count)
                            return;

                        var battleground = Battleground.Instances[index];
                        battleground.FreeConsume = !battleground.FreeConsume;

                        from.SendMessage(string.Format("{0} has been updated.", battleground.Name));
                        from.SendGump(new BattlegroundAdminGump());
                    }
                    break;

            }
        }
    }
}
