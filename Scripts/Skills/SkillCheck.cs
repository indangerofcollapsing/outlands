using System;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Multis;
using Server.Spells;
using Server.Achievements;
using Server.Regions;

namespace Server.Misc
{
    public class SkillCheck
    {   
        public static TimeSpan PowerHourResetTime = TimeSpan.FromHours(23);

        public static int BoostSkillMinimumLevel = 0;

        public static void Initialize()
        {
            Mobile.SkillCheckLocationHandler = new SkillCheckLocationHandler(XmlSpawnerSkillCheck.Mobile_SkillCheckLocation);
            Mobile.SkillCheckDirectLocationHandler = new SkillCheckDirectLocationHandler(XmlSpawnerSkillCheck.Mobile_SkillCheckDirectLocation);

            Mobile.SkillCheckTargetHandler = new SkillCheckTargetHandler(XmlSpawnerSkillCheck.Mobile_SkillCheckTarget);
            Mobile.SkillCheckDirectTargetHandler = new SkillCheckDirectTargetHandler(XmlSpawnerSkillCheck.Mobile_SkillCheckDirectTarget);

            SetGainFactors();
        }

        public static void SetGainFactors()
        {
            foreach(SkillInfo skillInfo in SkillInfo.Table)
            {
                switch (skillInfo.Name)
                {
                    case "Alchemy": skillInfo.GainFactor = .055; break;
                    case "Anatomy": skillInfo.GainFactor = .03; break;
                    case "Animal Lore": skillInfo.GainFactor = .07; break;
                    case "Item Identification": skillInfo.GainFactor = .09; break;
                    case "Arms Lore": skillInfo.GainFactor = .1; break;
                    case "Parrying": skillInfo.GainFactor = .034; break;
                    case "Begging": skillInfo.GainFactor = .1; break;
                    case "Blacksmithy": skillInfo.GainFactor = .03; break;
                    case "Peacemaking": skillInfo.GainFactor = .2; break;
                    case "Camping": skillInfo.GainFactor = .2; break;
                    case "Carpentry": skillInfo.GainFactor = .04; break;
                    case "Cartography": skillInfo.GainFactor = .07; break;
                    case "Cooking": skillInfo.GainFactor = .14; break;
                    case "Detecting Hidden": skillInfo.GainFactor = .15; break;
                    case "Discordance": skillInfo.GainFactor = .06; break;
                    case "Evaluating Intelligence": skillInfo.GainFactor = .062; break;
                    case "Healing": skillInfo.GainFactor = .099; break;
                    case "Fishing": skillInfo.GainFactor = .1; break;
                    case "Forensic Evaluation": skillInfo.GainFactor = .085; break;
                    case "Herding": skillInfo.GainFactor = .05; break;
                    case "Hiding": skillInfo.GainFactor = .11; break;
                    case "Provocation": skillInfo.GainFactor = .11; break;
                    case "Inscription": skillInfo.GainFactor = .035; break;
                    case "Lockpicking": skillInfo.GainFactor = .25; break;
                    case "Magery": skillInfo.GainFactor = .22; break;
                    case "Resisting Spells": skillInfo.GainFactor = .084; break;
                    case "Tactics": skillInfo.GainFactor = .075; break;
                    case "Snooping": skillInfo.GainFactor = .05; break;
                    case "Musicianship": skillInfo.GainFactor = .11; break;
                    case "Poisoning": skillInfo.GainFactor = .113; break;
                    case "Archery": skillInfo.GainFactor = .062; break;
                    case "Spirit Speak": skillInfo.GainFactor = .05; break;
                    case "Stealing": skillInfo.GainFactor = .085; break;
                    case "Tailoring": skillInfo.GainFactor = .075; break;
                    case "Animal Taming": skillInfo.GainFactor = .17; break;
                    case "Taste Identification": skillInfo.GainFactor = .15; break;
                    case "Tinkering": skillInfo.GainFactor = .032; break;
                    case "Tracking": skillInfo.GainFactor = .25; break;
                    case "Veterinary": skillInfo.GainFactor = .1; break;
                    case "Swordsmanship": skillInfo.GainFactor = .045; break;
                    case "Mace Fighting": skillInfo.GainFactor = .055; break;
                    case "Fencing": skillInfo.GainFactor = .045; break;
                    case "Wrestling": skillInfo.GainFactor = .07; break;
                    case "Lumberjacking": skillInfo.GainFactor = .15; break;
                    case "Mining": skillInfo.GainFactor = .057; break;
                    case "Meditation": skillInfo.GainFactor = .025; break;
                    case "Stealth": skillInfo.GainFactor = .11; break;
                    case "Remove Trap": skillInfo.GainFactor = .1; break;

                    default: skillInfo.GainFactor = .1; break;
                }
            }
        }

