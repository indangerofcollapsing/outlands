using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
    public class OgreLordMask : OrcMask
	{
        public override string DefaultName {
            get {
                return "ogre lord mask";
            }
        }
		public override int BaseFireResistance{ get{ return 4; } }

		public override int InitMinHits{ get{ return 150; } }
		public override int InitMaxHits{ get{ return 150; } }

		[Constructable]
		public OgreLordMask()
		{
            Hue = 2101;
		}

        public OgreLordMask(Serial serial)
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
