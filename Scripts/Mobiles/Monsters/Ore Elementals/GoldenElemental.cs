using System;
using Server;
using Server.Items;


namespace Server.Mobiles
{
	[CorpseName( "a golden elemental corpse" )]
	public class GoldenElemental : BaseCreature
	{
        public DateTime m_NextReflectAllowed;

        public static double oreElementalLevel = 5;
        public static double oreElementalMaxLevel = 9;

        public double NextReflectDelayMin = 1.0 + ((oreElementalMaxLevel - oreElementalLevel) * .25);
        public double NextReflectDelayMax = 2.0 + ((oreElementalMaxLevel - oreElementalLevel) * .5);

		[Constructable]
		public GoldenElemental() : this( 2 )
		{
		}

		[Constructable]
		public GoldenElemental( int oreAmount ) : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a golden elemental";
			Body = 166;
			BaseSoundID = 268;

            SetStr(100);
            SetDex(25);
            SetInt(25);

            SetHits(600);

            SetDamage(20, 34);

            SetSkill(SkillName.Wrestling, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 150;

			Fame = 3500;
			Karma = -3500;	

		}

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);           
        }

        public override bool OnBeforeDeath()
        {
            return base.OnBeforeDeath();
        }

        public override void SetUniqueAI()
        {            
            UniqueCreatureDifficultyScalar = 1.1;
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

		public GoldenElemental( Serial serial ) : base( serial )
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
