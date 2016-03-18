using System;
using System.Collections;
using Server.Items;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Spells.Sixth;
using Server.Targeting;
using Server.Achievements;

namespace Server.Mobiles
{
	[CorpseName( "a skeletal corpse" )]
	public class rSkeleton : Skeleton
	{
		[Constructable]
		public rSkeleton() : base()
		{
			Name = "a skeleton (rare)";
			Hue = 2101;

		}

		public rSkeleton( Serial serial ) : base( serial )
		{
		}

		public override void OnDeath(Container c)
		{
			base.OnDeath(c);

			// IPY ACHIEVEMENT TRIGGER 
			AwardAchievementForKiller(AchievementTriggers.Trigger_RareSkeleton);
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
