using System;
using Server;
using Server.Misc;
using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
	[CorpseName( "a juggernaut corpse" )]
	public class Juggernaut : BaseCreature
	{		
		[Constructable]
		public Juggernaut() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.3, 0.6 )
		{
			Name = "a blackthorn juggernaut";
			Body = 768;

            SetStr(100);
            SetDex(50);
            SetInt(25);

            SetHits(1500);

            SetDamage(25, 35);

            SetSkill(SkillName.Wrestling, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 200);

            VirtualArmor = 75;           

			Fame = 12000;
			Karma = -12000;
		}

        public override void SetUniqueAI()
        {
            UniqueCreatureDifficultyScalar = 1.1;
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < .2)
            {
                SpecialAbilities.EntangleSpecialAbility(1.0, this, defender, 1.0, 3, -1, false, "", "The creature's strike stuns you!");
                SpecialAbilities.CrippleSpecialAbility(1.0, this, defender, .2, 10, -1, false, "", "");
                SpecialAbilities.StunSpecialAbility(1.0, this, defender, .10, 10, -1, true, "", "");
            }
        }

        public override int PoisonResistance { get { return 5; } }
        public override bool AlwaysMurderer { get { return true; } }
       		
		public override int GetDeathSound(){return 0x423;}
		public override int GetAttackSound(){return 0x23B;}
		public override int GetHurtSound(){return 0x140;}		
		
		public Juggernaut( Serial serial ) : base( serial )
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
