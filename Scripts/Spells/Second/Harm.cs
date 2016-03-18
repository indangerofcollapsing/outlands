using System;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using System.Collections.Generic;
using Server.Items;
using Server.Custom;

namespace Server.Spells.Second
{
	public class HarmSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Harm", "An Mani",
				212,
				Core.AOS ? 9001 : 9041,
				Reagent.Nightshade,
				Reagent.SpidersSilk
			);

		public override SpellCircle Circle { get { return SpellCircle.Second; } }

		public HarmSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
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

		public override bool DelayedDamage{ get{ return false; } }


		public override double GetSlayerDamageScalar( Mobile target )
		{
			return 1.0; //This spell isn't affected by slayer spellbooks
		}

		public void Target( Mobile m )
		{
            if (!Caster.CanSee(m) || m.Hidden)
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}

			else if ( CheckHSequence( m ) )
			{
				SpellHelper.Turn( Caster, m );

				SpellHelper.CheckReflect( (int)this.Circle, Caster, ref m );

				double damage;

				damage = Utility.Random( 3, 5 );

                if (Caster is PlayerMobile && !(m is PlayerMobile) && !(m is Server.Custom.Townsystem.BaseFactionGuard))
                    damage += 4;

                if (m.Region is UOACZRegion)
                    damage = Utility.RandomMinMax(3, 6);
                    
				if ( CheckResisted( m ) )
				{
					damage /= 2;

					m.SendLocalizedMessage( 501783 ); // You feel yourself resisting magical energy.
				}

				damage *= GetDamageScalar( m );				

                bool enhancedSpellcast = SpellHelper.IsEnhancedSpell(Caster, m, EnhancedSpellbookType.Energy, true, true);
                Boolean chargedSpellcast = SpellHelper.IsChargedSpell(Caster, m, true, Scroll != null);
                Boolean isTamedTarget = SpellHelper.IsTamedTarget(Caster, m);

                int spellHue = PlayerEnhancementPersistance.GetSpellHueFor(Caster, HueableSpell.Harm);

                if (enhancedSpellcast)
                {
                    if (isTamedTarget)
                        damage *= SpellHelper.enhancedTamedCreatureMultiplier;

                    else
                        damage *= SpellHelper.enhancedMultiplier;
                }

                if (chargedSpellcast)
                {
                    if (isTamedTarget)
                        damage *= SpellHelper.chargedTamedCreatureMultiplier;

                    else
                        damage *= SpellHelper.chargedMultiplier;

                    m.FixedParticles(0x374A, 10, 30, 5013, spellHue, 0, EffectLayer.Waist);
                    m.PlaySound(0x1F1);
                }

                else
                {
                    m.FixedParticles(0x374A, 10, 15, 5013, spellHue, 0, EffectLayer.Waist);
                    m.PlaySound(0x1F1);
                }

                double chance = 0.25;

                if (Caster.CanBeginAction(typeof(HarmSpell)))
                {
                    chance = 1.0;
                    Caster.BeginAction(typeof(HarmSpell));
                    Timer.DelayCall(TimeSpan.FromSeconds(5), delegate { Caster.EndAction(typeof(HarmSpell)); });
                }

                if (m is BaseCreature)
                    ((BaseCreature)m).OnDamagedBySpell(this.Caster);

                SpellHelper.DamageChanceDisturb(this, m, damage, chance);
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private HarmSpell m_Owner;

			public InternalTarget( HarmSpell owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is Mobile )
				{
					m_Owner.Target( (Mobile)o );
				}
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}