using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Spells;
using Server.Spells.Seventh;
using Server.Spells.Sixth;
using Server.Spells.Third;
using Server.Achievements;

namespace Server.Mobiles
{
	[CorpseName( "an ogre corpse" )]
	public class Ogre : BaseCreature
	{
		[Constructable]
		public Ogre () : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "an ogre";
			Body = 1;
			BaseSoundID = 427;

            SetStr(75);
            SetDex(50);
            SetInt(25);

            SetHits(250);

            SetDamage(10, 20);

            SetSkill(SkillName.Wrestling, 65);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;
            
			Fame = 3000;
			Karma = -3000;
		}

        public override bool CanRummageCorpses { get { return true; } }

		public override bool OnBeforeDeath()
		{
			return base.OnBeforeDeath();
		}

		public Ogre( Serial serial ) : base( serial )
		{ 
		}

		public override void OnDeath(Container c)
		{
			base.OnDeath(c);
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
