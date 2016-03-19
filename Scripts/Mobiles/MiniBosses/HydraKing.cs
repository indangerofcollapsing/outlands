using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Misc;
using Server.Spells;
using Server.Spells.Seventh;
using Server.Spells.Sixth;
using Server.Spells.Third;
using Server.Achievements;
namespace Server.Mobiles
{
	[CorpseName( "the hydra king's corpse" )]
	public class HydraKing : BaseCreature
	{
        public DateTime m_NextAIChangeAllowed;
        public TimeSpan NextAIChangeDelay = TimeSpan.FromSeconds(30);

        public override Loot.LootTier LootTier { get { return Loot.LootTier.Eight; } }

		[Constructable]
		public HydraKing() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
            Name = "The Hydra King";
			Body = 265;
            BaseSoundID = 0x488;

			SetStr( 100 );
			SetDex( 75 );
			SetInt( 25 );

			SetHits( 5000 );
            SetStam( 5000 );
            SetMana( 5000 );

			SetDamage(30, 40);

            SetSkill(SkillName.Wrestling, 90);
            SetSkill(SkillName.Tactics, 100);

			SetSkill(SkillName.MagicResist, 100 );

            SetSkill(SkillName.Magery, 125);
            SetSkill(SkillName.EvalInt, 125);
            SetSkill(SkillName.Meditation, 200);

			Fame = 10000;
			Karma = -10000;

			VirtualArmor = 25;
		}

        public override void SetUniqueAI()
        {
            ResolveAcquireTargetDelay = 0.5;
            RangePerception = 24;

            UniqueCreatureDifficultyScalar = 1.33;
        }

        public override void OnThink()
        {
            base.OnThink();
            
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

                    case 3:
                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 10;
                        DictCombatTargetingWeight[CombatTargetingWeight.HighestHitPoints] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                        break;

                    case 4:
                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.HighestHitPoints] = 10;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                        break;

                    case 5:
                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.HighestHitPoints] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 10;
                        break;
                }

                m_NextAIChangeAllowed = DateTime.UtcNow + NextAIChangeDelay;
            }            
        }

        public override bool AlwaysMurderer { get { return true; } }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomMinMax(1, 50) == 1)
                c.AddItem(new HydraSpawnling());
        }

        public HydraKing(Serial serial): base(serial)
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
