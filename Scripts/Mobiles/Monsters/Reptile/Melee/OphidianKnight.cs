using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Custom.Ubercrafting;

namespace Server.Mobiles
{
	[CorpseName( "an ophidian corpse" )]
	[TypeAlias( "Server.Mobiles.OphidianAvenger" )]
	public class OphidianKnight : BaseCreature
	{
		private static string[] m_Names = new string[]
			{
				"an ophidian knight-errant",
				"an ophidian avenger"
			};

		[Constructable]
		public OphidianKnight() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = m_Names[Utility.Random( m_Names.Length )];
			Body = 86;
			BaseSoundID = 634;
            Hue = 2534;

            SetStr(100);
            SetDex(75);
            SetInt(25);

            SetHits(400);

            SetDamage(20, 30);

            SetSkill(SkillName.Wrestling, 95);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            SetSkill(SkillName.Poisoning, 15);

            VirtualArmor = 25;

			Fame = 10000;
			Karma = -10000;

			PackItem( new LesserPoisonPotion() );
		}

        public override int Meat { get { return 2; } }

        public override Poison HitPoison { get { return Poison.Deadly; } }
        public override Poison PoisonImmune { get { return Poison.Deadly; } }       
		
		public OphidianKnight( Serial serial ) : base( serial )
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
