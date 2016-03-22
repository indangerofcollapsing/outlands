using System;
using Server;
using Server.Regions;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;
using Server.Multis;
using System.Collections;
using System.Collections.Generic;
using Server.Items;

namespace Server.Custom
{   
    public enum UOACZRewardType
    {
        SurvivalTome, 
        CorruptionTome, 
        UOACZLotteryTicket,
        LargeDecoration,
        SilverWeapon,    
        EpicDecoration        
    }
    
    public static class UOACZRewards
    {
        public static UOACZRewardDetail GetRewardDetail(UOACZRewardType rewardType)
        {
            UOACZRewardDetail rewardDetail = new UOACZRewardDetail();

            switch (rewardType)
            {
                case UOACZRewardType.SurvivalTome:
                    rewardDetail.Name = "Survival Tome";
                    rewardDetail.Description = new string[] { "Receive a Survival Tome to manage your UOACZ Human Profile in the normal game world." };
                    rewardDetail.RewardCost = 0;
                    rewardDetail.ItemId = 8786;
                    rewardDetail.ItemHue = 2599;
                    rewardDetail.OffsetX = 0;
                    rewardDetail.OffsetY = 10;
                break;

                case UOACZRewardType.CorruptionTome:
                    rewardDetail.Name = "Corruption Tome";
                    rewardDetail.Description = new string[] { "Receive a Corruption Tome to manage your UOACZ Undead Profile in the normal game world." };
                    rewardDetail.RewardCost = 0;
                    rewardDetail.ItemId = 8787;
                    rewardDetail.ItemHue = 1104;
                    rewardDetail.OffsetX = 0;
                    rewardDetail.OffsetY = 10;
                break;
                
                case UOACZRewardType.UOACZLotteryTicket:
                    rewardDetail.Name = "UOACZ Lottery Ticket";
                    rewardDetail.Description = new string[] { "Receive a UOACZ Lottery Ticket in the normal game world." };
                    rewardDetail.RewardCost = 1;
                    rewardDetail.ItemId = 0x14ED;
                    rewardDetail.ItemHue = 2408;
                    rewardDetail.OffsetX = -8;
                    rewardDetail.OffsetY = 5;
                break; 
                
                case UOACZRewardType.LargeDecoration:
                    rewardDetail.Name = "Large Decoration";
                    rewardDetail.Description = new string[] { "Receive a randomized large dead tree addon in the normal game world." };
                    rewardDetail.RewardCost = 4;
                    rewardDetail.ItemId = 5360;
                    rewardDetail.ItemHue = 2500;
                    rewardDetail.OffsetX = -8;
                    rewardDetail.OffsetY = 5;
                break;

                case UOACZRewardType.SilverWeapon:
                    rewardDetail.Name = "Random Silver Weapon";
                    rewardDetail.Description = new string[] { "Receive a silver weapon in the normal game world." };
                    rewardDetail.RewardCost = 5;
                    rewardDetail.ItemId = 5049;
                    rewardDetail.ItemHue = 0;
                    rewardDetail.OffsetX = -8;
                    rewardDetail.OffsetY = 5;
                break;        

                case UOACZRewardType.EpicDecoration:
                    rewardDetail.Name = "Epic Decoration";
                    rewardDetail.Description = new string[] { "Receive an interactive large bone pile addon in the normal game world." };
                    rewardDetail.RewardCost = 12;
                    rewardDetail.ItemId = 5360;
                    rewardDetail.ItemHue = 2213;
                    rewardDetail.OffsetX = -8;
                    rewardDetail.OffsetY = 5;
                break;
            }

            return rewardDetail;
        }

