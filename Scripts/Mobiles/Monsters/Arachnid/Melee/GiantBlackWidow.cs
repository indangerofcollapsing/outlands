using System;
using Server.Items;
using Server.Targeting;
using System.Collections;

namespace Server.Mobiles
{
	[CorpseName( "a black widow corpse" )] 
	public class GiantBlackWidow : BaseCreature
	{
		[Constructable]
		public GiantBlackWidow() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a giant black widow";
			Body =  0x9D;
            Hue = 1107;
			BaseSoundID = 0x388;

            SetStr(50);
            SetDex(50);
            SetInt(75);

            SetHits(200);

            SetDamage(5, 10);

            SetSkill(SkillName.Wrestling, 70);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            SetSkill(SkillName.Poisoning, 15);

            VirtualArmor = 25;

			Fame = 3500;
			Karma = -3500;
		}

        public override Poison HitPoison { get { return Poison.Lethal; } }
        public override int PoisonResistance { get { return 5; } }

		public GiantBlackWidow( Serial serial ) : base( serial )
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
