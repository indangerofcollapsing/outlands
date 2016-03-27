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

        public static bool ValidTarget(Mobile from, Mobile target)
        {
            var playerFrom = from as PlayerMobile;
            var playerTarget = target as PlayerMobile;

            if (playerFrom == null || playerTarget == null)
                return true;

            return true;
        }

		private class InternalTarget : Target
		{
			public InternalTarget() : base( 12, true, TargetFlags.None )
			{
			}

			protected override void OnTarget( Mobile from, object target )
			{
				bool foundAnyone = false;

				Point3D p;

				if ( target is Mobile )
					p = ((Mobile)target).Location;

				else if ( target is Item )
					p = ((Item)target).Location;

				else if ( target is IPoint3D )
					p = new Point3D( (IPoint3D)target );

				else 
					p = from.Location;

                //Boat Searching: Similar to House Search
                BaseBoat boat = BaseBoat.FindBoatAt(p, from.Map);

                if (boat != null)
                {
                    if (!boat.Contains(from))
                    {
                        from.SendMessage("You must be onboard this boat in order to search it.");
                        return;
                    }

                    //Auto-Reveal Similar to Houses for Friends, Co-Owners, Owners of the Boat
                    if (boat.IsFriend(from) || boat.IsCoOwner(from) || boat.IsOwner(from))
                    {
                        List<Mobile> m_MobilesOnBoard = boat.GetMobilesOnBoat(false, true);

                        foreach (Mobile mobile in m_MobilesOnBoard)
                        {
                            if (mobile.Hidden && !mobile.RevealImmune && from.AccessLevel >= mobile.AccessLevel)
                            {
                                mobile.RevealingAction();
                                mobile.SendLocalizedMessage(500814); // You have been revealed!
                                foundAnyone = true;
                            }
                        }

                        if (!foundAnyone)
                            from.SendMessage("You search the decks and find no one hiding onboard.");

                        return;
                    }

                    //Non-Friendly Players Proceed to Search as Normal
                    else
                    {
                    }
                }

                from.NextSkillTime = Core.TickCount + (int)(SkillCooldown.DetectHiddenCooldown * 1000);

				double srcSkill = from.Skills[SkillName.DetectHidden].Value;
				int range = (int)(srcSkill / 10.0);

                if (!from.CheckSkill(SkillName.DetectHidden, 0.0, 100.0, 1.0))
                    range /= 2;

                BaseHouse house = BaseHouse.FindHouseAt( p, from.Map, 16 );                

				bool inHouse = ( house != null && house.IsFriend( from ) );

				if ( inHouse )
					range = 22;

				if ( range > 0 )
				{
					IPooledEnumerable inRange = from.Map.GetMobilesInRange( p, range );

                    foreach (Mobile trg in inRange)
                    {
                        if (trg.Hidden && from != trg && DetectHidden.ValidTarget(from, trg))
                        {
                            double ss = srcSkill + Utility.Random(21) - 10;
                            double ts = trg.Skills[SkillName.Hiding].Value + Utility.Random(21) - 10;

                            if (!trg.RevealImmune && from.AccessLevel >= trg.AccessLevel && (ss >= ts || (inHouse && house.IsInside(trg))))
                            {
                                trg.RevealingAction();
                                trg.SendLocalizedMessage(500814); // You have been revealed!
                                foundAnyone = true;
                            }
                        }
                    }

                    inRange.Free();
				}

				if ( !foundAnyone )				
					from.SendLocalizedMessage( 500817 ); // You can see nothing hidden there.				
			}
		}
	}
}
