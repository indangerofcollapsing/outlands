using System;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;
using Server.Spells;
using Server.Multis;

namespace Server
{
	public class PoisonImpl : Poison
	{
		[CallPriority( 10 )]
		public static void Configure()
		{
            Register(new PoisonImpl("Lesser", 0, 1, 5, 2.500, 3.5, 10.0, 10, 2)); //30 DPM or .5 DPS
            Register(new PoisonImpl("Regular", 1, 2, 10, 5.000, 3.5, 8.0, 10, 2)); //75 DPM or 1.25 DPS
            Register(new PoisonImpl("Greater", 2, 5, 15, 10.000, 3.5, 7.0, 10, 2)); //128 DPM or 2.133 DPS
            Register(new PoisonImpl("Deadly", 3, 10, 20, 15.000, 3.5, 6.0, 10, 2)); //200 DPM or 3.333 DPS
            Register(new PoisonImpl("Lethal", 4, 15, 25, 20.000, 3.5, 5.0, 10, 2));	//300 DPM or 5 DPS
		
            /*            
            Register(new PoisonImpl("Lesser", 0, 4, 15, 2.500, 3.5, 10.0, 5, 2));
            Register(new PoisonImpl("Regular", 1, 5, 26, 3.125, 3.5, 9.0, 10, 2));
            Register(new PoisonImpl("Greater", 2, 6, 26, 6.250, 3.5, 8.0, 10, 2));
            Register(new PoisonImpl("Deadly", 3, 7, 26, 12.500, 3.5, 6.5, 10, 2));
            Register(new PoisonImpl("Lethal", 4, 9, 26, 25.000, 3.5, 5.0, 10, 2));	
            */
		}

		public static Poison IncreaseLevel( Poison oldPoison )
		{
			Poison newPoison = ( oldPoison == null ? null : GetPoison( oldPoison.Level + 1 ) );

			return ( newPoison == null ? oldPoison : newPoison );
		}

        public static Poison DecreaseLevel(Poison oldPoison)
        {
            Poison newPoison = (oldPoison == null ? null : GetPoison(oldPoison.Level - 1));

            return (newPoison == null ? oldPoison : newPoison);
        }

		// Info
		private string m_Name;
		private int m_Level;

		// Damage
		private int m_Minimum, m_Maximum;
		private double m_Scalar;

		// Timers
		private TimeSpan m_Delay;
		private TimeSpan m_Interval;
		private int m_Count, m_MessageInterval;

		public PoisonImpl( string name, int level, int min, int max, double percent, double delay, double interval, int count, int messageInterval )
		{
			m_Name = name;
			m_Level = level;
			m_Minimum = min;
			m_Maximum = max;
			m_Scalar = percent * 0.01;
			m_Delay = TimeSpan.FromSeconds( delay );
			m_Interval = TimeSpan.FromSeconds( interval );
			m_Count = count;
			m_MessageInterval = messageInterval;
		}

		public override string Name{ get{ return m_Name; } }
		public override int Level{ get{ return m_Level; } }

		public class PoisonTimer : Timer
		{
			private PoisonImpl m_Poison;
			private Mobile m_Mobile;
			private Mobile m_From;
			private int m_LastDamage;
			private int m_Index;

			public Mobile From{ get{ return m_From; } set{ m_From = value; } }

			public PoisonTimer( Mobile m, PoisonImpl p ) : base( p.m_Delay, p.m_Interval )
			{
				m_From = m;
				m_Mobile = m;
				m_Poison = p;
			}

