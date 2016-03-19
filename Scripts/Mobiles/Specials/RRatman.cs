using System;
using System.Collections;
using Server.Misc;
using Server.Items;
using Server.Targeting;
using Server.Achievements;
namespace Server.Mobiles
{
	public class rRatman : Ratman
	{
		[Constructable]
		public rRatman() : base()
		{
			Name = string.Format("{0} (rare)", NameList.RandomName( "ratman" ));
			Hue = 2101;
		}

		public rRatman( Serial serial ) : base( serial )
		{
		}

		public override void OnDeath(Container c)
		{
			base.OnDeath(c);

			// IPY ACHIEVEMENT TRIGGER 
			AwardAchievementForKiller(AchievementTriggers.Trigger_RareRatman);
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
