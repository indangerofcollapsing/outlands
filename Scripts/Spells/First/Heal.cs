using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;

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
                    this.Target(casterCreature.SpellTarget);                
            }

            else            
                Caster.Target = new InternalTarget(this);            
		}

		public void Target( Mobile target )
		{
            if (!Caster.CanSee(target) || target.Hidden)            
                Caster.SendLocalizedMessage(500237); // Target can not be seen.
            
			else if ( target.IsDeadBondedPet )			
				Caster.SendLocalizedMessage( 1060177 ); // You cannot heal a creature that is already dead!
			
			else if ( target is BaseCreature && ((BaseCreature)target).IsAnimatedDead )			
				Caster.SendLocalizedMessage( 1061654 ); // You cannot heal that which is not alive.
			
			else if ( target is Golem )			
				Caster.LocalOverheadMessage( MessageType.Regular, 0x3B2, 500951 ); // You cannot heal that.
			
			else if ( CheckBSequence( target ) )
            {
                SpellHelper.Turn(Caster, target);
                
				int healAmount = Utility.RandomMinMax(8, 12);

                double mageryScalar = .5 + (.5 * (Caster.Skills[SkillName.Magery].Value / 100));
                double diminishedEffectScalar = 1.0;
                double poisonScalar = 1.0;

                if (Caster.SpellHealWindowEnd > DateTime.UtcNow)
                {
                    diminishedEffectScalar -= (SpellHelper.SpellHealScalarAdjustmentPerCount * (double)Caster.LastSpellCount);

                    if (diminishedEffectScalar < 0)
                        diminishedEffectScalar = 0;

                    Caster.LastSpellCount++;
                    Caster.SpellHealWindowEnd = DateTime.UtcNow + SpellHelper.SpellHealWindowDuration;
                }

                else
                {
                    Caster.LastSpellCount = 1;
                    Caster.SpellHealWindowEnd = DateTime.UtcNow + SpellHelper.SpellHealWindowDuration;
                }

                if (target.Poisoned)
                    poisonScalar = SpellHelper.HealThroughPoisonScalar;

                healAmount = (int)(Math.Round((double)healAmount * mageryScalar * diminishedEffectScalar * poisonScalar));

                if (healAmount < 1)
                    healAmount = 1;

                int spellHue = 0;            

                target.Heal(healAmount);

                target.FixedParticles(0x376A, 9, 32, 5005, spellHue, 0, EffectLayer.Waist);
                target.FixedParticles(0x376A, 9, 32, 5005, spellHue, 0, EffectLayer.Waist);

                target.PlaySound(0x1F2);

                target.NextSpellTime = DateTime.UtcNow + TimeSpan.FromMilliseconds(250);
            }

			FinishSequence();
		}

		public class InternalTarget : Target
		{
			private HealSpell m_Owner;

			public InternalTarget( HealSpell owner ) : base( 12, false, TargetFlags.Beneficial )
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