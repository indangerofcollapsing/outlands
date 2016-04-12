using System;
using System.Collections;
using Server.Items;
using Server.Targeting;


namespace Server.Mobiles
{
	[CorpseName( "a myconid tallstalk corpse" )]
	public class MyconidTallstalk : BaseCreature
	{
        public DateTime m_NextMushroomExplosionAllowed;
        public TimeSpan NextMushroomExplosionDelay = TimeSpan.FromSeconds(Utility.RandomMinMax(13, 17));
        
        [Constructable]
		public MyconidTallstalk () : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
            Name = "a myconid tallstalk";
            Body = 285;
			BaseSoundID = 461;

			SetStr( 75 );
			SetDex( 25 );
			SetInt( 25 );

			SetHits( 600 );

			SetDamage( 15, 25 );

            SetSkill(SkillName.Wrestling, 80);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 75);

            VirtualArmor = 75;

			Fame = 3500;
			Karma = -3500;
		}

        public override void SetUniqueAI()
        {
            UniqueCreatureDifficultyScalar = 2.1;
        }

        public override int PoisonResistance { get { return 5; } }

        public override void OnThink()
        {
            base.OnThink();
            
            if (Utility.RandomDouble() < 0.05 && DateTime.UtcNow > m_NextMushroomExplosionAllowed)
            {
                if (Combatant != null)
                {
                    SpecialAbilities.MushroomExplosionAbility(this, 6, 8, 0, 4, true);

                    m_NextMushroomExplosionAllowed = DateTime.UtcNow + NextMushroomExplosionDelay;
                }
            }            
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {            
            if (willKill)
                SpecialAbilities.MushroomExplosionAbility(this, 8, 10, 0, 4, false);            

            base.OnDamage(amount, from, willKill);
        }

        public override int GetAngerSound(){return 0x452;}
        public override int GetAttackSound(){return 0x453;}
        public override int GetHurtSound(){return 0x454;}
        public override int GetDeathSound(){return 0x455;}
        public override int GetIdleSound(){return 0x451;}	

        public MyconidTallstalk(Serial serial): base(serial)
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
