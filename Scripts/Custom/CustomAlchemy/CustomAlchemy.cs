using System;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Gumps;
using Server.Custom;
using System.Collections;
using System.Collections.Generic;
using Server.Targeting;
using Server.Spells;

namespace Server.Items
{
    public class CustomAlchemy
    {
        public enum EffectType
        {
            //Positive
            StrengthIncrease,
            DexterityIncrease,
            IntelligenceIncrease,
            HitsRegenIncrease,
            StamRegenIncrease,
            ManaRegenIncrease,
            MeleeDamageDealtIncrease,
            MeleeDamageResistIncrease,
            SpellDamageDealtIncrease,
            SpellDamageResistIncrease,
            AccuracyIncrease,
            EvasionIncrease,

            //Negative
            Entangle,
            Petrify,
            FireDamage,
            FrostDamage,
            AbyssalDamage,
            ShockDamage,
            EarthDamage,
            BleedDamage,
            Disease,
            Poison,
            AccuracyReduction,
            EvasionReduction
        }

        public enum EffectPotencyType
        {
            Target,
            SmallAoE,
            MediumAoE,
            LargeAoE
        }

        public static int DistillationBaseChargesPerCraftingComponent = 3;
        public static double CookingSkillFirstExtraDistillationChargeThreshold = 100;
        public static double CookingSkillSecondExtraDistillationChargeThreshold = 120;

        public static int DistillIngredientsSound = 0x65A;
        public static int AddIngredientSound = 0x242;
        public static int RemoveIngredientSound = 0x5AC;
        public static int PotionSuccessSound = 0x04E;
        public static int PotionFailureSound = 0x5D1;

        public static double baseChance = .10;
        public static double bonusChancePerSkill = .01;
        public static double minimumSkill = 100;

        public static int minIngredients = 3;
        public static double extraIngredientModifier = 0.25;

        public static double upgradeChancePerThirdIngredient = .10;
        public static double upgradeTierScalarPerUpgrade = .5;

        public static double positivePotionChance = .5;

        public static double BaseCooldown = 60; //In Minutes
        public static double AlchemySkillCooldownPerSkillPoint = .4166; // In Minutes    

        public static double BaseMaxRange = 12;

        #region Potion Name

