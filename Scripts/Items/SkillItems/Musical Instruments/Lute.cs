using System;
using Server.Achievements;
using Server.Mobiles;

namespace Server.Items
{
	public class Lute : BaseInstrument
	{
		[Constructable]
		public Lute() : base( 0xEB3, 0x4C, 0x4D )
		{
            Name = "lute";
			Weight = 3.0;
		}

        public override void PlayInstrumentWell(Mobile from)
        {
            base.PlayInstrumentWell(from);
            // IPY ACHIEVEMENT 
            if (from is PlayerMobile)
                AchievementSystem.Instance.TickProgress(from, AchievementTriggers.Trigger_PlayLuteWell);
            // IPY ACHIEVEMENT 
        }

		public Lute( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}		

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}