        public static bool IsDungeonBoostedSkill(SkillName skillName)
        {
            switch (skillName)
            {
                case SkillName.Alchemy: return false; break;
                case SkillName.Anatomy: return false; break;
                case SkillName.AnimalLore: return false; break;
                case SkillName.AnimalTaming: return false; break;
                case SkillName.Archery: return false; break;
                case SkillName.ArmsLore: return false; break;
                case SkillName.Begging: return false; break;
                case SkillName.Blacksmith: return false; break;
                case SkillName.Camping: return false; break;
                case SkillName.Carpentry: return false; break;
                case SkillName.Cartography: return false; break;
                case SkillName.Cooking: return false; break;
                case SkillName.DetectHidden: return false; break;
                case SkillName.Discordance: return false; break;
                case SkillName.EvalInt: return false; break;
                case SkillName.Fencing: return false; break;
                case SkillName.Fishing: return false; break;
                case SkillName.Forensics: return false; break;
                case SkillName.Healing: return false; break;
                case SkillName.Herding: return false; break;
                case SkillName.Hiding: return false; break;
                case SkillName.Inscribe: return false; break;
                case SkillName.ItemID: return false; break;
                case SkillName.Lockpicking: return false; break;
                case SkillName.Lumberjacking: return false; break;
                case SkillName.Macing: return false; break;
                case SkillName.Magery: return false; break;
                case SkillName.MagicResist: return false; break;
                case SkillName.Meditation: return false; break;
                case SkillName.Mining: return false; break;
                case SkillName.Musicianship: return false; break;
                case SkillName.Parry: return false; break;
                case SkillName.Peacemaking: return false; break;
                case SkillName.Poisoning: return false; break;
                case SkillName.Provocation: return false; break;
                case SkillName.RemoveTrap: return false; break;
                case SkillName.Snooping: return false; break;
                case SkillName.SpiritSpeak: return false; break;
                case SkillName.Stealing: return false; break;
                case SkillName.Stealth: return false; break;
                case SkillName.Swords: return false; break;
                case SkillName.Tactics: return false; break;
                case SkillName.Tailoring: return false; break;
                case SkillName.TasteID: return false; break;
                case SkillName.Tinkering: return false; break;
                case SkillName.Tracking: return false; break;
                case SkillName.Veterinary: return false; break;
                case SkillName.Wrestling: return false; break;
            }

            return false;
        }

