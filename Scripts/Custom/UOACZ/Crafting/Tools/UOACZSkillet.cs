using System;
using Server;
using Server.Engines.Craft;

namespace Server.Items
{
	public class UOACZSkillet : BaseTool
	{
		public override int LabelNumber{ get{ return 1044567; } } // SkilletUOACZ

        public override CraftSystem CraftSystem { get { return UOACZDefCooking.CraftSystem; } }

        public override int MaxUses { get { return 50; } }

		[Constructable]
		public UOACZSkillet() : base( 0x97F )
		{
            Name = "skillet";

			Weight = 1.0;

            UsesRemaining = MaxUses;      
		}

		[Constructable]
		public UOACZSkillet( int uses ) : base( uses, 0x97F )
		{
            Name = "skillet";

			Weight = 1.0;

            UsesRemaining = MaxUses;      
		}

		public UOACZSkillet( Serial serial ) : base( serial )
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