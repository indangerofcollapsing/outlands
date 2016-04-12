using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a corpser corpse" )]
	public class Corpser : BaseCreature
	{
		[Constructable]
		public Corpser() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a corpser";
			Body = 8;
			BaseSoundID = 684;

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(200);

            SetDamage(8, 16);

            SetSkill(SkillName.Wrestling, 50);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

			Fame = 1000;
			Karma = -1000;	      
		}

        public override int PoisonResistance { get { return 5; } }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c); 
        }
		
		public override bool DisallowAllMoves{ get{ return true; } }

		public Corpser( Serial serial ) : base( serial )
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
