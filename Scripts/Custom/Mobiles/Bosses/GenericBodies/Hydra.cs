using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Misc;
using Server.Spells;
using Server.Spells.Seventh;
using Server.Spells.Sixth;
using Server.Spells.Third;
using Server.Achievements;
namespace Server.Mobiles
{
	[CorpseName( "a hydra corpse" )]
	public class Hydra : BaseCreature
	{
		[Constructable]
		public Hydra() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
            Name = "a hydra";
			Body = 265;

			SetStr( 100 );
			SetDex( 50 );
			SetInt( 25 );

			SetHits( 5000 );
            SetStam( 2500 );

			SetDamage( 20, 30 );

            SetSkill(SkillName.Wrestling, 80);
            SetSkill(SkillName.Tactics, 100);

			SetSkill( SkillName.MagicResist, 50 );

			Fame = 10000;
			Karma = -10000;

			VirtualArmor = 25;
		}

    //public override int GoldWorth { get { return Utility.RandomMinMax(1200, 1600); } }
		
        
        public Hydra(Serial serial): base(serial)
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