        public static string GetPotionName(CustomAlchemy.EffectType primaryEffect, CustomAlchemy.EffectType secondaryEffect, bool positiveEffect, CustomAlchemy.EffectPotencyType effectArea)
        {
            string potionName = "";

            switch (effectArea)
            {   
                case EffectPotencyType.SmallAoE: potionName += "Improved "; break;
                case EffectPotencyType.MediumAoE: potionName += "Greater "; break;
                case EffectPotencyType.LargeAoE: potionName += "Epic "; break;
            }

            if (positiveEffect)
            {
                switch (secondaryEffect)
                {
                    case EffectType.IntelligenceIncrease: potionName += "Clever"; break;
                    case EffectType.StamRegenIncrease: potionName += "Conditioning"; break;
                    case EffectType.ManaRegenIncrease: potionName += "Sagacious"; break;
                    case EffectType.SpellDamageDealtIncrease: potionName += "Sorcerous"; break;
                    case EffectType.DexterityIncrease: potionName += "Agile"; break;
                    case EffectType.SpellDamageResistIncrease: potionName += "Resisting"; break;
                    case EffectType.AccuracyIncrease: potionName += "Accurate"; break;
                    case EffectType.HitsRegenIncrease: potionName += "Regenerating"; break;
                    case EffectType.MeleeDamageResistIncrease: potionName += "Fortifying"; break;
                    case EffectType.MeleeDamageDealtIncrease: potionName += "Violent"; break;
                    case EffectType.EvasionIncrease: potionName += "Evasive"; break;
                    case EffectType.StrengthIncrease: potionName += "Enduring"; break;

                }
            }

            else
            {
                switch (secondaryEffect)
                {
                    case EffectType.FrostDamage: potionName += "Freezing"; break;
                    case EffectType.Entangle: potionName += "Entangling"; break;
                    case EffectType.Petrify: potionName += "Petrifying"; break;
                    case EffectType.FireDamage: potionName += "Fiery"; break;
                    case EffectType.AbyssalDamage: potionName += "Abyssal"; break;
                    case EffectType.Disease: potionName += "Diseased"; break;
                    case EffectType.ShockDamage: potionName += "Shocking"; break;
                    case EffectType.EvasionReduction: potionName += "Marking"; break;
                    case EffectType.EarthDamage: potionName += "Crushing"; break;
                    case EffectType.BleedDamage: potionName += "Bleeding"; break;
                    case EffectType.Poison: potionName += "Poisoning"; break;
                    case EffectType.AccuracyReduction: potionName += "Stunning"; break;
                }
            }

            potionName += " Potion of ";

            if (positiveEffect)
            {
                potionName += "the ";

                switch (primaryEffect)
                {
                    case EffectType.IntelligenceIncrease: potionName += "Imp"; break;
                    case EffectType.StamRegenIncrease: potionName += "Ostard"; break;
                    case EffectType.ManaRegenIncrease: potionName += "Wyrm"; break;
                    case EffectType.SpellDamageDealtIncrease: potionName += "Wisp"; break;
                    case EffectType.DexterityIncrease: potionName += "Wolf"; break;
                    case EffectType.SpellDamageResistIncrease: potionName += "Basilisk"; break;
                    case EffectType.AccuracyIncrease: potionName += "Eagle"; break;
                    case EffectType.HitsRegenIncrease: potionName += "Bat"; break;
                    case EffectType.MeleeDamageResistIncrease: potionName += "Crab"; break;
                    case EffectType.MeleeDamageDealtIncrease: potionName += "Sabertusk"; break;
                    case EffectType.EvasionIncrease: potionName += "Mongoose"; break;
                    case EffectType.StrengthIncrease: potionName += "Bear"; break;
                }
            }

            else
            {
                switch (primaryEffect)
                {
                    case EffectType.FrostDamage: potionName += "Frost"; break;
                    case EffectType.Entangle: potionName += "Entanglement"; break;
                    case EffectType.Petrify: potionName += "Petrification"; break;
                    case EffectType.FireDamage: potionName += "Flame"; break;
                    case EffectType.AbyssalDamage: potionName += "Abyss"; break;
                    case EffectType.Disease: potionName += "Disease"; break;
                    case EffectType.ShockDamage: potionName += "Lightning"; break;
                    case EffectType.EvasionReduction: potionName += "Marking"; break;
                    case EffectType.EarthDamage: potionName += "Crushing"; break;
                    case EffectType.BleedDamage: potionName += "Blood"; break;
                    case EffectType.Poison: potionName += "Poison"; break;
                    case EffectType.AccuracyReduction: potionName += "Stunning"; break;
                }
            }

            return potionName;
        }

        #endregion

        #region Component Detail

