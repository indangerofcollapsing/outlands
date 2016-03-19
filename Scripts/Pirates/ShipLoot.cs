using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Misc;
using Server.Mobiles;
using Server.Items;
using Server.Multis;

namespace Server.Items
{
    public static class ShipLoot
    {
        public static void GenerateShipLoot(BaseBoat boat)
        {
            if (boat == null) return;
            if (boat.Deleted) return;
            if (boat.Hold == null) return;
            if (boat.Hold.Deleted) return;

            int shipLevel = 1;

            if (boat is MediumBoat || boat is MediumDragonBoat) shipLevel = 2;
            if (boat is LargeBoat || boat is LargeDragonBoat) shipLevel = 3;
            if (boat is CarrackBoat) shipLevel = 4;
            if (boat is GalleonBoat) shipLevel = 5;

            int gold = 0;
            int cannonShot = 0;    
            int boards = 0;
            int cloth = 0;
            int ironIngots = 0;

            double rareChance = 0;
            double prestigeScrollChance = 0;
            double craftingComponentChance = 0;

            switch (boat.MobileFactionType)
            {
                case MobileFactionType.Fishing:
                    gold = 500 + (Utility.RandomMinMax(200 * (shipLevel - 1), 300 * (shipLevel - 1)));                    
                    boards = Utility.RandomMinMax(10 * shipLevel, 20 * shipLevel);
                    cloth = Utility.RandomMinMax(10 * shipLevel, 20 * shipLevel);
                    ironIngots = Utility.RandomMinMax(10 * shipLevel, 20 * shipLevel);
                    cannonShot = Utility.RandomMinMax(10 * shipLevel, 20 * shipLevel);                  
                    rareChance = 0.0075 * shipLevel;
                    prestigeScrollChance = 0.02 * shipLevel;
                    craftingComponentChance = 0.02 * shipLevel;
                break;

                case MobileFactionType.Pirate:
                    gold = 2000 + (Utility.RandomMinMax(450 * (shipLevel - 1), 550 * (shipLevel - 1)));                    
                    boards = Utility.RandomMinMax(10 * shipLevel, 20 * shipLevel);
                    cloth = Utility.RandomMinMax(10 * shipLevel, 20 * shipLevel);
                    ironIngots = Utility.RandomMinMax(10 * shipLevel, 20 * shipLevel);
                    cannonShot = Utility.RandomMinMax(10 * shipLevel, 20 * shipLevel);                  
                    rareChance = 0.02 * shipLevel;
                    prestigeScrollChance = 0.06 * shipLevel;
                    craftingComponentChance = 0.06 * shipLevel;
                break;

                case MobileFactionType.Britain:
                    gold = 2000 + (Utility.RandomMinMax(450 * (shipLevel - 1), 550 * (shipLevel - 1)));                    
                    boards = Utility.RandomMinMax(10 * shipLevel, 20 * shipLevel);
                    cloth = Utility.RandomMinMax(10 * shipLevel, 20 * shipLevel);
                    ironIngots = Utility.RandomMinMax(10 * shipLevel, 20 * shipLevel);
                    cannonShot = Utility.RandomMinMax(10 * shipLevel, 20 * shipLevel);               
                    rareChance = 0.02 * shipLevel;
                    prestigeScrollChance = 0.06 * shipLevel;
                    craftingComponentChance = 0.06 * shipLevel;
                break;                

                case MobileFactionType.Orc:
                    gold = 2500 + (Utility.RandomMinMax(700 * (shipLevel - 1), 800 * (shipLevel - 1)));                    
                    boards = Utility.RandomMinMax(10 * shipLevel, 20 * shipLevel);
                    cloth = Utility.RandomMinMax(10 * shipLevel, 20 * shipLevel);
                    ironIngots = Utility.RandomMinMax(10 * shipLevel, 20 * shipLevel);
                    cannonShot = Utility.RandomMinMax(10 * shipLevel, 20 * shipLevel);              
                    rareChance = 0.025 * shipLevel;
                    prestigeScrollChance = 0.07 * shipLevel;
                    craftingComponentChance = 0.07 * shipLevel;
                break;

                case MobileFactionType.Orghereim:
                    gold = 2500 + (Utility.RandomMinMax(700 * (shipLevel - 1), 800 * (shipLevel - 1)));                    
                    boards = Utility.RandomMinMax(10 * shipLevel, 20 * shipLevel);
                    cloth = Utility.RandomMinMax(10 * shipLevel, 20 * shipLevel);
                    ironIngots = Utility.RandomMinMax(10 * shipLevel, 20 * shipLevel);
                    cannonShot = Utility.RandomMinMax(10 * shipLevel, 20 * shipLevel);                 
                    rareChance = 0.025 * shipLevel;
                    prestigeScrollChance = 0.07 * shipLevel;
                    craftingComponentChance = 0.07 * shipLevel;
                break;

                case MobileFactionType.Undead:
                    gold = 3000 + (Utility.RandomMinMax(950 * (shipLevel - 1), 1050 * (shipLevel - 1)));
                    boards = Utility.RandomMinMax(10 * shipLevel, 20 * shipLevel);
                    cloth = Utility.RandomMinMax(10 * shipLevel, 20 * shipLevel);
                    ironIngots = Utility.RandomMinMax(10 * shipLevel, 20 * shipLevel);
                    cannonShot = Utility.RandomMinMax(10 * shipLevel, 20 * shipLevel);                    
                    rareChance = 0.025 * shipLevel;
                    prestigeScrollChance = 0.08 * shipLevel;
                    craftingComponentChance = 0.08 * shipLevel;
                break;
            }

            if (gold > 0)
                boat.Hold.DropItem(new Gold(gold));            

            if (cannonShot > 0)
                boat.Hold.DropItem(new CannonShot(cannonShot));

            if (boards > 0)
                boat.Hold.DropItem(new Board(boards));

            if (cloth > 0)
                boat.Hold.DropItem(new Cloth(cloth));

            if (ironIngots > 0)
                boat.Hold.DropItem(new IronIngot(ironIngots));
          
            if (Utility.RandomDouble() < rareChance)
            {
                Item item = GetShipRare();
                
                if (item != null)
                    boat.Hold.DropItem(item);               
            }

            if (Utility.RandomDouble() < prestigeScrollChance)
            {
                if (Utility.RandomDouble() <= .5)
                    boat.Hold.DropItem(new PrestigeScroll(IndexedRegionName.BuccaneeersDen));
                else
                    boat.Hold.DropItem(new PrestigeScroll());
            }

            if (Utility.RandomDouble() < craftingComponentChance)
                boat.Hold.DropItem(CraftingComponent.GetRandomCraftingComponent(1));
        }

