using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a sea serpent corpse" )]
	public class SeaSerpent : BaseCreature
	{
		[Constructable]
		public SeaSerpent() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a sea serpent";
			Body = 150;
			BaseSoundID = 447;

			Hue = Utility.Random( 0x530, 9 );

            SetStr(75);
            SetDex(50);
            SetInt(100);

            SetHits(200);

            SetDamage(10, 20);

            SetSkill(SkillName.Wrestling, 70);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 25;

			Fame = 6000;
			Karma = -6000;

			CanSwim = true;
			CantWalk = true;			
		}

        public override int DoubloonValue { get { return 6; } }
        public override bool IsOceanCreature { get { return true; } }     

        public override void SetUniqueAI()
        {
            DictCombatAction[CombatAction.CombatSpecialAction] = 3;
            DictCombatSpecialAction[CombatSpecialAction.FireBreathAttack] = 1;
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }

		public SeaSerpent( Serial serial ) : base( serial )
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
