using System;
using Server;

namespace Server.Items
{
	public class Blood : Item
	{
		[Constructable]
        public Blood(): this(Utility.RandomList(0x1645, 0x122A, 0x122B, 0x122C, 0x122D, 0x122E, 0x122F))
		//public Blood() : this( 0x1645 )
		{
		}

		[Constructable]
		public Blood( int itemID ) : base( itemID )
		{
			Movable = false;

			new InternalTimer( this, TimeSpan.FromSeconds(5) ).Start();
		}

        [Constructable]
        public Blood(TimeSpan duration): base(Utility.RandomList(0x1645, 0x122A, 0x122B, 0x122C, 0x122D, 0x122E, 0x122F))
        {
            Movable = false;

            new InternalTimer(this, duration).Start();
        }

		public Blood( Serial serial ) : base( serial )
		{
            new InternalTimer(this, TimeSpan.FromSeconds(5)).Start();
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

		private class InternalTimer : Timer
		{
			private Item m_Blood;

            public InternalTimer(Item blood, TimeSpan duration): base(duration)
			{
				Priority = TimerPriority.OneSecond;

				m_Blood = blood;
			}

			protected override void OnTick()
			{
				m_Blood.Delete();
			}
		}
	}
}