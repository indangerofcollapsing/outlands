namespace Server.Items
{
	public class ShadowBannerEastAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] 
        {
			  {13917, -1, 0, 0},
              {13916, 0, 0, 0},
		};
            
		public override BaseAddonDeed Deed { get { return new ShadowBannerEastAddonDeed(); }}

		[ Constructable ]
		public ShadowBannerEastAddon()
		{
            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent(new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );
        }

        public ShadowBannerEastAddon(Serial serial): base(serial)
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

            PlayerClass = PlayerClass.Murderer;
            PlayerClassRestricted = false;
		}
	}

	public class ShadowBannerEastAddonDeed : BaseAddonDeed
	{
        public override int PlayerClassCurrencyValue { get { return 5000; } }

		public override BaseAddon Addon { get { return new ShadowBannerEastAddon(); } }

		[Constructable]
		public ShadowBannerEastAddonDeed()
		{
            Name = "a shadow banner deed (east)";

            PlayerClass = PlayerClass.Murderer;
            PlayerClassRestricted = false;
		}

		public ShadowBannerEastAddonDeed( Serial serial ) : base( serial )
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

    public class ShadowBannerNorthAddon : BaseAddon
    {
        private static int[,] m_AddOnSimpleComponents = new int[,] 
        {
			  {13918, 0, -1, 0},
              {13919, 0, 0, 0},
		};

        public override BaseAddonDeed Deed { get { return new ShadowBannerNorthAddonDeed(); } }

        [Constructable]
        public ShadowBannerNorthAddon()
        {
            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent(new AddonComponent(m_AddOnSimpleComponents[i, 0]), m_AddOnSimpleComponents[i, 1], m_AddOnSimpleComponents[i, 2], m_AddOnSimpleComponents[i, 3]);
        }

        public ShadowBannerNorthAddon(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class ShadowBannerNorthAddonDeed : BaseAddonDeed
    {
        public override int PlayerClassCurrencyValue { get { return 5000; } }

        public override BaseAddon Addon { get { return new ShadowBannerNorthAddon(); } }

        [Constructable]
        public ShadowBannerNorthAddonDeed()
        {
            Name = "a shadow banner deed (north)";

            PlayerClass = PlayerClass.Murderer;
            PlayerClassRestricted = false;
        }

        public ShadowBannerNorthAddonDeed(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}