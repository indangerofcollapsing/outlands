using System;
using System.Collections;
using Server.Items;
using Server.Targeting;


namespace Server.Mobiles
{
	[CorpseName( "a gargoyle corpse" )]
	public class StoneGargoyle : BaseCreature
	{
		[Constructable]
		public StoneGargoyle() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a stone gargoyle";
			Body = 67;
			BaseSoundID = 0x174;

            SetStr(75);
            SetDex(50);
            SetInt(25);

            SetHits(400);

            SetDamage(15, 25);

            SetSkill(SkillName.Wrestling, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            VirtualArmor = 125;

			Fame = 4000;
			Karma = -4000;
		}

		public StoneGargoyle( Serial serial ) : base( serial )
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
