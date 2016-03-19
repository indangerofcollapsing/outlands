using System;
using System.Collections;
using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
	[CorpseName( "a solen queen corpse" )]
	public class BlackSolenQueen : BaseCreature
	{
		private bool m_BurstSac;
		public bool BurstSac{ get{ return m_BurstSac; } }

		[Constructable]
		public BlackSolenQueen() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a black solen queen";
			Body = 807;
			BaseSoundID = 959;
			Hue = 2051;

            SetStr(100);
            SetDex(50);
            SetInt(25);

            SetHits(500);

            SetDamage(12, 24);

            SetSkill(SkillName.Wrestling, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 50;

			Fame = 4500;
			Karma = -4500;
			PackItem( new ZoogiFungus( ( Utility.RandomDouble() > 0.05 )? 5 : 25 ) );
		}

        public override void SetUniqueAI()
        {
            UniqueCreatureDifficultyScalar = 1.05;
        }

		public override int GetAngerSound()
		{
			return 0x259;
		}

		public override int GetIdleSound()
		{
			return 0x259;
		}

		public override int GetAttackSound()
		{
			return 0x195;
		}

		public override int GetHurtSound()
		{
			return 0x250;
		}

		public override int GetDeathSound()
		{
			return 0x25B;
		}

		public override void OnDamage( int amount, Mobile from, bool willKill )
		{
			if ( !willKill )
			{
				if ( !BurstSac )
				{
					if ( Hits < 50 )
					{
						PublicOverheadMessage( MessageType.Regular, 0x3B2, true, "* The solen's acid sac is burst open! *" );
						m_BurstSac = true;
					}
				}

				else if ( from != null && from != this && InRange( from, 1 ) )
				{
					SpillAcid( from, 1 );
				}
			}

			base.OnDamage( amount, from, willKill );
		}

		public override bool OnBeforeDeath()
		{
			SpillAcid( 4 );

			return base.OnBeforeDeath();
		}

		public BlackSolenQueen( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 );
			writer.Write( m_BurstSac );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			
			switch( version )
			{
				case 1:
				{
					m_BurstSac = reader.ReadBool();
					break;
				}
			}	
		}
	}
}
