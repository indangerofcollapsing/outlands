using System; 
using System.Collections; 
using Server.Items; 
using Server.ContextMenus; 
using Server.Misc; 
using Server.Network; 

namespace Server.Mobiles 
{ 	
	[CorpseName( "a sanguine defender corpse" )] 
	public class SanguinDefender : BaseSanguin 
	{ 
		[Constructable] 
		public SanguinDefender() : base() 
		{ 			
			Name = "a sanguine defender"; 

			SetStr( 75 );
			SetDex( 75 );
			SetInt( 25 );

			SetHits( 600 );

			SetDamage( 15, 25 ); 

            SetSkill(SkillName.Swords, 95);
            SetSkill(SkillName.Macing, 95);
            SetSkill(SkillName.Fencing, 95);

            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Parry, 30);
           
			SetSkill( SkillName.MagicResist, 75);

            VirtualArmor = 100;  

			Fame = 2500;
			Karma = -2500;

            Utility.AssignRandomHair(this, hairHue);

            AddItem(new ChainCoif() { Movable = false, Hue = itemHue });
            AddItem(new ChainChest() { Movable = false, Hue = itemHue });
            AddItem(new PlateArms() { Movable = false, Hue = weaponHue });
            AddItem(new LeatherGloves() { Movable = false, Hue = itemHue });
            AddItem(new Kilt() { Movable = false, Hue = weaponHue });
            AddItem(new Sandals() { Movable = false, Hue = itemHue });

            switch (Utility.RandomMinMax(1, 4))
            {
                case 1: AddItem(new Broadsword() { Movable = false, Hue = weaponHue }); break;
                case 2: AddItem(new Maul() { Movable = false, Hue = weaponHue }); break;
                case 3: AddItem(new WarFork() { Movable = false, Hue = weaponHue }); break;
                case 4: AddItem(new Spear() { Movable = false, Hue = weaponHue, Layer = Layer.FirstValid }); break;
            }

            AddItem(new BronzeShield() { Movable = false, Hue = weaponHue });		
		}    

		public SanguinDefender( Serial serial ) : base( serial ) 
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