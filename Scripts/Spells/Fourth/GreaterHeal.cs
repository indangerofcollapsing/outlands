using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;

using Server.Items;
using Server.Custom;
using Server.Achievements;

namespace Server.Spells.Fourth
{
	public class GreaterHealSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Greater Heal", "In Vas Mani",
				204,
				9061,
				Reagent.Garlic,
				Reagent.Ginseng,
				Reagent.MandrakeRoot,
				Reagent.SpidersSilk
			);

		public override SpellCircle Circle { get { return SpellCircle.Fourth; } }

		public GreaterHealSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
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
			else if ( m is BaseCreature && ((BaseCreature)m).IsAnimatedDead )
			{
				Caster.SendLocalizedMessage( 1061654 ); // You cannot heal that which is not alive.
			}
			else if ( m.IsDeadBondedPet )
			{
				Caster.SendLocalizedMessage( 1060177 ); // You cannot heal a creature that is already dead!
			}
			else if ( m is Golem )
			{
				Caster.LocalOverheadMessage( MessageType.Regular, 0x3B2, 500951 ); // You cannot heal that.
			}
			
            else if ( CheckBSequence( m ) )
			{
				SpellHelper.Turn( Caster, m );

				// Algorithm: (40% of magery) + (1-10)

				int toHeal = (int)(Caster.Skills[SkillName.Magery].Value * 0.35);
				toHeal += Utility.Random( 1, 10 );

				//m.Heal( toHeal, Caster );
				//Check this method to see if it is like IPY
                SpellHelper.Heal( toHeal, m, Caster );

                // IPY ACHIEVEMENT (heal newbie)
                if (Caster != m && 3000 > m.SkillsTotal && m.Player && Caster.Player)
                    AchievementSystem.Instance.TickProgress(Caster, AchievementTriggers.Trigger_HealPlayerUnder300Skill);
                // IPY ACHIEVEMENT

                int spellHue = 0; //PlayerEnhancementPersistance.GetSpellHueFor(Caster, HueableSpell.GreaterHeal);

                m.FixedParticles(0x376A, 9, 32, 5030, spellHue, 0, EffectLayer.Waist);
				m.PlaySound( 0x202 );
			}

			FinishSequence();
		}

		public class InternalTarget : Target
		{
			private GreaterHealSpell m_Owner;

			public InternalTarget( GreaterHealSpell owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Beneficial )
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