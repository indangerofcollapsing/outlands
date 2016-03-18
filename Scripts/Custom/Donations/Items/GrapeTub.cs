using System;
using Server;

namespace Server.Items
{
	public class GrapeTubAddon : BaseAddon
	{
        public override BaseAddonDeed Deed { get { return new GrapeTubDeed(); } }

		[Constructable]
        public GrapeTubAddon()
		{
			AddComponent( new AddonComponent( 5409 ), 0, 0, 0 );
            AddComponent(new AddonComponent(5411), 0, -1, 0);
            AddComponent(new AddonComponent(5343), 0, -2, 0);
            AddComponent(new AddonComponent(5412), 2, 0, 0);
            AddComponent(new AddonComponent(5408), 0, 2, 0);
            AddComponent(new AddonComponent(5335), 1, 1, 0);
            AddComponent(new AddonComponent(5410), -1, 0, 0);
            AddComponent(new AddonComponent(5342), -2, 0, 0);
            AddComponent(new AddonComponent(5406), -1, -1, 0);
		}

		public GrapeTubAddon( Serial serial ) : base( serial )
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

    public class GrapeTubDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new GrapeTubAddon(); } }
        public override string DefaultName {
            get {
                return "grape tub deed";
            }
        }

        [Constructable]
        public GrapeTubDeed() {
        }

        public GrapeTubDeed(Serial serial)
            : base(serial) {
        }

        public override void Serialize(GenericWriter writer) {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader) {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}