using System;
using Server.Targeting;
using Server.Network;
using System.Collections;
using System.Collections.Generic;

using Server.Mobiles;
using Server.Items;
using Server.Custom;

namespace Server.Spells.Sixth
{
	public class ExplosionSpell : MagerySpell
	{
		private static Hashtable m_ExpRegistry = new Hashtable();
		public static Hashtable Registry { get { return m_ExpRegistry; } }
		private static SpellInfo m_Info = new SpellInfo(
			"Explosion", "Vas Ort Flam",
			230,
			9041,
			Reagent.Bloodmoss,
			Reagent.MandrakeRoot
			);

        public override SpellCircle Circle { get { return SpellCircle.Sixth; } }

		public ExplosionSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
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

            else if (CheckHSequence(mobile))
            {
                Mobile source = Caster;
                SpellHelper.Turn(Caster, mobile);
                SpellHelper.CheckReflect((int)this.Circle, ref source, ref mobile);

                double damage = (double)Utility.RandomMinMax(20, 35);
                double damageBonus = 0;

                CheckMagicResist(mobile);

                bool enhancedSpellcast = SpellHelper.IsEnhancedSpell(Caster, mobile, EnhancedSpellbookType.Fire, true, true);
                Boolean chargedSpellcast = SpellHelper.IsChargedSpell(Caster, mobile, true, Scroll != null);
                Boolean isTamedTarget = SpellHelper.IsTamedTarget(Caster, mobile);

                int spellHue = PlayerEnhancementPersistance.GetSpellHueFor(Caster, HueableSpell.EnergyBolt);

                if (enhancedSpellcast)
                {
                    if (isTamedTarget)
                        damageBonus += SpellHelper.EnhancedSpellTamedCreatureBonus;

                    else
                        damageBonus += SpellHelper.EnhancedSpellBonus;
                }

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
                }

                Timer.DelayCall(TimeSpan.FromSeconds(2.5), delegate
                {
                    if (!SpecialAbilities.Exists(mobile)) return;

                    if (chargedSpellcast)
                    {
                        mobile.FixedParticles(0x36BD, 20, 20, 5044, spellHue, 0, EffectLayer.Head);
                        mobile.PlaySound(0x307);
                    }

                    else
                    {
                        mobile.FixedParticles(0x36BD, 20, 10, 5044, spellHue, 0, EffectLayer.Head);
                        mobile.PlaySound(0x307);
                    }

                    damage *= GetDamageScalar(mobile, damageBonus);

                    SpellHelper.Damage(this, Caster, mobile, damage);
                }); 
            }

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private ExplosionSpell m_Owner;

			public InternalTarget( ExplosionSpell owner ) : base( 12, false, TargetFlags.Harmful )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( m_ExpRegistry.ContainsKey( from ) )				
					return;				

				if ( o != null && o is Mobile )				
					m_Owner.Target( (Mobile)o );				
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}