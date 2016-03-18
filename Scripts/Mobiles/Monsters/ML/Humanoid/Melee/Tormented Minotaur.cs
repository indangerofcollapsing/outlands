using System;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a tormented minotaur corpse" )]
	public class TormentedMinotaur : BaseCreature
	{
		[Constructable]
		public TormentedMinotaur() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Tormented Minotaur";
			Body = 262;

            SetStr(100);
            SetDex(75);
            SetInt(25);

            SetHits(1800);

            SetDamage(30, 45);

            SetSkill(SkillName.Wrestling, 105);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 100;

			Fame = 20000;
			Karma = -20000;
		}

        public override void SetUniqueAI()
        {
            if (Global_AllowAbilities)
                UniqueCreatureDifficultyScalar = 1.05;
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            SpecialAbilities.BleedSpecialAbility(.2, this, defender, DamageMax, 8.0, -1, true, "", "The beast gores you with it's horns, causing you to bleed!");
        }

		public override int Meat { get { return 4; } }
        
		public override int GetDeathSound()	{ return 0x596; }
		public override int GetAttackSound() { return 0x597; }
		public override int GetIdleSound() { return 0x598; }
		public override int GetAngerSound() { return 0x599; }
		public override int GetHurtSound() { return 0x59A; }

		public TormentedMinotaur( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}
