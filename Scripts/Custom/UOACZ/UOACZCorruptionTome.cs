using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using System.Collections;
using System.Collections.Generic;
using Server.Gumps;
using Server.Achievements;

namespace Server.Items
{
    public class UOACZCorruptionTome : Item
    {
        [Constructable]
        public UOACZCorruptionTome(): base(8787)
        {
            Name = "a corruption tome";
            Hue = 1104;

            Weight = 0;

            LootType = LootType.Blessed;
        }

        public UOACZCorruptionTome(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            if (!UOACZPersistance.Enabled)
            {
                from.SendMessage("UOACZ is currently disabled.");
                return;
            }

            player.SendSound(UOACZSystem.openGumpSound);
            
            from.CloseGump(typeof(UndeadProfileGump));
            from.SendGump(new UndeadProfileGump(player));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version           
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class UndeadProfileGump : Gump
    {
        PlayerMobile player;
        
        public UndeadProfileGump(PlayerMobile player): base(10, 10)
        {
            if (player == null) return;
            if (player.Deleted) return;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            if (player.m_UOACZAccountEntry == null) return;
            if (player.m_UOACZAccountEntry.UndeadProfile == null) return;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddImage(205, 193, 11008);
            AddImage(204, 1, 11008);
            AddImage(3, 192, 11008);
            AddImage(3, 1, 11008);
            AddImage(302, 75, 2081, 2499);
            AddImage(300, 270, 2081, 2499);
            AddImage(301, 141, 2081, 2499);
            AddImage(301, 5, 2081, 2499);
            AddImage(301, 206, 2081, 2499);
            AddImage(299, 338, 2081, 2499);
            AddImage(44, 6, 2081, 2499);
            AddImage(44, 75, 2081, 2499);
            AddImage(43, 141, 2081, 2499);
            AddImage(43, 206, 2081, 2499);
            AddImage(41, 335, 2081);
            AddImage(43, 274, 2081, 2499);
            AddImageTiled(301, 2, 6, 405, 2701);
            AddImage(41, 338, 2081, 2499);                        

            //Left Page
            AddLabel(102, 10, UOACZSystem.orangeTextHue, "UOACZ: Undead Profile");

            int monsterTier = player.m_UOACZAccountEntry.UndeadProfile.MonsterTier;
            int minimumDamage = player.m_UOACZAccountEntry.UndeadProfile.DamageMin;
            int maximumDamage = player.m_UOACZAccountEntry.UndeadProfile.DamageMax;
            int armor = player.m_UOACZAccountEntry.UndeadProfile.VirtualArmor;

            int followers = 0;

            if (player.m_UOACZAccountEntry.ActiveProfile == UOACZAccountEntry.ActiveProfileType.Undead)
                followers = player.Followers;

            int followersMax = player.m_UOACZAccountEntry.UndeadProfile.FollowersMax;            

            int undeadScore = player.m_UOACZAccountEntry.CurrentSessionUndeadScore;

            int upgradesSpent = player.m_UOACZAccountEntry.UndeadProfile.UpgradesSpent;

            string timeSpentUndead = Utility.CreateTimeRemainingString(DateTime.UtcNow, DateTime.UtcNow + TimeSpan.FromMinutes(player.m_UOACZAccountEntry.UndeadProfile.TotalMinutesUndead), true, true, true, true, true);

            if (timeSpentUndead == "" || timeSpentUndead == "1 second")
                timeSpentUndead = "0m";
            
            bool enableOverheadStatsText = player.m_UOACZAccountEntry.UndeadProfile.EnableOverheadStatsText;

            int corruptionPoints = player.m_UOACZAccountEntry.UndeadProfile.CorruptionPoints;
            int upgradePoints = player.m_UOACZAccountEntry.UndeadProfile.UpgradePoints;

            switch (player.m_UOACZAccountEntry.UndeadProfile.LeftPageNumber)
            {
                case 1:
                    string scoreLabel = "Undead Score";
                    int scoreLabelX = 93;
                    string scoreText = player.m_UOACZAccountEntry.CurrentSessionUndeadScore.ToString();

                    if (player.m_UOACZAccountEntry.CurrentSessionUndeadScore != player.m_UOACZAccountEntry.CurrentSessionTotalScore)
                    {
                        scoreLabel = "Undead Score (Total)";
                        scoreLabelX = 55;

                        scoreText = player.m_UOACZAccountEntry.CurrentSessionUndeadScore.ToString() + " (" + player.m_UOACZAccountEntry.CurrentSessionTotalScore.ToString() + ")";
                    }

                    AddLabel(scoreLabelX, 40, UOACZSystem.whiteTextHue, scoreLabel);
                    AddItem(179, 44, 6884);
                    AddLabel(221, 40, UOACZSystem.greenTextHue, scoreText);

                    AddLabel(58, 70, UOACZSystem.whiteTextHue, "Time Spent Undead");
                    AddItem(177, 66, 6160);
                    AddLabel(221, 70, UOACZSystem.greenTextHue, timeSpentUndead);

                    AddLabel(83, 100, UOACZSystem.whiteTextHue, "Upgrades Spent");
                    AddItem(185, 101, 6935);
                    AddLabel(221, 100, UOACZSystem.greenTextHue, upgradesSpent.ToString());

                    AddButton(72, 140, 2152, 2154, 6, GumpButtonType.Reply, 0);
                    AddLabel(111, 145, UOACZSystem.whiteTextHue, "Show Stats Hotbar");

                    AddButton(72, 178, 2152, 2154, 7, GumpButtonType.Reply, 0);
                    AddLabel(112, 183, UOACZSystem.whiteTextHue, "Show Abilities Hotbar");

                    AddButton(72, 217, 2152, 2154, 8, GumpButtonType.Reply, 0);
                    AddLabel(112, 221, UOACZSystem.whiteTextHue, "Show Objectives Hotbar");
                    break;

                case 2:
                    AddLabel(86, 42, UOACZSystem.whiteTextHue, "Monster Tier");
                    AddItem(161, 37, 7782, 2510);
                    AddLabel(207, 42, UOACZSystem.greenTextHue, monsterTier.ToString());

                    AddLabel(120, 72, UOACZSystem.whiteTextHue, "Damage");            
                    AddItem(168, 74, 11704);
                    AddLabel(207, 72, UOACZSystem.greenTextHue, minimumDamage.ToString() + " - " + maximumDamage.ToString());

                    AddLabel(127, 102, UOACZSystem.whiteTextHue, "Armor");
                    AddItem(164, 101, 7034);
                    AddLabel(207, 102, UOACZSystem.greenTextHue, armor.ToString());

                    AddLabel(93, 132, UOACZSystem.whiteTextHue, "Swarm Size");
                    AddItem(178, 120, 9685);
                    AddLabel(207, 132, UOACZSystem.greenTextHue, followers.ToString() + " / " + followersMax.ToString());  
                  
                    AddLabel(120, 162, UOACZSystem.whiteTextHue, "Fatigue");
                    AddItem(164, 157, 2598);

                    string fatigueRemaining = "None";

                    if (player.m_UOACZAccountEntry.FatigueExpiration > DateTime.UtcNow)
                        fatigueRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, player.m_UOACZAccountEntry.FatigueExpiration, true, true, true, true, true);

                    AddLabel(207, 162, UOACZSystem.greenTextHue, fatigueRemaining); 
                break;                
            }

            AddLabel(111, 257, UOACZSystem.whiteTextHue, "More Info");

            if (player.m_UOACZAccountEntry.UndeadProfile.LeftPageNumber > 1)
                AddButton(177, 255, 4014, 4016, 5, GumpButtonType.Reply, 0); //Left

            if (player.m_UOACZAccountEntry.UndeadProfile.LeftPageNumber == 1)
                AddButton(177, 255, 4005, 4007, 5, GumpButtonType.Reply, 0); //Right

            AddLabel(63, 294, UOACZSystem.greyTextHue, "Corruption Points Available");
            AddItem(222, 297, 22336);
            AddLabel(260, 294, UOACZSystem.blueTextHue, corruptionPoints.ToString());

            AddLabel(80, 319, UOACZSystem.yellowTextHue, "Upgrade Points Available");
            AddItem(229, 321, 6935);
            AddLabel(260, 320, UOACZSystem.blueTextHue, upgradePoints.ToString());      

            UOACZUndeadUpgradeType m_ActiveForm;
            UOACZUndeadUpgradeDetail m_UpgradeDetail;

            string formName;
            int iconItemID;
            int iconHue;
            int iconOffsetX;
            int iconOffsetY;

            int startY = 60;

            //Right Page
            switch(player.m_UOACZAccountEntry.UndeadProfile.ActivePage)
            {
                case UOACZAccountEntry.UndeadProfileEntry.UndeadActivePageType.Abilities:
                    m_ActiveForm = player.m_UOACZAccountEntry.UndeadProfile.ActiveForm;

                    if (m_ActiveForm == null)
                        return;

                    m_UpgradeDetail = UOACZUndeadUpgrades.GetUpgradeDetail(m_ActiveForm);

                    if (m_UpgradeDetail == null)
                        return;

                    formName = m_UpgradeDetail.m_Name;
                    iconItemID = m_UpgradeDetail.m_IconItemID;
                    iconHue = m_UpgradeDetail.m_IconHue;
                    iconOffsetX = m_UpgradeDetail.m_IconOffsetX;
                    iconOffsetY = m_UpgradeDetail.m_IconOffsetY;
                                        
                    AddLabel(386, 10, UOACZSystem.orangeTextHue, "Current Form");
                    AddLabel(Utility.CenteredTextOffset(430, formName), 30, UOACZSystem.greenTextHue, formName);
                    AddItem(498 + iconOffsetX, 8 + iconOffsetY, iconItemID, iconHue);     
                    
                    //TEST: Implement Display of Generic Abilities
                    for (int a = 0; a < m_UpgradeDetail.m_Abilities.Count; a++)
                    {                 
                        UOACZUndeadAbilityType abilityType = m_UpgradeDetail.m_Abilities[a];
                        UOACZUndeadAbilityDetail abilityDetail = UOACZUndeadAbilities.GetAbilityDetail(abilityType);
                        UOACZUndeadAbilityEntry abilityEntry = UOACZUndeadAbilities.GetAbilityEntry(player.m_UOACZAccountEntry, abilityType);

                        string cooldownDurationText = "";
                        int cooldownTextHue = 0;

                        if (player.m_UOACZAccountEntry.ActiveProfile != UOACZAccountEntry.ActiveProfileType.Undead)
                        {
                            string adjustedCooldownText = Utility.CreateTimeRemainingString(DateTime.UtcNow, DateTime.UtcNow + TimeSpan.FromMinutes(abilityEntry.m_CooldownMinutes), true, false, true, true, true);

                            cooldownDurationText = "Cooldown: " + adjustedCooldownText;
                            cooldownTextHue = UOACZSystem.blueTextHue;
                        }

                        else
                        {
                            if (DateTime.UtcNow >= abilityEntry.m_NextUsageAllowed)
                            {
                                cooldownDurationText = "Ready";
                                cooldownTextHue = UOACZSystem.greenTextHue;
                            }

                            else
                            {
                                cooldownDurationText = "Usable in " + Utility.CreateTimeRemainingString(DateTime.UtcNow, abilityEntry.m_NextUsageAllowed, true, false, true, true, true);
                                cooldownTextHue = UOACZSystem.blueTextHue;
                            }
                        }

                        AddButton(320, startY, 24012, 24012, 50 + a, GumpButtonType.Reply, 0);
                        AddLabel(370, startY - 7, UOACZSystem.yellowTextHue, abilityDetail.Name);
                        AddLabel(370, startY + 11, cooldownTextHue, cooldownDurationText);
                        AddButton(370, startY + 32, 2118, 2118, 60 + a, GumpButtonType.Reply, 0);
                        AddLabel(388, startY + 29, UOACZSystem.lightGreenTextHue, "Info");                       

                        startY += 55;
                    }
                break;

                case UOACZAccountEntry.UndeadProfileEntry.UndeadActivePageType.StatsAndSkills:    
                     List<KeyValuePair<SkillName, double>> m_ValidSkills = new List<KeyValuePair<SkillName, double>>();

                    foreach (KeyValuePair<SkillName, double> keyValuePair in player.m_UOACZAccountEntry.UndeadProfile.m_Skills)
                    {
                        if (keyValuePair.Value > 0)
                            m_ValidSkills.Add(keyValuePair);
                    }

                    List<KeyValuePair<SkillName, double>> m_SkillsOrder = new List<KeyValuePair<SkillName, double>>();

                    for (int a = 0; a < m_ValidSkills.Count; a++)
                    {
                        int index = -1;                        

                        for (int b = 0; b < m_SkillsOrder.Count; b++)
                        {
                            int skillIndex = m_SkillsOrder.Count - 1 - b;

                            if (skillIndex >= 0 && skillIndex < m_SkillsOrder.Count)
                            {
                                if (m_ValidSkills[a].Value >= m_SkillsOrder[skillIndex].Value)
                                    index = skillIndex;
                            }
                        }

                        if (index == -1)
                            m_SkillsOrder.Add(m_ValidSkills[a]);
                        else
                            m_SkillsOrder.Insert(index, m_ValidSkills[a]);
                    }

                    List<string> m_Items = new List<string>();

                    if (!player.m_UOACZAccountEntry.UndeadProfile.m_Stats.ContainsKey(StatType.Str))
                        player.m_UOACZAccountEntry.UndeadProfile.m_Stats.Add(StatType.Str, 10);

                    if (!player.m_UOACZAccountEntry.UndeadProfile.m_Stats.ContainsKey(StatType.Dex))
                        player.m_UOACZAccountEntry.UndeadProfile.m_Stats.Add(StatType.Dex, 10);

                    if (!player.m_UOACZAccountEntry.UndeadProfile.m_Stats.ContainsKey(StatType.Int))
                        player.m_UOACZAccountEntry.UndeadProfile.m_Stats.Add(StatType.Int, 10);

                    m_Items.Add(player.m_UOACZAccountEntry.UndeadProfile.m_Stats[StatType.Str].ToString() + " Strength");
                    m_Items.Add(player.m_UOACZAccountEntry.UndeadProfile.m_Stats[StatType.Dex].ToString() + " Dexterity");
                    m_Items.Add(player.m_UOACZAccountEntry.UndeadProfile.m_Stats[StatType.Int].ToString() + " Intelligence");

                    m_Items.Add("");

                    for (int a = 0; a < m_SkillsOrder.Count; a++)
                    {
                        SkillName skillName = m_SkillsOrder[a].Key;
                        double value = m_SkillsOrder[a].Value;

                        string skillText = skillName.ToString();
                        
                        switch (skillName)
                        {
                            case SkillName.AnimalLore: skillText = "Animal Lore"; break;
                            case SkillName.AnimalTaming: skillText = "Animal Taming"; break;
                            case SkillName.ArmsLore: skillText = "Arms Lore"; break;
                            case SkillName.DetectHidden: skillText = "Detect Hidden"; break;
                            case SkillName.EvalInt: skillText = "Evaluating Intelligence"; break;
                            case SkillName.ItemID: skillText = "Item Identification"; break;
                            case SkillName.MagicResist: skillText = "Magic Resist"; break;
                            case SkillName.RemoveTrap: skillText = "Remove Trap"; break;
                            case SkillName.SpiritSpeak: skillText = "Spirit Speaking"; break;
                            case SkillName.TasteID: skillText = "Taste Identification"; break;
                        }

                        m_Items.Add(value.ToString() + " " + skillText);
                    }

                    int textLinesPerPage = 12;
                    int totalStatSkillPages = (int)(Math.Ceiling((double)m_Items.Count / (double)textLinesPerPage));

                    if (totalStatSkillPages == 0)
                        totalStatSkillPages = 1;

                    if (player.m_UOACZAccountEntry.UndeadProfile.RightPageNumber < 1)
                        player.m_UOACZAccountEntry.UndeadProfile.RightPageNumber = 1;

                    if (player.m_UOACZAccountEntry.UndeadProfile.RightPageNumber > totalStatSkillPages)
                        player.m_UOACZAccountEntry.UndeadProfile.RightPageNumber = totalStatSkillPages;

                    AddLabel(385, 10, UOACZSystem.orangeTextHue, "Stats and Skills");

                    startY = 42;

                    int startIndex = (player.m_UOACZAccountEntry.UndeadProfile.RightPageNumber - 1) * textLinesPerPage;
                    int endIndex;

                    if (player.m_UOACZAccountEntry.UndeadProfile.RightPageNumber < totalStatSkillPages)
                        endIndex = (player.m_UOACZAccountEntry.UndeadProfile.RightPageNumber * textLinesPerPage) - 1;
                    else
                        endIndex = m_Items.Count - 1; 

                    for (int a = startIndex; a < endIndex + 1; a++)
                    {
                        if (a >= 0 && a < m_Items.Count)
                        {
                            if (player.m_UOACZAccountEntry.UndeadProfile.RightPageNumber == 1 && a == 0 || a == 1 || a == 2)
                                AddLabel(395, startY, UOACZSystem.blueTextHue, m_Items[a]);
                            else
                                AddLabel(395, startY, UOACZSystem.yellowTextHue, m_Items[a]);
                        }

                        startY += 20;
                    }

                    //Display Stats and Skills
                    if (player.m_UOACZAccountEntry.UndeadProfile.RightPageNumber > 1)            
                        AddButton(320, 323, 4014, 4016, 9, GumpButtonType.Reply, 0); //Left

                    if (player.m_UOACZAccountEntry.UndeadProfile.RightPageNumber < totalStatSkillPages)
                        AddButton(515, 323, 4005, 4007, 10, GumpButtonType.Reply, 0); //Right
                break;
            }

            AddLabel(60, 350, UOACZSystem.orangeTextHue, "Overview / Teams");
            AddButton(100, 375, 9721, 9724, 11, GumpButtonType.Reply, 0);

            AddLabel(190, 350, UOACZSystem.greenTextHue, "Spend Upgrades");
            AddButton(230, 375, 9721, 9724, 1, GumpButtonType.Reply, 0);

            if (player.m_UOACZAccountEntry.UndeadProfile.ActivePage == UOACZAccountEntry.UndeadProfileEntry.UndeadActivePageType.Abilities)
            {
                AddLabel(342, 350, UOACZSystem.blueTextHue, "Abilities");
                AddButton(352, 375, 9724, 9721, 2, GumpButtonType.Reply, 0);
            }

            else
            {
                AddLabel(342, 350, UOACZSystem.whiteTextHue, "Abilities");
                AddButton(352, 375, 9721, 9724, 2, GumpButtonType.Reply, 0);
            }

            if (player.m_UOACZAccountEntry.UndeadProfile.ActivePage == UOACZAccountEntry.UndeadProfileEntry.UndeadActivePageType.StatsAndSkills)
            {
                AddLabel(441, 350, UOACZSystem.orangeTextHue, "Stats and Skills");
                AddButton(478, 375, 9724, 9721, 3, GumpButtonType.Reply, 0);
            }

            else
            {
                AddLabel(441, 350, UOACZSystem.whiteTextHue, "Stats and Skills");
                AddButton(478, 375, 9721, 9724, 3, GumpButtonType.Reply, 0);
            }            
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (sender == null)
                return;

            PlayerMobile player = sender.Mobile as PlayerMobile;

            if (player == null) return;
            if (player.Deleted) return;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            bool closeGump = true;

            switch (info.ButtonID)
            {
                case 1:
                    //Spend Upgrade
                    player.SendGump(new UndeadProfileGump(player));

                    player.CloseGump(typeof(UndeadProfileUpgradeGump));
                    player.SendGump(new UndeadProfileUpgradeGump(player));

                    player.SendSound(UOACZSystem.changeGumpSound);

                    return;
                break;

                case 2:
                    //Active Form Page                    
                    player.m_UOACZAccountEntry.UndeadProfile.RightPageNumber = 1;
                    player.m_UOACZAccountEntry.UndeadProfile.ActivePage = UOACZAccountEntry.UndeadProfileEntry.UndeadActivePageType.Abilities;

                    player.SendSound(UOACZSystem.changeGumpSound);

                    closeGump = false;                    
                break;

                case 3:
                    //Stats and Skills Page                    
                    player.m_UOACZAccountEntry.UndeadProfile.RightPageNumber = 1;
                    player.m_UOACZAccountEntry.UndeadProfile.ActivePage = UOACZAccountEntry.UndeadProfileEntry.UndeadActivePageType.StatsAndSkills;

                    player.SendSound(UOACZSystem.changeGumpSound);

                    closeGump = false;                    
                break;

                case 5:
                    //Previous Left Page
                    if (player.m_UOACZAccountEntry.UndeadProfile.LeftPageNumber > 1)
                        player.m_UOACZAccountEntry.UndeadProfile.LeftPageNumber = 1;

                    //Next Left Page
                    else
                        player.m_UOACZAccountEntry.UndeadProfile.LeftPageNumber = 2;

                    player.SendSound(UOACZSystem.changeGumpSound);

                    closeGump = false;
                break;

                case 6:
                    //Show Stats Hotbar
                    player.CloseGump(typeof(UndeadProfileStatsHotbarGump));
                    player.SendGump(new UndeadProfileStatsHotbarGump(player));

                    player.SendSound(UOACZSystem.selectionSound);

                    closeGump = false;
                break;

                case 7:
                    //Show Abilities Hotbar
                    player.CloseGump(typeof(UndeadProfileAbilitiesHotbarGump));
                    player.SendGump(new UndeadProfileAbilitiesHotbarGump(player));

                    player.SendSound(UOACZSystem.selectionSound);

                    closeGump = false;
                break;

                case 8:
                    //Show Objectives Hotbar               
                    player.CloseGump(typeof(ObjectivesHotbarGump));
                    player.SendGump(new ObjectivesHotbarGump(player));

                    player.SendSound(UOACZSystem.selectionSound);

                    closeGump = false;
                break;

                case 9:
                    //Previous Right Page
                    if (player.m_UOACZAccountEntry.UndeadProfile.RightPageNumber > 1)
                    {                        
                        player.m_UOACZAccountEntry.UndeadProfile.RightPageNumber--;
                        player.SendSound(UOACZSystem.changeGumpSound);
                    }

                    closeGump = false;
                break;

                case 10:
                    //Next Right Page
                    player.m_UOACZAccountEntry.UndeadProfile.RightPageNumber++;
                    player.SendSound(UOACZSystem.changeGumpSound);

                    closeGump = false;
                break;

                case 11:
                    //Overview / Teams      
                    player.SendGump(new UndeadProfileGump(player));

                    player.CloseGump(typeof(UOACZOverviewGump));
                    player.SendGump(new UOACZOverviewGump(player));                    

                    player.SendSound(UOACZSystem.selectionSound);

                    return;
                break; 
            }

            //Active Form
            if (player.m_UOACZAccountEntry.UndeadProfile.ActivePage == UOACZAccountEntry.UndeadProfileEntry.UndeadActivePageType.Abilities)
            {
                UOACZUndeadUpgradeDetail upgradeDetail = UOACZUndeadUpgrades.GetUpgradeDetail(player.m_UOACZAccountEntry.UndeadProfile.ActiveForm);
                
                //Activate Ability
                if (info.ButtonID >= 50 && info.ButtonID <= 54)
                {                    
                    int abilityIndex = info.ButtonID - 50;

                    if (abilityIndex >= 0 && abilityIndex < upgradeDetail.m_Abilities.Count)
                    {
                        UOACZUndeadAbilityType abilityType = upgradeDetail.m_Abilities[abilityIndex];
                        UOACZUndeadAbilities.ActivateAbility(player, abilityType);
                    }

                    closeGump = false;
                }

                //Display Description
                if (info.ButtonID >= 60 && info.ButtonID <= 64)
                {
                    int abilityIndex = info.ButtonID - 60;

                    if (abilityIndex >= 0 && abilityIndex < upgradeDetail.m_Abilities.Count)
                    {
                        UOACZUndeadAbilityType abilityType = upgradeDetail.m_Abilities[abilityIndex];
                        UOACZUndeadAbilityDetail abilityDetail = UOACZUndeadAbilities.GetAbilityDetail(abilityType);
                        UOACZUndeadAbilityEntry abilityEntry = UOACZUndeadAbilities.GetAbilityEntry(player.m_UOACZAccountEntry, abilityType);

                        string description = "";

                        if (abilityDetail.Description != null)
                        {
                            if (abilityDetail.Description.Length > 0)
                            {
                                for (int a = 0; a < abilityDetail.Description.Length; a++)
                                {
                                    description += abilityDetail.Description[a];
                                }
                            }
                        }

                        string adjustedCooldownText = Utility.CreateTimeRemainingString(DateTime.UtcNow, DateTime.UtcNow + TimeSpan.FromMinutes(abilityEntry.m_CooldownMinutes), true, false, true, true, true);

                        player.SendMessage(UOACZSystem.yellowTextHue, abilityDetail.Name + ": " + description + " (Has a cooldown of " + adjustedCooldownText + ")");
                    }

                    closeGump = false;
                }
            }

            if (!closeGump)
                player.SendGump(new UndeadProfileGump(player));

            else
            {
                player.CloseGump(typeof(UndeadProfileUpgradeGump));
                player.SendSound(UOACZSystem.closeGumpSound);
            }
        }
    }

