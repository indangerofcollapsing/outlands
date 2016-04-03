using System;
using Server.Targeting;
using Server.Network;
using Server.Achievements;
using Server.Mobiles;
using Server.Items;
using Server.Custom;

namespace Server.Spells.Third
{
	public class BlessSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Bless", "Rel Sanct",
				203,
				9061,
				Reagent.Garlic,
				Reagent.MandrakeRoot
			);

		public override SpellCircle Circle { get { return SpellCircle.Third; } }

		public BlessSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
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

			else if ( CheckBSequence( m ) )
			{
				SpellHelper.Turn( Caster, m );

				SpellHelper.AddStatBonus( Caster, m, StatType.Str ); SpellHelper.DisableSkillCheck = true;
				SpellHelper.AddStatBonus( Caster, m, StatType.Dex );
				SpellHelper.AddStatBonus( Caster, m, StatType.Int ); SpellHelper.DisableSkillCheck = false;
                
                bool enhancedSpellcast = SpellHelper.IsEnhancedSpell(Caster, m, EnhancedSpellbookType.Wizard, false, true);

                int spellHue = PlayerEnhancementPersistance.GetSpellHueFor(Caster, HueableSpell.Bless);

                if (enhancedSpellcast)
                {
                    m.FixedParticles(0x373A, 10, 30, 5018, spellHue, 0, EffectLayer.Waist);
                    m.PlaySound(0x1EA);
                }

                else
                {
                    m.FixedParticles(0x373A, 10, 15, 5018, spellHue, 0, EffectLayer.Waist);
                    m.PlaySound(0x1EA);
                }					

                // IPY ACHIEVEMENT (bless newbie)
                if (Caster != m && 3000 > m.SkillsTotal && m.Player && Caster.Player)
                    AchievementSystem.Instance.TickProgress(Caster, AchievementTriggers.Trigger_BlessPlayerUnder300Skill);
                // IPY ACHIEVEMENT
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private BlessSpell m_Owner;

			public InternalTarget( BlessSpell owner ) : base( 12, false, TargetFlags.Beneficial )
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