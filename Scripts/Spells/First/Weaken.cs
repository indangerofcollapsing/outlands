using System;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Custom;

namespace Server.Spells.First
{
	public class WeakenSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Weaken", "Des Mani",
				212,
				9031,
				Reagent.Garlic,
				Reagent.Nightshade
			);

		public override SpellCircle Circle { get { return SpellCircle.First; } }

		public WeakenSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
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

				SpellHelper.AddStatCurse( Caster, m, StatType.Str );

                //Commented on IPY
				//if ( m.Spell != null )
					//m.Spell.OnCasterHurt();

                int spellHue = PlayerEnhancementPersistance.GetSpellHueFor(Caster, HueableSpell.Weaken); 

				m.Paralyzed = false;

                m.FixedParticles(0x3779, 10, 15, 5002, spellHue, 0, EffectLayer.Head);
				m.PlaySound( 0x1E6 );
               
				/*	IPY
				int percentage = (int)(SpellHelper.GetOffsetScalar( Caster, m, true )*100);
				TimeSpan length = SpellHelper.GetDuration( Caster, m );

				BuffInfo.AddBuff( m, new BuffInfo( BuffIcon.Weaken, 1075837, length, m, percentage.ToString() ) );

				HarmfulSpell( m );
				*/
			}

			FinishSequence();
		}

		public class InternalTarget : Target
		{
			private WeakenSpell m_Owner;

			public InternalTarget( WeakenSpell owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
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