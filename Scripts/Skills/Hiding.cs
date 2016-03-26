using System;
using Server.Targeting;
using Server.Items;
using Server.Network;
using Server.Multis;
using Server.Mobiles;

namespace Server.SkillHandlers
{
	public class Hiding
	{
		private static bool m_CombatOverride;

		public static bool CombatOverride
		{
			get{ return m_CombatOverride; }
			set{ m_CombatOverride = value; }
		}

		public static void Initialize()
		{
            SkillInfo.Table[(int)SkillName.Hiding].Callback = new SkillUseCallback(OnUse);
		}

		public static TimeSpan OnUse( Mobile mobile )
		{
            if (!mobile.CanBeginAction(typeof(Hiding)))
            {
                mobile.SendMessage("You must wait a few moments to use another skill.");

                if (mobile.m_HidingTimer != null)
                {
                    double secondsRemaining = ((mobile.m_HidingTimer.m_StartTime + TimeSpan.FromSeconds(10)) - DateTime.UtcNow).TotalSeconds;
                    
                    return TimeSpan.FromSeconds(secondsRemaining);
                }

                return TimeSpan.FromSeconds(10);
            }
            
            if ( mobile.Target != null || mobile.Spell != null )
			{
				mobile.SendLocalizedMessage( 501238 ); // You are busy doing something else and cannot hide.
				return TimeSpan.FromSeconds( 1.0 );
			}

			double bonus = 0.0;

			BaseHouse house = BaseHouse.FindHouseAt( mobile );

			if ( house != null && house.IsFriend( mobile ) )
			{
				bonus = 100.0;
			}

			else if ( !Core.AOS )
			{
				if ( house == null )
					house = BaseHouse.FindHouseAt( new Point3D( mobile.X - 1, mobile.Y, 127 ), mobile.Map, 16 );

				if ( house == null )
					house = BaseHouse.FindHouseAt( new Point3D( mobile.X + 1, mobile.Y, 127 ), mobile.Map, 16 );

				if ( house == null )
					house = BaseHouse.FindHouseAt( new Point3D( mobile.X, mobile.Y - 1, 127 ), mobile.Map, 16 );

				if ( house == null )
					house = BaseHouse.FindHouseAt( new Point3D( mobile.X, mobile.Y + 1, 127 ), mobile.Map, 16 );

				if ( house != null )
					bonus = 50.0;
			}
			
			int range = 18 - (int)(mobile.Skills[SkillName.Hiding].Value / 10);
            //Added by IPY
			//int range = 18 - (int)(m.Skills[SkillName.Hiding].Value / 10);

			bool badCombat = ( !m_CombatOverride && mobile.Combatant != null && mobile.InRange( mobile.Combatant.Location, range ) && mobile.Combatant.InLOS( mobile ) );
			bool ok = ( !badCombat /*&& m.CheckSkill( SkillName.Hiding, 0.0 - bonus, 100.0 - bonus )*/ );

			if ( ok )
			{
				if ( !m_CombatOverride )
				{
                    IPooledEnumerable eable = mobile.GetMobilesInRange(range);

					foreach ( Mobile check in eable)
					{						
						if ( check.InLOS( mobile ) && check.Combatant == mobile )
						{
							badCombat = true;
							ok = false;
							break;
						}
					}

                    eable.Free();
				}
			
				ok = ( !badCombat && mobile.CheckSkill( SkillName.Hiding, 0.0 - bonus, 100.0 - bonus ) );
			}

			if ( badCombat )
			{
				mobile.RevealingAction();
				mobile.LocalOverheadMessage( MessageType.Regular, 0x22, 501237 ); // You can't seem to hide right now.

				return TimeSpan.FromSeconds( 1.0 );
			}

			else 
			{
				if ( ok )
				{
					mobile.Combatant = null;
					mobile.Hidden = true;
					mobile.Warmode = false;
					mobile.LocalOverheadMessage( MessageType.Regular, 0x1F4, 501240 ); // You have hidden yourself well.

                    if (mobile is PlayerMobile)
                    {
                        PlayerMobile pm = mobile as PlayerMobile;
                        pm.TrueHidden = true;
                    }

                    //Clear Invisibility Spell Timer if Exists
                    if (Spells.Sixth.InvisibilitySpell.HasTimer(mobile))                    
                        Spells.Sixth.InvisibilitySpell.RemoveTimer(mobile);

                    mobile.BeginAction((typeof(Hiding)));
                    Timer.DelayCall(TimeSpan.FromSeconds(9.9), delegate { mobile.EndAction(typeof(Hiding)); });

                    mobile.BeginAction((typeof(Stealth)));
                    Timer.DelayCall(TimeSpan.FromSeconds(9.9), delegate { mobile.EndAction(typeof(Stealth)); });

                    mobile.m_HidingTimer = null;
                    mobile.m_HidingTimer = new Mobile.HidingTimer(mobile, DateTime.UtcNow, false);
                    mobile.m_HidingTimer.Start();
				}

				else
				{
					mobile.RevealingAction();
					mobile.LocalOverheadMessage( MessageType.Regular, 0x22, 501241 ); // You can't seem to hide here.
				}

				return TimeSpan.FromSeconds( 10.0 );
			}
		}
	}
}