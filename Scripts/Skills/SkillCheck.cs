using System;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Multis;
using Server.Spells;
using Server.Achievements;
using Server.Regions;
using Server.Network;
using Server.SkillHandlers;

namespace Server.Misc
{
    public class SkillCheck
    {
        public static void Initialize()
        {
            Mobile.SkillCheckLocationHandler = new SkillCheckLocationHandler(XmlSpawnerSkillCheck.Mobile_SkillCheckLocation);
            Mobile.SkillCheckDirectLocationHandler = new SkillCheckDirectLocationHandler(XmlSpawnerSkillCheck.Mobile_SkillCheckDirectLocation);

            Mobile.SkillCheckTargetHandler = new SkillCheckTargetHandler(XmlSpawnerSkillCheck.Mobile_SkillCheckTarget);
            Mobile.SkillCheckDirectTargetHandler = new SkillCheckDirectTargetHandler(XmlSpawnerSkillCheck.Mobile_SkillCheckDirectTarget);
        }

        public static double HousingSkillGainModifier = .25;
        public static double SkillRangeIncrement = 5.0;
        public static double StatRangeIncrement = 5.0;

        public static bool AdminShowSkillGainChance = false;

        public enum Stat 
        {   
            Str,
            Dex, 
            Int 
        }

        private static SkillName[] m_CombatSkills = new SkillName[]
		{
            SkillName.Archery, 
            SkillName.Fencing, 
            SkillName.Macing, 
            SkillName.Swords,
            SkillName.Wrestling, 
            SkillName.Tactics
        };

        public static bool IsCombatSkill(SkillName skillName)
        {
            for (int a = 0; a < m_CombatSkills.Length; a++)
            {
                if (m_CombatSkills[a] == skillName)                
                    return true;                
            }

            return false;
        }

