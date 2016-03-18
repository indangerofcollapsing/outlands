namespace Server.Items
{
	public class FallenLogEastAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] 
        {
			  {3316, 0, 1, 0},
              {3315, 0, 0, 0} //1 2	
		};
            
		public override BaseAddonDeed Deed { get { return new FallenLogEastAddonDeed(); }}

		[ Constructable ]
		public FallenLogEastAddon()
		{
            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent(new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );
        }

		public FallenLogEastAddon( Serial serial ) : base( serial )
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

	public class FallenLogEastAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new FallenLogEastAddon();
			}
		}

		[Constructable]
		public FallenLogEastAddonDeed()
		{
            Name = "a fallen log (east)";
		}

		public FallenLogEastAddonDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // Version
		}

		public override void	Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}