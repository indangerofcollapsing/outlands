using System;
using Server;
using Server.Items;
using Server.Spells;
using Server.Mobiles;

namespace Server.Misc
{
	public class RegenRates
	{
		[CallPriority( 10 )]
		public static void Configure()
		{
			Mobile.DefaultHitsRate = TimeSpan.FromSeconds( 10.0 );
			Mobile.DefaultStamRate = TimeSpan.FromSeconds(  5.0 );
			Mobile.DefaultManaRate = TimeSpan.FromSeconds(  7.0 );

            Mobile.HitsRegenRateHandler = new RegenRateHandler(Mobile_HitsRegenRate);
            Mobile.StamRegenRateHandler = new RegenRateHandler(Mobile_StamRegenRate);
			Mobile.ManaRegenRateHandler = new RegenRateHandler(Mobile_ManaRegenRate);
		}

		private static void CheckBonusSkill( Mobile m, int cur, int max, SkillName skill )
		{
			if ( !m.Alive )
				return;

			double n = (double)cur / max;
			double v = Math.Sqrt( m.Skills[skill].Value * 0.005 );

			n *= (1.0 - v);
			n += v;

			m.CheckSkill( skill, n , 1.0);
		}

		private static bool CheckTransform( Mobile m, Type type )
		{
			return TransformationSpellHelper.UnderTransformation( m, type );
		}
        
		private static TimeSpan Mobile_HitsRegenRate( Mobile from )
		{            
            PlayerMobile player = from as PlayerMobile;

            if (player != null && from.Region is UOACZRegion)
            {
                double totalValue = 0;
                double regenRate = UOACZSystem.HitsRegen;

                player.GetSpecialAbilityEntryValue(SpecialAbilityEffect.Hardy, out totalValue);

                if (totalValue > 0)                
                    regenRate *= totalValue;                

                return TimeSpan.FromSeconds(regenRate);
            }              

            return Mobile.DefaultHitsRate;
		}

		private static TimeSpan Mobile_StamRegenRate( Mobile from )
		{            
            PlayerMobile player = from as PlayerMobile;

            if (player != null && from.Region is UOACZRegion)
            {
                double regenRate = UOACZSystem.StamRegen;

                return TimeSpan.FromSeconds(regenRate);
            }            

            return Mobile.DefaultStamRate;
		}

		private static TimeSpan Mobile_ManaRegenRate( Mobile from )
		{
            PlayerMobile player = from as PlayerMobile;

			if ( from.Skills == null )
				return Mobile.DefaultManaRate;

			if ( !from.Meditating )
				CheckBonusSkill( from, from.Mana, from.ManaMax, SkillName.Meditation );

			double rate;
			double armorPenalty = GetArmorOffset( from );

			double medPoints = (from.Int + from.Skills[SkillName.Meditation].Value) * 0.5;
			
			if ( medPoints <= 0 )
				rate = 7.0;

			else if ( medPoints <= 100 )
				rate = (7.0 - (239 * medPoints / 2400) + (19 * medPoints * medPoints / 48000)) * 0.85;

			else if ( medPoints < 120 )
				rate = 1.0;

			else
				rate = 0.75;

            if (!(from.Region is UOACZRegion))
			    rate *= armorPenalty;
            
            if (player != null && from.Region is UOACZRegion)
            {
                rate *= UOACZSystem.ManaRegenScalar;

                return TimeSpan.FromSeconds(rate);
            }            

			if ( from.Meditating )
				rate *= 0.55;

			if ( rate < 0.50 )
				rate = 0.50;

			else if ( rate > 7.0 )
				rate = 7.0;       

			return TimeSpan.FromSeconds( rate );
		}

		public static double GetArmorOffset( Mobile from )
		{
            double fullMeditationScalar = 1.0; //Full Meditation 
            double noMeditationScalar = 4.0; //No Meditation is 4x Slower than Full Meditation (Half Meditation is 2x Slower)

            double effectiveMeditationScalar = 1.0;

            double armorPenaltyPercent = 0;

            PlayerMobile player = from as PlayerMobile;
             
            if (player == null)
                return effectiveMeditationScalar;

            DungeonArmor.PlayerDungeonArmorProfile playerDungeonArmor = new DungeonArmor.PlayerDungeonArmorProfile(player, null);

            //Player Is Wearing Dungeon Armor
            if (playerDungeonArmor.MatchingSet)
            {
                switch (playerDungeonArmor.DungeonArmorDetail.MeditationAllowance)
                {
                    case ArmorMeditationAllowance.All: armorPenaltyPercent = 0; break;
                    case ArmorMeditationAllowance.ThreeQuarter: armorPenaltyPercent = .25; break; break;
                    case ArmorMeditationAllowance.Half: armorPenaltyPercent = .5; break;
                    case ArmorMeditationAllowance.Quarter: armorPenaltyPercent = .75; break;
                    case ArmorMeditationAllowance.None: armorPenaltyPercent = 1.0; break;
                }
            }

            else
            {
                //m_ArmorScalars = { 0.07, 0.07, 0.14, 0.15, 0.22, 0.35 };
                //Each Layer of Armor Has Different "Effect Amount" on How Much It Can Penalize Meditation
                armorPenaltyPercent += GetArmorMeditationValue(from.NeckArmor as BaseArmor) * BaseArmor.ArmorScalars[0];
                armorPenaltyPercent += GetArmorMeditationValue(from.HandArmor as BaseArmor) * BaseArmor.ArmorScalars[1];
                armorPenaltyPercent += GetArmorMeditationValue(from.HeadArmor as BaseArmor) * BaseArmor.ArmorScalars[2];
                armorPenaltyPercent += GetArmorMeditationValue(from.ArmsArmor as BaseArmor) * BaseArmor.ArmorScalars[3];
                armorPenaltyPercent += GetArmorMeditationValue(from.LegsArmor as BaseArmor) * BaseArmor.ArmorScalars[4];
                armorPenaltyPercent += GetArmorMeditationValue(from.ChestArmor as BaseArmor) * BaseArmor.ArmorScalars[5];
            }

            armorPenaltyPercent += GetArmorMeditationValue(from.ShieldArmor as BaseArmor) * .25;

            if (armorPenaltyPercent > 1.0)
                armorPenaltyPercent = 1.0;

            effectiveMeditationScalar = 4.0 * armorPenaltyPercent;

            if (effectiveMeditationScalar < fullMeditationScalar)
                effectiveMeditationScalar = fullMeditationScalar;

            if (effectiveMeditationScalar > noMeditationScalar)
                effectiveMeditationScalar = noMeditationScalar;

            return effectiveMeditationScalar;
		}

		private static double GetArmorMeditationValue( BaseArmor ar )
		{
			if ( ar == null || ar.ArmorAttributes.MageArmor != 0 || ar.Attributes.SpellChanneling != 0 )
				return 0.0;

			switch ( ar.MeditationAllowance )
			{
				default:
				case ArmorMeditationAllowance.None: return 1.0;
                case ArmorMeditationAllowance.Quarter: return .75;
				case ArmorMeditationAllowance.Half: return .5;
                case ArmorMeditationAllowance.ThreeQuarter: return .25;
				case ArmorMeditationAllowance.All:  return 0.0;
			}
		}
	}
}