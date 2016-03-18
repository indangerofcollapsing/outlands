using System;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Targeting;
using Server.Engines.Quests;
using Server.Engines.Quests.Haven;
using Server.ContextMenus;

namespace Server.Mobiles
{
	public class CorruptedSoul : BaseCreature
	{
		public override bool DeleteCorpseOnDeath{ get{ return true; } }
		
		[Constructable]
		public CorruptedSoul() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, .1, 5 )
		{
			Name = "a corrupted soul";
			Body = 0x3CA;
			Hue = 0x453;

            SetStr(25);
            SetDex(75);
            SetInt(100);

            SetHits(125);

            SetDamage(12, 24);

            SetSkill(SkillName.Wrestling, 80);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 75);

            VirtualArmor = 25;

			Fame = 5000;
			Karma = -5000;
		}

        public override bool AlwaysMurderer { get { return true; } }

		public override int GetAttackSound()
		{
			return 0x233;
		}

		public override bool OnBeforeDeath()
		{
			if ( !base.OnBeforeDeath() )
				return false;

			Effects.SendLocationEffect( Location, Map, 0x376A, 10, 1 );

			return true;
		}

		public CorruptedSoul( Serial serial ) : base( serial )
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
