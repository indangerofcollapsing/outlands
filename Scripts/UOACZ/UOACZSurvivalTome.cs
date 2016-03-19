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
    public class UOACZSurvivalTome : Item
    {
        [Constructable]
        public UOACZSurvivalTome(): base(8786)
        {
            Name = "a survival tome";
            Hue = 2599;

            Weight = 0;

            LootType = LootType.Blessed;
        }

        public UOACZSurvivalTome(Serial serial): base(serial)
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

            from.CloseGump(typeof(HumanProfileGump));
            from.SendGump(new HumanProfileGump(player));
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

    public class HumanProfileGump : Gump
    {
        public UOACZAccountEntry m_AccountEntry;
        public PlayerMobile player;

        public HumanProfileGump(PlayerMobile player): base(10, 10)
        {
            if (player == null) return;
            if (player.Deleted) return;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            if (player.m_UOACZAccountEntry == null) return;
            if (player.m_UOACZAccountEntry.HumanProfile == null) return;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddImage(205, 193, 11009);
            AddImage(204, 1, 11009);
            AddImage(3, 192, 11009);
            AddImage(3, 1, 11009);
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
            AddLabel(110, 10, 1164, "UOACZ: Human Profile");

            //Hunger
            int hunger = player.m_UOACZAccountEntry.HumanProfile.HungerPoints;
            int maxHunger = player.m_UOACZAccountEntry.HumanProfile.MaxHungerPoints;
            int missingHunger = UOACZSystem.MissingHungerStatReductionThreshold - hunger;

            string hungerPenaltyText = "No Penalty";

            if (missingHunger > 0)
            {
                double penaltyPercent = missingHunger * UOACZSystem.MissingHungerStatReduction;
                hungerPenaltyText = "-" + (penaltyPercent * 100).ToString() + "% to Stats";
            }

            int hungerTextHue = UOACZSystem.hungerTextHue;            
            int hungerPenaltyTextHue = UOACZSystem.whiteTextHue;

            //Thirst
            int thirst = player.m_UOACZAccountEntry.HumanProfile.ThirstPoints;
            int maxThirst = player.m_UOACZAccountEntry.HumanProfile.MaxThirstPoints;
            int missingThirst = UOACZSystem.MissingThirstSkillReductionThreshold - thirst;

            string thirstPenaltyText = "No Penalty";

            if (missingThirst > 0)
            {
                double penaltyPercent = missingThirst * UOACZSystem.MissingThirstSkillReduction;
                thirstPenaltyText = "-" + (penaltyPercent * 100).ToString() + "% to Skills";
            }

            int thirstTextHue = UOACZSystem.thirstTextHue;
            int thirstPenaltyTextHue = UOACZSystem.whiteTextHue;            

            //Honor
            int honor = player.m_UOACZAccountEntry.HumanProfile.HonorPoints;
            int maxHonor = player.m_UOACZAccountEntry.HumanProfile.MaxHonorPoints;

            string honorText = "No Penalty";

            if (honor <= UOACZSystem.HonorAggressionThreshold)
                honorText = "Attackable by Other Humans";

            int honorTextHue = UOACZSystem.honorTextHue;
            int honorPenaltyTextHue = UOACZSystem.whiteTextHue;

            //Fatigue
            string fatigueText = "None";

            if (player.m_UOACZAccountEntry.FatigueExpiration > DateTime.UtcNow)
                fatigueText = Utility.CreateTimeRemainingString(DateTime.UtcNow, player.m_UOACZAccountEntry.FatigueExpiration, true, true, true, true, true);

            string fatiguePenaltyText = "No Penalty";

            if (player.m_UOACZAccountEntry.FatigueExpiration > DateTime.UtcNow)
            {
                string fatiguePercentText = ((1.0 - UOACZSystem.FatigueActiveScalar) * 100).ToString() + "%";
                fatiguePenaltyText = "-" + fatiguePercentText + " Penalty to All PvP Damage";
            }

            int fatigueTextHue = UOACZSystem.humanityTextHue;
            int fatiguePenaltyTextHue = UOACZSystem.whiteTextHue;

            //Misc            
            string timeSpentAlive = Utility.CreateTimeRemainingString(DateTime.UtcNow, DateTime.UtcNow + TimeSpan.FromMinutes(player.m_UOACZAccountEntry.HumanProfile.TotalMinutesAlive), true, true, true, true, true);

            if (timeSpentAlive == "" || timeSpentAlive == "1 second")
                timeSpentAlive = "0m";
            
            int upgradesSpent = player.m_UOACZAccountEntry.HumanProfile.UpgradesSpent;

            bool enableOverheadStatsText = player.m_UOACZAccountEntry.HumanProfile.EnableOverheadStatsText;

            int survivalPoints = player.m_UOACZAccountEntry.HumanProfile.SurvivalPoints;
            int upgradePoints = player.m_UOACZAccountEntry.HumanProfile.UpgradePoints;

            switch (player.m_UOACZAccountEntry.HumanProfile.LeftPageNumber)
            {
                case 1:
                    string scoreLabel = "Human Score";
                    int scoreLabelX = 93;
                    string scoreText = player.m_UOACZAccountEntry.CurrentSessionHumanScore.ToString();

                    if (player.m_UOACZAccountEntry.CurrentSessionHumanScore != player.m_UOACZAccountEntry.CurrentSessionTotalScore)
                    {
                        scoreLabel = "Human Score (Total)";
                        scoreLabelX = 55;

                        scoreText = player.m_UOACZAccountEntry.CurrentSessionHumanScore.ToString() + " (" + player.m_UOACZAccountEntry.CurrentSessionTotalScore.ToString() + ")";
                    }

                    AddLabel(scoreLabelX, 42, UOACZSystem.whiteTextHue, scoreLabel);
                    AddItem(166, 38, 2581);
                    AddLabel(217, 42, UOACZSystem.greenTextHue, scoreText);

                    AddLabel(72, 72, UOACZSystem.whiteTextHue, "Time Spent Alive");
                    AddItem(175, 68, 6160);
                    AddLabel(217, 72, UOACZSystem.greenTextHue, timeSpentAlive);

                    AddLabel(78, 102, UOACZSystem.whiteTextHue, "Upgrades Spent");
                    AddItem(169, 99, 4029);
                    AddLabel(217, 102, UOACZSystem.greenTextHue, upgradesSpent.ToString());

                    AddButton(72, 140, 2152, 2154, 6, GumpButtonType.Reply, 0);
                    AddLabel(111, 145, UOACZSystem.whiteTextHue, "Show Stats Hotbar");

                    AddButton(72, 178, 2152, 2154, 7, GumpButtonType.Reply, 0);
                    AddLabel(112, 183, UOACZSystem.whiteTextHue, "Show Abilities Hotbar");

                    AddButton(72, 217, 2152, 2154, 8, GumpButtonType.Reply, 0);
                    AddLabel(112, 221, UOACZSystem.whiteTextHue, "Show Objectives Hotbar");
                    break;

                case 2:
                    AddLabel(113, 42, hungerTextHue, "Hunger");
                    AddItem(150, 45, 2505);
                    AddLabel(190, 42, hungerTextHue, hunger.ToString() + " / " + maxHunger.ToString());
                    AddLabel(Utility.CenteredTextOffset(175, hungerPenaltyText), 63, hungerPenaltyTextHue, hungerPenaltyText);

                    AddLabel(116, 93, thirstTextHue, "Thirst");
                    AddItem(145, 94, 2471);
                    AddLabel(190, 93, thirstTextHue, thirst.ToString() + " / " + maxThirst.ToString());
                    AddLabel(Utility.CenteredTextOffset(175, thirstPenaltyText), 113, thirstPenaltyTextHue, thirstPenaltyText);

                    AddLabel(117, 143, honorTextHue, "Honor");
                    AddItem(149, 146, 6884);
                    AddLabel(189, 143, honorTextHue, honor.ToString() + " / " + maxHonor.ToString());
                    AddLabel(Utility.CenteredTextOffset(175, honorText), 163, honorPenaltyTextHue, honorText);

                    AddLabel(108, 193, fatigueTextHue, "Fatigue");
                    AddItem(149, 186, 2598);
                    AddLabel(190, 193, fatigueTextHue, fatigueText);
                    AddLabel(Utility.CenteredTextOffset(175, fatiguePenaltyText), 213, fatiguePenaltyTextHue, fatiguePenaltyText);
                break;                
            }

            AddLabel(111, 257, UOACZSystem.whiteTextHue, "More Info");
            if (player.m_UOACZAccountEntry.HumanProfile.LeftPageNumber > 1)
                AddButton(177, 255, 4014, 4016, 5, GumpButtonType.Reply, 0); //Left

            if (player.m_UOACZAccountEntry.HumanProfile.LeftPageNumber == 1)
                AddButton(177, 255, 4005, 4007, 5, GumpButtonType.Reply, 0); //Right

            AddLabel(79, 294, UOACZSystem.lightGreenTextHue, "Survival Points Available");
            AddItem(222, 297, 571, 2655);
            AddLabel(260, 294, UOACZSystem.blueTextHue, survivalPoints.ToString());

            AddLabel(80, 319, UOACZSystem.yellowTextHue, "Upgrade Points Available");
            AddItem(216, 318, 4029);
            AddLabel(260, 320, UOACZSystem.blueTextHue, upgradePoints.ToString());                   

            //Right Page
            switch (player.m_UOACZAccountEntry.HumanProfile.ActivePage)
            {
                case UOACZAccountEntry.HumanProfileEntry.HumanActivePageType.Abilities:
                    AddLabel(408, 10, UOACZSystem.lightPurpleTextHue, "Abilities");

                    int abilitiesPerPage = UOACZSystem.humanAbilitiesPerPage;
                    int totalAbilities = player.m_UOACZAccountEntry.HumanProfile.m_Abilities.Count;
                    int totalAbilityPages = (int)(Math.Ceiling((double)totalAbilities / (double)abilitiesPerPage));

                    if (totalAbilityPages <= 0)
                        totalAbilityPages = 1;

                    int abilitiesOnPage;

                    if (player.m_UOACZAccountEntry.HumanProfile.RightPageNumber < 1)
                        player.m_UOACZAccountEntry.HumanProfile.RightPageNumber = 1;

                    if (player.m_UOACZAccountEntry.HumanProfile.RightPageNumber > totalAbilityPages)
                        player.m_UOACZAccountEntry.HumanProfile.RightPageNumber = totalAbilityPages;

                    if (player.m_UOACZAccountEntry.HumanProfile.RightPageNumber < totalAbilityPages)
                        abilitiesOnPage = abilitiesPerPage;

                    else            
                        abilitiesOnPage = totalAbilities - ((totalAbilityPages - 1) * abilitiesPerPage);            
                
                    int startY = 53;

                    for (int a = 0; a < abilitiesOnPage; a++)
                    {
                        int abilityIndex = ((player.m_UOACZAccountEntry.HumanProfile.RightPageNumber - 1) * abilitiesPerPage) + a;

                        if (abilityIndex < player.m_UOACZAccountEntry.HumanProfile.m_Abilities.Count)
                        {
                            UOACZHumanAbilityType abilityType = player.m_UOACZAccountEntry.HumanProfile.m_Abilities[abilityIndex].m_AbilityType;
                            UOACZHumanAbilityDetail abilityDetail = UOACZHumanAbilities.GetAbilityDetail(abilityType);
                            UOACZHumanAbilityEntry abilityEntry = UOACZHumanAbilities.GetAbilityEntry(player.m_UOACZAccountEntry, abilityType);

                            int abilityLevel = abilityEntry.m_TimesAcquired;
                            string cooldownDurationText = "";

                            int cooldownTextHue = 0;

                            if (player.m_UOACZAccountEntry.ActiveProfile != UOACZAccountEntry.ActiveProfileType.Human)
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

                            AddButton(320, startY, 24019, 24019, 50 + a, GumpButtonType.Reply, 0);
                            AddLabel(370, startY - 7, UOACZSystem.yellowTextHue, abilityDetail.Name);
                            AddLabel(370, startY + 11, cooldownTextHue, cooldownDurationText);
                            AddButton(370, startY + 32, 2118, 2118, 60 + a, GumpButtonType.Reply, 0);
                            AddLabel(388, startY + 29, UOACZSystem.lightGreenTextHue, "Info");
                        }
                                
                        startY += 55;
                    }

                    if (player.m_UOACZAccountEntry.HumanProfile.RightPageNumber > 1)            
                        AddButton(320, 323, 4014, 4016, 9, GumpButtonType.Reply, 0); //Left

                    if (player.m_UOACZAccountEntry.HumanProfile.RightPageNumber < totalAbilityPages)
                        AddButton(515, 323, 4005, 4007, 10, GumpButtonType.Reply, 0); //Right
                break;

                case UOACZAccountEntry.HumanProfileEntry.HumanActivePageType.StatsAndSkills:
                    List<KeyValuePair<SkillName, double>> m_ValidSkills = new List<KeyValuePair<SkillName, double>>();

                    foreach (KeyValuePair<SkillName, double> keyValuePair in player.m_UOACZAccountEntry.HumanProfile.m_Skills)
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

                    List<string> m_StatSkillName = new List<string>();
                    List<string> m_BaseValue = new List<string>();
                    List<string> m_ModdedValue = new List<string>();

                    m_StatSkillName.Add("Strength");
                    m_StatSkillName.Add("Dexterity");
                    m_StatSkillName.Add("Intelligence");
                    m_StatSkillName.Add("");

                    if (!player.m_UOACZAccountEntry.HumanProfile.m_Stats.ContainsKey(StatType.Str))
                        player.m_UOACZAccountEntry.HumanProfile.m_Stats.Add(StatType.Str, 10);

                    if (!player.m_UOACZAccountEntry.HumanProfile.m_Stats.ContainsKey(StatType.Dex))
                        player.m_UOACZAccountEntry.HumanProfile.m_Stats.Add(StatType.Dex, 10);

                    if (!player.m_UOACZAccountEntry.HumanProfile.m_Stats.ContainsKey(StatType.Int))
                        player.m_UOACZAccountEntry.HumanProfile.m_Stats.Add(StatType.Int, 10);

                    int strValue = (int)player.m_UOACZAccountEntry.HumanProfile.m_Stats[StatType.Str];
                    int dexValue = (int)player.m_UOACZAccountEntry.HumanProfile.m_Stats[StatType.Dex];
                    int intValue = (int)player.m_UOACZAccountEntry.HumanProfile.m_Stats[StatType.Int];                        

                    m_BaseValue.Add(strValue.ToString());
                    m_BaseValue.Add(dexValue.ToString());
                    m_BaseValue.Add(intValue.ToString());
                    m_BaseValue.Add("");

                    m_ModdedValue.Add(UOACZSystem.GetModdedStatValue(player, strValue).ToString());
                    m_ModdedValue.Add(UOACZSystem.GetModdedStatValue(player, dexValue).ToString());
                    m_ModdedValue.Add(UOACZSystem.GetModdedStatValue(player, intValue).ToString());
                    m_ModdedValue.Add("");
                    
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
                            case SkillName.EvalInt: skillText = "Eval Int"; break;
                            case SkillName.ItemID: skillText = "Item I"; break;
                            case SkillName.MagicResist: skillText = "Magic Resist"; break;
                            case SkillName.RemoveTrap: skillText = "Remove Trap"; break;
                            case SkillName.SpiritSpeak: skillText = "Spirit Speaking"; break;
                            case SkillName.TasteID: skillText = "Taste Id"; break;
                            case SkillName.Parry: skillText = "Parrying"; break; 
                        }

                        m_StatSkillName.Add(skillText);
                        m_BaseValue.Add(value.ToString());
                        m_ModdedValue.Add(UOACZSystem.GetModdedSkillValue(player, (int)value).ToString());
                    }

                    int textLinesPerPage = 12;
                    int totalStatSkillPages = (int)(Math.Ceiling((double)m_StatSkillName.Count / (double)textLinesPerPage));

                    if (totalStatSkillPages == 0)
                        totalStatSkillPages = 1;

                    if (player.m_UOACZAccountEntry.HumanProfile.RightPageNumber < 1)
                        player.m_UOACZAccountEntry.HumanProfile.RightPageNumber = 1;

                    if (player.m_UOACZAccountEntry.HumanProfile.RightPageNumber > totalStatSkillPages)
                        player.m_UOACZAccountEntry.HumanProfile.RightPageNumber = totalStatSkillPages;

                    AddLabel(385, 10, UOACZSystem.lightPurpleTextHue, "Stats and Skills");

                    if (player.m_UOACZAccountEntry.ActiveProfile == UOACZAccountEntry.ActiveProfileType.Human)
                    {
                        AddLabel(440, 40, UOACZSystem.whiteTextHue, "Base");
                        AddLabel(492, 40, UOACZSystem.whiteTextHue, "Modified");
                    }

                    startY = 60;

                    int startIndex = (player.m_UOACZAccountEntry.HumanProfile.RightPageNumber - 1) * textLinesPerPage;
                    int endIndex;

                    if (player.m_UOACZAccountEntry.HumanProfile.RightPageNumber < totalStatSkillPages)
                        endIndex = (player.m_UOACZAccountEntry.HumanProfile.RightPageNumber * textLinesPerPage) - 1;
                    else
                        endIndex = m_StatSkillName.Count - 1;

                    int skillNameX = 320;
                    int baseValueX = 445;
                    int moddedValueX = 508;

                    int normalX = 395;

                    for (int a = startIndex; a < endIndex + 1; a++)
                    {
                        if (player.m_UOACZAccountEntry.ActiveProfile == UOACZAccountEntry.ActiveProfileType.Human)
                        {
                            if (player.m_UOACZAccountEntry.HumanProfile.RightPageNumber == 1 && a == 0 || a == 1 || a == 2)
                            {
                                if (a < m_StatSkillName.Count && a < m_BaseValue.Count && a < m_ModdedValue.Count)
                                {
                                    string skillName = m_StatSkillName[a];
                                    string skillBaseValue = m_BaseValue[a];
                                    string skillModdedValue = m_ModdedValue[a];

                                    AddLabel(skillNameX, startY, UOACZSystem.blueTextHue, skillName);
                                    AddLabel(baseValueX, startY, UOACZSystem.whiteTextHue, skillBaseValue);

                                    if (skillBaseValue != skillModdedValue)
                                        AddLabel(moddedValueX, startY, UOACZSystem.redTextHue, skillModdedValue);
                                    else
                                        AddLabel(moddedValueX, startY, UOACZSystem.whiteTextHue, "-");
                                }
                            }

                            else if (player.m_UOACZAccountEntry.HumanProfile.RightPageNumber == 1 && a == 3)
                            {                               
                            }

                            else
                            {                                
                                int textHue = UOACZSystem.whiteTextHue;

                                if (a < m_StatSkillName.Count)
                                {
                                    string statSkillName = m_StatSkillName[a];

                                    string skillName = m_StatSkillName[a];
                                    string skillBaseValue = m_BaseValue[a];
                                    string skillModdedValue = m_ModdedValue[a];

                                    if (statSkillName.IndexOf("Fencing") >= 0 || statSkillName.IndexOf("Macing") >= 0 || statSkillName.IndexOf("Swords") >= 0 || statSkillName.IndexOf("Archery") >= 0 || statSkillName.IndexOf("Wrestling") >= 0 || statSkillName.IndexOf("Tactics") >= 0 || statSkillName.IndexOf("Arms Lore") >= 0 || statSkillName.IndexOf("Parry") >= 0 || statSkillName.IndexOf("Magic Resist") >= 0)
                                        textHue = UOACZSystem.orangeTextHue;

                                    if (statSkillName.IndexOf("Tactics") >= 0 || statSkillName.IndexOf("Arms Lore") >= 0 || statSkillName.IndexOf("Parrying") >= 0 || statSkillName.IndexOf("Magic Resist") >= 0)
                                        textHue = UOACZSystem.yellowTextHue;

                                    if (statSkillName.IndexOf("Healing") >= 0 || statSkillName.IndexOf("Anatomy") >= 0 || statSkillName.IndexOf("Meditation") >= 0 || statSkillName.IndexOf("Tracking") >= 0)
                                        textHue = UOACZSystem.whiteTextHue;

                                    if (statSkillName.IndexOf("Tinkering") >= 0 || statSkillName.IndexOf("Tailoring") >= 0 || statSkillName.IndexOf("Cooking") >= 0 || statSkillName.IndexOf("Carpentry") >= 0 || statSkillName.IndexOf("Fletching") >= 0 || statSkillName.IndexOf("Blacksmithing") >= 0 || statSkillName.IndexOf("Alchemy") >= 0)
                                        textHue = UOACZSystem.lightPurpleTextHue;

                                    if (statSkillName.IndexOf("Camping") >= 0 || statSkillName.IndexOf("Fishing") >= 0 || statSkillName.IndexOf("Lockpicking") >= 0 || statSkillName.IndexOf("Remove Trap") >= 0 || statSkillName.IndexOf("Lumberjacking") >= 0 || statSkillName.IndexOf("Mining") >= 0)
                                        textHue = UOACZSystem.lightGreenTextHue;

                                    AddLabel(skillNameX, startY, textHue, skillName);
                                    AddLabel(baseValueX, startY, UOACZSystem.whiteTextHue, skillBaseValue);

                                    if (skillBaseValue != m_ModdedValue[a])
                                        AddLabel(moddedValueX, startY, UOACZSystem.redTextHue, skillModdedValue);
                                    else
                                        AddLabel(moddedValueX, startY, UOACZSystem.whiteTextHue, "-");
                                }
                            }
                        }

                        else
                        {
                            if (a < m_StatSkillName.Count)
                            {
                                string statSkillName = m_StatSkillName[a];

                                string skillName = m_StatSkillName[a];
                                string skillBaseValue = m_BaseValue[a];

                                if (player.m_UOACZAccountEntry.HumanProfile.RightPageNumber == 1 && a == 0 || a == 1 || a == 2)
                                    AddLabel(normalX, startY, UOACZSystem.blueTextHue, skillBaseValue + " " + statSkillName);

                                else
                                    AddLabel(normalX, startY, UOACZSystem.yellowTextHue, skillBaseValue + " " + statSkillName);
                            }
                        }

                        startY += 20;
                    }

                    //Display Stats and Skills
                    if (player.m_UOACZAccountEntry.HumanProfile.RightPageNumber > 1)            
                        AddButton(320, 323, 4014, 4016, 9, GumpButtonType.Reply, 0); //Left

                    if (player.m_UOACZAccountEntry.HumanProfile.RightPageNumber < totalStatSkillPages)
                        AddButton(515, 323, 4005, 4007, 10, GumpButtonType.Reply, 0); //Right
                break;
            }

            AddLabel(60, 350, 1164, "Overview / Teams");
            AddButton(100, 375, 9721, 9724, 11, GumpButtonType.Reply, 0);

            AddLabel(190, 350, UOACZSystem.greenTextHue, "Spend Upgrades");
            AddButton(230, 375, 9721, 9724, 1, GumpButtonType.Reply, 0);     

            if (player.m_UOACZAccountEntry.HumanProfile.ActivePage == UOACZAccountEntry.HumanProfileEntry.HumanActivePageType.Abilities)
            {
                AddLabel(342, 350, UOACZSystem.blueTextHue, "Abilities");
                AddButton(352, 375, 9724, 9721, 2, GumpButtonType.Reply, 0);
            }

            else
            {
                AddLabel(342, 350, UOACZSystem.whiteTextHue, "Abilities");
                AddButton(352, 375, 9721, 9724, 2, GumpButtonType.Reply, 0);
            }

            if (player.m_UOACZAccountEntry.HumanProfile.ActivePage == UOACZAccountEntry.HumanProfileEntry.HumanActivePageType.StatsAndSkills)
            {
                AddLabel(441, 350, UOACZSystem.blueTextHue, "Stats and Skills");
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
                    player.SendGump(new HumanProfileGump(player));

                    player.CloseGump(typeof(HumanProfileUpgradeGump));
                    player.SendGump(new HumanProfileUpgradeGump(player));

                    player.SendSound(UOACZSystem.changeGumpSound);

                    return;
                break;

                case 2:
                    //Abilities Page
                    player.m_UOACZAccountEntry.HumanProfile.RightPageNumber = 1;
                    player.m_UOACZAccountEntry.HumanProfile.ActivePage = UOACZAccountEntry.HumanProfileEntry.HumanActivePageType.Abilities;

                    player.SendSound(UOACZSystem.changeGumpSound);

                    closeGump = false;                                    
                break;

                case 3:
                    //Stats and Skills Page
                    player.m_UOACZAccountEntry.HumanProfile.RightPageNumber = 1;
                    player.m_UOACZAccountEntry.HumanProfile.ActivePage = UOACZAccountEntry.HumanProfileEntry.HumanActivePageType.StatsAndSkills;

                    player.SendSound(UOACZSystem.changeGumpSound);

                    closeGump = false;                    
                break;


                case 5:
                    //Previous Left Page
                    if (player.m_UOACZAccountEntry.HumanProfile.LeftPageNumber > 1)
                        player.m_UOACZAccountEntry.HumanProfile.LeftPageNumber--;

                    //Next Left Page
                    else
                        player.m_UOACZAccountEntry.HumanProfile.LeftPageNumber++;

                    player.SendSound(UOACZSystem.changeGumpSound);

                    closeGump = false;
                break;                

                case 6:
                    //Show Stats Hotbar  
                    player.CloseGump(typeof(HumanProfileStatsHotbarGump));
                    player.SendGump(new HumanProfileStatsHotbarGump(player));

                    player.SendSound(UOACZSystem.selectionSound);

                    closeGump = false;
                break;

                case 7:
                    //Show Abilities Hotbar 
                    player.CloseGump(typeof(HumanProfileAbilitiesHotbarGump));
                    player.SendGump(new HumanProfileAbilitiesHotbarGump(player));

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
                    if (player.m_UOACZAccountEntry.HumanProfile.RightPageNumber > 1)
                    {
                        player.m_UOACZAccountEntry.HumanProfile.RightPageNumber--;
                        player.SendSound(UOACZSystem.changeGumpSound);
                    }

                    closeGump = false;
                break;

                case 10:
                    //Next Right Page
                    player.m_UOACZAccountEntry.HumanProfile.RightPageNumber++;
                    player.SendSound(UOACZSystem.changeGumpSound);

                    closeGump = false;
                break;

                case 11:
                    //Overview / Teams
                    player.SendGump(new HumanProfileGump(player));

                    player.CloseGump(typeof(UOACZOverviewGump));
                    player.SendGump(new UOACZOverviewGump(player));

                    player.SendSound(UOACZSystem.selectionSound);

                    return;
                break;  
            }

            //Abilities
            if (player.m_UOACZAccountEntry.HumanProfile.ActivePage == UOACZAccountEntry.HumanProfileEntry.HumanActivePageType.Abilities)
            {
                int abilitiesPerPage = UOACZSystem.humanAbilitiesPerPage;
                int totalAbilities = player.m_UOACZAccountEntry.HumanProfile.m_Abilities.Count;
                int totalAbilityPages = (int)(Math.Ceiling((double)totalAbilities / (double)abilitiesPerPage));

                if (totalAbilityPages <= 0)
                    totalAbilityPages = 1;

                int abilitiesOnPage;

                if (player.m_UOACZAccountEntry.HumanProfile.RightPageNumber > totalAbilityPages)
                    player.m_UOACZAccountEntry.HumanProfile.RightPageNumber = totalAbilityPages;

                if (player.m_UOACZAccountEntry.HumanProfile.RightPageNumber < totalAbilityPages)
                    abilitiesOnPage = abilitiesPerPage;

                else
                    abilitiesOnPage = totalAbilities - ((totalAbilityPages - 1) * abilitiesPerPage);

                if (info.ButtonID >= 50 && info.ButtonID <= 54)
                {
                    //Activate Ability
                    int abilityIndex = ((player.m_UOACZAccountEntry.HumanProfile.RightPageNumber - 1) * abilitiesPerPage) + info.ButtonID - 50;

                    if (abilityIndex >= 0 && abilityIndex < player.m_UOACZAccountEntry.HumanProfile.m_Abilities.Count)
                    {
                        UOACZHumanAbilityType abilityType = player.m_UOACZAccountEntry.HumanProfile.m_Abilities[abilityIndex].m_AbilityType;
                        UOACZHumanAbilities.ActivateAbility(player, abilityType);
                    }

                    closeGump = false;
                }

                if (info.ButtonID >= 60 && info.ButtonID <= 64)
                {
                    //Display Description
                    int abilityIndex = ((player.m_UOACZAccountEntry.HumanProfile.RightPageNumber - 1) * abilitiesPerPage) + info.ButtonID - 60;

                    if (abilityIndex >= 0 && abilityIndex < player.m_UOACZAccountEntry.HumanProfile.m_Abilities.Count)
                    {
                        UOACZHumanAbilityType abilityType = player.m_UOACZAccountEntry.HumanProfile.m_Abilities[abilityIndex].m_AbilityType;
                        UOACZHumanAbilityDetail abilityDetail = UOACZHumanAbilities.GetAbilityDetail(abilityType);
                        UOACZHumanAbilityEntry abilityEntry = UOACZHumanAbilities.GetAbilityEntry(player.m_UOACZAccountEntry, abilityType);

                        string description = "";

                        if (abilityDetail != null)
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
                player.SendGump(new HumanProfileGump(player));

            else
            {
                player.CloseGump(typeof(HumanProfileUpgradeGump));
                player.SendSound(UOACZSystem.closeGumpSound);
            }
        }
    }

    public class HumanProfileUpgradeGump : Gump
    {
        UOACZAccountEntry m_AccountEntry;
        PlayerMobile player;

        UOACZHumanUpgradeType m_HumanUpgradeType = UOACZHumanUpgradeType.Armorer;
        int m_UpgradeCost = 1;

        public HumanProfileUpgradeGump(PlayerMobile pm_From): base(200, 100)
        {
            if (pm_From == null) return;
            if (pm_From.Deleted) return;

            player = pm_From;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            if (player.m_UOACZAccountEntry.HumanProfile.m_UpgradesAvailable.Count < UOACZSystem.HumanUpgradesAvailablePerShuffle)
                UOACZHumanUpgrades.ShuffleAvailableUpgrades(player.m_UOACZAccountEntry);
            
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

            if (player.m_UOACZAccountEntry.HumanProfile.m_UpgradesAvailable.Count == 0)
                return;

            if (player.m_UOACZAccountEntry.HumanProfile.RadialSelectionNumber >= player.m_UOACZAccountEntry.HumanProfile.m_UpgradesAvailable.Count)
                player.m_UOACZAccountEntry.HumanProfile.RadialSelectionNumber = player.m_UOACZAccountEntry.HumanProfile.m_UpgradesAvailable.Count - 1;
            
            UOACZHumanUpgradeType selectedUpgradeType = player.m_UOACZAccountEntry.HumanProfile.m_UpgradesAvailable[player.m_UOACZAccountEntry.HumanProfile.RadialSelectionNumber];
            UOACZHumanUpgradeDetail selectedUpgradeDetail = UOACZHumanUpgrades.GetUpgradeDetail(selectedUpgradeType);
            
            int survivalPoints = player.m_UOACZAccountEntry.HumanProfile.SurvivalPoints;
            int upgradePoints = player.m_UOACZAccountEntry.HumanProfile.UpgradePoints;
            int upgradeCost = selectedUpgradeDetail.m_UpgradePointCost;

            m_HumanUpgradeType = selectedUpgradeType;
            m_UpgradeCost = upgradeCost;

            AddLabel(250, 15, UOACZSystem.greenTextHue, "Selected Upgrade");
            AddLabel(250, 35, UOACZSystem.blueTextHue, selectedUpgradeDetail.m_Name);

            int optionsStartY = 40;

            UOACZHumanUpgradeType optionUpdateType;
            UOACZHumanUpgradeDetail optionUpgradeDetail;

            for (int a = 0; a < player.m_UOACZAccountEntry.HumanProfile.m_UpgradesAvailable.Count; a++)
            {
                AddLabel(40, optionsStartY, UOACZSystem.greenTextHue, "Upgrade Option " + (a + 1).ToString());

                optionUpdateType = player.m_UOACZAccountEntry.HumanProfile.m_UpgradesAvailable[a];
                optionUpgradeDetail = UOACZHumanUpgrades.GetUpgradeDetail(optionUpdateType);

                if (player.m_UOACZAccountEntry.HumanProfile.RadialSelectionNumber == a)
                {
                    AddLabel(40, optionsStartY + 20, UOACZSystem.blueTextHue, optionUpgradeDetail.m_Name);
                    AddButton(75, optionsStartY + 45, 2154, 2152, 3 + a, GumpButtonType.Reply, 0);
                }

                else
                {
                    AddLabel(40, optionsStartY + 20, UOACZSystem.whiteTextHue, optionUpgradeDetail.m_Name);
                    AddButton(75, optionsStartY + 45, 2152, 2154, 3 + a, GumpButtonType.Reply, 0);
                }

                optionsStartY += 80;
            }

            AddLabel(26, 280, UOACZSystem.greenTextHue, "Shuffle Upgrades");
            AddLabel(26, 301, UOACZSystem.lightGreenTextHue, "Survival Cost");
            AddItem(105, 304, 571, 2655);
            AddLabel(140, 301, 2603, "1");
            AddLabel(26, 321, UOACZSystem.lightGreenTextHue, "Survival Points Available");
            AddItem(170, 324, 571, 2655);
            AddLabel(204, 321, 2603, survivalPoints.ToString());
            AddButton(75, 344, 2152, 2152, 2, GumpButtonType.Reply, 0);

            AddLabel(204, 75, UOACZSystem.greenTextHue, "Skill and Stat Increases Granted");

            int skillStatsStartY = 94;

            //Description
            List<string> m_DescriptionLines = new List<string>();

            if (selectedUpgradeDetail.m_Stats[StatType.Str] > 0)
                m_DescriptionLines.Add("+ " + selectedUpgradeDetail.m_Stats[StatType.Str].ToString() + " Strength");

            if (selectedUpgradeDetail.m_Stats[StatType.Dex] > 0)
                m_DescriptionLines.Add("+ " + selectedUpgradeDetail.m_Stats[StatType.Dex].ToString() + " Dexterity");

            if (selectedUpgradeDetail.m_Stats[StatType.Int] > 0)
                m_DescriptionLines.Add("+ " + selectedUpgradeDetail.m_Stats[StatType.Int].ToString() + " Intelligence");

            if (m_DescriptionLines.Count > 0)
                m_DescriptionLines.Add("");

            List<KeyValuePair<SkillName, double>> m_ValidSkills = new List<KeyValuePair<SkillName, double>>();

            foreach (KeyValuePair<SkillName, double> keyValuePair in selectedUpgradeDetail.m_Skills)
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
                    case SkillName.EvalInt: skillText = "Eval Int"; break;
                    case SkillName.ItemID: skillText = "Item I"; break;
                    case SkillName.MagicResist: skillText = "Magic Resist"; break;
                    case SkillName.RemoveTrap: skillText = "Remove Trap"; break;
                    case SkillName.SpiritSpeak: skillText = "Spirit Speaking"; break;
                    case SkillName.TasteID: skillText = "Taste Id"; break;
                    case SkillName.Blacksmith: skillText = "Blacksmithing"; break;
                    case SkillName.Parry: skillText = "Parrying"; break;
                }

                m_DescriptionLines.Add("+ " + value.ToString() + " " + skillText);
            }

            if (selectedUpgradeDetail.m_MaxHungerPoints > 0)
                m_DescriptionLines.Add("+ " + selectedUpgradeDetail.m_MaxHungerPoints.ToString() + " Maximum Hunger");

            if (selectedUpgradeDetail.m_MaxThirstPoints > 0)
                m_DescriptionLines.Add("+ " + selectedUpgradeDetail.m_MaxThirstPoints.ToString() + " Maximum Thirst");

            if (selectedUpgradeDetail.m_MaxHumanityPoints > 0)
                m_DescriptionLines.Add("+ " + selectedUpgradeDetail.m_MaxHumanityPoints.ToString() + " Maximum Humanity");
            
            for (int a = 0; a < m_DescriptionLines.Count; a++)
            {
                int textHue = UOACZSystem.whiteTextHue;

                string descriptionLine = "";
                
                if (m_DescriptionLines[a] != null)
                    descriptionLine = m_DescriptionLines[a];

                if (descriptionLine.IndexOf("Strength") >= 0 || descriptionLine.IndexOf("Dexterity") >= 0 || descriptionLine.IndexOf("Intelligence") >= 0)
                    textHue = UOACZSystem.blueTextHue;

                if (descriptionLine.IndexOf("Hunger") >= 0)
                    textHue = UOACZSystem.hungerTextHue;

                if (descriptionLine.IndexOf("Thirst") >= 0)
                    textHue = UOACZSystem.thirstTextHue;

                if (descriptionLine.IndexOf("Humanity") >= 0)
                    textHue = UOACZSystem.humanityTextHue;

                if (descriptionLine.IndexOf("Fencing") >= 0 || descriptionLine.IndexOf("Macing") >= 0 || descriptionLine.IndexOf("Swords") >= 0 || descriptionLine.IndexOf("Archery") >= 0 || descriptionLine.IndexOf("Wrestling") >= 0)
                    textHue = UOACZSystem.orangeTextHue;

                if (descriptionLine.IndexOf("Tactics") >= 0 || descriptionLine.IndexOf("Arms Lore") >= 0 || descriptionLine.IndexOf("Parrying") >= 0 || descriptionLine.IndexOf("Magic Resist") >= 0)
                    textHue = UOACZSystem.yellowTextHue;

                if (descriptionLine.IndexOf("Healing") >= 0 || descriptionLine.IndexOf("Anatomy") >= 0 || descriptionLine.IndexOf("Meditation") >= 0 || descriptionLine.IndexOf("Tracking") >= 0)
                    textHue = UOACZSystem.whiteTextHue;

                if (descriptionLine.IndexOf("Tinkering") >= 0 || descriptionLine.IndexOf("Tailoring") >= 0 || descriptionLine.IndexOf("Cooking") >= 0 || descriptionLine.IndexOf("Carpentry") >= 0 || descriptionLine.IndexOf("Fletching") >= 0 || descriptionLine.IndexOf("Blacksmithing") >= 0 || descriptionLine.IndexOf("Alchemy") >= 0)
                    textHue = UOACZSystem.lightPurpleTextHue;

                if (descriptionLine.IndexOf("Camping") >= 0 || descriptionLine.IndexOf("Fishing") >= 0 || descriptionLine.IndexOf("Lockpicking") >= 0 || descriptionLine.IndexOf("Remove Trap") >= 0 || descriptionLine.IndexOf("Lumberjacking") >= 0 || descriptionLine.IndexOf("Mining") >= 0)
                    textHue = UOACZSystem.lightGreenTextHue;

                AddLabel(255, skillStatsStartY, textHue, descriptionLine);

                skillStatsStartY += 20;
            }

            AddLabel(440, 75, UOACZSystem.greenTextHue, "Abilities Granted or Improved");

            int abilityStartY = 100;

            for (int a = 0; a < selectedUpgradeDetail.m_Abilities.Count; a++)
            {
                UOACZHumanAbilityType abilityType = selectedUpgradeDetail.m_Abilities[a];
                UOACZHumanAbilityDetail abilityDetail = UOACZHumanAbilities.GetAbilityDetail(abilityType);

                UOACZHumanAbilityEntry abilityEntry = UOACZHumanAbilities.GetAbilityEntry(player.m_UOACZAccountEntry, abilityType);

                int level;

                if (abilityEntry == null)
                    level = 1;
                else
                    level = abilityEntry.m_TimesAcquired + 1;

                double cooldownMinutes;

                if (abilityEntry == null)
                    cooldownMinutes = abilityDetail.CooldownMinutes;
                else
                    cooldownMinutes = abilityEntry.m_CooldownMinutes;

                double adjustedCooldown = cooldownMinutes;

                double cooldownReduction = abilityDetail.CooldownMinutesDecreasePerTimesAcquired;                
                string cooldownReductionText = Utility.CreateTimeRemainingString(DateTime.UtcNow, DateTime.UtcNow + TimeSpan.FromMinutes(cooldownReduction), true, false, true, true, true);

                bool maxCooldownReached = false;

                if (cooldownMinutes == abilityDetail.CooldownMinimumMinutes)
                    maxCooldownReached = true;

                AddButton(440, abilityStartY, 24019, 24019, 50 + a, GumpButtonType.Reply, 0);
                AddLabel(490, abilityStartY - 4, UOACZSystem.yellowTextHue, abilityDetail.Name);

                if (maxCooldownReached)                
                    AddLabel(490, abilityStartY + 12, UOACZSystem.whiteTextHue, "Level " + level.ToString() + ": Max Cooldown");                

                else
                {
                    if (level == 1)
                        AddLabel(490, abilityStartY + 12, UOACZSystem.whiteTextHue, "Level " + level.ToString());
                    else
                    {
                        adjustedCooldown -= cooldownReduction;
                        AddLabel(490, abilityStartY + 12, UOACZSystem.whiteTextHue, "Level " + level.ToString() + ": Cooldown -" + cooldownReductionText);
                    }
                }

                string adjustedCooldownText = Utility.CreateTimeRemainingString(DateTime.UtcNow, DateTime.UtcNow + TimeSpan.FromMinutes(adjustedCooldown), true, false, true, true, true);

                AddButton(488, abilityStartY + 34, 2118, 2118, 60 + a, GumpButtonType.Reply, 0);
                AddLabel(509, abilityStartY + 31, UOACZSystem.lightGreenTextHue, "Info");
                AddLabel(547, abilityStartY + 31, UOACZSystem.blueTextHue, "Cooldown: " + adjustedCooldownText);

                abilityStartY += 55;
            }

            AddButton(209, 397, 2152, 2152, 1, GumpButtonType.Reply, 0);
            AddLabel(248, 381, UOACZSystem.greenTextHue, "Purchase This Upgrade");
            AddLabel(248, 401, UOACZSystem.yellowTextHue, "Upgrade Cost");
            AddItem(323, 400, 4029);
            AddLabel(372, 401, UOACZSystem.blueTextHue, upgradeCost.ToString());
            AddLabel(248, 421, UOACZSystem.yellowTextHue, "Upgrade Points Available");
            AddItem(392, 419, 4029);
            AddLabel(435, 421, UOACZSystem.blueTextHue, upgradePoints.ToString());            
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
                    //Purchase Upgrade
                    int upgradePointsAvailable = player.m_UOACZAccountEntry.HumanProfile.UpgradePoints;
                    
                    if (upgradePointsAvailable < m_UpgradeCost)
                    {
                        player.SendMessage(UOACZSystem.greenTextHue, "You do not have enough upgrade points available to purchase this upgrade.");
                        closeGump = false;
                        break;
                    }

                    UOACZHumanUpgradeDetail upgradeDetail = UOACZHumanUpgrades.GetUpgradeDetail(m_HumanUpgradeType);

                    player.SendSound(UOACZSystem.purchaseUpgradeSound);
                    UOACZSystem.ChangeStat(player, UOACZSystem.UOACZStatType.HumanUpgradePoints, -1 * m_UpgradeCost, false);

                    UOACZHumanUpgrades.PurchaseUpgrade(player, m_HumanUpgradeType);
                    player.SendMessage(UOACZSystem.greenTextHue, "You purchase the upgrade: " + upgradeDetail.m_Name + ".");

                    if (player.m_UOACZAccountEntry.HumanProfile.ShufflesSpent == 0 && player.m_UOACZAccountEntry.HumanProfile.UpgradesSpent >= 25)
                        AchievementSystemImpl.Instance.TickProgressMulti(player, AchievementTriggers.Trigger_UOACZPurchaseUpgradesWithoutShuffles, 1);

                    List<UOACZHumanUpgradeType> m_UpgradeTypes = new List<UOACZHumanUpgradeType>();

                    foreach (UOACZHumanUpgradeEntry upgradeEntry in player.m_UOACZAccountEntry.HumanProfile.m_Upgrades)
                    {
                        UOACZHumanUpgradeType upgradeType = upgradeEntry.m_UpgradeType;

                        if (!m_UpgradeTypes.Contains(upgradeType))
                            m_UpgradeTypes.Add(upgradeType);
                    }

                    if (m_UpgradeTypes.Count >= 25)
                        AchievementSystemImpl.Instance.TickProgressMulti(player, AchievementTriggers.Trigger_UOACZPurchaseDifferentHumanUpgrades, 1);

                    UOACZSystem.RefreshAllGumps(player);
                    player.SendGump(new HumanProfileUpgradeGump(player));

                    return;
                break;

                case 2:
                    //Shuffle Upgrades
                    if (player.m_UOACZAccountEntry.HumanProfile.SurvivalPoints == 0)
                    {
                        player.SendMessage(UOACZSystem.greenTextHue, "You do not have enough survival points available to shuffle available upgrades.");
                        closeGump = false;
                        break;
                    }

                    player.SendSound(UOACZSystem.shuffleSound);

                    UOACZSystem.ChangeStat(player, UOACZSystem.UOACZStatType.SurvivalPoints, -1, false);
                    UOACZHumanUpgrades.ShuffleAvailableUpgrades(player.m_UOACZAccountEntry);

                    player.SendMessage(UOACZSystem.greenTextHue, "You shuffle the upgrades available for purchase.");

                    player.m_UOACZAccountEntry.HumanProfile.ShufflesSpent++;

                    closeGump = false;
                break;

                case 3:
                    //Upgrade Option
                    player.m_UOACZAccountEntry.HumanProfile.RadialSelectionNumber = 0;
                    player.SendSound(UOACZSystem.selectionSound);

                    closeGump = false;
                break;

                case 4:
                    //Upgrade Option
                    player.m_UOACZAccountEntry.HumanProfile.RadialSelectionNumber = 1;
                    player.SendSound(UOACZSystem.selectionSound);

                    closeGump = false;
                break;

                case 5:
                    //Upgrade Option
                    player.m_UOACZAccountEntry.HumanProfile.RadialSelectionNumber = 2;
                    player.SendSound(UOACZSystem.selectionSound);

                    closeGump = false;
                break;
            }

            if (info.ButtonID >= 50 && info.ButtonID <= 54)
            {
                closeGump = false;
            }

            if (info.ButtonID >= 60 && info.ButtonID <= 64)
            {
                int abilityIndex = info.ButtonID - 60;               

                UOACZHumanUpgradeDetail upgradeDetail = UOACZHumanUpgrades.GetUpgradeDetail(m_HumanUpgradeType);

                if (abilityIndex >= upgradeDetail.m_Abilities.Count)                
                    return;

                UOACZHumanAbilityType abilityType = upgradeDetail.m_Abilities[abilityIndex];
                UOACZHumanAbilityDetail abilityDetail = UOACZHumanAbilities.GetAbilityDetail(abilityType);

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

                UOACZHumanAbilityEntry abilityEntry = UOACZHumanAbilities.GetAbilityEntry(player.m_UOACZAccountEntry, abilityType);

                double cooldownMinutes;

                if (abilityEntry == null)
                    cooldownMinutes = abilityDetail.CooldownMinutes;
                else
                    cooldownMinutes = abilityEntry.m_CooldownMinutes;

                double adjustedCooldown = cooldownMinutes;
                double cooldownReduction = abilityDetail.CooldownMinutesDecreasePerTimesAcquired;

                bool maxCooldownReached = false;

                if (cooldownMinutes == abilityDetail.CooldownMinimumMinutes)
                    adjustedCooldown = cooldownMinutes;
                else
                {
                    if (abilityEntry == null)
                        adjustedCooldown = abilityDetail.CooldownMinutes;
                    else
                        adjustedCooldown = abilityDetail.CooldownMinutes - cooldownReduction;
                }

                string adjustedCooldownText = Utility.CreateTimeRemainingString(DateTime.UtcNow, DateTime.UtcNow + TimeSpan.FromMinutes(adjustedCooldown), true, false, true, true, true);
                
                player.SendMessage(UOACZSystem.yellowTextHue, abilityDetail.Name + ": " + description + " (Will have a cooldown of " + adjustedCooldownText + ")");

                closeGump = false;
            }
            
            if (!closeGump)
                player.SendGump(new HumanProfileUpgradeGump(player));

            else
                player.SendSound(UOACZSystem.closeGumpSound);
        }
    }

    public class HumanProfileStatsHotbarGump : Gump
    {
        UOACZAccountEntry m_AccountEntry;
        PlayerMobile player;

        public HumanProfileStatsHotbarGump(PlayerMobile pm_From): base(10, 10)
        {
            if (pm_From == null) return;
            if (pm_From.Deleted) return;

            player = pm_From;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            int hunger = player.m_UOACZAccountEntry.HumanProfile.HungerPoints;
            int maxHunger = player.m_UOACZAccountEntry.HumanProfile.MaxHungerPoints;
            int hungerTextHue = UOACZSystem.hungerTextHue;
            string hungerString = "Hunger";

            if (hunger < UOACZSystem.MissingHungerStatReductionThreshold)
                hungerString = "Hunger*";
           
            int thirst = player.m_UOACZAccountEntry.HumanProfile.ThirstPoints;
            int maxThirst = player.m_UOACZAccountEntry.HumanProfile.MaxThirstPoints;
            int thirstTextHue = UOACZSystem.thirstTextHue;
            string thirstString = "Thirst";

            if (thirst < UOACZSystem.MissingThirstSkillReductionThreshold)
                thirstString = "Thirst*";

            string fatigueText = "-";

            if (player.m_UOACZAccountEntry.FatigueExpiration > DateTime.UtcNow)
                fatigueText = Utility.CreateTimeRemainingString(DateTime.UtcNow, player.m_UOACZAccountEntry.FatigueExpiration, true, true, true, true, true);

            int fatigueTextHue = UOACZSystem.humanityTextHue;
            string fatigueString = "Fatigue";
           
            int honor = player.m_UOACZAccountEntry.HumanProfile.HonorPoints;
            int maxHonor = player.m_UOACZAccountEntry.HumanProfile.MaxHonorPoints;
            int honorTextHue = UOACZSystem.honorTextHue;
            string honorString = "Honor";

            if (honor <= UOACZSystem.HonorAggressionThreshold)
                honorString = "Honor*";
           
            int upgrades = player.m_UOACZAccountEntry.HumanProfile.UpgradePoints;
            int survival = player.m_UOACZAccountEntry.HumanProfile.SurvivalPoints;

            AddButton(15, 39, 11012, 11012, 1, GumpButtonType.Reply, 0);

            AddLabel(65, 12, UOACZSystem.hungerTextHue, hungerString);
            AddLabel(131, 12, UOACZSystem.hungerTextHue, hunger.ToString() + " / " + maxHunger.ToString());

            AddLabel(65, 27, UOACZSystem.thirstTextHue, thirstString);
            AddLabel(131, 27, UOACZSystem.thirstTextHue, thirst.ToString() + " / " + maxThirst.ToString());

            AddLabel(65, 42, UOACZSystem.honorTextHue, honorString);
            AddLabel(131, 42, UOACZSystem.honorTextHue, honor.ToString() + " / " + maxHonor.ToString());

            AddLabel(65, 57, UOACZSystem.humanityTextHue, fatigueString);
            AddLabel(131, 57, UOACZSystem.humanityTextHue, fatigueText);            

            string scoreText = player.m_UOACZAccountEntry.CurrentSessionHumanScore.ToString();

            if (player.m_UOACZAccountEntry.CurrentSessionHumanScore != player.m_UOACZAccountEntry.CurrentSessionTotalScore)
                scoreText = player.m_UOACZAccountEntry.CurrentSessionHumanScore.ToString() + " (" + player.m_UOACZAccountEntry.CurrentSessionTotalScore.ToString() + ")";

            AddLabel(65, 73, UOACZSystem.whiteTextHue, "Score");
            AddLabel(133, 73, UOACZSystem.whiteTextHue, scoreText);

            AddLabel(65, 88, UOACZSystem.yellowTextHue, "Upgrades");
            AddLabel(133, 88, UOACZSystem.yellowTextHue, upgrades.ToString());

            AddLabel(65, 103, UOACZSystem.lightGreenTextHue, "Survival");
            AddLabel(133, 103, UOACZSystem.lightGreenTextHue, survival.ToString());
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (sender == null)
                return;

            bool closeGump = true;

            if (info.ButtonID == 1)
            {
                player.CloseGump(typeof(HumanProfileGump));
                player.SendGump(new HumanProfileGump(player));

                player.SendSound(UOACZSystem.openGumpSound);

                closeGump = false;
            }

            if (!closeGump)
                player.SendGump(new HumanProfileStatsHotbarGump(player));
        }
    }

    public class HumanProfileAbilitiesHotbarGump : Gump
    {
        UOACZAccountEntry m_AccountEntry;
        PlayerMobile player;

        public HumanProfileAbilitiesHotbarGump(PlayerMobile player): base(10, 10)
        {
            if (player == null) return;
            if (player.Deleted) return;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);            

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            int abilitiesPerPage = UOACZSystem.humanAbilitiesPerPage;
            int totalAbilities = player.m_UOACZAccountEntry.HumanProfile.m_Abilities.Count;
            int totalAbilityPages = (int)(Math.Ceiling((double)totalAbilities / (double)abilitiesPerPage));

            if (totalAbilityPages <= 0)
                totalAbilityPages = 1;

            int abilitiesOnPage;

            if (player.m_UOACZAccountEntry.HumanProfile.AbilitiesHotbarPageNumber < 1)
                player.m_UOACZAccountEntry.HumanProfile.AbilitiesHotbarPageNumber = 1;
            
            if (player.m_UOACZAccountEntry.HumanProfile.AbilitiesHotbarPageNumber > totalAbilityPages)
                player.m_UOACZAccountEntry.HumanProfile.AbilitiesHotbarPageNumber = totalAbilityPages;

            if (player.m_UOACZAccountEntry.HumanProfile.AbilitiesHotbarPageNumber < totalAbilityPages)
                abilitiesOnPage = abilitiesPerPage;

            else
                abilitiesOnPage = totalAbilities - ((totalAbilityPages - 1) * abilitiesPerPage); 

            int abilitiesStartY = 157;

            if (player.m_UOACZAccountEntry.HumanProfile.AbilitiesHotbarPageNumber > 1)
                AddButton(23, abilitiesStartY, 9909, 9909, 1, GumpButtonType.Reply, 0); //Previous

            if (totalAbilityPages > 1)
                AddLabel(53, abilitiesStartY, UOACZSystem.blueTextHue, player.m_UOACZAccountEntry.HumanProfile.AbilitiesHotbarPageNumber.ToString());

            if (player.m_UOACZAccountEntry.HumanProfile.AbilitiesHotbarPageNumber < totalAbilityPages)
                AddButton(71, abilitiesStartY, 9903, 9903, 2, GumpButtonType.Reply, 0); //Next

            abilitiesStartY += 25;

            for (int a = 0; a < abilitiesOnPage; a++)
            {
                int abilityIndex = ((player.m_UOACZAccountEntry.HumanProfile.AbilitiesHotbarPageNumber - 1) * abilitiesPerPage) + a;

                if (abilityIndex >= 0 && abilityIndex < player.m_UOACZAccountEntry.HumanProfile.m_Abilities.Count)
                {
                    UOACZHumanAbilityType abilityType = player.m_UOACZAccountEntry.HumanProfile.m_Abilities[abilityIndex].m_AbilityType;
                    UOACZHumanAbilityDetail abilityDetail = UOACZHumanAbilities.GetAbilityDetail(abilityType);
                    UOACZHumanAbilityEntry abilityEntry = UOACZHumanAbilities.GetAbilityEntry(player.m_UOACZAccountEntry, abilityType);

                    string cooldownDurationText = "";

                    int cooldownTextHue = 0;

                    if (player.m_UOACZAccountEntry.ActiveProfile != UOACZAccountEntry.ActiveProfileType.Human)
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
                            cooldownDurationText = Utility.CreateTimeRemainingString(DateTime.UtcNow, abilityEntry.m_NextUsageAllowed, true, false, true, true, true);
                            cooldownTextHue = UOACZSystem.blueTextHue;
                        }
                    }

                    AddButton(19, abilitiesStartY, 24019, 24019, 50 + a, GumpButtonType.Reply, 0);
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

            int abilitiesPerPage = UOACZSystem.humanAbilitiesPerPage;
            int totalAbilities = player.m_UOACZAccountEntry.HumanProfile.m_Abilities.Count;
            int totalAbilityPages = (int)(Math.Ceiling((double)totalAbilities / (double)abilitiesPerPage));

            if (totalAbilities == 0)
                return;

            if (totalAbilityPages <= 0)
                totalAbilityPages = 1;
            
            int abilitiesOnPage;

            if (player.m_UOACZAccountEntry.HumanProfile.AbilitiesHotbarPageNumber < totalAbilityPages)
                abilitiesOnPage = abilitiesPerPage;

            else
                abilitiesOnPage = totalAbilities - ((totalAbilityPages - 1) * abilitiesPerPage);

            bool closeGump = true;

            switch (info.ButtonID)
            {
                case 1:
                    //Previous Hotbar Page
                    if (player.m_UOACZAccountEntry.HumanProfile.AbilitiesHotbarPageNumber > 1)
                    {                        
                        player.m_UOACZAccountEntry.HumanProfile.AbilitiesHotbarPageNumber--;
                        closeGump = false;
                    }
                break;

                case 2:
                    //Next Hotbar Page
                    if (player.m_UOACZAccountEntry.HumanProfile.AbilitiesHotbarPageNumber < totalAbilityPages)
                    {                       
                        player.m_UOACZAccountEntry.HumanProfile.AbilitiesHotbarPageNumber++;
                        closeGump = false;
                    }
                break;
            }

            if (info.ButtonID >= 50 && info.ButtonID <= 54)
            {
                //Activate Ability
                int abilityIndex = ((player.m_UOACZAccountEntry.HumanProfile.AbilitiesHotbarPageNumber - 1) * abilitiesPerPage) + info.ButtonID - 50;

                if (abilityIndex >= 0 && abilityIndex < player.m_UOACZAccountEntry.HumanProfile.m_Abilities.Count)
                {
                    UOACZHumanAbilityType abilityType = player.m_UOACZAccountEntry.HumanProfile.m_Abilities[abilityIndex].m_AbilityType;
                    UOACZHumanAbilities.ActivateAbility(player, abilityType);
                }

                closeGump = false;
            }

            if (info.ButtonID >= 60 && info.ButtonID <= 64)
            {
                //Display Description
                int abilityIndex = ((player.m_UOACZAccountEntry.HumanProfile.AbilitiesHotbarPageNumber - 1) * abilitiesPerPage) + info.ButtonID - 60;

                if (abilityIndex >= 0 && abilityIndex < player.m_UOACZAccountEntry.HumanProfile.m_Abilities.Count)
                {
                    UOACZHumanAbilityType abilityType = player.m_UOACZAccountEntry.HumanProfile.m_Abilities[abilityIndex].m_AbilityType;
                    UOACZHumanAbilityDetail abilityDetail = UOACZHumanAbilities.GetAbilityDetail(abilityType);
                    UOACZHumanAbilityEntry abilityEntry = UOACZHumanAbilities.GetAbilityEntry(player.m_UOACZAccountEntry, abilityType);

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

            if (!closeGump)
                player.SendGump(new HumanProfileAbilitiesHotbarGump(player));            
        }
    }    
}