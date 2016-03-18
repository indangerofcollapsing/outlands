using System; 
using Server; 
using Server.Items;

namespace Server.Items
{ 
   public class BagofChainArmor: Bag 
   { 
		[Constructable] 
		public BagofChainArmor()
		{ 
			Movable = true; 
			Hue = 0x35; 
			Name = "a bag of Chain Armor";

			ChainChest chest = new ChainChest();
			chest.Quality = ArmorQuality.Exceptional;
			chest.LootType = LootType.Blessed;
			DropItem( chest );

			ChainLegs legs = new ChainLegs();
			legs.Quality = ArmorQuality.Exceptional;
			legs.LootType = LootType.Blessed;
			DropItem( legs );

			ChainCoif coif = new ChainCoif();
			coif.Quality = ArmorQuality.Exceptional;
			coif.LootType = LootType.Blessed;
			DropItem( coif );
		}

      public BagofChainArmor( Serial serial ) : base( serial ) 
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
