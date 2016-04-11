using System;
using Server.Items;

namespace Server.Items
{
	public class PlateGorget : BaseArmor
	{
		public override int BasePhysicalResistance{ get{ return 5; } }
		public override int BaseFireResistance{ get{ return 3; } }
		public override int BaseColdResistance{ get{ return 2; } }
		public override int BasePoisonResistance{ get{ return 3; } }
		public override int BaseEnergyResistance{ get{ return 2; } }
		
		public override int InitMinHits{ get{ return 60; } }
		public override int InitMaxHits{ get{ return 100; } }

		public override int AosStrReq{ get{ return 45; } }
		public override int OldStrReq{ get{ return 30; } }

        public override int RevertArmorBase{ get{ return 2; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Plate; } }

        public override int ArmorBase { get { return 40; } }
        public override int OldDexBonus { get { return 0; } }

        public override int IconItemId { get { return 5139; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 6; } }
        public override int IconOffsetY { get { return 9; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorMeditationAllowance.None; } }

		[Constructable]
		public PlateGorget() : base( 5139 )
		{
            Name = "platemail gorget";
			Weight = 2.0;
		}

		public PlateGorget( Serial serial ) : base( serial )
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