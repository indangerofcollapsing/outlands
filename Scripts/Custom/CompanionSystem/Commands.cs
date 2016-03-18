using System;
using System.Collections.Generic;
using System.Reflection;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Multis;
using Server.Menus.Questions;
using Server.Accounting;
using Server.Targeting;
using Server.Targets;
using Server.Gumps;
using Server.Engines.Help;
using Server.Regions;
using Server.Spells;
using Server.Custom;
using System.Linq;

namespace Server.Commands
{


    public class ListNewPlayers
    {
        public static void Initialize()
        {
            CommandSystem.Register("listnew", AccessLevel.Player, new CommandEventHandler(ListNP_Command));
            CommandSystem.Register("return", AccessLevel.Player, new CommandEventHandler(LastLoc_Command));

        }

        public static void LastLoc_Command(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm != null && pm.Companion)
            {
                if (pm.CompanionLastLocation != Point3D.Zero)
                {
                    if (pm.Criminal)
                    {
                        pm.SendLocalizedMessage(1005561, "", 0x22); // Thou'rt a criminal and cannot escape so easily.

                    }
                    else if (SpellHelper.CheckCombat(pm, true))
                    {
                        pm.SendLocalizedMessage(1005564, "", 0x22); // Wouldst thou flee during the heat of battle??

                    }
                    else if (!SpellHelper.CheckTravel(pm, TravelCheckType.RecallFrom))
                    {
                        pm.SendMessage("Companions may not teleport away from this location");
                    }
                    else
                    {
                        CommandLogging.WriteLine(pm, "{0} {1} teleporting from {2} to {3}", pm.AccessLevel, CommandLogging.Format(pm), new Point3D(pm.Location), pm.CompanionLastLocation);
                        pm.Location = pm.CompanionLastLocation;
                        pm.CompanionLastLocation = Point3D.Zero;
                    }
                }
                else
                    pm.SendAsciiMessage("You must first teleport to a new player before returning to your last location.");
            }
        }

        public static void ListNP_Command(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm != null && pm.Companion)
            {
                pm.SendGump(new NewPlayerList(pm));
            }
        }

        private class NewPlayerList : Gump
        {
            private enum Buttons
            {
                PageUp=-1,
                PageDown=-2,
            }

            private int m_Page = 0;
            private int m_Per = 18;
            private PlayerMobile m_From;
            private List<PlayerMobile> m_Youngs;

            public NewPlayerList(PlayerMobile companion, int page = 0) 
                : base(50, 50)
            {
                m_From = companion;
                m_Page = page;

                Closable=true;
			    Disposable=true;
			    Dragable=true;
			    Resizable=false;

			    AddPage(0);
			    AddBackground(14, 19, 205, 478, 9200);
			    AddLabel(85, 31, 1153, @"New Players");

                m_Youngs = PlayerMobile.YoungChatListeners.FindAll(p => p.Young).Skip(page * m_Per).Take(m_Per).Distinct().ToList();

                int startY = 60;
                for (int i = 0; i < m_Youngs.Count; i++)
                {
                    var player = m_Youngs[i];
			        AddLabel(60, startY, 0, player.Name);
			        AddButton(22, startY, 4005, 4007, i + 1, GumpButtonType.Reply, 0);
                    startY += 25;
                }

                if (PlayerMobile.YoungChatListeners.Count > m_Per * (m_Page + 1))
                    AddButton(191, 477, 2224, 2224, (int)Buttons.PageUp, GumpButtonType.Reply, 0);
                if (page > 0)
                    AddButton(164, 477, 2223, 2223, (int)Buttons.PageDown, GumpButtonType.Reply, 0);
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                base.OnResponse(sender, info);
                var player = sender.Mobile as PlayerMobile;
                if (!player.Companion) return;
                switch (info.ButtonID)
                {
                    case (int)Buttons.PageUp:
                        m_From.SendGump(new NewPlayerList(m_From, m_Page += 1));
                        break;
                    case (int)Buttons.PageDown:
                        m_From.SendGump(new NewPlayerList(m_From, m_Page -= 1));
                        break;
                    default:
                        int idx = info.ButtonID - 1;
                        if (idx >= 0 && idx < m_Youngs.Count)
                            TravelToPlayer(m_Youngs[idx]);
                        break;
                }
            }

            private void TravelToPlayer(PlayerMobile target)
            {
                if (target == null || target.NetState == null)
                {
                    m_From.SendMessage("That player is not online");
                    return;
                }

                BaseHouse house = BaseHouse.FindHouseAt(target);

                if (house != null && house.IsInside(target))
                {
                    m_From.SendAsciiMessage("This player is inside a house and you cannot teleport to them.");
                }
                else
                {
                    if (m_From.Criminal)
                    {
                        m_From.SendLocalizedMessage(1005561, "", 0x22); // Thou'rt a criminal and cannot escape so easily.

                    }
                    else if (SpellHelper.CheckCombat(m_From, true))
                    {
                        m_From.SendLocalizedMessage(1005564, "", 0x22); // Wouldst thou flee during the heat of battle??

                    }
                    else if (target.Map != Map.Felucca)
                    {
                        m_From.SendLocalizedMessage(1019004); // You are not allowed to travel there.
                    }
                    else if (!SpellHelper.CheckTravel(m_From, TravelCheckType.RecallFrom))
                    {
                        m_From.SendMessage("Companions may not teleport away from this location");
                    }
                    else if (target.Region.Name == "Hythloth" || (target.Region is Jail) || target.Region.Name == "Deceit")
                    {
                        m_From.SendMessage("Companions may not teleport to that area");
                    }
                    else
                    {
                        m_From.CompanionLastLocation = m_From.Location;
                        m_From.Hidden = true;
                        m_From.Location = target.Location;
                        m_From.CompanionTarget = target;
                        CommandLogging.WriteLine(m_From, "{0} {1} teleporting to {2} at {3}", m_From.AccessLevel, CommandLogging.Format(m_From), CommandLogging.Format(target), new Point3D(target.Location));

                    }

                }
            }
            
        }
    }

    public class RevokeYoungStatus
    {

        private class InternalTarget : Target
        {

            public InternalTarget(Mobile mobile)
                : base(20, false, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object target)
            {
                if (target is PlayerMobile)
                {
                    if (from != null && target != null && ((PlayerMobile)target).Young)
                    {
                        from.SendMessage("You have revoked their young status.");
                        ((PlayerMobile)target).SendMessage("You may no longer enjoy the benefits of young player status.");
                        ((PlayerMobile)target).Young = false;
                        CommandLogging.WriteLine(from, "{0} {1} revoked the young status of {2} at {3}", from.AccessLevel, CommandLogging.Format(from), CommandLogging.Format(((PlayerMobile)target)), new Point3D(from.Location));
                    }
                    else
                    {
                        from.SendMessage("You must target a young player.");
                    }
                }
                else
                {
                    from.SendMessage("You must target a young player.");
                }
            }


        }

        public static void Initialize()
        {
            CommandSystem.Register("revokeyoung", AccessLevel.Player, new CommandEventHandler(RevYoung_Command));
        }
        public static void RevYoung_Command(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm != null && pm.Companion)
            {
                pm.Target = new InternalTarget(pm);
            }
        }

    }
}