using System;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Custom;

namespace Server.Spells.Third
{
	public class FireballSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Fireball", "Vas Flam",
				203,
				9041,
				Reagent.BlackPearl
			);

		public override SpellCircle Circle { get { return SpellCircle.Third; } }

		public FireballSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
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
				Mobile source = Caster;

				SpellHelper.Turn( source, m );

				SpellHelper.CheckReflect( (int)this.Circle, ref source, ref m );

				double damage;

				damage = Utility.Random( 5, 5 );

                if (Caster is PlayerMobile && !(m is PlayerMobile) && !(m is Server.Custom.Townsystem.BaseFactionGuard))
                    damage += 6;

                if (m.Region is UOACZRegion)
                    damage = Utility.RandomMinMax(5, 10);

				if ( CheckResisted( m ) )
				{
					damage *= 0.50;

					m.SendLocalizedMessage( 501783 ); // You feel yourself resisting magical energy.
				}

				damage *= GetDamageScalar( m );                   
				
                bool enhancedSpellcast = SpellHelper.IsEnhancedSpell(Caster, m, EnhancedSpellbookType.Fire, true, true);
                Boolean chargedSpellcast = SpellHelper.IsChargedSpell(Caster, m, true, Scroll != null);
                Boolean isTamedTarget = SpellHelper.IsTamedTarget(Caster, m);

                int spellHue = PlayerEnhancementPersistance.GetSpellHueFor(Caster, HueableSpell.Fireball);

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

                    source.MovingParticles(m, 0x36D4, 4, 0, false, true, spellHue, 0, 9502, 4019, 0x160, 0);
                    source.MovingParticles(m, 0x36D4, 10, 0, false, true, spellHue, 0, 9502, 4019, 0x160, 0);

                    source.PlaySound(0x44B);
                }

                else
                {
                    source.MovingParticles(m, 0x36D4, 7, 0, false, true, spellHue, 0, 9502, 4019, 0x160, 0);

                    source.PlaySound(0x44B);
                }   			

				double chance = 0.25;
				if (Caster.CanBeginAction(typeof(FireballSpell)))
				{
					chance = 1.0;
					Caster.BeginAction(typeof(FireballSpell));
					Timer.DelayCall(TimeSpan.FromSeconds(5), delegate { Caster.EndAction(typeof(FireballSpell)); });
				}               

				SpellHelper.DamageChanceDisturb( this, m, damage, chance );
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private FireballSpell m_Owner;

			public InternalTarget( FireballSpell owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is Mobile )
					m_Owner.Target( (Mobile)o );
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}