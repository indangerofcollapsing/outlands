using System; 
using System.Collections; 
using Server.Items; 
using Server.ContextMenus; 
using Server.Misc; 
using Server.Network; 

namespace Server.Mobiles 
{ 
	public class DeathKnightKing : BaseCreature 
	{ 
		[Constructable] 
		public DeathKnightKing() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 ) 
		{ 
			SpeechHue = Utility.RandomDyedHue(); 
			Title = "the Death Knight"; 
			Hue = 1;

			if ( this.Female = Utility.RandomBool() ) 
			{ 
				this.Body = 0x191; 
				this.Name = NameList.RandomName( "female" ); 
				AddItem( new Skirt( Utility.RandomRedHue() ) ); 
			} 
			else 
			{ 
				this.Body = 0x190; 
				this.Name = NameList.RandomName( "male" ); 
				AddItem( new FancyShirt( Utility.RandomRedHue() ) ); 
			} 

			SetStr( 800, 900 );
			SetDex( 140, 200 );
			SetInt( 161, 175 );

			SetHits( 900, 1200 );

			SetDamage( 8, 15 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 35, 45 );
			SetResistance( ResistanceType.Fire, 25, 30 );
			SetResistance( ResistanceType.Cold, 25, 30 );
			SetResistance( ResistanceType.Poison, 10, 20 );
			SetResistance( ResistanceType.Energy, 10, 20 );

			SetSkill( SkillName.Anatomy, 125.0 );
			SetSkill( SkillName.Fencing, 46.0, 77.5 );
			SetSkill( SkillName.Macing, 35.0, 57.5 );
			SetSkill( SkillName.Poisoning, 60.0, 82.5 );
			SetSkill( SkillName.MagicResist, 130.5, 145.5 );
			SetSkill( SkillName.Swords, 145.0 );
			SetSkill( SkillName.Tactics, 145.0 );
			SetSkill( SkillName.Lumberjacking, 125.0 );
			SetSkill( SkillName.Parry, 125.0 );

			Fame = 25000;
			Karma = -25000;

			VirtualArmor = 40;
			
			PlateArms arms = new PlateArms();
			arms.Hue = 0x966;
			arms.LootType = LootType.Regular;
			AddItem( arms );
			
			PlateLegs legs = new PlateLegs();
			legs.Hue = 0x966;
			legs.LootType = LootType.Regular;
			AddItem( legs );

			PlateGorget gorget = new PlateGorget();
			gorget.Hue = 0x966;
			gorget.LootType = LootType.Regular;
			AddItem( gorget );

			PlateChest tunic = new PlateChest();
			tunic.Hue = 0x966;
			tunic.LootType = LootType.Regular;
			AddItem( tunic );

			PlateGloves gloves = new PlateGloves();
			gloves.Hue = 0x966;
			gloves.LootType = LootType.Regular;
			AddItem( gloves );

			VikingSword weapon = new VikingSword();
			weapon.Hue = 0x846;
			weapon.MinDamage = 15;
			weapon.MaxDamage = 35;
			weapon.LootType = LootType.Blessed;
			weapon.Movable = false;
			AddItem( weapon );

			MetalKiteShield shield = new MetalKiteShield();
			shield.Movable = true;
			AddItem( shield );

			Item hair = new Item( Utility.RandomList( 0x203B, 0x2049, 0x2048, 0x204A ) ); 
			hair.Hue = Utility.RandomNondyedHue(); 
			hair.Layer = Layer.Hair; 
			hair.Movable = false; 
			AddItem( hair ); 


		} 
    //public override int GoldWorth { get { return Utility.RandomMinMax(750, 1200); } }


		public override bool AlwaysMurderer{ get{ return true; } }
		public override bool AutoDispel{ get{ return true; } }
		//public override int TreasureMapLevel{ get{ return 4; } }

		public DeathKnightKing( Serial serial ) : base( serial ) 
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