        public static void AttemptPurchase(PlayerMobile player, UOACZRewardType rewardType)
        {
            if (player == null) return;
            if (player.Deleted || !player.Alive) return;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            if (UOACZSystem.IsUOACZValidMobile(player))
            {
                player.SendMessage("You may not purchase UOACZ Rewards while participating in a UOACZ session.");
                return;
            }

            UOACZRewardDetail rewardDetail = UOACZRewards.GetRewardDetail(rewardType);

            int rewardPointsAvailable = player.m_UOACZAccountEntry.RewardPoints;

            if (rewardDetail.RewardCost > rewardPointsAvailable && player.AccessLevel == AccessLevel.Player)
            {
                player.SendMessage("You don't have enough UOACZ Reward Points to purchase that.");
                return;
            }

            if (player.Backpack.Items.Count >= player.Backpack.MaxItems && player.AccessLevel == AccessLevel.Player)
            {
                player.SendMessage("You are carrying too many items to receive that item.");
                return;
            }

            bool madePurchase = false;

            switch (rewardType)
            {
                case UOACZRewardType.SurvivalTome:
                    Item[] item = player.Backpack.FindItemsByType(typeof(UOACZSurvivalTome));

                    if (item.Length == 0)
                    {
                        if (player.Backpack.Items.Count < player.Backpack.MaxItems)
                        {
                            if (!player.CanBeginAction(typeof(UOACZSurvivalTome)))
                            {
                                player.SendMessage("You have acquired a Survival Tome too recently to acquire another one and must wait 10 minutes.");
                                return;
                            }

                            player.Backpack.DropItem(new UOACZSurvivalTome());
                            player.SendMessage("You recieve a UOACZ Survival Tome.");

                            madePurchase = true;

                            player.BeginAction(typeof(UOACZSurvivalTome));

                            Timer.DelayCall(TimeSpan.FromMinutes(10), delegate
                            {
                                if (player != null)
                                    player.EndAction(typeof(UOACZSurvivalTome));
                            });
                        }

                        else
                        {
                            player.SendMessage("You are carrying too many items to receive that item.");
                            return;
                        }
                    }

                    else
                    {
                        player.SendMessage("You already have a UOACZ Survival Tome in your backpack.");
                        return;
                    }
                break;

                case UOACZRewardType.CorruptionTome:
                    item = player.Backpack.FindItemsByType(typeof(UOACZCorruptionTome));

                    if (item.Length == 0)
                    {
                        if (player.Backpack.Items.Count < player.Backpack.MaxItems)
                        {
                            if (!player.CanBeginAction(typeof(UOACZCorruptionTome)))
                            {
                                player.SendMessage("You have acquired a Corruption Tome too recently to acquire another one and must wait 10 minutes.");
                                return;
                            }

                            player.Backpack.DropItem(new UOACZCorruptionTome());
                            player.SendMessage("You recieve a UOACZ Corruption Tome.");

                            madePurchase = true;

                            player.BeginAction(typeof(UOACZCorruptionTome));

                            Timer.DelayCall(TimeSpan.FromMinutes(10), delegate
                            {
                                if (player != null)
                                    player.EndAction(typeof(UOACZCorruptionTome));
                            });
                        }

                        else
                        {
                            player.SendMessage("You are carrying too many items to receive that item.");
                            return;
                        }
                    }

                    else
                    {
                        player.SendMessage("You already have a UOACZ Corruption Tome in your backpack.");
                        return;
                    }
                    
                break;

                case UOACZRewardType.SilverWeapon:                    
                    BaseWeapon weapon = Loot.RandomWeapon();

                    if (weapon != null)
                    {
                        int accuracyLevel = 1;
                        int damageLevel = 1;
                        int durabilityLevel = 1;

                        double accuracyResult = Utility.RandomDouble();
                        double damageResult = Utility.RandomDouble();

                        //Accuracy
                        if (accuracyResult <= .50)
                            accuracyLevel = 1;

                        else if (accuracyResult <= .90)
                            accuracyLevel = 2;

                        else if (accuracyResult <= .97)
                            accuracyLevel = 3;

                        else if (accuracyResult <= .99)
                            accuracyLevel = 4;

                        else
                            accuracyLevel = 5;

                        //Damage
                        if (damageResult <= .50)
                            damageLevel = 1;

                        else if (damageResult <= .90)
                            damageLevel = 2;

                        else if (damageResult <= .97)
                            damageLevel = 3;

                        else if (damageResult <= .99)
                            damageLevel = 4;

                        else
                            damageLevel = 5;

                        //Durability
                        durabilityLevel = Utility.RandomMinMax(1, 5);

                        weapon.AccuracyLevel = (WeaponAccuracyLevel)accuracyLevel;
                        weapon.DamageLevel = (WeaponDamageLevel)damageLevel;
                        weapon.DurabilityLevel = (WeaponDurabilityLevel)durabilityLevel;
                        
                        madePurchase = true;

                        player.Backpack.DropItem(weapon);
                        player.SendMessage("You receive a silver weapon.");
                    }
                break;

                case UOACZRewardType.UOACZLotteryTicket:
                    UOACZLotteryTicket lotteryTicket = new UOACZLotteryTicket();

                    if (lotteryTicket == null)
                        return;

                    madePurchase = true;

                    player.Backpack.DropItem(lotteryTicket);
                    player.SendMessage("You receive a UOACZ Lottery Ticket.");
                break;                  

                case UOACZRewardType.LargeDecoration:
                    Item largeItem = null;

                    switch (Utility.RandomMinMax(1, 5))
                    {
                        case 1: largeItem = new UOACZDeadTree1RewardAddonDeed(); break;
                        case 2: largeItem = new UOACZDeadTree2RewardAddonDeed(); break;
                        case 3: largeItem = new UOACZDeadTree3RewardAddonDeed(); break;
                        case 4: largeItem = new UOACZDeadTree4RewardAddonDeed(); break;
                        case 5: largeItem = new UOACZDeadTree5RewardAddonDeed(); break;
                    }

                    if (largeItem == null)
                        return;

                    madePurchase = true;

                    player.Backpack.DropItem(largeItem);
                    player.SendMessage("You receive a large reward item.");                   
                break;

                case UOACZRewardType.EpicDecoration:                    
                    Item epicItem = new UOACZSkullPileRewardAddonDeed();

                    if (epicItem == null)
                        return;

                    madePurchase = true;

                    player.Backpack.DropItem(epicItem);
                    player.SendMessage("You receive an epic reward item.");  
                break;
            }

            if (madePurchase)
            {
                player.SendSound(UOACZSystem.purchaseUpgradeSound);

                if (player.AccessLevel == AccessLevel.Player)
                    player.m_UOACZAccountEntry.RewardPoints -= rewardDetail.RewardCost;
            }
        }
    }

    public class UOACZRewardDetail
    {
        public string Name = "Reward Detail";
        public string[] Description = new string[] { };
        public int RewardCost = 1;
        public int ItemId = 8786;
        public int ItemHue = 2599;
        public int OffsetX = 0;
        public int OffsetY = 0;     
    }    
}