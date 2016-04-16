using System;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x144f, 0x1454 )]
	public class BoneChest : BaseArmor
	{
		public override int InitMinHits{ get{ return 20; } }
		public override int InitMaxHits{ get{ return 25; } }

        public override int ArmorBase { get { return 25; } }
        public override int OldDexBonus { get { return -2; } }

        public override int IconItemId { get { return 5204; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return -3; } }
        public override int IconOffsetY { get { return -3; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Bone; } }
        public override CraftResource DefaultResource { get { return CraftResource.RegularLeather; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorMeditationAllowance.Quarter; } }

		[Constructable]
		public BoneChest() : base( 5204 )
		{
            Name = "bone chest";
			Weight = 6.0;
		}

		public BoneChest( Serial serial ) : base( serial )
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