using System;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Custom;

namespace Server.Spells.Seventh
{
	public class FlameStrikeSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Flame Strike", "Kal Vas Flam",
				245,
				9042,
				Reagent.SpidersSilk,
				Reagent.SulfurousAsh
			);

		public override SpellCircle Circle { get { return SpellCircle.Seventh; } }

		public FlameStrikeSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
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

                double damage = (double)Utility.RandomMinMax(30, 45);
                double damageBonus = 0;

                if (mobile is BaseCreature)                
                    damageBonus += .5;                

                CheckMagicResist(mobile);	
		
                bool enhancedSpellcast = SpellHelper.IsEnhancedSpell(Caster, mobile, EnhancedSpellbookType.Fire, true, true);
                Boolean chargedSpellcast = SpellHelper.IsChargedSpell(Caster, mobile, true, Scroll != null);
                Boolean isTamedTarget = SpellHelper.IsTamedTarget(Caster, mobile);

                int spellHue = PlayerEnhancementPersistance.GetSpellHueFor(Caster, HueableSpell.Flamestrike);

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

                    mobile.FixedParticles(0x3709, 10, 60, 5052, spellHue, 0, EffectLayer.LeftFoot);
                    mobile.PlaySound(0x208); 
                }

                else
                {
                    mobile.FixedParticles(0x3709, 10, 30, 5052, spellHue, 0, EffectLayer.LeftFoot);
                    mobile.PlaySound(0x208); 
                }

                damage *= GetDamageScalar(mobile, damageBonus);

                SpellHelper.Damage(this, Caster, mobile, damage);
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private FlameStrikeSpell m_Owner;

			public InternalTarget( FlameStrikeSpell owner ) : base( 12, false, TargetFlags.Harmful )
			{
				m_Owner = owner;
			}

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is Mobile)                
                    m_Owner.Target((Mobile)o);                
            }

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}