    public class UndeadProfileUpgradeGump : Gump
    {  
        PlayerMobile player;        

        public UndeadProfileUpgradeGump(PlayerMobile pm_From): base(200, 100)
        {
            if (pm_From == null) return;
            if (pm_From.Deleted) return;

            player = pm_From;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);            

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddImage(648, 29, 3505, 2499);
            AddImage(412, 4, 3501, 2499);
            AddImage(648, 4, 3502, 2499);
            AddImage(3, 430, 3506, 2499);
            AddImage(3, 188, 3503, 2499);
            AddImage(3, 3, 3500, 2499);
            AddImage(648, 431, 3508, 2499);
            AddImage(27, 4, 3501, 2499);
            AddImage(265, 4, 3501, 2499);
            AddImage(648, 191, 3505, 2499);
            AddImage(3, 27, 3503, 2499);
            AddImage(28, 429, 3507, 2499);
            AddImage(256, 429, 3507, 2499);
            AddImage(410, 429, 3507, 2499);
            AddImage(28, 28, 3504, 2499);
            AddImage(28, 193, 3504, 2499);
            AddImage(266, 29, 3504, 2499);
            AddImage(265, 197, 3504, 2499);
            AddImage(409, 24, 3504, 2499);
            AddImage(412, 190, 3504, 2499);
            
