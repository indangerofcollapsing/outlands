using System;
using System.Collections;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Custom;

namespace Server.Spells.Second
{
	public class ProtectionSpell : MagerySpell
	{
		private static Hashtable m_Registry = new Hashtable();
		public static Hashtable Registry { get { return m_Registry; } }

		private static SpellInfo m_Info = new SpellInfo(
				"Protection", "Uus Sanct",
				236,
				9011,
				Reagent.Garlic,
				Reagent.Ginseng,
				Reagent.SulfurousAsh
			);

		public override SpellCircle Circle { get { return SpellCircle.Second; } }

		public ProtectionSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override bool CheckCast()
		{
			if ( Core.AOS )
				return true;

			if ( m_Registry.ContainsKey( Caster ) )
			{
				Caster.SendLocalizedMessage( 1005559 ); // This spell is already in effect.
				
                return false;
			}

			return true;
		}

		private static Hashtable m_Table = new Hashtable();

        public static bool IsUnderProtection(Mobile m)
        {
            object[] mods = (object[])m_Table[m];
            return mods != null;
        }

		public static void Toggle( Mobile caster, Mobile target )
		{
			object[] mods = (object[])m_Table[target];

			if ( mods == null )
			{
				target.PlaySound( 0x1E9 );
				target.FixedParticles( 0x375A, 9, 20, 5016, EffectLayer.Waist );

				mods = new object[2]
							{
								new ResistanceMod( ResistanceType.Physical, -15 + (int)(caster.Skills[SkillName.Inscribe].Value / 20) ),
								new DefaultSkillMod( SkillName.MagicResist, true, -35 + (int)(caster.Skills[SkillName.Inscribe].Value / 20) )
							};

				m_Table[target] = mods;
				Registry[target] = 100.0;

				target.AddResistanceMod( (ResistanceMod)mods[0] );
				target.AddSkillMod( (SkillMod)mods[1] );
			}

			else
			{
				target.PlaySound( 0x1ED );
				target.FixedParticles( 0x375A, 9, 20, 5016, EffectLayer.Waist );

				m_Table.Remove( target );
				Registry.Remove( target );

				target.RemoveResistanceMod( (ResistanceMod)mods[0] );
				target.RemoveSkillMod( (SkillMod)mods[1] );
			}
		}

		public override void OnCast()
		{
            if (Core.AOS)
            {
                if (CheckSequence())
                    Toggle(Caster, Caster);

                FinishSequence();
            }

            //Changed by IPY
            else
            {
                BaseCreature casterCreature = Caster as BaseCreature;

                if (casterCreature != null)
                {
                    if (casterCreature.SpellTarget != null)                    
                        Target(casterCreature.SpellTarget);                    
                }

                else
                {
                    Caster.Target = new InternalTarget(this);
                }
            }
		}

		public void Target( Mobile m )
		{
            if (!m.CanSee(m) || m.Hidden)			
				m.SendLocalizedMessage( 500237 ); // Target can not be seen.			

			else if ( CheckBSequence( m ) )
			{				
			    SpellHelper.Turn( Caster, m );

			    int val = (int)(m.Skills[SkillName.Magery].Value / 10.0 + 1);

				if ( m.BeginAction( typeof( ProtectionSpell ) ) )
				{
				    Caster.DoBeneficial( m );
				    m.VirtualArmorMod += val;

				    new InternalTimer( m, Caster, val ).Start();

                    bool enhancedSpellcast = SpellHelper.IsEnhancedSpell(Caster, m, EnhancedSpellbookType.Wizard, false, true);
                    
                    int spellHue = PlayerEnhancementPersistance.GetSpellHueFor(Caster, HueableSpell.Protection);

                    if (enhancedSpellcast)
                    {
                        m.FixedParticles(0x375A, 9, 40, 5027, spellHue, 0, EffectLayer.Waist);
                        m.PlaySound(0x1F7);
                    }

                    else
                    {
                        m.FixedParticles(0x375A, 9, 20, 5027, spellHue, 0, EffectLayer.Waist);
                        m.PlaySound(0x1F7);
                    }	
				}
			}

			FinishSequence();
		}

        //Changed by IPY
		private class InternalTimer : Timer
		{
			private Mobile m_Owner;
			private int m_Val;

			public InternalTimer( Mobile target, Mobile caster, int val ) : base( TimeSpan.FromSeconds( 0 ) )
			{
				double time = caster.Skills[SkillName.Magery].Value * 1.2;

				if ( time > 144 )
					time = 144;

                bool enhancedSpellcast = SpellHelper.IsEnhancedSpell(caster, target, EnhancedSpellbookType.Wizard, true, true);

                if (enhancedSpellcast)                
                    time *= 5;                

				Delay = TimeSpan.FromSeconds( time );
				Priority = TimerPriority.TwoFiftyMS;

				m_Owner = target;
				m_Val = val;
			}
           
            //Changed by IPY
			protected override void OnTick()
			{
				m_Owner.EndAction( typeof( ProtectionSpell ) );
				m_Owner.VirtualArmorMod -= m_Val;

				if ( m_Owner.VirtualArmorMod < 0 )
					m_Owner.VirtualArmorMod = 0;
			}
		}
 
        //Changed by IPY
		private class InternalTarget : Target
		{
			private ProtectionSpell m_Owner;

			public InternalTarget( ProtectionSpell owner ) : base( 12, false, TargetFlags.Beneficial )
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
