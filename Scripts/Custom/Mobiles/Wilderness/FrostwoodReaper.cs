using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a frostwood reaper's corpse" )]
	public class FrostwoodReaper : BaseCreature
	{
		[Constructable]
		public FrostwoodReaper() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a frostwood reaper";
			Body = 47;
            Hue = 0x47F;
			BaseSoundID = 442;

            SetStr(75);
            SetDex(25);
            SetInt(25);

            SetHits(300);
            SetMana(2000);

            SetDamage(8, 16);

            SetSkill(SkillName.Wrestling, 50);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 100);           

            SpellHue = 2603;
            SpellSpeedScalar = .2;            

			Fame = 3500;
			Karma = -3500;

			PackItem( new Log( 10 ) );
			PackItem( new MandrakeRoot( 5 ) );
            PackItem( new Engines.Plants.Seed() );      
		}

        public override void SetUniqueAI()
        {
            DictCombatRange[CombatRange.WeaponAttackRange] = 0;
            DictCombatRange[CombatRange.SpellRange] = 50;
            DictCombatRange[CombatRange.Withdraw] = 0;

            DictCombatAction[CombatAction.AttackOnly] = 0;
            DictCombatAction[CombatAction.CombatSpell] = 50;
            DictCombatAction[CombatAction.CombatHealSelf] = 0;

            DictCombatSpell[CombatSpell.SpellDamage1] = 0;
            DictCombatSpell[CombatSpell.SpellDamage2] = 0;
            DictCombatSpell[CombatSpell.SpellDamage3] = 0;
            DictCombatSpell[CombatSpell.SpellDamage4] = 0;
            DictCombatSpell[CombatSpell.SpellDamage5] = 50;
            DictCombatSpell[CombatSpell.SpellDamage6] = 0;
            DictCombatSpell[CombatSpell.SpellDamage7] = 0;
            DictCombatSpell[CombatSpell.SpellDamageAOE7] = 0;
            DictCombatSpell[CombatSpell.SpellPoison] = 0;
            DictCombatSpell[CombatSpell.SpellNegative1to3] = 0;
            DictCombatSpell[CombatSpell.SpellNegative4to7] = 0;
            DictCombatSpell[CombatSpell.SpellSummon5] = 0;
            DictCombatSpell[CombatSpell.SpellSummon8] = 0;
            DictCombatSpell[CombatSpell.SpellDispelSummon] = 0;
            DictCombatSpell[CombatSpell.SpellHarmfulField] = 0;
            DictCombatSpell[CombatSpell.SpellNegativeField] = 0;
            DictCombatSpell[CombatSpell.SpellBeneficial1to2] = 0;
            DictCombatSpell[CombatSpell.SpellBeneficial3to5] = 0;

            DictCombatHealSelf[CombatHealSelf.SpellHealSelf100] = 0;
            DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 0;
            DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 0;
            DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 0;
            DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 0;

            DictWanderAction[WanderAction.None] = 0;
            DictWanderAction[WanderAction.SpellHealSelf100] = 0;
            DictWanderAction[WanderAction.SpellCureSelf] = 0;

            SpellDelayMin = 0;
            SpellDelayMax = 0;

            ResolveAcquireTargetDelay = .25;
            RangePerception = 18;

            AcquireNewTargetEveryCombatAction = true;
            AcquireRandomizedTarget = true;
            AcquireRandomizedTargetSearchRange = 8;

            m_NextAcquireTargetDelay = TimeSpan.FromSeconds(3);           
        }

        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public override bool DisallowAllMoves { get { return true; } }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            switch (Utility.Random(500))
            {
                case 0: { c.AddItem(new RangerArms()); } break;
                case 1: { c.AddItem(new RangerChest()); } break;
                case 2: { c.AddItem(new RangerGloves()); } break;
                case 3: { c.AddItem(new RangerGorget()); } break;
                case 4: { c.AddItem(new RangerLegs()); } break;
            }
        }

		public override bool OnBeforeDeath()
		{
			PackItem( new Log( 10 ) );			

			return base.OnBeforeDeath();
		}
		
		public FrostwoodReaper( Serial serial ) : base( serial )
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
