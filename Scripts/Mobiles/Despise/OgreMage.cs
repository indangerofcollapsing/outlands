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
	[CorpseName( "an ogre mage corpse" )]
	public class OgreMage : BaseCreature
	{
		[Constructable]
		public OgreMage () : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "an ogre mage";
			Body = 1;
			BaseSoundID = 427;
            Hue = 2500;

            SetStr(75);
            SetDex(50);
            SetInt(75);

            SetHits(450);
            SetMana(2000);

            SetDamage(20, 30);

            SetSkill(SkillName.Wrestling, 95);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 75);

            SetSkill(SkillName.Magery, 75);
            SetSkill(SkillName.EvalInt, 75);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 25;
            
			Fame = 3000;
			Karma = -3000;

			PackItem( new Club() );
		}

        public override bool CanRummageCorpses { get { return true; } }

		public override bool OnBeforeDeath()
		{
			return base.OnBeforeDeath();
		}

		public OgreMage( Serial serial ) : base( serial )
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
