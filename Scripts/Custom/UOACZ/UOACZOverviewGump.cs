using System;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Custom;
using Server.Gumps;

namespace Server.Items
{
    public class UOACZOverviewGump : Gump
    {
        public PlayerMobile m_Player;       

        public UOACZOverviewGump(PlayerMobile player): base(10, 10)
        {
            if (player == null) return;
            if (player.Deleted) return;           

            m_Player = player;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddImage(2, 3, 9390);
            AddImage(513, 5, 9392);
            AddImage(2, 386, 9396);
            AddImage(514, 386, 9398);
            AddImageTiled(2, 139, 118, 250, 9393);
            AddImageTiled(115, 4, 400, 142, 9391);
            AddImageTiled(115, 386, 400, 140, 9397);
            AddImageTiled(513, 143, 104, 250, 9395);
            AddImageTiled(115, 142, 400, 250, 9394);

            //------------

            int textHue = 2655;

            AddLabel(28, 9, 149, "UOACZ Guide Website");
            AddButton(47, 30, 2095, 2094, 1, GumpButtonType.Reply, 0);

            AddLabel(243, 35, 2606, "UOACZ Session Overview");

            int humanPlayers = 0;
            int undeadPlayers = 0;
            int averageHumanScore = 0;
            int averageUndeadScore = 0;

            int totalHumanScores = 0;
            int totalUndeadScores = 0;

            if (UOACZPersistance.Active)
            {
                foreach (NetState state in NetState.Instances)
                {
                    Mobile mobile = state.Mobile;
                    PlayerMobile pm_Mobile = mobile as PlayerMobile;

                    if (pm_Mobile == null)
                        continue;

                    if (UOACZRegion.ContainsMobile(pm_Mobile))
                    {
                        if (pm_Mobile.AccessLevel > AccessLevel.Player)
                            continue;
                    }

                    if (pm_Mobile.IsUOACZHuman)
                    {
                        humanPlayers++;
                        totalHumanScores += pm_Mobile.m_UOACZAccountEntry.CurrentSessionHumanScore;
                    }

                    if (pm_Mobile.IsUOACZUndead)
                    {
                        undeadPlayers++;
                        totalUndeadScores += pm_Mobile.m_UOACZAccountEntry.CurrentSessionUndeadScore;
                    }
                }
            }

            if (humanPlayers > 0)            
                averageHumanScore = (int)(Math.Ceiling((double)totalHumanScores / (double)humanPlayers));            

            if (undeadPlayers > 0)
                averageUndeadScore = (int)(Math.Ceiling((double)totalUndeadScores / (double)undeadPlayers));               

            int humanPlayersTextHue = UOACZSystem.whiteTextHue;
            int undeadPlayersTextHue = UOACZSystem.whiteTextHue;
            int humanTotalScoreTextHue = UOACZSystem.whiteTextHue;
            int undeadTotalScoreTextHue = UOACZSystem.whiteTextHue;

            double humanUnderdogBonus = UOACZPersistance.HumanBalanceScalar - 1;
            double undeadUnderdogBonus = UOACZPersistance.UndeadBalanceScalar - 1;

            string humanUnderdogBonusText = Utility.CreateDecimalPercentageString(humanUnderdogBonus, 0);
            string undeadUnderdogBonusText = Utility.CreateDecimalPercentageString(undeadUnderdogBonus, 0);

            int humanUnderdogBonusTextHue = UOACZSystem.whiteTextHue;
            int undeadUnderdogBonusTextHue = UOACZSystem.whiteTextHue;

            bool humanRecommended = false;
            bool undeadRecommended = false;            
            
            if (humanPlayers > undeadPlayers)
                humanPlayersTextHue = UOACZSystem.greenTextHue;

            if (undeadPlayers > humanPlayers)
                undeadPlayersTextHue = UOACZSystem.greenTextHue;

            if (totalHumanScores > totalUndeadScores)
                humanTotalScoreTextHue = UOACZSystem.greenTextHue;

            if (totalUndeadScores > totalHumanScores)
                undeadTotalScoreTextHue = UOACZSystem.greenTextHue;

            if (humanUnderdogBonus > undeadUnderdogBonus)
            {
                humanUnderdogBonusTextHue = UOACZSystem.greenTextHue;
                humanRecommended = true;
            }

            if (undeadUnderdogBonus > humanUnderdogBonus)
            {
                undeadUnderdogBonusTextHue = UOACZSystem.greenTextHue;
                undeadRecommended = true;
            }

            if (humanUnderdogBonus == undeadUnderdogBonus)
            {
                humanRecommended = true;
                undeadRecommended = false;
            }

            //Active Players + Avg Score
            AddLabel(118, 60, 1164, "Human Team");
            AddLabel(49, 85, 1164, "Active Players");
            AddLabel(Utility.CenteredTextOffset(95, humanPlayers.ToString()), 105, humanPlayersTextHue, humanPlayers.ToString());
            AddLabel(186, 85, 1164, "Total Score");
            AddLabel(Utility.CenteredTextOffset(215, totalHumanScores.ToString()), 105, humanTotalScoreTextHue, totalHumanScores.ToString());

            AddLabel(446, 60, 2115, "Undead Team");
            AddLabel(380, 85, 2115, "Active Players");
            AddLabel(Utility.CenteredTextOffset(425, undeadPlayers.ToString()) , 105, undeadPlayersTextHue, undeadPlayers.ToString());
            AddLabel(516, 85, 2115, "Total Score");
            AddLabel(Utility.CenteredTextOffset(545,totalUndeadScores.ToString()) , 105, undeadTotalScoreTextHue, totalUndeadScores.ToString());

            //Human Objectives
            #region Objectives Determination

            int humanObjectivesCompleted = 0;
            int undeadObjectivesCompleted = 0;

            int humanTotalObjectives = UOACZSystem.ObjectivesNeededForVictory;
            int undeadTotalObjectives = UOACZSystem.ObjectivesNeededForVictory;

            bool humanObjective1Completed = false;
            bool humanObjective2Completed = false;
            bool humanObjective3Completed = false;
            bool humanObjective4Completed = false;
            bool humanObjective5Completed = false;

            bool undeadObjective1Completed = false;
            bool undeadObjective2Completed = false;
            bool undeadObjective3Completed = false;
            bool undeadObjective4Completed = false;
            bool undeadObjective5Completed = false;

            string humanObjective1Progress = "0%";
            string humanObjective2Progress = "0%";
            string humanObjective3Progress = "0%";
            string humanObjective4Progress = "0%";
            string humanObjective5Progress = "0%";

            string undeadObjective1Progress = "0%";
            string undeadObjective2Progress = "0%";
            string undeadObjective3Progress = "0%";
            string undeadObjective4Progress = "0%";
            string undeadObjective5Progress = "0%";

            double progress = 0;

            //Human Objective Progress
            if (UOACZPersistance.m_HumanObjective1Target != 0)
            {
                progress = UOACZPersistance.m_HumanObjective1 / UOACZPersistance.m_HumanObjective1Target;
                humanObjective1Progress = Utility.CreateDecimalPercentageString(progress, 1);

                if (UOACZPersistance.m_HumanObjective1 == UOACZPersistance.m_HumanObjective1Target)
                {
                    humanObjective1Completed = true;
                    humanObjectivesCompleted++;
                }
            }

            if (UOACZPersistance.m_HumanObjective2Target != 0)
            {
                progress = UOACZPersistance.m_HumanObjective2 / UOACZPersistance.m_HumanObjective2Target;
                humanObjective2Progress = Utility.CreateDecimalPercentageString(progress, 1);

                if (UOACZPersistance.m_HumanObjective2 == UOACZPersistance.m_HumanObjective2Target)
                {
                    humanObjective2Completed = true;
                    humanObjectivesCompleted++;
                }
            }

            if (UOACZPersistance.m_HumanObjective3Target != 0)
            {
                progress = UOACZPersistance.m_HumanObjective3 / UOACZPersistance.m_HumanObjective3Target;
                humanObjective3Progress = Utility.CreateDecimalPercentageString(progress, 1);

                if (UOACZPersistance.m_HumanObjective3 == UOACZPersistance.m_HumanObjective3Target)
                {
                    humanObjective3Completed = true;
                    humanObjectivesCompleted++;
                }
            }

            if (UOACZPersistance.m_HumanObjective4Target != 0)
            {
                progress = UOACZPersistance.m_HumanObjective4 / UOACZPersistance.m_HumanObjective4Target;
                humanObjective4Progress = Utility.CreateDecimalPercentageString(progress, 1);

                if (UOACZPersistance.m_HumanObjective4 == UOACZPersistance.m_HumanObjective4Target)
                {
                    humanObjective4Completed = true;
                    humanObjectivesCompleted++;
                }
            }

            if (UOACZPersistance.m_HumanObjective5Target != 0)
            {
                progress = UOACZPersistance.m_HumanObjective5 / UOACZPersistance.m_HumanObjective5Target;
                humanObjective5Progress = Utility.CreateDecimalPercentageString(progress, 1);

                if (UOACZPersistance.m_HumanObjective5 == UOACZPersistance.m_HumanObjective5Target)
                {
                    humanObjective5Completed = true;
                    humanObjectivesCompleted++;
                }
            }

            if (humanObjectivesCompleted > humanTotalObjectives)
                humanObjectivesCompleted = humanTotalObjectives;

            //Undead Objectives
            if (UOACZPersistance.m_UndeadObjective1Target != 0)
            {
                progress = UOACZPersistance.m_UndeadObjective1 / UOACZPersistance.m_UndeadObjective1Target;
                undeadObjective1Progress = Utility.CreateDecimalPercentageString(progress, 1);

                if (UOACZPersistance.m_UndeadObjective1 == UOACZPersistance.m_UndeadObjective1Target)
                {
                    undeadObjective1Completed = true;
                    undeadObjectivesCompleted++;
                }
            }

            if (UOACZPersistance.m_UndeadObjective2Target != 0)
            {
                progress = UOACZPersistance.m_UndeadObjective2 / UOACZPersistance.m_UndeadObjective2Target;
                undeadObjective2Progress = Utility.CreateDecimalPercentageString(progress, 1);

                if (UOACZPersistance.m_UndeadObjective2 == UOACZPersistance.m_UndeadObjective2Target)
                {
                    undeadObjective2Completed = true;
                    undeadObjectivesCompleted++;
                }
            }

            if (UOACZPersistance.m_UndeadObjective3Target != 0)
            {
                progress = UOACZPersistance.m_UndeadObjective3 / UOACZPersistance.m_UndeadObjective3Target;
                undeadObjective3Progress = Utility.CreateDecimalPercentageString(progress, 1);

                if (UOACZPersistance.m_UndeadObjective3 == UOACZPersistance.m_UndeadObjective3Target)
                {
                    undeadObjective3Completed = true;
                    undeadObjectivesCompleted++;
                }
            }

            if (UOACZPersistance.m_UndeadObjective4Target != 0)
            {
                progress = UOACZPersistance.m_UndeadObjective4 / UOACZPersistance.m_UndeadObjective4Target;
                undeadObjective4Progress = Utility.CreateDecimalPercentageString(progress, 1);

                if (UOACZPersistance.m_UndeadObjective4 == UOACZPersistance.m_UndeadObjective4Target)
                {
                    undeadObjective4Completed = true;
                    undeadObjectivesCompleted++;
                }
            }

            if (UOACZPersistance.m_UndeadObjective5Target != 0)
            {
                progress = UOACZPersistance.m_UndeadObjective5 / UOACZPersistance.m_UndeadObjective5Target;
                undeadObjective5Progress = Utility.CreateDecimalPercentageString(progress, 1);

                if (UOACZPersistance.m_UndeadObjective5 == UOACZPersistance.m_UndeadObjective5Target)
                {
                    undeadObjective5Completed = true;
                    undeadObjectivesCompleted++;
                }
            }

            if (undeadObjectivesCompleted > undeadTotalObjectives)
                undeadObjectivesCompleted = undeadTotalObjectives;

            #endregion
            
            AddLabel(35, 136, 1164, "Human Objectives (" + humanObjectivesCompleted.ToString() + "/" + humanTotalObjectives.ToString() + ")");

            if (humanObjective1Completed)
                textHue = UOACZSystem.greenTextHue;
            else
                textHue = UOACZSystem.whiteTextHue;

            AddLabel(35, 156, textHue, "Destroy Corruption Sourcestone");
            AddLabel(250, 156, textHue, humanObjective1Progress);

            if (humanObjective2Completed)
                textHue = UOACZSystem.greenTextHue;
            else
                textHue = UOACZSystem.whiteTextHue;

            AddLabel(35, 176, textHue, "Build the Outpost");
            AddLabel(250, 176, textHue, humanObjective2Progress);

            if (humanObjective3Completed)
                textHue = UOACZSystem.greenTextHue;
            else
                textHue = UOACZSystem.whiteTextHue;

            AddLabel(35, 196, textHue, "Gather Bones");
            AddLabel(250, 195, textHue, humanObjective3Progress);

            if (humanObjective4Completed)
                textHue = UOACZSystem.greenTextHue;
            else
                textHue = UOACZSystem.whiteTextHue;
            
            if (UOACZPersistance.UndeadChampion == null && !humanObjective4Completed)
            {
                int minutesUntilSpawn = UOACZSystem.UndeadChampionSpawnMinutes - UOACZPersistance.m_MinutesActive;
                string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, DateTime.UtcNow + TimeSpan.FromMinutes(minutesUntilSpawn), true, false, true, true, false);

                AddLabel(35, 215, textHue, "Undead Champion");
                AddLabel(250, 215, textHue, "(" + timeRemaining + ")");
            }

