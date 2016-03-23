using System; 
using Server; 
using Server.Items;

namespace Server.Items
{ 
   public class BagOfRingmailArmor: Bag 
   { 
		[Constructable] 
		public BagOfRingmailArmor()
		{ 
			Movable = true; 
			Hue = 0x45; 
			Name = "a bag of Ringmail Armor";

			RingmailChest chest = new RingmailChest();
            chest.Quality = Quality.Exceptional;
			chest.LootType = LootType.Blessed;
			DropItem( chest );

			RingmailArms arms = new RingmailArms ();
            arms.Quality = Quality.Exceptional;
			arms.LootType = LootType.Blessed;
			DropItem( arms );

			RingmailGloves gloves = new RingmailGloves();
            gloves.Quality = Quality.Exceptional;
			gloves.LootType = LootType.Blessed;
			DropItem( gloves );

			RingmailLegs legs = new RingmailLegs();
            legs.Quality = Quality.Exceptional;
			legs.LootType = LootType.Blessed;
			DropItem( legs );
		}

      public BagOfRingmailArmor( Serial serial ) : base( serial ) 
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
