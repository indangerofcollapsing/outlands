using System;
using Server;
using Server.Regions;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;
using Server.Multis;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Accounting;
using Server.Achievements;

namespace Server
{
    public enum UOACZHumanUpgradeType
    {
        Armorer,
        Weaponsmith,
        Armsman,

        Skinner,
        Trapper,
        Hunter,

        Woodworker,
        Bowyer,
        Ranger,
        Marksman,

        Artificer,
        Agent,
        Engineer,

        Chef,
        Scavenger,
        Survivalist,

        Sailor,
        Gatherer,
        Adventurer,

        Thief,
        Burglar,

        Doctor,
        FieldMedic,
        Surgeon,
        Monk,

        Guardian,
        Soldier,
        Duelist,
        Sentinel,
        Berserker,

        Apprentice,

    }

    public static class UOACZHumanUpgrades
    {
        public static UOACZHumanUpgradeDetail GetUpgradeDetail(UOACZHumanUpgradeType upgrade)
        {
            UOACZHumanUpgradeDetail upgradeDetail = new UOACZHumanUpgradeDetail();

            switch (upgrade)
            {
                //Blacksmith
                case UOACZHumanUpgradeType.Armorer:
                    upgradeDetail.m_Name = "Armorer";

                    upgradeDetail.m_UpgradePointCost = 1;

                    upgradeDetail.m_Stats[StatType.Str] = 2;
                    upgradeDetail.m_Stats[StatType.Dex] = 1;
                    upgradeDetail.m_Stats[StatType.Int] = 1;

                    upgradeDetail.m_Skills[SkillName.Fencing] = 1;
                    upgradeDetail.m_Skills[SkillName.Macing] = 1;
                    upgradeDetail.m_Skills[SkillName.Swords] = 1;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 1;

                    upgradeDetail.m_Skills[SkillName.Blacksmith] = 10;
                    upgradeDetail.m_Skills[SkillName.Mining] = 10;

                    upgradeDetail.m_Abilities.Add(UOACZHumanAbilityType.Inspiration);
                    break;

                case UOACZHumanUpgradeType.Weaponsmith:
                    upgradeDetail.m_Name = "Weaponsmith";

                    upgradeDetail.m_UpgradePointCost = 1;

                    upgradeDetail.m_Stats[StatType.Str] = 2;
                    upgradeDetail.m_Stats[StatType.Dex] = 1;
                    upgradeDetail.m_Stats[StatType.Int] = 1;

                    upgradeDetail.m_Skills[SkillName.Fencing] = 2;
                    upgradeDetail.m_Skills[SkillName.Macing] = 2;
                    upgradeDetail.m_Skills[SkillName.Swords] = 2;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 1;

                    upgradeDetail.m_Skills[SkillName.Blacksmith] = 10;

                    upgradeDetail.m_Abilities.Add(UOACZHumanAbilityType.Inspiration);
                    break;

                case UOACZHumanUpgradeType.Armsman:
                    upgradeDetail.m_Name = "Armsman";

                    upgradeDetail.m_UpgradePointCost = 1;

                    upgradeDetail.m_Stats[StatType.Str] = 2;
                    upgradeDetail.m_Stats[StatType.Dex] = 1;
                    upgradeDetail.m_Stats[StatType.Int] = 1;

                    upgradeDetail.m_Skills[SkillName.Fencing] = 2;
                    upgradeDetail.m_Skills[SkillName.Macing] = 2;
                    upgradeDetail.m_Skills[SkillName.Swords] = 2;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 1;

                    upgradeDetail.m_Skills[SkillName.Blacksmith] = 5;
                    upgradeDetail.m_Skills[SkillName.ArmsLore] = 5;

                    upgradeDetail.m_Abilities.Add(UOACZHumanAbilityType.Knockback);
                    break;

                //Tailoring
                case UOACZHumanUpgradeType.Skinner:
                    upgradeDetail.m_Name = "Skinner";

                    upgradeDetail.m_UpgradePointCost = 1;

                    upgradeDetail.m_Stats[StatType.Str] = 1;
                    upgradeDetail.m_Stats[StatType.Dex] = 2;
                    upgradeDetail.m_Stats[StatType.Int] = 1;

                    upgradeDetail.m_Skills[SkillName.Fencing] = 1;
                    upgradeDetail.m_Skills[SkillName.Macing] = 1;
                    upgradeDetail.m_Skills[SkillName.Swords] = 1;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 1;

                    upgradeDetail.m_Skills[SkillName.Tailoring] = 10;
                    upgradeDetail.m_Skills[SkillName.Camping] = 10;
                    upgradeDetail.m_Skills[SkillName.Cooking] = 5;

                    upgradeDetail.m_Abilities.Add(UOACZHumanAbilityType.Inspiration);
                    break;

                case UOACZHumanUpgradeType.Trapper:
                    upgradeDetail.m_Name = "Trapper";

                    upgradeDetail.m_UpgradePointCost = 1;

                    upgradeDetail.m_Stats[StatType.Str] = 1;
                    upgradeDetail.m_Stats[StatType.Dex] = 2;
                    upgradeDetail.m_Stats[StatType.Int] = 1;

                    upgradeDetail.m_Skills[SkillName.Fencing] = 1;
                    upgradeDetail.m_Skills[SkillName.Macing] = 1;
                    upgradeDetail.m_Skills[SkillName.Swords] = 1;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 1;

                    upgradeDetail.m_Skills[SkillName.Tailoring] = 10;
                    upgradeDetail.m_Skills[SkillName.Tracking] = 10;
                    upgradeDetail.m_Skills[SkillName.RemoveTrap] = 10;

                    upgradeDetail.m_Abilities.Add(UOACZHumanAbilityType.Snare);
                    break;

                case UOACZHumanUpgradeType.Hunter:
                    upgradeDetail.m_Name = "Hunter";

                    upgradeDetail.m_UpgradePointCost = 1;

                    upgradeDetail.m_Stats[StatType.Str] = 1;
                    upgradeDetail.m_Stats[StatType.Dex] = 2;
                    upgradeDetail.m_Stats[StatType.Int] = 1;

                    upgradeDetail.m_Skills[SkillName.Fencing] = 1;
                    upgradeDetail.m_Skills[SkillName.Macing] = 1;
                    upgradeDetail.m_Skills[SkillName.Swords] = 1;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 1;

                    upgradeDetail.m_Skills[SkillName.Archery] = 2;

                    upgradeDetail.m_Skills[SkillName.Tailoring] = 5;
                    upgradeDetail.m_Skills[SkillName.Tracking] = 5;

                    upgradeDetail.m_Abilities.Add(UOACZHumanAbilityType.Hamstring);
                    break;

                //Carpentry + Fletching
                case UOACZHumanUpgradeType.Woodworker:
                    upgradeDetail.m_Name = "Woodworker";

                    upgradeDetail.m_UpgradePointCost = 1;

                    upgradeDetail.m_Stats[StatType.Str] = 2;
                    upgradeDetail.m_Stats[StatType.Dex] = 1;
                    upgradeDetail.m_Stats[StatType.Int] = 1;

                    upgradeDetail.m_Skills[SkillName.Fencing] = 1;
                    upgradeDetail.m_Skills[SkillName.Macing] = 1;
                    upgradeDetail.m_Skills[SkillName.Swords] = 1;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 1;

                    upgradeDetail.m_Skills[SkillName.Carpentry] = 10;
                    upgradeDetail.m_Skills[SkillName.Lumberjacking] = 10;
                    upgradeDetail.m_Skills[SkillName.Tinkering] = 5;

                    upgradeDetail.m_Abilities.Add(UOACZHumanAbilityType.Inspiration);
                    break;

                case UOACZHumanUpgradeType.Bowyer:
                    upgradeDetail.m_Name = "Bowyer";

                    upgradeDetail.m_UpgradePointCost = 1;

                    upgradeDetail.m_Stats[StatType.Str] = 2;
                    upgradeDetail.m_Stats[StatType.Dex] = 1;
                    upgradeDetail.m_Stats[StatType.Int] = 1;

                    upgradeDetail.m_Skills[SkillName.Fencing] = 1;
                    upgradeDetail.m_Skills[SkillName.Macing] = 1;
                    upgradeDetail.m_Skills[SkillName.Swords] = 1;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 1;

                    upgradeDetail.m_Skills[SkillName.Archery] = 1;

                    upgradeDetail.m_Skills[SkillName.Carpentry] = 10;
                    upgradeDetail.m_Skills[SkillName.Lumberjacking] = 5;

                    upgradeDetail.m_Abilities.Add(UOACZHumanAbilityType.Longshot);
                    break;

                case UOACZHumanUpgradeType.Ranger:
                    upgradeDetail.m_Name = "Ranger";

                    upgradeDetail.m_UpgradePointCost = 1;

                    upgradeDetail.m_Stats[StatType.Str] = 1;
                    upgradeDetail.m_Stats[StatType.Dex] = 1;
                    upgradeDetail.m_Stats[StatType.Int] = 2;

                    upgradeDetail.m_Skills[SkillName.Fencing] = 1;
                    upgradeDetail.m_Skills[SkillName.Macing] = 1;
                    upgradeDetail.m_Skills[SkillName.Swords] = 1;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 1;

                    upgradeDetail.m_Skills[SkillName.Archery] = 2;

                    upgradeDetail.m_Skills[SkillName.Carpentry] = 5;
                    upgradeDetail.m_Skills[SkillName.Camping] = 5;
                    upgradeDetail.m_Skills[SkillName.Tracking] = 5;

                    upgradeDetail.m_Abilities.Add(UOACZHumanAbilityType.HeartpierceArrow);
                    break;

                case UOACZHumanUpgradeType.Marksman:
                    upgradeDetail.m_Name = "Marksman";

                    upgradeDetail.m_UpgradePointCost = 1;

                    upgradeDetail.m_Stats[StatType.Str] = 1;
                    upgradeDetail.m_Stats[StatType.Dex] = 2;
                    upgradeDetail.m_Stats[StatType.Int] = 1;

                    upgradeDetail.m_Skills[SkillName.Fencing] = 1;
                    upgradeDetail.m_Skills[SkillName.Macing] = 1;
                    upgradeDetail.m_Skills[SkillName.Swords] = 1;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 1;

                    upgradeDetail.m_Skills[SkillName.Archery] = 2;

                    upgradeDetail.m_Skills[SkillName.Anatomy] = 10;
                    upgradeDetail.m_Skills[SkillName.ArmsLore] = 5;

                    upgradeDetail.m_Abilities.Add(UOACZHumanAbilityType.MarkTarget);
                    break;

                //Alchemy + Tinkering
                case UOACZHumanUpgradeType.Artificer:
                    upgradeDetail.m_Name = "Artificer";

                    upgradeDetail.m_UpgradePointCost = 1;

                    upgradeDetail.m_Stats[StatType.Str] = 1;
                    upgradeDetail.m_Stats[StatType.Dex] = 1;
                    upgradeDetail.m_Stats[StatType.Int] = 2;

                    upgradeDetail.m_Skills[SkillName.Fencing] = 1;
                    upgradeDetail.m_Skills[SkillName.Macing] = 1;
                    upgradeDetail.m_Skills[SkillName.Swords] = 1;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 1;

                    upgradeDetail.m_Skills[SkillName.Alchemy] = 10;
                    upgradeDetail.m_Skills[SkillName.Tinkering] = 10;
                    upgradeDetail.m_Skills[SkillName.Meditation] = 5;
                    upgradeDetail.m_Skills[SkillName.MagicResist] = 5;
                    upgradeDetail.m_Skills[SkillName.Healing] = 5;

                    upgradeDetail.m_Abilities.Add(UOACZHumanAbilityType.Scientist);
                    break;

                case UOACZHumanUpgradeType.Engineer:
                    upgradeDetail.m_Name = "Engineer";

                    upgradeDetail.m_UpgradePointCost = 1;

                    upgradeDetail.m_Stats[StatType.Str] = 1;
                    upgradeDetail.m_Stats[StatType.Dex] = 2;
                    upgradeDetail.m_Stats[StatType.Int] = 2;

                    upgradeDetail.m_Skills[SkillName.Fencing] = 1;
                    upgradeDetail.m_Skills[SkillName.Macing] = 1;
                    upgradeDetail.m_Skills[SkillName.Swords] = 1;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 1;

                    upgradeDetail.m_Skills[SkillName.Tinkering] = 10;
                    upgradeDetail.m_Skills[SkillName.Alchemy] = 10;
                    upgradeDetail.m_Skills[SkillName.Mining] = 5;
                    upgradeDetail.m_Skills[SkillName.RemoveTrap] = 5;
                    upgradeDetail.m_Skills[SkillName.Lockpicking] = 5;

                    upgradeDetail.m_Abilities.Add(UOACZHumanAbilityType.Technician);
                    break;

                case UOACZHumanUpgradeType.Agent:
                    upgradeDetail.m_Name = "Agent";

                    upgradeDetail.m_UpgradePointCost = 1;

                    upgradeDetail.m_Stats[StatType.Str] = 1;
                    upgradeDetail.m_Stats[StatType.Dex] = 1;
                    upgradeDetail.m_Stats[StatType.Int] = 2;

                    upgradeDetail.m_Skills[SkillName.Fencing] = 2;
                    upgradeDetail.m_Skills[SkillName.Macing] = 2;
                    upgradeDetail.m_Skills[SkillName.Swords] = 2;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 1;

                    upgradeDetail.m_Skills[SkillName.Alchemy] = 5;
                    upgradeDetail.m_Skills[SkillName.Tinkering] = 5;

                    upgradeDetail.m_Abilities.Add(UOACZHumanAbilityType.Shadowstrike);
                    break;

                //Cooking
                case UOACZHumanUpgradeType.Chef:
                    upgradeDetail.m_Name = "Chef";

                    upgradeDetail.m_UpgradePointCost = 1;

                    upgradeDetail.m_Stats[StatType.Str] = 1;
                    upgradeDetail.m_Stats[StatType.Dex] = 2;
                    upgradeDetail.m_Stats[StatType.Int] = 1;

                    upgradeDetail.m_Skills[SkillName.Fencing] = 1;
                    upgradeDetail.m_Skills[SkillName.Macing] = 1;
                    upgradeDetail.m_Skills[SkillName.Swords] = 1;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 1;

                    upgradeDetail.m_Skills[SkillName.Cooking] = 10;
                    upgradeDetail.m_Skills[SkillName.Camping] = 10;
                    upgradeDetail.m_Skills[SkillName.Fishing] = 10;
                    upgradeDetail.m_Skills[SkillName.Tinkering] = 5;

                    upgradeDetail.m_Abilities.Add(UOACZHumanAbilityType.Provider);
                    break;

                case UOACZHumanUpgradeType.Scavenger:
                    upgradeDetail.m_Name = "Scavenger";

                    upgradeDetail.m_UpgradePointCost = 1;

                    upgradeDetail.m_Stats[StatType.Str] = 1;
                    upgradeDetail.m_Stats[StatType.Dex] = 2;
                    upgradeDetail.m_Stats[StatType.Int] = 1;

                    upgradeDetail.m_Skills[SkillName.Fencing] = 1;
                    upgradeDetail.m_Skills[SkillName.Macing] = 1;
                    upgradeDetail.m_Skills[SkillName.Swords] = 1;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 1;

                    upgradeDetail.m_Skills[SkillName.Cooking] = 10;
                    upgradeDetail.m_Skills[SkillName.Camping] = 10;
                    upgradeDetail.m_Skills[SkillName.Lockpicking] = 10;
                    upgradeDetail.m_Skills[SkillName.RemoveTrap] = 10;
                    upgradeDetail.m_Skills[SkillName.Tracking] = 5;

                    upgradeDetail.m_Abilities.Add(UOACZHumanAbilityType.Searcher);
                    break;

                case UOACZHumanUpgradeType.Survivalist:
                    upgradeDetail.m_Name = "Survivalist";

                    upgradeDetail.m_UpgradePointCost = 1;

                    upgradeDetail.m_Stats[StatType.Str] = 2;
                    upgradeDetail.m_Stats[StatType.Dex] = 2;
                    upgradeDetail.m_Stats[StatType.Int] = 2;

                    upgradeDetail.m_Skills[SkillName.Fencing] = 2;
                    upgradeDetail.m_Skills[SkillName.Macing] = 2;
                    upgradeDetail.m_Skills[SkillName.Swords] = 2;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 1;

                    upgradeDetail.m_Skills[SkillName.Cooking] = 5;
                    upgradeDetail.m_Skills[SkillName.Camping] = 5;
                    upgradeDetail.m_Skills[SkillName.Lockpicking] = 5;
                    upgradeDetail.m_Skills[SkillName.RemoveTrap] = 5;

                    upgradeDetail.m_Abilities.Add(UOACZHumanAbilityType.Camel);
                    break;

                //Fishing
                case UOACZHumanUpgradeType.Gatherer:
                    upgradeDetail.m_Name = "Gatherer";

                    upgradeDetail.m_UpgradePointCost = 1;

                    upgradeDetail.m_Stats[StatType.Str] = 2;
                    upgradeDetail.m_Stats[StatType.Dex] = 1;
                    upgradeDetail.m_Stats[StatType.Int] = 1;

                    upgradeDetail.m_Skills[SkillName.Fencing] = 1;
                    upgradeDetail.m_Skills[SkillName.Macing] = 1;
                    upgradeDetail.m_Skills[SkillName.Swords] = 1;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 1;

                    upgradeDetail.m_Skills[SkillName.Fishing] = 10;
                    upgradeDetail.m_Skills[SkillName.Lumberjacking] = 10;
                    upgradeDetail.m_Skills[SkillName.Mining] = 10;
                    upgradeDetail.m_Skills[SkillName.Camping] = 10;

                    upgradeDetail.m_Abilities.Add(UOACZHumanAbilityType.SecretStash);
                    break;

                case UOACZHumanUpgradeType.Sailor:
                    upgradeDetail.m_Name = "Sailor";

                    upgradeDetail.m_UpgradePointCost = 1;

                    upgradeDetail.m_Stats[StatType.Str] = 1;
                    upgradeDetail.m_Stats[StatType.Dex] = 2;
                    upgradeDetail.m_Stats[StatType.Int] = 1;

                    upgradeDetail.m_Skills[SkillName.Fencing] = 2;
                    upgradeDetail.m_Skills[SkillName.Macing] = 2;
                    upgradeDetail.m_Skills[SkillName.Swords] = 2;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 1;

                    upgradeDetail.m_Skills[SkillName.Fishing] = 10;
                    upgradeDetail.m_Skills[SkillName.Cooking] = 5;

                    upgradeDetail.m_Abilities.Add(UOACZHumanAbilityType.Throw);
                    break;

                case UOACZHumanUpgradeType.Adventurer:
                    upgradeDetail.m_Name = "Adventurer";

                    upgradeDetail.m_UpgradePointCost = 1;

                    upgradeDetail.m_Stats[StatType.Str] = 1;
                    upgradeDetail.m_Stats[StatType.Dex] = 2;
                    upgradeDetail.m_Stats[StatType.Int] = 2;

                    upgradeDetail.m_Skills[SkillName.Fencing] = 2;
                    upgradeDetail.m_Skills[SkillName.Macing] = 2;
                    upgradeDetail.m_Skills[SkillName.Swords] = 2;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 1;

                    upgradeDetail.m_Skills[SkillName.Cooking] = 5;
                    upgradeDetail.m_Skills[SkillName.Camping] = 5;
                    upgradeDetail.m_Skills[SkillName.Fishing] = 5;
                    upgradeDetail.m_Skills[SkillName.Healing] = 5;

                    upgradeDetail.m_Abilities.Add(UOACZHumanAbilityType.Hardy);
                    break;

                //Lockpicking + Remove Trap
                case UOACZHumanUpgradeType.Thief:
                    upgradeDetail.m_Name = "Thief";

                    upgradeDetail.m_UpgradePointCost = 1;

                    upgradeDetail.m_Stats[StatType.Str] = 1;
                    upgradeDetail.m_Stats[StatType.Dex] = 2;
                    upgradeDetail.m_Stats[StatType.Int] = 1;

                    upgradeDetail.m_Skills[SkillName.Fencing] = 2;
                    upgradeDetail.m_Skills[SkillName.Macing] = 2;
                    upgradeDetail.m_Skills[SkillName.Swords] = 2;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 1;

                    upgradeDetail.m_Skills[SkillName.Lockpicking] = 10;
                    upgradeDetail.m_Skills[SkillName.RemoveTrap] = 10;
                    upgradeDetail.m_Skills[SkillName.Tinkering] = 5;

                    upgradeDetail.m_Abilities.Add(UOACZHumanAbilityType.Evasion);
                    break;

                case UOACZHumanUpgradeType.Burglar:
                    upgradeDetail.m_Name = "Burglar";

                    upgradeDetail.m_UpgradePointCost = 1;

                    upgradeDetail.m_Stats[StatType.Str] = 1;
                    upgradeDetail.m_Stats[StatType.Dex] = 2;
                    upgradeDetail.m_Stats[StatType.Int] = 2;

                    upgradeDetail.m_Skills[SkillName.Fencing] = 1;
                    upgradeDetail.m_Skills[SkillName.Macing] = 1;
                    upgradeDetail.m_Skills[SkillName.Swords] = 1;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 1;

                    upgradeDetail.m_Skills[SkillName.Lockpicking] = 10;
                    upgradeDetail.m_Skills[SkillName.RemoveTrap] = 10;
                    upgradeDetail.m_Skills[SkillName.Tinkering] = 10;
                    upgradeDetail.m_Skills[SkillName.Meditation] = 10;
                    upgradeDetail.m_Skills[SkillName.MagicResist] = 10;
                    upgradeDetail.m_Skills[SkillName.Tracking] = 10;

                    upgradeDetail.m_Abilities.Add(UOACZHumanAbilityType.Flee);
                    break;

                //Healing
                case UOACZHumanUpgradeType.Doctor:
                    upgradeDetail.m_Name = "Doctor";

                    upgradeDetail.m_UpgradePointCost = 1;

                    upgradeDetail.m_Stats[StatType.Str] = 1;
                    upgradeDetail.m_Stats[StatType.Dex] = 1;
                    upgradeDetail.m_Stats[StatType.Int] = 2;

                    upgradeDetail.m_Skills[SkillName.Fencing] = 1;
                    upgradeDetail.m_Skills[SkillName.Macing] = 1;
                    upgradeDetail.m_Skills[SkillName.Swords] = 1;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 1;

                    upgradeDetail.m_Skills[SkillName.Healing] = 10;
                    upgradeDetail.m_Skills[SkillName.Anatomy] = 10;
                    upgradeDetail.m_Skills[SkillName.MagicResist] = 10;
                    upgradeDetail.m_Skills[SkillName.Meditation] = 5;
                    upgradeDetail.m_Skills[SkillName.Alchemy] = 5;

                    upgradeDetail.m_Abilities.Add(UOACZHumanAbilityType.FirstAid);
                    break;

                case UOACZHumanUpgradeType.Surgeon:
                    upgradeDetail.m_Name = "Surgeon";

                    upgradeDetail.m_UpgradePointCost = 1;

                    upgradeDetail.m_Stats[StatType.Str] = 1;
                    upgradeDetail.m_Stats[StatType.Dex] = 1;
                    upgradeDetail.m_Stats[StatType.Int] = 2;

                    upgradeDetail.m_Skills[SkillName.Fencing] = 1;
                    upgradeDetail.m_Skills[SkillName.Macing] = 1;
                    upgradeDetail.m_Skills[SkillName.Swords] = 1;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 1;

                    upgradeDetail.m_Skills[SkillName.Healing] = 10;
                    upgradeDetail.m_Skills[SkillName.Anatomy] = 10;
                    upgradeDetail.m_Skills[SkillName.Meditation] = 10;
                    upgradeDetail.m_Skills[SkillName.MagicResist] = 5;
                    upgradeDetail.m_Skills[SkillName.Tailoring] = 5;

                    upgradeDetail.m_Abilities.Add(UOACZHumanAbilityType.SuperiorHealing);
                    break;

                case UOACZHumanUpgradeType.FieldMedic:
                    upgradeDetail.m_Name = "Field Medic";

                    upgradeDetail.m_UpgradePointCost = 1;

                    upgradeDetail.m_Stats[StatType.Str] = 1;
                    upgradeDetail.m_Stats[StatType.Dex] = 1;
                    upgradeDetail.m_Stats[StatType.Int] = 2;

                    upgradeDetail.m_Skills[SkillName.Fencing] = 2;
                    upgradeDetail.m_Skills[SkillName.Macing] = 2;
                    upgradeDetail.m_Skills[SkillName.Swords] = 2;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 1;

                    upgradeDetail.m_Skills[SkillName.Healing] = 10;
                    upgradeDetail.m_Skills[SkillName.Anatomy] = 10;

                    upgradeDetail.m_Abilities.Add(UOACZHumanAbilityType.RapidTreatment);
                    break;

                case UOACZHumanUpgradeType.Monk:
                    upgradeDetail.m_Name = "Monk";

                    upgradeDetail.m_UpgradePointCost = 1;

                    upgradeDetail.m_Stats[StatType.Str] = 2;
                    upgradeDetail.m_Stats[StatType.Dex] = 2;
                    upgradeDetail.m_Stats[StatType.Int] = 2;

                    upgradeDetail.m_Skills[SkillName.Wrestling] = 3;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 1;

                    upgradeDetail.m_Skills[SkillName.Anatomy] = 10;                    
                    upgradeDetail.m_Skills[SkillName.Meditation] = 10;
                    upgradeDetail.m_Skills[SkillName.MagicResist] = 10;
                    upgradeDetail.m_Skills[SkillName.Healing] = 5;

                    upgradeDetail.m_Abilities.Add(UOACZHumanAbilityType.IronFists);
                    break;

                //Combat
                case UOACZHumanUpgradeType.Guardian:
                    upgradeDetail.m_Name = "Guardian";

                    upgradeDetail.m_UpgradePointCost = 1;

                    upgradeDetail.m_Stats[StatType.Str] = 2;
                    upgradeDetail.m_Stats[StatType.Dex] = 1;
                    upgradeDetail.m_Stats[StatType.Int] = 1;

                    upgradeDetail.m_Skills[SkillName.Fencing] = 2;
                    upgradeDetail.m_Skills[SkillName.Macing] = 2;
                    upgradeDetail.m_Skills[SkillName.Swords] = 2;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 1;

                    upgradeDetail.m_Skills[SkillName.Parry] = 10;

                    upgradeDetail.m_Abilities.Add(UOACZHumanAbilityType.ShieldWall);
                    break;

                case UOACZHumanUpgradeType.Soldier:
                    upgradeDetail.m_Name = "Soldier";

                    upgradeDetail.m_UpgradePointCost = 1;

                    upgradeDetail.m_Stats[StatType.Str] = 2;
                    upgradeDetail.m_Stats[StatType.Dex] = 1;
                    upgradeDetail.m_Stats[StatType.Int] = 1;

                    upgradeDetail.m_Skills[SkillName.Fencing] = 2;
                    upgradeDetail.m_Skills[SkillName.Macing] = 2;
                    upgradeDetail.m_Skills[SkillName.Swords] = 2;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 1;

                    upgradeDetail.m_Skills[SkillName.Parry] = 5;
                    upgradeDetail.m_Skills[SkillName.ArmsLore] = 5;
                    upgradeDetail.m_Skills[SkillName.Camping] = 5;

                    upgradeDetail.m_Abilities.Add(UOACZHumanAbilityType.Overpower);
                    break;

                case UOACZHumanUpgradeType.Sentinel:
                    upgradeDetail.m_Name = "Sentinel";

                    upgradeDetail.m_UpgradePointCost = 1;

                    upgradeDetail.m_Stats[StatType.Str] = 1;
                    upgradeDetail.m_Stats[StatType.Dex] = 1;
                    upgradeDetail.m_Stats[StatType.Int] = 2;

                    upgradeDetail.m_Skills[SkillName.Fencing] = 2;
                    upgradeDetail.m_Skills[SkillName.Macing] = 2;
                    upgradeDetail.m_Skills[SkillName.Swords] = 2;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 1;

                    upgradeDetail.m_Skills[SkillName.Parry] = 5;
                    upgradeDetail.m_Skills[SkillName.MagicResist] = 10;
                    upgradeDetail.m_Skills[SkillName.Meditation] = 10;

                    upgradeDetail.m_Abilities.Add(UOACZHumanAbilityType.Spellbreaking);
                    break;

                case UOACZHumanUpgradeType.Duelist:
                    upgradeDetail.m_Name = "Duelist";

                    upgradeDetail.m_UpgradePointCost = 1;

                    upgradeDetail.m_Stats[StatType.Str] = 1;
                    upgradeDetail.m_Stats[StatType.Dex] = 2;
                    upgradeDetail.m_Stats[StatType.Int] = 1;

                    upgradeDetail.m_Skills[SkillName.Fencing] = 2;
                    upgradeDetail.m_Skills[SkillName.Macing] = 2;
                    upgradeDetail.m_Skills[SkillName.Swords] = 2;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 1;

                    upgradeDetail.m_Skills[SkillName.ArmsLore] = 10;
                    upgradeDetail.m_Skills[SkillName.Anatomy] = 10;

                    upgradeDetail.m_Abilities.Add(UOACZHumanAbilityType.Expertise);
                    break;

                case UOACZHumanUpgradeType.Berserker:
                    upgradeDetail.m_Name = "Berserker";

                    upgradeDetail.m_UpgradePointCost = 1;

                    upgradeDetail.m_Stats[StatType.Str] = 2;
                    upgradeDetail.m_Stats[StatType.Dex] = 1;
                    upgradeDetail.m_Stats[StatType.Int] = 1;

                    upgradeDetail.m_Skills[SkillName.Fencing] = 2;
                    upgradeDetail.m_Skills[SkillName.Macing] = 2;
                    upgradeDetail.m_Skills[SkillName.Swords] = 2;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 1;

                    upgradeDetail.m_Skills[SkillName.ArmsLore] = 10;
                    upgradeDetail.m_Skills[SkillName.MagicResist] = 10;

                    upgradeDetail.m_Abilities.Add(UOACZHumanAbilityType.Cleave);
                    break;

                //Misc
                case UOACZHumanUpgradeType.Apprentice:
                    upgradeDetail.m_Name = "Apprentice";

                    upgradeDetail.m_UpgradePointCost = 1;

                    upgradeDetail.m_Stats[StatType.Str] = 1;
                    upgradeDetail.m_Stats[StatType.Dex] = 1;
                    upgradeDetail.m_Stats[StatType.Int] = 2;

                    upgradeDetail.m_Skills[SkillName.Wrestling] = 2;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 1;

                    upgradeDetail.m_Skills[SkillName.Blacksmith] = 5;
                    upgradeDetail.m_Skills[SkillName.Tailoring] = 5;
                    upgradeDetail.m_Skills[SkillName.Alchemy] = 5;
                    upgradeDetail.m_Skills[SkillName.Carpentry] = 5;
                    upgradeDetail.m_Skills[SkillName.Cooking] = 5;
                    upgradeDetail.m_Skills[SkillName.Tinkering] = 5;

                    upgradeDetail.m_Abilities.Add(UOACZHumanAbilityType.EmergencyRepairs);
                    break;
            }

            return upgradeDetail;
        }