        public static double GetSkillChanceMultiplier(Skill s, Mobile from)
        {
            //TEST: Adjust This (To Interval System)

            switch (s.SkillName)
            {
                case SkillName.Blacksmith:
                    if (s.Base <= 60.0) return 6.0;
                    if (s.Base <= 80.0 && s.Base > 60.0) return 2.0;
                    if (s.Base <= 91.0 && s.Base > 80.0) return 1.3;
                    if (s.Base > 91.0) return 1.2;      
                break;

                case SkillName.Tinkering:
                    if (s.Base <= 60.0) return 6.0;
                    if (s.Base <= 80.0 && s.Base > 60.0) return 2.0;
                    if (s.Base <= 91.0 && s.Base > 80.0) return 1.3;
                    if (s.Base > 91.0) return 1.2;      
                break;

                case SkillName.Cooking:
                    if (s.Base <= 60.0) return 6.0;
                    if (s.Base <= 80.0 && s.Base > 60.0) return 2.0;
                    if (s.Base <= 91.0 && s.Base > 80.0) return 1.3;
                    if (s.Base > 91.0) return 1.2;                   
                break;

                case SkillName.Magery:
                    if (s.Base <= 60.0) return 6.0;	
                    if (s.Base <= 80.0 && s.Base > 60.0) return 2.0;
                    if (s.Base <= 91.0 && s.Base > 80.0) return 2.0;
                    if (s.Base <= 95.0 && s.Base > 91.0) return 1.0;
                    if (s.Base <= 99.0 && s.Base > 95.0) return 1.0 / 1.2;
                    if (s.Base > 99.0) return 1.0 / 3.0;
                break;

                case SkillName.MagicResist:
                    double multip = from.IncomingResistCheckFromMonster ? (Math.Max(s.Base, 30.0)) / 15.0 : 1.0;
                    if (s.Base <= 60.0) return 6.0 * multip;
                    if (s.Base <= 80.0 && s.Base > 60.0) return 2.0 * multip;
                    if (s.Base <= 91.0 && s.Base > 80.0) return 1.8 * multip;
                    if (s.Base > 91.0) return 1.8 * multip;                    
                break;

                case SkillName.Carpentry:
                    if (s.Base <= 60.0) return 6.0;
                    if (s.Base <= 80.0 && s.Base > 60.0) return 2.0;
                    if (s.Base <= 91.0 && s.Base > 80.0) return 1.0;
                    if (s.Base > 91.0) return 1.0 / 1.5;                   
                break;

                case SkillName.Cartography:
                    if (s.Base <= 60.0) return 3.0;
                    if (s.Base > 60.0) return 1.5;
                break;

                case SkillName.Inscribe:
                    if (s.Base <= 60.0) return 6.0;
                    if (s.Base <= 80.0 && s.Base > 60.0) return 2.0;
                    if (s.Base > 80.0) return 1.0;                   
                break;

                case SkillName.Healing:
                    if (s.Base <= 60.0) return 6.0;
                    if (s.Base <= 80.0 && s.Base > 60.0) return 2.0;
                    if (s.Base <= 91.0 && s.Base > 80.0) return 1.0;
                    if (s.Base <= 95.0 && s.Base > 91.0) return 1.0;
                    if (s.Base <= 99.0 && s.Base > 95.0) return 1.0;
                    if (s.Base > 99.0) return 1.0 / 4.0;
                break;

                case SkillName.Veterinary:
                    if (s.Base <= 60.0) return 6.0;
                    if (s.Base <= 80.0 && s.Base > 60.0) return 2.0;
                    if (s.Base <= 91.0 && s.Base > 80.0) return 1.0;
                    if (s.Base <= 95.0 && s.Base > 91.0) return 1.0;
                    if (s.Base <= 99.0 && s.Base > 95.0) return 1.0;
                    if (s.Base > 99.0) return 1.0 / 4.0;
                break;

                case SkillName.Poisoning:
                    if (s.Base <= 60.0) return 6.0;
                    if (s.Base <= 80.0 && s.Base > 60.0) return 2.0;
                    if (s.Base <= 91.0 && s.Base > 80.0) return 2.0;
                    if (s.Base <= 95.0 && s.Base > 91.0) return 1.0;
                    if (s.Base <= 99.0 && s.Base > 95.0) return 1.0;
                    if (s.Base > 99.0) return 1.0;
                break;

                case SkillName.AnimalTaming:
                    if (s.Base <= 60.0) return 6.0;
                    if (s.Base <= 80.0 && s.Base > 60.0) return 2.0;
                    if (s.Base <= 91.0 && s.Base > 80.0) return 1.0;
                    if (s.Base <= 95.0 && s.Base > 91.0) return 0.9;
                    if (s.Base <= 99.0 && s.Base > 95.0) return 0.8;
                    if (s.Base > 99.0) return 0.7;
                break;

                case SkillName.Lockpicking:
                    if (s.Base <= 60.0) return 6.0;
                    if (s.Base <= 80.0 && s.Base > 60.0) return 2.0;
                    if (s.Base <= 91.0 && s.Base > 80.0) return 1.0;
                    if (s.Base <= 95.0 && s.Base > 91.0) return 1.0 / 3.0;
                    if (s.Base <= 99.0 && s.Base > 95.0) return 1.0 / 3.5;
                    if (s.Base > 99.0) return 1.0 / 3.0;
                break;

                case SkillName.Alchemy:
                    if (s.Base <= 60.0) return 6.0;
                    if (s.Base <= 80.0 && s.Base > 60.0) return 2.0;
                    if (s.Base <= 91.0 && s.Base > 80.0) return 1.0;
                    if (s.Base > 91.0) return 1.0 / 2.0;                   
                break;

                case SkillName.Meditation:
                    if (s.Base <= 60.0) return 6.0;
                    if (s.Base <= 80.0 && s.Base > 60.0) return 2.0;
                    if (s.Base <= 91.0 && s.Base > 80.0) return 1.8;
                    if (s.Base > 91.0) return 1.0;                   
                break;

                case SkillName.Mining:
                    if (s.Base <= 60.0) return 6.0;
                    if (s.Base <= 80.0 && s.Base > 60.0) return 3.0;
                    if (s.Base <= 91.0 && s.Base > 80.0) return 2.0;
                    if (s.Base > 91.0) return 1.0;                   
                break;

                case SkillName.Fishing:
                    if (s.Base <= 60.0) return 6.0;
                    if (s.Base <= 80.0 && s.Base > 60.0) return 3.0;
                    if (s.Base <= 91.0 && s.Base > 80.0) return 2.0;
                    if (s.Base > 91.0) return 1.0;
                break;

                case SkillName.Lumberjacking:
                    if (s.Base <= 60.0) return 6.0;
                    if (s.Base <= 80.0 && s.Base > 60.0) return 3.0;
                    if (s.Base <= 91.0 && s.Base > 80.0) return 2.0;
                    if (s.Base > 91.0) return 1.0;
                break;

                case SkillName.Stealing:
                    if (s.Base <= 60.0) return 6.0;
                    if (s.Base <= 80.0 && s.Base > 60.0) return 2.0;
                    if (s.Base <= 91.0 && s.Base > 80.0) return 1.0;
                    if (s.Base <= 95.0 && s.Base > 91.0) return 1.0;
                    if (s.Base <= 99.0 && s.Base > 95.0) return 0.88;
                    if (s.Base > 99.0) return 1.0 / 2.0;
                break;

                default:
                    if (s.Base <= 60.0) return 6.0;
                    if (s.Base <= 80.0 && s.Base > 60.0) return 2.0;
                    if (s.Base <= 91.0 && s.Base > 80.0) return 1.0;
                    if (s.Base <= 95.0 && s.Base > 91.0) return 1.0 / 2.0;
                    if (s.Base > 95.0) return 1.0 / 3.0;
                break;
            }

            return 1.0;
        }

