namespace Server.Items
{
	public class HorseBardingEastAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] 
        {
			  {4982, -1, 0, 0},
              {4983, 0, 0, 0},
		};
            
		public override BaseAddonDeed Deed { get { return new HorseBardingEastAddonDeed(); }}

		[ Constructable ]
		public HorseBardingEastAddon()
		{
            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent(new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );
        }

        public HorseBardingEastAddon(Serial serial): base(serial)
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

	public class HorseBardingEastAddonDeed : BaseAddonDeed
	{
        public override int PlayerClassCurrencyValue { get { return 5000; } }

		public override BaseAddon Addon { get { return new HorseBardingEastAddon(); } }

		[Constructable]
		public HorseBardingEastAddonDeed()
		{
            Name = "a horse barding deed (east)";

            PlayerClass = PlayerClass.Paladin;
            PlayerClassRestricted = false;
		}

		public HorseBardingEastAddonDeed( Serial serial ) : base( serial )
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

            PlayerClass = PlayerClass.Paladin;
            PlayerClassRestricted = false;
		}
	}

    public class HorseBardingNorthAddon : BaseAddon
    {
        private static int[,] m_AddOnSimpleComponents = new int[,] 
        {
			  {4985, 0, -1, 0},
              {4984, 0, 0, 0},
		};

        public override BaseAddonDeed Deed { get { return new HorseBardingNorthAddonDeed(); } }

        [Constructable]
        public HorseBardingNorthAddon()
        {
            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent(new AddonComponent(m_AddOnSimpleComponents[i, 0]), m_AddOnSimpleComponents[i, 1], m_AddOnSimpleComponents[i, 2], m_AddOnSimpleComponents[i, 3]);
        }

        public HorseBardingNorthAddon(Serial serial): base(serial)
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

    public class HorseBardingNorthAddonDeed : BaseAddonDeed
    {
        public override int PlayerClassCurrencyValue { get { return 5000; } }

        public override BaseAddon Addon { get { return new HorseBardingNorthAddon(); } }

        [Constructable]
        public HorseBardingNorthAddonDeed()
        {
            Name = "a horse barding deed (north)";

            PlayerClass = PlayerClass.Paladin;
            PlayerClassRestricted = false;
        }

        public HorseBardingNorthAddonDeed(Serial serial): base(serial)
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

            PlayerClass = PlayerClass.Paladin;
            PlayerClassRestricted = false;
        }
    }
}