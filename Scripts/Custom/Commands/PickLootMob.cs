using System;
using Server;
using Server.Targeting;
using Server.Commands;
using Server.Commands.Generic;
using Server.Mobiles;

namespace Server.Targets
{
	public class PickLootMob : Target
	{

        private int m_Flag;

		public PickLootMob(int flag) : base( -1, false, TargetFlags.None )
		{
            m_Flag = flag;
		}
       
		protected override void OnTarget( Mobile from, object o )
		{
			if ( !BaseCommand.IsAccessible( from, o ) )
			{
				from.SendMessage( "That is not accessible." );
				return;
			}

			if ( o is Mobile )
				from.Target = new DropLoot.DropLootTarget( (Mobile)o, m_Flag); //Pass targeted mobile to new target, target item to be gifted.
		}
	}
}