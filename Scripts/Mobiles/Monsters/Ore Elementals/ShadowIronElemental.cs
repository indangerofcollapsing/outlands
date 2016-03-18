using System;
using Server;
using Server.Items;
using Server.Achievements;

namespace Server.Mobiles
{
	[CorpseName( "a shadow iron elemental corpse" )]
	public class ShadowIronElemental : BaseCreature
	{
        public DateTime m_NextReflectAllowed;

        public static double oreElementalLevel = 4;
        public static double oreElementalMaxLevel = 9;

        public double NextReflectDelayMin = 1.0 + ((oreElementalMaxLevel - oreElementalLevel) * .25);
        public double NextReflectDelayMax = 2.0 + ((oreElementalMaxLevel - oreElementalLevel) * .5);

		[Constructable]
		public ShadowIronElemental() : this( 2 )
		{
		}

		[Constructable]
		public ShadowIronElemental( int oreAmount ) : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a shadow iron elemental";
			Body = 111;
			BaseSoundID = 268;

            SetStr(100);
            SetDex(25);
            SetInt(25);

            SetHits(400);

            SetDamage(14, 28);

            SetSkill(SkillName.Wrestling, 85);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 150;

			Fame = 4500;
			Karma = -4500;	

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

        public override bool OnBeforeDeath()
        {
            PackItem(new ShadowIronOre(2));

            return base.OnBeforeDeath();
        }

        public override void SetUniqueAI()
        {
            if (Global_AllowAbilities)
                UniqueCreatureDifficultyScalar = 1.04;
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Global_AllowAbilities)
            {
                if (!Hidden && DateTime.UtcNow > m_NextReflectAllowed && MagicDamageAbsorb < 1)
                {
                    FixedParticles(0x375A, 10, 15, 5037, EffectLayer.Waist);
                    MagicDamageAbsorb = 1;

                    PlaySound(0x1E9);

                    double reflectDelay = NextReflectDelayMin + (Utility.RandomDouble() * (NextReflectDelayMax - NextReflectDelayMin));
                    m_NextReflectAllowed = DateTime.UtcNow + TimeSpan.FromSeconds(reflectDelay);
                }
            }
        }

		public ShadowIronElemental( Serial serial ) : base( serial )
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