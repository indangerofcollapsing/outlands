using System;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using Server.Multis;
using Server.Items;
using Server.Custom;

namespace Server.Spells.Fifth
{
    public class BladeSpiritsSpell : MagerySpell
    {
        private static SpellInfo m_Info = new SpellInfo(
                "Blade Spirits", "In Jux Hur Ylem",
                266,
                9040,
                false,
                Reagent.BlackPearl,
                Reagent.MandrakeRoot,
                Reagent.Nightshade
            );

        public override SpellCircle Circle { get { return SpellCircle.Fifth; } }

        public BladeSpiritsSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override TimeSpan CastDelayBase
        {            
            get { return base.CastDelayBase + TimeSpan.FromSeconds(28); }
        }

        public override double CastDelayFastScalar
        {
            get { return 5; }
        }

        public override TimeSpan GetCastDelay()
        {
            if (Core.AOS)
                return TimeSpan.FromTicks(base.GetCastDelay().Ticks * ((Core.SE) ? 3 : 5));

            return base.GetCastDelay() + TimeSpan.FromSeconds(6.0);
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;

            if ((Caster.Followers + 1) > Caster.FollowersMax)
            {
                Caster.SendLocalizedMessage(1049645); // You have too many followers to summon that creature.
                return false;
            }

            return true;
        }

        public override void OnCast()
        {
            BaseCreature casterCreature = Caster as BaseCreature;

            if (casterCreature != null)
            {
                if (casterCreature.SpellTarget != null)
                {
                    this.Target(casterCreature.SpellTarget);
                }
            }

            else
            {
                Caster.Target = new InternalTarget(this);
            }
        }

        public void Target(IPoint3D p)
        {
            Map map = Caster.Map;

            SpellHelper.GetSurfaceTop(ref p);

            if (map == null || !map.CanSpawnMobile(p.X, p.Y, p.Z) || BaseBoat.FindBoatAt(p, map) != null)                           
                Caster.SendLocalizedMessage(501942); // That location is blocked.  

            else if (SpellHelper.CheckTown(p, Caster) && CheckSequence())
            {
                bool enhancedSpellcast = SpellHelper.IsEnhancedSpell(Caster, null, EnhancedSpellbookType.Summoner, false, true);

                BaseCreature summon = new BladeSpirits();

                summon.StoreBaseSummonValues();

                double duration = 2.0 * Caster.Skills[SkillName.Magery].Value;

                if (enhancedSpellcast)
                {
                    duration *= SpellHelper.EnhancedSummonDurationMultiplier;

                    summon.DamageMin = (int)((double)summon.DamageMin * SpellHelper.EnhancedSummonDamageMultiplier);
                    summon.DamageMax = (int)((double)summon.DamageMax * SpellHelper.EnhancedSummonDamageMultiplier);

                    summon.SetHitsMax((int)((double)summon.HitsMax * SpellHelper.EnhancedSummonHitPointsMultiplier));
                    summon.Hits = summon.HitsMax;
                }

                summon.SetDispelResistance(Caster, enhancedSpellcast, 0);

                int spellHue = PlayerEnhancementPersistance.GetSpellHueFor(Caster, HueableSpell.BladeSpirits);

                summon.Hue = spellHue;

                BaseCreature.Summon(summon, false, Caster, new Point3D(p), 0x212, TimeSpan.FromSeconds(duration));
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private BladeSpiritsSpell m_Owner;

            public InternalTarget(BladeSpiritsSpell owner)
                : base(Core.ML ? 10 : 12, true, TargetFlags.None)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is IPoint3D)
                    m_Owner.Target((IPoint3D)o);
            }

            protected override void OnTargetOutOfLOS(Mobile from, object o)
            {
                from.SendLocalizedMessage(501943); // Target cannot be seen. Try again.
                from.Target = new InternalTarget(m_Owner);
                from.Target.BeginTimeout(from, TimeoutTime - DateTime.UtcNow);
                m_Owner = null;
            }

            protected override void OnTargetFinish(Mobile from)
            {
                if (m_Owner != null)
                    m_Owner.FinishSequence();
            }
        }
    }
}