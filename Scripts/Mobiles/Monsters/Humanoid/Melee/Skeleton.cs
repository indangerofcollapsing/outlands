using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Achievements;

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

            if (Utility.RandomMinMax(1, 5) == 1)
            {
                switch (Utility.RandomMinMax(1, 5))
                {
                    case 1: PackItem(new BoneHelm()); break;
                    case 2: PackItem(new BoneChest()); break;
                    case 3: PackItem(new BoneArms()); break;
                    case 4: PackItem(new BoneLegs()); break;
                    case 5: PackItem(new BoneGloves()); break;
                }
            }

            PackItem(new Bone(4));
		}

        public override Poison PoisonImmune { get { return Poison.Lethal; } }

		public override void OnDeath( Container c )
		{			
    		base.OnDeath( c );

			AwardDailyAchievementForKiller(NewbCategory.KillSkeletons);

            if (Utility.RandomMinMax(1, 5) == 1)
                c.AddItem(new Bonemeal());
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
