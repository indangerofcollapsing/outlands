using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a gore fiend corpse" )]
	public class GoreFiend : BaseCreature
	{
		[Constructable]
		public GoreFiend() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a gore fiend";
			Body = 305;
			BaseSoundID = 224;
            Hue = 1779;

            SetStr(100);
            SetDex(50);
            SetInt(25);

            SetHits(300);

            SetDamage(8, 16);

            SetSkill(SkillName.Wrestling, 60);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 25;

			Fame = 1500;
			Karma = -1500;
		}

		public override int GetDeathSound()
		{
			return 1218;
		}

		public GoreFiend( Serial serial ) : base( serial )
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