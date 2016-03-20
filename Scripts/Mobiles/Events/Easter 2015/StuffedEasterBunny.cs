// Delceri
using System; 
using Server; 
using Server.Network; 

namespace Server.Items 
{ 
   public class StuffedEasterBunny : Item 
   { 
      [Constructable] 
      public StuffedEasterBunny() : base( 0x2125 ) 
      { 
         Name = "a stuffed easter bunny"; 
         Weight = 3;
         Hue = 1150;
      } 

      public override void OnDoubleClick( Mobile from ) 
      {
        from.SendSound(0x0C9);
        from.SendMessage("It doesn't look very alive.");
      }

      public StuffedEasterBunny(Serial serial): base(serial) 
      { 
      } 

      public override void Serialize( GenericWriter writer ) 
      { 
         base.Serialize( writer ); 
         writer.Write( (int) 0 ); //version 
      } 

      public override void Deserialize( GenericReader reader ) 
      { 
         base.Deserialize( reader ); 
         int version = reader.ReadInt(); 
      } 
   } 
}