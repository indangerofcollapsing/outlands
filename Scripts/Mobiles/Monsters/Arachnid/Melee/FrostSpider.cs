using System;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a frost spider corpse" )]
	public class FrostSpider : BaseCreature
	{
		[Constructable]
		public FrostSpider() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a frost spider";
			Body = 20;
			Hue = 2119;
			BaseSoundID = 0x388;

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(150);

            SetDamage(5, 10);

            SetSkill(SkillName.Wrestling, 60);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            SetSkill(SkillName.Poisoning, 25);

            VirtualArmor = 25;

			Fame = 775;
			Karma = -775;
		}

        public override Poison HitPoison { get { return Poison.Regular; } }
        public override Poison PoisonImmune { get { return Poison.Regular; } }   

		public FrostSpider( Serial serial ) : base( serial )
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
