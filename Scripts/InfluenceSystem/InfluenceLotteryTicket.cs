using Server.Multis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Mobiles;
using Server.Misc;
using Server.Network;

namespace Server.Items
{
    class InfluenceLotteryTicket : Item
    {  
        public bool inUse = false;

        [Constructable]
        public InfluenceLotteryTicket(): base(0x14ED)
        {
            Name = "an influence lottery ticket";

            Hue = 2615;
            Weight = 0.1;            
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (inUse)
                return;

            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("That item must be in your pack in order to use it.");
                return;
            }

            if (from.TotalWeight >= 375 && from.AccessLevel == AccessLevel.Player)
            {
                from.SendMessage("You are carrying too much weight to be able to accept any rewards.");
                return;
            }

            from.SendSound(0x249);
            from.SendMessage("You unfurl an influence lottery ticket...");

            inUse = true;

            Timer.DelayCall(TimeSpan.FromSeconds(2), delegate
            {
                if (from == null)
                    return;

                ReceiveReward(from);
            });
        }

        public void ReceiveReward(Mobile from)
        {
            int TotalValues = 0;   

            Item item = null;
            string rewardText = "";
            int rewardSound = 0x5AA;

            Dictionary<string, int> DictTemp = new Dictionary<string, int>();
            
            //Used to Determine Chance of Last Reward Item (GoldSmallAmount)
            int maxRandomValue = 1000;

            int ultraRareValue = 2;
            int rareValue = 7;
            int uncommonValue = 20;
            int commonValue = 150;
            
            //Ultra Rares
            DictTemp.Add("PlayerCustomizationDeed", ultraRareValue);
            DictTemp.Add("AntiqueStandingClock", ultraRareValue);
            DictTemp.Add("CityLampPost", ultraRareValue);
            DictTemp.Add("LuniteIngots", ultraRareValue);
            DictTemp.Add("AquariumDeed", ultraRareValue);
            DictTemp.Add("LuxuriousFurniture", ultraRareValue);
            DictTemp.Add("MiniatureTownPiece", ultraRareValue);
            DictTemp.Add("UltraRareCloth", ultraRareValue);            
            DictTemp.Add("DisplayCaseLargeAddon", ultraRareValue);
            DictTemp.Add("DisplayCaseMediumAddon", ultraRareValue);
            DictTemp.Add("LargeBenchAddon", ultraRareValue);
            DictTemp.Add("BarTopGlassMugs", ultraRareValue);
            DictTemp.Add("BarTopGoblets", ultraRareValue);
            DictTemp.Add("ExplodingBullvore", ultraRareValue);

            //Rares
            DictTemp.Add("UnfinishedFurniture", rareValue);
            DictTemp.Add("SpellHueDeed", rareValue);
            DictTemp.Add("MiniatureHome", rareValue);
            DictTemp.Add("CraftingComponentsLargeAmount", rareValue);
            DictTemp.Add("GoldHugeAmount", rareValue);
            DictTemp.Add("TableWithRunner", rareValue);
            DictTemp.Add("ExplodingLlama", rareValue);
            DictTemp.Add("MagicalWeapon", rareValue);
            DictTemp.Add("MagicalArmor", rareValue);
            DictTemp.Add("RareCloth", rareValue);
            DictTemp.Add("HideRack", rareValue);
            DictTemp.Add("HideRackLargeAddon", rareValue);
            DictTemp.Add("DisplayCaseSmall", rareValue);

            //Uncommon
            DictTemp.Add("CraftingComponentsMediumAmount", uncommonValue);
            DictTemp.Add("SpiderSilkFishingNet", uncommonValue);
            DictTemp.Add("CampingFirepit", uncommonValue);
            DictTemp.Add("GoldLargeAmount", uncommonValue);
            DictTemp.Add("ParagonWand", uncommonValue);
            DictTemp.Add("Lure", uncommonValue);
            DictTemp.Add("ResearchMaterials", uncommonValue);
            DictTemp.Add("AncientMysteryScroll", uncommonValue);
            DictTemp.Add("LavishPillow", uncommonValue);
            DictTemp.Add("VeterinarySalts", uncommonValue);
            DictTemp.Add("ExplodingSheep", uncommonValue);
            DictTemp.Add("DiamondHarvestingTool", uncommonValue);

            //Common
            DictTemp.Add("PrestigeScroll", commonValue);
            DictTemp.Add("GoldMediumAmount", commonValue);
            DictTemp.Add("CraftingComponentsSmallAmount", (int)Math.Round((double)commonValue / 2));
            DictTemp.Add("Reagents", commonValue);

            foreach (KeyValuePair<string, int> pair in DictTemp)
            {
                TotalValues += pair.Value;
            }
           
            int remainderItemChance = maxRandomValue - TotalValues;

            if (remainderItemChance < 1)
                remainderItemChance = 1;

            //Last Reward Item (Chance = maxRandomValue - Sum of All Item Values Combined)
            DictTemp.Add("GoldSmallAmount", remainderItemChance);

            TotalValues += remainderItemChance;    
            
            double ItemCheck = Utility.RandomDouble();

            double CumulativeAmount = 0.0;
            double AdditionalAmount = 0.0;

            //Determine Reward                      
            foreach (KeyValuePair<string, int> pair in DictTemp)
            {
                AdditionalAmount = (double)pair.Value / (double)TotalValues;

                if (ItemCheck >= CumulativeAmount && ItemCheck < (CumulativeAmount + AdditionalAmount))
                {
                    Bag bag;

                    switch (pair.Key)
                    {
                        //Ultra Rares
                        case "PlayerCustomizationDeed":
                            item = new Custom.PlayerCustomizationDeed();
                            rewardText = "You receive a player customization deed.";
                        break;    
                        
                        case "AntiqueStandingClock":
                            item = new AntiqueStandingClock();
                            rewardText = "You receive an antique standing clock.";
                        break;

                        case "CityLampPost":
                            item = new CityLampPost();
                            rewardText = "You receive a city lamp post.";
                        break;

                        case "LuniteIngots":
                            item = new LuniteIngot(Utility.RandomMinMax(25, 50));
                            rewardText = "You receive some lunite ingots.";
                        break;

                        case "AquariumDeed":
                            if (Utility.RandomDouble() <= .5)                           
                                item = new AquariumEastDeed();
                            else
                                item = new AquariumNorthDeed();

                            rewardText = "You receive an aquarium deed";
                        break;

                        case "LuxuriousFurniture":
                            switch (Utility.RandomMinMax(1, 12))
                            {
                                case 1: item = new LuxuriousBookstand(); break;
                                case 2: item = new LuxuriousCouch(); break;
                                case 3: item = new LuxuriousOvalTable(); break;
                                case 4: item = new LuxuriousChair(); break;
                                case 5: item = new LuxuriousTable(); break;
                                case 6: item = new LargeLuxuriousChest(); break;
                                case 7: item = new LargeLuxuriousChair(); break;
                                case 8: item = new LuxuriousBookshelf(); break;
                                case 9: item = new LuxuriousChest(); break;
                                case 10: item = new LuxuriousStorageBox(); break;
                                case 11: item = new LuxuriousReadingDesk(); break;
                                case 12: item = new LuxuriousAlchemyTable(); break;
                            }
                           
                            rewardText = "You receive some luxurious furniture.";
                        break;

                        case "MiniatureTownPiece":
                            item = new MiniatureTownPiece();
                            rewardText = "You receive a miniature town piece.";
                        break;

                        case "UltraRareCloth":
                            item = new UltraRareCloth();
                            rewardText = "You receive some ultra rare cloth.";
                        break;                        

                        case "DisplayCaseLargeAddon":
                            switch (Utility.RandomMinMax(1, 2))
                            {
                                case 1: item = new DisplayCaseLargeEastAddonDeed(); break;
                                case 2: item = new DisplayCaseLargeNorthAddonDeed(); break;
                            }
                            
                            rewardText = "You receive a large display case.";
                        break;

                        case "DisplayCaseMediumAddon":
                            switch (Utility.RandomMinMax(1, 2))
                            {
                                case 1: item = new DisplayCaseMediumEastAddonDeed(); break;
                                case 2: item = new DisplayCaseMediumNorthAddonDeed(); break;
                            }

                            rewardText = "You receive a medium display case.";
                        break;

                        case "LargeBenchAddon":
                             switch (Utility.RandomMinMax(1, 2))
                            {
                                case 1: item = new LargeBenchEastAddonDeed(); break;
                                case 2: item = new LargeBenchNorthAddonDeed(); break;
                            }

                            rewardText = "You receive a large bench.";
                        break;

                        case "BarTopGlassMugs":
                            item = new BarTopGlassMugs();
                            rewardText = "You receive bar top glass mugs.";
                        break;

                        case "BarTopGoblets":
                            item = new BarTopGoblets();
                            rewardText = "You receive bar top goblets.";
                        break;

                        case "ExplodingBullvore":
                            item = new ExplodingBullvore();
                            rewardText = "You receive an exploding bullvore.";
                        break;

                        //Rares
                        case "UnfinishedFurniture":
                            switch (Utility.RandomMinMax(1, 5))
                            {
                                case 1: item = new UnfinishedWoodenChair(); break;
                                case 2: item = new UnfinishedDresser(); break;
                                case 3: item = new UnfinishedTableLegs(); break;
                                case 4: item = new UnfinishedTablePieces(); break;
                                case 5: item = new UnfinishedBookshelf(); break;
                            }
                           
                            rewardText = "You receive some unfinished furniture.";
                        break;

                        case "SpellHueDeed":
                            item = new Custom.SpellHueDeed();
                            rewardText = "You receive a spell hue deed.";
                        break;

                        case "MiniatureHome":
                            item = new MiniatureHome();
                            rewardText = "You receive a miniature home.";
                        break;

                        case "CraftingComponentsLargeAmount":
                            item = CraftingComponent.GetRandomCraftingComponent(10);
                            rewardText = "You receive a large number of crafting components.";
                        break;

                        case "GoldHugeAmount":
                            item = new Gold(Utility.RandomMinMax(2500, 10000));
                            rewardText = "You receive a very large amount of gold.";
                        break;

                        case "TableWithRunner":
                            item = new TableWithRunner();
                            rewardText = "You receive a small table with runner.";
                        break;

                        case "ExplodingLlama":
                            item = new ExplodingLlama();
                            rewardText = "You receive an exploding llama.";
                        break;

                        case "MagicalWeapon":
                            BaseWeapon weapon = Loot.RandomWeapon();

                            if (weapon != null)
                            {
                                weapon.AccuracyLevel = (WeaponAccuracyLevel)Utility.RandomMinMax(3, 5);
                                weapon.DamageLevel = (WeaponDamageLevel)Utility.RandomMinMax(3, 5);
                                weapon.DurabilityLevel = (WeaponDurabilityLevel)Utility.RandomMinMax(3, 5);
                            }

                            item = weapon;
                            rewardText = "You receive a magical weapon.";
                        break;

                        case "MagicalArmor":
                            BaseArmor armor = Loot.RandomArmor();

                            if (armor != null)
                            {
                                armor.ProtectionLevel = (ArmorProtectionLevel)Utility.RandomMinMax(4, 5);
                                armor.DurabilityLevel = (ArmorDurabilityLevel)Utility.RandomMinMax(4, 5);
                            }

                            item = armor;
                            rewardText = "You receive some magical armor.";
                        break;

                        case "RareCloth":
                            item = new RareCloth();
                            rewardText = "You receive some rare cloth.";
                        break;

                        case "HideRack":
                            item = new HideRack();
                            rewardText = "You receive a hide rack.";
                        break;

                        case "HideRackLargeAddon":
                            switch (Utility.RandomMinMax(1, 2))
                            {
                                case 1: item = new HideRackLargeEastAddonDeed(); break;
                                case 2: item = new HideRackLargeNorthAddonDeed(); break;
                            }

                            rewardText = "You receive a large hide rack.";
                        break;

                        case "DisplayCaseSmall":
                            item = new DisplayCaseSmall();
                            rewardText = "You receive a small display case.";
                        break;

                        //Uncommon
                        case "CraftingComponentsMediumAmount":
                            item = CraftingComponent.GetRandomCraftingComponent(6);
                            rewardText = "You receive a moderate number of crafting components.";
                        break;                        

                        case "SpiderSilkFishingNet":
                            item = new Custom.SpidersilkFishingNet();
                            rewardText = "You receive a spidersilk fishing net.";
                        break;

                        case "CampingFirepit":
                            item = new Custom.CampingFirepit();
                            rewardText = "You receive a camping firepit.";
                        break;

                        case "GoldLargeAmount":
                            item = new Gold(Utility.RandomMinMax(1000, 2500));
                            rewardText = "You receive a large amount of gold.";
                        break;

                        case "ParagonWand":
                            item = new ParagonWand();
                            rewardText = "You receive a paragon wand.";
                        break;

                        case "Lure":
                            if (Utility.RandomDouble() <= .66)
                                item = new Custom.FaintLure();
                            else
                                item = new Custom.PotentLure();
                            
                            rewardText = "You receive a lure.";
                        break;

                        case "ResearchMaterials":
                            item = new Custom.ResearchMaterials();
                            rewardText = "You receive some research materials.";
                        break;

                        case "AncientMysteryScroll":
                            item = new Custom.AncientMystery.AncientMysteryScroll();
                            rewardText = "You receive an ancient mystery scroll.";
                        break;

                        case "LavishPillow":
                            item = new LavishPillow();
                            rewardText = "You receive a lavish pillow.";
                        break;

                        case "VeterinarySalts":
                            item = new Custom.Items.VeterinarySalts();
                            rewardText = "You receive some veterinary salts.";
                        break;

                        case "ExplodingSheep":
                            item = new ExplodingSheep();
                            rewardText = "You receive an exploding sheep.";
                        break;

                        case "DiamondHarvestingTool":
                            switch (Utility.RandomMinMax(1, 2))
                            {
                                case 1: item = new DiamondPickaxe(); break;
                                case 2: item = new DiamondShovel(); break;
                            }

                            rewardText = "You receive a diamond-coated harvesting tool.";
                        break;

                        //Common
                        case "PrestigeScroll":
                            item = new PrestigeScroll();
                            rewardText = "You receive a prestige scroll.";
                        break;

                        case "GoldMediumAmount":
                            item = new Gold(Utility.RandomMinMax(500, 1000));
                            rewardText = "You receive a moderate amount of gold.";
                        break;

                        case "CraftingComponentsSmallAmount":
                            item = CraftingComponent.GetRandomCraftingComponent(3);
                            rewardText = "You receive a small number of crafting components.";
                        break;

                        case "Reagents":
                            int reagentsAmount = Utility.RandomMinMax(150, 250);

                            switch(Utility.RandomMinMax(1, 8))
                            {
                                case 1: item = new BlackPearl(reagentsAmount); break;
                                case 2: item = new Bloodmoss(reagentsAmount); break;
                                case 3: item = new MandrakeRoot(reagentsAmount); break;
                                case 4: item = new Garlic(reagentsAmount); break;
                                case 5: item = new Ginseng(reagentsAmount); break;
                                case 6: item = new SpidersSilk(reagentsAmount); break;
                                case 7: item = new SulfurousAsh(reagentsAmount); break;
                                case 8: item = new Nightshade(reagentsAmount); break;
                            }
                          
                            rewardText = "You receive some reagents.";
                        break;

                        //Remainder
                        case "GoldSmallAmount":
                            item = new Gold(Utility.RandomMinMax(250, 500));
                            rewardText = "You receive a small amount of gold.";
                        break;
                    }

                    break;
                }

                CumulativeAmount += AdditionalAmount;
            }
            
            if (item != null)
            {
                if (from.TotalWeight >= 390 && from.AccessLevel == AccessLevel.Player)
                {
                    item.Delete();
                    from.SendMessage("You are carrying too much weight to be able to accept any rewards.");

                    inUse = false;

                    return;
                }

                if (!from.AddToBackpack(item))
                {
                    item.Delete();
                    from.SendMessage("You don't have enough room in your backpack. Please make room and try again.");

                    inUse = false;

                    return;
                }

                else
                {
                    if (from.NetState != null)
                        from.PrivateOverheadMessage(MessageType.Regular, 0, false, rewardText, from.NetState);

                    if (rewardSound != -1)
                        from.SendSound(rewardSound);

                    Delete();
                }
            }

            else            
                inUse = false;            
        }

        public InfluenceLotteryTicket(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);         
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
