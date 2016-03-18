using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a cyclopean corpse" )]
	public class Cyclops : BaseCreature
	{
		[Constructable]
		public Cyclops() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a cyclopean warrior";
			Body = 75;
			BaseSoundID = 604;

            SetStr(100);
            SetDex(50);
            SetInt(25);

            SetHits(400);

            SetDamage(20, 30);

            SetSkill(SkillName.Wrestling, 75);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            VirtualArmor = 75;

			Fame = 4500;
			Karma = -4500;
		}
        		
		public override int Meat{ get{ return 4; } }

		public Cyclops( Serial serial ) : base( serial )
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
