using System;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x13eb, 0x13f2 )]
	public class ChainGloves : BaseArmor
	{
		public override int InitMinHits{ get{ return 40; } }
		public override int InitMaxHits{ get{ return 50; } }
		
        public override int ArmorBase { get { return 30; } }
        public override int OldDexBonus { get { return 0; } }

        public override int IconItemId { get { return 5106; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 3; } }
        public override int IconOffsetY { get { return 5; } }

        public override CraftResource DefaultResource { get { return CraftResource.Iron; } }
        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Chainmail; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorMeditationAllowance.Quarter; } }

		[Constructable]
		public ChainGloves() : base( 5106 )
		{
            Name = "chainmail gloves";
            Hue = 2500;

			Weight = 2.0;
		}

		public ChainGloves( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
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