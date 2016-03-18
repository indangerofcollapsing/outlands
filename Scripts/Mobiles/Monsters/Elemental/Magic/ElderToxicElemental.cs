using System;
using Server;
using Server.Items;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
	[CorpseName( "an elder toxic elemental corpse" )]
	public class ElderToxicElemental : BaseCreature
	{
        public DateTime m_NextAIChangeAllowed;
        public TimeSpan NextAIChangeDelay = TimeSpan.FromSeconds(30);

		[Constructable]
		public ElderToxicElemental () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "an elder toxic elemental";
			Body = 13;
			Hue = 2004;
			BaseSoundID = 263;

            SetStr(100);
            SetDex(75);
            SetInt(100);

            SetHits(1250);
            SetMana(2000);

            SetDamage(15, 25);

            SetSkill(SkillName.Wrestling, 100);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.Poisoning, 30);

            VirtualArmor = 25;

			ControlSlots = 2;
		}

        public override void SetUniqueAI()
        {
            DictCombatAction[CombatAction.CombatSpecialAction] = 3;
            DictCombatSpecialAction[CombatSpecialAction.PoisonBreathAttack] = 1;
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

            switch (Utility.Random(500))
            {
                case 0: { c.AddItem(SpellScroll.MakeMaster(new SummonAirElementalScroll())); } break;
            }
        }

		public ElderToxicElemental( Serial serial ) : base( serial )
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