using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Achievements;
using Server.Items;
using Server.Custom;

namespace Server.Spells.First
{
	public class HealSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Heal", "In Mani",
				224,
				9061,
				Reagent.Garlic,
				Reagent.Ginseng,
				Reagent.SpidersSilk
			);

		public override SpellCircle Circle { get { return SpellCircle.First; } }

		public override TimeSpan GetCastDelay() 
		{       			
			return TimeSpan.FromSeconds( 0.5 ); 
		}

		public HealSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
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
                Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
			else if ( m.IsDeadBondedPet )
			{
				Caster.SendLocalizedMessage( 1060177 ); // You cannot heal a creature that is already dead!
			}
			else if ( m is BaseCreature && ((BaseCreature)m).IsAnimatedDead )
			{
				Caster.SendLocalizedMessage( 1061654 ); // You cannot heal that which is not alive.
			}
			else if ( m is Golem )
			{
				Caster.LocalOverheadMessage( MessageType.Regular, 0x3B2, 500951 ); // You cannot heal that.
			}
			else if ( CheckBSequence( m ) )
            {
                SpellHelper.Turn(Caster, m);

                // Algorithm: (10% of magery) + (1-5)

				int toHeal = 0;
				if (SpellHelper.SPELLS_USE_IPY3_STYLE_DISRUPTS_AND_HEALS)
				{
					// ipy3
					toHeal = (int)(Caster.Skills[SkillName.Magery].Value * 0.06); // was 0.08 for a 9 - 11 range
					toHeal += Utility.Random(1, 3); // 7 - 9
				}
				else
				{
					// ipy2 heavily nerfed
					toHeal = (int)(Caster.Skills[SkillName.Magery].Value * 0.06);
					toHeal += Utility.Random(1, 3); // 7 - 9
				}

                int spellHue = 0; // PlayerEnhancementPersistance.GetSpellHueFor(Caster, HueableSpell.Heal);                

                m.Heal(toHeal);

                m.FixedParticles(0x376A, 9, 32, 5005, spellHue, 0, EffectLayer.Waist);
                m.FixedParticles(0x376A, 9, 32, 5005, spellHue, 0, EffectLayer.Waist);

                m.PlaySound(0x1F2);

                m.NextSpellTime = DateTime.UtcNow + TimeSpan.FromMilliseconds(250);

                // IPY ACHIEVEMENT (heal newbie)
                if (Caster != m && 3000 > m.SkillsTotal && m.Player && Caster.Player)
                    AchievementSystem.Instance.TickProgress(Caster, AchievementTriggers.Trigger_HealPlayerUnder300Skill);
                // IPY ACHIEVEMENT
            }

			FinishSequence();
		}

		public class InternalTarget : Target
		{
			private HealSpell m_Owner;

			public InternalTarget( HealSpell owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Beneficial )
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