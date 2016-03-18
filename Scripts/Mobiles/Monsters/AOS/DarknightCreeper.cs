using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a darknight creeper corpse" )]
	public class DarknightCreeper : BaseCreature
	{
		public override bool IgnoreYoungProtection { get { return Core.ML; } }

		[Constructable]
		public DarknightCreeper() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = NameList.RandomName( "darknight creeper" );
			Body = 313;
			BaseSoundID = 0xE0;
            Hue = 2539;

            SetStr(100);
            SetDex(50);
            SetInt(100);

            SetHits(2500);

            SetDamage(25, 40);

            SetSkill(SkillName.Wrestling, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            SetSkill(SkillName.Hiding, 95);
            SetSkill(SkillName.Stealth, 95);

            VirtualArmor = 25;
            
			Fame = 22000;
			Karma = -22000;
		}
        
		public DarknightCreeper( Serial serial ) : base( serial )
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

			if ( BaseSoundID == 471 )
				BaseSoundID = 0xE0;
		}
	}
}