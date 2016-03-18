using System;
using Server;
using Server.Misc;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a savage corpse" )]
	public class Savage : BaseCreature
	{
		[Constructable]
		public Savage() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = NameList.RandomName( "savage" );

			if ( Female = Utility.RandomBool() )
				Body = 184;
			else
				Body = 183;

            SetStr(50);
            SetDex(75);
            SetInt(25);

            SetHits(400);

            SetDamage(9, 18);

            SetSkill(SkillName.Fencing, 85);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 75);

            VirtualArmor = 50;

			Fame = 1000;
			Karma = -1000;
            
			AddItem( new Spear() );
			AddItem( new BoneArms() );
			AddItem( new BoneLegs() );
		}

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomMinMax(1, 5) == 1)
                c.AddItem(new Bloodroot());
        }

        public override int Meat { get { return 1; } }
        public override bool AlwaysMurderer { get { return true; } }
        public override bool ShowFameTitle { get { return false; } }

		public Savage( Serial serial ) : base( serial )
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
