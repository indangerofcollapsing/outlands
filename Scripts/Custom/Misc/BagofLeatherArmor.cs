using System; 
using Server; 
using Server.Items;

namespace Server.Items
{ 
   public class BagofLeatherArmor: Bag 
   { 
		[Constructable] 
		public BagofLeatherArmor()
		{ 
			Movable = true; 
			Hue = 0x55; 
			Name = "a bag of Leather Armor";

			LeatherChest chest = new LeatherChest();
			chest.Quality = ArmorQuality.Exceptional;
			chest.LootType = LootType.Blessed;
			DropItem( chest );

			LeatherArms arms = new LeatherArms ();
			arms.Quality = ArmorQuality.Exceptional;
			arms.LootType = LootType.Blessed;
			DropItem( arms );

			LeatherGloves gloves = new LeatherGloves();
			gloves.Quality = ArmorQuality.Exceptional;
			gloves.LootType = LootType.Blessed;
			DropItem( gloves );

			LeatherGorget gorget = new LeatherGorget();
			gorget.Quality = ArmorQuality.Exceptional;
			gorget.LootType = LootType.Blessed;
			DropItem( gorget );

			LeatherLegs legs = new LeatherLegs();
			legs.Quality = ArmorQuality.Exceptional;
			legs.LootType = LootType.Blessed;
			DropItem( legs );

			LeatherCap cap = new LeatherCap();
			cap.Quality = ArmorQuality.Exceptional;
			cap.LootType = LootType.Blessed;
			DropItem( cap );
		}

      public BagofLeatherArmor( Serial serial ) : base( serial ) 
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
