using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
    public class TrollMask : OrcMask
	{
        public override string DefaultName {
            get {
                return "troll mask";
            }
        }
		public override int BaseFireResistance{ get{ return 4; } }

		public override int InitMinHits{ get{ return 150; } }
		public override int InitMaxHits{ get{ return 150; } }

		[Constructable]
		public TrollMask()
		{
            Hue = 2221;
		}

        public TrollMask(Serial serial)
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
