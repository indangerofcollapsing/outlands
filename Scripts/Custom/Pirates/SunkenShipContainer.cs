/***************************************************************************
 *                              SunkenShipContainer.cs
 *                            ------------------
 *   begin                : August 2010
 *   author               : Sean Stavropoulos
 *   email                : sean.stavro@gmail.com
 *
 *
 ***************************************************************************/
using System;

namespace Server.Items
{
	public class SunkenShipContainer : SmallCrate
	{
        DateTime m_Created;
        
        [Constructable]
		public SunkenShipContainer() : base()
		{
            Name = "a sunken ship hold";
            m_Created = DateTime.UtcNow;
		}

		public SunkenShipContainer( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

            if ((DateTime.UtcNow > m_Created + Custom.Pirates.PirateFishing.m_SunkenShipDecayTime) && Map == Map.Internal)
            {
                Timer m_Timer = Timer.DelayCall(TimeSpan.FromMinutes(1), delegate { Delete(); });
            }
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}