using System;
using System.Collections;
using Server.Misc;
using Server.Items;
using Server.Targeting;


namespace Server.Mobiles
{
	[CorpseName( "an orcish corpse" )]
	public class OrcishLord : BaseCreature
	{
		public override InhumanSpeech SpeechType{ get{ return InhumanSpeech.Orc; } }

		[Constructable]
		public OrcishLord() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "an orcish lord";
			Body = 138;
			BaseSoundID = 0x45A;

            SetStr(75);
            SetDex(50);
            SetInt(25);

            SetHits(200);

            SetDamage(7, 14);

            SetSkill(SkillName.Wrestling, 75);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

			Fame = 2500;
			Karma = -2500;
		}

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }	
		
		public override bool CanRummageCorpses{ get{ return true; } }

		public OrcishLord( Serial serial ) : base( serial )
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
