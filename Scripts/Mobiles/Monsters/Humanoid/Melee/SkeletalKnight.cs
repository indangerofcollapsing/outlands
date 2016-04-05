using System;
using System.Collections;
using Server.Items;
using Server.Targeting;


namespace Server.Mobiles
{
	[CorpseName( "a skeletal corpse" )]
	public class SkeletalKnight : BaseCreature
	{
		[Constructable]
		public SkeletalKnight() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a skeletal knight";
			Body = 147;
			BaseSoundID = 451;

            SetStr(100);
            SetDex(50);
            SetInt(25);

            SetHits(250);

            SetDamage(8, 16);

            SetSkill(SkillName.Wrestling, 85);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 25;

			Fame = 3000;
			Karma = -3000;
		}

        public override Poison PoisonImmune { get { return Poison.Lethal; } }

		public override bool OnBeforeDeath()
		{
			return base.OnBeforeDeath();
		}

		public SkeletalKnight( Serial serial ) : base( serial )
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
