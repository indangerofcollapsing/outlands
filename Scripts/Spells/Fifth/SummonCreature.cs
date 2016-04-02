using System;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using Server.Items;
using Server.Custom;

namespace Server.Spells.Fifth
{
	public class SummonCreatureSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Summon Creature", "Kal Xen",
				266,
				9040,
				Reagent.Bloodmoss,
				Reagent.MandrakeRoot,
				Reagent.SpidersSilk
			);

		public override SpellCircle Circle { get { return SpellCircle.Fifth; } }

		public SummonCreatureSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		// NOTE: Creature list based on 1hr of summon/release on OSI.

		private static Type[] m_Types = new Type[]
			{
				typeof( PolarBear ),
				typeof( GrizzlyBear ),
				typeof( BlackBear ),				
				typeof( Walrus ),
				typeof( Chicken ),
				typeof( Scorpion ),				
				typeof( Alligator ),
				typeof( GreyWolf ),
				typeof( Slime ),
				typeof( Eagle ),
				typeof( Gorilla ),
				typeof( SnowLeopard ),
				typeof( Pig ),
				typeof( Hind ),
				typeof( Rabbit )
			};

		public override void OnCast()
		{
			if ( CheckSequence() )
			{
				try
				{
                    bool enhancedSpellcast = SpellHelper.IsEnhancedSpell(Caster, null, EnhancedSpellbookType.Summoner, false, true);

                    BaseCreature summon = (BaseCreature)Activator.CreateInstance(m_Types[Utility.Random(m_Types.Length)]);

                    summon.StoreBaseSummonValues();

                    double duration = 4.0 * Caster.Skills[SkillName.Magery].Value;

                    if (enhancedSpellcast)
                    {
                        duration *= SpellHelper.EnhancedSummonDurationMultiplier;

                        summon.DamageMin = (int)((double)summon.DamageMin * SpellHelper.EnhancedSummonDamageMultiplier);
                        summon.DamageMax = (int)((double)summon.DamageMax * SpellHelper.EnhancedSummonDamageMultiplier);

                        summon.SetHitsMax((int)((double)summon.HitsMax * SpellHelper.EnhancedSummonHitPointsMultiplier));
                        summon.Hits = summon.HitsMax;
                    }

                    summon.SetDispelResistance(Caster, enhancedSpellcast, 0);

                    int spellHue = PlayerEnhancementPersistance.GetSpellHueFor(Caster, HueableSpell.SummonCreature);

                    summon.Hue = spellHue;

                    SpellHelper.Summon(summon, Caster, 0x217, TimeSpan.FromSeconds(duration), false, false);                  
                }

				catch
				{
				}
			}

			FinishSequence();
		}

		public override TimeSpan CastDelayBase
		{  
            //TODO: Check this
			get{ return base.CastDelayBase + TimeSpan.FromSeconds( 28 ); }
		}

		public override double CastDelayFastScalar
		{
			get{ return 5; }
		}

		public override TimeSpan GetCastDelay()
		{
			if ( Core.AOS )
				return TimeSpan.FromTicks( base.GetCastDelay().Ticks * 5 );

			return base.GetCastDelay() + TimeSpan.FromSeconds( 6.0 );
		}
	}
}