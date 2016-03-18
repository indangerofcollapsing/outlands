using System;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Multis;
using Server.Spells;
using Server.Custom.Townsystem;
using Server.Achievements;
using Server.Custom.Battlegrounds;
using Server.Custom.Battlegrounds.Regions;
using Server.PortalSystem;
using Server.Regions;

namespace Server.Misc
{
    public class SkillCheck
    {
        private static readonly bool AntiMacroCode = false;		//Change this to false to disable anti-macro code
        public static TimeSpan AntiMacroExpire = TimeSpan.FromMinutes(5.0); //How long do we remember targets/locations?
        public const int Allowance = 3;	//How many times may we use the same location/target for gain
        private const int LocationSize = 5; //The size of eeach location, make this smaller so players dont have to move as far
        private static bool[] UseAntiMacro = new bool[]
		{
			// true if this skill uses the anti-macro code, false if it does not
			false,// Alchemy = 0,
			true,// Anatomy = 1,
			true,// AnimalLore = 2,
			true,// ItemID = 3,
			true,// ArmsLore = 4,
			true,// Parry = 5,
			true,// Begging = 6,
			false,// Blacksmith = 7,
			false,// Fletching = 8,
			true,// Peacemaking = 9,
			true,// Camping = 10,
			false,// Carpentry = 11,
			false,// Cartography = 12,
			false,// Cooking = 13,
			true,// DetectHidden = 14,
			true,// Discordance = 15,
			true,// EvalInt = 16,
			true,// Healing = 17,
			true,// Fishing = 18,
			true,// Forensics = 19,
			true,// Herding = 20,
			true,// Hiding = 21,
			true,// Provocation = 22,
			false,// Inscribe = 23,
			true,// Lockpicking = 24,
			true,// Magery = 25,
			true,// MagicResist = 26,
			true,// Tactics = 27,
			true,// Snooping = 28,
			true,// Musicianship = 29,
			true,// Poisoning = 30,
			true,// Archery = 31,
			true,// SpiritSpeak = 32,
			true,// Stealing = 33,
			false,// Tailoring = 34,
			true,// AnimalTaming = 35,
			true,// TasteID = 36,
			false,// Tinkering = 37,
			true,// Tracking = 38,
			true,// Veterinary = 39,
			true,// Swords = 40,
			true,// Macing = 41,
			true,// Fencing = 42,
			true,// Wrestling = 43,
			true,// Lumberjacking = 44,
			true,// Mining = 45,
			true,// Meditation = 46,
			true,// Stealth = 47,
			true,// RemoveTrap = 48,
			true,// Necromancy = 49,
			false,// Focus = 50,
			true,// Chivalry = 51
			true,// Bushido = 52
			true,//Ninjitsu = 53
			true // Spellweaving
		};

        //SkillScroll addition
        private static bool[] DungeonBoostedSkill = new bool[]
		{
            // true if the skill gain is boosted inside a dungeon
            false,// Alchemy = 0,
            true,// Anatomy = 1,
            false,// AnimalLore = 2,
            false,// ItemID = 3,
            false,// ArmsLore = 4,
            true,// Parry = 5,
            false,// Begging = 6,
            false,// Blacksmith = 7,
            false,// Fletching = 8,
            true,// Peacemaking = 9,
            false,// Camping = 10,
            false,// Carpentry = 11,
            false,// Cartography = 12,
            false,// Cooking = 13,
            true,// DetectHidden = 14,
            true,// Discordance = 15,
            true,// EvalInt = 16,
            true,// Healing = 17,
            false,// Fishing = 18,
            true,// Forensics = 19,
            false,// Herding = 20,
            true,// Hiding = 21,
            true,// Provocation = 22,
            false,// Inscribe = 23,
            true,// Lockpicking = 24,
            true,// Magery = 25,
            true,// MagicResist = 26,
            true,// Tactics = 27,
            false,// Snooping = 28,
            true,// Musicianship = 29,
            false,// Poisoning = 30,
            true,// Archery = 31,
            true,// SpiritSpeak = 32,
            false,// Stealing = 33,
            false,// Tailoring = 34,
            false,// AnimalTaming = 35,
            false,// TasteID = 36,
            false,// Tinkering = 37,
            true,// Tracking = 38,
            true,// Veterinary = 39,
            true,// Swords = 40,
            true,// Macing = 41,
            true,// Fencing = 42,
            true,// Wrestling = 43,
            false,// Lumberjacking = 44,
            true,// Mining = 45,
            true,// Meditation = 46,
            true,// Stealth = 47,
            true,// RemoveTrap = 48,
            true,// Necromancy = 49,
            false,// Focus = 50,
            true,// Chivalry = 51
            true,// Bushido = 52
            true,// Ninjitsu = 53
            true // Spellweaving
		};
        // SkillScroll end

