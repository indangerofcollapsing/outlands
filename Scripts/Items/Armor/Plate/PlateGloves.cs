using System;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x1414, 0x1418 )]
	public class PlateGloves : BaseArmor
	{
		public override int InitMinHits{ get{ return 60; } }
		public override int InitMaxHits{ get{ return 100; } }

        public override int ArmorBase { get { return 40; } }
        public override int OldDexBonus { get { return 0; } }

        public override int IconItemId { get { return 5144; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 3; } }
        public override int IconOffsetY { get { return 5; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Plate; } }
        public override CraftResource DefaultResource { get { return CraftResource.Iron; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorMeditationAllowance.None; } }

		[Constructable]
		public PlateGloves() : base( 5144 )
		{
            Name = "platemail gloves";
			Weight = 2.0;
		}

		public PlateGloves( Serial serial ) : base( serial )
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