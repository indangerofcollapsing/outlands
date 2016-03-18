using System;
using Server;
using Server.Items;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Achievements;

namespace Server.Mobiles
{
	[CorpseName( "an elder poison elemental corpse" )]
	public class ElderPoisonElemental : BaseCreature
	{
        public DateTime m_NextAIChangeAllowed;
        public TimeSpan NextAIChangeDelay = TimeSpan.FromSeconds(30);

		[Constructable]
		public ElderPoisonElemental () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "an elder poison elemental";
			Body = 162;
			BaseSoundID = 263;
            Hue = 1268;

			SetStr(75);
            SetDex(75);
            SetInt(100);

            SetHits(1250);
            SetMana(2000);

            SetDamage(20, 35);

            SetSkill(SkillName.Wrestling, 95);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.Poisoning, 75);

            VirtualArmor = 25;

			Fame = 12500;
			Karma = -12500;

			PackItem( new Nightshade( 30 ) );
			PackItem( new LesserPoisonPotion() );
		}

        public override void SetUniqueAI()
        {
            if (Global_AllowAbilities)
                UniqueCreatureDifficultyScalar = 0.9;
        }

        public override Poison HitPoison { get { return Poison.Lethal; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public override void OnThink()
        {
            base.OnThink();

            if (Global_AllowAbilities)
            {
                if (Utility.RandomDouble() < .01 && DateTime.UtcNow > m_NextAIChangeAllowed)
                {
                    Effects.PlaySound(Location, Map, GetAngerSound());

                    switch (Utility.RandomMinMax(1, 5))
                    {
                        case 1:
                            DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                            DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 0;
                            DictCombatTargetingWeight[CombatTargetingWeight.HighestHitPoints] = 0;
                            DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                            break;

                        case 2:
                            DictCombatTargetingWeight[CombatTargetingWeight.Player] = 10;
                            DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 0;
                            DictCombatTargetingWeight[CombatTargetingWeight.HighestHitPoints] = 0;
                            DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                            break;

                    }

                    m_NextAIChangeAllowed = DateTime.UtcNow + NextAIChangeDelay;
                }
            }
        }

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

		public ElderPoisonElemental( Serial serial ) : base( serial )
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
