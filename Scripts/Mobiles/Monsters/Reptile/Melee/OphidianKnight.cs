using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "an ophidian knight corpse" )]
	public class OphidianKnight : BaseCreature
	{
		[Constructable]
		public OphidianKnight() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
            Name = "an ophidian knight";

			Body = 86;
			BaseSoundID = 634;
            Hue = 2534;

            SetStr(100);
            SetDex(75);
            SetInt(25);

            SetHits(400);

            SetDamage(20, 30);

            SetSkill(SkillName.Wrestling, 95);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            SetSkill(SkillName.Poisoning, 15);

            VirtualArmor = 25;

			Fame = 10000;
			Karma = -10000;
		}
        
        public override Poison HitPoison { get { return Poison.Deadly; } }
        public override int PoisonResistance { get { return 4; } }       
		
		public OphidianKnight( Serial serial ) : base( serial )
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
