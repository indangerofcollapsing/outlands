using System;
using Server.Items;

namespace Server.Items
{
	public class BoneGorget : BaseArmor
	{		
		public override int InitMinHits{ get{ return 60; } }
		public override int InitMaxHits{ get{ return 100; } }		

        public override int ArmorBase { get { return 25; } }
        public override int OldDexBonus { get { return 0; } }

        public override int IconItemId { get { return 5139; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 6; } }
        public override int IconOffsetY { get { return 9; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Bone; } }
        public override CraftResource DefaultResource { get { return CraftResource.RegularLeather; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorMeditationAllowance.None; } }

		[Constructable]
		public BoneGorget() : base( 5139 )
		{
            Name = "bone gorget";
            Hue = 2958;

			Weight = 2.0;
		}

		public BoneGorget( Serial serial ) : base( serial )
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