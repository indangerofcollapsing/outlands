using System; 
using System.Collections; 
using Server.Items; 
using Server.ContextMenus; 
using Server.Misc; 
using Server.Network; 

namespace Server.Mobiles 
{ 	
	[CorpseName( "a sanguin conscript corpse" )] 
	public class SanguinConscript : BaseSanguin 
	{ 
		[Constructable] 
		public SanguinConscript() : base() 
		{ 			
			Name = "a sanguin conscript"; 

			SetStr( 50 );
			SetDex( 50 );
			SetInt( 25 );

			SetHits( 400 );

			SetDamage(11, 22);    

            SetSkill(SkillName.Macing, 80);
            SetSkill(SkillName.Fencing, 80);
            SetSkill(SkillName.Swords, 80);

            SetSkill(SkillName.Tactics, 100);

			SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

			Fame = 1500;
			Karma = -1500;

            Utility.AssignRandomHair(this, hairHue);

            AddItem(new ChainmailChest() { LootType = Server.LootType.Unlootable, Hue = itemHue });
            AddItem(new LeatherGorget() { LootType = Server.LootType.Unlootable, Hue = weaponHue });
            AddItem(new RingmailArms() { LootType = Server.LootType.Unlootable, Hue = itemHue });
            AddItem(new BodySash() { LootType = Server.LootType.Unlootable, Hue = weaponHue });
            AddItem(new LeatherGloves() { LootType = Server.LootType.Unlootable, Hue = itemHue });
            AddItem(new Kilt() { LootType = Server.LootType.Unlootable, Hue = weaponHue });
            AddItem(new Sandals() { LootType = Server.LootType.Unlootable, Hue = itemHue });          
		
			switch ( Utility.RandomMinMax( 1, 4 ))
			{
                case 1: AddItem(new Club() { LootType = Server.LootType.Unlootable, Hue = weaponHue }); break;
                case 2: AddItem(new Axe() { LootType = Server.LootType.Unlootable, Hue = weaponHue }); break;
                case 3: AddItem(new Pitchfork() { LootType = Server.LootType.Unlootable, Hue = weaponHue }); break;
                case 4: AddItem(new GnarledStaff() { LootType = Server.LootType.Unlootable, Hue = weaponHue }); break;
			}			
		}

		public SanguinConscript( Serial serial ) : base( serial ) 
		{ 
		} 

		public override void Serialize( GenericWriter writer ) 
		{ 
			base.Serialize( writer ); 
			writer.Write( (int) 0 ); //version 
		} 

		public override void Deserialize( GenericReader reader ) 
		{
            base.Deserialize(reader); 
			int version = reader.ReadInt(); 
		} 
	} 
}