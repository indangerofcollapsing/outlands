using System;
using Server;
using Server.Engines.Craft;

namespace Server.Items
{
	public class UOACZSewingKit : BaseTool
	{
        public override CraftSystem CraftSystem { get { return UOACZDefTailoring.CraftSystem; } }

        public override int MaxUses { get { return 50; } }

		[Constructable]
		public UOACZSewingKit() : base( 0xF9D )
		{
			Weight = 1.0;

            UsesRemaining = MaxUses;  
		}

		[Constructable]
		public UOACZSewingKit( int uses ) : base( uses, 0xF9D )
		{
			Weight = 1.0;

            UsesRemaining = MaxUses;  
		}

		public UOACZSewingKit( Serial serial ) : base( serial )
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