        public static double SkillSpeedScalar(Mobile from, SkillName skillName)
        {
            //Weapon Swings
            if (IsCombatSkill(skillName))
            {
                int weaponSpeed = BaseWeapon.PlayerFistSpeed;

                if (from.Weapon != null)
                {
                    BaseWeapon weapon = from.Weapon as BaseWeapon;

                    if (!(weapon is Fists))
                        weaponSpeed = weapon.Speed;
                }
                                
                double playerSwing = 15000.0 / (int)(((double)from.Stam + 100) * weaponSpeed);
                double fastestSwingPossible = 15000.0 / (int)(((double)100 + 100) * 60);
                
                double weaponSwingScalar = playerSwing / fastestSwingPossible;

                return weaponSwingScalar;
            }

            //Other Skills
            switch (skillName)
            {
                case SkillName.Parry: return SkillCooldown.ParryCooldown; break;
                case SkillName.MagicResist: return SkillCooldown.MagicResistCooldown; break;
                case SkillName.Healing: return (SkillCooldown.HealingSelfCooldown + SkillCooldown.HealingOtherCooldown) / 2; break;
                case SkillName.Anatomy: return SkillCooldown.AnatomyCooldown; break;
                case SkillName.Veterinary: return SkillCooldown.HealingOtherCooldown; break;
                case SkillName.AnimalLore: return SkillCooldown.AnimalLoreCooldown; break;
                case SkillName.Herding: return (SkillCooldown.HerdingSuccessCooldown + SkillCooldown.HerdingFailureCooldown) / 2; break;
                case SkillName.ArmsLore: return SkillCooldown.ArmsLoreCooldown; break;
                case SkillName.Magery: return SkillCooldown.MageryCooldown; break;
                case SkillName.EvalInt: return SkillCooldown.EvalIntCooldown; break;
                case SkillName.Meditation: return SkillCooldown.MeditationValidCooldown; break;
                case SkillName.Musicianship: return SkillCooldown.MusicianshipCooldown; break;
                case SkillName.Provocation: return (SkillCooldown.ProvocationSuccessCooldown + SkillCooldown.ProvocationFailureCooldown) / 2; break;
                case SkillName.Peacemaking: return (SkillCooldown.PeacemakingSuccessCooldown + SkillCooldown.PeacemakingFailureCooldown) / 2; break;
                case SkillName.Discordance: return (SkillCooldown.DiscordanceSuccessCooldown + SkillCooldown.DiscordanceFailureCooldown) / 2; break;
                case SkillName.SpiritSpeak: return SkillCooldown.SpiritSpeakCooldown; break;
                case SkillName.Tracking: return SkillCooldown.TrackingCooldown; break;
                case SkillName.Forensics: return SkillCooldown.ForensicsCooldown; break;
                case SkillName.Hiding: return SkillCooldown.HidingCooldown; break;
                case SkillName.Stealth: return SkillCooldown.StealthCooldown; break;
                case SkillName.DetectHidden: return SkillCooldown.DetectHiddenCooldown; break;
                case SkillName.Lockpicking: return SkillCooldown.LockpickingCooldown; break;
                case SkillName.RemoveTrap: return SkillCooldown.RemoveTrapCooldown; break;
                case SkillName.AnimalTaming: return SkillCooldown.AnimalTamingCooldown; break;

                case SkillName.Blacksmith: return SkillCooldown.BlacksmithCooldown; break;
                case SkillName.Carpentry: return SkillCooldown.CarpentryCooldown; break;
                case SkillName.Tailoring: return SkillCooldown.TailoringCooldown; break;
                case SkillName.Tinkering: return SkillCooldown.TinkeringCooldown; break;
                case SkillName.Alchemy: return SkillCooldown.AlchemyCooldown; break;
                case SkillName.Cartography: return SkillCooldown.CartographyCooldown; break;
                case SkillName.Cooking: return SkillCooldown.CookingCooldown; break;
                case SkillName.Poisoning: return SkillCooldown.PoisoningCooldown; break;
                case SkillName.Begging: return (SkillCooldown.BeggingSuccessCooldown + SkillCooldown.BeggingFailureCooldown) / 2; break;
                case SkillName.Camping: return SkillCooldown.CampingCooldown; break;
                case SkillName.Fishing: return SkillCooldown.FishingCooldown; break;
                case SkillName.Inscribe: return SkillCooldown.InscribeCooldown; break;
                case SkillName.ItemID: return SkillCooldown.ItemIDCooldown; break;
                case SkillName.Lumberjacking: return SkillCooldown.LumberjackingCooldown; break;
                case SkillName.Mining: return SkillCooldown.MiningCooldown; break;
                case SkillName.Snooping: return SkillCooldown.SnoopingCooldown; break;
                case SkillName.Stealing: return SkillCooldown.StealingCooldown; break;
                case SkillName.TasteID: return SkillCooldown.TasteIDCooldown; break;
            }

            return 1.0;
        }
        
        public static double DungeonSkillScalar(SkillName skillName)
        {
            switch (skillName)
            {
                //Dungeon Boosted
                case SkillName.Archery:         return 4.0; break;
                case SkillName.Fencing:         return 4.0; break;
                case SkillName.Macing:          return 4.0; break;
                case SkillName.Swords:          return 4.0; break;
                case SkillName.Wrestling:       return 4.0; break;
                case SkillName.Tactics:         return 4.0; break;
                case SkillName.Parry:           return 4.0; break;
                case SkillName.MagicResist:     return 4.0; break;
                case SkillName.Healing:         return 4.0; break;
                case SkillName.Anatomy:         return 4.0; break;
                case SkillName.Veterinary:      return 4.0; break;
                case SkillName.AnimalLore:      return 4.0; break;
                case SkillName.Herding:         return 4.0; break;
                case SkillName.ArmsLore:        return 4.0; break;
                case SkillName.Magery:          return 4.0; break;
                case SkillName.EvalInt:         return 4.0; break;
                case SkillName.Meditation:      return 4.0; break;
                case SkillName.Musicianship:    return 4.0; break;
                case SkillName.Provocation:     return 4.0; break;
                case SkillName.Peacemaking:     return 4.0; break;
                case SkillName.Discordance:     return 4.0; break;
                case SkillName.SpiritSpeak:     return 4.0; break;
                case SkillName.Tracking:        return 4.0; break;
                case SkillName.Forensics:       return 4.0; break;
                case SkillName.Hiding:          return 4.0; break;
                case SkillName.Stealth:         return 4.0; break;
                case SkillName.DetectHidden:    return 4.0; break;
                case SkillName.Lockpicking:     return 4.0; break;
                case SkillName.RemoveTrap:      return 4.0; break;
                case SkillName.AnimalTaming:    return 4.0; break;
                
                //No Boost
                case SkillName.Blacksmith:      return 1.0; break;
                case SkillName.Carpentry:       return 1.0; break;
                case SkillName.Tailoring:       return 1.0; break;
                case SkillName.Tinkering:       return 1.0; break;
                case SkillName.Alchemy:         return 1.0; break;
                case SkillName.Cartography:     return 1.0; break;
                case SkillName.Cooking:         return 1.0; break;
                case SkillName.Poisoning:       return 1.0; break;
                case SkillName.Begging:         return 1.0; break;
                case SkillName.Camping:         return 1.0; break;
                case SkillName.Fishing:         return 1.0; break;
                case SkillName.Inscribe:        return 1.0; break;
                case SkillName.ItemID:          return 1.0; break;
                case SkillName.Lumberjacking:   return 1.0; break;
                case SkillName.Mining:          return 1.0; break;
                case SkillName.Snooping:        return 1.0; break;
                case SkillName.Stealing:        return 1.0; break;
                case SkillName.TasteID:         return 1.0; break;
            }

            return 1.0;
        }

