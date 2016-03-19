using System;
using Server;
using Server.Engines.Craft;

namespace Server.Items
{
	public class UOACZMortarPestle : BaseTool
	{
        public override CraftSystem CraftSystem { get { return UOACZDefAlchemy.CraftSystem; } }

        public override int MaxUses { get { return 50; } }

		[Constructable]
		public UOACZMortarPestle() : base( 0xE9B )
		{
            Name = "mortar and pestle";

			Weight = 1.0;

            UsesRemaining = MaxUses; 
		}

		[Constructable]
		public UOACZMortarPestle( int uses ) : base( uses, 0xE9B )
		{
            Name = "mortar and pestle";

			Weight = 1.0;

            UsesRemaining = MaxUses;
		}

		public UOACZMortarPestle( Serial serial ) : base( serial )
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