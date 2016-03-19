using System;
using Server;
using Server.Misc;
using Server.Items;

namespace Server.Mobiles 
{ 
	[CorpseName( "a sanguine wizard corpse" )] 
	public class SanguinWizard : BaseSanguin 
	{ 
		[Constructable] 
		public SanguinWizard() : base() 
		{ 
			Name = "a sanguine wizard";

            SetStr(50);
            SetDex(75);
            SetInt(100);

            SetHits(800);
            SetMana(2000);

            SetDamage(15, 25);

            SetSkill(SkillName.Macing, 100);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.MagicResist, 200);

            VirtualArmor = 25;

			Fame = 2500;
			Karma = -2500;

            Utility.AssignRandomHair(this, hairHue);

            AddItem(new WizardsHat() { Movable = false, Hue = itemHue });
            AddItem(new Robe() { Movable = false, Hue = itemHue });
            AddItem(new LeatherGloves() { Movable = false, Hue = weaponHue });
            AddItem(new Sandals() { Movable = false, Hue = itemHue });

            AddItem(new BlackStaff() { Movable = false, Hue = weaponHue });
		}

		public SanguinWizard( Serial serial ) : base( serial )
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