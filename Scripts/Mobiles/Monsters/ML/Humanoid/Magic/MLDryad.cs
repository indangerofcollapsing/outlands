using System;
using Server.Engines.Plants;

namespace Server.Mobiles
{
	[CorpseName( "a dryad's corpse" )]
	public class MLDryad : BaseCreature
	{
		public override bool InitialInnocent { get { return true; } }

		public override OppositionGroup OppositionGroup
		{
			get { return OppositionGroup.FeyAndUndead; }
		}

		[Constructable]
		public MLDryad() : base( AIType.AI_Mage, FightMode.Evil, 10, 1, 0.2, 0.4 )
		{
			Name = "a dryad";
			Body = 266;
			BaseSoundID = 0x57B;

            SetStr(25);
            SetDex(75);
            SetInt(100);

            SetHits(200);
            SetMana(2000);

            SetDamage(4, 8);

            SetSkill(SkillName.Wrestling, 100);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 75);
            SetSkill(SkillName.EvalInt, 75);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.MagicResist, 125);

            VirtualArmor = 25;

			Fame = 5000;
			Karma = 5000;

			if ( Core.ML && Utility.RandomDouble() < .60 )
				PackItem( Seed.RandomPeculiarSeed( 1 ) );
		}
        
		public override int Meat { get { return 1; } }

		public override void OnThink()
		{
			base.OnThink();

			AreaPeace();
			AreaUndress();
		}

		#region Area Peace
		private DateTime m_NextPeace;

		public void AreaPeace()
		{
			if ( Combatant == null || Deleted || !Alive || m_NextPeace > DateTime.UtcNow || 0.1 < Utility.RandomDouble() )
				return;

			TimeSpan duration = TimeSpan.FromSeconds( Utility.RandomMinMax( 20, 80 ) );

			foreach ( Mobile m in GetMobilesInRange( RangePerception ) )
			{
				PlayerMobile p = m as PlayerMobile;

				if ( IsValidTarget( p ) )
				{
					p.PeacedUntil = DateTime.UtcNow + duration;
					p.SendLocalizedMessage( 1072065 ); // You gaze upon the dryad's beauty, and forget to continue battling!
					p.FixedParticles( 0x376A, 1, 20, 0x7F5, EffectLayer.Waist );
					p.Combatant = null;
				}
			}

			m_NextPeace = DateTime.UtcNow + TimeSpan.FromSeconds( 10 );
			PlaySound( 0x1D3 );
		}

		public bool IsValidTarget( PlayerMobile m )
		{
			if ( m != null && m.PeacedUntil < DateTime.UtcNow && !m.Hidden && m.AccessLevel == AccessLevel.Player && CanBeHarmful( m ) )
				return true;

			return false;
		}
		#endregion

		#region Undress
		private DateTime m_NextUndress;

		public void AreaUndress()
		{
			if ( Combatant == null || Deleted || !Alive || m_NextUndress > DateTime.UtcNow || 0.005 < Utility.RandomDouble() )
				return;

			foreach ( Mobile m in GetMobilesInRange( RangePerception ) )
			{
				if ( m != null && m.Player && !m.Female && !m.Hidden && m.AccessLevel == AccessLevel.Player && CanBeHarmful( m ) )
				{
					UndressItem( m, Layer.OuterTorso );
					UndressItem( m, Layer.InnerTorso );
					UndressItem( m, Layer.MiddleTorso );
					UndressItem( m, Layer.Pants );
					UndressItem( m, Layer.Shirt );

					m.SendLocalizedMessage( 1072197 ); // The dryad's beauty makes your blood race. Your clothing is too confining.
				}
			}

			m_NextUndress = DateTime.UtcNow + TimeSpan.FromMinutes( 1 );
		}

		public void UndressItem( Mobile m, Layer layer )
		{
			Item item = m.FindItemOnLayer( layer );

			if ( item != null && item.Movable )
				m.PlaceInBackpack( item );
		}
		#endregion

		public MLDryad( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); //version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}
