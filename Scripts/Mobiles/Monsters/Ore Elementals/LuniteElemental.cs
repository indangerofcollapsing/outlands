using System;
using Server;
using Server.Items;
using Server.Achievements;

namespace Server.Mobiles
{
	[CorpseName( "a lunite elemental corpse" )]
	public class LuniteElemental : BaseCreature
	{
        public DateTime m_NextReflectAllowed;

        public static double oreElementalLevel = 9;
        public static double oreElementalMaxLevel = 9;

        public double NextReflectDelayMin = 1.0 + ((oreElementalMaxLevel - oreElementalLevel) * .25);
        public double NextReflectDelayMax = 2.0 + ((oreElementalMaxLevel - oreElementalLevel) * .5);

		[Constructable]
		public LuniteElemental() : this( 1 )
		{
		}

		[Constructable]
		public LuniteElemental( int oreAmount ) : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a lunite elemental";

			Body = 113;
            Hue = 2603;
			BaseSoundID = 268;

            SetStr(100);
            SetDex(50);
            SetInt(25);

            SetHits(2000);

            SetDamage(30, 50);

            SetSkill(SkillName.Wrestling, 110);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 200;

            Fame = 3500;
            Karma = -3500;
		}

        public override bool OnBeforeDeath()
        {
            PackItem(new LuniteOre(1));

            return base.OnBeforeDeath();
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
            AwardAchievementForKiller(AchievementTriggers.Trigger_EarthElementalKilled);

            switch (Utility.Random(1000))
            {
                case 0: { c.AddItem(SpellScroll.MakeMaster(new SummonEarthElementalScroll())); } break;
            }
        }

        public override void SetUniqueAI()
        {
            UniqueCreatureDifficultyScalar = 1.2;
        }

        public override void OnThink()
        {
            base.OnThink();

            if (!Hidden && DateTime.UtcNow > m_NextReflectAllowed && MagicDamageAbsorb < 1)
            {
                FixedParticles(0x375A, 10, 15, 5037, EffectLayer.Waist);
                MagicDamageAbsorb = 1;

                PlaySound(0x1E9);

                double reflectDelay = NextReflectDelayMin + (Utility.RandomDouble() * (NextReflectDelayMax - NextReflectDelayMin));
                m_NextReflectAllowed = DateTime.UtcNow + TimeSpan.FromSeconds(reflectDelay);
            }            
        }

		public LuniteElemental( Serial serial ) : base( serial )
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
