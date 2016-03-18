using System;
using Server;
using Server.Items;

namespace Server.Custom.Townsystem
{
	public class FactionBerserker : BaseFactionGuard
	{
		[Constructable]
		public FactionBerserker() : base( "the berserker" )
		{
            SetStr(75);
            SetDex(100);
            SetInt(25);

            SetHits(700);
            SetStam(700);

            SetDamage(20, 30);

            SetSkill(SkillName.Swords, 100);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            GenerateBody(false, false);

			AddItem( Immovable( Rehued( new BodySash(), 1645 ) ) );
			AddItem( Immovable( Rehued( new Kilt(), 1645 ) ) );
			AddItem( Immovable( Rehued( new Sandals(), 1645 ) ) );

			AddItem( Newbied( new DoubleAxe() ) );

			HairItemID = 0x2047;
			HairHue = 0x29;

			FacialHairItemID = 0x204B;
			FacialHairHue = 0x29;

			PackItem( new Bandage( Utility.RandomMinMax( 30, 40 ) ) );
			PackStrongPotions( 6, 12 );
		}

		public FactionBerserker( Serial serial ) : base( serial )
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