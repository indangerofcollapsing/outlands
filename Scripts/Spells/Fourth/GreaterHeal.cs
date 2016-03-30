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
                    this.Target(casterCreature.SpellTarget);                
            }

            else            
                Caster.Target = new InternalTarget(this);            
		}

		public void Target( Mobile target )
		{
            if (!Caster.CanSee(target) || target.Hidden)			
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			
			else if ( target is BaseCreature && ((BaseCreature)target).IsAnimatedDead )			
				Caster.SendLocalizedMessage( 1061654 ); // You cannot heal that which is not alive.
			
			else if ( target.IsDeadBondedPet )			
				Caster.SendLocalizedMessage( 1060177 ); // You cannot heal a creature that is already dead!
			
			else if ( target is Golem )			
				Caster.LocalOverheadMessage( MessageType.Regular, 0x3B2, 500951 ); // You cannot heal that.			
			
            else if ( CheckBSequence( target ) )
			{
				SpellHelper.Turn( Caster, target );

                int healAmount = Utility.RandomMinMax(35, 45);
                double mageryScalar = .5 + (.5 * (Caster.Skills[SkillName.Magery].Value / 100));
                double poisonScalar = 1.0;

                if (target.Poisoned)
                    poisonScalar = SpellHelper.HealThroughPoisonScalar;

                healAmount = (int)(Math.Round((double)healAmount * mageryScalar * poisonScalar));

                if (healAmount < 1)
                    healAmount = 1;

                SpellHelper.Heal(healAmount, target, Caster);

                // IPY ACHIEVEMENT (heal newbie)
                if (Caster != target && 3000 > target.SkillsTotal && target.Player && Caster.Player)
                    AchievementSystem.Instance.TickProgress(Caster, AchievementTriggers.Trigger_HealPlayerUnder300Skill);
                // IPY ACHIEVEMENT

                int spellHue = 0;

                target.FixedParticles(0x376A, 9, 32, 5030, spellHue, 0, EffectLayer.Waist);
				target.PlaySound( 0x202 );
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
					m_Owner.Target( (Mobile)o );				
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}