        public static double StatGainCooldown(double statValue)
        {
            double statGainCooldown = 0; //Minutes That Must Pass Between Stat Gain Increases For Each Stat

            double[] cooldown = new double[] {          
                                                        .05, .05,    //0-5, 5-10
                                                        .1,  .15,    //10-15, 15-20
                                                        .2,  .25,    //20-25, 25-30
                                                        .3,  .35,    //30-35, 30-40
                                                        .4,  .45,    //40-45, 45-50
                                                        .5,  .55,    //50-55, 55-60
                                                        .6,  .65,    //60-65, 65-70
                                                        .7,  .75,    //70-75, 75-80
                                                        .8,  .85,    //80-85, 85-90
                                                         1,   2};    //90-95, 95-100

            //Determine Cooldown
            for (int a = 0; a < cooldown.Length; a++)
            {
                double rangeBottom = a * StatRangeIncrement;
                double rangeTop = (a * StatRangeIncrement) + StatRangeIncrement;

                if (statValue >= rangeBottom && statValue < rangeTop)
                {
                    return cooldown[a];
                    break;
                }
            }

            return statGainCooldown;
        }

        public static double SkillGainCooldown(SkillName skillName, double skillValue)
        {
            double skillGainCooldown = 0; //Minutes That Must Pass Between Skill Gain Increases For Each Skill

            double[] cooldown = new double[] {          
                                                        .05, .05,    //0-5, 5-10
                                                        .1,  .15,    //10-15, 15-20
                                                        .2,  .25,    //20-25, 25-30
                                                        .3,  .35,    //30-35, 30-40
                                                        .4,  .45,    //40-45, 45-50
                                                        .5,  .55,    //50-55, 55-60
                                                        .6,  .65,    //60-65, 65-70
                                                        .7,  .75,    //70-75, 75-80
                                                        .8,  .85,    //80-85, 85-90
                                                        1,   2,      //90-95, 95-100
                                                        3,   4,     //100-105, 105-110
                                                        6,   8};    //110-115, 115-120

            //Crafting Skill Cooldown Overrides
            bool craftingSKill = (  skillName == SkillName.Alchemy || skillName == SkillName.Blacksmith || skillName == SkillName.Carpentry ||
                                    skillName == SkillName.Cartography || skillName == SkillName.Cooking || skillName == SkillName.Inscribe ||
                                    skillName == SkillName.Poisoning || skillName == SkillName.Tailoring || skillName == SkillName.Tinkering);
           
            if (craftingSKill)
            {
                cooldown = new double[] {               
                                                        .05,  .05,   //0-5, 5-10
                                                        .1,   .1,    //10-15, 15-20
                                                        .15,  .15,   //20-25, 25-30
                                                        .2,   .2,    //30-35, 30-40
                                                        .25,  .25,   //40-45, 45-50
                                                        .3,   .3,    //50-55, 55-60
                                                        .35,  .35,   //60-65, 65-70
                                                        .4,   .4,    //70-75, 75-80
                                                        .5,   .6,    //80-85, 85-90
                                                        .7,   .8,    //90-95, 95-100
                                                        1,    1.25,  //100-105, 105-110
                                                        1.5,  2};    //110-115, 115-120
            }

            //Individual Skill Cooldown Overrides
            else
            {                
                switch (skillName)
                {
                }
            }

            //Determine Cooldown
            for (int a = 0; a < cooldown.Length; a++)
            {
                double rangeBottom = a * SkillRangeIncrement;
                double rangeTop = (a * SkillRangeIncrement) + SkillRangeIncrement;

                if (skillValue >= rangeBottom && skillValue < rangeTop)
                {
                    return cooldown[a];
                    break;
                }
            }

            return skillGainCooldown;
        }

