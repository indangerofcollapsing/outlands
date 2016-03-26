using System;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Multis;
using Server.Spells;
using Server.Achievements;
using Server.Regions;
using Server.Network;

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

        public static double SkillSpeedScalar(Mobile from, SkillName skillName)
        {
            //Weapon Swings
            if (skillName == SkillName.Archery || skillName == SkillName.Fencing || skillName == SkillName.Macing ||
                skillName == SkillName.Swords || skillName == SkillName.Tactics || skillName == SkillName.Wrestling)
            {
                int weaponSpeed = BaseWeapon.PlayerFistSpeed;

                if (from.Weapon != null)
                {
                    BaseWeapon weapon = from.Weapon as BaseWeapon;

                    weaponSpeed = weapon.Speed;
                }
                                
                double playerSwing = 15000.0 / (int)(((double)from.Stam + 100) * weaponSpeed);
                double fastestSwingPossible = 15000.0 / (int)(((double)100 + 100) * 60);
                
                double weaponSwingScalar = playerSwing / fastestSwingPossible;

                return weaponSwingScalar;
            }

            return 1.0;
        }
        
        public static double DungeonSkillScalar(SkillName skillName)
        {
            switch (skillName)
            {
                //Dungeon Boosted
                case SkillName.Archery:         return 2.0; break;
                case SkillName.Fencing:         return 2.0; break;
                case SkillName.Macing:          return 2.0; break;
                case SkillName.Swords:          return 2.0; break;
                case SkillName.Wrestling:       return 2.0; break;
                case SkillName.Tactics:         return 2.0; break;
                case SkillName.Parry:           return 2.0; break;
                case SkillName.MagicResist:     return 2.0; break;
                case SkillName.Healing:         return 2.0; break;
                case SkillName.Anatomy:         return 2.0; break;
                case SkillName.Veterinary:      return 2.0; break;
                case SkillName.AnimalLore:      return 2.0; break;
                case SkillName.Herding:         return 2.0; break;
                case SkillName.ArmsLore:        return 2.0; break;
                case SkillName.Magery:          return 2.0; break;
                case SkillName.EvalInt:         return 2.0; break;
                case SkillName.Meditation:      return 2.0; break;
                case SkillName.Musicianship:    return 2.0; break;
                case SkillName.Provocation:     return 2.0; break;
                case SkillName.Peacemaking:     return 2.0; break;
                case SkillName.Discordance:     return 2.0; break;
                case SkillName.SpiritSpeak:     return 2.0; break;
                case SkillName.Tracking:        return 2.0; break;
                case SkillName.Forensics:       return 2.0; break;
                case SkillName.Hiding:          return 2.0; break;
                case SkillName.Stealth:         return 2.0; break;
                case SkillName.DetectHidden:    return 2.0; break;
                case SkillName.Lockpicking:     return 2.0; break;
                case SkillName.RemoveTrap:      return 2.0; break;
                case SkillName.AnimalTaming:    return 2.0; break;
                
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

            double rangeIncrement = 5.0;

            int requiredUses = skillGainRange.m_UsesPerRange[0];

            for (int a = 0; a < skillGainRange.m_UsesPerRange.Length; a++)
            {
                double rangeBottom = a * rangeIncrement;
                double rangeTop = (a * rangeIncrement) + rangeIncrement;

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

            //Skill Speed
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
            bool allowGain = AllowGain(from, skill);

            if (from.AccessLevel > AccessLevel.Player && from.NetState != null)            
                from.PrivateOverheadMessage(MessageType.Regular, 2550, false, skillName.ToString() + " " + skillGainChance.ToString(), from.NetState);            

            if (allowGain && from.Alive && Utility.RandomDouble() <= skillGainChance)            
                Gain(from, skill);       

            return success;
        }

        public static void CheckStatGain(Mobile from, Skill skill)
        {
            SkillInfo info = skill.Info;

            double strGainChance = info.StrGain / 50;
            double dexGainChance = info.DexGain / 50;
            double intGainChance = info.IntGain / 50;
            
            if (from.StrLock == StatLockType.Up && strGainChance > Utility.RandomDouble())
                GainStat(from, Stat.Str);

            else if (from.DexLock == StatLockType.Up && dexGainChance > Utility.RandomDouble())
                GainStat(from, Stat.Dex);

            else if (from.IntLock == StatLockType.Up && intGainChance > Utility.RandomDouble())
                GainStat(from, Stat.Int);
        }
        
        private static bool AllowGain(Mobile from, Skill skill)
        {
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

        public enum Stat { Str, Dex, Int }

        public static void Gain(Mobile from, Skill skill)
        {
            PlayerMobile player = from as PlayerMobile;
            BaseCreature bc_Creature = from as BaseCreature;

            //Tamed Creature
            if (bc_Creature != null)
            {
                if (bc_Creature.Controlled && bc_Creature.ControlMaster is PlayerMobile)
                    return;
            }
           
            if (player != null)
            {
                if (player.Region is UOACZRegion)
                    return;
            }

            if (from.Region.IsPartOf(typeof(Regions.Jail)))
                return;

            if (from is BaseCreature && ((BaseCreature)from).IsDeadPet)
                return;

            int skillIncrease = 1;
            
            if (skill.Base < skill.Cap && skill.Lock == SkillLock.Up)
            {
                Skills skills = from.Skills;

                if ((skills.Total / skills.Cap) >= Utility.RandomDouble())
                {
                    for (int i = 0; i < skills.Length; ++i)
                    {
                        Skill toLower = skills[i];

                        if (toLower != skill && toLower.Lock == SkillLock.Down && toLower.BaseFixedPoint >= skillIncrease)
                        {
                            toLower.BaseFixedPoint -= skillIncrease;
                            break;
                        }
                    }
                }

                if ((skills.Total + skillIncrease) <= skills.Cap)
                    skill.BaseFixedPoint += skillIncrease;                
            }

            if (from.Player)
                DailyAchievement.TickProgress(Category.Newb, (PlayerMobile)from, NewbCategory.GainSkill, skillIncrease);
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
                        ++from.RawStr;                    

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
                        ++from.RawDex;                    

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
                        ++from.RawInt;                    

                    break;
                }
            }
        }

        private static TimeSpan GetStatDelay(double statLevel)
        {
            TimeSpan defaultTimeSpan = TimeSpan.FromMinutes(1);
            return defaultTimeSpan;
        }

        public static void GainStat(Mobile from, Stat stat)
        {
            switch (stat)
            {
                case Stat.Str:
                {
                    if ((from.LastStrGain + GetStatDelay(from.RawStr)) >= DateTime.UtcNow)
                        return;

                    from.LastStrGain = DateTime.UtcNow;
                    break;
                }

                case Stat.Dex:
                {
                    if ((from.LastDexGain + GetStatDelay(from.RawDex)) >= DateTime.UtcNow)
                        return;

                    from.LastDexGain = DateTime.UtcNow;
                    break;
                }

                case Stat.Int:
                {
                    if ((from.LastIntGain + GetStatDelay(from.RawInt)) >= DateTime.UtcNow)
                        return;

                    from.LastIntGain = DateTime.UtcNow;
                    break;
                }
            }

            bool atrophy = ((from.RawStatTotal / (double)from.StatCap) >= Utility.RandomDouble());

            IncreaseStat(from, stat, atrophy);
        }

        #region SkillGainRanges

        public class SkillGainRange
        {
            public SkillName m_SkillName;
            public int[] m_UsesPerRange = new int[] { };

            public SkillGainRange(SkillName skillName, int[] usesPerRange)
            {
                m_SkillName = skillName;
                m_UsesPerRange = usesPerRange;
            }
        }

        private static SkillGainRange[] m_SkillGainRanges = new SkillGainRange[49]
		{
            new SkillGainRange(SkillName.Archery, new int[]{        10, 10,     //0-5, 5-10
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

            new SkillGainRange(SkillName.Anatomy, new int[]{        1, 2,     //0-5, 5-10
                                                                    3, 4,     //10-15, 15-20
                                                                    5, 6,     //20-25, 25-30
                                                                    7, 8,     //30-35, 30-40
                                                                    9, 10,     //40-45, 45-50
                                                                    11, 12,     //50-55, 55-60
                                                                    13, 14,     //60-65, 65-70
                                                                    15, 16,     //70-75, 75-80
                                                                    17, 18,     //80-85, 85-90
                                                                    19, 20,     //90-95, 95-100
                                                                    21, 22,     //100-105, 105-110
                                                                    22, 23}),   //110-115, 115-120

            new SkillGainRange(SkillName.AnimalLore, new int[]{  10, 10,     //0-5, 5-10
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

            new SkillGainRange(SkillName.AnimalTaming, new int[]{  10, 10,     //0-5, 5-10
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

            new SkillGainRange(SkillName.Archery, new int[]{  10, 10,     //0-5, 5-10
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

            new SkillGainRange(SkillName.ArmsLore, new int[]{  10, 10,     //0-5, 5-10
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

            new SkillGainRange(SkillName.Begging, new int[]{  10, 10,     //0-5, 5-10
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

            new SkillGainRange(SkillName.Blacksmith, new int[]{  10, 10,     //0-5, 5-10
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

            new SkillGainRange(SkillName.Bowcraft, new int[]{  10, 10,     //0-5, 5-10
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

        new SkillGainRange(SkillName.Camping, new int[]{  10, 10,     //0-5, 5-10
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

        new SkillGainRange(SkillName.Carpentry, new int[]{  10, 10,     //0-5, 5-10
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

        new SkillGainRange(SkillName.Cartography, new int[]{  10, 10,     //0-5, 5-10
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

        new SkillGainRange(SkillName.Cooking, new int[]{  10, 10,     //0-5, 5-10
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

        new SkillGainRange(SkillName.DetectHidden, new int[]{  10, 10,     //0-5, 5-10
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

        new SkillGainRange(SkillName.Discordance, new int[]{  10, 10,     //0-5, 5-10
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

        new SkillGainRange(SkillName.EvalInt, new int[]{  10, 10,     //0-5, 5-10
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

        new SkillGainRange(SkillName.Fencing, new int[]{  10, 10,     //0-5, 5-10
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

        new SkillGainRange(SkillName.Fishing, new int[]{  10, 10,     //0-5, 5-10
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

        new SkillGainRange(SkillName.Forensics, new int[]{  10, 10,     //0-5, 5-10
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

        new SkillGainRange(SkillName.Healing, new int[]{  10, 10,     //0-5, 5-10
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

        new SkillGainRange(SkillName.Herding, new int[]{  10, 10,     //0-5, 5-10
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

        new SkillGainRange(SkillName.Hiding, new int[]{  10, 10,     //0-5, 5-10
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

        new SkillGainRange(SkillName.Inscribe, new int[]{  10, 10,     //0-5, 5-10
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

        new SkillGainRange(SkillName.ItemID, new int[]{  10, 10,     //0-5, 5-10
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

        new SkillGainRange(SkillName.Lockpicking, new int[]{  10, 10,     //0-5, 5-10
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

        new SkillGainRange(SkillName.Lumberjacking, new int[]{  10, 10,     //0-5, 5-10
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

        new SkillGainRange(SkillName.Macing, new int[]{  10, 10,     //0-5, 5-10
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

        new SkillGainRange(SkillName.Magery, new int[]{  10, 10,     //0-5, 5-10
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

        new SkillGainRange(SkillName.MagicResist, new int[]{  10, 10,     //0-5, 5-10
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

        new SkillGainRange(SkillName.Meditation, new int[]{  10, 10,     //0-5, 5-10
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

        new SkillGainRange(SkillName.Mining, new int[]{  10, 10,     //0-5, 5-10
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

        new SkillGainRange(SkillName.Musicianship, new int[]{  10, 10,     //0-5, 5-10
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

        new SkillGainRange(SkillName.Parry, new int[]{  10, 10,     //0-5, 5-10
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

        new SkillGainRange(SkillName.Peacemaking, new int[]{  10, 10,     //0-5, 5-10
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

        new SkillGainRange(SkillName.Poisoning, new int[]{  10, 10,     //0-5, 5-10
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

        new SkillGainRange(SkillName.Provocation, new int[]{  10, 10,     //0-5, 5-10
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

        new SkillGainRange(SkillName.RemoveTrap, new int[]{  10, 10,     //0-5, 5-10
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

        new SkillGainRange(SkillName.Snooping, new int[]{  10, 10,     //0-5, 5-10
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

        new SkillGainRange(SkillName.SpiritSpeak, new int[]{  10, 10,     //0-5, 5-10
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

        new SkillGainRange(SkillName.Stealing, new int[]{  10, 10,     //0-5, 5-10
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

        new SkillGainRange(SkillName.Stealth, new int[]{  10, 10,     //0-5, 5-10
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

        new SkillGainRange(SkillName.Swords, new int[]{  10, 10,     //0-5, 5-10
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

        new SkillGainRange(SkillName.Tactics, new int[]{  10, 10,     //0-5, 5-10
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

        new SkillGainRange(SkillName.Tailoring, new int[]{  10, 10,     //0-5, 5-10
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

        new SkillGainRange(SkillName.TasteID, new int[]{  10, 10,     //0-5, 5-10
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

        new SkillGainRange(SkillName.Tinkering, new int[]{  10, 10,     //0-5, 5-10
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

        new SkillGainRange(SkillName.Tracking, new int[]{  10, 10,     //0-5, 5-10
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

        new SkillGainRange(SkillName.Veterinary, new int[]{  10, 10,     //0-5, 5-10
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

        new SkillGainRange(SkillName.Wrestling, new int[]{  10, 10,     //0-5, 5-10
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