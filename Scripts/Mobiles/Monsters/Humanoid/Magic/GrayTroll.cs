using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Spells;
using Server.Spells.Seventh;
using Server.Spells.Sixth;
using Server.Spells.Third;

namespace Server.Mobiles
{
	[CorpseName( "a gray troll corpse" )]
	public class GrayTroll : BaseCreature
	{
		[Constructable]
		public GrayTroll () : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a gray troll";
			Body = Utility.RandomList( 53, 54 );
			BaseSoundID = 461;
			Hue = 2101;

            SetStr(75);
            SetDex(50);
            SetInt(75);

            SetHits(250);
            SetMana(1000);

            SetDamage(10, 20);

            SetSkill(SkillName.Wrestling, 80);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            SetSkill(SkillName.Magery, 50);
            SetSkill(SkillName.EvalInt, 50);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 25;

			Fame = 9500;
			Karma = -9500;
		}        

		public override void OnDeath( Container c )
		{			
    		base.OnDeath( c );
		}	
		
		public override bool CanRummageCorpses{ get{ return true; } }	  	

		public GrayTroll( Serial serial ) : base( serial )
		{
		}

		public override void OnThink()
		{
			base.OnThink();
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
