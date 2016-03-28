using System;
using Server.Achievements;
using Server.Mobiles;

namespace Server.Items
{
	public class TambourineTassel : BaseInstrument
	{
		[Constructable]
		public TambourineTassel() : base( 0xE9E, 0x52, 0x53 )
		{
            Name = "tambourine";
			Weight = 2.0;
		}

		public TambourineTassel( Serial serial ) : base( serial )
		{
		}

		public override void PlayInstrumentWell(Mobile from)
		{
			base.PlayInstrumentWell(from);
			// IPY ACHIEVEMENT 
			if (from is PlayerMobile)
				AchievementSystem.Instance.TickProgress(from, AchievementTriggers.Trigger_PlayTambourineWell);
			// IPY ACHIEVEMENT 
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