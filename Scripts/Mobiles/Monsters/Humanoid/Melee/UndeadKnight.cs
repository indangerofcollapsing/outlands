using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "an undead knight corpse" )]
	public class UndeadKnight : BaseCreature
	{
		[Constructable]
		public UndeadKnight() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "an undead knight";
			Body = 57;
			Hue = 2610;
			BaseSoundID = 451;

            SetStr(75);
            SetDex(25);
            SetInt(25);

            SetHits(350);

            SetDamage(15, 25);

            SetSkill(SkillName.Wrestling, 95);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 75;            

			Fame = 7000;
			Karma = -7000;
		}

        public override Poison PoisonImmune { get { return Poison.Lethal; } }

		public override bool OnBeforeDeath()
		{			
			return base.OnBeforeDeath();
		}

		public UndeadKnight( Serial serial ) : base( serial )
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