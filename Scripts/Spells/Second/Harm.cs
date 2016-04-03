using System;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using System.Collections.Generic;
using Server.Items;
using Server.Custom;

namespace Server.Spells.Second
{
	public class HarmSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Harm", "An Mani",
				212,
				Core.AOS ? 9001 : 9041,
				Reagent.Nightshade,
				Reagent.SpidersSilk
			);

		public override SpellCircle Circle { get { return SpellCircle.Second; } }

		public HarmSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
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
				SpellHelper.Turn( Caster, mobile );
				SpellHelper.CheckReflect( (int)this.Circle, Caster, ref mobile );
                
                double damage = (double)Utility.RandomMinMax(4, 8);
                double damageBonus = 0;
                
                CheckMagicResist(mobile);								

                bool enhancedSpellcast = SpellHelper.IsEnhancedSpell(Caster, mobile, EnhancedSpellbookType.Energy, true, true);
                Boolean chargedSpellcast = SpellHelper.IsChargedSpell(Caster, mobile, true, Scroll != null);
                Boolean isTamedTarget = SpellHelper.IsTamedTarget(Caster, mobile);

                int spellHue = PlayerEnhancementPersistance.GetSpellHueFor(Caster, HueableSpell.Harm);

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

                    mobile.FixedParticles(0x374A, 10, 30, 5013, spellHue, 0, EffectLayer.Waist);
                    mobile.PlaySound(0x1F1);
                }

                else
                {
                    mobile.FixedParticles(0x374A, 10, 15, 5013, spellHue, 0, EffectLayer.Waist);
                    mobile.PlaySound(0x1F1);
                }

                damage *= GetDamageScalar(mobile, damageBonus);

                SpellHelper.Damage(this, Caster, mobile, damage);
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private HarmSpell m_Owner;

			public InternalTarget( HarmSpell owner ) : base( 12, false, TargetFlags.Harmful )
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