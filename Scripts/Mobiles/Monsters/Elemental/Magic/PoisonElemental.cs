using System;
using Server;
using Server.Items;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Achievements;

namespace Server.Mobiles
{
	[CorpseName( "a poison elementals corpse" )]
	public class PoisonElemental : BaseCreature
	{
		[Constructable]
		public PoisonElemental () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a poison elemental";
			Body = 162;
			BaseSoundID = 263;
            Hue = 1268;

			SetStr(100);
            SetDex(75);
            SetInt(100);

            SetHits(1000);
            SetMana(2000);

            SetDamage(15, 25);

            SetSkill(SkillName.Wrestling, 85);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            SetSkill(SkillName.Magery, 75);
            SetSkill(SkillName.EvalInt, 75);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.Poisoning, 50);

            VirtualArmor = 25;

			Fame = 12500;
			Karma = -12500;

			PackItem( new Nightshade( 15 ) );
			PackItem( new LesserPoisonPotion() );
		}

        public override void SetUniqueAI()
        {
            if (Global_AllowAbilities)
                UniqueCreatureDifficultyScalar = 0.9;
        }

        public override Poison HitPoison { get { return Poison.Lethal; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }

		public override void OnDeath( Container c )
		{			
			base.OnDeath( c );

			// IPY ACHIEVEMENT TRIGGER 
			AwardAchievementForKiller(AchievementTriggers.Trigger_PoisonElementalKilled);
			AwardDailyAchievementForKiller(PvECategory.KillPoisonElementals);
			// END IPY ACHIEVEMENT TRIGGER

    		switch( Utility.Random( 500 ) )
    		{
         		case 0: { c.AddItem( SpellScroll.MakeMaster( new EnergyVortexScroll( ) ) ); } break;
    		}
		}

		public PoisonElemental( Serial serial ) : base( serial )
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