            else
            {
                AddLabel(35, 215, textHue, "Slay Undead Champion");
                AddLabel(250, 215, textHue, humanObjective4Progress);
            }

            if (humanObjective5Completed)
                textHue = UOACZSystem.greenTextHue;
            else
                textHue = UOACZSystem.whiteTextHue;

            if (UOACZPersistance.UndeadBoss == null && !humanObjective5Completed)
            {
                int minutesUntilSpawn = UOACZSystem.UndeadBossSpawnMinutes - UOACZPersistance.m_MinutesActive;
                string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, DateTime.UtcNow + TimeSpan.FromMinutes(minutesUntilSpawn), true, false, true, true, false);

                AddLabel(35, 235, textHue, "Undead Boss");
                AddLabel(250, 235, textHue, "(" + timeRemaining + ")");
            }

            else
            {
                AddLabel(35, 235, textHue, "Slay Undead Boss");
                AddLabel(250, 235, textHue, humanObjective5Progress);
            }

            //Undead Objectives
            AddLabel(380, 136, 2115, "Undead Objectives (" + undeadObjectivesCompleted.ToString() + "/" + undeadTotalObjectives.ToString() + ")");

            if (undeadObjective1Completed)
                textHue = UOACZSystem.greenTextHue;
            else
                textHue = UOACZSystem.whiteTextHue;

