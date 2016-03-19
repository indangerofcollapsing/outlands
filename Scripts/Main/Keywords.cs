using System;
using System.Collections;
using Server;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;
using Server.Multis;
using System.Collections.Generic;
using Server.Commands;
using Server.Targeting;
using Server.Targets;
using Server.Custom;
using Server.Mobiles;

namespace Server.Mobiles
{
    public class Keywords
    {
        public static void Initialize()
        {
            EventSink.Speech += new SpeechEventHandler(EventSink_Speech);
        }

        //Stealth Squelchable Phrases (Non-staff saying these words will make their text hidden to everyone except that player for 24 hours)
        private static string[] m_StealthSquelchablePhrases = new string[]
        {
            "UOF",           
            //"UO F",
            //"U OF",
            "U O F",
            "UOForever",
            "UO Forever",
            "U OForever",
            "UO Forever",
            "U O Forever",
            "UOForum",
            "UO Forum",
            "UO4 Ever",
            "UO4Ever",
            "UO 4Ever",
            "U O4Ever",
            "You Oh Eff"
        };

        private static void EventSink_Speech(SpeechEventArgs e)
        {
            Mobile from = e.Mobile;
            int[] keywords = e.Keywords;

            string text = e.Speech.Trim().ToLower();
            
            PlayerMobile pm = from as PlayerMobile;
                      
            //Restricted Speech Handling
            bool foundRestrictedPhrase = false;
            
            if (pm != null && from.AccessLevel == AccessLevel.Player)
            {
                foreach (string phrase in m_StealthSquelchablePhrases)
                {
                    if (text.IndexOf(phrase.Trim().ToLower()) != -1)
                    {
                        if (from.StealthSquelchedExpiration < DateTime.UtcNow)
                        {   
                            foreach (NetState state in NetState.Instances)
                            {
                                Mobile m = state.Mobile;                                
                                PlayerMobile player = m as PlayerMobile;
                                
                                if (player != null)
                                {
                                    if (player.AccessLevel > AccessLevel.Player && player.m_ShowAdminFilterText)
                                        player.SendMessage(0x482, "Stealth Squelch Filter Activated For Player: " + from.RawName);
                                }
                            }
                        }

                        break;
                    }
                }
            }           

            //Custom Speech Handling
            if (text.IndexOf("all patrol") != -1)
            {
                if (pm != null)
                {
                    if (pm.IsInTempStatLoss)
                    {
                        pm.SendMessage("That command cannot be issued while you are in temporary stat-loss");
                        return;
                    }

                    if (pm.AllFollowers.Count <= 0)
                    {
                        pm.SendMessage("You do not have any followers to send on patrol!");
                        return;
                    }

                    BaseAI.BeginPickAllPatrolTarget(pm);
                }
            }

            if (text.IndexOf("all fetch") != -1)
            {
                if (pm != null)
                {
                    if (pm.IsInTempStatLoss)
                    {
                        pm.SendMessage("That command cannot be issued while you are in temporary stat-loss");
                        return;
                    }

                    if (pm.AllFollowers.Count <= 0)
                    {
                        pm.SendMessage("You do not have any followers to order to fetch!");
                        return;
                    }

                    BaseAI.BeginPickAllFetchTarget(pm);
                }
            }

            if (text.IndexOf("all release") != -1)
            {
                e.Mobile.SendGump(new Gumps.ConfirmReleaseAllGump(e.Mobile));

                return;
            }
        }
    }
}
