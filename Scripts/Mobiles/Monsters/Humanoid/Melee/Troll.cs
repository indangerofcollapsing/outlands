using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Achievements;

namespace Server.Mobiles
{
	[CorpseName( "a troll corpse" )]
	public class Troll : BaseCreature
	{
		[Constructable]
		public Troll () : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a troll";

			Body = Utility.RandomList( 53, 54 );
			BaseSoundID = 461;

            SetStr(75);
            SetDex(50);
            SetInt(25);

            SetHits(250);

            SetDamage(8, 16);

            SetSkill(SkillName.Wrestling, 60);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;            

			Fame = 3500;
			Karma = -3500;
		}

        public override bool CanRummageCorpses { get { return true; } }

		public override void OnDeath( Container c )
		{			
    		base.OnDeath( c );
		}	

		public Troll( Serial serial ) : base( serial )
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
