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
	[CorpseName( "shadow of peradun's corpse" )]
	public class ShadowOfPeradun : BaseCreature
	{
        public DateTime m_NextAIChangeAllowed;
        public TimeSpan NextAIChangeDelay = TimeSpan.FromSeconds(30);

        public override Loot.LootTier LootTier { get { return Loot.LootTier.Eight; } }

		[Constructable]
		public ShadowOfPeradun() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
            Name = "Shadow of Peradun";

			Body = 1071;
            Hue = 25000;
            BaseSoundID = 357;

			SetStr( 100 );
			SetDex( 75 );
			SetInt( 100 );

			SetHits( 5000 );
            SetStam( 3000 );
            SetMana( 5000) ;

			SetDamage( 25, 45 );

            SetSkill(SkillName.Wrestling, 100);
            SetSkill(SkillName.Tactics, 100);

			SetSkill( SkillName.MagicResist, 115 );

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

            UniqueCreatureDifficultyScalar = 1.19;
        }

        public override bool AlwaysMiniBoss { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }

        public override bool IsHighSeasBodyType { get { return true; } }

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
        }
        
        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomMinMax(1, 50) == 1)
                c.AddItem(new PeradunsSkull());
        }

        public ShadowOfPeradun(Serial serial): base(serial)
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