        public static TimeSpan PowerHourResetTime = TimeSpan.FromHours(23);

        public static int BoostSkillMinimumLevel = 0;

        public static void Initialize()
        {
            // Begin mod to enable XmlSpawner skill triggering
            Mobile.SkillCheckLocationHandler = new SkillCheckLocationHandler(XmlSpawnerSkillCheck.Mobile_SkillCheckLocation);
            Mobile.SkillCheckDirectLocationHandler = new SkillCheckDirectLocationHandler(XmlSpawnerSkillCheck.Mobile_SkillCheckDirectLocation);

            Mobile.SkillCheckTargetHandler = new SkillCheckTargetHandler(XmlSpawnerSkillCheck.Mobile_SkillCheckTarget);
            Mobile.SkillCheckDirectTargetHandler = new SkillCheckDirectTargetHandler(XmlSpawnerSkillCheck.Mobile_SkillCheckDirectTarget);
            // End mod to enable XmlSpawner skill triggering

            // Start IPY specific (?)
            SkillInfo.Table[0].GainFactor = .055;// Alchemy = 0, 
            SkillInfo.Table[1].GainFactor = .03;// Anatomy = 1, 
            SkillInfo.Table[2].GainFactor = .07;// AnimalLore = 2, 
            SkillInfo.Table[3].GainFactor = .09;// ItemID = 3, 
            SkillInfo.Table[4].GainFactor = .1;// ArmsLore = 4, 
            SkillInfo.Table[5].GainFactor = .034;// Parry = 5, 
            SkillInfo.Table[6].GainFactor = .10;// Begging = 6, 
            SkillInfo.Table[7].GainFactor = .030;// Blacksmith = 7, 
            SkillInfo.Table[8].GainFactor = .07;// Fletching = 8, 
            SkillInfo.Table[9].GainFactor = .20;// Peacemaking = 9, 
            SkillInfo.Table[10].GainFactor = .20;// Camping = 10, 
            SkillInfo.Table[11].GainFactor = .04;// Carpentry = 11, 
            SkillInfo.Table[12].GainFactor = .070;// Cartography = 12, 
            SkillInfo.Table[13].GainFactor = .14;// Cooking = 13, 
            SkillInfo.Table[14].GainFactor = .15;// DetectHidden = 14, 
            SkillInfo.Table[15].GainFactor = .06;// Discordance = 15, 
            SkillInfo.Table[16].GainFactor = .062;// EvalInt = 16, 
            SkillInfo.Table[17].GainFactor = .099;// Healing = 17, 
            SkillInfo.Table[18].GainFactor = .1;// Fishing = 18, 
            SkillInfo.Table[19].GainFactor = .085;// Forensics = 19, 
            SkillInfo.Table[20].GainFactor = .05;// Herding = 20, 
            SkillInfo.Table[21].GainFactor = .11;// Hiding = 21, 
            SkillInfo.Table[22].GainFactor = .11;// Provocation = 22, 
            SkillInfo.Table[23].GainFactor = .035;// Inscribe = 23, 
            SkillInfo.Table[24].GainFactor = .25;// Lockpicking = 24, 
            SkillInfo.Table[25].GainFactor = .22;// Magery = 25, 
            SkillInfo.Table[26].GainFactor = .084;// MagicResist = 26, 
            SkillInfo.Table[27].GainFactor = .075;// Tactics = 27, 
            SkillInfo.Table[28].GainFactor = .05;// Snooping = 28, 
            SkillInfo.Table[29].GainFactor = .11;// Musicianship = 29, 
            SkillInfo.Table[30].GainFactor = .113;// Poisoning = 30 
            SkillInfo.Table[31].GainFactor = .062;// Archery = 31 
            SkillInfo.Table[32].GainFactor = .05;// SpiritSpeak = 32 
            SkillInfo.Table[33].GainFactor = .085;// Stealing = 33 
            SkillInfo.Table[34].GainFactor = .075;// Tailoring = 34 
            SkillInfo.Table[35].GainFactor = .17;// AnimalTaming = 35 
            SkillInfo.Table[36].GainFactor = .15;// TasteID = 36 
            SkillInfo.Table[37].GainFactor = .032;// Tinkering = 37 
            SkillInfo.Table[38].GainFactor = .25;// Tracking = 38 
            SkillInfo.Table[39].GainFactor = .1;// Veterinary = 39 
            SkillInfo.Table[40].GainFactor = .045;// Swords = 40 
            SkillInfo.Table[41].GainFactor = .055;// Macing = 41 
            SkillInfo.Table[42].GainFactor = .045;// Fencing = 42 
            SkillInfo.Table[43].GainFactor = .07;// Wrestling = 43 
            SkillInfo.Table[44].GainFactor = .15;// Lumberjacking = 44 
            SkillInfo.Table[45].GainFactor = .057;// Mining = 45 
            SkillInfo.Table[46].GainFactor = .025;// Meditation = 46 
            SkillInfo.Table[47].GainFactor = .11;// Stealth = 47 
            SkillInfo.Table[48].GainFactor = .1;// RemoveTrap = 48 
            SkillInfo.Table[49].GainFactor = .01;// Necromancy = 49 
            SkillInfo.Table[50].GainFactor = .01;// Focus = 50 
            SkillInfo.Table[51].GainFactor = .01;// Chivalry = 51 
            // End IPY Specific
        }

