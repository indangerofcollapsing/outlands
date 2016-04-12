using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a risen noble's corpse" )]
	public class RisenKnight : BaseCreature
	{
		[Constructable]
		public RisenKnight() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a risen knight";
			Body = 57;
			Hue = 2587;
			BaseSoundID = 451;

            SetStr(75);
            SetDex(50);
            SetInt(25);

            SetHits(550);

            SetDamage(20, 35);

            SetSkill(SkillName.Wrestling, 100);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 75;            

			Fame = 7000;
			Karma = -7000;
		}

        public override int PoisonResistance { get { return 5; } }
        
        public RisenKnight(Serial serial): base(serial)
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