			protected override void OnTick()
			{
				if ( m_Index++ == m_Poison.m_Count )
				{
					m_Mobile.SendLocalizedMessage( 502136 ); // The poison seems to have worn off.
					m_Mobile.Poison = null;

					Stop();
					return;
				}

				double damage;

				damage = 1 + ((double)m_Mobile.HitsMax * m_Poison.m_Scalar);

                double minimum = m_Poison.m_Minimum;
                double maximum = m_Poison.m_Maximum;

                PlayerMobile pm_From = m_From as PlayerMobile;
                BaseCreature bc_From = m_From as BaseCreature;

                PlayerMobile pm_Target = m_Mobile as PlayerMobile;
                BaseCreature bc_Target = m_Mobile as BaseCreature;

                if (bc_Target != null)                
                    bc_Target.m_TakenDamageFromPoison = true;                

                if (pm_From != null && bc_Target != null)
                {
                    double poisonDamageScalar = 1.0;

                    DungeonArmor.PlayerDungeonArmorProfile poisonerDungeonArmor = new DungeonArmor.PlayerDungeonArmorProfile(pm_From, null);

                    if (poisonerDungeonArmor.MatchingSet && !poisonerDungeonArmor.InPlayerCombat)
                        poisonDamageScalar = poisonerDungeonArmor.DungeonArmorDetail.PoisonDamageInflictedScalar;

                    minimum = (double)minimum * 1.5 * poisonDamageScalar;
                    maximum = (double)maximum * 1.5 * poisonDamageScalar;
                }

                if (bc_From != null && pm_Target != null)
                {
                    double poisonDamageScalar = 1.0;

                    DungeonArmor.PlayerDungeonArmorProfile defenderDungeonArmor = new DungeonArmor.PlayerDungeonArmorProfile(pm_Target, null);

                    if (defenderDungeonArmor.MatchingSet && !defenderDungeonArmor.InPlayerCombat)                    
                        poisonDamageScalar = defenderDungeonArmor.DungeonArmorDetail.PoisonDamageReceivedScalar;

                    if (bc_From.Controlled && bc_From.ControlMaster is PlayerMobile)
                    {
                        minimum = (double)minimum * bc_From.PvPAbilityDamageScalar;
                        maximum = (double)maximum * bc_From.PvPAbilityDamageScalar;
                    }                    
                                        
                    minimum *= defenderDungeonArmor.DungeonArmorDetail.PoisonDamageReceivedScalar;
                    maximum *= defenderDungeonArmor.DungeonArmorDetail.PoisonDamageReceivedScalar;                    
                }

                if (damage < minimum)
                    damage = minimum;

                else if (damage > maximum)
                    damage = maximum;

                int finalDamage = (int)(Math.Round(damage));

                if (finalDamage < 1)
                    finalDamage = 1;

                m_LastDamage = finalDamage;				

				if ( m_From != null )
					m_From.DoHarmful( m_Mobile, true );				

                int adjustedDamageDisplayed = finalDamage;
                int discordancePenalty = 0;                
                
                if (bc_Target != null)
                {    
                    //Discordance                    
                    adjustedDamageDisplayed = (int)((double)adjustedDamageDisplayed * (1 + bc_Target.DiscordEffect)); 

                    //Ship Combat
                    if (BaseBoat.UseShipBasedDamageModifer(m_From, bc_Target))
                        adjustedDamageDisplayed = (int)((double)adjustedDamageDisplayed * BaseBoat.shipBasedDamageToCreatureScalar); 
                }

                if (pm_Target != null)
                {
                    //Ship Combat
                    if (BaseBoat.UseShipBasedDamageModifer(m_From, pm_Target))
                        adjustedDamageDisplayed = (int)((double)adjustedDamageDisplayed * BaseBoat.shipBasedDamageToPlayerScalar); 
                }

                //Display Player Poison Damage
                if (pm_From != null && m_From.GetDistanceToSqrt(m_Mobile) <= 20)                
                    DamageTracker.RecordDamage(pm_From, pm_From, m_Mobile, DamageTracker.DamageType.PoisonDamage, adjustedDamageDisplayed);                    
                
                //Display Follower Poison Damage
                if (bc_From != null && m_From.GetDistanceToSqrt(m_Mobile) <= 20)
                {
                    if (bc_From.Controlled && bc_From.ControlMaster is PlayerMobile)
                    {
                        PlayerMobile playerOwner = bc_From.ControlMaster as PlayerMobile;

                        DamageTracker.RecordDamage(playerOwner, bc_From, m_Mobile, DamageTracker.DamageType.FollowerDamage, adjustedDamageDisplayed);                        
                    }
                }

                //Display Provoked Creature Poison Damage
                if (bc_From != null && m_From.GetDistanceToSqrt(m_Mobile) <= 20)
                {
                    if (bc_From.BardProvoked && bc_From.BardMaster is PlayerMobile)
                    {
                        PlayerMobile playerBard = bc_From.BardMaster as PlayerMobile;

                        DamageTracker.RecordDamage(playerBard, bc_From, m_Mobile, DamageTracker.DamageType.ProvocationDamage, adjustedDamageDisplayed);
                    }
                }

                AOS.Damage(m_Mobile, m_From, finalDamage, 0, 0, 0, 100, 0);

				if ( (m_Index % m_Poison.m_MessageInterval) == 0 )
					m_Mobile.OnPoisoned( m_From, m_Poison, m_Poison );
			}
		}

		public override Timer ConstructTimer( Mobile m )
		{
			return new PoisonTimer( m, this );
		}
	}
}