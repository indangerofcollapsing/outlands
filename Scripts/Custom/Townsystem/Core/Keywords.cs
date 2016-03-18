using System;
using System.Collections;
using Server;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;
using Server.Multis;
using System.Collections.Generic;
using Server.Commands;
using Server.Custom.Townsystem;

namespace Server.Custom.Townsystem
{
	public class Keywords
	{
		public static void Initialize()
		{
			EventSink.Speech += new SpeechEventHandler( EventSink_Speech );
            CommandSystem.Register("RemoveFactionTitles", AccessLevel.Administrator, RemoveFactionTitles_Handler);

		}

        private static readonly string[] oldTitles = new string[] { "Squire of Order", "Defender of Order", "Knight of Order", "Order Sentinel", "Vindicator", "Sacred Sword of Order", "Crusader Dealer", "Champion of Order", "Hero of Order", "Servant of Chaos", "Purveyor of Chaos", "Knight of Chaos", "Chaos Dragoon", "Chaos Zealot", "Avenger of Chaos", "Death Dealer", "Champion of Chaos", "Avatar of Chaos" };

        public static void RemoveFactionTitles_Handler(CommandEventArgs e)
        {
            RemoveFactionTitles();
        }

        public static void RemoveFactionTitles()
        {
            PlayerMobile pm;

            foreach (Mobile m in World.Mobiles.Values)
            {
                if (!(m is PlayerMobile))
                    continue;

                pm = m as PlayerMobile;

                for (int i = 0; i < oldTitles.Length; ++i)
					pm.RemovePrefixTitle = oldTitles[i];
            }
        }

		private static void ShowScore_Sandbox( object state )
		{
			PlayerState pl = (PlayerState)state;

			if ( pl != null )
				pl.Mobile.PublicOverheadMessage( MessageType.Regular, pl.Mobile.SpeechHue, true, pl.KillPoints.ToString( "N0" ) ); // NOTE: Added 'N0'
		}

		private static void ShowNearbyCitizenships(PlayerMobile from)
		{
            if (from == null)
                return;

            try 
            {
    			IPooledEnumerable eable = from.Map.GetMobilesInRange(from.Location, 15);
    			foreach (Mobile m in eable)
    			{
    				PlayerMobile pm = m as PlayerMobile;
    				if (pm != null)
    				{
    					if (from.AccessLevel == AccessLevel.Player && pm.Hidden)
    						continue;

    					if(from.AccessLevel < pm.AccessLevel)
    						continue;

    					bool citizen = pm.CitizenshipPlayerState != null;
    					Custom.Townsystem.PlayerState ps = pm.TownsystemPlayerState;
    					bool faction_member = ps != null && ps.Faction != null;
    					if( faction_member )
    					{
    						// militia citizen
    						string s = "*I am fighting for " + pm.CitizenshipPlayerState.Town.Definition.FriendlyName + "*";
    						pm.PrivateOverheadMessage(MessageType.Regular, ps.Faction.Definition.HuePrimary, true, s, from.NetState);
    					}
    					else if( citizen )
    					{
    						// regular citizen
    						string s = "*I am from " + pm.CitizenshipPlayerState.Town.Definition.FriendlyName+"*";
    						pm.PrivateOverheadMessage(MessageType.Regular, pm.CitizenshipPlayerState.Town.HomeFaction.Definition.HuePrimary, true, s, from.NetState);
    					}
    					else
    					{
    						// not a citizen
    						pm.PrivateOverheadMessage(MessageType.Regular, 0, true, "*I wander Britannia*", from.NetState);
    					}
    				}
    			}
                eable.Free();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ShowNearbyCitizenships error {0}", ex.Message);
            }
		}