        public static CustomAlchemyComponentDetail GetComponentDetail(CraftingComponent.CraftingComponentType craftingComponentType)
        {
            CustomAlchemyComponentDetail componentDetail = new CustomAlchemyComponentDetail();

            switch (craftingComponentType)
            {
                case CraftingComponent.CraftingComponentType.BluecapMushroom:
                    componentDetail.m_PositiveEffectType = EffectType.IntelligenceIncrease;                    
                    componentDetail.m_NegativeEffectType = EffectType.FrostDamage;

                    componentDetail.m_PositivePotionHue = 2629;
                    componentDetail.m_NegativePotionHue = 2123;
                break;

                case CraftingComponent.CraftingComponentType.CockatriceEgg:
                    componentDetail.m_PositiveEffectType = EffectType.ManaRegenIncrease;
                    componentDetail.m_NegativeEffectType = EffectType.Petrify;

                    componentDetail.m_PositivePotionHue = 2602;
                    componentDetail.m_NegativePotionHue = 2302;
                break;

                case CraftingComponent.CraftingComponentType.Creepervine:
                    componentDetail.m_PositiveEffectType = EffectType.StamRegenIncrease;
                    componentDetail.m_NegativeEffectType = EffectType.Entangle;

                    componentDetail.m_PositivePotionHue = 2109;
                    componentDetail.m_NegativePotionHue = 2207;
                break;                

                case CraftingComponent.CraftingComponentType.FireEssence:
                    componentDetail.m_PositiveEffectType = EffectType.SpellDamageDealtIncrease;
                    componentDetail.m_NegativeEffectType = EffectType.FireDamage;

                    componentDetail.m_PositivePotionHue = 1257;
                    componentDetail.m_NegativePotionHue = 2117;
                break;

                case CraftingComponent.CraftingComponentType.Ghostweed:
                    componentDetail.m_PositiveEffectType = EffectType.DexterityIncrease;
                    componentDetail.m_NegativeEffectType = EffectType.AbyssalDamage;

                    componentDetail.m_PositivePotionHue = 2500;
                    componentDetail.m_NegativePotionHue = 1108;
                break;

                case CraftingComponent.CraftingComponentType.GhoulHide:
                    componentDetail.m_PositiveEffectType = EffectType.SpellDamageResistIncrease;
                    componentDetail.m_NegativeEffectType = EffectType.Disease;

                    componentDetail.m_PositivePotionHue = 2515;
                    componentDetail.m_NegativePotionHue = 2112;
                break;

                case CraftingComponent.CraftingComponentType.LuniteHeart:
                    componentDetail.m_PositiveEffectType = EffectType.AccuracyIncrease;
                    componentDetail.m_NegativeEffectType = EffectType.ShockDamage;

                    componentDetail.m_PositivePotionHue = 2603;
                    componentDetail.m_NegativePotionHue = 2615;
                break;

                case CraftingComponent.CraftingComponentType.ObsidianShard:
                    componentDetail.m_PositiveEffectType = EffectType.HitsRegenIncrease;
                    componentDetail.m_NegativeEffectType = EffectType.EvasionReduction;

                    componentDetail.m_PositivePotionHue = 2635;
                    componentDetail.m_NegativePotionHue = 2706;
                break;

                case CraftingComponent.CraftingComponentType.Quartzstone:
                    componentDetail.m_PositiveEffectType = EffectType.MeleeDamageResistIncrease;
                    componentDetail.m_NegativeEffectType = EffectType.EarthDamage;

                    componentDetail.m_PositivePotionHue = 2638;
                    componentDetail.m_NegativePotionHue = 2709;
                break;

                case CraftingComponent.CraftingComponentType.ShatteredCrystal:
                    componentDetail.m_PositiveEffectType = EffectType.MeleeDamageDealtIncrease;
                    componentDetail.m_NegativeEffectType = EffectType.BleedDamage;

                    componentDetail.m_PositivePotionHue = 2552;
                    componentDetail.m_NegativePotionHue = 2118;
                break;

                case CraftingComponent.CraftingComponentType.Snakeskin:
                    componentDetail.m_PositiveEffectType = EffectType.EvasionIncrease;
                    componentDetail.m_NegativeEffectType = EffectType.Poison;

                    componentDetail.m_PositivePotionHue = 1463;
                    componentDetail.m_NegativePotionHue = 1370;
                break;

                case CraftingComponent.CraftingComponentType.TrollFat:
                    componentDetail.m_PositiveEffectType = EffectType.StrengthIncrease;
                    componentDetail.m_NegativeEffectType = EffectType.AccuracyReduction;

                    componentDetail.m_PositivePotionHue = 2575;
                    componentDetail.m_NegativePotionHue = 2518;
                break;
            }

            return componentDetail;
        }

        #endregion

        public static void UsePotion(CustomAlchemyPotion potion, Mobile from)
        {
            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            if (!potion.IsChildOf(from.Backpack))
            {
                from.SendMessage("That item must be in your pack in order to use it.");
                return;
            }

            if (!from.CanBeginAction(typeof(CustomAlchemyPotion)) && player.AccessLevel == AccessLevel.Player)
            {
                from.SendMessage("You must wait before using another custom alchemy potion.");
                return;
            }

            if (potion.PositiveEffect)
                from.SendMessage("Target the creature or player to use this potion on.");
            else
                from.SendMessage("Target the creature to use this potion on.");

            from.Target = new PotionTarget(potion, player);
        }

        public class PotionTarget : Target
        {
            public CustomAlchemyPotion m_Potion;
            public PlayerMobile m_Player;

