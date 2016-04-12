using System;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a nightmare corpse" )]
	public class Nightmare : BaseCreature
	{
		[Constructable]
		public Nightmare () : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
		{
            Name = "Nightmare";            

            switch (Utility.Random(3))
            {
                case 0: { BodyValue = 116; break; }
                case 1: { BodyValue = 178; break; }
                case 2: { BodyValue = 179; break; }
            }

            SetStr(75);
            SetDex(75);
            SetInt(75);

            SetHits(800);
            SetMana(1000);

            SetDamage(25, 35);

            SetSkill(SkillName.Wrestling, 95);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            SetSkill(SkillName.Magery, 50);
            SetSkill(SkillName.EvalInt, 50);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 25;            

			Fame = 14000;
			Karma = -14000;

			Tameable = true;
			ControlSlots = 2;
			MinTameSkill = 95;			
		}

        public override int TamedItemId { get { return 8480; } }
        public override int TamedItemHue { get { return 2019; } }
        public override int TamedItemXOffset { get { return 5; } }
        public override int TamedItemYOffset { get { return 10; } }

        public override int TamedBaseMaxHits { get { return 250; } }
        public override int TamedBaseMinDamage { get { return 16; } }
        public override int TamedBaseMaxDamage { get { return 18; } }
        public override double TamedBaseWrestling { get { return 95; } }
        public override double TamedBaseEvalInt { get { return 50; } }

        public override int TamedBaseStr { get { return 5; } }
        public override int TamedBaseDex { get { return 50; } }
        public override int TamedBaseInt { get { return 50; } }
        public override int TamedBaseMaxMana { get { return 1000; } }
        public override double TamedBaseMagicResist { get { return 100; } }
        public override double TamedBaseMagery { get { return 50; } }
        public override double TamedBasePoisoning { get { return 0; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 100; } }
        public override int TamedBaseVirtualArmor { get { return 50; } }

        public override void SetUniqueAI()
        {
            DictCombatAction[CombatAction.CombatSpecialAction] = 3;
            DictCombatSpecialAction[CombatSpecialAction.FireBreathAttack] = 1;            
        }

        public override void SetTamedAI()
        {
            AISubGroup = AISubGroupType.MeleeMage2;
            UpdateAI(false);
            
            DictCombatSpell[CombatSpell.SpellDamage3] += 1;
            DictCombatSpell[CombatSpell.SpellDamage4] += 1;
            DictCombatSpell[CombatSpell.SpellDamage5] -= 1;
            DictCombatSpell[CombatSpell.SpellDamage6] -= 1;

            DictCombatAction[CombatAction.CombatSpecialAction] = 3;
            DictCombatSpecialAction[CombatSpecialAction.FireBreathAttack] = 1;
        }

        public override SpeedGroupType BaseSpeedGroup { get { return SpeedGroupType.VeryFast; } }
        public override AIGroupType AIBaseGroup { get { return AIGroupType.EvilMonster; } }
        public override AISubGroupType AIBaseSubGroup { get { return AISubGroupType.Melee; } }
        public override double BaseUniqueDifficultyScalar { get { return 1.0; } }

        public override void OnThink()
        {
            base.OnThink();
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }

        public override int GetAngerSound() {return 0x4BE; }
        public override int GetIdleSound(){ return 0x4BD; }
        public override int GetAttackSound() { return 0x4F6;}
        public override int GetHurtSound() {return 0x4BF;}
        public override int GetDeathSound() { return 0x4C0;}

		public Nightmare( Serial serial ) : base( serial )
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
