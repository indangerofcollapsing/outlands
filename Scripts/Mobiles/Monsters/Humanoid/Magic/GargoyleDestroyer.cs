using System;
using Server.Items;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Spells.Seventh;
using Server.Spells.Sixth;

namespace Server.Mobiles
{
	[CorpseName( "a gargoyle corpse" )]
	public class GargoyleDestroyer : BaseCreature
	{
		[Constructable]
		public GargoyleDestroyer() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Gargoyle Destroyer";
			Body = 0x2F3;
			BaseSoundID = 0x174;

            SetStr(75);
            SetDex(50);
            SetInt(100);

            SetHits(800);
            SetMana(2000);

            SetDamage(25, 35);

            SetSkill(SkillName.Wrestling, 95);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 75);

            SetSkill(SkillName.Magery, 75);
            SetSkill(SkillName.EvalInt, 75);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 75;

			Fame = 10000;
			Karma = -10000;
		}

        public override bool BardImmune { get { return false; } }
        public override int Meat { get { return 1; } }
        public override bool CanFly { get { return true; } }

		public override void GenerateLoot()
		{			
			AddLoot( LootPack.Gems, 2 );
		}	

		public GargoyleDestroyer( Serial serial ) : base( serial )
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