            UOACZUndeadUpgradeType activeUpgradeType = player.m_UOACZAccountEntry.UndeadProfile.ActiveForm;
            UOACZUndeadUpgradeDetail upgradeDetail = UOACZUndeadUpgrades.GetUpgradeDetail(activeUpgradeType);
            
            int corruptionPoints = player.m_UOACZAccountEntry.UndeadProfile.CorruptionPoints;
            int upgradePoints = player.m_UOACZAccountEntry.UndeadProfile.UpgradePoints;
            int monsterTier = upgradeDetail.m_MonsterTier;
            int corruptionCost = 1;
            int upgradeCost = UOACZSystem.UndeadMonsterTierUpgradeCosts[monsterTier - 1];

            AddLabel(75, 15, UOACZSystem.greenTextHue, "Current Form");
            AddLabel(75, 35, UOACZSystem.orangeTextHue, upgradeDetail.m_Name);
            AddLabel(75, 55, UOACZSystem.lightPurpleTextHue, "Tier " + monsterTier.ToString() + " Monster");

            int iconItemID = upgradeDetail.m_IconItemID;
            int iconHue = upgradeDetail.m_IconHue;
            int iconOffsetX = upgradeDetail.m_IconOffsetX;
            int iconOffsetY = upgradeDetail.m_IconOffsetY;            
            
