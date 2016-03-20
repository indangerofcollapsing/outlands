using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a fleshrenderer corpse" )]
	public class FleshRenderer : BaseCreature
	{
		[Constructable]
		public FleshRenderer() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a fleshrenderer";
			Body = 315;

            SetStr(100);
            SetDex(50);
            SetInt(25);

            SetHits(1500);

            SetDamage(20, 30);

            SetSkill(SkillName.Wrestling, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 75;

			Fame = 23000;
			Karma = -23000;
		}

        public override void SetUniqueAI()
        {
            UniqueCreatureDifficultyScalar = 1.05;
        }
      
        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            SpecialAbilities.PierceSpecialAbility(.20, this, defender, .5, 20, -1, true, "", "Their crushing bite momentarily weakens your armor!");
        }

		public override void OnDeath( Container c )
		{
			base.OnDeath( c );			
		}		

		public override int GetAttackSound()
		{
			return 0x34C;
		}

		public override int GetHurtSound()
		{
			return 0x354;
		}

		public override int GetAngerSound()
		{
			return 0x34C;
		}

		public override int GetIdleSound()
		{
			return 0x34C;
		}

		public override int GetDeathSound()
		{
			return 0x354;
		}

		public FleshRenderer( Serial serial ) : base( serial )
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