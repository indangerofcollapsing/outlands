using System;
using Server;
using Server.Items;
using Server.Factions;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Spells.Seventh;
using Server.Spells.Sixth;
using Server.Achievements;

namespace Server.Mobiles
{
	[CorpseName( "a daemon corpse" )]
	public class Daemon : BaseCreature
	{
		public override double DispelDifficulty{ get{ return 125.0; } }
		public override double DispelFocus{ get{ return 45.0; } }

		public override Faction FactionAllegiance { get { return Shadowlords.Instance; } }
		public override Ethics.Ethic EthicAllegiance { get { return Ethics.Ethic.Evil; } }

		[Constructable]
		public Daemon () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = NameList.RandomName( "daemon" );
			Body = 9;
			BaseSoundID = 357;

            SetStr(100);
            SetDex(75);
            SetInt(100);

            SetHits(1000);
            SetMana(2000);

            SetDamage(20, 40);

            SetSkill(SkillName.Wrestling, 95);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            SetSkill(SkillName.Magery, 75);
            SetSkill(SkillName.EvalInt, 75);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 25;

			Fame = 15000;
			Karma = -15000;

			ControlSlots = 2;
		}

		public override void OnDeath( Container c )
		{			
			base.OnDeath( c );

			// IPY ACHIEVEMENT TRIGGER 
			AwardAchievementForKiller(AchievementTriggers.Trigger_DaemonKilled);
			AwardDailyAchievementForKiller(PvECategory.KillDemons);
			// END IPY ACHIEVEMENT TRIGGER

          switch( Utility.Random( 500 ) )
          {
            case 0: { c.AddItem(SpellScroll.MakeMaster(new SummonDaemonScroll())); } break;
          }
		}
		
		public override bool CanRummageCorpses{ get{ return true; } }		
		public override int Meat{ get{ return 1; } }
		public override bool CanFly { get { return true; } }

		public Daemon( Serial serial ) : base( serial )
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