            AddItem(95 + iconOffsetX, 85 + iconOffsetY, iconItemID, iconHue);

            AddLabel(41, 185, UOACZSystem.greenTextHue, "Transform to a Different");
            AddLabel(35, 205, UOACZSystem.greenTextHue, "Randomized Tier " + monsterTier.ToString() + " Monster");
            AddLabel(45, 230, UOACZSystem.greyTextHue, "Corruption Cost");
            AddItem(140, 232, 22336, 0);
            AddLabel(177, 230, UOACZSystem.blueTextHue, corruptionCost.ToString());
            AddLabel(47, 250, UOACZSystem.greyTextHue, "Points Available");
            AddItem(140, 252, 22336, 0);
            AddLabel(177, 250, UOACZSystem.blueTextHue, corruptionPoints.ToString());
            AddButton(105, 275, 2152, 2152, 1, GumpButtonType.Reply, 0);

            if (monsterTier < 5)
            {
                AddLabel(35, 345, UOACZSystem.greenTextHue, "Upgrade to Monster Tier " + (monsterTier + 1).ToString());
                AddLabel(65, 365, UOACZSystem.yellowTextHue, "Upgrade Cost");
                AddItem(150, 365, 6935, 0);
                AddLabel(180, 365, UOACZSystem.blueTextHue, upgradeCost.ToString());
                AddLabel(50, 385, UOACZSystem.yellowTextHue, "Points Available");
                AddItem(150, 385, 6935, 0);
                AddLabel(180, 385, UOACZSystem.blueTextHue, upgradePoints.ToString());
                AddButton(105, 410, 2152, 2152, 2, GumpButtonType.Reply, 0);
            }

