using System;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Targeting;
using Server.ContextMenus;

namespace Server.Mobiles
{
	[CorpseName( "a ghostly corpse" )]
	public class RestlessSoul : BaseCreature
	{
		[Constructable]
		public RestlessSoul() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.4, 0.8 )
		{
			Name = "restless soul";
			Body = 0x3CA;
			Hue = 0x453;

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(50);

            SetDamage(3, 6);

            SetSkill(SkillName.Wrestling, 25);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

			Fame = 500;
			Karma = -500;
		}

        public override int PoisonResistance { get { return 5; } }

        public override bool AlwaysMurderer { get { return true; } }        

		public override void DisplayPaperdollTo(Mobile to)
		{
		}

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );

			for ( int i = 0; i < list.Count; ++i )
			{
				if ( list[i] is ContextMenus.PaperdollEntry )
					list.RemoveAt( i-- );
			}
		}

        public override int GetAngerSound() { return 0x381; }
        public override int GetIdleSound() { return 0x17F; }
        public override int GetAttackSound() { return 0x592; }
        public override int GetHurtSound() { return 0x594; }
        public override int GetDeathSound() { return 0x58D; }
        
		public RestlessSoul( Serial serial ) : base( serial )
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
