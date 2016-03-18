using System;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a swamp tentacle corpse" )]
	public class SwampTentacle : BaseCreature
	{
		[Constructable]
		public SwampTentacle() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a swamp tentacle";
			Body = 66;
			BaseSoundID = 352;

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(250);

            SetDamage(8, 16);

            SetSkill(SkillName.Wrestling, 60);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

			Fame = 3000;
			Karma = -3000;

			PackReg( 7 );
            PackItem( new Engines.Plants.Seed() );
		}

        public override Poison PoisonImmune { get { return Poison.Lethal; } }

		public SwampTentacle( Serial serial ) : base( serial )
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
