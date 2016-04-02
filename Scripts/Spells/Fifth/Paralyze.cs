using System;
using Server.Mobiles;
using Server.Targeting;
using Server.Network;
using Server.Items;
using Server.Custom;

namespace Server.Spells.Fifth
{
	public class ParalyzeSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Paralyze", "An Ex Por",
				218,
				9012,
				Reagent.Garlic,
				Reagent.MandrakeRoot,
				Reagent.SpidersSilk
			);

		public override SpellCircle Circle { get { return SpellCircle.Fifth; } }

		public ParalyzeSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override bool DelayedDamage{ get{ return true; } }

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

		public void Target( Mobile m )
		{
            if (!Caster.CanSee(m) || m.Hidden)			
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.			

			else if ( CheckHSequence( m ) )
			{
				SpellHelper.Turn( Caster, m );
				SpellHelper.CheckReflect( (int)this.Circle, Caster, ref m );

				double duration;

				if (m.Player)
				{						
					duration = 5 + (Caster.Skills[SkillName.Magery].Value * 0.05);

					if (CheckMagicResist(m))
						duration *= 0.5;
				}

				else
				{						
					duration = 10.0 + (Caster.Skills[SkillName.Magery].Value * 0.2);

					if (CheckMagicResist(m))
						duration *= 0.75;
				}				
                                
                bool enhancedSpellcast = SpellHelper.IsEnhancedSpell(Caster, m, EnhancedSpellbookType.Warlock, true, true);

                int spellHue = 0;

                if (enhancedSpellcast)
                {
                    if (m.Paralyze(Caster, duration * 5))
                    {
                        m.FixedEffect(0x376A, 10, 30, spellHue, 0);
                        m.PlaySound(0x204);         
                    }

                    else if (m is PlayerMobile)
                    {
                        m.FixedEffect(0x376A, 10, 30, spellHue, 0);
                        m.PlaySound(0x204);   
                    }
                }

                else
                {
                    if (m.Paralyze(Caster, duration))
                    {
                        m.FixedEffect(0x376A, 10, 15, spellHue, 0);
                        m.PlaySound(0x204);      
                    }

                    else if (m is PlayerMobile)
                    {
                        m.FixedEffect(0x376A, 10, 15, spellHue, 0);
                        m.PlaySound(0x204);  
                    }
                }				

				HarmfulSpell( m );
			}

			FinishSequence();
		}

		public class InternalTarget : Target
		{
			private ParalyzeSpell m_Owner;

			public InternalTarget( ParalyzeSpell owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
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
