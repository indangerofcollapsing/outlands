using System;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x1c00, 0x1c01 )]
	public class LeatherShorts : BaseArmor
	{
		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 37; } }

        public override int ArmorBase { get { return 15; } }
        public override int OldDexBonus { get { return 0; } }

        public override int IconItemId { get { return 7169; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return -6; } }
        public override int IconOffsetY { get { return 9; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Leather; } }
		public override CraftResource DefaultResource{ get{ return CraftResource.RegularLeather; } }

		public override ArmorMeditationAllowance DefMedAllowance{ get{ return ArmorMeditationAllowance.All; } }

		public override bool AllowMaleWearer{ get{ return false; } }

		[Constructable]
		public LeatherShorts() : base( 7169 )
		{
            Name = "leather shorts";
			Weight = 3.0;
		}

		public LeatherShorts( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}