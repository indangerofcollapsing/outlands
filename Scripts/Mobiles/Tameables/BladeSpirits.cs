using System;
using System.Collections;
using Server.Misc;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a blade spirit corpse" )]
	public class BladeSpirits : BaseCreature
	{
        public override bool DeleteCorpseOnDeath { get { return Summoned; } }
        public override bool AlwaysMurderer { get { return true; } }
		public override bool IsHouseSummonable { get { return true; } }

		public override double DispelDifficulty { get { return 0.0; } }
		public override double DispelFocus { get { return 20.0; } }

		[Constructable]
		public BladeSpirits(): base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.3, 0.6 )
		{
			Name = "a blade spirit";
			Body = 574;

            SetStr(100);
            SetDex(75);
            SetInt(25);

            SetHits(600);

            SetDamage(18, 20);

            SetSkill(SkillName.Wrestling, 100);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 100;

            Fame = 0;
            Karma = 0;            

            ControlSlots = 2;
		}

        public override void SetUniqueAI()
        {
            ReturnsHome = false;
        }
		
		public override Poison PoisonImmune { get { return Poison.Lethal; } }

		public override int GetAngerSound(){return 0x23A;}
		public override int GetAttackSound(){return 0x3B8;}
		public override int GetHurtSound(){return 0x23A;}

		public BladeSpirits( Serial serial ): base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}