        public static UOACZHumanUpgradeEntry GetUpgradeEntry(UOACZAccountEntry playerEntry, UOACZHumanUpgradeType upgradeType)
        {
            if (playerEntry == null)
                return null;

            UOACZHumanUpgradeEntry upgradeEntry = null;

            foreach (UOACZHumanUpgradeEntry entry in playerEntry.HumanProfile.m_Upgrades)
            {
                if (entry.m_UpgradeType == upgradeType)
                    return entry;
            }

            return upgradeEntry;
        }

        public static void ShuffleAvailableUpgrades(UOACZAccountEntry entry)
        {
            if (entry == null)
                return;

            entry.HumanProfile.m_UpgradesAvailable.Clear();

            int upgradeCount = Enum.GetNames(typeof(UOACZHumanUpgradeType)).Length;

            if (upgradeCount == 0)
                return;

            for (int a = 0; a < UOACZSystem.HumanUpgradesAvailablePerShuffle; a++)
            {
                List<UOACZHumanUpgradeType> m_ValidUpgrades = new List<UOACZHumanUpgradeType>();

                for (int b = 0; b < upgradeCount; b++)
                {
                    UOACZHumanUpgradeType upgradeType = (UOACZHumanUpgradeType)b;
                    UOACZHumanUpgradeDetail upgradeDetail = GetUpgradeDetail(upgradeType);

                    if (entry.HumanProfile.m_UpgradesAvailable.Contains(upgradeType))
                        continue;

                    //First Upgrade Available Should Always Be One The Player Can Afford or Costs 1
                    if (a == 0)
                    {
                        if ((upgradeDetail.m_UpgradePointCost <= entry.HumanProfile.UpgradePoints) || upgradeDetail.m_UpgradePointCost == 1)
                            m_ValidUpgrades.Add(upgradeType);
                    }

                    else
                        m_ValidUpgrades.Add(upgradeType);
                }

                UOACZHumanUpgradeType selectedUpgradeType = m_ValidUpgrades[Utility.RandomMinMax(0, m_ValidUpgrades.Count - 1)];

                entry.HumanProfile.m_UpgradesAvailable.Add(selectedUpgradeType);
            }

            entry.HumanProfile.RadialSelectionNumber = 0;
        }

