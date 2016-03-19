using System;
using System.Collections;
using Server;
using Server.Targeting;
using Server.Mobiles;
using Server.SkillHandlers;
using Server.Items;
using Server.Custom;
using System.Collections.Generic;

namespace Server.Spells.Sixth
{
	public class InvisibilitySpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Invisibility", "An Lor Xen",
				206,
				9002,
				Reagent.Bloodmoss,
				Reagent.Nightshade
			);

		public override SpellCircle Circle { get { return SpellCircle.Sixth; } }

		public InvisibilitySpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
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
                {
                    this.Target(casterCreature.SpellTarget);
                }
            }

            else
            {
                Caster.Target = new InternalTarget(this);
            }
		}

		public void Target( Mobile mobile )
		{
            if (!Caster.CanSee(mobile) || mobile.Hidden)
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}

            //It was on IPY but seems right...U cant hide vendor and others with higher access level.
			else if ( mobile is Mobiles.BaseVendor || mobile is Mobiles.PlayerVendor || mobile is Mobiles.PlayerBarkeeper || mobile.AccessLevel > Caster.AccessLevel )
			{
				Caster.SendLocalizedMessage( 501857 ); // This spell won't work on that!
			}

			else if ( CheckBSequence( mobile ) )
			{
				SpellHelper.Turn( Caster, mobile );

                int spellHue = 0;

                //Player Enhancement Customization: Vanish
                bool vanish = PlayerEnhancementPersistance.IsCustomizationEntryActive(Caster, CustomizationType.Vanish);

                if (vanish)                
                    CustomizationAbilities.Vanish(mobile);

                else
                {
                    Effects.SendLocationParticles(EffectItem.Create(new Point3D(mobile.X, mobile.Y, mobile.Z + 16), Caster.Map, EffectItem.DefaultDuration), 0x376A, 10, 15, spellHue, 0, 5045, 0);
                    mobile.PlaySound(0x3C4);
                }				

				mobile.Hidden = true;
                
                if (mobile is BaseCreature) 
                {
                    var bc = mobile as BaseCreature;
                    if (!bc.Controlled && !bc.Summoned && (!bc.InitialInnocent || bc.AlwaysAttackable || bc.IsMurderer()))
                        Caster.CriminalAction(false);
                }                                

				RemoveTimer( mobile );

                //Changed to the duration of IPY
				TimeSpan duration = TimeSpan.FromSeconds( Caster.Skills[SkillName.Magery].Value * 1.2 ); // 120% of magery

				Timer t = new InternalTimer( mobile, duration );

				m_Table[mobile] = t;

				t.Start();
			}

			FinishSequence();
		}

		private static Hashtable m_Table = new Hashtable();

		public static bool HasTimer( Mobile m )
		{
			return m_Table[m] != null;
		}

		public static void RemoveTimer( Mobile m )
		{
			Timer t = (Timer)m_Table[m];

			if ( t != null )
			{
				t.Stop();
				m_Table.Remove( m );
			}
		}

		private class InternalTimer : Timer
		{
			private Mobile m_Mobile;

			public InternalTimer( Mobile m, TimeSpan duration ) : base( duration )
			{  
                //Removed by IPY
				//Priority = TimerPriority.OneSecond;
				m_Mobile = m;
			}

			protected override void OnTick()
			{
				m_Mobile.RevealingAction();
				RemoveTimer( m_Mobile );
			}
		}

		public class InternalTarget : Target
		{
			private InvisibilitySpell m_Owner;

			public InternalTarget( InvisibilitySpell owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Beneficial )
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