using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "an abyssmal horror corpse" )]
	public class AbysmalHorror : BaseCreature
	{
		[Constructable]
		public AbysmalHorror() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "an abyssmal horror";
			Body = 312;
			BaseSoundID = 0x451;
            Hue = 2075;

            SetStr(100);
            SetDex(50);
            SetInt(25);

            SetHits(2500);

            SetDamage(20, 30);

            SetSkill(SkillName.Wrestling, 95);
            SetSkill(SkillName.Tactics, 100);           

            SetSkill(SkillName.MagicResist, 100);

            SetSkill(SkillName.Poisoning, 20);

            VirtualArmor = 25;

			Fame = 26000;
			Karma = -26000;
		}

        public override Poison HitPoison { get { return Poison.Deadly; } }
        public override int PoisonResistance { get { return 4; } }


		public AbysmalHorror( Serial serial ) : base( serial )
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