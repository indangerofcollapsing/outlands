using System;
using Server.Items;
using Server.Network;

namespace Server.Items
{
	public class Fists : BaseMeleeWeapon
	{
		public static void Initialize()
		{
			Mobile.DefaultWeapon = new Fists();

			EventSink.DisarmRequest += new DisarmRequestEventHandler( EventSink_DisarmRequest );
			EventSink.StunRequest += new StunRequestEventHandler( EventSink_StunRequest );
		}

		public override int BaseMinDamage { get { return 1; } }
        public override int BaseMaxDamage { get { return 8; } }
        public override int BaseSpeed { get { return 30; } }
		
        public override int BaseHitSound { get { return -1; } }
		public override int BaseMissSound{ get{ return -1; } }

		public override SkillName BaseSkill{ get{ return SkillName.Wrestling; } }
		public override WeaponType BaseType{ get{ return WeaponType.Fists; } }
		public override WeaponAnimation BaseAnimation { get{ return WeaponAnimation.Wrestle; } }

		public Fists() : base( 0 )
		{
			Visible = false;
			Movable = false;
		}

		public Fists( Serial serial ) : base( serial )
		{
		}

		public override double GetDefendSkillValue( Mobile attacker, Mobile defender )
		{
			return defender.Skills[SkillName.Wrestling].Value;
		}

		private void CheckPreAOSMoves( Mobile attacker, Mobile defender )
		{
			if ( attacker.StunReady )
			{
				if ( attacker.CanBeginAction( typeof( Fists ) ) )
				{
					if ( attacker.Skills[SkillName.Anatomy].Value >= 80.0 && attacker.Skills[SkillName.Wrestling].Value >= 80.0 )
					{
						if ( attacker.Stam >= 15 )
						{
							attacker.Stam -= 15;

							if ( CheckMove( attacker, SkillName.Anatomy ) )
							{
								StartMoveDelay( attacker );

								attacker.StunReady = false;

								attacker.SendLocalizedMessage( 1004013 ); // You successfully stun your opponent!
								defender.SendLocalizedMessage( 1004014 ); // You have been stunned!

								defender.Freeze( TimeSpan.FromSeconds( 4.0 ) );
							}
							else
							{
								attacker.SendLocalizedMessage( 1004010 ); // You failed in your attempt to stun.
								defender.SendLocalizedMessage( 1004011 ); // Your opponent tried to stun you and failed.
							}
						}
						else
						{
							attacker.SendLocalizedMessage( 1004009 ); // You are too fatigued to attempt anything.
						}
					}
					else
					{
						attacker.SendLocalizedMessage( 1004008 ); // You are not skilled enough to stun your opponent.
						attacker.StunReady = false;
					}
				}
			}
			else if ( attacker.DisarmReady )
			{
				if ( attacker.CanBeginAction( typeof( Fists ) ) )
				{
					if ( defender.Player || defender.Body.IsHuman )
					{
						if ( attacker.Skills[SkillName.ArmsLore].Value >= 80.0 && attacker.Skills[SkillName.Wrestling].Value >= 80.0 )
						{
							if ( attacker.Stam >= 15 )
							{
								Item toDisarm = defender.FindItemOnLayer( Layer.OneHanded );

								if ( toDisarm == null || !toDisarm.Movable )
									toDisarm = defender.FindItemOnLayer( Layer.TwoHanded );

								Container pack = defender.Backpack;

								if ( pack == null || toDisarm == null || !toDisarm.Movable )
								{
									attacker.SendLocalizedMessage( 1004001 ); // You cannot disarm your opponent.
								}
								else if ( CheckMove( attacker, SkillName.ArmsLore ) )
								{
									StartMoveDelay( attacker );

									attacker.Stam -= 15;
									attacker.DisarmReady = false;

									attacker.SendLocalizedMessage( 1004006 ); // You successfully disarm your opponent!
									defender.SendLocalizedMessage( 1004007 ); // You have been disarmed!

									pack.DropItem( toDisarm );
								}
								else
								{
									attacker.Stam -= 15;

									attacker.SendLocalizedMessage( 1004004 ); // You failed in your attempt to disarm.
									defender.SendLocalizedMessage( 1004005 ); // Your opponent tried to disarm you but failed.
								}
							}
							else
							{
								attacker.SendLocalizedMessage( 1004003 ); // You are too fatigued to attempt anything.
							}
						}
						else
						{
							attacker.SendLocalizedMessage( 1004002 ); // You are not skilled enough to disarm your opponent.
							attacker.DisarmReady = false;
						}
					}
					else
					{
						attacker.SendLocalizedMessage( 1004001 ); // You cannot disarm your opponent.
					}
				}
			}
		}

		public override TimeSpan OnSwing( Mobile attacker, Mobile defender )
		{
			if ( !Core.AOS )
				CheckPreAOSMoves( attacker, defender );

			return base.OnSwing( attacker, defender );
		}

		public override void OnMiss( Mobile attacker, Mobile defender )
		{
            base.OnMiss(attacker, defender);
		}

        public override WeaponAnimation GetAnimation()
        {            
            WeaponAnimation animation = WeaponAnimation.Wrestle;
                       
            switch (Utility.Random(7))
            {
                case 0:
                    animation = WeaponAnimation.Wrestle;
                    break;

                case 1:
                    animation = WeaponAnimation.Wrestle;
                    break;

                case 2:
                    animation = WeaponAnimation.Slash1H;
                    break;

                case 3:
                    animation = WeaponAnimation.Pierce1H;
                    break;

                case 4:
                    animation = WeaponAnimation.Bash1H;
                break;

                case 5:
                    animation = WeaponAnimation.Block;
                break;

                case 6:
                    animation = WeaponAnimation.Pierce2H;
                break;
            }            

            return animation;
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

			Delete();
		}

		/* Wrestling moves */

		private static bool CheckMove( Mobile m, SkillName other )
		{
			double wresValue = m.Skills[SkillName.Wrestling].Value;
			double scndValue = m.Skills[other].Value;

			/* 40% chance at 80, 80
			 * 50% chance at 100, 100
			 * 60% chance at 120, 120
			 */

			double chance = (wresValue + scndValue) / 400.0;

			return ( chance >= Utility.RandomDouble() );
		}

		private static bool HasFreeHands( Mobile m )
		{
			Item item = m.FindItemOnLayer( Layer.OneHanded );

			if ( item != null && !(item is Spellbook) )
				return false;

			return m.FindItemOnLayer( Layer.TwoHanded ) == null;
		}

		private static void EventSink_DisarmRequest( DisarmRequestEventArgs e )
		{
			if ( Core.AOS )
				return;

			Mobile m = e.Mobile;
			m.DisarmReady = false;
			
		}

		private static void EventSink_StunRequest( StunRequestEventArgs e )
		{
			if ( Core.AOS )
				return;

			Mobile m = e.Mobile;


			m.StunReady = false;
		}

		private class MoveDelayTimer : Timer
		{
			private Mobile m_Mobile;

			public MoveDelayTimer( Mobile m ) : base( TimeSpan.FromSeconds( 10.0 ) )
			{
				m_Mobile = m;

				Priority = TimerPriority.TwoFiftyMS;

				m_Mobile.BeginAction( typeof( Fists ) );
			}

			protected override void OnTick()
			{
				m_Mobile.EndAction( typeof( Fists ) );
			}
		}

		private static void StartMoveDelay( Mobile m )
		{
			new MoveDelayTimer( m ).Start();
		}
	}
}