        public static double GetSkillChanceMultiplier(Skill s, Mobile from)
        {
            switch (s.SkillName)
            {
                case SkillName.Blacksmith:
                case SkillName.Tinkering:
                case SkillName.Cooking:
                    if (s.Base <= 60.0) return 6.0;			// BLACKSMITH
                    if (s.Base <= 80.0 && s.Base > 60.0) return 2.0;
                    if (s.Base <= 91.0 && s.Base > 80.0) return 1.3;
                    if (s.Base > 91.0) return 1.2;
                    //if (s.Base <= 99.0 && s.Base > 95.0)	return 1.1;
                    //if (s.Base > 99.0)						return 1.0 / 2.0;	// BLACKSMITH
                    break;

                case SkillName.Magery:
                    if (s.Base <= 60.0) return 6.0;			// MAGERY
                    if (s.Base <= 80.0 && s.Base > 60.0) return 2.0;
                    if (s.Base <= 91.0 && s.Base > 80.0) return 2.0;
                    if (s.Base <= 95.0 && s.Base > 91.0) return 1.0;
                    if (s.Base <= 99.0 && s.Base > 95.0) return 1.0 / 1.2;
                    if (s.Base > 99.0) return 1.0 / 3.0;	// MAGERY
                    break;
                case SkillName.MagicResist:
                    double multip = from.IncomingResistCheckFromMonster ? (Math.Max(s.Base, 30.0)) / 15.0 : 1.0;
                    if (s.Base <= 60.0) return 6.0 * multip;		// MAGIC RESIST
                    if (s.Base <= 80.0 && s.Base > 60.0) return 2.0 * multip;
                    if (s.Base <= 91.0 && s.Base > 80.0) return 1.8 * multip;
                    if (s.Base > 91.0) return 1.8 * multip;
                    //if (s.Base <= 99.0 && s.Base > 95.0) return 1.0		    * multip;
                    //if (s.Base > 99.0)					 return (1.0 / 2.0) * multip;		// MAGIC RESIST
                    break;
                case SkillName.Fletching:
                    if (s.Base <= 60.0) return 6.0;			// FLETCHING
                    if (s.Base <= 80.0 && s.Base > 60.0) return 2.0;
                    if (s.Base <= 91.0 && s.Base > 80.0) return 1.0;
                    if (s.Base > 91.0) return 1.0 / 1.5;
                    //if (s.Base <= 99.0 && s.Base > 95.0) return 1.0 / 1.75;
                    //if (s.Base > 99.0)					 return 1.0 / 3.0;		// FLETCHING
                    break;
                case SkillName.Carpentry:
                    if (s.Base <= 60.0) return 6.0;		// CARPENTRY
                    if (s.Base <= 80.0 && s.Base > 60.0) return 2.0;
                    if (s.Base <= 91.0 && s.Base > 80.0) return 1.0;
                    if (s.Base > 91.0) return 1.0 / 1.5;
                    //if (s.Base <= 99.0 && s.Base > 95.0) return 1.0 / 3.0;
                    //if (s.Base > 99.0)					 return 1.0 / 3.0;	// CARPENTRY
                    break;
                case SkillName.Cartography:
                    if (s.Base <= 60.0) return 3.0;		// CARTHOGRAPHY
                    if (s.Base > 60.0) return 1.5;
                    break;
                case SkillName.Inscribe:
                    if (s.Base <= 60.0) return 6.0;		// INSCRIPTION
                    if (s.Base <= 80.0 && s.Base > 60.0) return 2.0;
                    if (s.Base > 80.0) return 1.0;
                    //if (s.Base <= 95.0 && s.Base > 91.0) return 1.0;
                    //if (s.Base <= 99.0 && s.Base > 95.0) return 1.0;
                    //if (s.Base > 99.0)					 return 1.0 /3.0;	// INSCRIPTION
                    break;
                case SkillName.Healing:
                case SkillName.Veterinary:
                    if (s.Base <= 60.0) return 6.0;	// HEALING AND VETERINARY
                    if (s.Base <= 80.0 && s.Base > 60.0) return 2.0;
                    if (s.Base <= 91.0 && s.Base > 80.0) return 1.0;
                    if (s.Base <= 95.0 && s.Base > 91.0) return 1.0;
                    if (s.Base <= 99.0 && s.Base > 95.0) return 1.0;
                    if (s.Base > 99.0) return 1.0 / 4.0;				// HEALING AND VETERINARY
                    break;
                case SkillName.Poisoning:
                    if (s.Base <= 60.0) return 6.0;	// POISONING
                    if (s.Base <= 80.0 && s.Base > 60.0) return 2.0;
                    if (s.Base <= 91.0 && s.Base > 80.0) return 2.0;
                    if (s.Base <= 95.0 && s.Base > 91.0) return 1.0;
                    if (s.Base <= 99.0 && s.Base > 95.0) return 1.0;
                    if (s.Base > 99.0) return 1.0;	// POISONING
                    break;
                case SkillName.AnimalTaming:
                    if (s.Base <= 60.0) return 6.0;		// TAMING
                    if (s.Base <= 80.0 && s.Base > 60.0) return 2.0;
                    if (s.Base <= 91.0 && s.Base > 80.0) return 1.0;
                    if (s.Base <= 95.0 && s.Base > 91.0) return 0.9;
                    if (s.Base <= 99.0 && s.Base > 95.0) return 0.8;
                    if (s.Base > 99.0) return 0.7;		// TAMING
                    break;
                case SkillName.Lockpicking:
                    if (s.Base <= 60.0) return 6.0;		// LOCKPICKING
                    if (s.Base <= 80.0 && s.Base > 60.0) return 2.0;
                    if (s.Base <= 91.0 && s.Base > 80.0) return 1.0;
                    if (s.Base <= 95.0 && s.Base > 91.0) return 1.0 / 3.0;
                    if (s.Base <= 99.0 && s.Base > 95.0) return 1.0 / 3.5;
                    if (s.Base > 99.0) return 1.0 / 3.0;	// LOCKPICKING
                    break;
                case SkillName.Alchemy:
                    if (s.Base <= 60.0) return 6.0;		// ALCHEMY
                    if (s.Base <= 80.0 && s.Base > 60.0) return 2.0;
                    if (s.Base <= 91.0 && s.Base > 80.0) return 1.0;
                    if (s.Base > 91.0) return 1.0 / 2.0;
                    //if (s.Base <= 99.0 && s.Base > 95.0) return 1.0 / 3.0;
                    //if (s.Base > 99.0) return 1.0 / 6.0;					// ALCHEMY
                    break;
                case SkillName.Meditation:
                    if (s.Base <= 60.0) return 6.0;		// MEDITATION
                    if (s.Base <= 80.0 && s.Base > 60.0) return 2.0;
                    if (s.Base <= 91.0 && s.Base > 80.0) return 1.8;
                    if (s.Base > 91.0) return 1.0;
                    //if (s.Base <= 99.0 && s.Base > 95.0) return 1.0 / 3.5;
                    //if (s.Base > 99.0) return 1.0 / 3.8;					// MEDITATION
                    break;
                case SkillName.Mining:
                    if (s.Base <= 60.0) return 6.0;		// MINING
                    if (s.Base <= 80.0 && s.Base > 60.0) return 3.0;
                    if (s.Base <= 91.0 && s.Base > 80.0) return 2.0;
                    if (s.Base > 91.0) return 1.0;
                    //if (s.Base <= 99.0 && s.Base > 95.0) return 1.0 / 7.0;
                    //if (s.Base > 99.0) return 1.0 / 12.0;	// MINING
                    break;
                case SkillName.Fishing:
                case SkillName.Lumberjacking:
                    if (s.Base <= 60.0) return 6.0;		// FISHING / LJ
                    if (s.Base <= 80.0 && s.Base > 60.0) return 3.0;
                    if (s.Base <= 91.0 && s.Base > 80.0) return 2.0;
                    if (s.Base > 91.0) return 1.0;     // FISHING / LJ
                    break;
                case SkillName.Stealing:
                    if (s.Base <= 60.0) return 6.0;         // STEALING
                    if (s.Base <= 80.0 && s.Base > 60.0) return 2.0;
                    if (s.Base <= 91.0 && s.Base > 80.0) return 1.0;
                    if (s.Base <= 95.0 && s.Base > 91.0) return 1.0;
                    if (s.Base <= 99.0 && s.Base > 95.0) return 0.88;
                    if (s.Base > 99.0) return 1.0 / 2.0;	// STEALING
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
                return false; // Too difficult
            else if (value >= maxSkill)
                return true; // No challenge

            double chance = (value - minSkill) / (maxSkill - minSkill);

            Point2D loc = new Point2D(from.Location.X / LocationSize, from.Location.Y / LocationSize);
            return CheckSkill(from, skill, loc, chance);
        }

        public static bool Mobile_SkillCheckDirectLocation(Mobile from, SkillName skillName, double chance)
        {
            Skill skill = from.Skills[skillName];

            if (skill == null)
                return false;

            if (chance < 0.0)
                return false; // Too difficult
            else if (chance >= 1.0)
                return true; // No challenge

            Point2D loc = new Point2D(from.Location.X / LocationSize, from.Location.Y / LocationSize);
            return CheckSkill(from, skill, loc, chance);
        }

        public static bool CheckSkill(Mobile from, Skill skill, object amObj, double chance)
        {	// IPY : Heavily modified
            Skill mobileSkill = from.Skills[skill.SkillName];

            if (from.Skills.Cap == 0)
                return false;

            bool success = (chance >= Utility.RandomDouble());
            double gc = 0.5; // IPY - same basic gain chance regardless of how close to skill cap you are
            gc += (skill.Cap - skill.Base) / skill.Cap;
            gc /= 2;

            gc += (1.0 - chance) * (success ? 0.5 : 0.2);
            gc /= 2;


            //if( gc < 0.01 )
            //    gc = 0.01

            gc *= skill.Info.GainFactor;

            // The following are tweaks for IPY.
            if (skill.Base <= 30.0)
                gc *= 10.0;

            gc *= GetSkillChanceMultiplier(skill, from);


            // IPY
            if (from.Player)
            {
                HouseModifier(from, ref gc); // -50% skill gain in houses unless you're in a militia

                gc *= GetPowerhourModifier(from); // +100% boost modifier if in power hour

                TownBuffModifier(from, skill, ref gc);

                Items.TownSquare.TownSquareModifier(from, skill, ref gc); // +30% bonus to crafting skills if in range of town square item

                Items.BritainTownSquare.BritainTownSquareModifier(from, skill, ref gc); // +50% bonus at WBB

                DungeonModifier(from, skill, ref gc); // +300% if in dungeon 
            }
            else
            {
                if (from is BaseCreature && ((BaseCreature)from).Controlled)
                    gc *= 2;
            }

            // debug
            if (from.Player && from.AccessLevel > AccessLevel.Player && skill.Base != skill.Cap)
                from.PrivateOverheadMessage(Network.MessageType.Regular, 0x22, false, String.Format("{0:8} : {1:P2}", skill.Name.Substring(0, Math.Min(8, skill.Name.Length - 1)), gc), from.NetState);

            bool gainOccurred = gc >= Utility.RandomDouble();
            bool allowGain = AllowGain(from, skill, amObj);

            if (from.Alive && ((gainOccurred && allowGain) || skill.Base < 10.0))
                Gain(from, skill);

            return success;
        }

        public static SkillName[] BuffBoostedSkills = new SkillName[] 
        {
            SkillName.Blacksmith,
            SkillName.Carpentry,
            SkillName.Tinkering,
            SkillName.Fletching,
            SkillName.Tailoring,
            SkillName.Inscribe,
            SkillName.Alchemy,
        };

        public static void TownBuffModifier(Mobile from, Skill skill, ref double gc)
        {
            for (int i = 0; i < BuffBoostedSkills.Length; i++ )
            {
                if (BuffBoostedSkills[i] == skill.SkillName)
                {
                    Town town = Town.CheckCitizenship(from);
                    if (town != null && town.Region.Contains(from.Location) && town.HasActiveBuff(CitizenshipBuffs.Crafting))
                        gc *= 1.65;
                    return;
                }
            }
        }

        public static void HouseModifier(Mobile from, ref double gc)
        {
            if (from == null || from.Map == null || from.Map == Map.Internal)
                return;

            PlayerMobile pm = from as PlayerMobile;
            if (pm == null || pm.IsInMilitia)
                return; // no penalty

            Sector sector = from.Map.GetSector(from.X, from.Y);

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

        public static void DungeonModifier(Mobile from, Skill skill, ref double gc)
        {
            if (DungeonBoostedSkill[skill.SkillID])
            {
                if (from.Region.IsPartOf(typeof(NewbieDungeonRegion)))
                    gc *= 3.15;
                else if (from.Region.IsPartOf(typeof(DungeonRegion)) && !SpellHelper.IsBritainSewers(from.Map, from.Location))
                    gc *= 3.0;
            }
        }

        public static bool InPowerHour(PlayerMobile pm)
        {
            return DateTime.UtcNow < pm.PowerHourReset.Add(pm.PowerHourDuration);
        }

        private static double GetPowerhourModifier(Mobile from)
        {
            PlayerMobile pm = from as PlayerMobile;
            if (pm != null && InPowerHour(pm))
                return 2.0;
            else
                return 1.0;
        }

        public static bool Mobile_SkillCheckTarget(Mobile from, SkillName skillName, object target, double minSkill, double maxSkill)
        {
            Skill skill = from.Skills[skillName];

            if (skill == null)
                return false;

            double value = skill.Value;

            if (value < minSkill)
                return false; // Too difficult
            else if (value >= maxSkill)
                return true; // No challenge

            double chance = (value - minSkill) / (maxSkill - minSkill);

            return CheckSkill(from, skill, target, chance);
        }

        public static bool Mobile_SkillCheckDirectTarget(Mobile from, SkillName skillName, object target, double chance)
        {
            Skill skill = from.Skills[skillName];

            if (skill == null)
                return false;

            if (chance < 0.0)
                return false; // Too difficult
            else if (chance >= 1.0)
                return true; // No challenge

            return CheckSkill(from, skill, target, chance);
        }

        private static bool AllowGain(Mobile from, Skill skill, object obj)
        {
            if (from.Region is BattlegroundRegion || from.Region is DungeonPortalsRegion)
                return false;

            if (from.Region is NewbieDungeonRegion)
            {
                if (skill.Base >= NewbieDungeonRegion.SkillGainCap)
                {
                    from.SendMessage(string.Format("You have exceeded the skill cap for this area in {0}.", skill.SkillName));
                    return false;
                }
            }

            if (from is PlayerMobile)
            {
                PlayerMobile pm = from as PlayerMobile;

                if (AntiMacroCode && UseAntiMacro[skill.Info.SkillID])
                    return pm.AntiMacroCheck(skill, obj);
            }
            return true;
        }

        public enum Stat { Str, Dex, Int }

        public static void Gain(Mobile from, Skill skill)
        {
            PlayerMobile pm = from as PlayerMobile;
            BaseCreature bc = from as BaseCreature;

            //Tamed Creatures No Longer Gain Stats & Skills This Way: Now Use Experience to Create Stat + Skill Scalars
            if (bc != null)
            {
                if (bc.Controlled && bc.ControlMaster is PlayerMobile)
                    return;
            }

            //Cannot Gain Skill while in Temp Statloss
            if (pm != null)
            {
                if (pm.IsInTempStatLoss || pm.Region is UOACZRegion)
                    return;
            }

            if (pm != null && pm.IsInMilitia)
            {
                if (skill.SkillName == SkillName.AnimalTaming || skill.SkillName == SkillName.Begging)
                {
                    pm.SendMessage(0x22, "Militia members may not pursue a career in animal taming or begging");
                    return;
                }
            }

            if (from.Player)
                if (((PlayerMobile)from).IsInArenaFight)
                    return;

            if (from.Region.IsPartOf(typeof(Regions.Jail)))
                return;

            if (from is BaseCreature && ((BaseCreature)from).IsDeadPet)
                return;

            if (skill.SkillName == SkillName.Focus && from is BaseCreature)
                return;

            int toGain = 1;
            
            if (skill.Base < skill.Cap && skill.Lock == SkillLock.Up)
            {
                if (skill.Base <= 10.0)
                    toGain = Utility.Random(4) + 1;

                Skills skills = from.Skills;

                if ((skills.Total / skills.Cap) >= Utility.RandomDouble())//( skills.Total >= skills.Cap )
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

                if ((skills.Total + toGain) <= skills.Cap)
                {
                    skill.BaseFixedPoint += toGain;

                    // flag LastSkillGain for SkillScroll reference -Nummmnut
                    if (pm != null && skill.Base < 100)
                    {
                        foreach (var s in PlayerMobile.SkillPool)
                        {
                            var playerSkill = pm.Skills[s];
                            if (playerSkill.SkillID == skill.SkillID)
                            {
                                //((PlayerMobile)from).LastSkillGain = skill;
                                ((PlayerMobile)from).addLastSkillGain(skill);
                                break;
                            }
                        }

                    }
                    // SkillScroll -end
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
                        {
                            ++from.RawStr;
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
                        }

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