        public static void PurchaseUpgrade(PlayerMobile player, UOACZHumanUpgradeType upgradeType)
        {
            if (player == null)
                return;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            UOACZHumanUpgradeEntry upgradeEntry = GetUpgradeEntry(player.m_UOACZAccountEntry, upgradeType);

            if (upgradeEntry == null)
                player.m_UOACZAccountEntry.HumanProfile.m_Upgrades.Add(new UOACZHumanUpgradeEntry(upgradeType, 1));
            else
                upgradeEntry.m_TimesPurchased++;

            player.m_UOACZAccountEntry.HumanProfile.UpgradesSpent++;

            if (player.m_UOACZAccountEntry.HumanProfile.UpgradesSpent > player.m_UOACZAccountEntry.MostHumanUpgradesSpentInASession)
                player.m_UOACZAccountEntry.MostHumanUpgradesSpentInASession = player.m_UOACZAccountEntry.HumanProfile.UpgradesSpent;

            UOACZHumanUpgradeDetail upgradeDetail = GetUpgradeDetail(upgradeType);

            player.m_UOACZAccountEntry.HumanProfile.HungerPoints += upgradeDetail.m_HungerPoints;
            player.m_UOACZAccountEntry.HumanProfile.MaxHungerPoints += upgradeDetail.m_MaxHungerPoints;

            player.m_UOACZAccountEntry.HumanProfile.ThirstPoints += upgradeDetail.m_ThirstPoints;
            player.m_UOACZAccountEntry.HumanProfile.MaxThirstPoints += upgradeDetail.m_MaxThirstPoints;

            player.m_UOACZAccountEntry.HumanProfile.HumanityPoints += upgradeDetail.m_HumanityPoints;
            player.m_UOACZAccountEntry.HumanProfile.MaxHumanityPoints += upgradeDetail.m_MaxHumanityPoints;

            player.m_UOACZAccountEntry.HumanProfile.HonorPoints += upgradeDetail.m_HonorPoints;
            player.m_UOACZAccountEntry.HumanProfile.MaxHonorPoints += upgradeDetail.m_MaxHonorPoints;

            int currentHits = player.Hits;
            int currentStam = player.Stam;
            int currentMana = player.Mana;

            player.m_UOACZAccountEntry.HumanProfile.m_Stats[StatType.Str] += upgradeDetail.m_Stats[StatType.Str];
            player.m_UOACZAccountEntry.HumanProfile.m_Stats[StatType.Dex] += upgradeDetail.m_Stats[StatType.Dex];
            player.m_UOACZAccountEntry.HumanProfile.m_Stats[StatType.Int] += upgradeDetail.m_Stats[StatType.Int];

            for (int a = 0; a < player.m_UOACZAccountEntry.HumanProfile.m_Skills.Count; a++)
            {
                player.m_UOACZAccountEntry.HumanProfile.m_Skills[(SkillName)a] += upgradeDetail.m_Skills[(SkillName)a];

                SkillName skillname = (SkillName)a;

                List<SkillName> m_Skills = new List<SkillName>();

                m_Skills.Add(SkillName.Alchemy);
                m_Skills.Add(SkillName.Blacksmith);
                m_Skills.Add(SkillName.Carpentry);
                m_Skills.Add(SkillName.Cooking);
                m_Skills.Add(SkillName.Tailoring);
                m_Skills.Add(SkillName.Tinkering);

                if (m_Skills.Contains(skillname) && player.m_UOACZAccountEntry.HumanProfile.m_Skills[(SkillName)a] >= 100)
                {
                    if (player.m_UOACZAccountEntry.HumanProfile.m_GMCraftingSkills.Contains(skillname))
                    {
                        player.m_UOACZAccountEntry.HumanProfile.m_GMCraftingSkills.Add(skillname);
                        AchievementSystemImpl.Instance.TickProgressMulti(player, AchievementTriggers.Trigger_UOACZCraftingSkill, 1);
                    }
                }
            }

            for (int a = 0; a < upgradeDetail.m_Abilities.Count; a++)
            {
                UOACZHumanAbilityType abilityType = upgradeDetail.m_Abilities[a];
                UOACZHumanAbilityEntry playerAbilityEntry = UOACZHumanAbilities.GetAbilityEntry(player.m_UOACZAccountEntry, abilityType);
                UOACZHumanAbilityDetail abilityDetail = UOACZHumanAbilities.GetAbilityDetail(abilityType);

                if (playerAbilityEntry == null)
                    player.m_UOACZAccountEntry.HumanProfile.m_Abilities.Add(new UOACZHumanAbilityEntry(abilityType, 1, abilityDetail.CooldownMinutes, DateTime.UtcNow));

                else
                {
                    playerAbilityEntry.m_TimesAcquired++;

                    if (playerAbilityEntry.m_CooldownMinutes != abilityDetail.CooldownMinimumMinutes)
                        playerAbilityEntry.m_CooldownMinutes -= abilityDetail.CooldownMinutesDecreasePerTimesAcquired;
                }
            }

            UOACZHumanUpgrades.ShuffleAvailableUpgrades(player.m_UOACZAccountEntry);
            UOACZSystem.ApplyActiveProfile(player);

            player.Hits = currentHits;
            player.Stam = currentStam;
            player.Mana = currentMana;
        }
    }