            public PotionTarget(CustomAlchemyPotion potion, PlayerMobile player): base(25, true, TargetFlags.None, false)
            {
                m_Potion = potion;
                m_Player = player;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                if (m_Potion == null) return;
                if (m_Potion.Deleted) return;
                if (m_Player == null) return;
                if (m_Player.Deleted || !m_Player.Alive) return;

                if (!m_Potion.IsChildOf(m_Player.Backpack))
                {
                    m_Player.SendMessage("That item must be in your pack in order to use it.");
                    return;
                }

                Mobile mobileTarget = target as Mobile;

                if (mobileTarget == null)
                {
                    m_Player.SendMessage("That is not a creature or player.");
                    return;
                }

                if (m_Player.AccessLevel == AccessLevel.Player)
                {
                    if (!mobileTarget.CanBeDamaged() || !mobileTarget.Alive)
                    {
                        m_Player.SendMessage("That target cannot be damaged.");
                        return;
                    }

                    if (!mobileTarget.InLOS(m_Player) || mobileTarget.Hidden)
                    {
                        m_Player.SendMessage("That target is not in your line of sight.");
                        return;
                    }

                    if (!m_Player.CanBeginAction(typeof(CustomAlchemyPotion)))
                    {
                        from.SendMessage("You must wait before using another custom alchemy potion.");
                        return;
                    }
                }
                               
                if (m_Potion.PositiveEffect && m_Player.AccessLevel == AccessLevel.Player)
                {
                    if (mobileTarget.NextCustomAlchemyPositiveEffectAllowed > DateTime.UtcNow)
                    {
                        string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, mobileTarget.NextCustomAlchemyPositiveEffectAllowed, false, true, true, true, true);

                        if (mobileTarget == m_Player)
                            m_Player.SendMessage("You must wait another " + timeRemaining + " before receieving additional beneficial effects from custom alchemy.");

                        else
                            m_Player.SendMessage("That target must wait another " + timeRemaining + " before receieving additional beneficial effects from custom alchemy.");

                        return;
                    }
                }

                ResolvePotion(m_Player, mobileTarget, m_Potion);
            }
        }

        public static void ResolvePotion(PlayerMobile player, Mobile mobileTarget, CustomAlchemyPotion potion)
        {
            if (!SpecialAbilities.Exists(player)) return;
            if (!SpecialAbilities.Exists(mobileTarget)) return;

            bool positiveEffect = potion.PositiveEffect;

            CustomAlchemy.EffectType primaryEffect = potion.PrimaryEffect;
            CustomAlchemy.EffectType secondaryEffect = potion.SecondaryEffect;
            CustomAlchemy.EffectPotencyType potencyType = potion.EffectPotency;

            int throwSound = 0x5D3;
            int itemID = potion.ItemID;
            int itemHue = potion.Hue;

            int hitSound = Utility.RandomList(0x38E, 0x38F, 0x390);
            int effectSound = 0x5D8;   

            ConsumePotion(player, potion);

            SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1.0, 0.75, true, 0, false, "", "");

            Point3D location = player.Location;
            Map map = player.Map;

            Point3D targetLocation = mobileTarget.Location;
            Map targetMap = mobileTarget.Map;

            bool drinkPotion = false;

            if (positiveEffect && player == mobileTarget && potencyType == EffectPotencyType.Target)
                drinkPotion = true;
                
            if (drinkPotion)
            {
                player.Animate(34, 5, 1, true, false, 0);
                Effects.PlaySound(player.Location, player.Map, Utility.RandomList(0x4CC, 0x4CD, 0x030, 0x031));
            }

