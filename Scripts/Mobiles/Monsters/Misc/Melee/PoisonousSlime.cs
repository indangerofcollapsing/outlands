using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a slimey corpse" )]
	public class PoisonousSlime : BaseCreature
	{
		[Constructable]
		public PoisonousSlime () : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a poisonous slime";

			Body = 51;
			BaseSoundID = 0;
            Hue = 2963;

            SetStr(50);
            SetDex(25);
            SetInt(25);

            SetHits(150);

            SetDamage(7, 14);

            SetSkill(SkillName.Wrestling, 70);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 150);

            SetSkill(SkillName.Poisoning, 25);

            Fame = 1000;
            Karma = -1000;
		}

        public override int PoisonResistance { get { return 3; } }
        public override Poison HitPoison { get { return Poison.Greater; } }
        
		public PoisonousSlime( Serial serial ) : base( serial )
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