        public static bool Mobile_SkillCheckLocation(Mobile from, SkillName skillName, double minSkill, double maxSkill)
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

            Point2D loc = new Point2D(from.Location.X, from.Location.Y);
            return CheckSkill(from, skill, loc, chance);
        }

        public static bool Mobile_SkillCheckDirectLocation(Mobile from, SkillName skillName, double chance)
        {
            Skill skill = from.Skills[skillName];

            if (skill == null)
                return false;

            if (chance < 0.0)
                return false;

            else if (chance >= 1.0)
                return true;

            Point2D loc = new Point2D(from.Location.X, from.Location.Y);
            return CheckSkill(from, skill, loc, chance);
        }

        public static bool CheckSkill(Mobile from, Skill skill, object amObj, double chance)
        {
            //TEST: Adjust This
            Skill mobileSkill = from.Skills[skill.SkillName];

            if (from.Skills.Cap == 0)
                return false;

            bool success = (chance >= Utility.RandomDouble());
            double gc = 0.5;

            gc += (skill.Cap - skill.Base) / skill.Cap;
            gc /= 2;

            gc += (1.0 - chance) * (success ? 0.5 : 0.2);
            gc /= 2;
            
            gc *= skill.Info.GainFactor;

            if (skill.Base <= 30.0)
                gc *= 10.0;

            gc *= GetSkillChanceMultiplier(skill, from);

            if (from.Player)
            {
                //Region Skillgain Adjustments
                HouseModifier(from, ref gc); 
                Items.TownSquare.TownSquareModifier(from, skill, ref gc);
                Items.BritainTownSquare.BritainTownSquareModifier(from, skill, ref gc);
                DungeonModifier(from, skill.SkillName, ref gc);
            }

            bool gainOccurred = gc >= Utility.RandomDouble();
            bool allowGain = AllowGain(from, skill, amObj);

            if (from.Alive && ((gainOccurred && allowGain) || skill.Base < 10.0))
                Gain(from, skill);

            return success;
        }

        //TEST: Town Skills?
        public static SkillName[] BuffBoostedSkills = new SkillName[] 
        {
            SkillName.Blacksmith,
            SkillName.Carpentry,
            SkillName.Tinkering,
            SkillName.Tailoring,
            SkillName.Inscribe,
            SkillName.Alchemy,
        };        

