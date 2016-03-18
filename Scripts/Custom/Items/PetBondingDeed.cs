// Designed By: EternaL
// Part of the Armagedon scripts
using System; 
using Server.Network; 
using Server.Prompts; 
using Server.Items; 
using Server.Mobiles;
using Server.Targeting; 

namespace Server.Items 
{ 
   public class InternalTarget : Target 
   { 
      private PetBondingDeed m_Deed;

      public InternalTarget( PetBondingDeed deed ) : base( 1, false, TargetFlags.None ) 
      { 
         m_Deed = deed; 
      } 

      protected override void OnTarget( Mobile from, object target ) 
      { 
         if ( target is BaseCreature ) 
         { 
            BaseCreature t = ( BaseCreature ) target; 

            if ( t.IsBonded )
            { 
               from.SendMessage( "That pet is already bonded!" );
            } 
            else if ( t.ControlMaster != from ) 
            { 
               from.SendMessage( "That is not your pet!" ); 
            } 
            else  
            { 
               m_Deed.Use(from, t); // Use the deed 
            } 
            
         } 
         else 
         { 
            from.SendMessage( "That is not a valid traget." );  
         } 
      } 
   } 

   public class PetBondingDeed : Item // Create the item class which is derived from the base item class 
   { 
      protected int m_Charges;
     [CommandProperty(AccessLevel.GameMaster)]
     public int Charges { get { return m_Charges; } set { m_Charges = value; } }

      [Constructable] 
      public PetBondingDeed() : base( 0x14F0 ) 
      { 
         Weight = 1.0; 
         Name = "a pet bonding deed"; 
   	   Hue = 572;
         m_Charges = 5;
      } 

      public PetBondingDeed( Serial serial ) : base( serial ) 
      { 
      }

      public override void OnSingleClick(Mobile from)
     {
         base.OnSingleClick(from);
         LabelTo(from, String.Format("[charges: {0}]", m_Charges));
     }

      public bool Use(Mobile from, BaseCreature pet)
      {
         if (Deleted)
             return false;

         if (!IsChildOf(from.Backpack))
         {
             from.SendLocalizedMessage(1010095); //Must be on your person to use.
             return false;
         }

         if (pet == null || pet.Deleted)
             return false;

         if (!pet.Controlled || pet.ControlMaster != from)
         {
             from.SendMessage("The pet must belong to you!");
             return false;
         }

         if (pet.IsBonded)
         {
            from.SendMessage("That pet is already bonded!");
            return false;
         }

         int chargesNeeded = pet.ControlSlots;

         if (m_Charges - chargesNeeded < 0)
         {
             from.SendMessage("You do not have enough charges to bond that pet.");
             return false;
         }

         pet.IsBonded = true;

         from.SendMessage("You have bonded your pet.");

         m_Charges -= chargesNeeded;
         if (m_Charges <= 0)
         {
             from.SendMessage("You have used up the pet bonding deed.");
             Delete();
         }

         return true;
      }

      public override void Serialize( GenericWriter writer ) 
      { 
         base.Serialize( writer ); 

         writer.Write( (int) 1 ); // version 

         writer.Write( m_Charges );
      } 

      public override void Deserialize( GenericReader reader ) 
      { 
         base.Deserialize( reader ); 

         int version = reader.ReadInt();

         if (version > 0)
            m_Charges = reader.ReadInt();
         else
            m_Charges = 5;

      } 

      public override bool DisplayLootType{ get{ return false; } } 

      public override void OnDoubleClick( Mobile from ) // Override double click of the deed to call our target 
      { 
         if ( !IsChildOf( from.Backpack ) ) // Make sure its in their pack 
         { 
             from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it. 
         } 
         else 
         { 
            from.SendMessage( "Choose the pet you wish to bond with." );  
            from.Target = new InternalTarget( this ); // Call our target 
          } 
      }   
   } 
}