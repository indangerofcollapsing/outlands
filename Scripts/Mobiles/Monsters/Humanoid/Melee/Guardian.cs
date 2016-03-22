using System; 
using System.Collections; 
using Server.Misc; 
using Server.Items; 
using Server.Mobiles; 

namespace Server.Mobiles 
{ 
	public class Guardian : BaseCreature 
	{ 
		[Constructable] 
		public Guardian() : base( AIType.AI_Archer, FightMode.Aggressor, 10, 1, 0.2, 0.4 ) 
		{ 
			InitStats( 100, 125, 25 ); 
			Title = "the guardian"; 

			SpeechHue = Utility.RandomDyedHue(); 

			Hue = Utility.RandomSkinHue(); 

			if ( Female = Utility.RandomBool() ) 
			{ 
				Body = 0x191; 
				Name = NameList.RandomName( "female" ); 
			} 

			else 
			{ 
				Body = 0x190; 
				Name = NameList.RandomName( "male" ); 
			} 

			Bow bow = new Bow(); 

			bow.Movable = false; 
			bow.Quality = WeaponQuality.Exceptional; 

			AddItem( bow ); 

			PackItem( new Arrow( 250 ) );
			PackGold( 250, 500 );

			Skills[SkillName.Anatomy].Base = 120.0; 
			Skills[SkillName.Tactics].Base = 120.0; 
			Skills[SkillName.Archery].Base = 120.0; 
			Skills[SkillName.MagicResist].Base = 120.0; 
			Skills[SkillName.DetectHidden].Base = 100.0;
		} 

		public Guardian( Serial serial ) : base( serial ) 
		{ 
		} 

		public override void Serialize( GenericWriter writer ) 
		{ 
			base.Serialize( writer ); 
			writer.Write( (int) 0 ); // version 
		} 

		public override void Deserialize( GenericReader reader ) 
		{ 
			base.Deserialize( reader ); 
			int version = reader.ReadInt(); 
		} 
	} 
}