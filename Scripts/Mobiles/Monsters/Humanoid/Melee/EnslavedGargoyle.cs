using System;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "an enslaved gargoyle corpse" )]
	public class EnslavedGargoyle : BaseCreature
	{
		[Constructable]
		public EnslavedGargoyle() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "an enslaved gargoyle";
			Body = 0x2F1;
			BaseSoundID = 0x174;

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(300);

            SetDamage(8, 16);

            SetSkill(SkillName.Wrestling, 60);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 75);

            VirtualArmor = 75;

			Fame = 3500;
			Karma = 0;
		}

		public EnslavedGargoyle( Serial serial ) : base( serial )
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
