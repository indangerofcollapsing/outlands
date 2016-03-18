using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a ravager corpse" )]
	public class Ravager : BaseCreature
	{
		[Constructable]
		public Ravager() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a ravager";
			Body = 314;
			BaseSoundID = 357;

            SetStr(100);
            SetDex(75);
            SetInt(25);

            SetHits(1500);

            SetDamage(25, 40);

            SetSkill(SkillName.Wrestling, 80);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 25;

			Fame = 3500;
			Karma = -3500;
		}

		public Ravager( Serial serial ) : base( serial )
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