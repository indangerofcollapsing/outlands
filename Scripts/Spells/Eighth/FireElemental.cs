using System;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using Server.Items;
using Server.Custom;

namespace Server.Spells.Eighth
{
	public class FireElementalSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Fire Elemental", "Kal Vas Xen Flam",
				269,
				9050,
				false,
				Reagent.Bloodmoss,
				Reagent.MandrakeRoot,
				Reagent.SpidersSilk,
				Reagent.SulfurousAsh
			);

		public override SpellCircle Circle { get { return SpellCircle.Eighth; } }


		public override TimeSpan GetCastDelay()
		{
		    return TimeSpan.FromSeconds( 4.5 );
		}


		public FireElementalSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override bool CheckCast()
		{
			if ( !base.CheckCast() )
				return false;

			if ( (Caster.Followers + 2) > Caster.FollowersMax )
			{
				Caster.SendLocalizedMessage( 1049645 ); // You have too many followers to summon that creature.
				return false;
			}

			return true;
		}

		public override void OnCast()
		{
            if (CheckSequence())
            {
                bool enhancedSpellcast = SpellHelper.IsEnhancedSpell(Caster, null, EnhancedSpellbookType.Summoner, false, true);

                BaseCreature summon = new SummonedFireElemental();

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

                int spellHue = PlayerEnhancementPersistance.GetSpellHueFor(Caster, HueableSpell.FireElemental);

                summon.Hue = spellHue;

                SpellHelper.Summon(summon, Caster, 0x217, TimeSpan.FromSeconds(duration), false, false);
            }

			FinishSequence();
		}
	}
}