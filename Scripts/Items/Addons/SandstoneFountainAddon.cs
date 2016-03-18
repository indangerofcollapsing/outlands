using System;
using Server;

namespace Server.Items
{
	public class SandstoneFountainAddon : BaseAddon
	{
        public override BaseAddonDeed Deed
        {
            get
            {
                return new SandstoneFountainDeed();
            }
        }
		[Constructable]
		public SandstoneFountainAddon()
		{
			int itemID = 0x19C3;

			AddComponent( new AddonComponent( itemID++ ), -2, +1, 0 );
			AddComponent( new AddonComponent( itemID++ ), -1, +1, 0 );
			AddComponent( new AddonComponent( itemID++ ), +0, +1, 0 );
			AddComponent( new AddonComponent( itemID++ ), +1, +1, 0 );

			AddComponent( new AddonComponent( itemID++ ), +1, +0, 0 );
			AddComponent( new AddonComponent( itemID++ ), +1, -1, 0 );
			AddComponent( new AddonComponent( itemID++ ), +1, -2, 0 );

			AddComponent( new AddonComponent( itemID++ ), +0, -2, 0 );
			AddComponent( new AddonComponent( itemID++ ), +0, -1, 0 );
			AddComponent( new AddonComponent( itemID++ ), +0, +0, 0 );

			AddComponent( new AddonComponent( itemID++ ), -1, +0, 0 );
			AddComponent( new AddonComponent( itemID++ ), -2, +0, 0 );

			AddComponent( new AddonComponent( itemID++ ), -2, -1, 0 );
			AddComponent( new AddonComponent( itemID++ ), -1, -1, 0 );

			AddComponent( new AddonComponent( itemID++ ), -1, -2, 0 );
			AddComponent( new AddonComponent( ++itemID ), -2, -2, 0 );
		}

		public SandstoneFountainAddon( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
    }


    public class SandstoneFountainDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new SandstoneFountainAddon(); } }

        [Constructable]
        public SandstoneFountainDeed()
        {
            Name = "Sandstone Fountain";
        }

        public SandstoneFountainDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

}