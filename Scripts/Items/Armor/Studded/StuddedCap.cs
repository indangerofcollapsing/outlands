using System;
using Server;

namespace Server.Items
{
	[FlipableAttribute( 0x1db9, 0x1dba )]
	public class StuddedCap : BaseArmor
	{
		public override int InitMinHits{ get{ return 30; } }
		public override int InitMaxHits{ get{ return 40; } }		

        public override int ArmorBase { get { return 20; } }
        public override int OldDexBonus { get { return 0; } }

        public override int IconItemId { get { return 7610; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 0; } }
        public override int IconOffsetY { get { return 8; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Studded; } }
        public override CraftResource DefaultResource { get { return CraftResource.RegularLeather; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorMeditationAllowance.ThreeQuarter; } }

		[Constructable]
		public StuddedCap() : base( 7610 )
		{
            Name = "studded cap";
            Hue = 1507;

			Weight = 2.0;
		}

		public StuddedCap( Serial serial ) : base( serial )
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