using System;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Custom;

namespace Server.Spells.First
{
	public class MagicArrowSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Magic Arrow", "In Por Ylem",
				212,
				9041,
				Reagent.SulfurousAsh
			);

		public override SpellCircle Circle { get { return SpellCircle.First; } }

		public MagicArrowSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
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

		public void Target( Mobile mobile )
		{
            if (!Caster.CanSee(mobile) || mobile.Hidden)		
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			
			else if ( CheckHSequence( mobile ) )
			{
				Mobile source = Caster;

				SpellHelper.Turn( source, mobile );
				SpellHelper.CheckReflect( (int)this.Circle, ref source, ref mobile );				       

				double damage = (double)Utility.RandomMinMax(3, 5);
                double damageBonus = 0;

                CheckMagicResist(mobile);	

                bool enhancedSpellcast = SpellHelper.IsEnhancedSpell(Caster, mobile, EnhancedSpellbookType.Fire, true, true);
                Boolean chargedSpellcast = SpellHelper.IsChargedSpell(Caster, mobile, true, Scroll != null);
                Boolean isTamedTarget = SpellHelper.IsTamedTarget(Caster, mobile);

                int spellHue = PlayerEnhancementPersistance.GetSpellHueFor(Caster, HueableSpell.MagicArrow);                

                if (enhancedSpellcast)
                {
                    if (isTamedTarget)
                        damageBonus += SpellHelper.EnhancedSpellTamedCreatureBonus;

                    else
                        damageBonus += SpellHelper.EnhancedSpellBonus;
                }

                if (chargedSpellcast)
                {
                    if (isTamedTarget)
                        damageBonus += SpellHelper.ChargedSpellTamedCreatureBonus;

                    else
                        damageBonus += SpellHelper.ChargedSpellBonus;

                    source.MovingParticles(mobile, 0x36E4, 3, 0, false, true, spellHue, 0, 3006, 4006, 0, 0);
                    source.MovingParticles(mobile, 0x36E4, 7, 0, false, true, spellHue, 0, 3006, 4006, 0, 0);

                    source.PlaySound(0x1E5);
                }

                else
                {
                    source.MovingParticles(mobile, 0x36E4, 5, 0, false, true, spellHue, 0, 3006, 4006, 0, 0);
                    source.PlaySound(0x1E5);
                }

                damage *= GetDamageScalar(mobile, damageBonus);

                SpellHelper.Damage(this, Caster, mobile, damage);
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private MagicArrowSpell m_Owner;

			public InternalTarget( MagicArrowSpell owner ) : base( 12, false, TargetFlags.Harmful )
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