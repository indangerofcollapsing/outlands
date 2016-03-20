// Delceri

namespace Server.Items
{
	public class FallenLogSouthAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,]
        {
			  {3317, -1, 0, 0},
              {3319, 1, 0, 0},
              {3318, 0, 0, 0} // 1	2 3	
		}; 
            
		public override BaseAddonDeed Deed { get { return new FallenLogSouthAddonDeed(); } }

		[ Constructable ]
		public FallenLogSouthAddon()
		{
            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );
        }

		public FallenLogSouthAddon( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class FallenLogSouthAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon { get { return new FallenLogSouthAddon(); } }

		[Constructable]
		public FallenLogSouthAddonDeed()
		{
			Name = "a fallen log (south)";
		}

		public FallenLogSouthAddonDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}