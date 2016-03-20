using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Spells;
using Server.Spells.Seventh;
using Server.Spells.Sixth;
using Server.Spells.Third;

namespace Server.Mobiles
{
	[CorpseName( "a mongbat corpse" )]
	public class MongbatLord : BaseCreature
	{
		[Constructable]
		public MongbatLord() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a mongbat lord";
			Body = 39;
            BaseSoundID = 422;
			Hue = 2589;

            SetStr(75);
            SetDex(50);
            SetInt(75);

            SetHits(250);
            SetMana(1000);

            SetDamage(10, 20);

            SetSkill(SkillName.Wrestling, 75);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 75);

            SetSkill(SkillName.Magery, 50);
            SetSkill(SkillName.EvalInt, 50);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 25;

			Fame = 12000;
			Karma = -8000;
		}	

		public MongbatLord( Serial serial ) : base( serial )
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
