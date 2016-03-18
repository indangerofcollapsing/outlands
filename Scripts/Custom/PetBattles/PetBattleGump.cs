using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Server.Network;
using Server.Prompts;
using Server.Mobiles;
using Server.Items;
using Server.Custom;

namespace Server.Gumps
{
    public class PetBattleDetermineRulesGump : Gump
    {
        public Custom.PetBattle.Team m_Team;
        public Custom.PetBattle.Team m_EnemyTeam;

        public PetBattle m_PetBattle;

        public PetBattleDetermineRulesGump(PetBattle petBattle, PlayerMobile player)
            : base(20, 20)
        {
            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            if (petBattle == null || player == null)
                return;

            m_PetBattle = petBattle;

            int playerWins = 0;
            int playerTies = 0;
            int playerLosses = 0;

            int opponentWins = 0;
            int opponentTies = 0;
            int opponentLosses = 0;

            //Determine Teams
            foreach (Custom.PetBattle.Team team in m_PetBattle.m_Teams)
            {
                if (team.m_Player == player)
                {
                    m_Team = team;
                   
                    PetBattleCreatureCollection playerCollection = PetBattlePersistance.GetPlayerPetBattleCreatureCollection(player);

                    foreach (PetBattleCreatureEntry creatureEntry in playerCollection.m_CreatureEntries)
                    {
                        playerWins += creatureEntry.m_Wins;
                        playerTies += creatureEntry.m_Ties;
                        playerLosses += creatureEntry.m_Losses;
                    }
                }

                else
                {
                    m_EnemyTeam = team;

                    PetBattleCreatureCollection opponentCollection = PetBattlePersistance.GetPlayerPetBattleCreatureCollection(m_EnemyTeam.m_Player);

                    foreach (PetBattleCreatureEntry creatureEntry in opponentCollection.m_CreatureEntries)
                    {
                        opponentWins += creatureEntry.m_Wins;
                        opponentTies += creatureEntry.m_Ties;
                        opponentLosses += creatureEntry.m_Losses;
                    }
                }
            }

            if (m_Team == null || m_EnemyTeam == null)
                return;

            if (m_Team.m_Player == null || m_EnemyTeam.m_Player == null)
                return;

            AddPage(0);

            AddBackground(0, 0, 430, 430, 5054);
            AddBackground(10, 10, 410, 410, 3000);

            AddLabel(170, 20, 0, @"Pet Battle");

            //Player Info
            AddLabel(20, 60, 0, @"Player: " + m_Team.m_Player.RawName);            
            AddLabel(20, 80, 0, @"Record: " + playerWins.ToString() + "-" + playerTies.ToString() + "-" + playerLosses.ToString());

            //Opponent Info
            AddLabel(280, 60, 0, @"Opponent: " + m_EnemyTeam.m_Player.RawName);
            AddLabel(280, 80, 0, @"Record: " + opponentWins.ToString() + "-" + opponentTies.ToString() + "-" + opponentLosses.ToString());
            
            //Match Cost
            int goldCost = PetBattle.baseFee[(int)m_Team.selectedFormat];

            AddLabel(170, 100, 0, @"Match Cost");
            AddItem(155, 120, 3823); //Gold Image
            AddLabel(205, 123, 0, goldCost.ToString());

            AddLabel(60, 145, 0, @"Your Selection");

            //Player Ruleset Selection
            bool foundPlayer1Totem1 = false;
            bool foundPlayer1Totem2 = false;
            bool foundPlayer1Totem3 = false;

            bool foundPlayer2Totem1 = false;
            bool foundPlayer2Totem2 = false;
            bool foundPlayer2Totem3 = false;

            int creaturesAllowed = 0;

            foreach (PetBattleTotem totem in m_PetBattle.m_PetBattleSignupStone.m_PetBattleTotems)
            {
                if (totem != null)
                {
                    //Player 1
                    if (totem.TeamNumber == 1 && totem.PositionNumber == 1)
                        foundPlayer1Totem1 = true;

                    if (totem.TeamNumber == 1 && totem.PositionNumber == 2)
                        foundPlayer1Totem2 = true;

                    if (totem.TeamNumber == 1 && totem.PositionNumber == 3)
                        foundPlayer1Totem3 = true;

                    //Player 2
                    if (totem.TeamNumber == 1 && totem.PositionNumber == 1)
                        foundPlayer2Totem1 = true;

                    if (totem.TeamNumber == 1 && totem.PositionNumber == 2)
                        foundPlayer2Totem2 = true;

                    if (totem.TeamNumber == 1 && totem.PositionNumber == 3)
                        foundPlayer2Totem3 = true;
                }
            }

            if (foundPlayer1Totem1 && foundPlayer2Totem1)
                creaturesAllowed++;

            if (creaturesAllowed == 1 && (foundPlayer1Totem2 && foundPlayer2Totem2))
                creaturesAllowed++;

            if (creaturesAllowed == 2 && (foundPlayer1Totem3 && foundPlayer2Totem3))
                creaturesAllowed++;           

            if (creaturesAllowed >= 1)
            {
                AddLabel(60, 170, 0, @"1 vs 1: Solo");
                if ((bool)(m_Team.selectedFormat == PetBattleFormat.Solo1vs1))
                    AddButton(20, 165, 2153, 2153, 1, GumpButtonType.Reply, 0);
                else
                    AddButton(20, 165, 2151, 2151, 1, GumpButtonType.Reply, 0);
            }

            if (creaturesAllowed >= 2)
            {
                AddLabel(60, 200, 0, @"2 vs 2: Solo");
                if ((bool)(m_Team.selectedFormat == PetBattleFormat.Solo2vs2))
                    AddButton(20, 195, 2153, 2153, 2, GumpButtonType.Reply, 0);
                else
                    AddButton(20, 195, 2151, 2151, 2, GumpButtonType.Reply, 0);

                AddLabel(60, 230, 0, @"2 vs 2: Simultaneous");
                if ((bool)(m_Team.selectedFormat == PetBattleFormat.Simultaneous2vs2))
                    AddButton(20, 225, 2153, 2153, 3, GumpButtonType.Reply, 0);
                else
                    AddButton(20, 225, 2151, 2151, 3, GumpButtonType.Reply, 0);
            }

            if (creaturesAllowed >= 3)
            {
                AddLabel(60, 260, 0, @"3 vs 3: Solo");
                if ((bool)(m_Team.selectedFormat == PetBattleFormat.Solo3vs3))
                    AddButton(20, 255, 2153, 2153, 4, GumpButtonType.Reply, 0);
                else
                    AddButton(20, 255, 2151, 2151, 4, GumpButtonType.Reply, 0);

                AddLabel(60, 290, 0, @"3 vs 3: Simultaneous");
                if ((bool)(m_Team.selectedFormat == PetBattleFormat.Simultaneous3vs3))
                    AddButton(20, 285, 2153, 2153, 5, GumpButtonType.Reply, 0);
                else
                    AddButton(20, 285, 2151, 2151, 5, GumpButtonType.Reply, 0);
            }
            
            //Opponent's Selection
            AddLabel(280, 145, 0, @"Their Selection");

            if (creaturesAllowed >= 1)
            {
                AddLabel(280, 170, 0, @"1 vs 1: Solo");
                if (m_EnemyTeam.selectedFormat == PetBattleFormat.Solo1vs1)
                    AddImage(240, 165, 9724);
                else
                    AddImage(240, 165, 9721);
            }

            if (creaturesAllowed >= 2)
            {
                AddLabel(280, 200, 0, @"2 vs 2: Solo");
                if (m_EnemyTeam.selectedFormat == PetBattleFormat.Solo2vs2)
                    AddImage(240, 195, 9724);
                else
                    AddImage(240, 195, 9721);

                AddLabel(280, 230, 0, @"2 vs 2: Simultaneous");
                if (m_EnemyTeam.selectedFormat == PetBattleFormat.Simultaneous2vs2)
                    AddImage(240, 225, 9724);
                else
                    AddImage(240, 225, 9721);
            }

            if (creaturesAllowed >= 3)
            {
                AddLabel(280, 260, 0, @"3 vs 3: Solo");
                if (m_EnemyTeam.selectedFormat == PetBattleFormat.Solo3vs3)
                    AddImage(240, 255, 9724);
                else
                    AddImage(240, 255, 9721);

                AddLabel(280, 290, 0, @"3 vs 3: Simultaneous");
                if (m_EnemyTeam.selectedFormat == PetBattleFormat.Simultaneous3vs3)
                    AddImage(240, 285, 9724);
                else
                    AddImage(240, 285, 9721);
            }

            //Your Wager
            AddLabel(60, 320, 0, @"Your Wager (Optional)");
            AddItem(10, 342, 3823);
            AddImage(54, 344, 2501);
            AddTextEntry(62, 345, 127, 20, 0, 6, m_Team.goldWager.ToString());
            AddButton(200, 347, 1209, 1210, 7, GumpButtonType.Reply, 0);

            //Their Wager
            AddLabel(278, 320, 0, @"Their Wager");
            AddItem(234, 342, 3823);
            AddLabel(280, 345, 0, m_EnemyTeam.goldWager.ToString());

            //Your Ready Button
            AddLabel(60, 390, 0, @"Ready");

            if (m_Team.ready)
            {
                AddButton(20, 385, 2153, 2153, 8, GumpButtonType.Reply, 0);
            }

            else
            {
                AddButton(20, 385, 2151, 2151, 8, GumpButtonType.Reply, 0);
            }

            //Cancel Pet Battle
            AddButton(130, 385, 2151, 2152, 9, GumpButtonType.Reply, 0);
            AddLabel(170, 390, 0, @"Quit");

            //Their Ready Button
            if (m_EnemyTeam.ready)
            {
                AddImage(240, 385, 9724);
            }

            else
            {
                AddImage(240, 385, 9721);
            }

            AddLabel(280, 390, 0, @"They are ready");
        }

