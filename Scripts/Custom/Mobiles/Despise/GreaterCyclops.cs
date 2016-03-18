using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a greater cyclops corpse" )]
	public class GreaterCyclops : BaseCreature
	{
		[Constructable]
		public GreaterCyclops() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a greater cyclops";
			Body = 75;
			BaseSoundID = 604;
            Hue = 2107;

            SetStr(100);
            SetDex(50);
            SetInt(25);

            SetHits(1000);

            SetDamage(30, 45);

            SetSkill(SkillName.Wrestling, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            VirtualArmor = 25;

			Fame = 4500;
			Karma = -4500;
		}
        		
		public override int Meat{ get{ return 4; } }

		public GreaterCyclops( Serial serial ) : base( serial )
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