        public static void HouseModifier(Mobile from, ref double gc)
        {
            if (from == null || from.Map == null || from.Map == Map.Internal)
                return;

            PlayerMobile pm = from as PlayerMobile;
            
            if (pm == null)
                return;

            Sector sector = from.Map.GetSector(from.X, from.Y);

            //TEST: Adjust This
            for (int i = 0; i < sector.Multis.Count; ++i)
            {
                BaseMulti multi = (BaseMulti)sector.Multis[i];

                if (multi is BaseHouse && ((BaseHouse)multi).IsInside(from.Location, 16))
                {
                    gc *= 0.5;

                    return;
                }
            }
        }

        public static void DungeonModifier(Mobile from, SkillName skillName, ref double gc)
        {
            if (IsDungeonBoostedSkill(skillName))
            {
                if (from.Region.IsPartOf(typeof(NewbieDungeonRegion)))
                    gc *= 3.15;

                else if (from.Region.IsPartOf(typeof(DungeonRegion)) && !SpellHelper.IsBritainSewers(from.Map, from.Location))
                    gc *= 3.0;
            }
        }

        public static bool Mobile_SkillCheckTarget(Mobile from, SkillName skillName, object target, double minSkill, double maxSkill)
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

            return CheckSkill(from, skill, target, chance);
        }

        public static bool Mobile_SkillCheckDirectTarget(Mobile from, SkillName skillName, object target, double chance)
        {
            Skill skill = from.Skills[skillName];

            if (skill == null)
                return false;

            if (chance < 0.0)
                return false;

            else if (chance >= 1.0)
                return true;

            return CheckSkill(from, skill, target, chance);
        }

        private static bool AllowGain(Mobile from, Skill skill, object obj)
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
            PlayerMobile pm = from as PlayerMobile;
            BaseCreature bc = from as BaseCreature;

            //Tamed Creature
            if (bc != null)
            {
                if (bc.Controlled && bc.ControlMaster is PlayerMobile)
                    return;
            }
           
            if (pm != null)
            {
                if (pm.Region is UOACZRegion)
                    return;
            }

            if (from.Region.IsPartOf(typeof(Regions.Jail)))
                return;

            if (from is BaseCreature && ((BaseCreature)from).IsDeadPet)
                return;

            int toGain = 1;
            
            if (skill.Base < skill.Cap && skill.Lock == SkillLock.Up)
            {
                if (skill.Base <= 10.0)
                    toGain = Utility.Random(4) + 1;

                Skills skills = from.Skills;

                if ((skills.Total / skills.Cap) >= Utility.RandomDouble())
                {
                    for (int i = 0; i < skills.Length; ++i)
                    {
                        Skill toLower = skills[i];

                        if (toLower != skill && toLower.Lock == SkillLock.Down && toLower.BaseFixedPoint >= toGain)
                        {
                            toLower.BaseFixedPoint -= toGain;
                            break;
                        }
                    }
                }                
            }

            if (from.Player)
                DailyAchievement.TickProgress(Category.Newb, (PlayerMobile)from, NewbCategory.GainSkill, toGain);

            if (skill.Base < 10.0)
                return;

            if (skill.Lock == SkillLock.Up)
            {
                SkillInfo info = skill.Info;

                double gc = 1.0;
                
                StatScaleModifier(from, skill.Value, ref gc);

                if (from.StrLock == StatLockType.Up && (gc * info.StrGain / 12.0) > Utility.RandomDouble()) //33.3
                    GainStat(from, Stat.Str);

                else if (from.DexLock == StatLockType.Up && (gc * info.DexGain / 12.0) > Utility.RandomDouble()) //33.3
                    GainStat(from, Stat.Dex);

                else if (from.IntLock == StatLockType.Up && (gc * info.IntGain / 12.0) > Utility.RandomDouble()) //33.3
                    GainStat(from, Stat.Int);
            }
        }

        public static void StatScaleModifier(Mobile from, double skillValue, ref double gc)
        {
            PlayerMobile pm = from as PlayerMobile;

            if (pm == null)
                return;

            if (skillValue < 20.0)
                gc *= 0.4;

            else if (skillValue < 30.0)
                gc *= 0.7;

            else if (skillValue < 60.0)
                gc *= 1.0;

            else if (skillValue < 100.0)
                gc *= 2;
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
    }
}