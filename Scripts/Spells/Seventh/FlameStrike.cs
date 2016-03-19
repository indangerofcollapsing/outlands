using System;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Custom;

namespace Server.Spells.Seventh
{
	public class FlameStrikeSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Flame Strike", "Kal Vas Flam",
				245,
				9042,
				Reagent.SpidersSilk,
				Reagent.SulfurousAsh
			);

		public override SpellCircle Circle { get { return SpellCircle.Seventh; } }

		public FlameStrikeSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
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

		public override bool DelayedDamage{ get{ return true; } }

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
				
				damage = Utility.Random( 28, 12 );

                if (Caster is PlayerMobile && !(m is PlayerMobile))
                    damage += 10;

                if (m.Region is UOACZRegion)
                    damage = Utility.RandomMinMax(15, 30);

				if ( CheckResisted( m ) )
				{
					damage *= 0.6;

					m.SendLocalizedMessage( 501783 ); // You feel yourself resisting magical energy.
				}               

				damage *= GetDamageScalar( m );		
		
                bool enhancedSpellcast = SpellHelper.IsEnhancedSpell(Caster, m, EnhancedSpellbookType.Fire, true, true);
                Boolean chargedSpellcast = SpellHelper.IsChargedSpell(Caster, m, true, Scroll != null);
                Boolean isTamedTarget = SpellHelper.IsTamedTarget(Caster, m);

                int spellHue = PlayerEnhancementPersistance.GetSpellHueFor(Caster, HueableSpell.Flamestrike);

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

                    m.FixedParticles(0x3709, 10, 60, 5052, spellHue, 0, EffectLayer.LeftFoot);
                    m.PlaySound(0x208); 
                }

                else
                {
                    m.FixedParticles(0x3709, 10, 30, 5052, spellHue, 0, EffectLayer.LeftFoot);
                    m.PlaySound(0x208); 
                }

				SpellHelper.Damage( this, m, damage, 0, 100, 0, 0, 0 );
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private FlameStrikeSpell m_Owner;

			public InternalTarget( FlameStrikeSpell owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
			{
				m_Owner = owner;
			}

            protected override void OnTarget(Mobile from, object o) {
                if (o is Mobile) {
                    m_Owner.Target((Mobile)o);
                }
            }

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}