            AddLabel(380, 156, textHue, "Destroy Town Stockpiles");
            AddLabel(548, 156, textHue, undeadObjective1Progress);

            if (undeadObjective2Completed)
                textHue = UOACZSystem.greenTextHue;
            else
                textHue = UOACZSystem.whiteTextHue;

            AddLabel(380, 176, textHue, "Kill Town Civilians");
            AddLabel(547, 175, textHue, undeadObjective2Progress);

            if (undeadObjective3Completed)
                textHue = UOACZSystem.greenTextHue;
            else
                textHue = UOACZSystem.whiteTextHue;

            AddLabel(380, 196, textHue, "Spread Corruption");
            AddLabel(548, 196, textHue, undeadObjective3Progress);

            if (undeadObjective4Completed)
                textHue = UOACZSystem.greenTextHue;
            else
                textHue = UOACZSystem.whiteTextHue;

            if (UOACZPersistance.HumanChampion == null && !undeadObjective4Completed)
            {
                int minutesUntilSpawn = UOACZSystem.HumanChampionSpawnMinutes - UOACZPersistance.m_MinutesActive;
                string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, DateTime.UtcNow + TimeSpan.FromMinutes(minutesUntilSpawn), true, false, true, true, false);

                AddLabel(380, 216, textHue, "Human Champion");
                AddLabel(547, 216, textHue, "(" + timeRemaining + ")");
            }

            else
            {
                AddLabel(380, 216, textHue, "Slay Human Champion");
                AddLabel(547, 216, textHue, undeadObjective4Progress);
            }

            if (undeadObjective5Completed)
                textHue = UOACZSystem.greenTextHue;
            else
                textHue = UOACZSystem.whiteTextHue;

            if (UOACZPersistance.HumanBoss == null && !undeadObjective5Completed)
            {
                int minutesUntilSpawn = UOACZSystem.HumanBossSpawnMinutes - UOACZPersistance.m_MinutesActive;
                string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, DateTime.UtcNow + TimeSpan.FromMinutes(minutesUntilSpawn), true, false, true, true, false);

                AddLabel(379, 236, textHue, "Human Boss");
                AddLabel(547, 236, textHue, "(" + timeRemaining + ")");
            }
            else
            {
                AddLabel(379, 236, textHue, "Slay Human Boss");
                AddLabel(547, 236, textHue, undeadObjective5Progress);
            }
            
            //Underdog Bonus
            AddLabel(248, 263, 2550, "Team Underdog Bonus");
            AddLabel(111, 282, UOACZSystem.blueTextHue, "Bonus to Upgrade Chances, Loot Drops, and Ability Effectiveness");

            AddLabel(61, 314, 1164, "Human Player Underdog Bonus");
            AddImage(123, 338, 24019, 0);
            AddLabel(173, 348, humanUnderdogBonusTextHue, humanUnderdogBonusText);

            AddLabel(388, 314, 2115, "Undead Player Underdog Bonus");
            AddImage(450, 338, 24012, 0);
            AddLabel(503, 348, undeadUnderdogBonusTextHue, undeadUnderdogBonusText);
            
            //Human Team Selection
            if (humanRecommended)
                AddLabel(100, 392, 149, "(Recommended)");      
      
            if (player.m_UOACZAccountEntry.CurrentTeam == UOACZAccountEntry.ActiveProfileType.Human)
                AddLabel(Utility.CenteredTextOffset(145, "Currently Playing as Human"), 412, UOACZSystem.greenTextHue, "Currently Playing as Human");
            else
                AddLabel(Utility.CenteredTextOffset(145, "Become Human Player"), 412, UOACZSystem.whiteTextHue, "Become Human Player");
            
            if (player.m_UOACZAccountEntry.m_TeamSelectIndex == 0)
                AddButton(102, 447, 9724, 9721, 2, GumpButtonType.Reply, 0);
            else
                AddButton(102, 447, 9721, 9724, 2, GumpButtonType.Reply, 0);
            AddImage(140, 437, 11012, 0);

            //Undead Team Selection
            if (undeadRecommended)
                AddLabel(423, 392, 149, "(Recommended)");

            if (player.m_UOACZAccountEntry.CurrentTeam == UOACZAccountEntry.ActiveProfileType.Undead)
                AddLabel(Utility.CenteredTextOffset(470, "Currently Playing as Undead"), 412, UOACZSystem.greenTextHue, "Currently Playing as Undead");
            else
                AddLabel(Utility.CenteredTextOffset(470, "Become Undead Player"), 412, UOACZSystem.whiteTextHue, "Become Undead Player");
            
            if (player.m_UOACZAccountEntry.m_TeamSelectIndex == 1)
                AddButton(424, 447, 9724, 9721, 3, GumpButtonType.Reply, 0);
            else
                AddButton(424, 447, 9721, 9724, 3, GumpButtonType.Reply, 0);            
            AddImage(462, 437, 11011, 0);

            if ((player.m_UOACZAccountEntry.CurrentTeam == UOACZAccountEntry.ActiveProfileType.Human && player.m_UOACZAccountEntry.m_TeamSelectIndex == 1) ||
                (player.m_UOACZAccountEntry.CurrentTeam == UOACZAccountEntry.ActiveProfileType.Undead && player.m_UOACZAccountEntry.m_TeamSelectIndex == 0))
            {
                if (UOACZPersistance.Active)
                {
                    if (player.m_UOACZAccountEntry.NextTeamSwitchAllowed > DateTime.UtcNow)
                    {
                        string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, player.m_UOACZAccountEntry.NextTeamSwitchAllowed, true, true, true, true, true);
                        
                        AddLabel(Utility.CenteredTextOffset(315, "Cannot change teams for " + timeRemaining), 436, 149, "Cannot change teams for " + timeRemaining);
                    }

                    else
                    {
                        AddLabel(Utility.CenteredTextOffset(315, "Confirm Team Change"), 436, 149, "Confirm Team Change");
                        AddButton(283, 459, 238, 238, 4, GumpButtonType.Reply, 0);
                    }
                }

                else
                {
                    AddLabel(Utility.CenteredTextOffset(315, "Confirm Team Change"), 436, 149, "Confirm Team Change");
                    AddButton(283, 459, 238, 238, 4, GumpButtonType.Reply, 0);
                }
            }               
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            PlayerMobile player = sender.Mobile as PlayerMobile;

            if (player == null)
                return;

            if (player == null) return;
            if (player.Deleted) return;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            bool closeGump = true;

            switch (info.ButtonID)
            {
                //UOACZ Guide Website
                case 1:
                    string url = "http://forum.uoancorp.com/threads/the-monstrous-uoacz-guide.10635/";
                    player.LaunchBrowser(url);

                    closeGump = false;
                break;

                //Human Team
                case 2:
                    player.m_UOACZAccountEntry.m_TeamSelectIndex = 0;
                    player.SendSound(UOACZSystem.selectionSound);

                    closeGump = false;
                break;

                //Undead Team
                case 3:
                    player.m_UOACZAccountEntry.m_TeamSelectIndex = 1;
                    player.SendSound(UOACZSystem.selectionSound);

                    closeGump = false;
                break;

                //Confirm Team Change
                case 4:                    
                    if ((player.m_UOACZAccountEntry.CurrentTeam == UOACZAccountEntry.ActiveProfileType.Human && player.m_UOACZAccountEntry.m_TeamSelectIndex == 1) ||
                    (player.m_UOACZAccountEntry.CurrentTeam == UOACZAccountEntry.ActiveProfileType.Undead && player.m_UOACZAccountEntry.m_TeamSelectIndex == 0))
                    {
                        if (UOACZPersistance.Active)
                        {
                            if (player.m_UOACZAccountEntry.NextTeamSwitchAllowed <= DateTime.UtcNow)
                            {
                                if (player.LastPlayerCombatTime + UOACZSystem.TeamSwitchPvPThreshold > DateTime.UtcNow)
                                {
                                    string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, player.LastPlayerCombatTime + UOACZSystem.TeamSwitchPvPThreshold, false, true, true, true, true);

                                    player.SendMessage("You have been in PvP too recently to switch teams. You must wait another " + timeRemaining + ".");
                                }

                                else if (player.Frozen || player.CantWalk)
                                {
                                    player.SendMessage("You are currently frozen and must wait a few moments before you may switch teams.");
                                }

                                else
                                {
                                    if (player.m_UOACZAccountEntry.CurrentTeam == UOACZAccountEntry.ActiveProfileType.Human)
                                        UOACZSystem.SwitchTeams(player, true);

                                    else if (player.m_UOACZAccountEntry.CurrentTeam == UOACZAccountEntry.ActiveProfileType.Undead)
                                        UOACZSystem.SwitchTeams(player, false);
                                }
                            }
                        }

                        else
                        {
                            if (player.m_UOACZAccountEntry.CurrentTeam == UOACZAccountEntry.ActiveProfileType.Human)
                                UOACZSystem.SwitchTeams(player, true);

                            else if (player.m_UOACZAccountEntry.CurrentTeam == UOACZAccountEntry.ActiveProfileType.Undead)
                                UOACZSystem.SwitchTeams(player, false);
                        }
                    }                      

                    closeGump = false;
                break;
            }

            if (!closeGump)
                player.SendGump(new UOACZOverviewGump(player));

            else
            {
                player.CloseGump(typeof(UOACZOverviewGump));
                player.SendSound(UOACZSystem.closeGumpSound);
            }
        }
    }
}
