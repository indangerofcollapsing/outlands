using System;
using Server.Items;
using Server.Targeting;
using System.Collections;

namespace Server.Mobiles
{
	[CorpseName( "a terathan drone corpse" )]
	public class TerathanDrone : BaseCreature
	{
		[Constructable]
		public TerathanDrone() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a terathan drone";
			Body = 71;
			BaseSoundID = 594;

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(125);

            SetDamage(5, 10);

            SetSkill(SkillName.Wrestling, 65);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            SetSkill(SkillName.Poisoning, 10);

            VirtualArmor = 25;

			Fame = 2000;
			Karma = -2000;
		}

        public override Poison HitPoison { get { return Poison.Regular; } }
        public override Poison PoisonImmune { get { return Poison.Regular; } }   
        
		public TerathanDrone( Serial serial ) : base( serial )
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
