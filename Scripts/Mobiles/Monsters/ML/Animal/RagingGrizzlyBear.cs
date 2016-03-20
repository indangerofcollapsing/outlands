using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a grizzly bear corpse" )]
	public class RagingGrizzlyBear : BaseCreature
	{
		[Constructable]
		public RagingGrizzlyBear() : base( AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			Name = "a raging grizzly bear";
			Body = 212;
			BaseSoundID = 0xA3;

            SetStr(100);
            SetDex(50);
            SetInt(25);

            SetHits(1500);

            SetDamage(15, 30);

            SetSkill(SkillName.Wrestling, 80);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

			Fame = 10000;
			Karma = 10000;			
		}
        
		public RagingGrizzlyBear( Serial serial ) : base( serial )
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}
