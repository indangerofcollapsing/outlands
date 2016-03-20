using System;
using Server;
using Server.Items;
using Server.Achievements;

namespace Server.Mobiles
{
	[CorpseName( "a valorite elemental corpse" )]
	public class ValoriteElemental : BaseCreature
	{
        public DateTime m_NextReflectAllowed;

        public static double oreElementalLevel = 8;
        public static double oreElementalMaxLevel = 9;

        public double NextReflectDelayMin = 1.0 + ((oreElementalMaxLevel - oreElementalLevel) * .25);
        public double NextReflectDelayMax = 2.0 + ((oreElementalMaxLevel - oreElementalLevel) * .5);

		[Constructable]
		public ValoriteElemental() : this( 2 )
		{
		}

		[Constructable]
		public ValoriteElemental( int oreAmount ) : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{			
			Name = "a valorite elemental";
			Body = 112;
			BaseSoundID = 268;

            SetStr(100);
            SetDex(25);
            SetInt(25);

            SetHits(1250);

            SetDamage(20, 40);

            SetSkill(SkillName.Wrestling, 100);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 150;

			Fame = 3500;
			Karma = -3500;

		}

    public override bool OnBeforeDeath()
    {
        return base.OnBeforeDeath();
    }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }

        public override void SetUniqueAI()
        {           
            UniqueCreatureDifficultyScalar = 1.16;
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

		public ValoriteElemental( Serial serial ) : base( serial )
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
