/***************************************************************************
 *                            CreateCannonGump.cs
 *                            ------------------
 *   begin                : August 2010
 *   author               : Sean Stavropoulos
 *   email                : sean.stavro@gmail.com
 *
 *
 ***************************************************************************/
/*using System;
using System.Collections.Generic;
using Server.Gumps;
using System.Text;
using Server.Mobiles;
using Server.Multis;
using Server.Network;
using Server.Items;

namespace Server.Custom.Pirates
{
    public class CreateCannonGump : Gump
    {
        private Mobile m_From;
        private BaseCannonPlans m_Plans;
        private int m_SuccessChance;
        private int m_Boards;
        private int m_Ingots;
        private CannonTypes m_type;

        public CreateCannonGump(Mobile from, BaseCannonPlans plans, CannonTypes type)
            : base(150, 200)
        {
            m_From = from;
            m_Plans = plans;
            m_type = type;

            if (type == CannonTypes.Light)
            {
                m_Boards = 1000;
                m_Ingots = 5000;
            }
            else
            {
                m_Boards = 5000;
                m_Ingots = 10000;
            }

            m_From.CloseGump(typeof(ConfirmDockGump));

            AddPage(0);

            AddBackground(0, 0, 400, 400, 5054);
            AddBackground(10, 10, 380, 380, 3000);

            //AddHtmlLocalized(20, 20, 180, 80, 1018319, true, false); // Do you wish to dry dock this boat?
            AddHtml(20, 20, 360, 20, "<center>Cannon Creation</center>", false, false);

            AddHtml(20, 70, 360, 20, "In order to make a Light Cannon, you need the following", false, false);
            AddHtml(20, 90, 360, 20, "materials in your possession:", false, false);

            AddHtml(20, 110, 360, 20, string.Format("     1. {0} Boards", m_Boards), false, false);
            AddHtml(20, 130, 360, 20, string.Format("     2. {0} Iron Ore", m_Ingots), false, false);
            AddHtml(20, 150, 360, 20, "     3. a Saw", false, false);
            AddHtml(20, 170, 360, 20, "     4. a pair of Tongs", false, false);
            AddHtml(20, 190, 360, 20, "     5. Tinker Tools", false, false);

            m_SuccessChance = DetermineChanceOfSuccess(from);
            AddHtml(20, 210, 360, 20, String.Format("Based on your skills, you have a {0}% chance of creating", m_SuccessChance), false, false);
            AddHtml(20, 230, 360, 20, "a light cannon.", false, false);

            //AddHtml(20, 20, 360, 20, "Cannon Creation", true, false);

            AddHtml(55, 320, 250, 25, "ATTEMPT TO CREATE A CANNON", false, false); // CONTINUE
            AddButton(20, 320, 4005, 4007, 2, GumpButtonType.Reply, 0);

            AddHtmlLocalized(55, 350, 140, 25, 1011012, false, false); // CANCEL
            AddButton(20, 350, 4005, 4007, 1, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (info.ButtonID == 2)
            {
                if (!(m_Plans.IsChildOf( state.Mobile.Backpack)))
                {
                    state.Mobile.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                }
                else if (!CheckForItems(state.Mobile))
                {
                    state.Mobile.SendMessage("You don't have the required items.");
                }
                else
                {
                    m_Plans.m_Used = true;
                    state.Mobile.PlaySound(0x241);
                    Timer m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(5), new TimerStateCallback(OnResponse_Callback), new object[] { state.Mobile });
                }
            }
        }

        public void OnResponse_Callback(object state)
        {
            object[] states = (object[])state;
            Mobile from = (Mobile)states[0];

            bool success = (Utility.Random(100) < m_SuccessChance);
            m_Plans.GumpOK(from,success);
        }

        private int DetermineChanceOfSuccess(Mobile from)
        {
            double skillTink = from.Skills.Tinkering.Base;
            double skillBlacksmith = from.Skills.Blacksmith.Base;
            double skillCarpentry = from.Skills.Carpentry.Base;
            if (m_type == CannonTypes.Light)
            {
                if (skillTink < 90.0 || skillBlacksmith < 90.0 || skillCarpentry < 90.0)
                    return 0;

                double SkillMod = (skillTink - 90.0) + (skillBlacksmith - 90.0) + (skillCarpentry - 90.0);
                return (int)(40.0 + SkillMod);
            }
            else
            {
                if (skillTink < 100.0 || skillBlacksmith < 100.0 || skillCarpentry < 100.0)
                    return 0;

                return 50;
            }

        }

        private bool CheckForItems(Mobile from)
        {
            Tongs tongs = from.Backpack.FindItemByType(typeof(Tongs)) as Tongs;
            Saw saw = from.Backpack.FindItemByType(typeof(Saw)) as Saw;
            TinkerTools tools = from.Backpack.FindItemByType(typeof(TinkerTools)) as TinkerTools;

            if (m_Plans.CurrentOre < m_Plans.m_MaxIngots || m_Plans.m_CurrentBoards < m_Plans.m_MaxBoards || tongs == null || saw == null || tools == null)
                return false;

            return true;
        }
    }
}
*/