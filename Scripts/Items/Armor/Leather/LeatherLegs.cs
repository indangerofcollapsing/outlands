using System;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x13cb, 0x13d2 )]
	public class LeatherLegs : BaseArmor
	{       
		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 37; } }		

        public override int ArmorBase { get { return 15; } }
        public override int OldDexBonus { get { return 0; } }

        public override int IconItemId { get { return 5074; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 1; } }
        public override int IconOffsetY { get { return 2; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Leather; } }
        public override CraftResource DefaultResource { get { return CraftResource.RegularLeather; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorMeditationAllowance.All; } }

		[Constructable]
		public LeatherLegs() : base( 5074 )
		{
            Name = "leather leggings";
			Weight = 4.0;
		}

		public LeatherLegs( Serial serial ) : base( serial )
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