using Server.Multis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Engines.Plants;
using Server.Mobiles;
using Server.Misc;
using Server.Network;

namespace Server.Items
{
    class PouchOfGypsyGoods : Item
    {
        public bool inUse = false;

        [Constructable]
        public PouchOfGypsyGoods() : base(0x9B0)
        {
            Name = "a pouch of gyspy goods";
            Hue = 2210;
            Weight = 1;
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (inUse)
                return;

            if (from.InRange(GetWorldLocation(), 1))
            {
                from.SendSound(0x48);
                from.SendMessage("You open the pouch...");

                inUse = true;

                Timer.DelayCall(TimeSpan.FromSeconds(2), delegate
                {
                    if (from == null)
                        return;

                    ReceiveReward(from);
                });
            }

            else
                from.SendMessage("You are too far away to use that.");
        }

        public void ReceiveReward(Mobile from)
        {
            int TotalValues = 0;

            Item item = null;
            string rewardText = "";
            int rewardSound = -1;

            Dictionary<string, int> DictTemp = new Dictionary<string, int>();

            //Used to Determine Chance of Last Reward Item (GoldSmallAmount)
            int maxRandomValue = 10000;

            //Add Rewards (Chance = Value / Sum of All Item Values Combined)     
            DictTemp.Add("GoldMediumAmount", 500);
            DictTemp.Add("GoldLargeAmount", 250);
            
            DictTemp.Add("DragonCoinsSmallAmount", 500);
            DictTemp.Add("DragonCoinsMediumAmount", 250);
            DictTemp.Add("DragonCoinsLargeAmount", 125);
            
            DictTemp.Add("PackOfSeeds", 1000);
            DictTemp.Add("BagOfDirt", 1000);
            
            DictTemp.Add("CounterfeitGold", 50);
            DictTemp.Add("FruitBasket", 50);
            DictTemp.Add("GypsyTears", 50);
            DictTemp.Add("LoadedDice", 50);
            DictTemp.Add("MossyBedroll", 50);
            DictTemp.Add("FallenLogEastAddonDeed", 50);
            DictTemp.Add("FallenLogSouthAddonDeed", 50);
            DictTemp.Add("LargeCampfireAddonDeed", 50);
            DictTemp.Add("LargeGreenCampfireAddonDeed", 50);
            DictTemp.Add("MossyBustEast", 50);
            DictTemp.Add("MossyBustSouth", 50);
            
            DictTemp.Add("WeakMagicWeapon", 200); // Ruin
            DictTemp.Add("MediumMagicWeapon", 100); // Might, Force
            DictTemp.Add("StrongMagicWeapon", 50); // Power, Vanq
            
            DictTemp.Add("WeakMagicArmor", 200); // Defense
            DictTemp.Add("MediumMagicArmor", 100); // Guarding, Hardening
            DictTemp.Add("StrongMagicArmor", 50); // Fortification, Invul
            
            DictTemp.Add("PouchOfGypsyGoods", 500);
            DictTemp.Add("PeacefulSpringGlobe", 50);
            DictTemp.Add("PlantGrowthAccelerant", 50);
            DictTemp.Add("EssenceOfNature", 10);
            DictTemp.Add("DormantSpringSeed", 10);

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
                    BaseWeapon weapon = null;
                    BaseArmor armor = null;

                    switch (pair.Key)
                    {
                        case "GoldSmallAmount":
                            item = new Gold(Utility.RandomMinMax(250, 500));
                            rewardText = "You find a small sum of gold in the pouch.";
                        break;

                        case "GoldMediumAmount":
                            item = new Gold(Utility.RandomMinMax(500, 1000));
                            rewardText = "You find a modest sum of gold in the pouch.";
                        break;

                        case "GoldLargeAmount":
                            item = new Gold(Utility.RandomMinMax(1000, 2500));
                            rewardText = "You find a large sum of gold in the pouch!!";
                        break;

                        case "DragonCoinsSmallAmount":
                            item = new DragonCoin(Utility.RandomMinMax(10, 15));
                            rewardText = "You find a small sum of dragon coins in the pouch.";
                        break;

                        case "DragonCoinsMediumAmount":
                            item = new DragonCoin(Utility.RandomMinMax(15, 25));
                            rewardText = "You find a medium sum of dragon coins in the pouch.";
                        break;

                        case "DragonCoinsLargeAmount":
                            item = new DragonCoin(Utility.RandomMinMax(25, 50));
                            rewardText = "You find a large sum of dragon coins in the pouch!";
                        break;

                        case "PackOfSeeds":
                            item = new PackOfSeeds();
                            rewardText = "You find a pack of seeds inside the pouch.";
                        break;

                        case "BagOfDirt":
                            item = new BagOfDirt();
                            rewardText = "You find a bag of fertile dirt inside the pouch.";
                        break;

                        case "CounterfeitGold":
                            item = new CounterfeitGold();
                            rewardText = "You find a lot of gold inside the pouch! Or did you?";
                        break;

                        case "FruitBasket":
                            item = new FruitBasket();
                            rewardText = "You find a delicious looking fruitbasket stuffed inside the pouch!";
                        break;

                        case "GypsyTears":
                            item = new GypsyTears();
                            rewardText = "You find a flask of gypsy tears inside the pouch. Salty.";
                        break;

                        case "LoadedDice":
                            item = new LoadedDice();
                            rewardText = "You find a pair of dice inside the pouch! Something seems odd about them.";
                        break;

                        case "MossyBedroll":
                            item = new MossyBedRoll();
                            rewardText = "You find a mossy bedroll inside the pouch! ... You're unsure if it would be wise to sleep on it.";
                        break;

                        case "FallenLogEastAddonDeed":
                            item = new FallenLogEastAddonDeed();
                            rewardText = "You find a fallen log house addon deed!";
                        break;

                        case "FallenLogSouthAddonDeed":
                            item = new FallenLogSouthAddonDeed();
                            rewardText = "You find a fallen log house addon deed!";
                        break;

                        case "LargeCampfireAddonDeed":
                            item = new LargeCampfireAddonDeed();
                            rewardText = "You find a large campfire addon deed!";
                        break;

                        case "LargeGreenCampfireAddonDeed":
                            item = new LargeGreenCampfireAddonDeed();
                            rewardText = "You find a large green campfire addon deed!";
                        break;

                        case "MossyBustEast":
                            item = new MossyBustEast();
                            rewardText = "You find a mossy bust inside the pouch. Looks handsome.";
                        break;

                        case "MossyBustSouth":
                            item = new MossyBustSouth();
                            rewardText = "You find a mossy bust inside the pouch. Looks handsome.";
                        break;

                        case "WeakMagicWeapon":
                            weapon = Loot.RandomWeapon();

                            if (weapon != null)
                            {
                                weapon.AccuracyLevel = (WeaponAccuracyLevel)Utility.Random(6);
                                weapon.DamageLevel = WeaponDamageLevel.Ruin;
                                weapon.DurabilityLevel = (WeaponDurabilityLevel)Utility.Random(6);
                            }

                            rewardText = "You find a magic weapon!";
                            item = weapon;
                        break;

                        case "MediumMagicWeapon":
                            weapon = Loot.RandomWeapon();

                            if (weapon != null)
                            {
                                weapon.AccuracyLevel = (WeaponAccuracyLevel)Utility.Random(6);
                                weapon.DamageLevel = (WeaponDamageLevel)Utility.RandomMinMax(2, 3); // Might, Force
                                weapon.DurabilityLevel = (WeaponDurabilityLevel)Utility.Random(6);
                            }

                            rewardText = "You find a magic weapon!";
                            item = weapon;
                        break;

                        case "StrongMagicWeapon":
                            weapon = Loot.RandomWeapon();

                            if (weapon != null)
                            {
                                weapon.AccuracyLevel = (WeaponAccuracyLevel)Utility.Random(6);
                                weapon.DamageLevel = (WeaponDamageLevel)Utility.RandomMinMax(4, 5); // Power, Vanq
                                weapon.DurabilityLevel = (WeaponDurabilityLevel)Utility.Random(6);
                            }

                            rewardText = "You find a magic weapon!";
                            item = weapon;
                        break;

                        case "WeakMagicArmor":
                            armor = Loot.RandomArmor();

                            if (armor != null)
                            {
                                armor.ProtectionLevel = ArmorProtectionLevel.Defense;
                                armor.Durability = (ArmorDurabilityLevel)Utility.Random(6);
                            }

                            rewardText = "You find a magic piece of armor!";
                            item = armor;
                        break;

                        case "MediumMagicArmor":
                            armor = Loot.RandomArmor();

                            if (armor != null)
                            {
                                armor.ProtectionLevel = (ArmorProtectionLevel)Utility.RandomMinMax(2, 3); // Guarding, Hardening
                                armor.Durability = (ArmorDurabilityLevel)Utility.Random(6);
                            }

                            rewardText = "You find a magic piece of armor!";
                            item = armor;
                        break;

                        case "StrongMagicArmor":
                            armor = Loot.RandomArmor();

                            if (armor != null)
                            {
                                armor.ProtectionLevel = (ArmorProtectionLevel)Utility.RandomMinMax(4, 5); // Fortification, Invulnerability
                                armor.Durability = (ArmorDurabilityLevel)Utility.Random(6);
                            }

                            rewardText = "You find a magic piece of armor!";
                            item = armor;
                        break;

                        case "PouchOfGypsyGoods":
                            item = new PouchOfGypsyGoods();
                            rewardText = "You find another pouch inside the pouch. Silly gypsies.";
                        break;

                        case "PeacefulSpringGlobe":
                            item = new PeacefulSpringGlobe();
                            rewardText = "You find a rare globe!";
                        break;

                        case "PlantGrowthAccelerant":
                            item = new PlantGrowthAccelerant();
                            rewardText = "You find a pack of plant growth accelerants!";
                        break;

                        case "EssenceOfNature":
                            item = new EssenceOfNature();
                            rewardText = "You find an essence of nature!";
                        break;

                        case "DormantSpringSeed":
                            item = new DormantSpringSeed();
                            rewardText = "You find a dormant spring seed!";
                        break;
                    }

                    break;
                }

                CumulativeAmount += AdditionalAmount;
            }

            if (item != null)
            {
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

        public PouchOfGypsyGoods(Serial serial) : base(serial) {}

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