    public class UOACZHumanUpgradeDetail
    {
        public string m_Name = "Upgrade Detail";

        public int m_UpgradePointCost = 1;

        public int m_HungerPoints = 0;
        public int m_MaxHungerPoints = 0;

        public int m_ThirstPoints = 0;
        public int m_MaxThirstPoints = 0;

        public int m_HumanityPoints = 0;
        public int m_MaxHumanityPoints = 0;

        public int m_HonorPoints = 0;
        public int m_MaxHonorPoints = 0;

        public string[] m_Description = new string[] { "",
                                                        };

        public Dictionary<StatType, int> m_Stats = new Dictionary<StatType, int>();
        public Dictionary<SkillName, double> m_Skills = new Dictionary<SkillName, double>();
        public List<UOACZHumanAbilityType> m_Abilities = new List<UOACZHumanAbilityType>();

        public UOACZHumanUpgradeDetail()
        {
            #region Stats and Skills

            m_Stats.Add(StatType.Str, 0);
            m_Stats.Add(StatType.Dex, 0);
            m_Stats.Add(StatType.Int, 0);

            int skillsCount = Enum.GetNames(typeof(SkillName)).Length;

            for (int a = 0; a < skillsCount; a++)
            {
                m_Skills.Add((SkillName)a, 0);
            }

            #endregion
        }
    }

    public class UOACZHumanUpgradeEntry
    {
        public UOACZHumanUpgradeType m_UpgradeType = UOACZHumanUpgradeType.Armorer;
        public int m_TimesPurchased = 0;

        public UOACZHumanUpgradeEntry(UOACZHumanUpgradeType upgradeType, int timesPurchased)
        {
            m_UpgradeType = upgradeType;
            m_TimesPurchased = timesPurchased;
        }
    }
}