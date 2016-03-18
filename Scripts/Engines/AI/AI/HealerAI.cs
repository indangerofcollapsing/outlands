using System;
using System.Collections;
using Server.Targeting;
using Server.Network;
using Server.Spells;
using Server.Spells.First;
using Server.Spells.Second;
using Server.Spells.Fourth;

namespace Server.Mobiles
{
	public class HealerAI : BaseAI
	{		
		public HealerAI( BaseCreature m ) : base( m )
		{
		}		
	}
}