using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a crane corpse" )]
	public class Crane : BaseCreature
	{
		[Constructable]
		public Crane() : base( AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			Name = "a crane";
			Body = 254;
			BaseSoundID = 0x4D7;

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(75);

            SetDamage(3, 6);

            SetSkill(SkillName.Wrestling, 20);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

			Fame = 0;
			Karma = 200;
		}
        
		public override int GetAngerSound()
		{
			return 0x4D9;
		}

		public override int GetIdleSound()
		{
			return 0x4D8;
		}

		public override int GetAttackSound()
		{
			return 0x4D7;
		}

		public override int GetHurtSound()
		{
			return 0x4DA;
		}

		public override int GetDeathSound()
		{
			return 0x4D6;
		}

		public Crane(Serial serial) : base(serial)
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