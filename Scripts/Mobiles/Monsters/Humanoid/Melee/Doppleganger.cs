using System;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a doppleganger corpse" )]
	public class Doppleganger : BaseCreature
	{
		[Constructable]
		public Doppleganger() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a doppleganger";
			Body = 0x309;
			BaseSoundID = 0x451;

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(200);

            SetDamage(8, 16);

            SetSkill(SkillName.Wrestling, 85);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 75);

            VirtualArmor = 25;            

			Fame = 1000;
			Karma = -1000;

			VirtualArmor = 55;
		}
		
		public override int Hides{ get{ return 6; } }
		public override int Meat{ get{ return 1; } }

		public Doppleganger( Serial serial ) : base( serial )
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