        public static bool Mobile_SkillCheckLocation(Mobile from, SkillName skillName, double minSkill, double maxSkill, double skillGainScalar)
        {
            Skill skill = from.Skills[skillName];

            if (skill == null)
                return false;

            double value = skill.Value;

            if (value < minSkill)
                return false;

            else if (value >= maxSkill)
                return true;

            double chance = (value - minSkill) / (maxSkill - minSkill);

            return CheckSkill(from, skill, chance, skillGainScalar);
        }

        public static bool Mobile_SkillCheckDirectLocation(Mobile from, SkillName skillName, double chance, double skillGainScalar)
        {
            Skill skill = from.Skills[skillName];

            if (skill == null)
                return false;

            if (chance < 0.0)
                return false;

            else if (chance >= 1.0)
                return true;

            return CheckSkill(from, skill, chance, skillGainScalar);
        }

        public static bool Mobile_SkillCheckTarget(Mobile from, SkillName skillName, object target, double minSkill, double maxSkill, double skillGainScalar)
        {
            Skill skill = from.Skills[skillName];

            if (skill == null)
                return false;

            double value = skill.Value;

            if (value < minSkill)
                return false;

            else if (value >= maxSkill)
                return true;

            double chance = (value - minSkill) / (maxSkill - minSkill);

            return CheckSkill(from, skill, chance, skillGainScalar);
        }

        public static bool Mobile_SkillCheckDirectTarget(Mobile from, SkillName skillName, object target, double chance, double skillGainScalar)
        {
            Skill skill = from.Skills[skillName];

            if (skill == null)
                return false;

            if (chance < 0.0)
                return false;

            else if (chance >= 1.0)
                return true;

            return CheckSkill(from, skill, chance, skillGainScalar);
        }

        public static bool CheckSkill(Mobile from, Skill skill, double chance, double skillGainScalar)
        {  
            Skill mobileSkill = from.Skills[skill.SkillName];
            SkillName skillName = skill.SkillName;
            
            double skillValue = mobileSkill.Base;

            if (from.Skills.Cap == 0)
                return false;
            
            bool success = chance >= Utility.RandomDouble();

            if (from is BaseCreature)
                return success;
            
            //Check For Stat Gain            
            CheckStatGain(from, skill);

            SkillGainRange skillGainRange = m_SkillGainRanges[(int)skill.SkillName];
            
            double requiredUses = skillGainRange.m_UsesPerRange[0];

            for (int a = 0; a < skillGainRange.m_UsesPerRange.Length; a++)
            {
                double rangeBottom = a * SkillRangeIncrement;
                double rangeTop = (a * SkillRangeIncrement) + SkillRangeIncrement;

                if (skillValue >= rangeBottom && skillValue < rangeTop)
                {
                    requiredUses = skillGainRange.m_UsesPerRange[a];                    
                    break;
                }
            }

            double baseSkillGainScalar = skillGainScalar;
            double dungeonModifier = 1.0;
            double skillSpeedModifier = 1.0;
            double housingModifier = 1.0;
            double perkBonus = 1.0;
            double townBonus = 1.0;
            double craftingSquareBonus = 1.0;

            //Dungeon
            if (from.Region is DungeonRegion || from.Region is NewbieDungeonRegion)
                dungeonModifier = DungeonSkillScalar(skillName);

            //If Weapon Skill: Scale for Relative Speed
            if (IsCombatSkill(skillName))
                skillSpeedModifier = SkillSpeedScalar(from, skillName);
            
            //Housing
            BaseHouse house = BaseHouse.FindHouseAt(from.Location, from.Map, 16);

            if (house != null)
                housingModifier = HousingSkillGainModifier;            

            double finalSkillGainScalar = baseSkillGainScalar * dungeonModifier * skillSpeedModifier * housingModifier * perkBonus * townBonus * craftingSquareBonus;

            if (requiredUses == 0)
                return success;

            if (finalSkillGainScalar == 0)
                return success;           

            double skillGainChance = 1.0 / ((double)requiredUses / finalSkillGainScalar);
            int increaseAmount = (int)(Math.Ceiling(skillGainChance));

            if (increaseAmount < 1)
                increaseAmount = 1;

            if (increaseAmount > 5)
                increaseAmount = 5;

            if (AdminShowSkillGainChance && from.AccessLevel > AccessLevel.Player && from.NetState != null)            
                from.PrivateOverheadMessage(MessageType.Regular, 2550, false, skillName.ToString() + " " + skillGainChance.ToString(), from.NetState);

            if (from.Alive && Utility.RandomDouble() <= skillGainChance)
            {
                if (AllowSkillGain(from, skill))
                    GainSkill(from, skill, increaseAmount);
            }

            return success;
        }

