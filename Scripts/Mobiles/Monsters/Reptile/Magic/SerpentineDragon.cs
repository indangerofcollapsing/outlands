using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a serpentine drago corpse" )]
	public class SerpentineDragon : BaseCreature
	{
		[Constructable]
		public SerpentineDragon() : base( AIType.AI_Mage, FightMode.Evil, 10, 1, 0.2, 0.4 )
		{
			Name = "a serpentine dragon";
			Body = 103;
			BaseSoundID = 362;

            SetStr(100);
            SetDex(50);
            SetInt(100);

            SetHits(750);
            SetMana(2000);

            SetDamage(25, 35);

            SetSkill(SkillName.Wrestling, 100);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 25;

			Fame = 15000;
			Karma = 15000;			
		}
                
        public override int Scales { get { return 6; } }
        public override ScaleType ScaleType { get { return (Utility.RandomBool() ? ScaleType.Black : ScaleType.White); } }

        public override void SetUniqueAI()
        {
            DictCombatAction[CombatAction.CombatSpecialAction] = 3;
            DictCombatSpecialAction[CombatSpecialAction.FireBreathAttack] = 1;
        }
		
		public override int GetIdleSound()
		{
			return 0x2C4;
		}

		public override int GetAttackSound()
		{
			return 0x2C0;
		}

		public override int GetDeathSound()
		{
			return 0x2C1;
		}

		public override int GetAngerSound()
		{
			return 0x2C4;
		}

		public override int GetHurtSound()
		{
			return 0x2C3;
		}

		public SerpentineDragon( Serial serial ) : base( serial )
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
