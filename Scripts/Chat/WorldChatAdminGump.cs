using System;
using Server;
using Server.Mobiles;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Gumps;
using System.Linq;
using Server.Prompts;
using Server.Accounting;
using Server.HuePickers;

namespace Server.Custom
{
    public class WorldChatAdminGump : Gump
    {  
        public PlayerMobile player;
        public static int EntriesPerPage = 8;

        public WorldChatAdminGump(PlayerMobile pm_Mobile): base(10, 10)
        {
            if (pm_Mobile == null) return;
            if (pm_Mobile.Deleted) return;

            player = pm_Mobile;

            ChatPersistance.ClearExpiredSquelchEntries();

            ChatPersistance.CheckAndCreateWorldChatAccountEntry(player);

            if (player.m_WorldChatAccountEntry == null) 
                return;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            int whiteTextHue = 2655;
            int greenTextHue = 2599;

            AddPage(0);
            AddImage(305, 0, 201);
            AddImageTiled(186, 44, 546, 424, 200);
            AddImage(308, 468, 233);
            AddImage(0, 44, 202);
            AddImage(44, 0, 201);
            AddImage(0, 0, 206);
            AddImage(0, 468, 204);
            AddImage(731, 1, 207);
            AddImage(731, 468, 205);
            AddImage(44, 468, 233);
            AddImage(731, 45, 203);
            AddImageTiled(44, 44, 546, 424, 200);
            AddImage(0, 152, 202);
            AddImage(163, 0, 201);
            AddImage(166, 468, 233);
            AddImage(731, 152, 203);
          
            AddImage(31, 427, 3001);
            AddImage(274, 427, 3001);
            AddImage(355, 427, 3001);
            AddImage(29, 68, 3001);
            AddImage(272, 68, 3001);
            AddImage(353, 68, 3001);           
            
            AddImage(498, 68, 3001);
            AddImage(482, 427, 3001);

            AddLabel(302, 17, 149, "World Chat Administration");
            AddLabel(323, 41, 2606, "Squelched Players");  
            

            //-------------------------------

            int m_PageNumber = ChatPersistance.m_PlayersSquelchedAdminPage;

            int TotalPlayersSquelched = ChatPersistance.m_PlayersSquelched.Count;

            int m_TotalPages = (int)(Math.Ceiling((double)TotalPlayersSquelched / (double)EntriesPerPage));

            if (m_TotalPages == 0)
                m_TotalPages = 1;

            if (m_PageNumber < 1)
                m_PageNumber = 1;

            if (m_PageNumber > m_TotalPages)
                m_PageNumber = m_TotalPages;

            ChatPersistance.m_PlayersSquelchedAdminPage = m_PageNumber;

            if (TotalPlayersSquelched > 0)
            {
                AddLabel(30, 123, 149, "Remove");
                AddLabel(102, 123, 149, "Player Name");
                AddLabel(229, 123, 149, "Account Name");
                AddLabel(376, 123, 149, "Time Remaining");
                AddLabel(512, 123, 149, "24 Hours");
                AddLabel(583, 123, 149, "1 Week");
                AddLabel(642, 123, 149, "1 Month");
                AddLabel(706, 123, 149, "1 Year"); 

                switch (ChatPersistance.m_AdminFilterMode)
                {
                    case WorldChatAccountEntry.FilterMode.Name:
                        AddButton(29, 84, 2154, 2151, 1, GumpButtonType.Reply, 0);
                        AddLabel(70, 87, greenTextHue, "Filter Entries by Player Name");                      

                        AddButton(283, 84, 2151, 2154, 2, GumpButtonType.Reply, 0);
                        AddLabel(317, 87, whiteTextHue, "Filter Entries by Account");                        

                        AddButton(506, 84, 2151, 2154, 3, GumpButtonType.Reply, 0);
                        AddLabel(540, 87, whiteTextHue, "Filter Entries by Time Remaining");                      
                    break;

                    case WorldChatAccountEntry.FilterMode.Account:
                        AddButton(29, 84, 2151, 2154, 1, GumpButtonType.Reply, 0);
                        AddLabel(70, 87, whiteTextHue, "Filter Entries by Player Name");

                        AddButton(283, 84, 2154, 2151, 2, GumpButtonType.Reply, 0);
                        AddLabel(317, 87, greenTextHue, "Filter Entries by Account");                        

                        AddButton(506, 84, 2151, 2154, 3, GumpButtonType.Reply, 0);
                        AddLabel(540, 87, whiteTextHue, "Filter Entries by Time Remaining");  
                    break;

                    case WorldChatAccountEntry.FilterMode.Date:
                        AddButton(29, 84, 2151, 2154, 1, GumpButtonType.Reply, 0);
                        AddLabel(70, 87, whiteTextHue, "Filter Entries by Player Name");                      

                        AddButton(283, 84, 2151, 2154, 2, GumpButtonType.Reply, 0);
                        AddLabel(317, 87, whiteTextHue, "Filter Entries by Account");

                        AddButton(506, 84, 2154, 2151, 3, GumpButtonType.Reply, 0);
                        AddLabel(540, 87, greenTextHue, "Filter Entries by Time Remaining");  
                    break;
                }
            }

            if (ChatPersistance.m_PlayersSquelchedAdminPage > 1)
            {
                AddButton(127, 399, 4014, 4016, 4, GumpButtonType.Reply, 0);
                AddLabel(162, 399, whiteTextHue, "Previous");
            }

            if (ChatPersistance.m_PlayersSquelchedAdminPage < m_TotalPages)
            {
                AddButton(608, 399, 4005, 4007, 5, GumpButtonType.Reply, 0);
                AddLabel(648, 399, whiteTextHue, "Next");      
            }

            if (ChatPersistance.Enabled)
            {
                AddLabel(45, 436, 168, "World Chat Enabled");
                AddButton(91, 459, 2154, 2151, 6, GumpButtonType.Reply, 0);
            }

            else
            {
                AddLabel(45, 436, whiteTextHue, "World Chat Disabled");
                AddButton(91, 459, 2151, 2154, 6, GumpButtonType.Reply, 0);
            }

            AddLabel(207, 436, 1164, "Squelch Player by Name");
            AddButton(466, 463, 4010, 4007, 7, GumpButtonType.Reply, 0);

            AddLabel(404, 436, 2599, "Target Player to Squelch");
            AddButton(266, 463, 4010, 4007, 8, GumpButtonType.Reply, 0);     

            //---------------             
            
            int iStartY = 151;

            if (TotalPlayersSquelched > 0)
            {
                int startIndex = (m_PageNumber - 1) * EntriesPerPage;
                int endIndex = startIndex + EntriesPerPage;

                if (endIndex > TotalPlayersSquelched)
                    endIndex = TotalPlayersSquelched;

                int buttonIndex = 0;

                for (int a = startIndex; a < endIndex; a++)
                {
                    if (a < ChatPersistance.m_PlayersSquelched.Count)
                    {
                        PlayerSquelchEntry squelchEntry = ChatPersistance.m_PlayersSquelched[a];

                        string playerName = "";
                        string accountName = "";
                        string squelchExpiration = DateTime.UtcNow.ToShortDateString();

                        if (squelchEntry != null)
                        {
                            if (squelchEntry.m_Player != null)
                            {
                                playerName = squelchEntry.m_Player.RawName;

                                Account account = squelchEntry.m_Player.Account as Account;
                                accountName = account.Username;

                                squelchExpiration = Utility.CreateTimeRemainingString(DateTime.UtcNow, squelchEntry.m_SquelchExpiration, true, true, true, true, false);
                            }                            

                            AddButton(36, iStartY + 3, 4004, 4007, 10 + buttonIndex, GumpButtonType.Reply, 0); //Remove
                            AddLabel(Utility.CenteredTextOffset(136, playerName), iStartY, whiteTextHue, playerName);
                            AddLabel(Utility.CenteredTextOffset(269, accountName), iStartY, whiteTextHue, accountName);
                            AddLabel(Utility.CenteredTextOffset(425, squelchExpiration), iStartY, whiteTextHue, squelchExpiration);
                            AddButton(523, iStartY + 3, 4029, 4029, 20 + buttonIndex, GumpButtonType.Reply, 0);
                            AddButton(589, iStartY + 3, 4029, 4029, 30 + buttonIndex, GumpButtonType.Reply, 0);
                            AddButton(648, iStartY + 3, 4029, 4029, 40 + buttonIndex, GumpButtonType.Reply, 0);
                            AddButton(711, iStartY + 3, 4029, 4029, 50 + buttonIndex, GumpButtonType.Reply, 0);

                            iStartY += 32;
                        }
                    }

                    buttonIndex++;
                }
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (from == null) return;
            if (from.Deleted) return;
            if (player == null) return;
            if (player.Deleted) return;

            bool resendGump = false;

            ChatPersistance.CheckAndCreateWorldChatAccountEntry(player);

            int m_PageNumber = ChatPersistance.m_PlayersSquelchedAdminPage;

            int TotalPlayersSquelched = ChatPersistance.m_PlayersSquelched.Count;

            int m_TotalPages = (int)(Math.Ceiling((double)TotalPlayersSquelched / (double)EntriesPerPage));

            if (m_TotalPages == 0)
                m_TotalPages = 1;

            if (m_PageNumber < 1)
                m_PageNumber = 1;

            if (m_PageNumber > m_TotalPages)
                m_PageNumber = m_TotalPages;

            ChatPersistance.m_PlayersSquelchedAdminPage = m_PageNumber;

            //Filter By Name
            if (info.ButtonID == 1)
            {
                if (ChatPersistance.m_AdminFilterMode != WorldChatAccountEntry.FilterMode.Name)
                {
                    ChatPersistance.m_AdminFilterMode = WorldChatAccountEntry.FilterMode.Name;
                    ChatPersistance.m_PlayersSquelchedAdminPage = 1;

                    ChatPersistance.AdminSortSquelchedPlayersByName();
                    ChatPersistance.UpdateAdminGumps();                    
                }

                resendGump = true;
            }

            //Filter By Account
            if (info.ButtonID == 2)
            {
                if (ChatPersistance.m_AdminFilterMode != WorldChatAccountEntry.FilterMode.Account)
                {
                    ChatPersistance.m_AdminFilterMode = WorldChatAccountEntry.FilterMode.Account;
                    ChatPersistance.m_PlayersSquelchedAdminPage = 1;

                    ChatPersistance.AdminSortSquelchedPlayersByAccount();
                    ChatPersistance.UpdateAdminGumps();                    
                }

                resendGump = true;
            }

            //Filter By Expiration
            if (info.ButtonID == 3)
            {
                if (ChatPersistance.m_AdminFilterMode != WorldChatAccountEntry.FilterMode.Date)
                {
                    ChatPersistance.m_AdminFilterMode = WorldChatAccountEntry.FilterMode.Date;
                    ChatPersistance.m_PlayersSquelchedAdminPage = 1;

                    ChatPersistance.AdminSortSquelchedPlayersByDate();
                    ChatPersistance.UpdateAdminGumps();                   
                }

                resendGump = true;
            }

            //Previous
            if (info.ButtonID == 4)
            {
                if (ChatPersistance.m_PlayersSquelchedAdminPage > 1)
                    ChatPersistance.m_PlayersSquelchedAdminPage--;

                ChatPersistance.UpdateAdminGumps();

                resendGump = true;
            }

            //Next
            if (info.ButtonID == 5)
            {
                if (ChatPersistance.m_PlayersSquelchedAdminPage < m_TotalPages)
                    ChatPersistance.m_PlayersSquelchedAdminPage++;

                resendGump = true;
            }

            //Enable World Chat
            if (info.ButtonID == 6)
            {
                ChatPersistance.Enabled = !ChatPersistance.Enabled;

                if (ChatPersistance.Enabled)
                    player.SendMessage("WorldChat has been enabled for all players.");

                else
                    player.SendMessage("WorldChat has been disabled for all players.");

                ChatPersistance.UpdateAdminGumps();

                resendGump = true;
            }

            //Squelch Player By Target
            if (info.ButtonID == 7)
            {
                ChatPersistance.WorldChatAdminSquelch(player, TimeSpan.FromHours(24));

                resendGump = false;              
            }

            //Squelch Player By Name
            if (info.ButtonID == 8)
            {
                player.SendMessage("Please enter the name of the player to add to the Squelched List.");
                player.Prompt = new WorldChatAdminSquelchPlayerPrompt(player);

                resendGump = true;
            }

            //Remove Entry
            if (info.ButtonID >= 10 && info.ButtonID <= 19)
            {
                PlayerSquelchEntry squelchEntry = null;

                int entryIndex = (m_PageNumber - 1) * EntriesPerPage + (info.ButtonID - 10);

                if (entryIndex < ChatPersistance.m_PlayersSquelched.Count)
                {
                    ChatPersistance.m_PlayersSquelched.RemoveAt(entryIndex);
                }

                ChatPersistance.UpdateAdminGumps();

                resendGump = true;
            }

            //24 Hour Squelch
            if (info.ButtonID >= 20 && info.ButtonID <= 29)
            {
                PlayerSquelchEntry squelchEntry = null;

                int entryIndex = (m_PageNumber - 1) * EntriesPerPage + (info.ButtonID - 20);

                if (entryIndex < ChatPersistance.m_PlayersSquelched.Count)                
                    ChatPersistance.m_PlayersSquelched[entryIndex].m_SquelchExpiration = DateTime.UtcNow + TimeSpan.FromHours(24);

                ChatPersistance.UpdateAdminGumps();

                resendGump = true;
            }

            //1 Week Squelch
            if (info.ButtonID >= 30 && info.ButtonID <= 39)
            {
                PlayerSquelchEntry squelchEntry = null;

                int entryIndex = (m_PageNumber - 1) * EntriesPerPage + (info.ButtonID - 30);

                if (entryIndex < ChatPersistance.m_PlayersSquelched.Count)
                    ChatPersistance.m_PlayersSquelched[entryIndex].m_SquelchExpiration = DateTime.UtcNow + TimeSpan.FromDays(7);

                ChatPersistance.UpdateAdminGumps();

                resendGump = true;
            }

            //1 Month Squelch
            if (info.ButtonID >= 40 && info.ButtonID <= 49)
            {
                PlayerSquelchEntry squelchEntry = null;

                int entryIndex = (m_PageNumber - 1) * EntriesPerPage + (info.ButtonID - 40);

                if (entryIndex < ChatPersistance.m_PlayersSquelched.Count)
                    ChatPersistance.m_PlayersSquelched[entryIndex].m_SquelchExpiration = DateTime.UtcNow + TimeSpan.FromDays(30);

                ChatPersistance.UpdateAdminGumps();

                resendGump = true;
            }

            //1 Year Squelch
            if (info.ButtonID >= 50 && info.ButtonID <= 59)
            {
                PlayerSquelchEntry squelchEntry = null;

                int entryIndex = (m_PageNumber - 1) * EntriesPerPage + (info.ButtonID - 50);

                if (entryIndex < ChatPersistance.m_PlayersSquelched.Count)
                    ChatPersistance.m_PlayersSquelched[entryIndex].m_SquelchExpiration = DateTime.UtcNow + TimeSpan.FromDays(365);

                ChatPersistance.UpdateAdminGumps();

                resendGump = true;
            }

            if (resendGump)
            {            
                player.CloseGump(typeof(WorldChatAdminGump));
                player.SendGump(new WorldChatAdminGump(player));                
            }
        }
        
        private class WorldChatAdminSquelchPlayerPrompt : Prompt
        {
            private PlayerMobile m_Player;

            public WorldChatAdminSquelchPlayerPrompt(PlayerMobile player)
            {
                m_Player = player;
            }

            public override void OnCancel(Mobile from)
            {  
            }

            public override void OnResponse(Mobile from, string text)
            {
                PlayerMobile player = from as PlayerMobile;
                
                if (player == null) return;
                if (player.Deleted || !player.Alive) return;

                ChatPersistance.ClearExpiredSquelchEntries();
                
                ChatPersistance.CheckAndCreateWorldChatAccountEntry(player);

                if (text == "" || text == null)
                {
                    player.SendMessage("No player by that name has been found.");
                    return;
                }
                
                string playerTargetName = text.ToLower();

                Account playerAccount = player.Account as Account;

                if (playerAccount == null) return;
                if (playerAccount.accountMobiles == null) return;

                for (int a = 0; a < playerAccount.accountMobiles.Length; a++)
                {
                    PlayerMobile pm_Mobile = playerAccount.accountMobiles[a] as PlayerMobile;

                    if (pm_Mobile == null) continue;
                    if (pm_Mobile.RawName == null || pm_Mobile.RawName == "") continue;

                    if (pm_Mobile.RawName.ToLower() == playerTargetName)
                    {
                        player.SendMessage("You cannot squelch a player on your own account.");
                        return;
                    }
                }
                
                foreach (Account account in Accounts.GetAccounts())
                {
                    if (account == null) continue;
                    if (account.accountMobiles == null) continue;
                    
                    for (int a = 0; a < account.accountMobiles.Length; a++)
                    {
                        PlayerMobile playerTarget = account.accountMobiles[a] as PlayerMobile;
                        
                        if (playerTarget == null) continue;
                        if (playerTarget.RawName == null) continue;                       
                        
                        if (playerTarget.RawName.ToLower() == playerTargetName)
                        {
                            if (playerTarget.AccessLevel >= player.AccessLevel)
                            {
                                player.SendMessage("You must have a higher access level to Squelch that player.");
                                return;
                            }

                            string accountName = "";

                            Account playerTargetAccount = playerTarget.Account as Account;

                            if (playerTargetAccount == null)
                                return;                            
                            
                            for (int b = 0; b < ChatPersistance.m_PlayersSquelched.Count; b++)
                            {
                                if (ChatPersistance.m_PlayersSquelched[b].m_Player == null)
                                     continue;

                                Account squelchAccount = ChatPersistance.m_PlayersSquelched[b].m_Player.Account as Account;

                                if (squelchAccount != null)
                                {
                                    if (playerTargetAccount == squelchAccount)
                                    {
                                        accountName = playerTargetAccount.Username;

                                        player.SendMessage("That player's account (" + accountName + ") has already been Squelched. Use [WorldChatAdmin to manage current Squelches.");
                                        return;
                                    }
                                }
                            }

                            ChatPersistance.m_PlayersSquelched.Add(new PlayerSquelchEntry(playerTarget, true, DateTime.UtcNow + TimeSpan.FromHours(24)));
                            
                            player.SendMessage(playerTarget.RawName + " added to WorldChat Squelch List.");

                            switch (ChatPersistance.m_AdminFilterMode)
                            {
                                case WorldChatAccountEntry.FilterMode.Name:
                                    ChatPersistance.AdminSortSquelchedPlayersByName();
                                break;

                                case WorldChatAccountEntry.FilterMode.Date:
                                    ChatPersistance.AdminSortSquelchedPlayersByDate();
                                break;
                            }
                            
                            ChatPersistance.UpdateAdminGumps();

                            player.CloseGump(typeof(WorldChatAdminGump));
                            player.SendGump(new WorldChatAdminGump(player));

                            return;
                        }
                    }
                }
                
                player.SendMessage("No player by the name of " + playerTargetName + " has been found.");
            }
        }
    }    
}