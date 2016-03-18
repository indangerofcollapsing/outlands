using System; 
using Server; 
using Server.Items;

namespace Server.Items
{ 
   public class BagofPlateArmor: Bag 
   { 
		[Constructable] 
		public BagofPlateArmor()
		{ 
			Movable = true; 
			Hue = 0x25; 
			Name = "a bag of Plate Armor";

			PlateChest chest = new PlateChest();
			chest.Quality = ArmorQuality.Exceptional;
			chest.LootType = LootType.Blessed;
			DropItem( chest );

			PlateArms arms = new PlateArms ();
			arms.Quality = ArmorQuality.Exceptional;
			arms.LootType = LootType.Blessed;
			DropItem( arms );

			PlateGloves gloves = new PlateGloves();
			gloves.Quality = ArmorQuality.Exceptional;
			gloves.LootType = LootType.Blessed;
			DropItem( gloves );

			PlateGorget gorget = new PlateGorget();
			gorget.Quality = ArmorQuality.Exceptional;
			gorget.LootType = LootType.Blessed;
			DropItem( gorget );

			PlateLegs legs = new PlateLegs();
			legs.Quality = ArmorQuality.Exceptional;
			legs.LootType = LootType.Blessed;
			DropItem( legs );

			PlateHelm helm = new PlateHelm();
			helm.Quality = ArmorQuality.Exceptional;
			helm.LootType = LootType.Blessed;
			DropItem( helm );
		}

      public BagofPlateArmor( Serial serial ) : base( serial ) 
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
