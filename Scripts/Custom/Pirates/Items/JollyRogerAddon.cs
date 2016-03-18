namespace Server.Items
{
	public class JollyRogerAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] 
        {
			  {509, -1, 0, 0},
              {506, -1, -1, 0},
              {508, 0, 0, 0},
              {507, 0, -1, 0}
		};
            
		public override BaseAddonDeed Deed { get { return new JollyRogerAddonDeed(); }}

		[ Constructable ]
		public JollyRogerAddon()
		{
            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent(new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );
        }

        public JollyRogerAddon(Serial serial): base(serial)
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

	public class JollyRogerAddonDeed : BaseAddonDeed
	{
        public override int PlayerClassCurrencyValue { get { return 10000; } }

		public override BaseAddon Addon { get { return new JollyRogerAddon(); } }

		[Constructable]
		public JollyRogerAddonDeed()
		{
            Name = "a jolly roger deed";

            PlayerClass = PlayerClass.Pirate;
            PlayerClassRestricted = false;
		}

		public JollyRogerAddonDeed( Serial serial ) : base( serial )
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

            PlayerClass = PlayerClass.Pirate;
            PlayerClassRestricted = false;
		}
	}
}