using System;
using Server;
using Server.Gumps;
using System.Collections.Generic;
using Server.Items;
using Server.Misc;
using Server.Mobiles;

namespace Server.Gumps
{
    public class NameChangeGump : Gump
    {
        public static void Initialize()
        {
            EventSink.Login += new LoginEventHandler(OnLogin);
        }
        
        public static void OnLogin(LoginEventArgs e)
        {
            Mobile m = e.Mobile;

            if (m != null && m.RawName == "Generic Player")
            {
                Timer.DelayCall(TimeSpan.FromSeconds(2), delegate 
                {
					/*if (m is PlayerMobile && ((PlayerMobile)m).CreationTime + TimeSpan.FromDays(1) < DateTime.UtcNow)
					{
						m.SendMessage("Your character will be deleted if a new name is not chosen immediately.");
						Timer.DelayCall(TimeSpan.FromMinutes(5), delegate { ValidateCharacter(m); });
					}*/

                    if (m.HasGump(typeof(NameChangeGump)))
                        m.CloseGump(typeof(NameChangeGump));
                    
                    m.SendGump(new NameChangeGump(m, m.RawName)); 
                });
            }
        }

        public static void ValidateCharacter(Mobile m)
        {
            if (m != null && m.RawName == "Generic Player")
            {
                m.SendMessage("Your character is now being deleted for failing to pick a new name. Good bye.");
                Timer.DelayCall(TimeSpan.FromSeconds(5), delegate { m.Delete(); });
            }
        }

        public NameChangeGump(Mobile from, string name)
            : base(250, 125)
        {
            this.Closable = false;
            this.Disposable = false;
            this.Dragable = false;
            this.Resizable = false;
            this.AddPage(0);
            this.AddImageTiled(30, 29, 243, 245, 3504);
            this.AddImage(30, 4, 3501);
            this.AddImage(4, 4, 3500);
            this.AddImage(272, 4, 3502);
            this.AddImage(4, 30, 3503);
            this.AddImage(272, 29, 3505);
            this.AddImage(30, 271, 3507);
            this.AddImage(4, 271, 3506);
            this.AddImage(272, 271, 3508);
            this.AddHtml(30, 27, 241, 197, @"<center><h1>Welcome to UO Outlands!</h1></center><br>Unfortunately, the name you have chosen is either already taken by another player or is otherwise unacceptable. Many of the exciting new additions to UO An Corp hinge on all players having unique names. Please enter your selection for a new name below:", (bool)false, (bool)false);
            this.AddImage(27, 244, 2440);
            this.AddTextEntry(36, 246, 149, 16, 0, (int)Buttons.TextEntry, name);
            this.AddButton(198, 244, 239, 240, (int)Buttons.btnApply, GumpButtonType.Reply, 0);
        }

        public enum Buttons
        {
            Cancel,
            TextEntry,
            btnApply,
        }

        public override void OnResponse(Network.NetState sender, RelayInfo info)
        {
            if (info.ButtonID != (int)Buttons.btnApply)
                return;
            try 
            {
                Mobile m = sender.Mobile;

                TextRelay name = info.GetTextEntry((int)Buttons.TextEntry);

                string text = name.Text.Trim();

                short a;
                if (text.Length < 1 || Int16.TryParse(text.Substring(0,1), out a) || !NameVerification.Validate(text, 2, 16, true, true, true, 1, NameVerification.SpaceDashPeriodQuote, NameVerification.Disallowed, NameVerification.StartDisallowed))
                {
                    m.SendMessage("That name is either already taken or otherwise unnacceptable. Please choose a different name.");
                    
                    if (m.HasGump(typeof(NameChangeGump)))
                        m.CloseGump(typeof(NameChangeGump));

                    m.SendGump(new NameChangeGump(m, text));

                    return;
                }

                m.Name = text;
                m.SendMessage("You will henceforth be known as {0}.", text);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Name Change Gump OnResponse: {0}", ex.Message);
            }
        }
    }
}