using System;
using System.Collections;
using Server.Items;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Spells.Sixth;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a rotting corpse" )]
	public class ZombieMagi : BaseCreature
	{
        public DateTime TatteredWrappingEnd = DateTime.MinValue;

		[Constructable]
        public ZombieMagi(): base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
		{
			Name = "a zombie magi";
			Body = 3;
			BaseSoundID = 471;
            Hue = 2585;

            SetStr(50);
            SetDex(25);
            SetInt(50);

            SetHits(150);
            SetMana(1000);

            SetDamage(4, 8);

            SetSkill(SkillName.Wrestling, 30);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            SetSkill(SkillName.Magery, 25);
            SetSkill(SkillName.EvalInt, 25);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 25;

            Fame = 3000;
            Karma = -3000;
		}

        public override int PoisonResistance { get { return 5; } }

        public ZombieMagi(Serial serial): base(serial)
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
