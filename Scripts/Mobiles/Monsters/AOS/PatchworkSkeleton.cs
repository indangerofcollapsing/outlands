using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a patchwork skeletal corpse" )]
	public class PatchworkSkeleton : BaseCreature
	{
		[Constructable]
		public PatchworkSkeleton() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a patchwork skeleton";
			Body = 309;
			BaseSoundID = 0x48D;

            SetStr(100);
            SetDex(50);
            SetInt(25);

            SetHits(150);

            SetDamage(7, 14);

            SetSkill(SkillName.Wrestling, 60);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 25;

			Fame = 500;
			Karma = -500;
		}

		public override int PoisonResistance{ get{ return 5; } }

		public PatchworkSkeleton( Serial serial ) : base( serial )
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