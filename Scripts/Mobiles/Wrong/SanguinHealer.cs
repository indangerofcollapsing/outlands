using System;
using Server;
using Server.Misc;
using Server.Items;

namespace Server.Mobiles 
{ 
	[CorpseName( "a sanguine healer corpse" )] 
	public class SanguinHealer : BaseSanguin 
	{ 
		[Constructable] 
		public SanguinHealer() : base() 
		{ 
			Name = "a sanguine healer";
			
			SetStr(50);
			SetDex(25);
			SetInt(100);

            SetHits(600);
            SetMana(1000);

            SetDamage(8, 16);

            SetSkill(SkillName.Wrestling, 100);    
            SetSkill(SkillName.Tactics, 100);                       

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.MagicResist, 125); 

            VirtualArmor = 25;

            Fame = 2500;
            Karma = -2500;

            Utility.AssignRandomHair(this, hairHue);

            AddItem(new WizardsHat() { Movable = false, Hue = itemHue });
            AddItem(new Shirt() { Movable = false, Hue = itemHue });
            AddItem(new LeatherArms() { Movable = false, Hue = weaponHue });
            AddItem(new LeatherGloves() { Movable = false, Hue = itemHue });
            AddItem(new Skirt() { Movable = false, Hue = itemHue });
            AddItem(new Sandals() { Movable = false, Hue = itemHue });

            AddItem(new Spellbook() { Movable = false, Hue = weaponHue });
		}

		public SanguinHealer( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}