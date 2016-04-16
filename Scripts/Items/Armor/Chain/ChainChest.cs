using System;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x13bf, 0x13c4 )]
	public class ChainChest : BaseArmor
	{
		public override int InitMinHits{ get{ return 45; } }
		public override int InitMaxHits{ get{ return 60; } }		

        public override int ArmorBase { get { return 30; } }
        public override int OldDexBonus { get { return -3; } }

        public override int IconItemId { get { return 5060; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 3; } }
        public override int IconOffsetY { get { return 2; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Chainmail; } }
        public override CraftResource DefaultResource { get { return CraftResource.Iron; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorMeditationAllowance.Quarter; } }

		[Constructable]
		public ChainChest() : base( 5060 )
		{
            Name = "chainmail chest";
			Weight = 7.0;
		}

		public ChainChest( Serial serial ) : base( serial )
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