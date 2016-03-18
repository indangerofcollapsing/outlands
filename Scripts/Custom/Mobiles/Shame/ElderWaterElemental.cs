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
		public override double DispelDifficulty{ get{ return 117.5; } }
		public override double DispelFocus{ get{ return 45.0; } }

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

			PackItem( new BlackPearl( 3 ) );
		}

		public override void OnDeath( Container c )
		{			
    		base.OnDeath( c );

			// IPY ACHIEVEMENT TRIGGER 
			AwardAchievementForKiller(AchievementTriggers.Trigger_WaterElementalKilled);
			// END IPY ACHIEVEMENT TRIGGER

      switch( Utility.Random( 250 ) )
      {
        case 0: { c.AddItem(SpellScroll.MakeMaster(new SummonWaterElementalScroll())); } break;
      }
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
