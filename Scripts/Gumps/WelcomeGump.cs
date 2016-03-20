using System;
using Server;
using Server.Gumps;
using Server.Network;
using System.IO;

using System.Collections.Generic;
using Server.Commands;

namespace Server.Gumps
{
	public class WelcomeGump : Gump
	{
        public static void Initialize()
        {
            CommandSystem.Register("UpdatePatchNotes", AccessLevel.Player, new CommandEventHandler(UpdatePatchNotes_Command));
        }
        public static void UpdatePatchNotes_Command(CommandEventArgs e)
        {
            m_PatchNotesCache = null;
            e.Mobile.SendMessage("The patch notes cache has been cleared.");
        }

		public int m_PageNo = 0;

		public WelcomeGump(int PageNo) : base( 20, 20 )
		{
			this.Resizable = false;
		    this.Closable = true;
			this.Disposable = true;
			this.Dragable = true;
			AddPage(0);
			AddBackground( -1, 0, 400, 250, 0x2436 );

			string s = "";
         
            AddItem( 20, 25, 7774 );
			AddLabel(115, 20, 53, @"Welcome to UO Outlands!");
			AddButton(13, 105, 0x845, 0x846, 1, GumpButtonType.Reply, 0);
			AddLabel(35, 103, 1153, @"Rules");
			AddButton(13, 130, 0x845, 0x846, 2, GumpButtonType.Reply, 0);
			AddLabel(35, 127, 1153, @"Patch Notes");
			AddButton(13, 155, 0x845, 0x846, 3, GumpButtonType.Reply, 0);
			AddLabel(35, 152, 1153, @"Elections");
			AddButton(13, 180, 0x845, 0x846, 4, GumpButtonType.Reply, 0);
			AddLabel(35, 177, 1153, @"Wiki");
			AddButton(13, 205, 0x845, 0x846, 5, GumpButtonType.Reply, 0);
			AddLabel(35, 202, 1153, @"Website");

			m_PageNo = PageNo;
			switch (PageNo)
			{
				case 0:
				{
					s = "<i>Use the Quest button on your paperdoll to access most shard features and systems.</i><br><br>- Visit http://www.uoancorp.com for full shard rules and information.<br-> Use [yc for new player chat.<br>- No AFK macroing in dungeons.<br>- No AFK gathering.<br>- Do not ask for favors from staff.<br>- No racist or sexual harassment.";
					break;
				}
				case 1:
				{
                    s = ReadPatchNotes();			
						break;
				}
				case 2:
				{
                    /*
					s = "<B><Center><basefont size = 5>Town Election Status</Center></B><P>";

					for (int i = 0; i < Town.Towns.Count; i++)
					{
						var town = Town.Towns[i];
						var name = town.HomeFaction.Definition.FriendlyName;

						s += String.Format("<center><basefont color=#{1} size = 4>{0}<br><basefont color=#000000 size = 1>{2}</center><br>", name, getFont(name), electionStatus(town));
					}					
                    */

                    break;
				}
			}

			AddHtml(114, 45, 267, 186, s, true, true);
		}

        private static string m_PatchNotesCache;

        private string ReadPatchNotes()
        {
            if (m_PatchNotesCache == null)
            {
                string filePath = @"patchnotes.txt";
                using (StreamReader streamReader = new StreamReader(filePath))
                {
                    m_PatchNotesCache = streamReader.ReadToEnd();
                }
            }
            
            return m_PatchNotesCache;
        }

		public override void OnResponse(NetState state, RelayInfo info)
		{
			if (info.ButtonID == 0) // close/cancel
                return;
            if (info.ButtonID == 1)
			{
				state.Mobile.CloseGump(typeof(WelcomeGump));
				state.Mobile.SendGump(new WelcomeGump(0));
				m_PageNo = 0;
			}
			if (info.ButtonID == 2)
			{
				state.Mobile.CloseGump(typeof(WelcomeGump));
				state.Mobile.SendGump(new WelcomeGump(1));
				m_PageNo = 1;
			}
			if (info.ButtonID == 3)
			{
				state.Mobile.CloseGump(typeof(WelcomeGump));
				state.Mobile.SendGump(new WelcomeGump(2));
				m_PageNo = 2;

			}
			if (info.ButtonID == 4)
			{
				string url = "http://uoancorp.com/wiki/index.php/Main_Page";
				state.Mobile.LaunchBrowser(url);
				state.Mobile.SendGump(new WelcomeGump(m_PageNo));

			}
			if (info.ButtonID == 5)
			{
				string url = "http://www.uoancorp.com/";
				state.Mobile.LaunchBrowser(url);
				state.Mobile.SendGump(new WelcomeGump(m_PageNo));

			}
		}

		public String getFont(String name)
		{
			if (name == "Britain")
				return "bdde31";
			else if (name == "Serpent's Hold")
				return "ef9c94";
			else if (name == "Jhelom")
				return "a5ceef";
			else if (name == "Trinsic")
				return "e76300";
			else if (name == "Ocllo")
				return "4a00e7";
			else if (name == "Skara Brae")
				return "84d64a";
			else if (name == "Yew")
				return "efce42";
			else if (name == "Cove")
				return "424242";
			else if (name == "Minoc")
				return "736318";
			else if (name == "Nujel'm")
				return "946b52";
			else if (name == "Vesper")
				return "8cb5ef";
			else if (name == "Moonglow")
				return "ef8cd6";
			else if (name == "Magincia")
				return "840084";
			else
				return "0F0F0F";
		}
	}
}