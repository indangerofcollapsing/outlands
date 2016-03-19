using System; 
using System.Collections; 
using Server.Items; 
using Server.ContextMenus; 
using Server.Misc; 
using Server.Network; 

namespace Server.Mobiles 
{ 	
	[CorpseName( "atraxis the martyr's corpse" )]
    public class Atraxis : BaseCreature 
	{
        public DateTime m_NextAIChangeAllowed;
        public TimeSpan NextAIChangeDelay = TimeSpan.FromSeconds(30);

        public override Loot.LootTier LootTier { get { return Loot.LootTier.Eight; } }

		[Constructable]
        public Atraxis(): base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
		{   
            Name = "Atraxis the Martyr";

            Body = 401;

			SetStr( 100 );
			SetDex( 50 );
			SetInt( 100 );

			SetHits(5000);
            SetStam(3000);
            SetMana(5000);

            SetDamage(30, 50);

            SetSkill(SkillName.Swords, 110);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            SetSkill(SkillName.Magery, 125);
            SetSkill(SkillName.EvalInt, 125);
            SetSkill(SkillName.Meditation, 200);

            VirtualArmor = 25;

			Fame = 3500;
			Karma = -3500;

            AddItem(new HoodedShroudOfShadows() { Movable = false, Name = "Shrowd of Atraxis" });
            AddItem(new Halberd() { Movable = false, Hue = 2599, Name = "Broken Promise" });
            AddItem(new Sandals() { Movable = false, Hue = 1175});
            
            HairItemID = 8252;
            HairHue = 0;
		}

        public override void SetUniqueAI()
        {
            ResolveAcquireTargetDelay = 0.5;
            RangePerception = 24;

            UniqueCreatureDifficultyScalar = 1.1;
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

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomMinMax(1, 50) == 1)
                c.AddItem(new MummifiedCorpse());
        }

        public Atraxis(Serial serial): base(serial) 
		{ 
		}

        public override bool AlwaysMiniBoss { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }

		public override void Serialize( GenericWriter writer ) 
		{ 
			base.Serialize( writer ); 
			writer.Write( (int) 0 ); //version 
		} 

		public override void Deserialize( GenericReader reader ) 
		{ 
			base.Deserialize( reader ); 
			int version = reader.ReadInt(); 
		} 
	} 
}