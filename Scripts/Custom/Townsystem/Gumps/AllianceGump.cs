using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using System.Collections.Generic;

namespace Server.Custom.Townsystem
{
    public class AllianceGump : Gump
    {
        private Mobile m_From;
        private Faction m_Faction;
        private Town m_Town;

        private void CenterItem(int itemID, int x, int y, int w, int h)
        {
            Rectangle2D rc = ItemBounds.Table[itemID];
            AddItem(x + ((w - rc.Width) / 2) - rc.X, y + ((h - rc.Height) / 2) - rc.Y, itemID);
        }

        public AllianceGump(Mobile from, Town town)
            : base(50, 50)
        {
            m_From = from;
			m_Faction = town.HomeFaction;
            m_Town = town;

            if (m_Faction == null)
                return;

            bool isKing = town.IsKing(from);
            bool isAllianceLeader = m_Faction.Alliance != null && m_Faction.Alliance.Leader == m_Faction;


            AddPage(0);

            AddBackground(0, 0, 320, 410, 5054);
            AddBackground(10, 10, 300, 390, 3000);

            #region General
            AddPage(1);

            AddHtml(20, 30, 260, 25, "ALLIANCES", false, false);

            AddHtml(55, 90, 200, 25, "View Alliances", false, false);
            AddButton(20, 90, 4005, 4007, 0, GumpButtonType.Page, 2);

            // Prevent king's from changing alliance until OCB is changed to town militia
            // - Jimmy
            if (from.AccessLevel >= AccessLevel.GameMaster) // (isKing)
            {
                AddHtml(55, 120, 200, 25, "Change Alliance", false, false);
                AddButton(20, 120, 4005, 4007, 0, GumpButtonType.Page, 3);
            }

            AddHtmlLocalized(55, 360, 200, 25, 1011441, false, false); // Exit
            AddButton(20, 360, 4005, 4007, 0, GumpButtonType.Reply, 0);
            #endregion

            #region View Alliances
            AddPage(2);

            AddHtml(20, 30, 300, 25, "EXISTING ALLIANCES", false, false); // FINANCE STATEMENT

            int spacer = -1;

            for (int i = 0; i < AllianceInfo.Alliances.Count; ++i)
            {
                var alliance = AllianceInfo.Alliances[i];
                var leader = alliance.Leader;
                var members = alliance.Members;

                AddLabel(20, 60 + ++spacer * 20, 0, String.Format("{0} Alliance Leader:", alliance.Name));
                AddLabel(180, 60 + spacer * 20, leader.Definition.HuePrimary -1, leader.Definition.FriendlyName);

                for (int j = 0; j < members.Count; ++j)
                {
                    var fac = members[j];
					AddLabel(50, 60 + ++spacer * 20, fac.Definition.HuePrimary -1, fac.Definition.FriendlyName);
                }
            }

            AddHtmlLocalized(55, 360, 200, 25, 1011067, false, false); // Previous page
            AddButton(20, 360, 4005, 4007, 0, GumpButtonType.Page, 1);
            #endregion

            if (isKing)
            {
                AddPage(3);
                AddHtml(20, 30, 300, 25, "ALLIANCES", false, false); // FINANCE STATEMENT

                int spacer2 = -1;
                for (int i = 0; i < AllianceInfo.Alliances.Count; ++i)
                {
                    var alliance = AllianceInfo.Alliances[i];
                    var leader = alliance.Leader;

                    AddLabel(20, 80 + ++spacer2 * 20, 0, String.Format("{0} Alliance Leader:", alliance.Name));
					AddLabel(180, 80 + spacer2 * 20, leader.Definition.HuePrimary - 1, leader.Definition.FriendlyName);

                    AddHtml(90, 80 + ++spacer2 * 20, 200, 25, "Join Alliance", false, false);
                    AddButton(55, 80 + spacer2 * 20, 4005, 4007, 10 + i, GumpButtonType.Reply, 0);
                }
                AddHtmlLocalized(55, 360, 200, 25, 1011067, false, false); // Previous page
                AddButton(20, 360, 4005, 4007, 0, GumpButtonType.Page, 1);
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 0)
                return;

            Mobile from = sender.Mobile;

            var alliance = m_Faction.Alliance;
            // Prevent king's from changing alliance until OCB is changed to town militia
            // - Jimmy
            bool isKing = from.AccessLevel >= AccessLevel.GameMaster;//m_Town.IsKing(from);
            bool isAllianceLeader = alliance != null && alliance.Leader == m_Faction;

			if (!isKing || m_Town.HomeFaction != m_Faction)
            {
                m_From.SendLocalizedMessage(1010339); // You no longer control this city
                return;
            }

			if (isAllianceLeader)
			{
				m_From.SendMessage("As the alliance leader you choose to not change your allegiance"); // You no longer control this city
				return;
			}

			if (info.ButtonID >= 10 && info.ButtonID < 20)
			{
				if (m_Faction.State.NextAllowedAllianceChange > DateTime.UtcNow)
				{
					int days = (m_Faction.State.NextAllowedAllianceChange - DateTime.UtcNow).Days;
					int hours = (m_Faction.State.NextAllowedAllianceChange - DateTime.UtcNow).Hours;
					from.SendMessage("You can not change alliance for another {0} days, and {1} hours", days, hours);
				}
				else
				{
					int index = info.ButtonID - 10;
					if (index >= 0 && index < AllianceInfo.Alliances.Count)
					{
						AllianceInfo.Alliances[index].AddFaction(m_Faction);
					}
				}
			}
            from.SendGump(new AllianceGump(from, m_Town));
        }
    }
}