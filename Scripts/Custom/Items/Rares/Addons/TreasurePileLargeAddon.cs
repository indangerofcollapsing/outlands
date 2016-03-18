namespace Server.Items
{
	public class TreasurePileLargeAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] 
        {
			  {6995, 0, 0, 0},
              {7002, 0, -1, 0},
              {7019, 0, -2, 0},
              {7008, -1, 0, 0},
              {7018, -1, -2, 0},
              {6992, 1, -2, 0},
              {7001, -1, -1, 0},
              {6977, -2, 0, 0},
              {6989, -2, -1, 0},
              {6996, -2, -1, 0},
              {6992, -1, -2, 0},
              {7013, -1, 0, 0},
		};
            
		public override BaseAddonDeed Deed { get { return new TreasurePileLargeAddonDeed(); }}

		[ Constructable ]
		public TreasurePileLargeAddon()
		{
            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent(new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );
        }

        public TreasurePileLargeAddon(Serial serial): base(serial)
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

	public class TreasurePileLargeAddonDeed : BaseAddonDeed
	{
        public override int PlayerClassCurrencyValue { get { return 10000; } }

		public override BaseAddon Addon { get { return new TreasurePileLargeAddon(); } }

		[Constructable]
		public TreasurePileLargeAddonDeed()
		{
            Name = "a large treasure pile deed";
		}

		public TreasurePileLargeAddonDeed( Serial serial ) : base( serial )
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