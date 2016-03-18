/***************************************************************************
 *                              YewJailBailGump.cs
 *                            -------------------
 *   begin                : July 2010
 *   author               : Sean Stavropoulos
 *   email                : sean.stavro@gmail.com
 *
 *
 ***************************************************************************/

using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Gumps;

namespace Server.YewJail
{
    public class YewJailBombGump : Gump
    {
        private Mobile m_Owner;
        private YewJailItem m_JailItem;
        public YewJailItem JailItem { get { return m_JailItem; } set { m_JailItem = value; } }
        public Mobile Owner { get { return m_Owner; } set { m_Owner = value; } }

        public YewJailBombGump(YewJail.YewJailItem item)
            : base(100, 10)
        {
            Mobile owner = item.m_Jailed;
            JailItem = item;
            owner.CloseGump(typeof(YewJailBailGump));
            owner.CloseGump(typeof(DuelGump));

            int gumpX = 0; int gumpY = 0; //bool initialState = false;

            m_Owner = owner;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = true;

            AddPage(0);

            gumpX = 0; gumpY = 0;

            AddBackground(gumpX, gumpY, 450, 200, 0xE10);

            gumpX = 15; gumpY = 15;
            AddAlphaRegion(gumpX, gumpY, 420, 170);

            gumpX = 15; gumpY = 15;
            AddHtml(gumpX, gumpY, 420, 20, "<body text=white><center>Bomb Confirmation</center></body>", false, false);

            gumpX = 15; gumpY = 70;
            AddHtml(gumpX, gumpY, 420, 20, "<body text=white><center>Would you like to try and create a bomb?</center></body>", false, false);

            //gumpX = 15; gumpY = 90;
            //AddHtml(gumpX, gumpY, 420, 20, "<body text=white><center>in exchange for 12 hours of jailtime credit.</center></body>", false, false);

            gumpX = 125; gumpY = 140;
            AddButton(gumpX, gumpY, 0xF8, 0xF7, 2, GumpButtonType.Reply, 0);

            gumpX = 255; gumpY = 140;
            AddButton(gumpX, gumpY, 0xF2, 0xF1, 3, GumpButtonType.Reply, 0);

            gumpX = 15; gumpY = 110;


            //AddHtml(gumpX, gumpY, 420, 20, "<body text=white><center>Do you wish to post bail?</center></body>", false, false);


            AddPage(1);
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;

            switch (info.ButtonID)
            {
                case 2:
                    if (TryMakeBomb(from)) //OKAY
                    {
                        JailItem.PostBail(TimeSpan.FromHours(12));
                    }
                    break;
                case 3:
                    break;
            }
        }

        private bool TryMakeBomb(Mobile from)
        {
            Item i1 = from.Backpack.FindItemByType(typeof(HideableSilverWire));
            Item i2 = from.Backpack.FindItemByType(typeof(Manure));
            Item i3 = from.Backpack.FindItemByType(typeof(Flint));

            if (i1 == null || i2 == null || i3 == null)
            {
                from.LocalOverheadMessage(MessageType.Regular, from.SpeechHue, true, "You do not have the required items anymore!");
                return false;
            }
            else
            {
                i1.Delete();
                i2.Delete();
                i3.Delete();
                double tinkeringSkill = from.Skills.Tinkering.Base;
                from.PlaySound(0x241);
                double tinkmod = tinkeringSkill - 80.0;
                if (tinkmod > 0)
                {
                    bool b = (Utility.Random(1000) < (10 + tinkmod) * 10);
                    TinkerTimer m_timer = new TinkerTimer(this, b, from);
                    m_timer.Start();
                    return b;
                }
                else
                {
                    TinkerTimer m_timer = new TinkerTimer(this, false, from);
                    m_timer.Start();
                    return false;
                }
            }
        }

        public void MakeBomb(Mobile from, bool succeed)
        {
            if (succeed)
            {
                m_JailItem.BombHelper.m_Timer.Step = 8;
                from.LocalOverheadMessage(MessageType.Regular, from.SpeechHue, true, "You successfully complete the bomb!");
            }
            else
            {
                m_JailItem.BombHelper.m_Timer.Step = 11;
                from.LocalOverheadMessage(MessageType.Regular, from.SpeechHue, true, "You fail to make the bomb!");
            }
        }

        public class TinkerTimer : Timer
        {
            DateTime m_End;
            YewJailBombGump m_Gump;
            Mobile m_from;
            bool m_succeed;

            public TinkerTimer(YewJailBombGump gump, bool succeed, Mobile from)
                : base(TimeSpan.Zero, TimeSpan.FromMilliseconds(500))
            {
                m_Gump = gump;
                m_End = DateTime.UtcNow + TimeSpan.FromSeconds(5);
                m_succeed = succeed;
                m_from = from;
            }

            protected override void OnTick()
            {
                if (DateTime.UtcNow > m_End)
                {
                    m_Gump.MakeBomb(m_from, m_succeed);
                    this.Stop();
                }
            }
        }
    }
    
}
