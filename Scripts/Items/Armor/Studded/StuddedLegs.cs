using System;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x13da, 0x13e1 )]
	public class StuddedLegs : BaseArmor
	{
		public override int InitMinHits{ get{ return 35; } }
		public override int InitMaxHits{ get{ return 45; } }

        public override int ArmorBase { get { return 20; } }
        public override int OldDexBonus { get { return 0; } }

        public override int IconItemId { get { return 5089; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return -2; } }
        public override int IconOffsetY { get { return 2; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Studded; } }
        public override CraftResource DefaultResource { get { return CraftResource.RegularLeather; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorMeditationAllowance.ThreeQuarter; } }

		[Constructable]
		public StuddedLegs() : base( 5089 )
		{
            Name = "studded leggings";
			Weight = 5.0;
		}

		public StuddedLegs( Serial serial ) : base( serial )
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

			if ( Weight == 3.0 )
				Weight = 5.0;
		}
	}
}