        private static bool AllowStatGain(Mobile from, Stat stat)
        {
            if (from.Region is UOACZRegion)
                return false;
            
            switch (stat)
            {
                case Stat.Str:
                    if (from.NextStrGainAllowed > DateTime.UtcNow)
                        return false;
                break;

                case Stat.Dex:
                    if (from.NextDexGainAllowed > DateTime.UtcNow)
                        return false;
                break;

                case Stat.Int:
                    if (from.NextIntGainAllowed > DateTime.UtcNow)
                        return false;
                break;
            }

            return true;
        }

        private static bool AllowSkillGain(Mobile from, Skill skill)
        {
            if (from.NextSkillGainAllowed.ContainsKey(skill))
            {
                if (from.NextSkillGainAllowed[skill] > DateTime.UtcNow)
                    return false;
            }

            if (from.Region is UOACZRegion)
                return false;

            if (from.Region is NewbieDungeonRegion)
            {
                if (skill.Base >= NewbieDungeonRegion.SkillGainCap)
                {
                    from.SendMessage(string.Format("You have exceeded the skill cap for this area in {0}.", skill.SkillName));
                    return false;
                }
            }

            return true;
        }

        public static void CheckStatGain(Mobile from, Skill skill)
        {
            SkillInfo info = skill.Info;

            double skillSpeedScalar = SkillSpeedScalar(from, skill.SkillName);

            double strGainChance = info.StrGain * skillSpeedScalar;
            double dexGainChance = info.DexGain * skillSpeedScalar;
            double intGainChance = info.IntGain * skillSpeedScalar;
            
            if (from.StrLock == StatLockType.Up && strGainChance > Utility.RandomDouble())
                GainStat(from, Stat.Str);

            else if (from.DexLock == StatLockType.Up && dexGainChance > Utility.RandomDouble())
                GainStat(from, Stat.Dex);

            else if (from.IntLock == StatLockType.Up && intGainChance > Utility.RandomDouble())
                GainStat(from, Stat.Int);
        }     

        public static void GainSkill(Mobile from, Skill skill, int increase)
        {
            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;               

            if (from.Region.IsPartOf(typeof(Regions.Jail)))
                return;

            if (from is BaseCreature && ((BaseCreature)from).IsDeadPet)
                return;
                        
            if (skill.Base < skill.Cap && skill.Lock == SkillLock.Up)
            {
                Skills skills = from.Skills;

                if ((skills.Total / skills.Cap) >= Utility.RandomDouble())
                {
                    for (int i = 0; i < skills.Length; ++i)
                    {
                        Skill toLower = skills[i];

                        if (toLower != skill && toLower.Lock == SkillLock.Down && toLower.BaseFixedPoint >= increase)
                        {
                            toLower.BaseFixedPoint -= increase;
                            break;
                        }
                    }
                }

                if ((skills.Total + increase) <= skills.Cap)
                {
                    skill.BaseFixedPoint += increase;

                    double skillGainCooldown = SkillGainCooldown(skill.SkillName, skill.Base);

                    if (from.NextSkillGainAllowed.ContainsKey(skill))
                        from.NextSkillGainAllowed[skill] = DateTime.UtcNow + TimeSpan.FromMinutes(skillGainCooldown);

                    else
                        from.NextSkillGainAllowed.Add(skill, DateTime.UtcNow + TimeSpan.FromMinutes(skillGainCooldown));
                }
            }

            if (from.Player)
                DailyAchievement.TickProgress(Category.Newb, (PlayerMobile)from, NewbCategory.GainSkill, increase);
        }

