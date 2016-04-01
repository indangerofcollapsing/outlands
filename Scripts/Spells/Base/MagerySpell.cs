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
            int circle = (int)Circle;

            double firstCircleBonus = 0;

            if (circle == 0)
                firstCircleBonus = 10;

            if (Scroll != null)
                circle -= 2;

            min = (((double)circle + 1) * 10) - firstCircleBonus;
            max = (((double)circle + 1) * 10) + 20;

            //1st: 0-30
            //2nd: 20-40
            //3rd: 30-50
            //4th: 40-60
            //5th: 50-70
            //6th: 60-80
            //7th: 70-90
            //8th: 80-100
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

        public virtual bool CheckResisted(Mobile target)
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
