using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Achievements;


namespace Server.Mobiles
{
	public class rHarpy : Harpy
	{
		[Constructable]
		public rHarpy() : base()
		{
			Name = "a harpy (rare)";
			Hue = 2101;
		}

		public override void OnDeath(Container c)
		{
			base.OnDeath(c);

			// IPY ACHIEVEMENT TRIGGER 
			AwardAchievementForKiller(AchievementTriggers.Trigger_RareHarpy);
			// END IPY ACHIEVEMENT TRIGGER
		}

		public rHarpy( Serial serial ) : base( serial )
		{
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
