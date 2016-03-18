using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a gazer larva corpse" )]
	public class GazerLarva : BaseCreature
	{
		[Constructable]
		public GazerLarva () : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a gazer larva";
			Body = 778;
			BaseSoundID = 377;

            SetStr(75);
            SetDex(50);
            SetInt(25);

            SetHits(125);

            SetDamage(5, 10);

            SetSkill(SkillName.Wrestling, 50);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;            

			Fame = 900;
			Karma = -900;		
		}

        public override int Meat { get { return 1; } }


		public GazerLarva( Serial serial ) : base( serial )
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