        public void UpdatePetBattleGump()
        {
            foreach (Custom.PetBattle.Team team in m_PetBattle.m_Teams)
            {
                PlayerMobile pm = team.m_Player;

                if (pm != null)
                {
                    pm.CloseGump(typeof(Gumps.PetBattleDetermineRulesGump));
                    pm.SendGump(new Gumps.PetBattleDetermineRulesGump(m_PetBattle, pm));
                }
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            bool valuesChanged = false;
            bool clickedReady = false;

            if (m_Team == null || m_Team.m_Player == null)
                return;

            switch (info.ButtonID)
            {
                case 0:
                    return;
                    break;

                case 1:
                    if (m_Team.selectedFormat != PetBattleFormat.Solo1vs1)
                    {
                        m_Team.selectedFormat = PetBattleFormat.Solo1vs1;
                        valuesChanged = true;
                    }
                    break;

                case 2:
                    if (m_Team.selectedFormat != PetBattleFormat.Solo2vs2)
                    {
                        m_Team.selectedFormat = PetBattleFormat.Solo2vs2;
                        valuesChanged = true;
                    }
                    break;

                case 3:
                    if (m_Team.selectedFormat != PetBattleFormat.Simultaneous2vs2)
                    {
                        m_Team.selectedFormat = PetBattleFormat.Simultaneous2vs2;
                        valuesChanged = true;
                    }
                    break;

                case 4:
                    if (m_Team.selectedFormat != PetBattleFormat.Solo3vs3)
                    {
                        m_Team.selectedFormat = PetBattleFormat.Solo3vs3;
                        valuesChanged = true;
                    }
                    break;

                case 5:
                    if (m_Team.selectedFormat != PetBattleFormat.Simultaneous3vs3)
                    {
                        m_Team.selectedFormat = PetBattleFormat.Simultaneous3vs3;
                        valuesChanged = true;
                    }
                    break;

                case 6:
                    //Gold Text Entry
                    break;

                case 7: //Gold Confirmation
                    TextRelay textRelayGold = info.GetTextEntry(6);
                    string textGold = textRelayGold.Text.Trim();

                    int gold = 0;

                    try
                    {
                        gold = Convert.ToInt32(textGold);
                    }

                    catch (Exception e)
                    {
                        gold = 0;
                    }

                    finally
                    {
                        if (gold >= Int32.MaxValue || gold < 0)
                        {
                            gold = 0;
                        }

                        if (m_Team.goldWager != gold)
                        {
                            if (Banker.GetBalance(m_Team.m_Player) < m_Team.goldWager)
                            {
                                m_Team.m_Player.SendMessage("You do not have that much gold.");
                            }

                            else
                            {
                                m_Team.goldWager = gold;
                                valuesChanged = true;
                            }
                        }
                    }
                    break;

                case 8:
                    if (m_Team.ready)
                        m_Team.ready = false;

                    else
                        m_Team.ready = true;

                    clickedReady = true;
                    break;

                case 9:
                    //Quit
                    break;
            }

            //Value Changed    
            if (valuesChanged && clickedReady == false)
            {
                m_Team.ready = false;
                UpdatePetBattleGump();

                return;
            }

            UpdatePetBattleGump();
        }
    }

