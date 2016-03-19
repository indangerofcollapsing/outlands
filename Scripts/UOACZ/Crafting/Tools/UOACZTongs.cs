using System;
using Server;
using Server.Engines.Craft;

namespace Server.Items
{
	[FlipableAttribute( 0xfbb, 0xfbc )]
	public class UOACZTongs : BaseTool
	{
        public override CraftSystem CraftSystem { get { return UOACZDefBlacksmithy.CraftSystem; } }

        public override int MaxUses { get { return 50; } }

		[Constructable]
		public UOACZTongs() : base( 0xFBB )
		{
            Name = "tongs";

			Weight = 1.0;

            UsesRemaining = MaxUses;
		}

		[Constructable]
		public UOACZTongs( int uses ) : base( uses, 0xFBB )
		{
            Name = "tongs";

			Weight = 1.0;

            UsesRemaining = MaxUses;
		}

		public UOACZTongs( Serial serial ) : base( serial )
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