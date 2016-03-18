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
    public class WorldChatGump : Gump
    {  
        public PlayerMobile player;
        public static int EntriesPerPage = 8;

        public WorldChatGump(PlayerMobile pm_Mobile): base(10, 10)
        {
            if (pm_Mobile == null) return;
            if (pm_Mobile.Deleted) return;

            player = pm_Mobile;

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
            AddImage(0, 44, 202);
            AddImage(44, 0, 201);
            AddImage(0, 0, 206);
            AddImage(0, 468, 204);
            AddImage(590, 1, 207);
            AddImage(590, 468, 205);
            AddImage(44, 468, 233);
            AddImage(590, 45, 203);
            AddImageTiled(44, 44, 546, 424, 200);
            AddImage(0, 152, 202);
            AddImage(163, 0, 201);
            AddImage(166, 468, 233);
            AddImage(590, 152, 203);
            AddImage(31, 427, 3001);
            AddImage(274, 427, 3001);
            AddImage(355, 427, 3001);
            AddImage(29, 68, 3001);
            AddImage(272, 68, 3001);
            AddImage(353, 68, 3001);

            AddLabel(287, 17, 149, "World Chat");
            
            AddLabel(274, 41, 2550, "Ignored Players");           

            //-------------------------------

            int m_PageNumber = player.m_WorldChatAccountEntry.m_PlayersSquelchedPage;

            int TotalPlayersSquelched = player.m_WorldChatAccountEntry.m_PlayersSquelched.Count;

            int m_TotalPages = (int)(Math.Ceiling((double)TotalPlayersSquelched / (double)EntriesPerPage));

            if (m_TotalPages == 0)
                m_TotalPages = 1;

            if (m_PageNumber < 1)
                m_PageNumber = 1;

            if (m_PageNumber > m_TotalPages)
                m_PageNumber = m_TotalPages;

            player.m_WorldChatAccountEntry.m_PlayersSquelchedPage = m_PageNumber;

            if (TotalPlayersSquelched > 0)
            {
                switch (player.m_WorldChatAccountEntry.m_FilterMode)
                {
                    case WorldChatAccountEntry.FilterMode.Name:
                        AddLabel(70, 87, greenTextHue, "Filter Entries by Player Name");
                        AddButton(29, 84, 2154, 2151, 1, GumpButtonType.Reply, 0);

                        AddLabel(419, 87, whiteTextHue, "Filter Entries by Date Added");
                        AddButton(385, 84, 2151, 2154, 2, GumpButtonType.Reply, 0);
                        break;

                    case WorldChatAccountEntry.FilterMode.Date:
                        AddLabel(70, 87, whiteTextHue, "Filter Entries by Player Name");
                        AddButton(29, 84, 2151, 2154, 1, GumpButtonType.Reply, 0);

                        AddLabel(419, 87, greenTextHue, "Filter Entries by Date Added");
                        AddButton(385, 84, 2154, 2151, 2, GumpButtonType.Reply, 0);
                        break;
                }

                AddLabel(71, 123, 149, "Remove Entry");
                AddLabel(189, 123, 149, "Ignore All on Account");
                AddLabel(358, 123, 149, "Player Name");
                AddLabel(488, 123, 149, "Date Added");
            }

            if (player.m_WorldChatAccountEntry.m_PlayersSquelchedPage > 1)
            {
                AddButton(127, 399, 4014, 4016, 3, GumpButtonType.Reply, 0);
                AddLabel(162, 399, whiteTextHue, "Previous");
            }

            if (player.m_WorldChatAccountEntry.m_PlayersSquelchedPage < m_TotalPages)
            {
                AddButton(438, 399, 4005, 4007, 4, GumpButtonType.Reply, 0);
                AddLabel(478, 399, whiteTextHue, "Next");
            }

            if (player.m_WorldChatAccountEntry.Enabled)
            {
                AddLabel(45, 436, 168, "World Chat Enabled");
                AddButton(91, 459, 2154, 2151, 5, GumpButtonType.Reply, 0);
            }

            else
            {
                AddLabel(45, 436, whiteTextHue, "World Chat Disabled");
                AddButton(91, 459, 2151, 2154, 5, GumpButtonType.Reply, 0);
            }

            AddLabel(241, 436, 1164, "Add Player to Ignore List");
            AddButton(299, 463, 4008, 4010, 6, GumpButtonType.Reply, 0);

            int globalTextHue = ChatPersistance.GlobalTextHue;

            if (player.m_WorldChatAccountEntry.GlobalTextHue != -1)
                globalTextHue = player.m_WorldChatAccountEntry.GlobalTextHue;

            AddLabel(480, 436, globalTextHue - 1, "Chat Text Color");
            AddButton(502, 465, 2118, 2117, 7, GumpButtonType.Reply, 0);
            AddItem(510, 459, 4011, globalTextHue - 1);  

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
                    if (a < player.m_WorldChatAccountEntry.m_PlayersSquelched.Count)
                    {
                        PlayerSquelchEntry squelchEntry = player.m_WorldChatAccountEntry.m_PlayersSquelched[a];

                        string playerName = "";
                        string squelchDate = DateTime.UtcNow.ToShortDateString();

                        if (squelchEntry != null)
                        {
                            playerName = squelchEntry.m_Player.RawName;
                            squelchDate = squelchEntry.m_SquelchExpiration.ToShortDateString();

                            //Remove
                            AddButton(100, iStartY + 3, 4002, 4004, 10 + buttonIndex, GumpButtonType.Reply, 0);

                            //Ignore Account
                            if (squelchEntry.m_SquelchEntireAccount)
                                AddButton(240, iStartY + 3, 2154, 2151, 20 + buttonIndex, GumpButtonType.Reply, 0);
                            else
                                AddButton(240, iStartY + 3, 2151, 2154, 20 + buttonIndex, GumpButtonType.Reply, 0);

                            AddLabel(Utility.CenteredTextOffset(395, playerName), iStartY, whiteTextHue, playerName);
                            AddLabel(Utility.CenteredTextOffset(525, squelchDate), iStartY, whiteTextHue, squelchDate);

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

            int m_PageNumber = player.m_WorldChatAccountEntry.m_PlayersSquelchedPage;
            
            int TotalPlayersSquelched = player.m_WorldChatAccountEntry.m_PlayersSquelched.Count;

            int m_TotalPages = (int)(Math.Ceiling((double)TotalPlayersSquelched / (double)EntriesPerPage));

            if (m_TotalPages == 0)
                m_TotalPages = 1;

            if (m_PageNumber < 1)
                m_PageNumber = 1;

            if (m_PageNumber > m_TotalPages)
                m_PageNumber = m_TotalPages;

            player.m_WorldChatAccountEntry.m_PlayersSquelchedPage = m_PageNumber;

            //Filter By Name
            if (info.ButtonID == 1)
            {
                if (player.m_WorldChatAccountEntry.m_FilterMode != WorldChatAccountEntry.FilterMode.Name)
                {
                    player.m_WorldChatAccountEntry.m_FilterMode = WorldChatAccountEntry.FilterMode.Name;
                    player.m_WorldChatAccountEntry.m_PlayersSquelchedPage = 1;

                    ChatPersistance.SortSquelchedPlayersByName(player.m_WorldChatAccountEntry);
                }

                resendGump = true;
            }

            //Filter By Date Squelched
            if (info.ButtonID == 2)
            {
                if (player.m_WorldChatAccountEntry.m_FilterMode != WorldChatAccountEntry.FilterMode.Date)
                {
                    player.m_WorldChatAccountEntry.m_FilterMode = WorldChatAccountEntry.FilterMode.Date;
                    player.m_WorldChatAccountEntry.m_PlayersSquelchedPage = 1;

                    ChatPersistance.SortSquelchedPlayersByDate(player.m_WorldChatAccountEntry);
                }

                resendGump = true;
            }

            //Previous
            if (info.ButtonID == 3)
            {
                if (player.m_WorldChatAccountEntry.m_PlayersSquelchedPage > 1)
                    player.m_WorldChatAccountEntry.m_PlayersSquelchedPage--;

                resendGump = true;
            }

            //Next
            if (info.ButtonID == 4)
            {
                if (player.m_WorldChatAccountEntry.m_PlayersSquelchedPage < m_TotalPages)
                    player.m_WorldChatAccountEntry.m_PlayersSquelchedPage++;

                resendGump = true;
            }

            //Enable World Chat
            if (info.ButtonID == 5)
            {
                player.m_WorldChatAccountEntry.Enabled = !player.m_WorldChatAccountEntry.Enabled;

                if (player.m_WorldChatAccountEntry.Enabled)
                    player.SendMessage("WorldChat is now enabled.");

                else
                    player.SendMessage("WorldChat is now disabled. You will no longer hear WorldChat messages.");

                resendGump = true;
            }

            //Add
            if (info.ButtonID == 6)
            {
                player.SendMessage("Please enter the name of the player to add to your World Chat Ignore List.");
                player.Prompt = new WorldChatSquelchPlayerPrompt(player);

                resendGump = true;
            }

            //Set Font Color
            if (info.ButtonID == 7)
            {
                if (player.NetState != null)
                {
                    from.CloseGump(typeof(WorldChatGump));
                    from.SendGump(new WorldChatGump(player));

                    new WorldChatGlobalTextHuePicker(player).SendTo(player.NetState);

                    return;
                }                
            }

            //Remove Entry
            if (info.ButtonID >= 10 && info.ButtonID <= 19)
            {
                PlayerSquelchEntry squelchEntry = null;

                int entryIndex = (m_PageNumber - 1) * EntriesPerPage + (info.ButtonID - 10);

                if (entryIndex < player.m_WorldChatAccountEntry.m_PlayersSquelched.Count)
                {
                    player.m_WorldChatAccountEntry.m_PlayersSquelched.RemoveAt(entryIndex);
                }

                resendGump = true;
            }

            //Squelch By Account
            if (info.ButtonID >= 20 && info.ButtonID <= 29)
            {
                PlayerSquelchEntry squelchEntry = null;

                int entryIndex = (m_PageNumber - 1) * EntriesPerPage + (info.ButtonID - 20);

                if (entryIndex < player.m_WorldChatAccountEntry.m_PlayersSquelched.Count)
                {
                    squelchEntry = player.m_WorldChatAccountEntry.m_PlayersSquelched[entryIndex];
                    squelchEntry.m_SquelchEntireAccount = !squelchEntry.m_SquelchEntireAccount;
                }

                resendGump = true;
            }            

            //--------------------

            if (resendGump)
            {
                from.CloseGump(typeof(WorldChatGump));
                from.SendGump(new WorldChatGump(player));
            }
        }

        private class WorldChatGlobalTextHuePicker : HuePicker
        {
            private PlayerMobile m_Player;

            public WorldChatGlobalTextHuePicker(PlayerMobile player): base(0xFAB)
            {
                m_Player = player;
            }

            public override void OnResponse(int hue)
            {
                if (m_Player == null || m_Player.Deleted)
                    return;

                ChatPersistance.CheckAndCreateWorldChatAccountEntry(m_Player);

                m_Player.m_WorldChatAccountEntry.GlobalTextHue = hue;

                if (m_Player.HasGump(typeof(WorldChatGump)))                
                    m_Player.CloseGump(typeof(WorldChatGump));

                m_Player.SendGump(new WorldChatGump(m_Player));

                return;
            }
        }

        private class WorldChatSquelchPlayerPrompt : Prompt
        {
            private PlayerMobile m_Player;

            public WorldChatSquelchPlayerPrompt(PlayerMobile player)
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
                        player.SendMessage("You cannot ignore a player on your own account.");
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
                            bool foundMatch = false;
                            
                            for (int b = 0; b < player.m_WorldChatAccountEntry.m_PlayersSquelched.Count; b++)
                            {
                                if (playerTarget == player.m_WorldChatAccountEntry.m_PlayersSquelched[b].m_Player)
                                {
                                    player.SendMessage("That character is already on your WorldChat Ignore List.");
                                    return;
                                }
                            }
                            
                            player.m_WorldChatAccountEntry.m_PlayersSquelched.Add(new PlayerSquelchEntry(playerTarget, true, DateTime.UtcNow));
                            
                            player.SendMessage(playerTarget.RawName + " added to WorldChat Ignore List.");

                            switch (player.m_WorldChatAccountEntry.m_FilterMode)
                            {
                                case WorldChatAccountEntry.FilterMode.Name:
                                    ChatPersistance.SortSquelchedPlayersByName(player.m_WorldChatAccountEntry);
                                    break;

                                case WorldChatAccountEntry.FilterMode.Date:
                                    ChatPersistance.SortSquelchedPlayersByDate(player.m_WorldChatAccountEntry);
                                    break;
                            }

                            if (player.HasGump(typeof(WorldChatGump)))
                            {
                                player.CloseGump(typeof(WorldChatGump));
                                player.SendGump(new WorldChatGump(player));
                            }

                            return;
                        }
                    }
                }
                
                player.SendMessage("No player by the name of " + playerTargetName + " has been found.");
            }
        }
    }    
}