        public static void GainStat(Mobile from, Stat stat)
        {
            if (!AllowStatGain(from, stat))
                return;

            bool atrophy = ((from.RawStatTotal / (double)from.StatCap) >= Utility.RandomDouble());

            IncreaseStat(from, stat, atrophy);
        }

        public static void IncreaseStat(Mobile from, Stat stat, bool atrophy)
        {
            atrophy = (from.RawStatTotal >= from.StatCap);// || atrophy;

            switch (stat)
            {
                case Stat.Str:
                {
                    if (atrophy)
                    {
                        if (CanLower(from, Stat.Dex) && (from.RawDex < from.RawInt || !CanLower(from, Stat.Int)))
                            --from.RawDex;

                        else if (CanLower(from, Stat.Int))
                            --from.RawInt;
                    }

                    if (CanRaise(from, Stat.Str))
                    {
                        ++from.RawStr;

                        double cooldown = StatGainCooldown((double)from.RawStr);
                        from.NextStrGainAllowed = DateTime.UtcNow + TimeSpan.FromMinutes(cooldown);
                    }

                    break;
                }

                case Stat.Dex:
                {
                    if (atrophy)
                    {
                        if (CanLower(from, Stat.Str) && (from.RawStr < from.RawInt || !CanLower(from, Stat.Int)))
                            --from.RawStr;

                        else if (CanLower(from, Stat.Int))
                            --from.RawInt;
                    }

                    if (CanRaise(from, Stat.Dex))
                    {
                        ++from.RawDex;

                        double cooldown = StatGainCooldown((double)from.RawDex);
                        from.NextDexGainAllowed = DateTime.UtcNow + TimeSpan.FromMinutes(cooldown);
                    }

                    break;
                }

                case Stat.Int:
                {
                    if (atrophy)
                    {
                        if (CanLower(from, Stat.Str) && (from.RawStr < from.RawDex || !CanLower(from, Stat.Dex)))
                            --from.RawStr;

                        else if (CanLower(from, Stat.Dex))
                            --from.RawDex;
                    }

                    if (CanRaise(from, Stat.Int))
                    {
                        ++from.RawInt;

                        double cooldown = StatGainCooldown((double)from.RawInt);
                        from.NextIntGainAllowed = DateTime.UtcNow + TimeSpan.FromMinutes(cooldown);
                    }

                    break;
                }
            }
        }  

        public static bool CanLower(Mobile from, Stat stat)
        {
            switch (stat)
            {
                case Stat.Str: return (from.StrLock == StatLockType.Down && from.RawStr > 10);
                case Stat.Dex: return (from.DexLock == StatLockType.Down && from.RawDex > 10);
                case Stat.Int: return (from.IntLock == StatLockType.Down && from.RawInt > 10);
            }

            return false;
        }

        public static bool CanRaise(Mobile from, Stat stat)
        {
            if (!(from is BaseCreature && ((BaseCreature)from).Controlled))
            {
                if (from.RawStatTotal >= from.StatCap)
                    return false;
            }

            switch (stat)
            {
                case Stat.Str: return (from.StrLock == StatLockType.Up && from.RawStr < 100);
                case Stat.Dex: return (from.DexLock == StatLockType.Up && from.RawDex < 100);
                case Stat.Int: return (from.IntLock == StatLockType.Up && from.RawInt < 100);
            }

            return false;
        }   
       
        #region SkillGainRanges

        public class SkillGainRange
        {
            public SkillName m_SkillName;
            public double[] m_UsesPerRange = new double[] { };

            public SkillGainRange(SkillName skillName, double[] usesPerRange)
            {
                m_SkillName = skillName;
                m_UsesPerRange = usesPerRange;
            }
        }

