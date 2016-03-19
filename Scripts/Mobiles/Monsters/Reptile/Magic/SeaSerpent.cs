using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a sea serpents corpse" )]
	[TypeAlias( "Server.Mobiles.Seaserpant" )]
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

			if ( Utility.RandomBool() )
				PackItem( new SulfurousAsh( 8 ) );
			else
				PackItem( new BlackPearl( 8 ) );

			PackItem( new RawFishSteak(3) );			
		}

        public override int OceanDoubloonValue { get { return 6; } }
        public override bool IsOceanCreature { get { return true; } }        

        public override int Hides { get { return 10; } }
        public override HideType HideType { get { return HideType.Horned; } }

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
