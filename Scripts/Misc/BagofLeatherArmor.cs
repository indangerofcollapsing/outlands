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
            chest.Quality = Quality.Exceptional;
			chest.LootType = LootType.Blessed;
			DropItem( chest );

			LeatherArms arms = new LeatherArms ();
            arms.Quality = Quality.Exceptional;
			arms.LootType = LootType.Blessed;
			DropItem( arms );

			LeatherGloves gloves = new LeatherGloves();
            gloves.Quality = Quality.Exceptional;
			gloves.LootType = LootType.Blessed;
			DropItem( gloves );

			LeatherGorget gorget = new LeatherGorget();
            gorget.Quality = Quality.Exceptional;
			gorget.LootType = LootType.Blessed;
			DropItem( gorget );

			LeatherLegs legs = new LeatherLegs();
            legs.Quality = Quality.Exceptional;
			legs.LootType = LootType.Blessed;
			DropItem( legs );

			LeatherCap cap = new LeatherCap();
            cap.Quality = Quality.Exceptional;
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
