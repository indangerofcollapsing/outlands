using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Factions;
using Server.Spells;
using Server.Spells.Seventh;
using Server.Spells.Sixth;
using Server.Spells.Third;



namespace Server.Mobiles
{
	[CorpseName( "an ogre lords corpse" )]
	public class OgreLord : BaseCreature
	{
		[Constructable]
		public OgreLord () : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "an ogre lord";
			Body = 83;
			BaseSoundID = 427;

            SetStr(100);
            SetDex(50);
            SetInt(25);

            SetHits(750);

            SetDamage(25, 35);

            SetSkill(SkillName.Wrestling, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            VirtualArmor = 25;

            Fame = 15000;
            Karma = -15000;
		}

        public override bool CanRummageCorpses { get { return true; } }

		public override void OnDeath( Container c )
		{
    		base.OnDeath( c );
		}

		public override bool OnBeforeDeath()
		{
			return base.OnBeforeDeath();
		}
		
		public OgreLord( Serial serial ) : base( serial )
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