            else if (monsterTier == 5)
            {
                upgradeCost = UOACZSystem.UndeadPostTier5UpgradeCost;

                AddLabel(35, 345, UOACZSystem.greenTextHue, "Increase Stats and Skills by " + Utility.CreatePercentageString(UOACZSystem.UndeadPostTier5StatSkillIncrease));
                AddLabel(65, 365, UOACZSystem.yellowTextHue, "Upgrade Cost");
                AddItem(150, 365, 6935, 0);
                AddLabel(180, 365, UOACZSystem.blueTextHue, upgradeCost.ToString());
                AddLabel(50, 385, UOACZSystem.yellowTextHue, "Points Available");
                AddItem(150, 385, 6935, 0);
                AddLabel(180, 385, UOACZSystem.blueTextHue, upgradePoints.ToString());
                AddButton(105, 410, 2152, 2152, 2, GumpButtonType.Reply, 0);
            }

            AddLabel(275, 15, UOACZSystem.greenTextHue, "Stats and Skills");           

            int skillStatsStartY = 45;

            //Description
            List<string> m_DescriptionLines = new List<string>();

            string strText = upgradeDetail.m_Stats[StatType.Str].ToString();
            string dexText = upgradeDetail.m_Stats[StatType.Dex].ToString();
            string intText = upgradeDetail.m_Stats[StatType.Int].ToString();

            string damageMinText = upgradeDetail.m_DamageMin.ToString();
            string damageMaxText = upgradeDetail.m_DamageMax.ToString();
            string virtualArmorText = upgradeDetail.m_VirtualArmor.ToString();

            int postTier5Upgrades = player.m_UOACZAccountEntry.UndeadProfile.PostTier5Upgrades;

            if (postTier5Upgrades > 0)
            {
                strText = ((int)Math.Ceiling((double)upgradeDetail.m_Stats[StatType.Str] * (1 + ((double)postTier5Upgrades * UOACZSystem.UndeadPostTier5StatSkillIncrease)))).ToString();
                dexText = ((int)Math.Ceiling((double)upgradeDetail.m_Stats[StatType.Dex] * (1 + ((double)postTier5Upgrades * UOACZSystem.UndeadPostTier5StatSkillIncrease)))).ToString();
                intText = ((int)Math.Ceiling((double)upgradeDetail.m_Stats[StatType.Int] * (1 + ((double)postTier5Upgrades * UOACZSystem.UndeadPostTier5StatSkillIncrease)))).ToString();

                damageMinText = ((int)Math.Ceiling((double)upgradeDetail.m_DamageMin * (1 + ((double)postTier5Upgrades * UOACZSystem.UndeadPostTier5StatSkillIncrease)))).ToString();
                damageMaxText = ((int)Math.Ceiling((double)upgradeDetail.m_DamageMax * (1 + ((double)postTier5Upgrades * UOACZSystem.UndeadPostTier5StatSkillIncrease)))).ToString();
                virtualArmorText = ((int)Math.Ceiling((double)upgradeDetail.m_VirtualArmor * (1 + ((double)postTier5Upgrades * UOACZSystem.UndeadPostTier5StatSkillIncrease)))).ToString();
            }

            if (upgradeDetail.m_Stats.Count > 0)
            {
                if (upgradeDetail.m_Stats[StatType.Str] > 0)
                    m_DescriptionLines.Add(strText + " Strength");

                if (upgradeDetail.m_Stats[StatType.Dex] > 0)
                    m_DescriptionLines.Add(dexText + " Dexterity");

                if (upgradeDetail.m_Stats[StatType.Int] > 0)
                    m_DescriptionLines.Add(intText + " Intelligence");
            }

            m_DescriptionLines.Add("");

            m_DescriptionLines.Add(damageMinText + " - " + damageMaxText + " Damage");
            m_DescriptionLines.Add(virtualArmorText + " Armor");
            m_DescriptionLines.Add(upgradeDetail.m_MaxFollowers.ToString() + " Swarm Size");

            m_DescriptionLines.Add("");

            List<KeyValuePair<SkillName, double>> m_ValidSkills = new List<KeyValuePair<SkillName, double>>();

            foreach (KeyValuePair<SkillName, double> keyValuePair in upgradeDetail.m_Skills)
            {
                if (keyValuePair.Value > 0)
                {
                    m_ValidSkills.Add(keyValuePair);
                }
            }

            List<KeyValuePair<SkillName, double>> m_SkillsOrder = new List<KeyValuePair<SkillName, double>>();

            for (int a = 0; a < m_ValidSkills.Count; a++)
            {
                int index = -1;

                for (int b = 0; b < m_SkillsOrder.Count; b++)
                {
                    int skillIndex = m_SkillsOrder.Count - 1 - b;

                    if (skillIndex >= 0 && skillIndex < m_SkillsOrder.Count)
                    {
                        if (m_ValidSkills[a].Value >= m_SkillsOrder[skillIndex].Value)
                            index = skillIndex;
                    }
                }

                if (index == -1)
                    m_SkillsOrder.Add(m_ValidSkills[a]);
                else
                    m_SkillsOrder.Insert(index, m_ValidSkills[a]);
            }

            for (int a = 0; a < m_SkillsOrder.Count; a++)
            {
                SkillName skillName = m_SkillsOrder[a].Key;

                double value = m_SkillsOrder[a].Value;

                if (postTier5Upgrades > 0 && skillName != SkillName.Tactics)
                    value = Math.Round(value * (1 + ((double)postTier5Upgrades * UOACZSystem.UndeadPostTier5StatSkillIncrease)));                

                string skillText = skillName.ToString();

                switch (skillName)
                {
                    case SkillName.AnimalLore: skillText = "Animal Lore"; break;
                    case SkillName.AnimalTaming: skillText = "Animal Taming"; break;
                    case SkillName.ArmsLore: skillText = "Arms Lore"; break;
                    case SkillName.DetectHidden: skillText = "Detect Hidden"; break;
                    case SkillName.EvalInt: skillText = "Eval Int"; break;
                    case SkillName.ItemID: skillText = "Item Id"; break;
                    case SkillName.MagicResist: skillText = "Magic Resist"; break;
                    case SkillName.RemoveTrap: skillText = "Remove Trap"; break;
                    case SkillName.SpiritSpeak: skillText = "Spirit Speaking"; break;
                    case SkillName.TasteID: skillText = "Taste Id"; break;
                    case SkillName.Blacksmith: skillText = "Blacksmithing"; break;
                    case SkillName.Parry: skillText = "Parrying"; break;
                }

                m_DescriptionLines.Add(value.ToString() + " " + skillText);
            }
            
            for (int a = 0; a < m_DescriptionLines.Count; a++)
            {
                int textHue = UOACZSystem.yellowTextHue;

                string descriptionLine = m_DescriptionLines[a];

                if (descriptionLine.IndexOf("Strength") >= 0 || descriptionLine.IndexOf("Dexterity") >= 0 || descriptionLine.IndexOf("Intelligence") >= 0)
                    textHue = UOACZSystem.blueTextHue;                

                if (descriptionLine.IndexOf("Damage") >= 0 || descriptionLine.IndexOf("Armor") >= 0 || descriptionLine.IndexOf("Swarm Size") >= 0)
                    textHue = UOACZSystem.whiteTextHue;                

                if (descriptionLine.IndexOf("Wrestling") >= 0 || descriptionLine.IndexOf("Tactics") >= 0 || descriptionLine.IndexOf("Tracking") >= 0 ||
                    descriptionLine.IndexOf("Meditation") >= 0 || descriptionLine.IndexOf("Detect Hidden") >= 0)
                    textHue = UOACZSystem.yellowTextHue;

                AddLabel(285, skillStatsStartY, textHue, descriptionLine);

                skillStatsStartY += 20;
            }

