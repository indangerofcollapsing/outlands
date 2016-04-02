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

		public override bool DelayedDamageStacking { get { return !Core.AOS; } }

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

		public override bool DelayedDamage{ get{ return false; } }

		public void Target( Mobile m )
		{
            if (!Caster.CanSee(m) || m.Hidden)			
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.			

			if ( Caster.HarmfulCheck( m ) )
			{
				if ( Caster.CanBeHarmful( m ) && CheckSequence() )
				{
					Mobile attacker = Caster, defender = m;
					SpellHelper.Turn( Caster, m );

					SpellHelper.CheckReflect( (int)this.Circle, Caster, ref m );

					ExplosionSpell.Registry[attacker] = "true";

					InternalTimer t = new InternalTimer( this, attacker, defender, m );
					t.Start();
				}
			}

			FinishSequence();
		}

		private class InternalTimer : Timer
		{
			private MagerySpell m_Spell;
			private Mobile mobile;
			private Mobile m_Attacker, m_Defender;

			public InternalTimer( MagerySpell spell, Mobile attacker, Mobile defender, Mobile target ): base( TimeSpan.FromSeconds( 2.5 ) )
			{
				m_Spell = spell;
				m_Attacker = attacker;
				m_Defender = defender;
				mobile = target;

				Priority = TimerPriority.FiftyMS;
			}

			protected override void OnTick()
			{
                Map map = m_Attacker.Map;

                double damage = (double)Utility.RandomMinMax(20, 35);
                double damageBonus = 0;

                m_Spell.CheckMagicResist(mobile);	

                bool enhancedSpellcast = SpellHelper.IsEnhancedSpell(m_Spell.Caster, mobile, EnhancedSpellbookType.Fire, true, true);
                Boolean chargedSpellcast = SpellHelper.IsChargedSpell(m_Spell.Caster, mobile, true, m_Spell.Scroll != null);
                Boolean isTamedTarget = SpellHelper.IsTamedTarget(m_Spell.Caster, mobile);

                int spellHue = PlayerEnhancementPersistance.GetSpellHueFor(m_Spell.Caster, HueableSpell.Explosion);

                if (enhancedSpellcast)
                {
                    if (isTamedTarget)
                        damageBonus  += SpellHelper.EnhancedSpellTamedCreatureBonus;

                    else
                        damageBonus += SpellHelper.EnhancedSpellBonus;
                }
                
                if (chargedSpellcast)
                {
                    if (isTamedTarget)
                        damageBonus += SpellHelper.ChargedSpellTamedCreatureBonus;

                    else
                        damageBonus += SpellHelper.ChargedSpellBonus;

                    Registry.Remove(m_Attacker);

                    mobile.FixedParticles(0x36BD, 20, 20, 5044, spellHue, 0, EffectLayer.Head);
                    mobile.PlaySound(0x307);
                }

                else
                {
                    Registry.Remove(m_Attacker);

                    mobile.FixedParticles(0x36BD, 20, 10, 5044, spellHue, 0, EffectLayer.Head);
                    mobile.PlaySound(0x307);
                }

                damage *= m_Spell.GetDamageScalar(mobile, damageBonus);

				SpellHelper.Damage(m_Spell, mobile, damage, 0, 100, 0, 0, 0 );
			}
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