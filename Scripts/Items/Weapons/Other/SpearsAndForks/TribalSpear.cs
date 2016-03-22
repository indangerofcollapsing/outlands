using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0xF62, 0xF63 )]
	public class TribalSpear : BaseSpear
	{
		public override string DefaultName
		{
			get { return "a tribal spear"; }
		}

		[Constructable]
		public TribalSpear() : base( 0xF62 )
		{
			Weight = 7.0;
			Hue = 837;
		}

		public TribalSpear( Serial serial ) : base( serial )
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
}