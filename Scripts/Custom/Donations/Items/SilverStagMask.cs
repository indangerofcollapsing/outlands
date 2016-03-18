using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
    public class ArmoredSilverStagMask : ArmoredDeerMask
    {
        public override string DefaultName
        {
            get
            {
                return "armored silver stag mask";
            }
        }
        [Constructable]
		public ArmoredSilverStagMask()
		{
            Hue = 2102;
		}

        public ArmoredSilverStagMask(Serial serial)
            : base(serial)
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
    }

    public class SilverStagMask : DeerMask
	{
        public override string DefaultName {
            get {
                return "silver stag mask";
            }
        }
		public override int BaseFireResistance{ get{ return 4; } }

		public override int InitMinHits{ get{ return 150; } }
		public override int InitMaxHits{ get{ return 150; } }

		[Constructable]
		public SilverStagMask()
		{
            Hue = 2102;
		}

        public SilverStagMask(Serial serial)
            : base(serial)
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
