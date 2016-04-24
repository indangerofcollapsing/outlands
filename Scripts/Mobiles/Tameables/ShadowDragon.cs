using System;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a shadow dragon corpse" )]
	public class ShadowDragon : BaseCreature
	{
		[Constructable]
		public ShadowDragon() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a shadow dragon";
			Body = 106;
			BaseSoundID = 362;

            SetStr(100);
            SetDex(50);
            SetInt(100);

            SetHits(900);
            SetMana(1000);

            SetDamage(25, 35);

            SetSkill(SkillName.Wrestling, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            SetSkill(SkillName.Magery, 50);
            SetSkill(SkillName.EvalInt, 50);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 50;

            SetSkill(SkillName.Poisoning, 15);

			Fame = 22500;
			Karma = -22500;

            Tameable = true;
            ControlSlots = 4;
            MinTameSkill = 105.1;
		}

        public override int TamedItemId { get { return 9781; } }
        public override int TamedItemHue { get { return 0; } }
        public override int TamedItemXOffset { get { return 5; } }
        public override int TamedItemYOffset { get { return -15; } }

        public override int TamedBaseMaxHits { get { return 400; } }
        public override int TamedBaseMinDamage { get { return 30; } }
        public override int TamedBaseMaxDamage { get { return 32; } }
        public override double TamedBaseWrestling { get { return 95; } }
        public override double TamedBaseEvalInt { get { return 50; } }

        public override int TamedBaseStr { get { return 5; } }
        public override int TamedBaseDex { get { return 50; } }
        public override int TamedBaseInt { get { return 50; } }
        public override int TamedBaseMaxMana { get { return 500; } }
        public override double TamedBaseMagicResist { get { return 75; } }
        public override double TamedBaseMagery { get { return 50; } }
        public override double TamedBasePoisoning { get { return 25; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 100; } }
        public override int TamedBaseVirtualArmor { get { return 75; } } 

        public override void SetUniqueAI()
        {
            DictCombatAction[CombatAction.CombatSpecialAction] = 3;
            DictCombatSpecialAction[CombatSpecialAction.PoisonBreathAttack] = 1;
        }

        public override void SetTamedAI()
        {
            AISubGroup = AISubGroupType.MeleeMage1;
            UpdateAI(false);

            DictCombatSpell[CombatSpell.SpellDamage1] -= 2;
            DictCombatSpell[CombatSpell.SpellDamage2] -= 2;
            DictCombatSpell[CombatSpell.SpellDamage3] += 2;
            DictCombatSpell[CombatSpell.SpellDamage4] += 2;

            DictCombatAction[CombatAction.CombatSpecialAction] = 3;
            DictCombatSpecialAction[CombatSpecialAction.PoisonBreathAttack] = 1;
        }

        public override SlayerGroupType SlayerGroup { get { return SlayerGroupType.Beastial; } }
        public override SpeedGroupType BaseSpeedGroup { get { return SpeedGroupType.Medium; } }
        public override AIGroupType AIBaseGroup { get { return AIGroupType.EvilMonster; } }
        public override AISubGroupType AIBaseSubGroup { get { return AISubGroupType.MeleeMage2; } }
        public override double BaseUniqueDifficultyScalar { get { return 1.0; } }

        public override Poison HitPoison { get { return Poison.Deadly; } }
        public override int PoisonResistance { get { return 3; } }        

        public override bool CanFly { get { return true; } }

        public override void OnThink()
        {
            base.OnThink();
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }       
		
		public override int GetIdleSound(){return 0x2D5;}
		public override int GetHurtSound(){return 0x2D1;}

		public ShadowDragon( Serial serial ) : base( serial )
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
