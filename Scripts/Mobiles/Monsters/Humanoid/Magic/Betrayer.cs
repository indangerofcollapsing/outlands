using System;
using Server;
using Server.Misc;
using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
	[CorpseName( "a betrayer corpse" )]
	public class Betrayer : BaseCreature
	{		
		[Constructable]
		public Betrayer() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
            SpeechHue = Utility.RandomDyedHue();
            Name = "a betrayer";
			Body = 767;

            SetStr(75);
            SetDex(50);
            SetInt(75);

            SetHits(600);
            SetMana(1000);

            SetDamage(12, 24);

            SetSkill(SkillName.Wrestling, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            SetSkill(SkillName.Magery, 75);
            SetSkill(SkillName.EvalInt, 75);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 25;

			Fame = 15000;
			Karma = -15000;	
		}

        public override void SetUniqueAI()
        {            
            UniqueCreatureDifficultyScalar = 1.05;
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {   
            base.OnGaveMeleeAttack(defender);
                
            SpecialAbilities.StunSpecialAbility(.20, this, defender, .20, 10, -1, true, "", "The creature stuns you with its strike!");
        }
       
        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public override bool AlwaysMurderer { get { return true; } }

		public override int GetDeathSound(){return 0x423;}
		public override int GetAttackSound(){return 0x23B;}
		public override int GetHurtSound(){return 0x140;}	

		public Betrayer( Serial serial ): base( serial )
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
