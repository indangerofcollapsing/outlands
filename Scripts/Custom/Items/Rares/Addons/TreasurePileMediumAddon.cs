namespace Server.Items
{
	public class TreasurePileMediumAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] 
        {
			  {7019, 0, 0, 0},
              {7017, -1, 0, 0},
              {6976, -1, 1, 0},
              {7011, -2, 1, 0},
              {7016, -2, 0, 0},
              {7014, -3, 0, 0},
              {6992, -2, 1, 0},
              {7013, -3, 1, 0},

              {6989, -2, 1, 4},
		};
            
		public override BaseAddonDeed Deed { get { return new TreasurePileMediumAddonDeed(); }}

		[ Constructable ]
		public TreasurePileMediumAddon()
		{
            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent(new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );
        }

        public TreasurePileMediumAddon(Serial serial): base(serial)
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

	public class TreasurePileMediumAddonDeed : BaseAddonDeed
	{
        public override int PlayerClassCurrencyValue { get { return 10000; } }

		public override BaseAddon Addon { get { return new TreasurePileMediumAddon(); } }

		[Constructable]
		public TreasurePileMediumAddonDeed()
		{
            Name = "a medium treasure pile deed";
		}

		public TreasurePileMediumAddonDeed( Serial serial ) : base( serial )
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