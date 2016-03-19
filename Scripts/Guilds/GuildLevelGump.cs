using Server.Guilds;
using Server.Gumps;
using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.ExtensionMethods;
using Server.Network;

namespace Server.Custom.Guilds
{
    class GuildLevelGump : Gump
    {
        private PlayerMobile m_From;
        private Guild m_Guild;
        private bool m_Leader = false;

        private enum Buttons
        {
            Purchase =1
        }

        public GuildLevelGump(PlayerMobile from, Guild guild)
            : base(0, 0)
        {
            m_From = from;
            m_Guild = guild;
            m_Leader = BaseGuildGump.IsLeader(from, guild);

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage(0);
            AddBackground(0, 0, 599, 478, 9200);
            AddImage(-2, 0, 5500);
            AddImage(280, 207, 1418);
            AddImage(564, 91, 10441);

            AddLabel(250, 60, 1153, m_Guild.Name);


            AddImage(225, 105, 2501);
            AddLabel(269, 105, 1153, string.Format("{0} / {1}", m_Guild.Experience, Guild.ExperienceLevels[m_Guild.Level]));
            AddLabel(100, 105, 1153, string.Format("Current Level: {0}", (int)guild.Level));
            var next = guild.Level == GuildLevel.Ten ? 10 : (int)guild.Level + 1;
            AddLabel(400, 105, 1153, string.Format("Next Level: {0}", next));

            AddLabel(100, 140, 1153, @"Current Bonuses:");
            AddLabel(110, 160, 1266, string.Format("+{0}% Luck", (int)guild.Level));
            AddLabel(110, 180, 1359, string.Format("+{0}% Gold", (int)guild.Level));

            AddLabel(235, 140, 1153, @"Current Abilities:");
            var abilities = (GuildBonus[])Enum.GetValues(typeof(GuildBonus));
            int y = 160;

            foreach (var ability in abilities)
            {
                if (!m_Guild.HasBonus(ability))
                    continue;
                AddLabel(245, y, 1153, ability.GetAttribute<DescriptionAttribute>().Description);
                y += 20;
            }

            if (m_Guild.MeetsExperienceRequirement() && m_Leader && !m_Guild.CanPurchaseUnlock())
            {
                AddLabel(70, 365, 1359, "Congratulations!");
                AddLabel(70, 385, 1359, "You can now purchase your next guild level for");
                AddLabel(70, 405, 1359, string.Format("{0} treasury gold.", Guild.UnlockPrices[m_Guild.Level + 1]));
            }

            if (m_Guild.CanPurchaseUnlock() && m_Leader)
            {
                AddLabel(70, 345, 1153, @"Purchase Upgrade:");
                AddButton(70, 395, 239, 238, (int)Buttons.Purchase, GumpButtonType.Reply, 0);
                AddButton(140, 395, 242, 241, 0, GumpButtonType.Reply, 0);
                AddLabel(70, 365, 1359, string.Format("Cost: {0}", Guild.UnlockPrices[m_Guild.Level + 1]));
            }
            
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == (int)Buttons.Purchase)
            {
                if (m_Guild.CanPurchaseUnlock() && m_Leader)
                {
                    m_Guild.PurchaseUnlock();
                }
            }
            else
            {
                m_From.SendGump(new GuildInfoGump(m_From, m_Guild));
            }
        }
    }
}
