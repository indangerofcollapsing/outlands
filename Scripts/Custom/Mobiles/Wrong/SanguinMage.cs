using System;
using Server;
using Server.Misc;
using Server.Items;

namespace Server.Mobiles 
{ 
	[CorpseName( "a sanguine mage corpse" )] 
	public class SanguinMage : BaseSanguin 
	{ 
		[Constructable] 
		public SanguinMage() : base() 
		{ 
			Name = "a sanguine mage";

            SetStr(50);
            SetDex(75);
            SetInt(100);

            SetHits(600);
            SetMana(2000);

            SetDamage(12, 24);

            SetSkill(SkillName.Macing, 95);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 75);
            SetSkill(SkillName.EvalInt, 75);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.MagicResist, 150);

            VirtualArmor = 25;

			Fame = 2500;
			Karma = -2500;

            Utility.AssignRandomHair(this, hairHue);

            AddItem(new WizardsHat() { Movable = false, Hue = itemHue });
            AddItem(new FancyShirt() { Movable = false, Hue = itemHue });
            AddItem(new LeatherGloves() { Movable = false, Hue = weaponHue });
            AddItem(new BodySash() { Movable = false, Hue = weaponHue });
            AddItem(new LongPants() { Movable = false, Hue = itemHue });
            AddItem(new Sandals() { Movable = false, Hue = itemHue });

            AddItem(new BlackStaff() { Movable = false, Hue = weaponHue });
		}

		public SanguinMage( Serial serial ) : base( serial )
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