using System;
using System.Collections;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName("a fetid essence corpse")]
	public class  FetidEssence  : BaseCreature
	{
		[Constructable]
		public  FetidEssence () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a fetid essence";
			Body = 273;

            SetStr(75);
            SetDex(25);
            SetInt(25);

            SetHits(1000);

            SetDamage(15, 25);

            SetSkill(SkillName.Wrestling, 80);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            SetSkill(SkillName.Poisoning, 25);

            VirtualArmor = 25;
            
			Fame = 10000;
			Karma = -10000;
		}

        public override Poison HitPoison { get { return Poison.Deadly; } }
        public override int PoisonResistance { get { return 5; } }
		 
		public override int GetAngerSound()
		{
			return 0x56d;
		}

		public override int GetIdleSound()
		{
			return 0x56b;
		}

		public override int GetAttackSound()
		{
			return 0x56c;
		}

		public override int GetHurtSound()
		{
			return 0x56c;
		}

		public override int GetDeathSound()
		{
			return 0x56e;
		}

		public  FetidEssence ( Serial serial ) : base( serial )
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
