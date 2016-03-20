using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Targeting;
using Server.Achievements;

namespace Server.Mobiles
{
	[CorpseName( "a dragon whelp corpse" )]
	public class DragonWhelp : BaseCreature
	{
        public override bool CanBeResurrectedThroughVeterinary { get { return false; } }

		[Constructable]
		public DragonWhelp () : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a dragon whelp";
			Body = 718;
            Hue = 1205;
			BaseSoundID = 0x646;

            SetStr(50);
            SetDex(75);
            SetInt(50);

            SetHits(250);
            SetMana(1000);

            SetDamage(9, 18);

            SetSkill(SkillName.Wrestling, 80);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 75);

            SetSkill(SkillName.Magery, 25);
            SetSkill(SkillName.EvalInt, 25);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 25;            

			Fame = 3400;
			Karma = -3400;

            Tameable = true;
            ControlSlots = 1;
            MinTameSkill = 85;
        }
        public override bool CanFly { get { return true; } }

        public override int TamedItemId { get { return 8467; } }
        public override int TamedItemHue { get { return 1205; } }
        public override int TamedItemXOffset { get { return 10; } }
        public override int TamedItemYOffset { get { return 10; } }

        public override int TamedBaseMaxHits { get { return 125; } }
        public override int TamedBaseMinDamage { get { return 7; } }
        public override int TamedBaseMaxDamage { get { return 9; } }
        public override double TamedBaseWrestling { get { return 75; } }
        public override double TamedBaseEvalInt { get { return 25; } }

        public override int TamedBaseStr { get { return 5; } }
        public override int TamedBaseDex { get { return 50; } }
        public override int TamedBaseInt { get { return 50; } }
        public override int TamedBaseMaxMana { get { return 500; } }
        public override double TamedBaseMagicResist { get { return 75; } }
        public override double TamedBaseMagery { get { return 25; } }
        public override double TamedBasePoisoning { get { return 0; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 100; } }
        public override int TamedBaseVirtualArmor { get { return 75; } }

        public override void SetUniqueAI()
        {
            UniqueCreatureDifficultyScalar = .95;

            DictCombatAction[CombatAction.CombatSpecialAction] = 3;
            DictCombatSpecialAction[CombatSpecialAction.FireBreathAttack] = 1;
        }

        public override void SetTamedAI()
        {   
            SetSubGroup(AISubgroup.MeleeMage1);
            UpdateAI(false);

            DictCombatAction[CombatAction.CombatSpecialAction] = 3;
            DictCombatSpecialAction[CombatSpecialAction.FireBreathAttack] = 1;

            SpellDelayMin *= 1.5;
            SpellDelayMax *= 1.5;
        }

        public override bool IsHighSeasBodyType { get { return true; } }

        public override int GetDeathSound(){return 0x2CD; }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }

		public DragonWhelp( Serial serial ) : base( serial )
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
