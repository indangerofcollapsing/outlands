using System;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x13cd, 0x13c5 )]
	public class LeatherArms : BaseArmor
	{
		public override int InitMinHits{ get{ return 30; } }
		public override int InitMaxHits{ get{ return 40; } }		

        public override int ArmorBase { get { return 15; } }
        public override int OldDexBonus { get { return 0; } }

        public override int IconItemId { get { return 5061; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return -1; } }
        public override int IconOffsetY { get { return 4; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Leather; } }
        public override CraftResource DefaultResource { get { return CraftResource.RegularLeather; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorMeditationAllowance.All; } }

		[Constructable]
		public LeatherArms() : base( 5061 )
		{
            Name = "leather arms";
			Weight = 2.0;
		}

		public LeatherArms( Serial serial ) : base( serial )
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

			if ( Weight == 1.0 )
				Weight = 2.0;
		}
	}
}