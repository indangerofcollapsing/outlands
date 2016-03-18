using System;
using Server;
using Server.Gumps;
using Server.Network;

namespace Server.Chat
{
	public class ChatSystem
	{
		public static void Initialize()
		{
			EventSink.ChatRequest += new ChatRequestEventHandler( EventSink_ChatRequest );
		}

		private static void EventSink_ChatRequest( ChatRequestEventArgs e )
		{
			if (e.Mobile != null && e.Mobile.NetState != null)
			{
				Server.Engines.Chat.ChatSystem.StartChat(e.Mobile.NetState);
			}
			//e.Mobile.SendMessage( "Chat is not currently supported." );
		}
	}
}