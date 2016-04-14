using System;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x144f, 0x1454 )]
	public class LichChest : BaseArmor
	{
		public override int BasePhysicalResistance{ get{ return 3; } }
		public override int BaseFireResistance{ get{ return 3; } }
		public override int BaseColdResistance{ get{ return 4; } }
		public override int BasePoisonResistance{ get{ return 2; } }
		public override int BaseEnergyResistance{ get{ return 4; } }

        //IPY Values
		public override int InitMinHits{ get{ return 20; } }
		public override int InitMaxHits{ get{ return 25; } }

		public override int AosStrReq{ get{ return 60; } }
		public override int OldStrReq{ get{ return 40; } }
     
        //IPY value
		public override int OldDexBonus{ get{ return 0; } }

		public override int ArmorBase{ get{ return 30; } }

        //IPY Value
		public override int RevertArmorBase{ get{ return 13; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Leather; } }
		public override CraftResource DefaultResource{ get{ return CraftResource.RegularLeather; } }

		[Constructable]
		public LichChest() : base( 0x144F )
		{
			Weight = 6.0;
			Hue = 2101;
			Name = "lich chest";
		}

		public LichChest( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );

			if ( Weight == 1.0 )
				Weight = 6.0;
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}