    public class PetBattleSelectCreaturesGump : Gump
    {
        public Custom.PetBattle.Team m_Team;
        public Custom.PetBattle.Team m_EnemyTeam;

        public PetBattle m_PetBattle;        

        public PetBattleSelectCreaturesGump(PetBattle petBattle, PlayerMobile player): base(20, 20)
        {
            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            if (petBattle == null || player == null)
                return;

            m_PetBattle = petBattle;

            //Determine Teams
            foreach (Custom.PetBattle.Team team in m_PetBattle.m_Teams)
            {
                if (team.m_Player == player)
                    m_Team = team;
                else
                    m_EnemyTeam = team;
            }

            if (m_Team == null || m_EnemyTeam == null)
                return;

            if (m_Team.m_Player == null || m_EnemyTeam.m_Player == null)
                return;

            AddPage(0);

            AddBackground(0, 0, 430, 430, 5054);
            AddBackground(10, 10, 410, 410, 3000);

            AddLabel(160, 20, 0, @"Pet Battle vs " + m_EnemyTeam.m_Player.RawName);
            AddLabel(25, 55, 0, @"Selected Creatures");

            //Creature Button Groups   
            for (int a = 0; a < m_PetBattle.m_CreaturesPerTeam; a++)
            {
                //Get Creature Info
                PetBattleCreatureEntry creatureEntry = m_Team.m_CreatureEntries[a];
                BaseCreature creature = (BaseCreature)Activator.CreateInstance(creatureEntry.m_Type);

                if (creature == null)
                    continue;               

                int petBattleIcon;

                if (creature.PetBattleItemId != null)
                    petBattleIcon = creature.PetBattleItemId;
                else
                    petBattleIcon = 17087; //Default Icon

                if (creature.PetBattleBriefDescription == null)
                {
                    creature.PetBattleBriefDescription = "";
                }

                //Create Gump Entry
                if (m_Team.m_Player.PetBattleCreatureCollection.m_CreatureEntries.Count > 1)
                {
                    AddButton(25, 90 + (a * 60), 5600, 5604, 1 + a, GumpButtonType.Reply, 0); //Up Arrow
                    AddButton(25, 110 + (a * 60), 5602, 5606, 6 + a, GumpButtonType.Reply, 0); //Down Arrow
                }
               
                AddItem(45 + creature.PetBattleCreatureSelectItemIdOffsetX, 90 + (a * 60) + creature.PetBattleCreatureSelectItemIdOffsetY, petBattleIcon, creature.PetBattleItemHue); //Creature Icon
               
                AddButton(135, 90 + (a * 60), 82, 82, 11 + a, GumpButtonType.Reply, 0); //Hidden Button Region
                AddItem(120, 90 + (a * 65), 17087); //Book Icon
                AddLabel(170, 90 + (a * 60), 0, creature.PetBattleTitle + " (Level " + creatureEntry.m_Level + ")"); //Creature Name
                AddLabel(170, 105 + (a * 60), 0, creature.PetBattleBriefDescription); //Creature Brief Description

                creature.Delete();
            }

            //Confirmations
            if (m_Team.ready)
                AddButton(20, 381, 2153, 2151, 16, GumpButtonType.Reply, 0);
            else
                AddButton(20, 381, 2151, 2153, 16, GumpButtonType.Reply, 0);

            AddLabel(55, 385, 0, @"Ready");

            AddButton(165, 381, 2151, 2153, 17, GumpButtonType.Reply, 0);
            AddLabel(200, 385, 0, @"Quit");

            //Their Ready Button
            if (m_EnemyTeam.ready)
            {
                AddImage(280, 381, 9724);
            }

            else
            {
                AddImage(280, 381, 9721);
            }

            AddLabel(315, 385, 0, @"They are ready");
        }

