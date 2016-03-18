using System;
using Server;
using Server.Network;

namespace Server.Items
{
	public class CrackedFloor : Item
	{
        private static readonly int m_MinimumCloudDamage = 70;
        private static readonly int m_MaximumCloudDamage = 99;

		private DeleteTimer m_DeleteTimer;
		
		[Constructable]
		public CrackedFloor() : base( Utility.RandomMinMax(0x1B01, 0x1B08) )
		{
            Hue = 38;
			Movable = false;
			m_DeleteTimer = new DeleteTimer(this);
			m_DeleteTimer.Start();				
		}
		
		public void BlowUp()
		{
			Effects.SendLocationEffect( Location, Map, 0x3728, 13 );
			var mobiles = Map.GetMobilesInRange(Location, 0);
			foreach(Mobile m in mobiles)
			{
				Server.Spells.SpellHelper.Damage(TimeSpan.FromTicks(1), m, Utility.RandomMinMax(m_MinimumCloudDamage, m_MaximumCloudDamage));
			}
            mobiles.Free();
		}
		
		public CrackedFloor( Serial serial ) : base( serial )
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
			
			switch ( version )
			{
				case 0:
				{
					m_DeleteTimer = new DeleteTimer(this);
					m_DeleteTimer.Start();
					break;
				}
			}					
		}
		
		private class DeleteTimer : Timer
		{
			private CrackedFloor m_Owner;
			
			public DeleteTimer( CrackedFloor owner ) : base( TimeSpan.FromSeconds( 5.0 ) )
			{
				m_Owner = owner;
				Priority = TimerPriority.OneSecond;
			}

			protected override void OnTick()
			{
                if (Utility.RandomBool())
				    m_Owner.BlowUp();

				m_Owner.Delete();
			}
		}			
	}
}