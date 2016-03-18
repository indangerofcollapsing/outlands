using System;
using System.Collections;
using Server.Items;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Spells.Sixth;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a risen noble's corpse" )]
	public class RisenNoble : BaseCreature
	{
		[Constructable]
		public RisenNoble() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.4, 0.8 )
		{
			Name = "a risen noble";

            Hue = 2587;
			Body = 154;
			BaseSoundID = 471;

            SetStr(50);
            SetDex(50);
            SetInt(100);

            SetHits(600);
            SetMana(2000);

            SetDamage(15, 25);

            SetSkill(SkillName.Wrestling, 95);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 25;

			Fame = 4000;
			Karma = -4000;

            PackItem(new Bone(10));
            PackItem(new Bandage(10));
		}

        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public RisenNoble(Serial serial): base(serial)
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
