using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a wisp corpse" )]
	public class ShadowWisp : BaseCreature
	{
		[Constructable]
		public ShadowWisp() : base( AIType.AI_Mage, FightMode.Aggressor, 10, 1, 0.3, 0.6 )
		{
			Name = "a shadow wisp";
			Body = 165;
			BaseSoundID = 466;

            SetStr(50);
            SetDex(75);
            SetInt(100);

            SetHits(250);
            SetMana(2000);

            SetDamage(8, 16);

            SetSkill(SkillName.Wrestling, 100);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 150);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 25;

            Fame = 4000;
            Karma = -4000;

			AddItem( new LightSource() );
		}

		public ShadowWisp( Serial serial ) : base( serial )
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