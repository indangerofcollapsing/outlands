using System;
using Server.Targeting;
using Server.Network;

using Server.Mobiles;
using Server.Items;
using Server.Custom;

namespace Server.Spells.Second
{
	public class CureSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Cure", "An Nox",
				212,
				9061,
				Reagent.Garlic,
				Reagent.Ginseng
			);

		public override SpellCircle Circle { get { return SpellCircle.Second; } }

		public CureSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override bool CheckCast()
		{
			if ( Engines.ConPVP.DuelContext.CheckSuddenDeath( Caster ) )
			{
				Caster.SendMessage( 0x22, "You cannot cast this spell when in sudden death." );
				return false;
			}

			return base.CheckCast();
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
            else if (CheckBSequence(m))
            {
                SpellHelper.Turn(Caster, m);
                Poison p = m.Poison;

                if (p != null)
                {
                    double chanceToCure = (Caster.Skills[SkillName.Magery].Value * 0.75) + (110.0 - (p.Level + 1) * 33.0);
                    chanceToCure /= 100;

                    if (chanceToCure > Utility.RandomDouble())
                    {
                        if (m.CurePoison(Caster))
                        {
                            if (Caster != m)                            
                                Caster.SendLocalizedMessage(1010058); // You have cured the target of all poisons!
                            
                            m.SendLocalizedMessage(1010059); // You have been cured of all poisons.
                        }
                    }
                    else
                    {
                        m.SendLocalizedMessage(1010060); // You have failed to cure your target!
                    }
                }

                int spellHue = PlayerEnhancementPersistance.GetSpellHueFor(Caster, HueableSpell.Cure);    

                m.FixedParticles(0x373A, 10, 15, 5012, spellHue, 0, EffectLayer.Waist);
                m.PlaySound(0x1E0);
            }

			FinishSequence();
		}

		public class InternalTarget : Target
		{
			private CureSpell m_Owner;

			public InternalTarget( CureSpell owner ) : base( 12, false, TargetFlags.Beneficial )
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