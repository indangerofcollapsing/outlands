using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Factions;

namespace Server.Mobiles
{
	[CorpseName( "a charred protector's corpse" )]
	public class CharredProtector : BaseCreature
	{
		public override Faction FactionAllegiance { get { return Minax.Instance; } }
		public override Ethics.Ethic EthicAllegiance { get { return Ethics.Ethic.Evil; } }

		[Constructable]
		public CharredProtector () : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "an charred protector";
			Body = 0x12D;
			BaseSoundID = 427;
			Hue = 2412;

			SetStr( 100 );
			SetDex( 50 );
			SetInt( 25 );

			SetHits( 750 );

			SetDamage(30, 40);

            SetSkill(SkillName.Wrestling, 70);
            SetSkill(SkillName.Tactics, 100);

			SetSkill( SkillName.MagicResist, 150 );

            VirtualArmor = 75;

			Fame = 25000;
			Karma = -15000;
		}
		
		public override bool CanRummageCorpses{ get{ return true; } }
		public override int Meat{ get{ return 2; } }

		public CharredProtector( Serial serial ) : base( serial )
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
