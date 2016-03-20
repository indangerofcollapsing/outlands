using System;
using Server;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a pixie corpse" )]
	public class Pixie : BaseCreature
	{
		public override bool InitialInnocent{ get{ return true; } }

		[Constructable]
		public Pixie() : base( AIType.AI_Mage, FightMode.Evil, 10, 1, 0.2, 0.4 )
		{
			Name = NameList.RandomName( "pixie" );
			Body = 128;
			BaseSoundID = 0x467;

            SetStr(50);
            SetDex(75);
            SetInt(100);

            SetHits(100);
            SetMana(2000);

            SetDamage(4, 8);

            SetSkill(SkillName.Wrestling, 50);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 150);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 25;
            
			Fame = 7000;
			Karma = 7000;
		}
        
		public Pixie( Serial serial ) : base( serial )
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
