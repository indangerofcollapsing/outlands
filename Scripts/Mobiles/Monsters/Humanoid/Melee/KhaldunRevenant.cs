using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	public class KhaldunRevenant : BaseCreature
	{
		private static Hashtable m_Table = new Hashtable();

		public static void Initialize()
		{
			EventSink.PlayerDeath += new PlayerDeathEventHandler( EventSink_PlayerDeath );
		}
 
		public static void EventSink_PlayerDeath( PlayerDeathEventArgs e )
		{
			Mobile m = e.Mobile;
			Mobile lastKiller = m.LastKiller;

			if ( lastKiller is BaseCreature )
				lastKiller = ((BaseCreature)lastKiller).GetMaster();

			if ( IsInsideKhaldun( m ) && IsInsideKhaldun( lastKiller ) && lastKiller.Player && !m_Table.Contains( lastKiller ) )
			{
				foreach ( AggressorInfo ai in m.Aggressors )
				{
					if ( ai.Attacker == lastKiller && ai.CanReportMurder )
					{
						SummonRevenant( m, lastKiller );
						break;
					}
				}
			}
		}

		public static void SummonRevenant( Mobile victim, Mobile killer )
		{
			KhaldunRevenant revenant = new KhaldunRevenant( killer );

			revenant.MoveToWorld( victim.Location, victim.Map );
			revenant.Combatant = killer;
			revenant.FixedParticles( 0, 0, 0, 0x13A7, EffectLayer.Waist );
			Effects.PlaySound( revenant.Location, revenant.Map, 0x29 );

			m_Table.Add( killer, null );
		}

		public static bool IsInsideKhaldun( Mobile from )
		{
			return from != null && from.Region != null && from.Region.IsPartOf( "Khaldun" );
		}

		private Mobile m_Target;
		private DateTime m_ExpireTime;

		public override bool DeleteCorpseOnDeath{ get{ return true; } }

		public override void DisplayPaperdollTo( Mobile to )
		{
		}

		public override Mobile ConstantFocus{ get{ return m_Target; } }
		public override bool AlwaysAttackable{ get{ return true; } }

		public KhaldunRevenant( Mobile target ) : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.18, 0.36 )
		{
			Name = "a revenant";
			Body = 0x3CA;
			Hue = 0x41CE;

			m_Target = target;
			m_ExpireTime = DateTime.UtcNow + TimeSpan.FromMinutes( 10.0 );

            SetStr(100);
            SetDex(50);
            SetInt(25);

            SetHits(500);

            SetDamage(20, 30);

            SetSkill(SkillName.Swords, 120);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 125);

            VirtualArmor = 25;            

			Fame = 0;
			Karma = 0;

			Halberd weapon = new Halberd();
			weapon.Hue = 0x41CE;
			weapon.Movable = false;

			AddItem( weapon );
		}

        public override bool BardImmune { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }

		public override int GetIdleSound()
		{
			return 0x1BF;
		}

		public override int GetAngerSound()
		{
			return 0x107;
		}

		public override int GetDeathSound()
		{
			return 0xFD;
		}		

		public override void OnThink()
		{
			if ( !m_Target.Alive || DateTime.UtcNow > m_ExpireTime )
			{
				Delete();
				return;
			}

			if ( AIObject != null )
				AIObject.Action = ActionType.Combat;

			base.OnThink();
		}

		public override bool OnBeforeDeath()
		{
			Effects.SendLocationEffect( Location, Map, 0x376A, 10, 1 );

			return true;
		}

		public override void OnDelete()
		{
			if ( m_Target != null )
				m_Table.Remove( m_Target );

			base.OnDelete();
		}

		public KhaldunRevenant( Serial serial ) : base( serial )
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

			Delete();
		}
	}
}