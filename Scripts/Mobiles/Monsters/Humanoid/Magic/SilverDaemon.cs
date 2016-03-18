using System;
using Server;
using Server.Items;
using Server.Factions;
using Server.Achievements;

namespace Server.Mobiles
{
	[CorpseName( "a silver daemon corpse" )]
	public class SilverDaemon : BaseCreature
	{
		public override double DispelDifficulty{ get{ return 125.0; } }
		public override double DispelFocus{ get{ return 45.0; } }

		public override Faction FactionAllegiance { get { return Shadowlords.Instance; } }
		public override Ethics.Ethic EthicAllegiance { get { return Ethics.Ethic.Evil; } }

		[Constructable]
		public SilverDaemon () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a silver daemon" ;
			Body = 9;
			BaseSoundID = 357;
			Hue = 0x835;

            SetStr(100);
            SetDex(75);
            SetInt(100);

            SetHits(2500);
            SetMana(2000);

            SetDamage(20, 40);

            SetSkill(SkillName.Wrestling, 105);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 150);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.Poisoning, 25);

            VirtualArmor = 25;            

			Fame = 50000;
			Karma = -50000;
		}

        public override int Meat { get { return 4; } }

        public override Poison HitPoison { get { return Poison.Deadly; } }
        public override Poison PoisonImmune { get { return Poison.Deadly; } }

        public override bool CanRummageCorpses { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
            
            // IPY ACHIEVEMENT TRIGGER 
            AwardAchievementForKiller(AchievementTriggers.Trigger_DaemonKilled);
            AwardDailyAchievementForKiller(PvECategory.KillDemons);
            // END IPY ACHIEVEMENT TRIGGER

            switch (Utility.Random(100))
            {
                case 1: { c.AddItem(new DaemonBlood()); } break;
                case 2: { c.AddItem(new CandleSkull()); } break;
                case 3: { c.AddItem(new BlankScroll()); } break;
                case 4: { c.AddItem(new CandleLarge()); } break;
            }
        }

		public SilverDaemon( Serial serial ) : base( serial )
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