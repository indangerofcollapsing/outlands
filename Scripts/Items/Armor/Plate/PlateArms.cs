using System;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x1410, 0x1417 )]
	public class PlateArms : BaseArmor
	{
        public override int InitMinHits{ get{ return 60; } }
		public override int InitMaxHits{ get{ return 100; } }

        public override int ArmorBase { get { return 40; } }
        public override int OldDexBonus { get { return -1; } }

        public override int IconItemId { get { return 5143; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 4; } }
        public override int IconOffsetY { get { return 4; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Plate; } }
        public override CraftResource DefaultResource { get { return CraftResource.Iron; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorMeditationAllowance.None; } }

		[Constructable]
		public PlateArms() : base( 5143 )
		{
            Name = "platemail arms";
			Weight = 5.0;
		}

		public PlateArms( Serial serial ) : base( serial )
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
				Weight = 5.0;
		}
	}
}