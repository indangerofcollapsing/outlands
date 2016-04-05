using System;
using Server;
using Server.Items;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Spells.Seventh;
using Server.Spells.Sixth;


namespace Server.Mobiles
{
	[CorpseName( "a gargoyle corpse" )]
	public class Gargoyle : BaseCreature
	{
		[Constructable]
		public Gargoyle() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a gargoyle";
			Body = 4;
			BaseSoundID = 372;

            SetStr(75);
            SetDex(50);
            SetInt(100);

            SetHits(250);
            SetMana(1000);

            SetDamage(9, 18);

            SetSkill(SkillName.Wrestling, 75);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 75);

            SetSkill(SkillName.Magery, 75);
            SetSkill(SkillName.EvalInt, 75);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 75;

			Fame = 3500;
			Karma = -3500;
		}

		public override bool CanFly { get { return true; } }

		public override void OnDeath( Container c )
		{			
    		base.OnDeath( c );
		}

		public Gargoyle( Serial serial ) : base( serial )
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
