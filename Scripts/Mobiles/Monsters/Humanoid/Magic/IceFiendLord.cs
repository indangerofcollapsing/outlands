using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "an ice fiend lord's corpse" )]
	public class IceFiendLord : BaseCreature
	{
		[Constructable]
		public IceFiendLord () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "an ice fiend lord";
			Body = 43;
			BaseSoundID = 357;
            Hue = 2587;

            SetStr(100);
            SetDex(75);
            SetInt(100);

            SetHits(2500);
            SetMana(2000);

            SetDamage(25, 45);

            SetSkill(SkillName.Wrestling, 105);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 25;
            
			Fame = 18000;
			Karma = -18000;
		}

        public override void SetUniqueAI()
        {
            UniqueCreatureDifficultyScalar = 0.95;

            DictCombatAction[CombatAction.CombatSpecialAction] = 3;
            DictCombatSpecialAction[CombatSpecialAction.IceBreathAttack] = 1;
        }
		
		public override bool CanFly { get { return true; } }

        public IceFiendLord(Serial serial): base(serial)
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
