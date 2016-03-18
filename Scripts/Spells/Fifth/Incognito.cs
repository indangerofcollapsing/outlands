using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Misc;
using Server.Items;
using Server.Gumps;
using Server.Spells;
using Server.Spells.Seventh;
using Server.Custom;

namespace Server.Spells.Fifth
{
	public class IncognitoSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Incognito", "Kal In Ex",
				206,
				9002,
				Reagent.Bloodmoss,
				Reagent.Garlic,
				Reagent.Nightshade
			);

		public override SpellCircle Circle { get { return SpellCircle.Fifth; } }

		public IncognitoSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override bool CheckCast()
		{           
			if ( !Caster.CanBeginAction( typeof( IncognitoSpell ) ) )
			{
				Caster.SendLocalizedMessage( 1005559 ); // This spell is already in effect.
				return false;
			}

            else if (KinPaint.IsWearingKinPaint(Caster))
            {
                Caster.SendMessage("You cannot can incognito while wearing kin paint");
                return false;
            }

			return true;
		}

		public override void OnCast()
		{           
			if ( !Caster.CanBeginAction( typeof( IncognitoSpell ) ) )			
				Caster.SendLocalizedMessage( 1005559 ); // This spell is already in effect.

            else if (KinPaint.IsWearingKinPaint(Caster))            
                Caster.SendMessage("You cannot can incognito while wearing kin paint");               
            
			else if ( DisguiseTimers.IsDisguised( Caster ) )			
				Caster.SendLocalizedMessage( 1061631 ); // You can't do that while disguised.
			
			else if ( !Caster.CanBeginAction( typeof( PolymorphSpell ) ) || Caster.IsBodyMod )			
				DoFizzle();
			
			else if ( CheckSequence() )
			{
                if (Caster.BeginAction(typeof(IncognitoSpell)))
                {
                    DisguiseTimers.StopTimer(Caster);

                    bool shadowSkin = false;

                    //Player Enhancement Customization: Shadowskin
                    if (PlayerEnhancementPersistance.IsCustomizationEntryActive(Caster, CustomizationType.Shadowskin))
                    {
                        Caster.HueMod = 18999;
                        shadowSkin = true;
                    }

                    else
                        Caster.HueMod = Caster.Race.RandomSkinHue();

                    Caster.NameMod = Caster.Female ? NameList.RandomName("female") : NameList.RandomName("male");

                    PlayerMobile pm = Caster as PlayerMobile;

                    if (pm != null && pm.Race != null)
                    {
                        pm.SetHairMods(pm.Race.RandomHair(pm.Female), pm.Race.RandomFacialHair(pm.Female));
                        pm.HairHue = pm.Race.RandomHairHue();
                        pm.FacialHairHue = pm.Race.RandomHairHue();
                    }

                    Caster.FixedParticles(0x373A, 10, 15, 5036, EffectLayer.Head);
                    Caster.PlaySound(0x3BD);

                    BaseArmor.ValidateMobile(Caster);

                    StopTimer(Caster);

                    int val = (int)(Caster.Skills[SkillName.Magery].Value * 1.2);

                    if (val > 144)
                        val = 144;

                    else if (val < 15)
                        val = 15;

                    if (shadowSkin && val > 30)
                        val = 30;

                    TimeSpan length = TimeSpan.FromSeconds(val);

                    Timer t = new InternalTimer(Caster, length);

                    m_Timers[Caster] = t;

                    t.Start();
                }

                else
                    Caster.SendMessage("You are already under the effect of the Incognito spell.");		
			}

			FinishSequence();
		}

		private static Hashtable m_Timers = new Hashtable();

		public static bool StopTimer( Mobile m )
		{
			Timer t = (Timer)m_Timers[m];

			if ( t != null )
			{
				t.Stop();
				m_Timers.Remove( m );				
			}

			return ( t != null );
		}

		private static int[] m_HairIDs = new int[]
		{
			0x2044, 0x2045, 0x2046,
			0x203C, 0x203B, 0x203D,
			0x2047, 0x2048, 0x2049,
			0x204A, 0x0000
		};

		private static int[] m_BeardIDs = new int[]
		{
			0x203E, 0x203F, 0x2040,
			0x2041, 0x204B, 0x204C,
			0x204D, 0x0000
		};

		private class InternalTimer : Timer
		{
			private Mobile m_Owner;

			public InternalTimer( Mobile owner, TimeSpan length ) : base( length )
			{
				m_Owner = owner;
				Priority = TimerPriority.OneSecond;
			}

			protected override void OnTick()
			{
				if ( !m_Owner.CanBeginAction( typeof( IncognitoSpell ) ) )
				{
					if ( m_Owner is PlayerMobile )
						((PlayerMobile)m_Owner).SetHairMods( -1, -1 );

					m_Owner.BodyMod = 0;
					m_Owner.HueMod = -1;
					m_Owner.NameMod = null;
					m_Owner.EndAction( typeof( IncognitoSpell ) );

					BaseArmor.ValidateMobile( m_Owner );					
				}
			}
		}
	}
}
