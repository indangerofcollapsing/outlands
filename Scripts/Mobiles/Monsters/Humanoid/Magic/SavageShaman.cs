using System;
using System.Collections;
using Server;
using Server.Misc;
using Server.Items;
using Server.Spells;

namespace Server.Mobiles
{
	[CorpseName( "a savage corpse" )]
	public class SavageShaman : BaseCreature
	{
		[Constructable]
		public SavageShaman() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = NameList.RandomName( "savage shaman" );

			if ( Utility.RandomBool() )
				Body = 184;
			else
				Body = 183;

            SetStr(50);
            SetDex(50);
            SetInt(100);

            SetHits(500);
            SetMana(2000);

            SetDamage(11, 22);

            SetSkill(SkillName.Macing, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 75);

            SetSkill(SkillName.Magery, 75);
            SetSkill(SkillName.EvalInt, 75);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 25;

			Fame = 1000;
			Karma = -1000;
			
			PackItem( new Bandage( Utility.RandomMinMax( 1, 15 ) ) );

            AddItem( new GnarledStaff());
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

		public SavageShaman( Serial serial ) : base( serial )
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
