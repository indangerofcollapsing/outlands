using System;
using Server;
using Server.Items;

namespace Server.Custom.Townsystem
{
	public class FactionDeathKnight : BaseFactionGuard
	{
		[Constructable]
		public FactionDeathKnight() : base( "the death knight" )
		{
            SetStr(100);
            SetDex(75);
            SetInt(25);

            SetHits(700);

            SetDamage(20, 30);

            SetSkill(SkillName.Swords, 100);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 25);
            SetSkill(SkillName.EvalInt, 25);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.MagicResist, 100);

            VirtualArmor = 25;

            GenerateBody(false, false);

			Hue = 1;

			Item shroud = new Item( 0x204E );
			shroud.Layer = Layer.OuterTorso;

			AddItem( Immovable( Rehued( shroud, 1109 ) ) );

			AddItem( Newbied( Rehued( new ExecutionersAxe(), 2211 ) ) );

			PackItem( new Bandage( Utility.RandomMinMax( 30, 40 ) ) );
			PackStrongPotions( 6, 12 );
		}

		public FactionDeathKnight( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); //version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}