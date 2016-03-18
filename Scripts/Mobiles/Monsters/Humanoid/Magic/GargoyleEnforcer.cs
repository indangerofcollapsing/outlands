using System;
using Server.Items;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Spells.Seventh;
using Server.Spells.Sixth;
using Server.Achievements;

namespace Server.Mobiles
{
	[CorpseName( "a gargoyle corpse" )]
	public class GargoyleEnforcer : BaseCreature
	{
		[Constructable]
		public GargoyleEnforcer() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Gargoyle Enforcer";
			Body = 0x2F2;
			BaseSoundID = 0x174;

            SetStr(75);
            SetDex(50);
            SetInt(100);

            SetHits(600);
            SetMana(2000);

            SetDamage(20, 30);

            SetSkill(SkillName.Wrestling, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 75);

            SetSkill(SkillName.Magery, 75);
            SetSkill(SkillName.EvalInt, 75);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.DetectHidden, 75);

            VirtualArmor = 75;

			Fame = 5000;
			Karma = -5000;
		}

		public override bool CanFly { get { return true; } }
		public override int Meat{ get{ return 2; } }

        public override void OnDeath( Container c )
        {     
            base.OnDeath( c );

            // IPY ACHIEVEMENT TRIGGER 
            AwardAchievementForKiller(AchievementTriggers.Trigger_GargoyleKilled);
            AwardAchievementForKiller(AchievementTriggers.Trigger_GargoyleKilled_TIMED);
            AwardDailyAchievementForKiller(PvECategory.KillGargoyles);
            // END IPY ACHIEVEMENT TRIGGER

            switch( Utility.Random( 500 ) )
            {
                case 0: { c.AddItem(SpellScroll.MakeMaster(new EnergyFieldScroll())); } break;
            }
        }

		public GargoyleEnforcer( Serial serial ) : base( serial )
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
