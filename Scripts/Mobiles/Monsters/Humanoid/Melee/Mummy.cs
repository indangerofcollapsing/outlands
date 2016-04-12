using System;
using System.Collections;
using Server.Items;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Spells.Sixth;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a mummy corpse" )]
	public class Mummy : BaseCreature
	{
		[Constructable]
		public Mummy() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.4, 0.8 )
		{
			Name = "a mummy";
			Body = 154;
			BaseSoundID = 471;

            SetStr(75);
            SetDex(25);
            SetInt(25);

            SetHits(300);

            SetDamage(20, 25);

            SetSkill(SkillName.Wrestling, 70);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 50;

			Fame = 4000;
			Karma = -4000;
		}

        public override int PoisonResistance { get { return 5; } }

		public Mummy( Serial serial ) : base( serial )
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
