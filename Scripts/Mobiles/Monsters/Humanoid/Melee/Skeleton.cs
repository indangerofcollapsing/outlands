using System;
using System.Collections;
using Server.Items;
using Server.Targeting;


namespace Server.Mobiles
{
	[CorpseName( "a skeletal corpse" )]
	public class Skeleton : BaseCreature
	{
		[Constructable]
		public Skeleton() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a skeleton";
			Body = Utility.RandomList( 50, 56 );
			BaseSoundID = 0x48D;

            SetStr(25);
            SetDex(50);
            SetInt(25);

            SetHits(50);

            SetDamage(5, 10);

            SetSkill(SkillName.Wrestling, 40);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

			Fame = 450;
			Karma = -450;
		}

        public override Poison PoisonImmune { get { return Poison.Lethal; } }

		public override void OnDeath( Container c )
		{			
    		base.OnDeath( c );
		}
        		
		public Skeleton( Serial serial ) : base( serial )
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
