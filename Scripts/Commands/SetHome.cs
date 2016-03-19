using System; 
using Server; 
using Server.Misc; 
using Server.Targeting;
using Server.Mobiles;

namespace Server.Commands 
{ 
	public class SetHome 
	{ 
    	public static void Initialize() 
		{ 
			CommandSystem.Register( "SetHome", AccessLevel.GameMaster, new CommandEventHandler( SetHome_OnCommand ) ); 
		} 

		[Usage( "SetHome <range>" )] 
		[Description( "Moves a vendor to your current location, and sets their range to the specified number" )] 
		[Aliases( "SetHome" )] 
		public static void SetHome_OnCommand( CommandEventArgs e ) 
		{ 
			Mobile from = e.Mobile;
			if ( e.Length == 1 ) 
			{ 
				
                int range= 0;
                if (int.TryParse(e.GetString(0).Trim(), out range))
                {

                    from.SendMessage(String.Format("Set home location to {0} and the range to {1} for which mobile?", from.Location, range));
                    from.Target = new RangeTarget(range);
                }
                else
                {
                    e.Mobile.SendMessage(0x25, "Bad Format: SetHome <range>");      
                }
				
			} 
			else 
			{ 
				e.Mobile.SendMessage ( 0x25, "Bad Format: SetHome <range>" );      
			}
		} 

		private class RangeTarget : Target
		{
			private int m_Range;
			public RangeTarget( int range ) : base( 10, false, TargetFlags.None )
			{
				m_Range = range;
			}

			protected override void OnTarget( Mobile from, object targ )
			{
                if (targ != null && targ is BaseCreature)
                {
                    BaseCreature m = (BaseCreature)targ;

                    m.Home = from.Location;
                    m.RangeHome = m_Range;
                    m.Location = from.Location;

                }
                else
                {
                    from.SendMessage("Bad Target.");
                }
			}
		}
	} 
}