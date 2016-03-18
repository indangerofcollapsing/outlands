using System;
using Server;
using Server.Items;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Spells.Sixth;

namespace Server.Mobiles
{
	[CorpseName( "a ghostly corpse" )]
	public class Shade : BaseCreature
	{
		[Constructable]
		public Shade() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a shade";
			Body = 26;
            Hue = 2101; //0x4001;
			BaseSoundID = 0x482;

            SetStr(50);
            SetDex(50);
            SetInt(50);

            SetHits(100);
            SetMana(1000);

            SetDamage(4, 8);

            SetSkill(SkillName.Wrestling, 40);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            SetSkill(SkillName.Magery, 25);
            SetSkill(SkillName.EvalInt, 25);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 25;

			Fame = 4000;
			Karma = -4000;

			PackReg( 10 );
		}
		
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomMinMax(1, 5) == 1)
                c.AddItem(new Bonemeal());
        }

		public Shade( Serial serial ) : base( serial )
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
