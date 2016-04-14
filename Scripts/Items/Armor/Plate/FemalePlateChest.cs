using System;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x1c04, 0x1c05 )]
	public class FemalePlateChest : BaseArmor
	{
		public override int BasePhysicalResistance{ get{ return 5; } }
		public override int BaseFireResistance{ get{ return 3; } }
		public override int BaseColdResistance{ get{ return 2; } }
		public override int BasePoisonResistance{ get{ return 3; } }
		public override int BaseEnergyResistance{ get{ return 2; } }
		public override int InitMinHits{ get{ return 51; } }
		public override int InitMaxHits{ get{ return 65; } }

		public override int AosStrReq{ get{ return 95; } }
		public override int OldStrReq{ get{ return 45; } }
		public override int OldDexBonus{ get{ return -4; } }

		public override bool AllowMaleWearer{ get{ return false; } }

		public override int ArmorBase{ get{ return 30; } }
		public override int RevertArmorBase{ get{ return 13; } }

        public override int IconItemId { get { return 7173; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 1; } }
        public override int IconOffsetY { get { return 2; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Plate; } }

		public override ArmorMeditationAllowance DefMedAllowance{ get{ return ArmorMeditationAllowance.None; } }

		[Constructable]
		public FemalePlateChest() : base( 7173 )
		{
            Name = "female platemail chest";
			Weight = 4.0;
		}

		public FemalePlateChest( Serial serial ) : base( serial )
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
				Weight = 4.0;
		}
	}
}