using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

using Server.Network;

namespace Server.Mobiles
{
	[CorpseName( "a myconid corpse" )]
	public class Myconid : BaseCreature
	{
        public DateTime m_NextMushroomExplosionAllowed;
        public TimeSpan NextMushroomExplosionDelay = TimeSpan.FromSeconds(Utility.RandomMinMax(13, 17));
        
        [Constructable]
		public Myconid () : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a myconid";
            Body = 312;
			BaseSoundID = 461;

			SetStr( 50 );
			SetDex( 25 );
			SetInt( 25 );

			SetHits( 400 );

			SetDamage( 10, 20 );

            SetSkill(SkillName.Wrestling, 75);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 25;

			Fame = 3500;
			Karma = -3500;			
		}

        public override void SetUniqueAI()
        {
            UniqueCreatureDifficultyScalar = 2.5;           
        }

        public override int PoisonResistance { get { return 5; } }

        public override void OnThink()
        {
            base.OnThink();
            
            if (Utility.RandomDouble() < 0.05 && DateTime.UtcNow > m_NextMushroomExplosionAllowed)
            {
                if (Combatant != null)
                {
                    SpecialAbilities.MushroomExplosionAbility(this, 4, 6, 0, 4, true);

                    m_NextMushroomExplosionAllowed = DateTime.UtcNow + NextMushroomExplosionDelay;
                }
            }            
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {            
            if (willKill)
                SpecialAbilities.MushroomExplosionAbility(this, 6, 8, 0, 4, false);            

            base.OnDamage(amount, from, willKill);
        }

        public override int GetAngerSound(){return 0x452;}
        public override int GetAttackSound(){return 0x453;}
        public override int GetHurtSound(){return 0x454;}
        public override int GetDeathSound(){return 0x455;}
        public override int GetIdleSound(){return 0x451;}

        public Myconid(Serial serial): base(serial)
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
