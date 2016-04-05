using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Misc;
using Server.Spells;
using Server.Spells.Seventh;
using Server.Spells.Sixth;
using Server.Spells.Third;

namespace Server.Mobiles
{
	[CorpseName( "a frost orc corpse" )]
	public class FrostOrc : BaseCreature
	{
		public override InhumanSpeech SpeechType{ get{ return InhumanSpeech.Orc; } }

		[Constructable]
		public FrostOrc() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = NameList.RandomName( "orc" );
			Body = 17;
			BaseSoundID = 0x45A;
            Hue = 2221;

            SetStr(75);
            SetDex(50);
            SetInt(25);

            SetHits(100);

            SetDamage(5, 10);

            SetSkill(SkillName.Wrestling, 55);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

			Fame = 1500;
			Karma = -1500;
		}

        public override bool CanRummageCorpses { get { return true; } }

		public override void OnDeath( Container c )
		{			
    		base.OnDeath( c );
		}

        public FrostOrc(Serial serial): base(serial)
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
