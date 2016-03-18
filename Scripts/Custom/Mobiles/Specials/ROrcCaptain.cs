using System;
using System.Collections;
using Server.Misc;
using Server.Items;
using Server.Targeting;
using Server.Achievements;

namespace Server.Mobiles
{
	public class rOrcCaptain : OrcCaptain
	{

		[Constructable]
		public rOrcCaptain() : base()
		{
			Name = string.Format("{0} (rare)", NameList.RandomName( "orc" ));
			Hue = 2101;
		}

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            // IPY ACHIEVEMENT TRIGGER 
            AwardAchievementForKiller(AchievementTriggers.Trigger_RareOrc);
            // END IPY ACHIEVEMENT TRIGGER

        }

		public rOrcCaptain( Serial serial ) : base( serial )
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
