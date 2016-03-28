using System;
using Server.Mobiles;
using Server.Achievements;

namespace Server.Items
{
	public class Tambourine : BaseInstrument
	{
		[Constructable]
		public Tambourine() : base( 0xE9D, 0x52, 0x53 )
		{
            Name = "tambourine";
			Weight = 2.0;
		}

		public Tambourine( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void PlayInstrumentWell(Mobile from)
		{
			base.PlayInstrumentWell(from);
			// IPY ACHIEVEMENT 
			if (from is PlayerMobile)
				AchievementSystem.Instance.TickProgress(from, AchievementTriggers.Trigger_PlayTambourineWell);
			// IPY ACHIEVEMENT 
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}