        public void UpdatePetBattleSelectingCreaturesGump(PlayerMobile player)
        {
            player.CloseGump(typeof(Gumps.PetBattleSelectCreaturesGump));
            player.SendGump(new Gumps.PetBattleSelectCreaturesGump(m_PetBattle, player));
        }

        public void SelectingCreaturesReadyChange()
        {
            foreach (Custom.PetBattle.Team team in m_PetBattle.m_Teams)
            {
                PlayerMobile pm_Player = team.m_Player;

                if (pm_Player != null)
                {
                    pm_Player.CloseGump(typeof(Gumps.PetBattleSelectCreaturesGump));
                    pm_Player.SendGump(new Gumps.PetBattleSelectCreaturesGump(m_PetBattle, pm_Player));
                }

                if (pm_Player.HasGump(typeof(Gumps.PetBattleGrimoireGump)))
                {
                    if (team.browsingCreaturePosition <= team.m_CreatureEntries.Count - 1)
                    {
                        PetBattleCreatureCollection playerCreatureCollection = PetBattlePersistance.GetPlayerPetBattleCreatureCollection(pm_Player);                       

                        pm_Player.CloseGump(typeof(Gumps.PetBattleGrimoireGump));
                        pm_Player.SendGump(new Gumps.PetBattleGrimoireGump(team.grimoireBrowsingPage, team, pm_Player, playerCreatureCollection));
                    }
                }
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (m_Team == null) return;
            if (m_Team.m_Player == null) return;

            bool valuesChanged = false;
            bool clickedReady = false;

            //Up Arrow: Previous Creature in CreatureEntries List
            if (info.ButtonID >= 1 && info.ButtonID <= 5)
            {
                int creaturePosition = info.ButtonID - 1;
                int creatureIndex = -1;

                for (int a = 0; a < m_Team.m_Player.PetBattleCreatureCollection.m_CreatureEntries.Count; a++)
                {
                    if (creaturePosition >= m_Team.m_CreatureEntries.Count)
                        continue;

                    if (a >= m_Team.m_Player.PetBattleCreatureCollection.m_CreatureEntries.Count)
                        continue;

                    if (m_Team.m_CreatureEntries[creaturePosition].m_Type == m_Team.m_Player.PetBattleCreatureCollection.m_CreatureEntries[a].m_Type)
                    {
                        creatureIndex = a;
                        break;
                    }
                }

                if (creatureIndex != -1)
                {
                    if (creatureIndex == 0)
                        creatureIndex = m_Team.m_Player.PetBattleCreatureCollection.m_CreatureEntries.Count - 1;
                    else
                        creatureIndex--;

                    if (creatureIndex < 0 || creatureIndex >= m_Team.m_Player.PetBattleCreatureCollection.m_CreatureEntries.Count)
                        return;

                    PetBattleCreatureEntry newCreatureEntry = null;
                    
                    if (creatureIndex < m_Team.m_Player.PetBattleCreatureCollection.m_CreatureEntries.Count)
                        newCreatureEntry = m_Team.m_Player.PetBattleCreatureCollection.m_CreatureEntries[creatureIndex];

                    if (newCreatureEntry != null)
                    {
                        m_Team.m_CreatureEntries[creaturePosition] = newCreatureEntry;
                        m_Team.browsingCreaturePosition = creaturePosition;

                        valuesChanged = true;

                        if (m_Team.ready)
                        {
                            m_Team.ready = false;
                            SelectingCreaturesReadyChange();
                        }

                        else
                        {
                            m_Team.m_Player.SendGump(new Gumps.PetBattleSelectCreaturesGump(m_PetBattle, m_Team.m_Player));
                        }
                    }
                }

                if (m_Team.m_Player.HasGump(typeof(Gumps.PetBattleGrimoireGump)))
                {
                    if (creaturePosition < 0 || creaturePosition >= m_Team.m_CreatureEntries.Count)
                        return;

                    PetBattleCreatureEntry creatureEntry = null;
                    
                    if (creaturePosition < m_Team.m_CreatureEntries.Count)
                        creatureEntry = m_Team.m_CreatureEntries[creaturePosition];

                    if (creatureEntry != null)
                    {
                        int page = (creatureIndex * 2) + 1;
                        
                        m_Team.m_Player.CloseGump(typeof(Gumps.PetBattleGrimoireGump));
                        m_Team.m_Player.SendGump(new Gumps.PetBattleGrimoireGump(page, m_Team, m_Team.m_Player, m_Team.m_Player.PetBattleCreatureCollection));
                    }
                }
            }

            //Down Arrow: Next Creature in CreatureEntries List
            if (info.ButtonID >= 6 && info.ButtonID <= 10)
            {
                int creaturePosition = info.ButtonID - 6;
                int creatureIndex = -1;

                for (int a = 0; a < m_Team.m_Player.PetBattleCreatureCollection.m_CreatureEntries.Count; a++)
                {
                    if (creaturePosition >= m_Team.m_CreatureEntries.Count)
                        continue;

                    if (a >= m_Team.m_Player.PetBattleCreatureCollection.m_CreatureEntries.Count)
                        continue;

                    if (m_Team.m_CreatureEntries[creaturePosition].m_Type == m_Team.m_Player.PetBattleCreatureCollection.m_CreatureEntries[a].m_Type)
                    {
                        creatureIndex = a;
                        break;
                    }
                }

                if (creatureIndex != -1)
                {
                    if (creatureIndex >= m_Team.m_Player.PetBattleCreatureCollection.m_CreatureEntries.Count - 1)
                        creatureIndex = 0;
                    else
                        creatureIndex++;

                    if (creatureIndex < 0 || creatureIndex >= m_Team.m_Player.PetBattleCreatureCollection.m_CreatureEntries.Count)
                        return;

                    PetBattleCreatureEntry newCreatureEntry = null;
                    
                    if (creatureIndex < m_Team.m_Player.PetBattleCreatureCollection.m_CreatureEntries.Count)
                        newCreatureEntry = m_Team.m_Player.PetBattleCreatureCollection.m_CreatureEntries[creatureIndex];

                    if (newCreatureEntry != null)
                    {
                        m_Team.m_CreatureEntries[creaturePosition] = newCreatureEntry;
                        m_Team.browsingCreaturePosition = creaturePosition;

                        valuesChanged = true;

                        if (m_Team.ready)
                        {
                            m_Team.ready = false;
                            SelectingCreaturesReadyChange();
                        }

                        else
                        {
                            m_Team.m_Player.SendGump(new Gumps.PetBattleSelectCreaturesGump(m_PetBattle, m_Team.m_Player));
                        }
                    }
                }

                if (m_Team.m_Player.HasGump(typeof(Gumps.PetBattleGrimoireGump)))
                {
                    if (creaturePosition < 0 || creaturePosition >= m_Team.m_CreatureEntries.Count)
                        return;

                    PetBattleCreatureEntry creatureEntry = null;
                    
                    if (creaturePosition < m_Team.m_CreatureEntries.Count)
                        creatureEntry = m_Team.m_CreatureEntries[creaturePosition];

                    if (creatureEntry != null)
                    {
                        int page = (creatureIndex * 2) + 1;
                        
                        m_Team.m_Player.CloseGump(typeof(Gumps.PetBattleGrimoireGump));
                        m_Team.m_Player.SendGump(new Gumps.PetBattleGrimoireGump(page, m_Team, m_Team.m_Player, m_Team.m_Player.PetBattleCreatureCollection));
                    }
                }
            }

            //Open Grimoire for Creature
            if (info.ButtonID >= 11 && info.ButtonID <= 15)
            {   
                int creaturePosition = info.ButtonID - 11;

                if (creaturePosition < 0 || creaturePosition >= m_Team.m_CreatureEntries.Count)
                    return;

                PetBattleCreatureEntry creatureEntry = null;
                
                if (creaturePosition < m_Team.m_CreatureEntries.Count)
                    creatureEntry = m_Team.m_CreatureEntries[creaturePosition];

                int creatureIndex = 0;

                if (creatureEntry != null)
                {
                    m_Team.m_Player.PlaySound(0x055);

                    for (int a = 0; a < m_Team.m_Player.PetBattleCreatureCollection.m_CreatureEntries.Count; a++)
                    {
                        if (a >= m_Team.m_Player.PetBattleCreatureCollection.m_CreatureEntries.Count)
                            continue;

                        if (creatureEntry.m_Type == m_Team.m_Player.PetBattleCreatureCollection.m_CreatureEntries[a].m_Type)
                            creatureIndex = a;
                    }

                    int page = (creatureIndex * 2) + 1;
                    
                    m_Team.m_Player.CloseGump(typeof(Gumps.PetBattleGrimoireGump));

                    m_Team.m_Player.SendGump(new Gumps.PetBattleSelectCreaturesGump(m_PetBattle, m_Team.m_Player));
                    m_Team.m_Player.SendGump(new Gumps.PetBattleGrimoireGump(page, m_Team, m_Team.m_Player, m_Team.m_Player.PetBattleCreatureCollection));
                }
            }

            switch (info.ButtonID)
            {
                //Ready
                case 16:
                    if (m_Team.ready == true)
                        m_Team.ready = false;
                    else
                        m_Team.ready = true;

                    clickedReady = true;

                    SelectingCreaturesReadyChange();
                    break;

                //Quit
                case 17:
                    break;
            }

            if (valuesChanged && clickedReady == false)
                UpdatePetBattleSelectingCreaturesGump(m_Team.m_Player);
        }
    }

