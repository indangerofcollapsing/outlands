using System;
using Server;

namespace Server.Items
{
    public class WaterTubAddon : BaseAddon, IWaterSource
	{
        public override BaseAddonDeed Deed { get { return new WaterTubDeed(); } }

		[Constructable]
        public WaterTubAddon()
		{
			AddComponent( new AddonComponent( 5465 ), 0, 0, 0 );
            AddComponent(new AddonComponent(5460), 0, -1, 0);
            AddComponent(new AddonComponent(5343), 0, -2, 0);
            AddComponent(new AddonComponent(5462), 2, 0, 0);
            AddComponent(new AddonComponent(5457), 0, 2, 0);
            AddComponent(new AddonComponent(5335), 1, 1, 0);
            AddComponent(new AddonComponent(5458), -1, 0, 0);
            AddComponent(new AddonComponent(5342), -2, 0, 0);
            AddComponent(new AddonComponent(5464), -1, -1, 0);
		}

		public WaterTubAddon( Serial serial ) : base( serial )
		{
		}

        public int Quantity
        {
            get { return 5000; }
            set { }
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

    public class WaterTubDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new WaterTubAddon(); } }
        public override string DefaultName {
            get {
                return "water tub deed";
            }
        }

        [Constructable]
        public WaterTubDeed() {
        }

        public WaterTubDeed(Serial serial)
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