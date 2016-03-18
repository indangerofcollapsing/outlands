namespace Server.Items
{
	public class TreasurePileSmallAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] 
        {
			  {7003, 0, 0, 0},
              {7018, 0, -1, 0},
              {6977, -1, -1, 0},
              {6991, 0, -1, 1}
		};
            
		public override BaseAddonDeed Deed { get { return new TreasurePileSmallAddonDeed(); }}

		[ Constructable ]
		public TreasurePileSmallAddon()
		{
            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent(new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );
        }

        public TreasurePileSmallAddon(Serial serial): base(serial)
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

	public class TreasurePileSmallAddonDeed : BaseAddonDeed
	{
        public override BaseAddon Addon { get { return new TreasurePileSmallAddon(); } }

		[Constructable]
		public TreasurePileSmallAddonDeed()
		{
            Name = "a small treasure pile deed";
		}

		public TreasurePileSmallAddonDeed( Serial serial ) : base( serial )
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