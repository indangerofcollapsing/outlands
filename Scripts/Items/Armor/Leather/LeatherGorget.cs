using System;
using Server.Items;

namespace Server.Items
{
	public class LeatherGorget : BaseArmor
	{
		public override int BasePhysicalResistance{ get{ return 2; } }
		public override int BaseFireResistance{ get{ return 4; } }
		public override int BaseColdResistance{ get{ return 3; } }
		public override int BasePoisonResistance{ get{ return 3; } }
		public override int BaseEnergyResistance{ get{ return 3; } }
     
		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 37; } }

		public override int AosStrReq{ get{ return 20; } }
		public override int OldStrReq{ get{ return 10; } }

		public override int RevertArmorBase{ get{ return 1; } }

        public override int IconItemId { get { return 5063; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 8; } }
        public override int IconOffsetY { get { return 8; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Leather; } }
		public override CraftResource DefaultResource{ get{ return CraftResource.RegularLeather; } }

        public override int ArmorBase { get { return 15; } }
        public override int OldDexBonus { get { return 0; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorMeditationAllowance.All; } }

		[Constructable]
		public LeatherGorget() : base( 5063 )
		{
            Name = "leather gorget";
			Weight = 1.0;
		}

		public LeatherGorget( Serial serial ) : base( serial )
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