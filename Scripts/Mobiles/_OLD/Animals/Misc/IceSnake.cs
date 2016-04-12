using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "an ice snake corpse" )]
	[TypeAlias( "Server.Mobiles.Icesnake" )]
	public class IceSnake : BaseCreature
	{
		[Constructable]
		public IceSnake() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "an ice snake";
			Body = 52;
			Hue = 2220;
			BaseSoundID = 0xDB;

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(50);

            SetDamage(3, 6);

            SetSkill(SkillName.Wrestling, 40);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            SetSkill(SkillName.Poisoning, 20);

            VirtualArmor = 25; 

			Fame = 900;
			Karma = -900;
		}

        public override Poison HitPoison { get { return Poison.Regular; } }
        public override int PoisonResistance { get { return 2; } }

		public IceSnake(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}
	}
}
