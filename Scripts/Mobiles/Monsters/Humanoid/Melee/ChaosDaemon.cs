using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Achievements;

namespace Server.Mobiles
{
	[CorpseName( "a chaos daemon corpse" )]
	public class ChaosDaemon : BaseCreature
	{
		[Constructable]
		public ChaosDaemon() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a chaos daemon";
			Body = 792;
			BaseSoundID = 0x3E9;

            SetStr(100);
            SetDex(76);
            SetInt(25);

            SetHits(750);

            SetDamage(25, 35);

            SetSkill(SkillName.Wrestling, 100);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            VirtualArmor = 25;            

			Fame = 10000;
			Karma = -15000;
		}
        
		public ChaosDaemon( Serial serial ) : base( serial )
		{
		}

        public override void OnDeath( Container c )
        {     
          base.OnDeath( c );
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
