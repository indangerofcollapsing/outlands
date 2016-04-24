using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
	[CorpseName( "a hook horror corpse" )]
	public class HookHorror : BaseCreature
	{        
        [Constructable]
		public HookHorror() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
            Name = "a hook horror";

            Body = 306;
            BaseSoundID = 417;			

			SetStr(75);
			SetDex(100);
			SetInt(25);

			SetHits(700);

			SetDamage(10, 20);

            AttackSpeed = 40;

            SetSkill(SkillName.Wrestling, 85);
			SetSkill(SkillName.Tactics, 100);			

            SetSkill(SkillName.MagicResist, 50);

			Fame = 1500;
			Karma = -1500;

			VirtualArmor = 25;
		}

        public override void SetUniqueAI()
        {           
            UniqueCreatureDifficultyScalar = 1.33;

            DictCombatFlee[CombatFlee.Flee50] = 0;
            DictCombatFlee[CombatFlee.Flee25] = 0;
            DictCombatFlee[CombatFlee.Flee10] = 0;
            DictCombatFlee[CombatFlee.Flee5] = 0;           
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            SpecialAbilities.BleedSpecialAbility(.20, this, defender, DamageMax, 8.0, -1, true, "", "Their attack causes you to bleed!", "-1");
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            base.OnDamage(amount, from, willKill);
        }

        public override bool OnBeforeDeath()
        {
            return base.OnBeforeDeath();
        }

        public override int GetAttackSound(){return 0x5D7;}
        public override int GetHurtSound(){return 0x5D5;}
        public override int GetAngerSound(){return 0x584;}
        public override int GetIdleSound(){return 0x599;}
        public override int GetDeathSound(){return 0x633;}
        
        public HookHorror(Serial serial): base(serial)
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
