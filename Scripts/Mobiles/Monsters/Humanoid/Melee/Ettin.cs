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
	[CorpseName( "an ettins corpse" )]
	public class Ettin : BaseCreature
	{
		[Constructable]
		public Ettin() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "an ettin";
			Body = 18;
			BaseSoundID = 367;

            SetStr(75);
            SetDex(50);
            SetInt(25);

            SetHits(300);

            SetDamage(11, 22);

            SetSkill(SkillName.Wrestling, 70);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

			Fame = 3000;
			Karma = -3000;
		}

		public override void OnDeath( Container c )
		{
			base.OnDeath(c);
		}

		public override bool CanRummageCorpses{ get{ return true; } }

		public Ettin( Serial serial ) : base( serial )
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