        private static SkillGainRange[] m_SkillGainRanges = new SkillGainRange[]
		{
            new SkillGainRange(SkillName.Alchemy, new double[]{     
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

            new SkillGainRange(SkillName.Anatomy, new double[]{     
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

            new SkillGainRange(SkillName.AnimalLore, new double[]{  
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

            new SkillGainRange(SkillName.ItemID, new double[]{      
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

            new SkillGainRange(SkillName.ArmsLore, new double[]{
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

            new SkillGainRange(SkillName.Parry, new double[]{  
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

            new SkillGainRange(SkillName.Begging, new double[]{  
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

            new SkillGainRange(SkillName.Blacksmith, new double[]{ 
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

            new SkillGainRange(SkillName.Bowcraft, new double[]{ 
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

            new SkillGainRange(SkillName.Peacemaking, new double[]{  
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

            new SkillGainRange(SkillName.Camping, new double[]{  
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

            new SkillGainRange(SkillName.Carpentry, new double[]{  
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

            new SkillGainRange(SkillName.Cartography, new double[]{  
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

            new SkillGainRange(SkillName.Cooking, new double[]{  
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

            new SkillGainRange(SkillName.DetectHidden, new double[]{  
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

            new SkillGainRange(SkillName.Discordance, new double[]{  
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

            new SkillGainRange(SkillName.EvalInt, new double[]{  
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

            new SkillGainRange(SkillName.Healing, new double[]{  
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

         new SkillGainRange(SkillName.Fishing, new double[]{  
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

         new SkillGainRange(SkillName.Forensics, new double[]{  
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

        new SkillGainRange(SkillName.Herding, new double[]{  
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

        new SkillGainRange(SkillName.Hiding, new double[]{  
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

        new SkillGainRange(SkillName.Provocation, new double[]{  
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

        new SkillGainRange(SkillName.Inscribe, new double[]{  
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

        new SkillGainRange(SkillName.Lockpicking, new double[]{  
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

        
        new SkillGainRange(SkillName.Magery, new double[]{          
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

        new SkillGainRange(SkillName.MagicResist, new double[]{  
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

        new SkillGainRange(SkillName.Tactics, new double[]{  
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

        new SkillGainRange(SkillName.Snooping, new double[]{  
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

        new SkillGainRange(SkillName.Musicianship, new double[]{  
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

        new SkillGainRange(SkillName.Poisoning, new double[]{  
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

        new SkillGainRange(SkillName.Archery, new double[]{  
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120 
   
        new SkillGainRange(SkillName.SpiritSpeak, new double[]{  
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

        new SkillGainRange(SkillName.Stealing, new double[]{  
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

        new SkillGainRange(SkillName.Tailoring, new double[]{  
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

         new SkillGainRange(SkillName.AnimalTaming, new double[]{  
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

        new SkillGainRange(SkillName.TasteID, new double[]{  
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

        new SkillGainRange(SkillName.Tinkering, new double[]{  
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

         new SkillGainRange(SkillName.Tracking, new double[]{  
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

        new SkillGainRange(SkillName.Veterinary, new double[]{  
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

        new SkillGainRange(SkillName.Swords, new double[]{          
                                                                    1, 1,     //0-5, 5-10
                                                                    2, 2,     //10-15, 15-20
                                                                    3, 3,     //20-25, 25-30
                                                                    4, 4,     //30-35, 30-40
                                                                    5, 5,     //40-45, 45-50
                                                                    6, 8,     //50-55, 55-60
                                                                    10, 12,     //60-65, 65-70
                                                                    14, 16,     //70-75, 75-80
                                                                    18, 20,     //80-85, 85-90
                                                                    25, 30,     //90-95, 95-100
                                                                    35, 40,     //100-105, 105-110
                                                                    45, 50}),   //110-115, 115-120

        new SkillGainRange(SkillName.Macing, new double[]{  
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

        new SkillGainRange(SkillName.Fencing, new double[]{  
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

    new SkillGainRange(SkillName.Wrestling, new double[]{  
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

    new SkillGainRange(SkillName.Lumberjacking, new double[]{  
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

    new SkillGainRange(SkillName.Mining, new double[]{  
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

    new SkillGainRange(SkillName.Meditation, new double[]{  
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

    new SkillGainRange(SkillName.Stealth, new double[]{  
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

        new SkillGainRange(SkillName.RemoveTrap, new double[]{  
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120
        };

        #endregion
    }
}