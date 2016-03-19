using System; 
using Server; 
using Server.Items;

namespace Server.Items
{ 
   public class BagofStuddedArmor: Bag 
   { 
		[Constructable] 
		public BagofStuddedArmor()
		{ 
			Movable = true; 
			Hue = 0x65; 
			Name = "a bag of Studded Armor";

			StuddedChest chest = new StuddedChest();
			chest.Quality = ArmorQuality.Exceptional;
			chest.LootType = LootType.Blessed;
			DropItem( chest );

			StuddedArms arms = new StuddedArms ();
			arms.Quality = ArmorQuality.Exceptional;
			arms.LootType = LootType.Blessed;
			DropItem( arms );

			StuddedGloves gloves = new StuddedGloves();
			gloves.Quality = ArmorQuality.Exceptional;
			gloves.LootType = LootType.Blessed;
			DropItem( gloves );

			StuddedGorget gorget = new StuddedGorget();
			gorget.Quality = ArmorQuality.Exceptional;
			gorget.LootType = LootType.Blessed;
			DropItem( gorget );

			StuddedLegs legs = new StuddedLegs();
			legs.Quality = ArmorQuality.Exceptional;
			legs.LootType = LootType.Blessed;
			DropItem( legs );
		}

      public BagofStuddedArmor( Serial serial ) : base( serial ) 
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
