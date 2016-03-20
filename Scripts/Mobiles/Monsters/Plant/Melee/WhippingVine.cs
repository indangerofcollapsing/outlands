using System;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a whipping vine corpse" )]
	public class WhippingVine : BaseCreature
	{
		[Constructable]
		public WhippingVine() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a whipping vine";
			Body = 8;
			Hue = 0x851;
			BaseSoundID = 352;

            SetStr(50);
            SetDex(100);
            SetInt(25);

            SetHits(350);

            SetDamage(10, 20);

            SetSkill(SkillName.Wrestling, 75);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

			Fame = 1000;
			Karma = -1000;			
		}

        public override Poison PoisonImmune { get { return Poison.Lethal; } }

		public WhippingVine( Serial serial ) : base( serial )
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