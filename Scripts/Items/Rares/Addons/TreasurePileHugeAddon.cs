namespace Server.Items
{
	public class TreasurePileHugeAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] 
        {
			{7010, 0, 0, 0},
            {7000, 0, -1, 0},
            {6980, 0, -2, 0},
            {7007, 0, 1, 2},
            {6979, -1, 0, 0},
            {6999, -1, -1, 0},
            {6983, -1, -2, 0},
            {6996, -1, 1, 0},
            {6993, -2, 0, 0},
            {6997, -2, 1, 0},
            {6984, -2, -1, 0},
            {6989, -3, -1, 0},
            {6986, -3, 1, 0},
            {6990, -3, 0, 0},
            {6988, -4, 0, 0},
            {6987, -4, 1, 0},
            {6981, 1, -1, 0},
		};
            
		public override BaseAddonDeed Deed { get { return new TreasurePileHugeAddonDeed(); }}

		[ Constructable ]
		public TreasurePileHugeAddon()
		{
            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent(new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );
        }

        public TreasurePileHugeAddon(Serial serial): base(serial)
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

	public class TreasurePileHugeAddonDeed : BaseAddonDeed
	{
        public override int PlayerClassCurrencyValue { get { return 10000; } }

		public override BaseAddon Addon { get { return new TreasurePileHugeAddon(); } }

		[Constructable]
		public TreasurePileHugeAddonDeed()
		{
            Name = "a huge treasure pile deed";
		}

		public TreasurePileHugeAddonDeed( Serial serial ) : base( serial )
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