        public static Item GetShipRare()
        {
            Item item = null;

            Dictionary<Type, int> DictItemTypes = new Dictionary<Type, int>();

            //Rares Possible
            DictItemTypes.Add(typeof(DullCopperCannonMetalUpgradeDeed), 2);
            DictItemTypes.Add(typeof(CopperCannonMetalUpgradeDeed), 2);
            DictItemTypes.Add(typeof(BronzeCannonMetalUpgradeDeed), 2);
            DictItemTypes.Add(typeof(ShadowIronCannonMetalUpgradeDeed), 2);
            DictItemTypes.Add(typeof(GoldCannonMetalUpgradeDeed), 2);
            DictItemTypes.Add(typeof(AgapiteCannonMetalUpgradeDeed), 2);

            DictItemTypes.Add(typeof(VeriteCannonMetalUpgradeDeed), 1);
            DictItemTypes.Add(typeof(ValoriteCannonMetalUpgradeDeed), 1);
            DictItemTypes.Add(typeof(LuniteCannonMetalUpgradeDeed), 1);
            DictItemTypes.Add(typeof(ArcaneIronCannonMetalUpgradeDeed), 1);
            DictItemTypes.Add(typeof(ColdSteelCannonMetalUpgradeDeed), 1);
            DictItemTypes.Add(typeof(DeepSeaIronCannonMetalUpgradeDeed), 1);
            DictItemTypes.Add(typeof(FrostIronCannonMetalUpgradeDeed), 1);
            DictItemTypes.Add(typeof(RoseIronCannonMetalUpgradeDeed), 1);
            DictItemTypes.Add(typeof(WrathIronCannonMetalUpgradeDeed), 1);

            DictItemTypes.Add(typeof(ChainshotAmmunitionEpicAbilityUpgradeDeed), 1);
            DictItemTypes.Add(typeof(DoubleTimeEpicAbilityUpgradeDeed), 1);
            DictItemTypes.Add(typeof(GrapeshotAmmunitionEpicAbilityUpgradeDeed), 1);
            DictItemTypes.Add(typeof(HellfireAmmunitionEpicAbilityUpgradeDeed), 1);
            DictItemTypes.Add(typeof(HighExplosiveAmmunitionEpicAbilityUpgradeDeed), 1);
            DictItemTypes.Add(typeof(PowdersoakAmmunitionEpicAbilityUpgradeDeed), 1);
            DictItemTypes.Add(typeof(SmokeshotAmmunitionEpicAbilityUpgradeDeed), 1);

            DictItemTypes.Add(typeof(DestroyerOutfittingUpgradeDeed), 1);
            DictItemTypes.Add(typeof(DreadnoughtOutfittingUpgradeDeed), 1);
            DictItemTypes.Add(typeof(HunterOutfittingUpgradeDeed), 1);
            DictItemTypes.Add(typeof(MerchantOutfittingUpgradeDeed), 1);
            DictItemTypes.Add(typeof(RunnerOutfittingUpgradeDeed), 1);

            DictItemTypes.Add(typeof(DerelictThemeUpgrade), 2);
            DictItemTypes.Add(typeof(TreasureThemeUpgrade), 2);
            
            int TotalValues = 0;

            foreach (KeyValuePair<Type, int> pair in DictItemTypes)
            {
                TotalValues += pair.Value;
            }

            double ActionCheck = Utility.RandomDouble();
            double CumulativeAmount = 0.0;
            double AdditionalAmount = 0.0;

            bool foundDirection = true;

            foreach (KeyValuePair<Type, int> pair in DictItemTypes)
            {
                AdditionalAmount = (double)pair.Value / (double)TotalValues;

                if (ActionCheck >= CumulativeAmount && ActionCheck < (CumulativeAmount + AdditionalAmount))
                {
                    item = (Item)Activator.CreateInstance(pair.Key);

                    if (item == null)
                        continue;

                    return item;
                }

                CumulativeAmount += AdditionalAmount;
            }

            return item;
        }
    }
}
