using System;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a moloch corpse" )]
	public class Moloch : BaseCreature
	{
		[Constructable]
		public Moloch() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a moloch";
			Body = 0x311;
			BaseSoundID = 0x300;

            SetStr(75);
            SetDex(75);
            SetInt(25);

            SetHits(750);

            SetDamage(20, 30);

            SetSkill(SkillName.Wrestling, 95);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 75);

            VirtualArmor = 25;

			Fame = 7500;
			Karma = -7500;
		}

        public override int Meat { get { return 4; } }
		
		public Moloch( Serial serial ) : base( serial )
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
