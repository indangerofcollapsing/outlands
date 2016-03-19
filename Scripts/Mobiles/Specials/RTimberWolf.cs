using System;
using Server.Mobiles;
using Server.Achievements;
using Server.Items;

namespace Server.Mobiles
{
	public class rTimberWolf : TimberWolf
	{
		[Constructable]
		public rTimberWolf() : base()
		{
			Name = "a timber wolf (rare)";
			Hue = 2101;
		}

		public override void OnDeath(Container c)
		{
			base.OnDeath(c);

			// IPY ACHIEVEMENT TRIGGER 
			AwardAchievementForKiller(AchievementTriggers.Trigger_RareWolf);
			// END IPY ACHIEVEMENT TRIGGER
		}

		public rTimberWolf(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}
}
