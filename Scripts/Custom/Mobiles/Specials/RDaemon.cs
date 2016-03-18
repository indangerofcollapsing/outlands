using System;
using Server;
using Server.Items;
using Server.Factions;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Spells.Seventh;
using Server.Spells.Sixth;
using Server.Achievements;

namespace Server.Mobiles
{
	public class rDaemon : Daemon
	{
		[Constructable]
		public rDaemon () : base()
		{
			Name = string.Format("{0} (rare)", NameList.RandomName( "daemon" ));
			Hue = 2101;
		}

		public rDaemon( Serial serial ) : base( serial )
		{
		}

		public override void OnDeath(Container c)
		{
			base.OnDeath(c);

			// IPY ACHIEVEMENT TRIGGER 
			AwardAchievementForKiller(AchievementTriggers.Trigger_RareDemon);
			// END IPY ACHIEVEMENT TRIGGER
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}
