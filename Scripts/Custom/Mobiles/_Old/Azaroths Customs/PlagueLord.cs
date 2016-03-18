using System;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Targeting;
using Server.Spells;
using Server.Spells.Fifth;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a plague lord corpse" )]
	public class PlagueLord : BaseCreature
	{
		const int m_MaxPoisonFields = 6;
		TimeSpan m_PoisonFieldDuration;
		int m_CurrentPoisonFields = 0;
		
		TimeSpan m_CastDelay = TimeSpan.FromSeconds( 5.0 );
		DateTime m_NextCastTime = DateTime.UtcNow;
		List<DateTime> m_CastTimes = new List<DateTime>();
		
		Mobile m_Attacker = null;
		
		string[] m_SpawnSlimeText = { "* Burp *", "* Belch *", "* Grggrlrggrllgrgl *", "* Braaaapp *" };
		
		[Constructable]
		public PlagueLord() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Plague Lord Lkykjkgrgrblgr";
			
			Body = 775;
			Hue = 472;

			SetStr( 302, 500 );
			SetDex( 80 );
			SetInt( 16, 20 );

			SetHits( 318, 404 );

			SetDamage( 20, 24 );

			SetDamageType( ResistanceType.Physical, 60 );
			SetDamageType( ResistanceType.Poison, 40 );

			SetResistance( ResistanceType.Physical, 45, 55 );
			SetResistance( ResistanceType.Fire, 40, 50 );
			SetResistance( ResistanceType.Cold, 25, 35 );
			SetResistance( ResistanceType.Poison, 65, 75 );
			SetResistance( ResistanceType.Energy, 25, 35 );

			SetSkill( SkillName.Magery, 100.0 );
			SetSkill( SkillName.MagicResist, 35.0 );
			SetSkill( SkillName.Tactics, 100.0 );
			SetSkill( SkillName.Wrestling, 100.0 );

			Fame = 13000;
			Karma = -13000;

			VirtualArmor = 30;
			
			m_PoisonFieldDuration = TimeSpan.FromSeconds( 3.0 + ( this.Skills[SkillName.Magery].Value * 0.4 ) );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Gems, Utility.Random( 1, 3 ) );
		}

		public override bool AlwaysMurderer{ get{ return true; } }
	
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }

		public override void OnDamagedBySpell( Mobile caster )
		{
			if ( this.Map != null && caster != this )
			{
				BaseCreature spawn = new SlimeSpawn();

				spawn.Team = this.Team;
				spawn.Combatant = caster;
				spawn.MoveToWorld( this.Location, this.Map );

				Say( m_SpawnSlimeText[Utility.Random( m_SpawnSlimeText.Length )] );
				m_Attacker = caster;
			}

			base.OnDamagedBySpell( caster );
		}

		public override void OnGotMeleeAttack( Mobile attacker )
		{
			if ( this.Map != null && attacker != this )
			{
				BaseCreature spawn = new SlimeSpawn();

				spawn.Team = this.Team;
				spawn.Combatant = attacker;
				spawn.MoveToWorld( this.Location, this.Map );

				Say( m_SpawnSlimeText[Utility.Random( m_SpawnSlimeText.Length )] );
				m_Attacker = attacker;
			}

			base.OnGotMeleeAttack( attacker );
		}

		public override void OnThink()
		{
			// Ready to cast another spell.
			if (m_NextCastTime < DateTime.UtcNow)
			{
				// Check all the poison fields we've casted to see if any have expired.
				foreach ( DateTime time in m_CastTimes )
				{
					if (time < DateTime.UtcNow)
					{
						--m_CurrentPoisonFields;
					}
				}
				
				if ( m_Attacker != null && m_CurrentPoisonFields < m_MaxPoisonFields )
				{
					// Taunt the attacker who died and reset our attack target.
					if ( !m_Attacker.Alive )
					{
						Say( "Rjllzj lljff aalgg!" );
						Effects.PlaySound( this, this.Map, 0x1BF);
						m_Attacker = null;
						return;
					}
					
					CastPoisonField();
					++m_CurrentPoisonFields;
					m_NextCastTime = DateTime.UtcNow + m_CastDelay;
					m_CastTimes.Add(DateTime.UtcNow + m_PoisonFieldDuration);
				}
			}
		}

		public void CastPoisonField()
		{
			IPoint3D p = new Point3D( m_Attacker.Location );
			
			if ( !CanSee( p ) )
			{
				return;
			}
			else
			{
				SpellHelper.Turn( this, p );

				SpellHelper.GetSurfaceTop( ref p );

				int dx = Location.X - p.X;
				int dy = Location.Y - p.Y;
				int rx = (dx - dy) * 44;
				int ry = (dx + dy) * 44;

				bool eastToWest;

				if ( rx >= 0 && ry >= 0 )
				{
					eastToWest = false;
				}
				else if ( rx >= 0 )
				{
					eastToWest = true;
				}
				else if ( ry >= 0 )
				{
					eastToWest = true;
				}
				else
				{
					eastToWest = false;
				}

				Effects.PlaySound( p, Map, 0x20B );

				int itemID = eastToWest ? 0x3915 : 0x3922;

                //Changed Duration to IPY timers
				TimeSpan duration = TimeSpan.FromSeconds( 3.0 + (Skills[SkillName.Magery].Value * 0.4) );

				for ( int i = -2; i <= 2; ++i )
				{
					Point3D loc = new Point3D( eastToWest ? p.X + i : p.X, eastToWest ? p.Y : p.Y + i, p.Z );

					new InternalItem( itemID, loc, this, Map, duration, i );
				}

				Say( "* The beast hurls its poisonous flesh *" );
			}
		}
		
		[DispellableField]
		private class InternalItem : Item
		{
			private Timer m_Timer;
			private DateTime m_End;
			private Mobile m_Caster;

			public override bool BlocksFit{ get{ return true; } }

			public InternalItem( int itemID, Point3D loc, Mobile caster, Map map, TimeSpan duration, int val ) : base( itemID )
			{
				bool canFit = SpellHelper.AdjustField( ref loc, map, 12, false );

				Visible = false;
				Movable = false;
				Light = LightType.Circle300;

				MoveToWorld( loc, map );

				m_Caster = caster;

				m_End = DateTime.UtcNow + duration;

				m_Timer = new InternalTimer( this, TimeSpan.FromSeconds( Math.Abs( val ) * 0.2 ), caster.InLOS( this ), canFit );
				m_Timer.Start();
			}

			public override void OnAfterDelete()
			{
				base.OnAfterDelete();

				if ( m_Timer != null )
					m_Timer.Stop();
			}

			public InternalItem( Serial serial ) : base( serial )
			{
			}

			public override void Serialize( GenericWriter writer )
			{
				base.Serialize( writer );

				writer.Write( (int) 1 ); // version

				writer.Write( m_Caster );
				writer.WriteDeltaTime( m_End );
			}

			public override void Deserialize( GenericReader reader )
			{
				base.Deserialize( reader );

				int version = reader.ReadInt();

				switch ( version )
				{
					case 1:
					{
						m_Caster = reader.ReadMobile();

						goto case 0;
					}
					case 0:
					{
						m_End = reader.ReadDeltaTime();

						m_Timer = new InternalTimer( this, TimeSpan.Zero, true, true );
						m_Timer.Start();

						break;
					}
				}
			}

			public void ApplyPoisonTo( Mobile m )
			{
				// Don't attack slime spawns.
				if ( m_Caster == null || m is SlimeSpawn || m is PlagueLord )
					return;

				Poison p = Poison.Deadly;
				m.ApplyPoison( m_Caster, p );
			}

			public override bool OnMoveOver( Mobile m )
			{
				// Don't harm us!
				if ( m is SlimeSpawn || m is PlagueLord )
					return false;
				
				if ( Visible && m_Caster != null && SpellHelper.ValidIndirectTarget( m_Caster, m ) && m_Caster.CanBeHarmful( m, false ) )
				{
					m_Caster.DoHarmful( m );

					ApplyPoisonTo( m );
					m.PlaySound( 0x474 );
				}

				return true;
			}

			private class InternalTimer : Timer
			{
				private InternalItem m_Item;
				private bool m_InLOS, m_CanFit;

				private static Queue m_Queue = new Queue();

				public InternalTimer( InternalItem item, TimeSpan delay, bool inLOS, bool canFit ) : base( delay, TimeSpan.FromSeconds( 1.5 ) )
				{
					m_Item = item;
					m_InLOS = inLOS;
					m_CanFit = canFit;

					Priority = TimerPriority.FiftyMS;
				}

				protected override void OnTick()
				{
					if ( m_Item.Deleted )
						return;

					if ( !m_Item.Visible )
					{
						if ( m_InLOS && m_CanFit )
							m_Item.Visible = true;
						else
							m_Item.Delete();

						if ( !m_Item.Deleted )
						{
							m_Item.ProcessDelta();
							Effects.SendLocationParticles( EffectItem.Create( m_Item.Location, m_Item.Map, EffectItem.DefaultDuration ), 0x376A, 9, 10, 5040 );
						}
					}
					else if (DateTime.UtcNow > m_Item.m_End)
					{
						m_Item.Delete();
						Stop();
					}
					else
					{
						Map map = m_Item.Map;
						Mobile caster = m_Item.m_Caster;

						if ( map != null && caster != null )
						{
							bool eastToWest = ( m_Item.ItemID == 0x3915 );
							IPooledEnumerable eable = map.GetMobilesInBounds( new Rectangle2D( m_Item.X - (eastToWest ? 0 : 1), m_Item.Y - (eastToWest ? 1 : 0), (eastToWest ? 1 : 2), (eastToWest ? 2 : 1) ) );

							foreach ( Mobile m in eable )
							{
								if ( (m.Z + 16) > m_Item.Z && (m_Item.Z + 12) > m.Z && SpellHelper.ValidIndirectTarget( caster, m ) && caster.CanBeHarmful( m, false ) )
									m_Queue.Enqueue( m );
							}

							eable.Free();

							while ( m_Queue.Count > 0 )
							{
								Mobile m = (Mobile)m_Queue.Dequeue();

								// Don't harm us!
								if ( m is SlimeSpawn || m is PlagueLord )
								{
									continue;
								}
								else
								{
									caster.DoHarmful( m );
								}

								m_Item.ApplyPoisonTo( m );
								m.PlaySound( 0x474 );
							}
						}
					}
				}
			}
		}

		public PlagueLord( Serial serial ) : base( serial )
		{
		}

		public override int GetIdleSound()
		{
			return 0x1BF;
		}

		public override int GetAttackSound()
		{
			return 0x1C0;
		}

		public override int GetHurtSound()
		{
			return 0x1C1;
		}

		public override int GetDeathSound()
		{
			return 0x1C2;
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