            AddLabel(545, 15, UOACZSystem.greenTextHue, "Abilities");

            int abilityStartY = 45;

            for (int a = 0; a < upgradeDetail.m_Abilities.Count; a++)
            {
                UOACZUndeadAbilityType abilityType = upgradeDetail.m_Abilities[a];
                UOACZUndeadAbilityDetail abilityDetail = UOACZUndeadAbilities.GetAbilityDetail(abilityType);                

                double cooldownMinutes = abilityDetail.CooldownMinutes;

                AddButton(440, abilityStartY, 24012, 24012, 50 + a, GumpButtonType.Reply, 0);
                AddLabel(490, abilityStartY - 4, UOACZSystem.yellowTextHue, abilityDetail.Name);

                string adjustedCooldownText = Utility.CreateTimeRemainingString(DateTime.UtcNow, DateTime.UtcNow + TimeSpan.FromMinutes(cooldownMinutes), true, false, true, true, true);
                
                AddLabel(490, abilityStartY + 11, UOACZSystem.blueTextHue, "Cooldown: " + adjustedCooldownText);
                AddButton(488, abilityStartY + 32, 2118, 2118, 60 + a, GumpButtonType.Reply, 0);
                AddLabel(509, abilityStartY + 29, UOACZSystem.lightGreenTextHue, "Info");                

                abilityStartY += 55;
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (sender == null)
                return;

            PlayerMobile player = sender.Mobile as PlayerMobile;

            if (player == null) return;
            if (player.Deleted) return;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            bool closeGump = true;

            UOACZUndeadUpgradeType activeUpgradeType = player.m_UOACZAccountEntry.UndeadProfile.ActiveForm;
            UOACZUndeadUpgradeDetail upgradeDetail = UOACZUndeadUpgrades.GetUpgradeDetail(activeUpgradeType);

            int corruptionPoints = player.m_UOACZAccountEntry.UndeadProfile.CorruptionPoints;
            int upgradePoints = player.m_UOACZAccountEntry.UndeadProfile.UpgradePoints;
            int monsterTier = upgradeDetail.m_MonsterTier;
            int corruptionCost = UOACZSystem.UndeadMonsterShuffleCost;
            int upgradeCost = UOACZSystem.UndeadMonsterTierUpgradeCosts[monsterTier - 1];

            switch (info.ButtonID)
            {
                //Transform
                case 1:
                    if (player.Frozen && player.Region is UOACZRegion)                   
                        player.SendMessage("You are currently frozen and cannot do that.");                    

                    else
                    {
                        if (corruptionPoints >= corruptionCost)
                        {
                            player.m_UOACZAccountEntry.UndeadProfile.DyedHueMod = -1;

                            UOACZSystem.ChangeStat(player, UOACZSystem.UOACZStatType.CorruptionPoints, -1 * corruptionCost, false);

                            player.m_UOACZAccountEntry.UndeadProfile.ShufflesSpent += corruptionCost;

                            player.SendSound(0x65A);

                            UOACZUndeadUpgradeType newUpgradeType = UOACZUndeadUpgrades.GetRandomizedUpgrade(player.m_UOACZAccountEntry);
                            UOACZUndeadUpgradeDetail newUpgradeDetail = UOACZUndeadUpgrades.GetUpgradeDetail(newUpgradeType);
                            UOACZUndeadUpgrades.SetActiveForm(player.m_UOACZAccountEntry, newUpgradeType);

                            if (player.m_UOACZAccountEntry.ActiveProfile == UOACZAccountEntry.ActiveProfileType.Undead)
                            {
                                int currentHits = player.Hits;
                                int currentStam = player.Stam;
                                int currentMana = player.Mana;

                                UOACZSystem.ApplyActiveProfile(player);

                                player.Hits = currentHits;
                                player.Stam = currentStam;
                                player.Mana = currentMana;
                            }

                            UOACZSystem.RefreshAllGumps(player);
                            player.SendGump(new UndeadProfileUpgradeGump(player));

                            player.SendMessage(UOACZSystem.greenTextHue, "You transform into: " + newUpgradeDetail.m_Name + ".");

                            return;
                        }

                        else
                        {
                            player.SendMessage(UOACZSystem.yellowTextHue, "You do not have enough Corruption Points to transform.");
                            closeGump = false;
                        }
                    }
                break;

                //Increase Monster Tier Level
                case 2:
                    if (player.Frozen && player.Region is UOACZRegion)
                        player.SendMessage("You are currently frozen and cannot do that.");

                    else
                    {
                        if (monsterTier < 5)
                        {
                            if (upgradePoints >= upgradeCost)
                            {
                                player.m_UOACZAccountEntry.UndeadProfile.DyedHueMod = -1;

                                UOACZSystem.ChangeStat(player, UOACZSystem.UOACZStatType.UndeadUpgradePoints, -1 * upgradeCost, false);
                                UOACZSystem.ChangeStat(player, UOACZSystem.UOACZStatType.MonsterTier, 1, false);

                                if (player.m_UOACZAccountEntry.UndeadProfile.MonsterTier > player.m_UOACZAccountEntry.HighestMonsterTierLevel)
                                    player.m_UOACZAccountEntry.HighestMonsterTierLevel = player.m_UOACZAccountEntry.UndeadProfile.MonsterTier;

                                player.m_UOACZAccountEntry.UndeadProfile.UpgradesSpent += upgradeCost;

                                player.SendSound(0x5C8);

                                if (player.m_UOACZAccountEntry.UndeadProfile.MonsterTier == 5)
                                    AchievementSystemImpl.Instance.TickProgressMulti(player, AchievementTriggers.Trigger_UOACZMonsterTierLevel5, 1);

                                player.m_UOACZAccountEntry.UndeadProfile.AbilitiesHotbarPageNumber = 1;

                                UOACZUndeadUpgradeType newUpgradeType = UOACZUndeadUpgrades.GetRandomizedUpgrade(player.m_UOACZAccountEntry);
                                UOACZUndeadUpgradeDetail newUpgradeDetail = UOACZUndeadUpgrades.GetUpgradeDetail(newUpgradeType);
                                UOACZUndeadUpgrades.SetActiveForm(player.m_UOACZAccountEntry, newUpgradeType);

                                if (player.m_UOACZAccountEntry.ActiveProfile == UOACZAccountEntry.ActiveProfileType.Undead)
                                {
                                    int currentHits = player.Hits;
                                    int currentStam = player.Stam;
                                    int currentMana = player.Mana;

                                    UOACZSystem.ApplyActiveProfile(player);

                                    player.Hits = currentHits;
                                    player.Stam = currentStam;
                                    player.Mana = currentMana;

                                    player.Hidden = false;
                                    player.AllowedStealthSteps = 0;
                                }

                                UOACZSystem.RefreshAllGumps(player);
                                player.SendGump(new UndeadProfileUpgradeGump(player));

                                player.SendMessage(UOACZSystem.greenTextHue, "You increase your Monster Tier Level and transform into: " + newUpgradeDetail.m_Name + ".");

                                return;
                            }

                            else
                            {
                                player.SendMessage(UOACZSystem.yellowTextHue, "You do not have enough Upgrade Points to increase your Monster Tier Level.");
                                closeGump = false;
                            }
                        }

                        else
                        {
                            upgradeCost = UOACZSystem.UndeadPostTier5UpgradeCost;

                            if (upgradePoints >= upgradeCost)
                            {
                                UOACZSystem.ChangeStat(player, UOACZSystem.UOACZStatType.UndeadUpgradePoints, -1 * upgradeCost, false);

                                player.m_UOACZAccountEntry.UndeadProfile.UpgradesSpent += upgradeCost;
                                player.m_UOACZAccountEntry.UndeadProfile.PostTier5Upgrades++;

                                player.SendSound(0x5C8);

                                UOACZUndeadUpgrades.SetActiveForm(player.m_UOACZAccountEntry, player.m_UOACZAccountEntry.UndeadProfile.ActiveForm);

                                if (player.m_UOACZAccountEntry.ActiveProfile == UOACZAccountEntry.ActiveProfileType.Undead)
                                {
                                    int currentHits = player.Hits;
                                    int currentStam = player.Stam;
                                    int currentMana = player.Mana;

                                    UOACZSystem.ApplyActiveProfile(player);

                                    player.Hits = currentHits;
                                    player.Stam = currentStam;
                                    player.Mana = currentMana;
                                }

                                UOACZSystem.RefreshAllGumps(player);

                                player.SendGump(new UndeadProfileUpgradeGump(player));

                                player.SendMessage(UOACZSystem.greenTextHue, "You spend Upgrade Points to increase your Stats and Skills.");

                                return;
                            }

                            else
                            {
                                player.SendMessage(UOACZSystem.yellowTextHue, "You do not have enough Upgrade Points to increase your Stats and Skills.");
                                closeGump = false;
                            }
                        }
                    }
                break;                
            }

            if (info.ButtonID >= 50 && info.ButtonID <= 54)
            {
                closeGump = false;
            }

            if (info.ButtonID >= 60 && info.ButtonID <= 64)
            {
                int abilityIndex = info.ButtonID - 60;

                if (abilityIndex >= upgradeDetail.m_Abilities.Count)
                    return;

                UOACZUndeadAbilityType abilityType = upgradeDetail.m_Abilities[abilityIndex];
                UOACZUndeadAbilityDetail abilityDetail = UOACZUndeadAbilities.GetAbilityDetail(abilityType);

                string description = "";

                if (abilityDetail.Description != null)
                {
                    if (abilityDetail.Description.Length > 0)
                    {
                        for (int a = 0; a < abilityDetail.Description.Length; a++)
                        {
                            description += abilityDetail.Description[a];
                        }
                    }
                }

                double cooldownMinutes = abilityDetail.CooldownMinutes;
                string adjustedCooldownText = Utility.CreateTimeRemainingString(DateTime.UtcNow, DateTime.UtcNow + TimeSpan.FromMinutes(cooldownMinutes), true, false, true, true, true);

                player.SendMessage(UOACZSystem.yellowTextHue, abilityDetail.Name + ": " + description + " (Will have a cooldown of " + adjustedCooldownText + ")");

                closeGump = false;
            }

            if (!closeGump)
                player.SendGump(new UndeadProfileUpgradeGump(player));

            else
                player.SendSound(UOACZSystem.closeGumpSound);
        }
    }

