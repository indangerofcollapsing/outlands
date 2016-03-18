using System;
using System.Net;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Server;
using Server.Items;
using Server.Prompts;
using Server.Network;
using Server.Accounting;
using Server.Commands;
using Server.Multis;
using Server.Misc;
using Server.Gumps;
using Server.Commands;
using Server.Targeting;

namespace Server
{
    public class AccountComments
    {
        public static void Initialize()
        {
            CommandSystem.Register("accountcomments", AccessLevel.Counselor, new CommandEventHandler(OnAccountComments));
            CommandSystem.Register("ac", AccessLevel.Counselor, new CommandEventHandler(OnAccountComments));
        }

        private static void OnAccountComments(CommandEventArgs e)
        {
            e.Mobile.Target = new SelectViewCommentsTarget(e.Mobile);
        }

        private class SelectViewCommentsTarget
         : Target
        {

            private Mobile m_Selected;
            public SelectViewCommentsTarget(Mobile who)
                : base(18, false, TargetFlags.None)
            {

                m_Selected = who;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is Mobile && ((Mobile)targeted).Player)
                {
                    if (from.HasGump(typeof(AccountCommentsGump)))
                        from.CloseGump(typeof(AccountCommentsGump));

                    from.SendGump(new AccountCommentsGump(targeted as Mobile, from,null));

                }
            }
        }
             
        public class AccountCommentsGump : Gump
        {
            #region had take from admin gump for 'same feel' look 
            private const int LabelColor = 0x7FFF;
            private const int SelectedColor = 0x421F;
            private const int DisabledColor = 0x4210;

            private const int LabelColor32 = 0xFFFFFF;
            private const int SelectedColor32 = 0x8080FF;
            private const int DisabledColor32 = 0x808080;

            private const int LabelHue = 0x480;
            private const int GreenHue = 0x40;
            private const int RedHue = 0x20;
            #endregion

            private Mobile m_Viewing;
            private Mobile m_Viewer;

            public AccountCommentsGump(Mobile view,Mobile from,string notice) : base(0,0)
            {
                m_Viewer = from;
                m_Viewing = view;
                AddPage(0);

                AddBackground(0, 100, 420, 340, 5054);
                AddBlackAlpha(10, 120, 400, 260);
                AddBlackAlpha(10, 390, 400, 40);


                if (notice != null)
                    AddHtml(12, 392, 396, 36, Color(notice, LabelColor32), false, false);

                if (view != null && from != null)
                    Show();
            }
         

            public void AddBlackAlpha(int x, int y, int width, int height)
            {
                AddImageTiled(x, y, width, height, 2624);
                AddAlphaRegion(x, y, width, height);
            }
            public void AddButtonLabeled(int x, int y, int buttonID, string text)
            {
                AddButton(x, y - 1, 4005, 4007, buttonID, GumpButtonType.Reply, 0);
                AddHtml(x + 35, y, 240, 20, Color(text, LabelColor32), false, false);
            }

            public string Center(string text)
            {
                return String.Format("<CENTER>{0}</CENTER>", text);
            }

            public string Color(string text, int color)
            {
                return String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text);
            }

            private void Show()
            {
                if (m_Viewing.NetState == null || m_Viewing.NetState.Account == null)
                    return;

                Account a = m_Viewing.Account as Account;

                if (a == null)
                    return;

                AddHtml(10, 125, 400, 20, Color(Center(string.Format("Comments: {0}", DateTime.UtcNow.ToString())), LabelColor32), false, false);

                AddButtonLabeled(20, 150, 8, "Add Comment");

                StringBuilder sb = new StringBuilder();

                if (a.Comments.Count == 0)
                    sb.Append("There are no comments for this account.");

                for (int i = 0; i < a.Comments.Count; ++i)
                {
                    if (i > 0)
                        sb.Append("<BR><BR>");

                    AccountComment c = a.Comments[i];

                    sb.AppendFormat("[{0} on {1}]<BR>{2}", c.AddedBy, c.LastModified, c.Content);
                }

                AddHtml(20, 180, 380, 190, sb.ToString(), true, true);
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                switch (info.ButtonID)
                {

                    case 8:  // just like the number that's all :-P
                        {
                            if (sender.Mobile != null)
                            {
                                sender.Mobile.Prompt = new AddCommentPrompt(m_Viewing);
                                sender.Mobile.SendMessage("Enter the new account comment.");
                            }

                            break;
                        }

                }
            }

            private class AddCommentPrompt : Prompt
            {
                private Account m_Account;
                private Mobile m_Owner;

                public AddCommentPrompt(Mobile m)
                {
                    m_Owner = m;
                    m_Account = m.Account as Account;
                }

                public override void OnCancel(Mobile from)
                {
                    from.SendGump(new AccountCommentsGump(m_Owner, from,"Request to add comment was canceled."));
                }

                public override void OnResponse(Mobile from, string text)
                {
                    if (m_Account != null)
                    {
                        m_Account.Comments.Add(new AccountComment(from.RawName, text));
                        from.SendGump(new AccountCommentsGump(m_Owner, from, "Comment added."));
                    }
                }
            }
        }
    }

}