            else
            {
                player.Animate(31, 7, 1, true, false, 0);
                Effects.PlaySound(player.Location, player.Map, throwSound);
            }

            Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
            {
                if (!SpecialAbilities.Exists(player))
                    return;

                //Update Target Location if MobileTarget still Valid
                if (mobileTarget != null && !drinkPotion)
                {         
                    if (Utility.GetDistance(mobileTarget.Location, targetLocation) < 20 && mobileTarget.Map == targetMap)                    
                        targetLocation = mobileTarget.Location;                    
                }

                double distance = Utility.GetDistance(location, targetLocation);
                double destinationDelay = (double)distance * .04;
                
                IEntity startLocation = new Entity(Serial.Zero, new Point3D(location.X, location.Y, location.Z + 5), map);
                IEntity endLocation = new Entity(Serial.Zero, new Point3D(targetLocation.X, targetLocation.Y, targetLocation.Z + 5), targetMap);

                if (drinkPotion)
                    destinationDelay = 0;

                else
                    Effects.SendMovingEffect(startLocation, endLocation, itemID, 10, 0, false, false, itemHue, 0);               

                Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                {
                    //Update Target Location if MobileTarget still Valid
                    if (mobileTarget != null && !drinkPotion)
                    {                       
                        if (Utility.GetDistance(mobileTarget.Location, targetLocation) < 20 && mobileTarget.Map == targetMap)                        
                            targetLocation = mobileTarget.Location;                        
                    }

                    int primaryEffectItemId = 0x3709;
                    int primaryEffectHue = 0;
                    int primaryEffectSound = 0x208;

                    int secondaryEffectItemId = 0x3709;
                    int secondaryEffectHue = 0;
                    int secondaryEffectSound = 0x208;

                    List<Point3D> m_EffectLocations = new List<Point3D>();

                    if (drinkPotion && SpecialAbilities.Exists(player))
                        m_EffectLocations.Add(player.Location);

                    else
                    {
                        int radius = 0;

                        switch (potencyType)
                        {
                            case EffectPotencyType.SmallAoE: radius = 2; break;
                            case EffectPotencyType.MediumAoE: radius = 4; break;
                            case EffectPotencyType.LargeAoE: radius = 6; break;
                        }

                        int minRange = -1 * radius;
                        int maxRange = radius + 1;

                        for (int a = minRange; a < maxRange; a++)
                        {
                            for (int b = minRange; b < maxRange; b++)
                            {
                                Point3D newLocation = new Point3D(targetLocation.X + a, targetLocation.Y + b, targetLocation.Z);

                                if (!m_EffectLocations.Contains(newLocation))
                                    m_EffectLocations.Add(newLocation);
                            }
                        }
                    }    
               
                    foreach(Point3D point in m_EffectLocations)
                    {
                        Effects.SendLocationParticles(EffectItem.Create(point, map, TimeSpan.FromSeconds(0.25)), primaryEffectItemId, 10, 30, primaryEffectHue, 0, 5029, 0);
                    
                        //Effect
                    
                    }

                    /*
                    //Positive
                    StrengthIncrease,
                    DexterityIncrease,
                    IntelligenceIncrease,
                    HitsRegenIncrease,
                    StamRegenIncrease,
                    ManaRegenIncrease,
                    MeleeDamageDealtIncrease,
                    MeleeDamageResistIncrease,
                    SpellDamageDealtIncrease,
                    SpellDamageResistIncrease,
                    AccuracyIncrease,
                    EvasionIncrease,

                    //Negative
                    Entangle,
                    Petrify,
                    FireDamage,
                    FrostDamage,
                    AbyssalDamage,
                    ShockDamage,
                    EarthDamage,
                    BleedDamage,
                    Disease,
                    Poison,
                    AccuracyReduction,
                    EvasionReduction
                     
                    Target,
                    SmallAoE,
                    MediumAoE,
                    LargeAoE
                    */

                    if (drinkPotion)
                    {
                    }
                });                
            });
        }

        public static void ConsumePotion(PlayerMobile player, CustomAlchemyPotion potion)
        {
            if (player != null)
            {
                double cooldown = BaseCooldown;
                double alchemySkill = player.Skills.Alchemy.Value;

                cooldown -= (alchemySkill * AlchemySkillCooldownPerSkillPoint);

                TimeSpan cooldownDelay = TimeSpan.FromMinutes(cooldown);

                player.BeginAction(typeof(CustomAlchemyPotion));

                Timer.DelayCall(cooldownDelay, delegate
                {
                    if (player != null)
                        player.EndAction(typeof(CustomAlchemyPotion));
                });
            }

            if (potion != null)
                potion.Delete();
        }
    }

    public class CustomAlchemyComponentDetail
    {
        public CraftingComponent.CraftingComponentType m_CraftingComponent = CraftingComponent.CraftingComponentType.BluecapMushroom;
        
        public CustomAlchemy.EffectType m_PositiveEffectType = CustomAlchemy.EffectType.AccuracyIncrease;
        public CustomAlchemy.EffectType m_NegativeEffectType = CustomAlchemy.EffectType.Entangle;
        public int m_PositivePotionHue = 0;
        public int m_NegativePotionHue = 0;

        public CustomAlchemyComponentDetail()
        {
        }
    }
}