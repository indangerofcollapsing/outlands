using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "an ancient red wyrm corpse" )]
	public class AncientRedWyrm : BaseCreature
	{
		[Constructable]
		public AncientRedWyrm () : base( AIType.AI_Mage, FightMode.Weakest, 10, 1, 0.2, 0.4 )
		{
			Name = "an ancient red wyrm";
			Body = 46;
			BaseSoundID = 362;
			Hue = 1779;

            SetStr(100);
            SetDex(50);
            SetInt(100);

            SetHits(10000);
            SetStam(5000);
            SetMana(5000);

            SetDamage(25, 50);

            SetSkill(SkillName.Wrestling, 100);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 125);

            SetSkill(SkillName.Magery, 125);
            SetSkill(SkillName.EvalInt, 125);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 25;

			Fame = 45000;
			Karma = -45000;
		}

        public override bool AlwaysMurderer { get { return true; } }
               
        public override void SetUniqueAI()
        {
            DictCombatAction[CombatAction.CombatSpecialAction] = 3;
            DictCombatSpecialAction[CombatSpecialAction.FireBreathAttack] = 1;
        }

		public override bool OnBeforeDeath()
		{
			return base.OnBeforeDeath();
		}

		public override int GetIdleSound()
		{
			return 0x2D3;
		}

		public override int GetHurtSound()
		{
			return 0x2D1;
		}

		public AncientRedWyrm( Serial serial ) : base( serial )
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