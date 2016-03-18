using System;
using Server;
using Server.Items;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Spells.Seventh;
using Server.Spells.Sixth;
using Server.Achievements;

namespace Server.Mobiles
{
	[CorpseName( "a balron corpse" )]
	public class Balron : BaseCreature
	{
		[Constructable]
		public Balron () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = NameList.RandomName( "balron" );
			Body = 40;
			BaseSoundID = 357;

            SetStr(100);
            SetDex(70);
            SetInt(100);

            SetHits(2500);
            SetMana(2000);

            SetDamage(25, 45);

            SetSkill(SkillName.Wrestling, 110);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 25;

			Fame = 30000;
			Karma = -30000;
		}

        public override int Meat { get { return 5; } }
        
        public override bool CanRummageCorpses { get { return true; } }

		public override void OnDeath( Container c )
		{
			base.OnDeath(c);

			// IPY ACHIEVEMENT TRIGGER 
			AwardAchievementForKiller(AchievementTriggers.Trigger_BalronKilled);
			AwardDailyAchievementForKiller(PvECategory.KillBalrons);
			// END IPY ACHIEVEMENT TRIGGER

    		switch( Utility.Random( 500 ) )
			{
				case 0: { c.AddItem(SpellScroll.MakeMaster(new SummonDaemonScroll())); } break;
				case 1: { c.AddItem(SpellScroll.MakeMaster(new EarthquakeScroll())); } break;
			}
		}

		public Balron( Serial serial ) : base( serial )
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
