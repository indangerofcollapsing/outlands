using System;
using Server.Mobiles;
using Server.Achievements;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a jwilson corpse" )]
	public class Jwilson : BaseCreature
	{
		[Constructable]
		public Jwilson() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Hue = Utility.RandomList(0x89C,0x8A2,0x8A8,0x8AE);
			Body = 0x33;
			Name = ("a jwilson");

            SetStr(25);
            SetDex(25);
            SetInt(25);

            SetHits(50);

            SetDamage(4, 8);

            SetSkill(SkillName.Wrestling, 20);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 150);

            SetSkill(SkillName.Poisoning, 10);

            VirtualArmor = 25;

            Fame = 300;
            Karma = -300;
		}

        public override Poison PoisonImmune { get { return Poison.Regular; } }
        public override Poison HitPoison { get { return Poison.Regular; } }

		public Jwilson(Serial serial) : base(serial)
		{
		}

		public override int GetAngerSound() 
		{ 
			return 0x1C8; 
		} 

		public override int GetIdleSound() 
		{ 
			return 0x1C9; 
		} 

		public override int GetAttackSound() 
		{ 
			return 0x1CA; 
		} 

		public override int GetHurtSound() 
		{ 
			return 0x1CB; 
		} 

		public override int GetDeathSound() 
		{ 
			return 0x1CC; 
		}

		public override void OnDeath(Container c)
		{
			base.OnDeath(c);
			// IPY ACHIEVEMENT TRIGGER 
			AwardAchievementForKiller(AchievementTriggers.Trigger_JWilsonKilled);
			// END IPY ACHIEVEMENT TRIGGER
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
