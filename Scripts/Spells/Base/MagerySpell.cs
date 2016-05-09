using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Spells
{
    public abstract class MagerySpell : Spell
    {
        private static int[] m_ManaTable = new int[] { 4, 6, 9, 11, 14, 20, 40, 50 };

        public MagerySpell(Mobile caster, Item scroll, SpellInfo info): base(caster, scroll, info)
        {
        }

        public abstract SpellCircle Circle { get; }

        public override bool ConsumeReagents()
        {
            if (base.ConsumeReagents())
                return true;

            return false;
        }
        
        public override void GetCastSkills(out double min, out double max)
        {
            min = GetCastMinSkills((int)Circle, Scroll != null);
            max = GetCastMaxSkills((int)Circle, Scroll != null);

            //1st: 0-30
            //2nd: 20-40
            //3rd: 30-50
            //4th: 40-60
            //5th: 50-70
            //6th: 60-80
            //7th: 70-90
            //8th: 80-100
        }

        public static double GetCastMinSkills(int circle, bool scroll)
        {
            double firstCircleBonus = 0;

            if (circle == 0)
                firstCircleBonus = 10;

            if (scroll)
                circle -= 2;

            return (((double)circle + 1) * 10) - firstCircleBonus;
        }

        public static double GetCastMaxSkills(int circle, bool scroll)
        {
            double firstCircleBonus = 0;
            
            if (scroll)
                circle -= 2;

            return (((double)circle + 1) * 10) + 20;
        }

        public static string GetSpellName(Type type)
        {
            string name = "";

            #region Spells

            switch (type.Name)
            {
                case "ClumsyScroll": name = "Clumsy"; break;
                case "CreateFoodScroll": name = "Create Food"; break;
                case "FeeblemindScroll": name = "Feeblemind"; break;
                case "HealScroll": name = "Heal"; break;
                case "MagicArrowScroll": name = "Magic Arrow"; break;
                case "NightSightScroll": name = "Night Sight"; break;
                case "ReactiveArmorScroll": name = "Reactive Armor"; break;
                case "WeakenScroll": name = "Weaken"; break;

                case "AgilityScroll": name = "Agility"; break;
                case "CunningScroll": name = "Cunning"; break;
                case "CureScroll": name = "Cure"; break;
                case "HarmScroll": name = "Harm"; break;
                case "MagicTrapScroll": name = "Magic Trap"; break;
                case "ProtectionScroll": name = "Protection"; break;
                case "MagicUnTrapScroll": name = "Unlock"; break;
                case "StrengthScroll": name = "Strength"; break;

                case "BlessScroll": name = "Bless"; break;
                case "FireballScroll": name = "Fireball"; break;
                case "MagicLockScroll": name = "Magic Lock"; break;
                case "PoisonScroll": name = "Poison"; break;
                case "TelekinisisScroll": name = "Telekinisis"; break;
                case "TeleportScroll": name = "Teleport"; break;
                case "UnlockScroll": name = "Unlock"; break;
                case "WallOfStoneScroll": name = "Wall of Stone"; break;

                case "ArchCureScroll": name = "Arch Cure"; break;
                case "ArchProtectionScroll": name = "Arch Protection"; break;
                case "CurseScroll": name = "Curse"; break;
                case "FireFieldScroll": name = "Fire Field"; break;
                case "GreaterHealScroll": name = "Greater Heal"; break;
                case "LightningScroll": name = "Lightning"; break;
                case "ManaDrainScroll": name = "Mana Drain"; break;
                case "RecallScroll": name = "Recall"; break;

                case "BladeSpiritsScroll": name = "Blade Spirits"; break;
                case "DispelFieldScroll": name = "Dispel Field"; break;
                case "IncognitoScroll": name = "Incognito"; break;
                case "MagicReflectScroll": name = "Magic Reflect"; break;
                case "MindBlastScroll": name = "Mind Blast"; break;
                case "ParalyzeScroll": name = "Paralyze"; break;
                case "PoisonFieldScroll": name = "Poison Field"; break;
                case "SummonCreatureScroll": name = "Summon Creature"; break;

                case "DispelScroll": name = "Blade Spirits"; break;
                case "EnergyBoltScroll": name = "Energy Bolt"; break;
                case "ExplosionScroll": name = "Explosion"; break;
                case "InvisibilityScroll": name = "Invisibility"; break;
                case "MarkScroll": name = "Mark"; break;
                case "MassCurseScroll": name = "Mass Curse"; break;
                case "ParalyzeFieldScroll": name = "Paralyze Field"; break;
                case "RevealScroll": name = "Reveal"; break;

                case "ChainLightningScroll": name = "Chain Lightning"; break;
                case "EnergyFieldScroll": name = "Energy Field"; break;
                case "FlamestrikeScroll": name = "Flamestrike"; break;
                case "GateTravelScroll": name = "Gate Travel"; break;
                case "ManaVampireScroll": name = "Mana Vampire"; break;
                case "MassDispelScroll": name = "Mass Dispel"; break;
                case "MeteorSwarmScroll": name = "Meteor Swarm"; break;
                case "PolymorphScroll": name = "Polymorph"; break;

                case "SummonAirElementalScroll": name = "Air Elemental"; break;
                case "SummonEarthElementalScroll": name = "Earth Elemental"; break;
                case "EarthquakeScroll": name = "Earthquake"; break;
                case "EnergyVortexScroll": name = "Energy Vortex"; break;
                case "SummonFireElementalScroll": name = "Fire Elemental"; break;
                case "ResurrectionScroll": name = "Resurrection"; break;
                case "SummonDaemonScroll": name = "Daemon"; break;
                case "SummonWaterElementalScroll": name = "Water Elemental"; break;
            }

            #endregion

            return name;
        }

        public override int GetMana()
        {
            if (Scroll is BaseWand)
                return 0;

            return m_ManaTable[(int)Circle];
        }

        public override double GetResistSkill(Mobile m)
        {
            int maxSkill = (1 + (int)Circle) * 10;

            maxSkill += (1 + ((int)Circle / 6)) * 25;

            if (m.Skills[SkillName.MagicResist].Value < maxSkill)
                m.CheckSkill(SkillName.MagicResist, 0.0, 100.0, 1.0);

            return m.Skills[SkillName.MagicResist].Value;
        }

        public virtual bool CheckMagicResist(Mobile target)
        {
            double n = GetResistPercent(target);

            n /= 100.0;

            int maxSkill = (1 + (int)Circle) * 10;

            maxSkill += (1 + ((int)Circle / 6)) * 25;            
            
            bool monster_to_player = target.Player && !Caster.Player;

            if (monster_to_player)
                target.IncomingResistCheckFromMonster = true;

            if (target.Skills[SkillName.MagicResist].Value <= maxSkill)
                target.CheckSkill(SkillName.MagicResist, 0.0, 100.0, 1.0);

            if (monster_to_player)
                target.IncomingResistCheckFromMonster = false;

            return (n >= Utility.RandomDouble());
        }

        public virtual double GetResistPercentForCircle(Mobile target, SpellCircle circle)
        {
            double firstPercent = target.Skills[SkillName.MagicResist].Value / 5.0;
            double secondPercent = target.Skills[SkillName.MagicResist].Value - (((Caster.Skills[CastSkill].Value - 20.0) / 5.0) + (1 + (int)circle) * 5.0);

            return (firstPercent > secondPercent ? firstPercent : secondPercent) / 2.0;
        }

        public virtual double GetResistPercent(Mobile target)
        {
            return GetResistPercentForCircle(target, Circle);
        }

        public override TimeSpan GetCastDelay()
        {
            if (Scroll is BaseWand)
                return TimeSpan.Zero;

            return TimeSpan.FromSeconds(0.5 + (0.25 * (int)Circle));           
        }

        public override TimeSpan CastDelayBase
        {
            get
            {
                return TimeSpan.FromSeconds((3 + (int)Circle) * CastDelaySecondsPerTick);
            }
        }
    }
}