		private static void EventSink_Speech( SpeechEventArgs e )
		{
			Mobile from = e.Mobile;
			int[] keywords = e.Keywords;

            string text = e.Speech.Trim().ToLower();
			if( text.IndexOf("where art thou from") != -1)
			{
				ShowNearbyCitizenships(from as PlayerMobile);
			}
			if (text.IndexOf("i wish to become a citizen") != -1)
            {
                PlayerMobile pm = (PlayerMobile)from;
                Town town = Town.FromRegion(pm.Region);

                if (town == null || pm.Citizenship != null)
                    return;

                pm.SendGump(new CitizenshipGump(town));
            }
            else if (text.IndexOf("revoke my citizenship") != -1)
            {
                PlayerMobile pm = (PlayerMobile)from;

                if (pm.Citizenship == null)
                    return;

                if (pm.CitizenshipPlayerState.IsLeaving)
                {
                    pm.SendGump(new CitizenshipRevokeCancelGump(from));
                    return;
                }

                if (!Town.CanLeave(from))
                    return;

				pm.CitizenshipPlayerState.Leaving = DateTime.Now;
                from.SendMessage("Your town citizenship will expire in one hour.");
                Timer.DelayCall(TimeSpan.FromHours(1.01), delegate { Town.CheckLeaveTimer(from); });
            }
            else if (text.IndexOf("resign from the militia") != -1)
            {
                PlayerMobile pm = (PlayerMobile)from;
                if (pm.Citizenship == null)
                    return;

                if (Faction.Find(from) == null)
                    return;

                if (pm.TownsystemPlayerState.IsLeaving)
                {
                    pm.SendGump(new MilitiaRevokeCancelGump(from));
                    return;
                }

				pm.TownsystemPlayerState.Leaving = DateTime.Now + TimeSpan.FromHours(24);
                from.SendMessage("Your faction obligations will expire in 24 hours.");
            }
            else if (text.IndexOf("message faction") != -1)
            {
                Faction faction = Faction.Find(from);

                if (faction == null || !Town.CheckCitizenship(from).IsKing(from))
                    return;

                if (from.AccessLevel == AccessLevel.Player && !Town.CheckCitizenship(from).FactionMessageReady)
                    from.SendLocalizedMessage(1010264); // The required time has not yet passed since the last message was sent
                else
                    faction.BeginBroadcast(from);
            }
            else if (text.IndexOf("i am king") != -1 || text.IndexOf("i am queen") != -1 || text.IndexOf("i am commander") != -1)
            {
                Town town = Town.FromRegion(from.Region);

                if (town == null || !(town.IsKing(from) || town.IsCommander(from)) || !from.Alive)
                    return;

                if (from.HasGump(typeof(SheriffGump)))
                    from.SendLocalizedMessage(1042160); // You already have a faction menu open.
				else if (town.ControllingTown != null)
					from.SendGump(new SheriffGump((PlayerMobile)from, town.HomeFaction, town));

            }
            else if (text.IndexOf("i wish to resign as king") != -1)
            {

            }
            else if (text.IndexOf("appoint") != -1 && text.IndexOf("commander") != -1)
            {
                Town town = Town.FromRegion(from.Region);

                if (town == null || !town.IsKing(from) || !from.Alive)
                    return;

                town.BeginCommanderAppointing(from);
            }
            else if (text.IndexOf("i exile ") != -1)
            {
                Town town = Town.CheckCitizenship(from);
                if (town != null && (town.IsKing(from)))
                {
                    if (!town.CanExile && from.AccessLevel == AccessLevel.Player)
                    {
                        from.SendMessage("You cannot exile again so soon.");
                    }
                    else if (town.ExilesDisabled)
                    {
                        from.SendMessage("Exiling has been temporarily disabled.");
                    }
                    else
                    {
                        int startLoc = text.IndexOf("i exile");
                        string strToExile = text.Substring(startLoc + 7, text.Length - startLoc - 7).Trim();
                        Mobile toExile = Outcasts.FromName(strToExile);
                        if (toExile == null)
                        {
                            from.SendMessage(String.Format("No player by the name of '{0}' can be found.", strToExile));
                        }
                        else if (toExile == from)
                        {
                            from.SendMessage("You cannot exile yourself!");
                        }
                        else
                        {
                            from.SendGump(new ConfirmExileGump(from, town, toExile));
                        }
                    }
                }
            }
            else if (text.IndexOf("i unexile") != -1)
            {
                Town town = Town.CheckCitizenship(from);
                if (town != null && (town.IsKing(from)))
                {
                    int startLoc = text.IndexOf("i unexile");
                    string strToExile = text.Substring(startLoc + 9, text.Length - startLoc - 9).Trim();
                    Mobile toExile = Outcasts.FromName(strToExile);
                    if (toExile == null)
                    {
                        from.SendMessage(String.Format("No player by the name of '{0}' can be found.", strToExile));
                    }
                    else if (!town.Exiles.Contains(toExile))
                    {
                        from.SendMessage(String.Format("'{0}' is not currently exiled from {1}.", toExile.Name, town.Definition.FriendlyName));
                    }
                    else
                    {
                        town.Exiles.Remove(toExile);
                        from.SendMessage(String.Format("{0} has been unexiled from {1}.", toExile.Name, town.Definition.FriendlyName));
                    }
                }
            }
            else if (text.IndexOf("showscore") != -1)
            {
                PlayerState pl = PlayerState.Find(from);

                if (pl != null)
                    Timer.DelayCall(TimeSpan.Zero, new TimerStateCallback(ShowScore_Sandbox), pl);

            }
            else if (text.IndexOf("you are fired") != -1) // *you are fired*
            {
                Town town = Town.FromRegion(from.Region);

                if (town == null)
                    return;

                if (town.IsKing(from))
                    town.BeginOrderFiring(from);

            }
            else if (text.IndexOf("toggle rank") != -1)
            {
                PlayerState ps = PlayerState.Find(from);

                if (ps != null)
                {
                    ps.HideRank = !ps.HideRank;
                    from.SendMessage("Your faction rank is now {0}.", ps.HideRank ? "hidden" : "displayed");
                }
            }
		}
	}
}