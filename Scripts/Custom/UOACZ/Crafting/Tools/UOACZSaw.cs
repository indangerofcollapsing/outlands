using System;
using Server;
using Server.Engines.Craft;

namespace Server.Items
{
	[FlipableAttribute( 0x1034, 0x1035 )]
	public class UOACZSaw : BaseTool
	{
        public override CraftSystem CraftSystem { get { return UOACZDefCarpentry.CraftSystem; } }

        public override int MaxUses { get { return 50; } }

		[Constructable]
		public UOACZSaw() : base( 0x1034 )
		{
            Name = "saw";

			Weight = 1.0;

            UsesRemaining = MaxUses;
		}

		[Constructable]
		public UOACZSaw( int uses ) : base( uses, 0x1034 )
		{
            Name = "saw";

			Weight = 1.0;

            UsesRemaining = MaxUses;
		}

		public UOACZSaw( Serial serial ) : base( serial )
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