using System;
using Server.Mobiles;
using Server.Factions;
using Server.Spells;
using Server.Spells.Seventh;
using Server.Spells.Sixth;
using Server.Spells.Third;
using Server.Achievements;
using Server.Items;

namespace Server.Mobiles
{
	public class rSilverSerpent : SilverSerpent
	{

		[Constructable]
		public rSilverSerpent() : base()
		{
			Name = "a silver serpent (rare)";
			Hue = 2101;
		}

		public override void OnDeath(Container c)
		{
			base.OnDeath(c);

			// IPY ACHIEVEMENT TRIGGER 
			AwardAchievementForKiller(AchievementTriggers.Trigger_RareSilverSerpent);
			// END IPY ACHIEVEMENT TRIGGER
		}

		public rSilverSerpent(Serial serial) : base(serial)
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
