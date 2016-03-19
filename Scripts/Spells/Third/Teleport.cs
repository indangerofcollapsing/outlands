using System;
using Server.Targeting;
using Server.Network;
using Server.Regions;
using Server.Items;
using Server.Custom;
using Server.Mobiles;

namespace Server.Spells.Third
{
	public class TeleportSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Teleport", "Rel Por",
				215,
				9031,
				Reagent.Bloodmoss,
				Reagent.MandrakeRoot
			);

		public override SpellCircle Circle { get { return SpellCircle.Third; } }

		public TeleportSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override bool CheckCast()
		{
            WarpBlockerTotem recallBlocker = WarpBlockerTotem.RecallBlockerTriggered(Caster, WarpBlockerTotem.MovementMode.TeleportOut, Caster.Location, Caster.Map);

            if (recallBlocker != null)
            {
                if (recallBlocker.PreventTeleportOutResponse != "")
                    Caster.SendMessage(recallBlocker.PreventTeleportOutResponse);
                else
                    Caster.SendMessage(WarpBlockerTotem.DefaultTeleportOutResponse);

                return false;
            }
            
			if ( Server.Misc.WeightOverloading.IsOverloaded( Caster ) )
			{
				Caster.SendLocalizedMessage( 502359, "", 0x22 ); // Thou art too encumbered to move.
				return false;
			}

            else if (!Caster.CanBeginAction(typeof(TeleportSpell)))
            {
                Caster.SendLocalizedMessage(502644, "", 0x22); //You have not yet recovered from casting a spell.
                return false;
            }

			//return true;
			return SpellHelper.CheckTravel(Caster, TravelCheckType.TeleportFrom );
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public void Target( IPoint3D p )
		{
			IPoint3D orig = p;
			Map map = Caster.Map;

			SpellHelper.GetSurfaceTop( ref p );
			Point3D pP = new Point3D(p);

            WarpBlockerTotem recallBlocker = WarpBlockerTotem.RecallBlockerTriggered(Caster, WarpBlockerTotem.MovementMode.TeleportIn, pP, Caster.Map);

            if (recallBlocker != null)
            {
                if (recallBlocker.PreventTeleportInResponse != "")
                    Caster.SendMessage(recallBlocker.PreventTeleportInResponse);
                else
                    Caster.SendMessage(WarpBlockerTotem.DefaultTeleportInResponse);               
            }

            else if ( Server.Misc.WeightOverloading.IsOverloaded( Caster ) )
			{
				Caster.SendLocalizedMessage( 502359, "", 0x22 ); // Thou art too encumbered to move.
			}

    		else if ( !SpellHelper.CheckTravel( Caster, TravelCheckType.TeleportFrom ) )
			{
                Caster.SendLocalizedMessage(501942); // That location is blocked.
			}

			else if (!SpellHelper.CheckTravel(Caster, map, pP, TravelCheckType.TeleportTo))
			{
                Caster.SendLocalizedMessage(501942); // That location is blocked.
			}

			else if ( map == null || !map.CanSpawnMobile( p.X, p.Y, p.Z ) )
			{
				Caster.SendLocalizedMessage( 501942 ); // That location is blocked.
			}

			else if (SpellHelper.CheckMulti(pP, map))
			{
				Caster.SendLocalizedMessage( 501942 ); // That location is blocked.
			}

			else
			{
                if ( CheckSequence() )
				{
					SpellHelper.Turn( Caster, orig );

					Mobile m = Caster;

					Point3D from = m.Location;
					Point3D to = pP;

					m.Location = to;
					m.ProcessDelta();

                    int spellHue = PlayerEnhancementPersistance.GetSpellHueFor(Caster, HueableSpell.Teleport);

                    if (m.Player)
                    {
                        Effects.SendLocationParticles(EffectItem.Create(from, m.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, spellHue, 0, 2023, 0);

                        //Player Enhancement Customization: Blink
                        bool blink = PlayerEnhancementPersistance.IsCustomizationEntryActive(Caster, CustomizationType.Blink);

                        if (blink)
                        {
                            int distance = Utility.GetDistance(from, to);

                            Point3D effectStep = from;

                            for (int a = 0; a < distance + 1; a++)
                            {
                                Timer.DelayCall(TimeSpan.FromSeconds(a * .05), delegate
                                {
                                    if (m == null) return;

                                    Direction direction = Utility.GetDirection(effectStep, to);
                                    effectStep = SpecialAbilities.GetPointByDirection(effectStep, direction);

                                    Effects.SendLocationParticles(EffectItem.Create(effectStep, m.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, spellHue, 0, 2023, 0);
                                    m.PlaySound(0x5C6);
                                });
                            }
                        }

                        else
                        {
                            Effects.SendLocationParticles(EffectItem.Create(to, m.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, spellHue, 0, 2023, 0);
                            m.PlaySound(0x1FE);
                        }
                    }

                    else
                    {
                        m.FixedParticles(0x376A, 9, 32, 0x13AF, spellHue, 0, EffectLayer.Waist);
                        m.PlaySound(0x1FE);
                    }

					IPooledEnumerable eable = m.GetItemsInRange( 0 );
	
					foreach ( Item item in eable )
					{
						if ( item is Server.Spells.Sixth.ParalyzeFieldSpell.InternalItem || item is Server.Spells.Fifth.PoisonFieldSpell.InternalItem || item is Server.Spells.Fourth.FireFieldSpell.FireFieldItem )
							item.OnMoveOver( m );
					}
	
					eable.Free();			

					m.BeginAction(typeof(TeleportSpell));
					Timer.DelayCall(TimeSpan.FromSeconds(6), delegate { m.EndAction(typeof(TeleportSpell)); });
				}
			}

			FinishSequence();
		}

		public class InternalTarget : Target
		{
			private TeleportSpell m_Owner;

			public InternalTarget( TeleportSpell owner ) : base( 8, true, TargetFlags.None )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				IPoint3D p = o as IPoint3D;

				if ( p != null )
					m_Owner.Target( p );
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}