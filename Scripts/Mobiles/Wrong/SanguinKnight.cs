using System; 
using System.Collections; 
using Server.Items; 
using Server.ContextMenus; 
using Server.Misc; 
using Server.Network; 

namespace Server.Mobiles 
{ 	
	[CorpseName( "a sanguine knight corpse" )] 
	public class SanguinKnight : BaseSanguin 
	{ 
		[Constructable] 
		public SanguinKnight() : base() 
		{
            Body = 0x190;
            
            Name = "a sanguine knight"; 

			SetStr( 100 );
			SetDex( 50 );
			SetInt( 25 );

			SetHits( 800 );

            SetDamage(25, 35);

            SetSkill(SkillName.Swords, 100);
            SetSkill(SkillName.Macing, 100);
            SetSkill(SkillName.Fencing, 100);

            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Parry, 30);

            SetSkill(SkillName.MagicResist, 100);

            VirtualArmor = 75;

			Fame = 3500;
			Karma = -3500;

            Utility.AssignRandomHair(this, hairHue);

            AddItem(new PlateHelm() { Movable = false, Hue = itemHue });
            AddItem(new PlateGorget() { Movable = false, Hue = itemHue });
            AddItem(new PlateArms() { Movable = false, Hue = itemHue });
            AddItem(new PlateGloves() { Movable = false, Hue = itemHue });
            AddItem(new PlateChest() { Movable = false, Hue = itemHue });
            AddItem(new BodySash() { Movable = false, Hue = weaponHue });
            AddItem(new Kilt() { Movable = false, Hue = weaponHue });
            AddItem(new PlateLegs() { Movable = false, Hue = itemHue });

            switch (Utility.RandomMinMax(1, 6))
            {
                case 1:
                    AddItem(new VikingSword() { Movable = false, Hue = weaponHue});
                    AddItem(new MetalKiteShield() { Movable = false, Hue = weaponHue });
                break;

                case 2:
                    AddItem(new WarMace() { Movable = false, Hue = weaponHue});
                    AddItem(new MetalKiteShield() { Movable = false, Hue = weaponHue });
                break;

                case 3:
                    AddItem(new LargeBattleAxe() { Movable = false, Hue = weaponHue});                    
                break;

                case 4:
                    AddItem(new Bardiche() { Movable = false, Hue = weaponHue});                    
                break;

                case 5:
                    AddItem(new Halberd() { Movable = false, Hue = weaponHue});                    
                break;

                case 6:
                    AddItem(new WarHammer() { Movable = false, Hue = weaponHue });
                break;
            }		
		}        

		public SanguinKnight( Serial serial ) : base( serial ) 
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