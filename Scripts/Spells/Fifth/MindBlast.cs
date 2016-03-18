using System;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Custom;

namespace Server.Spells.Fifth
{
	public class MindBlastSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
			"Mind Blast", "Por Corp Wis",
			218,
			9032,
			Reagent.BlackPearl,
			Reagent.MandrakeRoot,
			Reagent.Nightshade,
			Reagent.SulfurousAsh
			);

		public override SpellCircle Circle { get { return SpellCircle.Fifth; } }

		public MindBlastSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
			if ( Core.AOS )
				m_Info.LeftHandEffect = m_Info.RightHandEffect = 9002;
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

		private void AosDelay_Callback( object state )
		{
			object[] states = (object[])state;
			Mobile caster = (Mobile)states[0];
			Mobile target = (Mobile)states[1];
			Mobile defender = (Mobile)states[2];
			int damage = (int)states[3];

			if ( caster.HarmfulCheck( defender ) )
			{
				SpellHelper.Damage( this, target, Utility.RandomMinMax( damage, damage + 4 ), 0, 0, 100, 0, 0 );

				target.FixedParticles( 0x374A, 10, 15, 5038, 1181, 2, EffectLayer.Head );
				target.PlaySound( 0x213 );
			}
		}

		public override bool DelayedDamage{ get{ return false; } } // IPY

		public void Target( Mobile m )
		{
            if (!Caster.CanSee(m) || m.Hidden)
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}

			else if ( CheckHSequence( m ) )
			{
				Mobile from = Caster, target = m;

				SpellHelper.Turn( from, target );

				SpellHelper.CheckReflect( (int)this.Circle, ref from, ref target );

				// Algorithm: (highestStat - lowestStat) / 2 [- 50% if resisted]

				int damage = Utility.RandomMinMax( 11, 13 ) + ( Caster.Int - target.Int ) / 10;//less damage

                if (from is BaseCreature)
                    damage = (int)((double)damage * .75);

				if ( damage > 35 )
					damage = 35;

				else if ( damage  < 15 )
					damage = 15;

                if (m.Region is UOACZRegion)
                    damage = Utility.RandomMinMax(9, 18);

				if ( CheckResisted( target ) )
				{
					damage /= 2;
					target.SendLocalizedMessage( 501783 ); // You feel yourself resisting magical energy.
				}

                damage = (int)(damage * GetDamageScalar(m));

                bool enhancedSpellcast = SpellHelper.IsEnhancedSpell(Caster, m, EnhancedSpellbookType.Warlock, true, true);
                Boolean chargedSpellcast = SpellHelper.IsChargedSpell(Caster, m, true, Scroll != null);
                Boolean isTamedTarget = SpellHelper.IsTamedTarget(Caster, m);

                if (enhancedSpellcast)
                {
                    if (isTamedTarget)
                        damage = (int)((double)damage * SpellHelper.enhancedTamedCreatureMultiplier);

                    else
                        damage = (int)((double)damage * SpellHelper.enhancedMultiplier);
                }

                int spellHue = PlayerEnhancementPersistance.GetSpellHueFor(Caster, HueableSpell.MindBlast);

                if (chargedSpellcast)
                {
                    if (isTamedTarget)
                        damage = (int)((double)damage * SpellHelper.chargedTamedCreatureMultiplier);

                    else
                        damage = (int)((double)damage * SpellHelper.chargedMultiplier);


                    from.FixedParticles(0x374A, 10, 30, 2038, spellHue, 0, EffectLayer.Head);

                    target.FixedParticles(0x374A, 10, 30, 5038, spellHue, 0, EffectLayer.Head);
                    target.PlaySound(0x213);
                }

                else
                {
                    from.FixedParticles(0x374A, 10, 15, 2038, spellHue, 0, EffectLayer.Head);

                    target.FixedParticles(0x374A, 10, 15, 5038, spellHue, 0, EffectLayer.Head);
                    target.PlaySound(0x213);
                }				

				SpellHelper.Damage( this, target, damage, 0, 0, 100, 0, 0 );
			}

			FinishSequence();
		}

		public override double GetSlayerDamageScalar( Mobile target )
		{
			return 1.0; //This spell isn't affected by slayer spellbooks
		}

		private class InternalTarget : Target
		{
			private MindBlastSpell m_Owner;

			public InternalTarget( MindBlastSpell owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
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