    public class UndeadProfileStatsHotbarGump : Gump
    {
        UOACZAccountEntry m_AccountEntry;
        PlayerMobile player;

        public UndeadProfileStatsHotbarGump(PlayerMobile pm_From): base(10, 10)
        {
            if (pm_From == null) return;
            if (pm_From.Deleted) return;

            player = pm_From;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            int score = player.m_UOACZAccountEntry.CurrentSessionUndeadScore;
            int upgrades = player.m_UOACZAccountEntry.UndeadProfile.UpgradePoints;
            int corruption = player.m_UOACZAccountEntry.UndeadProfile.CorruptionPoints;

            int playerFollowers = player.Followers;
            int playerFollowersMax = player.m_UOACZAccountEntry.UndeadProfile.FollowersMax;

            AddButton(262, 39, 11011, 11011, 1, GumpButtonType.Reply, 0);

            string scoreText = player.m_UOACZAccountEntry.CurrentSessionUndeadScore.ToString();

            string fatigueText = "-";

            if (player.m_UOACZAccountEntry.FatigueExpiration > DateTime.UtcNow)
                fatigueText = Utility.CreateTimeRemainingString(DateTime.UtcNow, player.m_UOACZAccountEntry.FatigueExpiration, true, true, true, true, true);

            if (player.m_UOACZAccountEntry.CurrentSessionUndeadScore != player.m_UOACZAccountEntry.CurrentSessionTotalScore)
                scoreText = player.m_UOACZAccountEntry.CurrentSessionUndeadScore.ToString() + " (" + player.m_UOACZAccountEntry.CurrentSessionTotalScore.ToString() + ")";

            int startY = 40;

            AddLabel(310, startY, UOACZSystem.greenTextHue, "Swarm");
            AddLabel(378, startY, UOACZSystem.greenTextHue, playerFollowers.ToString() + "/" + playerFollowersMax.ToString());

            startY += 15;

            AddLabel(310, startY, UOACZSystem.humanityTextHue, "Fatigue");
            AddLabel(378, startY, UOACZSystem.humanityTextHue, fatigueText);

            startY += 15;
            
            AddLabel(310, startY, UOACZSystem.whiteTextHue, "Score");
            AddLabel(378, startY, UOACZSystem.whiteTextHue, scoreText);

            startY += 15;

            AddLabel(310, startY, UOACZSystem.yellowTextHue, "Upgrades");
            AddLabel(378, startY, UOACZSystem.yellowTextHue, upgrades.ToString());

            startY += 15;

            AddLabel(310, startY, UOACZSystem.greyTextHue, "Corruption");
            AddLabel(378, startY, UOACZSystem.greyTextHue, corruption.ToString());            
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (sender == null)
                return;

            bool closeGump = true;

            if (info.ButtonID == 1)
            {
                player.CloseGump(typeof(UndeadProfileGump));
                player.SendGump(new UndeadProfileGump(player));

                player.SendSound(UOACZSystem.openGumpSound);

                closeGump = false;
            }

            if (!closeGump)
                player.SendGump(new UndeadProfileStatsHotbarGump(player));
        }
    }

    public class UndeadProfileAbilitiesHotbarGump : Gump
    {
        UOACZAccountEntry m_AccountEntry;
        PlayerMobile player;

        public UndeadProfileAbilitiesHotbarGump(PlayerMobile player): base(10, 10)
        {
            if (player == null) return;
            if (player.Deleted) return;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);            

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            int abilitiesPerPage = UOACZSystem.undeadAbilitiesPerPage;
            int totalAbilities = player.m_UOACZAccountEntry.UndeadProfile.m_Abilities.Count;
            int totalAbilityPages = (int)(Math.Ceiling((double)totalAbilities / (double)abilitiesPerPage));

            if (totalAbilityPages <= 0)
                totalAbilityPages = 1;

            if (player.m_UOACZAccountEntry.UndeadProfile.AbilitiesHotbarPageNumber < 1)
                player.m_UOACZAccountEntry.UndeadProfile.AbilitiesHotbarPageNumber = 1;

            if (player.m_UOACZAccountEntry.UndeadProfile.AbilitiesHotbarPageNumber > totalAbilityPages)
                player.m_UOACZAccountEntry.UndeadProfile.AbilitiesHotbarPageNumber = totalAbilityPages;

            int abilitiesOnPage;

            if (player.m_UOACZAccountEntry.UndeadProfile.AbilitiesHotbarPageNumber < totalAbilityPages)
                abilitiesOnPage = abilitiesPerPage;

