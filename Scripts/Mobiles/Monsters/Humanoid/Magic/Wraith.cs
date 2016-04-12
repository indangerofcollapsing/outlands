using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a ghostly corpse" )]
	public class Wraith : BaseCreature
	{
		[Constructable]
		public Wraith() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a wraith";
			Body = 26;
            Hue = 2615;
			BaseSoundID = 0x482;

            SetStr(50);
            SetDex(50);
            SetInt(50);

            SetHits(100);
            SetMana(1000);

            SetDamage(6, 12);

            SetSkill(SkillName.Wrestling, 50);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            SetSkill(SkillName.Magery, 25);
            SetSkill(SkillName.EvalInt, 25);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 25;

			Fame = 4000;
			Karma = -4000;
		}

		public override int PoisonResistance{ get{ return 5; } }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }

		public Wraith( Serial serial ) : base( serial )
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
