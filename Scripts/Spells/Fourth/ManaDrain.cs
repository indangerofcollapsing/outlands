using System;
using System.Collections.Generic;
using Server.Network;
using Server.Targeting;
using Server.Mobiles;
using Server.Items;

namespace Server.Spells.Fourth
{
	public class ManaDrainSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Mana Drain", "Ort Rel",
				215,
				9031,
				Reagent.BlackPearl,
				Reagent.MandrakeRoot,
				Reagent.SpidersSilk
			);

		public override SpellCircle Circle { get { return SpellCircle.Fourth; } }

		public ManaDrainSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
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

		private static Dictionary<Mobile, Timer> m_Table = new Dictionary<Mobile, Timer>();

		private void AosDelay_Callback( object state )
		{
			object[] states = (object[])state;

			Mobile m = (Mobile)states[0];
			int mana = (int)states[1];

			if ( m.Alive && !m.IsDeadBondedPet )
			{
				m.Mana += mana;

				m.FixedEffect( 0x3779, 10, 25 );
				m.PlaySound( 0x28E );
			}

			m_Table.Remove( m );
		}

		public void Target( Mobile mobile )
		{
            if (!Caster.CanSee(mobile) || mobile.Hidden)			
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			
			else if ( CheckHSequence( mobile ) )
			{
				SpellHelper.Turn( Caster, mobile );
				SpellHelper.CheckReflect( (int)this.Circle, Caster, ref mobile );

				if ( mobile.Spell != null )
					mobile.Spell.OnCasterHurt();

				mobile.Paralyzed = false;
               
                int manaLoss = 20;      
                    
                if (CheckMagicResist(mobile))
                {
                    mobile.SendLocalizedMessage(501783); // You feel yourself resisting magical energy. 
                    manaLoss = 0;
                }

                bool enhancedSpellcast = SpellHelper.IsEnhancedSpell(Caster, mobile, EnhancedSpellbookType.Warlock, true, true);

                if (enhancedSpellcast)
                {
                    manaLoss *= 2;
                        
                    mobile.FixedParticles(0x374A, 10, 30, 5032, EffectLayer.Head);
                    mobile.PlaySound(0x1F8);
                }

                else
                {
                    mobile.FixedParticles(0x374A, 10, 15, 5032, EffectLayer.Head);
                    mobile.PlaySound(0x1F8);
                }

                mobile.Mana -= manaLoss;	

				HarmfulSpell( mobile );
			}

			FinishSequence();
		}

		public override double GetResistPercent( Mobile target )
		{
            if (target != null && target is PlayerMobile)
            {
                return 99.0;
            }

            else
            {
                return base.GetResistPercent(target);
            }
		}

		private class InternalTarget : Target
		{
			private ManaDrainSpell m_Owner;

			public InternalTarget( ManaDrainSpell owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
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