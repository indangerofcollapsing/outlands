using System;
using Server.Items;
using Server.Multis;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using Server.Regions;

using Server.Custom.Items;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Server.Custom;

namespace Server.Spells.Fourth
{
	public class RecallSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Recall", "Kal Ort Por",
				239,
				9031,
				Reagent.BlackPearl,
				Reagent.Bloodmoss,
				Reagent.MandrakeRoot
			);

		public override SpellCircle Circle { get { return SpellCircle.Fourth; } }

		private RunebookEntry m_Entry;
		private Runebook m_Book;
		
		public override TimeSpan GetCastDelay() 
		{
            return TimeSpan.FromSeconds(1.75);
		}

		public RecallSpell( Mobile caster, Item scroll ) : this( caster, scroll, null, null )
		{
		}

		public RecallSpell( Mobile caster, Item scroll, RunebookEntry entry, Runebook book ) : base( caster, scroll, m_Info )
		{
			m_Entry = entry;
			m_Book = book;
		}

		public override void GetCastSkills( out double min, out double max )
		{
			if( m_Book != null )	//recall using Runebook charge
				min = max = 0;

			else
				base.GetCastSkills( out min, out max );
		}

		public override void OnCast()
		{
			if ( m_Entry == null )
				Caster.Target = new InternalTarget( this );
			else
				Effect( m_Entry.Location, m_Entry.Map, true );
		}
        
		public override bool CheckCast()
		{
			PlayerMobile pm_Caster = Caster as PlayerMobile;

            WarpBlockerTotem recallBlocker = WarpBlockerTotem.RecallBlockerTriggered(Caster, WarpBlockerTotem.MovementMode.RecallOut, Caster.Location, Caster.Map);

            if (recallBlocker != null)
            {
                if (recallBlocker.PreventRecallOutResponse != "")
                    Caster.SendMessage(recallBlocker.PreventRecallOutResponse);
                else
                    Caster.SendMessage(WarpBlockerTotem.DefaultRecallOutResponse);

                return false;
            }
                        
			else if ( Caster.Criminal )
			{
				Caster.SendLocalizedMessage( 1005561, "", 0x22 ); // Thou'rt a criminal and cannot escape so easily.
				return false;
			}
            
			else if ( SpellHelper.CheckCombat( Caster ) )
			{
				Caster.SendLocalizedMessage( 1005564, "", 0x22 ); // Wouldst thou flee during the heat of battle??
				return false;
			}            

			else if ( Server.Misc.WeightOverloading.IsOverloaded( Caster ) )
			{
				Caster.SendLocalizedMessage( 502359, "", 0x22 ); // Thou art too encumbered to move.
				return false;
			}

            if (pm_Caster != null)
            {
                if (pm_Caster.RecallRestrictionExpiration > DateTime.UtcNow)
                {   
                    int minutes = pm_Caster.RecallRestrictionExpiration.Subtract(DateTime.UtcNow).Minutes;
                    int seconds = pm_Caster.RecallRestrictionExpiration.Subtract(DateTime.UtcNow).Seconds;

                    string sTime = "";

                    if (minutes > 1)
                        sTime += minutes.ToString() + " minutes ";

                    else if (minutes == 1)
                        sTime += minutes.ToString() + " minute ";

                    if (seconds > 1)
                        sTime += seconds.ToString() + " seconds ";

                    else if (seconds == 1)
                        sTime += seconds.ToString() + " second ";

                    sTime = sTime.Trim();

                    if (sTime != "")
                        pm_Caster.SendMessage("You are unable to cast this spell for another " + sTime + ".");

                    return false;
                }
            }

			return SpellHelper.CheckTravel( Caster, TravelCheckType.RecallFrom );
		}

		public void Effect( Point3D loc, Map map, bool checkMulti )
		{
			PlayerMobile pm = Caster as PlayerMobile;

            BaseBoat boat = BaseBoat.FindBoatAt(loc, map);

            WarpBlockerTotem recallBlocker = WarpBlockerTotem.RecallBlockerTriggered(Caster, WarpBlockerTotem.MovementMode.RecallIn, loc, map);

            if (recallBlocker != null)
            {
                if (recallBlocker.PreventRecallInResponse != "")
                    Caster.SendMessage(recallBlocker.PreventRecallInResponse);
                else
                    Caster.SendMessage(WarpBlockerTotem.DefaultRecallInResponse);               
            }
            
            else if ( map == null || (!Core.AOS && Caster.Map != map) )
			{
				Caster.SendLocalizedMessage( 1005569 ); // You can not recall to another facet.
			}

            else if (!SpellHelper.CheckTravel(Caster, map, loc, TravelCheckType.RecallTo))
            {
                Caster.SendLocalizedMessage(501802); // Thy spell doth not appear to work...
            }

            else if (SpellHelper.IsAnyT2A(map, loc) && pm != null)
            {
            }

            else if (Server.Misc.WeightOverloading.IsOverloaded(Caster))
            {
                Caster.SendLocalizedMessage(502359, "", 0x22); // Thou art too encumbered to move.
            }

            else if (map != Map.Felucca)
            {
                Caster.SendLocalizedMessage(1019004); // You are not allowed to travel there.
            }

            else if (Caster.ShortTermMurders >= 5 && map != Map.Felucca)
            {
                Caster.SendLocalizedMessage(1019004); // You are not allowed to travel there.
            }

            else if (!SpellHelper.CheckIfOK(Caster.Map, loc.X, loc.Y, loc.Z))
            {
                Caster.SendLocalizedMessage(501942); // That location is blocked.
            }

            else if ((checkMulti && SpellHelper.CheckMulti(loc, map)))
            {
                Caster.SendLocalizedMessage(501942); // That location is blocked.
            }

            else if (SpellHelper.IsSolenHiveLoc(loc))
            {
                Caster.SendLocalizedMessage(1019004); // You are not allowed to travel there.
            }

            else if (SpellHelper.IsStarRoom(loc))
            {
                Caster.SendLocalizedMessage(1019004); // You are not allowed to travel there.
            }

            else if (SpellHelper.IsWindLoc(loc))
            {
                Caster.SendLocalizedMessage(1019004); // You are not allowed to travel there.
            }

            else if (boat != null && boat.Owner != Caster)
            {
                Caster.SendLocalizedMessage(1019004); // You are not allowed to travel there.
            }

            else if (m_Book != null && m_Book.CurCharges <= 0)
            {
                Caster.SendLocalizedMessage(502412); // There are no charges left on that item.
            }

            else if (CheckSequence())
            {
                if (m_Book != null)
                    --m_Book.CurCharges;

                var astralLeash = Caster.Backpack.FindItemByType<AstralLeash>();
                if (astralLeash != null && !astralLeash.Deleted && astralLeash.CanConsume() && Caster.Followers > 0)
                {
                    if (BaseCreature.TeleportPets(Caster, loc, map, false))
                        astralLeash.Consume();
                }

                Point3D sourceLocation = Caster.Location;
                Map sourceMap = Caster.Map;

                Point3D targetLocation = loc;
                Map targetMap = map;

                //Player Enhancement Customization: PhaseShift
                bool phaseShift = PlayerEnhancementPersistance.IsCustomizationEntryActive(Caster, CustomizationType.PhaseShift);

                if (phaseShift)
                {
                    Caster.MoveToWorld(loc, map);

                    Effects.PlaySound(sourceLocation, sourceMap, 0x1FC);
                    Effects.SendLocationEffect(sourceLocation, sourceMap, 0x3967, 30, 15, 2499, 0);

                    Effects.PlaySound(targetLocation, targetMap, 0x1FC);
                    Effects.SendLocationEffect(targetLocation, targetMap, 0x3967, 30, 15, 2499, 0);                    
                }

                else
                {
                    Caster.PlaySound(0x1FC);
                    Caster.MoveToWorld(loc, map);
                    Caster.PlaySound(0x1FC);
                }
            }

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private RecallSpell m_Owner;

			public InternalTarget( RecallSpell owner ) : base( 12, false, TargetFlags.None )
			{
				m_Owner = owner;

				owner.Caster.LocalOverheadMessage( MessageType.Regular, 0x3B2, 501029 ); // Select Marked item.
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is RecallRune )
				{
					RecallRune rune = (RecallRune)o;

					if ( rune.Marked )
						m_Owner.Effect( rune.Target, rune.TargetMap, true );
					else
						from.SendLocalizedMessage( 501805 ); // That rune is not yet marked.
				}

				else if ( o is Runebook )
				{
					RunebookEntry e = ((Runebook)o).Default;

					if ( e != null )
						m_Owner.Effect( e.Location, e.Map, true );
					else
						from.SendLocalizedMessage( 502354 ); // Target is not marked.
				}

                else if (o is BoatRune)
                {
                    BoatRune rune = (BoatRune)o;
                    BaseBoat m_Boat;

                    if (rune.m_Boat != null)
                    {
                        m_Boat = rune.m_Boat;

                        if (m_Boat.Deleted)
                        {
                            from.SendMessage("The boat bound to this rune no longer exists.");     
                            return;
                        }

                        if (m_Boat.Owner == from)
                        {
                            m_Boat.TransferEmbarkedMobile(from);
                            m_Owner.Effect(m_Boat.GetRandomEmbarkLocation(true), m_Boat.Map, false);
                        }
                        else
                            from.SendMessage("You must be the owner of that ship to use this rune.");
                    }

                    else                    
                        from.SendMessage("The boat bound to this rune no longer exists.");                                   
                }

				else if ( o is HouseRaffleDeed && ((HouseRaffleDeed)o).ValidLocation() )
				{
					HouseRaffleDeed deed = (HouseRaffleDeed)o;

					m_Owner.Effect( deed.PlotLocation, deed.PlotFacet, true );
				}

				else
				{
					from.Send( new MessageLocalized( from.Serial, from.Body, MessageType.Regular, 0x3B2, 3, 502357, from.Name, "" ) ); // I can not recall from that object.
				}
			}
			
			protected override void OnNonlocalTarget( Mobile from, object o )
			{
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}