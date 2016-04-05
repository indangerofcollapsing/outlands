using System;
using Server;
using Server.Items;


namespace Server.Mobiles
{
	[CorpseName( "a sand vortex corpse" )]
	public class SandVortex : BaseCreature
	{
		[Constructable]
		public SandVortex() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a sand vortex";
			Body = 790;
			BaseSoundID = 263;

            SetStr(50);
            SetDex(75);
            SetInt(25);

            SetHits(150);

            SetDamage(6, 12);

            SetSkill(SkillName.Wrestling, 80);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            VirtualArmor = 25;

			Fame = 4500;
			Karma = -4500;
			
			PackItem( new Bone() );
		}

        public override void SetUniqueAI()
        {
            UniqueCreatureDifficultyScalar = 1.1;
        }

		public override void OnDeath( Container c )
		{			
            base.OnDeath( c );
		}

		private DateTime m_NextAttack;

		public override void OnActionCombat()
		{
			Mobile combatant = Combatant;

			if ( combatant == null || combatant.Deleted || combatant.Map != Map || !InRange( combatant, 12 ) || !CanBeHarmful( combatant ) || !InLOS( combatant ) )
				return;

			if ( DateTime.UtcNow >= m_NextAttack )
			{
				SandAttack( combatant );
				m_NextAttack = DateTime.UtcNow + TimeSpan.FromSeconds( 10.0 + (10.0 * Utility.RandomDouble()) );
			}
		}

		public void SandAttack( Mobile m )
		{
			DoHarmful( m );
			m.FixedParticles( 0x36B0, 10, 25, 9540, 2413, 0, EffectLayer.Waist );

			new InternalTimer( m, this ).Start();
		}

		private class InternalTimer : Timer
		{
			private Mobile m_Mobile, m_From;

			public InternalTimer( Mobile m, Mobile from ) : base( TimeSpan.FromSeconds( 1.0 ) )
			{
				m_Mobile = m;
				m_From = from;
				Priority = TimerPriority.TwoFiftyMS;
			}

			protected override void OnTick()
			{
				m_Mobile.PlaySound( 0x4CF );

				AOS.Damage( m_Mobile, m_From, Utility.RandomMinMax( 10, 20 ), 90, 10, 0, 0, 0 );
			}
		}

		public SandVortex( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}