using System;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using Server.Targeting;
using Server.Regions;
using Server.Custom;
using System.Collections;
using System.Collections.Generic;

namespace Server.SkillHandlers
{
	public class DetectHidden
	{
        public static int HouseSearchRange = 24;
        public static int MaxSearchRadius = 8;

		public static void Initialize()
		{
			SkillInfo.Table[(int)SkillName.DetectHidden].Callback = new SkillUseCallback( OnUse );
		}

		public static TimeSpan OnUse( Mobile src )
		{
			src.SendLocalizedMessage( 500819 );//Where will you search?
			src.Target = new InternalTarget();

			return TimeSpan.FromSeconds( 6.0 );
		}        

		private class InternalTarget : Target
		{
			public InternalTarget() : base( 12, true, TargetFlags.None )
			{
			}

			protected override void OnTarget( Mobile from, object target )
			{
				bool foundAnyone = false;

                from.RevealingAction();
                from.NextSkillTime = Core.TickCount + (int)(SkillCooldown.DetectHiddenCooldown * 1000);

				Point3D p;

				if ( target is Mobile )
					p = ((Mobile)target).Location;

				else if ( target is Item )
					p = ((Item)target).Location;

				else if ( target is IPoint3D )
					p = new Point3D( (IPoint3D)target );

				else 
					p = from.Location;

                //Boat Searching: Automatic Success for Owner, Co-Owner, Owner 
                BaseBoat boat = BaseBoat.FindBoatAt(p, from.Map);

                if (boat != null)
                {
                    if (!boat.Contains(from))
                    {
                        from.SendMessage("You must be onboard this boat in order to search it.");
                        return;
                    }

                    if (boat.IsFriend(from) || boat.IsCoOwner(from) || boat.IsOwner(from))
                    {
                        List<Mobile> m_MobilesOnBoard = boat.GetMobilesOnBoat(false, true);

                        foreach (Mobile mobile in m_MobilesOnBoard)
                        {
                            if (mobile == from)
                                continue;

                            if (mobile.Hidden && !mobile.RevealImmune && from.AccessLevel >= mobile.AccessLevel)
                            {
                                mobile.RevealingAction();
                                mobile.SendLocalizedMessage(500814); // You have been revealed!

                                foundAnyone = true;
                            }
                        }

                        if (foundAnyone)
                            from.SendMessage("You reveal what was hidden.");  
 
                        else
                            from.SendMessage("You search the decks and find no one hiding onboard.");                        
                    }

                    return;
                }

                //House Searching: Automatic Success for Owner, Co-Owner, Owner 
                BaseHouse house = BaseHouse.FindHouseAt(p, from.Map, 16);

                if (house != null)
                {
                    if (!house.Contains(from.Location))
                    {
                        from.SendMessage("You must be inside this house in order to search it.");
                        return;
                    }

                    if (house.IsFriend(from) || house.IsCoOwner(from) || house.IsOwner(from))
                    {
                        IPooledEnumerable nearbyMobiles = from.Map.GetMobilesInRange(p, HouseSearchRange);

                        foreach (Mobile mobile in nearbyMobiles)
                        {
                            if (mobile == from)
                                continue;

                            BaseHouse mobileHouse = BaseHouse.FindHouseAt(p, from.Map, 16);

                            if (mobile == null || mobileHouse != house)
                                continue;

                            if (mobile.Hidden && !mobile.RevealImmune && from.AccessLevel >= mobile.AccessLevel)
                            {
                                mobile.RevealingAction();
                                mobile.SendLocalizedMessage(500814); // You have been revealed!

                                foundAnyone = true;
                            }
                        }                        

                        nearbyMobiles.Free();

                        if (foundAnyone)
                            from.SendMessage("You reveal what was hidden.");  

                        else
                            from.SendMessage("You search the home and find no one hiding within.");
                    }

                    return;
                }

                from.CheckSkill(SkillName.DetectHidden, 0.0, 100.0, 1.0);

                double successChance = (from.Skills[SkillName.DetectHidden].Value / 100);

                int searchRadius = (int)(Math.Floor(from.Skills[SkillName.DetectHidden].Value / 100) * MaxSearchRadius);

                if (Utility.RandomDouble() <= successChance)
                {
                    IPooledEnumerable nearbyMobiles = from.Map.GetMobilesInRange(p, searchRadius);

                    foreach (Mobile mobile in nearbyMobiles)
                    {
                        if (mobile == from)
                            continue;

                        if (mobile.Hidden && !mobile.RevealImmune && from.AccessLevel >= mobile.AccessLevel)
                        {
                            mobile.RevealingAction();
                            mobile.SendLocalizedMessage(500814); // You have been revealed!

                            foundAnyone = true;
                        }
                    }                        

                    nearbyMobiles.Free();

                    if (foundAnyone)
                        from.SendMessage("You reveal what was hidden.");  

                    else
                        from.SendMessage("You search the area but find nothing hidden.");
                }

                else
                {
                    from.SendMessage("You are not certain what lies hidden nearby.");
                    return;
                }
			}
		}
	}
}
