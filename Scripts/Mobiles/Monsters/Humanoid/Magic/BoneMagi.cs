using System;
using Server;
using Server.Items;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Spells.Sixth;

namespace Server.Mobiles
{
	[CorpseName( "a skeletal corpse" )]
	public class BoneMagi : BaseCreature
	{
		[Constructable]
		public BoneMagi() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a bone mage";
			Body = 148;
			BaseSoundID = 451;

            SetStr(25);
            SetDex(25);
            SetInt(75);

            SetHits(100);
            SetMana(1000);

            SetDamage(4, 8);

            SetSkill(SkillName.Wrestling, 50);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            SetSkill(SkillName.Magery, 50);
            SetSkill(SkillName.EvalInt, 50);
            SetSkill(SkillName.Meditation, 75);

            VirtualArmor = 25;

			Fame = 3000;
			Karma = -3000;

            PackItem(new Bone(8));
		}

        public override Poison PoisonImmune { get { return Poison.Lethal; } }

		public override void OnDeath(Container c)
		{
			base.OnDeath(c);
			switch (Utility.Random(500))
			{
				case 0: { c.AddItem(SpellScroll.MakeMaster(new MassDispelScroll())); } break;
			}
		}

		public BoneMagi( Serial serial ) : base( serial )
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