    public class PetBattleGrimoireGump : Gump
    {
        public int m_PageNo = 1;
        public int m_TotalPages = 0;

        public Custom.PetBattle.Team m_Team;
        public PetBattleCreatureCollection m_PlayerCreatureCollection;
        public PlayerMobile m_Player; 

        public PetBattleGrimoireGump(int PageNo, Custom.PetBattle.Team team, PlayerMobile player, PetBattleCreatureCollection playerCreatureCollection)
            : base(430, 125)
        {
            if (player == null || playerCreatureCollection == null)
                return;

            m_PageNo = PageNo;

            if (team != null)
            {
                m_Team = team;
                m_Team.grimoireBrowsingPage = m_PageNo;
            }

            m_PlayerCreatureCollection = playerCreatureCollection;
            m_Player = player;        

            m_TotalPages = playerCreatureCollection.m_CreatureEntries.Count * 2;                         
            
            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;          

            AddPage(0);

            //Book Image
            AddImage(0, 0, 11055);

            //Left Button
            if (m_PageNo > 1)
                AddButton(47, 9, 2205, 2205, 11, GumpButtonType.Reply, 0);

            //Right Button
            if (PageNo < m_TotalPages)
                AddButton(321, 9, 2206, 2206, 12, GumpButtonType.Reply, 0);

            bool attributesPage = true;

            if (m_PageNo % 2 == 0)
                attributesPage = false;

            int creatureIndex = (int)(Math.Ceiling((double)m_PageNo / 2)) - 1;

            PetBattleCreatureEntry creatureEntry = m_PlayerCreatureCollection.m_CreatureEntries[creatureIndex];
            BaseCreature creatureInstance = (BaseCreature)Activator.CreateInstance(creatureEntry.m_Type);

            if (creatureEntry == null || creatureInstance == null)
                return;

            //Attributes and Abilities Page
            if (attributesPage)
            {
                AddLabel(90, 10, 0, creatureInstance.PetBattleTitle);

                if (creatureInstance.PetBattleItemHue != -1)
                    AddItem(100 + creatureInstance.PetBattleGrimoireItemIdOffsetX, 30 + creatureInstance.PetBattleGrimoireItemIdOffsetY, creatureInstance.PetBattleItemId, creatureInstance.PetBattleItemHue);
                else
                    AddItem(100 + creatureInstance.PetBattleGrimoireItemIdOffsetX, 30 + creatureInstance.PetBattleGrimoireItemIdOffsetY, creatureInstance.PetBattleItemId);

                int startY = 75;
                int spacingY = 16;

                int leftTextX = 65;
                int leftDotsStartX = 95;
                int dotsSpacing = 13;

                //Health
                AddLabel(leftTextX, startY, 0, "HP");
                for (int b = 0; b < creatureInstance.PetBattleCreatureHealth; b++)
                {
                    AddImage(leftDotsStartX + (dotsSpacing * b), startY + 4, 2360);
                }

                startY += spacingY;

                //Damage
                AddLabel(leftTextX, startY, 0, "Dmg");
                for (int b = 0; b < creatureInstance.PetBattleCreatureDamage; b++)
                {
                    AddImage(leftDotsStartX + (dotsSpacing * b), startY + 4, 2360);
                }

                startY += spacingY;

                //Skill
                AddLabel(leftTextX, startY, 0, "Atk");
                for (int b = 0; b < creatureInstance.PetBattleCreatureAttack; b++)
                {
                    AddImage(leftDotsStartX + (dotsSpacing * b), startY + 4, 2360);
                }

                startY += spacingY;

                //Defend
                AddLabel(leftTextX, startY, 0, "Def");
                for (int b = 0; b < creatureInstance.PetBattleCreatureDefend; b++)
                {
                    AddImage(leftDotsStartX + (dotsSpacing * b), startY + 4, 2360);
                }

                startY += spacingY;

                //Speed
                AddLabel(leftTextX, startY, 0, "Spd");
                for (int b = 0; b < creatureInstance.PetBattleCreatureSpeed; b++)
                {
                    AddImage(leftDotsStartX + (dotsSpacing * b), startY + 4, 2360);
                }

                startY += spacingY;

                //Armor
                AddLabel(leftTextX, startY, 0, "AR");
                for (int b = 0; b < creatureInstance.PetBattleCreatureArmor; b++)
                {
                    AddImage(leftDotsStartX + (dotsSpacing * b), startY + 4, 2360);
                }

                startY += spacingY;

                //Resist
                AddLabel(leftTextX, startY, 0, "Res");
                for (int b = 0; b < creatureInstance.PetBattleCreatureResist; b++)
                {
                    AddImage(leftDotsStartX + (dotsSpacing * b), startY + 4, 2360);
                }

                //Right Side
                startY = 10;
                spacingY = 15;

                AddLabel(215, startY, 0, "Offensive Abilities");
                startY += spacingY;

                for (int a = 0; a < creatureInstance.m_PetBattleOffensiveAbilities.Count; a++)
                {
                    if (a < creatureEntry.m_OffensivePower)
                    {
                        AddButton(215, startY + 5, 2104, 2103, a + 1, GumpButtonType.Reply, 0);
                        AddLabel(230, startY, 0, creatureInstance.m_PetBattleOffensiveAbilities[a].m_Name);
                    }

                    else
                    {
                        AddLabel(230, startY, 0, "???");
                    }

                    startY += spacingY;
                }

                startY += 8;

                AddLabel(215, startY, 0, "Defensive Abilities");
                startY += spacingY;

                for (int a = 0; a < creatureInstance.m_PetBattleDefensiveAbilities.Count; a++)
                {
                    if (a < creatureEntry.m_DefensivePower)
                    {
                        AddButton(215, startY + 5, 2104, 2103, a + 6, GumpButtonType.Reply, 0);
                        AddLabel(230, startY, 0, creatureInstance.m_PetBattleDefensiveAbilities[a].m_Name);
                    }

                    else
                        AddLabel(230, startY, 0, "???");

                    startY += spacingY;
                }
            }

            //Stats and Description Page
            if (!attributesPage)
            {
                AddLabel(90, 10, 0, creatureInstance.PetBattleTitle);

                if (creatureInstance.PetBattleItemHue != -1)
                    AddItem(100 + creatureInstance.PetBattleGrimoireItemIdOffsetX, 30 + creatureInstance.PetBattleGrimoireItemIdOffsetY, creatureInstance.PetBattleItemId, creatureInstance.PetBattleItemHue);
                else
                    AddItem(100 + creatureInstance.PetBattleGrimoireItemIdOffsetX, 30 + creatureInstance.PetBattleGrimoireItemIdOffsetY, creatureInstance.PetBattleItemId);

                string expNeeded = "";

                if (creatureEntry.m_Level < PetBattle.levelExperience.Length)                
                    expNeeded = " / " + PetBattle.levelExperience[creatureEntry.m_Level];                

                AddLabel(70, 100, 0, "Level: " + creatureEntry.m_Level.ToString());
                AddLabel(70, 118, 0, "Exp: " + creatureEntry.m_Experience.ToString() + expNeeded);
                AddLabel(70, 136, 0, "Wins: " + creatureEntry.m_Wins.ToString());
                AddLabel(70, 154, 0, "Ties: " + creatureEntry.m_Ties.ToString());
                AddLabel(70, 172, 0, "Losses: " + creatureEntry.m_Losses.ToString());

                AddHtml(215, 35, 141, 300, creatureInstance.PetBattleDescription, (bool)false, (bool)false); 
            }

            creatureInstance.Delete();
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            base.OnResponse(sender, info);

            PlayerMobile pm_From = sender.Mobile as PlayerMobile;

            if (pm_From == null)
                return;

            int creatureIndex = (int)(Math.Ceiling((double)m_PageNo / 2)) - 1;

            BaseCreature creatureInstance = (BaseCreature)Activator.CreateInstance(m_PlayerCreatureCollection.m_CreatureEntries[creatureIndex].m_Type);
            
            if (creatureInstance == null)
                return;            
            
            //Offensive Ability
            if (info.ButtonID >= 1 && info.ButtonID <= 5)
            {
                pm_From.SendMessage(creatureInstance.m_PetBattleOffensiveAbilities[info.ButtonID - 1].m_Name + ": " + creatureInstance.m_PetBattleOffensiveAbilities[info.ButtonID - 1].m_Description);

                pm_From.CloseGump(typeof(Gumps.PetBattleGrimoireGump));
                pm_From.SendGump(new PetBattleGrimoireGump(m_PageNo, m_Team, m_Player, m_PlayerCreatureCollection));
            }

            //Defensive Ability
            if (info.ButtonID >= 6 && info.ButtonID <= 10)
            {
                pm_From.SendMessage(creatureInstance.m_PetBattleDefensiveAbilities[info.ButtonID - 6].m_Name + ": " + creatureInstance.m_PetBattleDefensiveAbilities[info.ButtonID - 6].m_Description);

                pm_From.CloseGump(typeof(Gumps.PetBattleGrimoireGump));
                pm_From.SendGump(new PetBattleGrimoireGump(m_PageNo, m_Team, m_Player, m_PlayerCreatureCollection));
            }

            switch (info.ButtonID)
            {
                //Previous
                case 11:
                    {
                        pm_From.PlaySound(0x055);
                        
                        if (m_PageNo == 1)
                        {
                            pm_From.CloseGump(typeof(Gumps.PetBattleGrimoireGump));
                            pm_From.SendGump(new PetBattleGrimoireGump(m_PageNo, m_Team, m_Player, m_PlayerCreatureCollection));
                        }

                        else
                        {
                            pm_From.CloseGump(typeof(Gumps.PetBattleGrimoireGump));
                            pm_From.SendGump(new PetBattleGrimoireGump(--m_PageNo, m_Team, m_Player, m_PlayerCreatureCollection));
                        }
                    }
                break;
                
                //Next
                case 12:
                    {
                        pm_From.PlaySound(0x055);

                        if (m_PageNo == m_TotalPages)
                        {
                            pm_From.CloseGump(typeof(Gumps.PetBattleGrimoireGump));
                            pm_From.SendGump(new PetBattleGrimoireGump(m_PageNo, m_Team, m_Player, m_PlayerCreatureCollection));
                        }

                        else
                        {
                            pm_From.CloseGump(typeof(Gumps.PetBattleGrimoireGump));
                            pm_From.SendGump(new PetBattleGrimoireGump(++m_PageNo, m_Team, m_Player, m_PlayerCreatureCollection));
                        }
                    }
                break;                
            }            
        }
    }
}