using System;
using Server;
using Server.Engines.Craft;

namespace Server.Items
{
	[FlipableAttribute( 0x1022, 0x1023 )]
	public class UOACZFletcherTools : BaseTool
	{
        public override CraftSystem CraftSystem { get { return UOACZDefBowFletching.CraftSystem; } }

        public override int MaxUses { get { return 50; } }

		[Constructable]
		public UOACZFletcherTools() : base( 0x1022 )
		{
            Name = "fletcher tools";

			Weight = 1.0;

            UsesRemaining = MaxUses;
		}

		[Constructable]
		public UOACZFletcherTools( int uses ) : base( uses, 0x1022 )
		{
            Name = "fletcher tools";

			Weight = 1.0;

            UsesRemaining = MaxUses; 
		}

		public UOACZFletcherTools( Serial serial ) : base( serial )
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