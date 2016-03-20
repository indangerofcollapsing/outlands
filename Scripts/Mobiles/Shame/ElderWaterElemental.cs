using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Achievements;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an elder water elemental corpse")]
    public class ElderWaterElemental: BaseCreature
	{
		[Constructable]
		public ElderWaterElemental () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "an elder water elemental";
			Body = 16;
		    Hue = 2124;
            
			BaseSoundID = 278;

            SetStr(75);
            SetDex(50);
            SetInt(100);

            SetHits(500);
            SetMana(2000);

            SetDamage(15, 25);

            SetSkill(SkillName.Wrestling, 95);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 75);
            SetSkill(SkillName.EvalInt, 75);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.MagicResist, 150);

            VirtualArmor = 25;

			Fame = 4500;
			Karma = -4500;
			
			ControlSlots = 3;
			CanSwim = true;
		}

		public override void OnDeath( Container c )
		{			
    		base.OnDeath( c );
		}	
        
		public ElderWaterElemental( Serial serial ) : base( serial )
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