            else
                abilitiesOnPage = totalAbilities - ((totalAbilityPages - 1) * abilitiesPerPage); 

            int abilitiesStartY = 157;

            if (player.m_UOACZAccountEntry.UndeadProfile.AbilitiesHotbarPageNumber > 1)
                AddButton(23, abilitiesStartY, 9909, 9909, 1, GumpButtonType.Reply, 0); //Previous

            if (totalAbilityPages > 1)
                AddLabel(53, abilitiesStartY, UOACZSystem.blueTextHue, player.m_UOACZAccountEntry.UndeadProfile.AbilitiesHotbarPageNumber.ToString());

            if (player.m_UOACZAccountEntry.UndeadProfile.AbilitiesHotbarPageNumber < totalAbilityPages)
                AddButton(71, abilitiesStartY, 9903, 9903, 2, GumpButtonType.Reply, 0); //Next

            abilitiesStartY += 25;

            for (int a = 0; a < abilitiesOnPage; a++)
            {
                int abilityIndex = ((player.m_UOACZAccountEntry.UndeadProfile.AbilitiesHotbarPageNumber - 1) * abilitiesPerPage) + a;

                if (abilityIndex >= 0 && abilityIndex < player.m_UOACZAccountEntry.UndeadProfile.m_Abilities.Count)
                {
                    UOACZUndeadAbilityType abilityType = player.m_UOACZAccountEntry.UndeadProfile.m_Abilities[abilityIndex].m_AbilityType;
                    UOACZUndeadAbilityDetail abilityDetail = UOACZUndeadAbilities.GetAbilityDetail(abilityType);
                    UOACZUndeadAbilityEntry abilityEntry = UOACZUndeadAbilities.GetAbilityEntry(player.m_UOACZAccountEntry, abilityType);

                    string cooldownDurationText = "";

                    int cooldownTextHue = 0;

                    if (player.m_UOACZAccountEntry.ActiveProfile != UOACZAccountEntry.ActiveProfileType.Undead)
                    {
                        string adjustedCooldownText = Utility.CreateTimeRemainingString(DateTime.UtcNow, DateTime.UtcNow + TimeSpan.FromMinutes(abilityDetail.CooldownMinutes), true, false, true, true, true);

                        cooldownDurationText = "Cooldown: " + adjustedCooldownText;
                        cooldownTextHue = UOACZSystem.blueTextHue;
                    }

                    else
                    {
                        if (DateTime.UtcNow >= abilityEntry.m_NextUsageAllowed)
                        {
                            cooldownDurationText = "Ready";
                            cooldownTextHue = UOACZSystem.greenTextHue;
                        }

                        else
                        {
                            cooldownDurationText = Utility.CreateTimeRemainingString(DateTime.UtcNow, abilityEntry.m_NextUsageAllowed, true, false, true, true, true);
                            cooldownTextHue = UOACZSystem.blueTextHue;
                        }
                    }

                    AddButton(19, abilitiesStartY, 24012, 24012, 50 + a, GumpButtonType.Reply, 0);
                    AddLabel(71, abilitiesStartY - 4, UOACZSystem.yellowTextHue, abilityDetail.Name);
                    AddLabel(71, abilitiesStartY + 10, cooldownTextHue, cooldownDurationText);
                    AddButton(70, abilitiesStartY + 30, 2118, 2118, 60 + a, GumpButtonType.Reply, 0);
                    AddLabel(89, abilitiesStartY + 28, UOACZSystem.lightGreenTextHue, "Info");
                }

                abilitiesStartY += 55;
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (sender == null)
                return;

            PlayerMobile player = sender.Mobile as PlayerMobile;

            if (player == null) return;
            if (player.Deleted) return;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            int abilitiesPerPage = UOACZSystem.undeadAbilitiesPerPage;
            int totalAbilities = player.m_UOACZAccountEntry.UndeadProfile.m_Abilities.Count;
            int totalAbilityPages = (int)(Math.Ceiling((double)totalAbilities / (double)abilitiesPerPage));

            if (totalAbilities == 0)
                return;

            if (totalAbilityPages <= 0)
                totalAbilityPages = 1;
            
            int abilitiesOnPage;

            if (player.m_UOACZAccountEntry.UndeadProfile.AbilitiesHotbarPageNumber < totalAbilityPages)
                abilitiesOnPage = abilitiesPerPage;

            else
                abilitiesOnPage = totalAbilities - ((totalAbilityPages - 1) * abilitiesPerPage);

            bool closeGump = true;

            switch (info.ButtonID)
            {
                case 1:
                    //Previous Hotbar Page
                    if (player.m_UOACZAccountEntry.UndeadProfile.AbilitiesHotbarPageNumber > 1)
                    {
                        player.m_UOACZAccountEntry.UndeadProfile.AbilitiesHotbarPageNumber--;
                        closeGump = false;
                    }
                break;

                case 2:
                    //Next Hotbar Page
                    if (player.m_UOACZAccountEntry.UndeadProfile.AbilitiesHotbarPageNumber < totalAbilityPages)
                    {
                        player.m_UOACZAccountEntry.UndeadProfile.AbilitiesHotbarPageNumber++;
                        closeGump = false;
                    }
                break;
            }

            if (info.ButtonID >= 50 && info.ButtonID <= 54)
            {
                //Activate Ability
                int abilityIndex = ((player.m_UOACZAccountEntry.UndeadProfile.AbilitiesHotbarPageNumber - 1) * abilitiesPerPage) + info.ButtonID - 50;

                if (abilityIndex >= 0 && abilityIndex < player.m_UOACZAccountEntry.UndeadProfile.m_Abilities.Count)
                {
                    UOACZUndeadAbilityType abilityType = player.m_UOACZAccountEntry.UndeadProfile.m_Abilities[abilityIndex].m_AbilityType;
                    UOACZUndeadAbilities.ActivateAbility(player, abilityType);
                }

                closeGump = false;
            }

            if (info.ButtonID >= 60 && info.ButtonID <= 64)
            {
                //Display Description
                int abilityIndex = ((player.m_UOACZAccountEntry.UndeadProfile.AbilitiesHotbarPageNumber - 1) * abilitiesPerPage) + info.ButtonID - 60;

                if (abilityIndex >= 0 && abilityIndex < player.m_UOACZAccountEntry.UndeadProfile.m_Abilities.Count)
                {
                    UOACZUndeadAbilityType abilityType = player.m_UOACZAccountEntry.UndeadProfile.m_Abilities[abilityIndex].m_AbilityType;
                    UOACZUndeadAbilityDetail abilityDetail = UOACZUndeadAbilities.GetAbilityDetail(abilityType);                    

                    string description = "";

                    if (abilityDetail.Description != null)
                    {
                        if (abilityDetail.Description.Length > 0)
                        {
                            for (int a = 0; a < abilityDetail.Description.Length; a++)
                            {
                                description += abilityDetail.Description[a];
                            }
                        }
                    }

                    string adjustedCooldownText = Utility.CreateTimeRemainingString(DateTime.UtcNow, DateTime.UtcNow + TimeSpan.FromMinutes(abilityDetail.CooldownMinutes), true, false, true, true, true);

                    player.SendMessage(UOACZSystem.yellowTextHue, abilityDetail.Name + ": " + description + " (Has a cooldown of " + adjustedCooldownText + ")");
                }

                closeGump = false;
            }

            if (!closeGump)
                player.SendGump(new UndeadProfileAbilitiesHotbarGump(player));            
        }
    }
}