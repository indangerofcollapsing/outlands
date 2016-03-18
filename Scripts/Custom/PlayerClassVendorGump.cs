using System;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Multis;
using Server.Mobiles;
using System.Collections.Generic;
using Server.Network;

namespace Server.Custom.Paladin
{
    public class RansomEntryGump : Gump
    {
        PlayerClassVendor m_Vendor;
        PlayerMobile pm_From;
        Item m_Item;

        public RansomEntryGump(PlayerClassVendor vendor, PlayerMobile player, Item item): base(50, 50)
        {
            if (vendor == null || player == null || item == null) return;
            if (vendor.Deleted || player.Deleted || item.Deleted) return;
            if (item.PlayerClassOwner == null) return;

            m_Vendor = vendor;
            pm_From = player;
            m_Item = item;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;
           
            AddBackground(0, 0, 260, 249, 9200);

            AddPage(0);

            int textHue = 2036;

            AddLabel(57, 10, textHue, @"Issue Offer of Ransom");

            AddItem(101, 63, item.ItemID, item.Hue);

            AddLabel(10, 120, textHue, @"Item:");
            AddLabel(48, 120, textHue, item.Name);

            AddLabel(10, 145, textHue, @"Player:");
            AddLabel(63, 145, textHue, item.PlayerClassOwner.Name);

            AddLabel(10, 170, textHue, @"Gold Demanded:");
            AddImage(110, 170, 2501);
            AddTextEntry(115, 170, 124, 20, 2036, 3, "0", 7);

            AddButton(12, 213, 2151, 2152, 1, GumpButtonType.Reply, 0);
            AddLabel(50, 216, textHue, @"Confirm");

            AddLabel(171, 218, textHue, @"Cancel");
            AddButton(223, 213, 2472, 2473, 2, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            PlayerMobile pm_From = sender.Mobile as PlayerMobile;

            if (pm_From == null || m_Vendor == null) return;
            if (pm_From.Deleted || m_Vendor.Deleted) return;
                        
            switch (info.ButtonID)
            {
                case 0:
                    pm_From.CloseGump(typeof(RansomEntryGump));
                break;

                case 1:
                    PlayerMobile pm_ItemOwner = m_Item.PlayerClassOwner as PlayerMobile;

                    bool goldValid = true;

                    int goldDemanded = 0;

                    TextRelay textRelayGoldAccepted = info.GetTextEntry(3);
                    string textGoldAccepted = textRelayGoldAccepted.Text.Trim();

                    try { goldDemanded = Convert.ToInt32(textGoldAccepted); }

                    catch (Exception e)
                    {
                        goldDemanded = 0;                            
                        goldValid = false;
                    }

                    if (!goldValid || goldDemanded < 0)
                    {
                        sender.Mobile.SendMessage("That is not a valid gold amount.");
                        return;
                    }

                    if (m_Item == null)
                    {
                        sender.Mobile.SendMessage("The item you were attempting to ransom no longer exists.");
                        return;
                    }

                    if (m_Item.Deleted)
                    {
                        sender.Mobile.SendMessage("The item you were attempting to ransom no longer exists.");
                        return;
                    }                        

                    if (pm_ItemOwner == null)
                    {
                        sender.Mobile.SendMessage("The original owner of this item can no longer be found in these lands.");
                        return;
                    }

                    if (pm_ItemOwner.Deleted)
                    {
                        sender.Mobile.SendMessage("The original owner of this item can no longer be found in these lands.");
                        return;
                    }                    

                    if (m_Item.PlayerClass == null)
                    {
                        sender.Mobile.SendMessage("That particular cannot be ransomed.");
                        return;
                    }

                    if (m_Item.PlayerClass == PlayerClass.None)
                    {
                        sender.Mobile.SendMessage("That particular cannot be ransomed.");
                        return;
                    }

                    if (m_Item is BaseWeapon)
                    {
                        sender.Mobile.SendMessage("Dyed weapons cannot be ransomed.");
                        return;
                    }

                    if (m_Item is Spellbook)
                    {
                        sender.Mobile.SendMessage("Dyed spellbooks cannot be ransomed.");
                        return;
                    }

                    if (m_Item.RootParentEntity != pm_From)
                    {
                        sender.Mobile.SendMessage("The item you were attempting to ransom is no longer in your pack.");
                        return;
                    }

                    if (pm_From.GetDistanceToSqrt(m_Vendor.Location) > 10)
                    {
                        sender.Mobile.SendMessage("You are too far away from the vendor to continue with the ransom.");
                        return;
                    }

                    if (!pm_From.Alive)                        
                        return;

                    pm_ItemOwner.m_PlayerClassRansomedItemsAvailable.Add(new PlayerClassItemRansomEntry(pm_From, m_Item.GetType(), goldDemanded));

                    m_Vendor.Say("I shall extend an offer on this item to it's owner.");
                    pm_ItemOwner.SendMessage("A ransom offer on one of your " + m_Item.PlayerClass.ToString() + " items has been made.");

                    m_Item.Delete();                                          
                break;

                case 2:
                    pm_From.CloseGump(typeof(RansomEntryGump));
                break;
            }
        }
    }

    public class PlayerClassVendorGump : Gump
    {
        public PlayerClassVendor m_Vendor;

        public int m_PageNo = 0;
        public int m_TotalPages = 0;
        
        public PlayerClass m_PlayerClassType;

        public Type m_CurrencyType;
        public int m_CurrencyItemId;
        public int m_CurrencyItemHue;

        public int m_BookBackgroundId;

        public List<PlayerClassVendor.PlayerClassItemVendorEntry> m_VendorItems = new List<PlayerClassVendor.PlayerClassItemVendorEntry>();

        public PlayerClassVendorGump(PlayerClassVendor vendor, int PageNo, int bookBackgroundId, PlayerClass playerClassType, Type currencyType, int currencyItemId, int currencyItemHue, List<PlayerClassVendor.PlayerClassItemVendorEntry> vendorItemEntries): base(0, 0)
        {
            m_Vendor = vendor;

            m_PageNo = PageNo;

            m_BookBackgroundId = bookBackgroundId;
            
            m_PlayerClassType = playerClassType;

            m_CurrencyType = currencyType;  
            m_CurrencyItemId = currencyItemId;
            m_CurrencyItemHue = currencyItemHue;
        
            m_VendorItems = vendorItemEntries; 

            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;
            this.AddPage(0);
            this.AddImage(0, 0, m_BookBackgroundId);

            m_TotalPages = (int)(Math.Ceiling((double)m_VendorItems.Count / 2));

            int arrayIndex = (PageNo * 2) - 2;

            //Left Button
            if (m_PageNo > 1)
                AddButton(47, 9, 2205, 2205, (int)Buttons.btnPrev, GumpButtonType.Reply, 0);                

            //Right Button
            if (PageNo < m_TotalPages)
                AddButton(321, 9, 2206, 2206, (int)Buttons.btnNext, GumpButtonType.Reply, 0); 

            Item currency = (Item)Activator.CreateInstance(m_CurrencyType);            

            //Left Item
            if (arrayIndex <= (m_VendorItems.Count - 1))
            {
                PlayerClassVendor.PlayerClassItemVendorEntry leftItem = m_VendorItems[arrayIndex];

                this.AddHtml(52, 30, 145, 44, "<center>" + leftItem.m_Name + "</center>", (bool)false, (bool)false);

                this.AddItem(100, 61, leftItem.m_ItemID, leftItem.m_ItemHue);

                if (leftItem.m_IsRansomItem)
                {
                    this.AddItem(70, 120, PlayerClassPersistance.NoClassCurrencyItemId, PlayerClassPersistance.NoClassCurrencyItemHue);
                    this.AddLabel(115, 124, 0x0, leftItem.m_Cost.ToString());

                    this.AddButton(60, 160, 2153, 2154, (int)Buttons.btnPurchaseLeftItem, GumpButtonType.Reply, 0);
                    this.AddButton(110, 160, 9721, 9722, (int)Buttons.btnDeclineLeftItem, GumpButtonType.Reply, 0);
                    this.AddButton(160, 160, 2472, 2473, (int)Buttons.btnDeleteLeftItem, GumpButtonType.Reply, 0);                    
                }

                else
                {
                    this.AddItem(70, 120, currencyItemId, currencyItemHue);
                    this.AddLabel(115, 124, 0x0, leftItem.m_Cost.ToString());

                    this.AddButton(78, 160, 2153, 2154, (int)Buttons.btnPurchaseLeftItem, GumpButtonType.Reply, 0);
                    this.AddLabel(115, 165, 0x0, @"Purchase");
                }
            }

            //Right Item
            if ((arrayIndex + 1) <= (m_VendorItems.Count - 1))
            {
                PlayerClassVendor.PlayerClassItemVendorEntry rightItem = m_VendorItems[arrayIndex + 1];

                this.AddHtml(208, 30, 145, 44, "<center>" + rightItem.m_Name + "</center>", (bool)false, (bool)false);

                this.AddItem(256, 61, rightItem.m_ItemID, rightItem.m_ItemHue);
                
                if (rightItem.m_IsRansomItem)
                {
                    this.AddItem(230, 120, PlayerClassPersistance.NoClassCurrencyItemId, PlayerClassPersistance.NoClassCurrencyItemHue);
                    this.AddLabel(275, 124, 0x0, rightItem.m_Cost.ToString());

                    this.AddButton(220, 160, 2153, 2154, (int)Buttons.btnPurchaseRightItem, GumpButtonType.Reply, 0);
                    this.AddButton(270, 160, 9721, 9722, (int)Buttons.btnDeclineRightItem, GumpButtonType.Reply, 0);
                    this.AddButton(320, 160, 2472, 2473, (int)Buttons.btnDeleteRightItem, GumpButtonType.Reply, 0);     
                }

                else
                {
                    this.AddItem(230, 120, currencyItemId, currencyItemHue);
                    this.AddLabel(275, 124, 0x0, rightItem.m_Cost.ToString());

                    this.AddButton(238, 160, 2154, 2154, (int)Buttons.btnPurchaseRightItem, GumpButtonType.Reply, 0);
                    this.AddLabel(275, 165, 0x0, @"Purchase");
                }
            }

            currency.Delete();
        } 

        public enum Buttons
        {
            btnCancel,
            btnNext,
            btnPrev, 
            btnPurchaseLeftItem,
            btnDeclineLeftItem,
            btnDeleteLeftItem,
            btnPurchaseRightItem,
            btnDeclineRightItem,
            btnDeleteRightItem,                       
        }

        public override void OnResponse(Network.NetState sender, RelayInfo info)
        {
            base.OnResponse(sender, info);

            PlayerMobile pm_From = sender.Mobile as PlayerMobile;

            if (pm_From == null)
                return;

            int arrayIndex = 0;
            int playerClassRansomedItemsAvailable = 0;

            PlayerClassVendor.PlayerClassItemVendorEntry entry;

            switch (info.ButtonID)
            {
                case (int)Buttons.btnCancel:
                    pm_From.CloseGump(typeof(PlayerClassVendorGump));
                break;

                case (int)Buttons.btnNext:
                    {
                        sender.Mobile.CloseAllGumps();

                        sender.Mobile.SendSound(0x055);

                        if (m_PageNo == m_TotalPages)
                            sender.Mobile.SendGump(new PlayerClassVendorGump(m_Vendor, m_PageNo, m_BookBackgroundId, m_PlayerClassType, m_CurrencyType, m_CurrencyItemId, m_CurrencyItemHue, m_VendorItems));
                        else
                            sender.Mobile.SendGump(new PlayerClassVendorGump(m_Vendor, ++m_PageNo, m_BookBackgroundId, m_PlayerClassType, m_CurrencyType, m_CurrencyItemId, m_CurrencyItemHue, m_VendorItems));
                    }
                break;

                case (int)Buttons.btnPrev:
                    {
                        sender.Mobile.CloseAllGumps();

                        sender.Mobile.SendSound(0x055);

                        if (m_PageNo == 1)
                            sender.Mobile.SendGump(new PlayerClassVendorGump(m_Vendor, m_PageNo, m_BookBackgroundId, m_PlayerClassType, m_CurrencyType, m_CurrencyItemId, m_CurrencyItemHue, m_VendorItems));
                        else
                            sender.Mobile.SendGump(new PlayerClassVendorGump(m_Vendor, --m_PageNo, m_BookBackgroundId, m_PlayerClassType, m_CurrencyType, m_CurrencyItemId, m_CurrencyItemHue, m_VendorItems));
                    }

                break;

                case (int)Buttons.btnPurchaseLeftItem:
                    arrayIndex = (m_PageNo * 2) - 2;

                    if (arrayIndex < m_VendorItems.Count && m_VendorItems.Count > 0)
                        entry = m_VendorItems[arrayIndex];
                    else
                        return;

                    if (entry == null)
                        return;

                    if (entry.m_IsRansomItem)
                    {
                        playerClassRansomedItemsAvailable = pm_From.m_PlayerClassRansomedItemsAvailable.Count;

                        for (int a = 0; a < playerClassRansomedItemsAvailable; a++)
                        {
                            if (a >= pm_From.m_PlayerClassRansomedItemsAvailable.Count)
                                continue;

                            PlayerClassItemRansomEntry ransomEntry = pm_From.m_PlayerClassRansomedItemsAvailable[a];

                            if (ransomEntry == null)
                                continue;

                            if (ransomEntry.m_ItemType == entry.m_Type && ransomEntry.m_GoldCost == entry.m_Cost)
                            {
                                BankBox playerBank = pm_From.FindBankNoCreate();
                                bool validPlayerBank = true;

                                if (playerBank == null)
                                    validPlayerBank = false;

                                else
                                {
                                    if (Banker.GetBalance(pm_From) < ransomEntry.m_GoldCost)
                                        validPlayerBank = false;
                                }

                                if (!validPlayerBank)
                                {
                                    pm_From.SendMessage("You do not have the neccessary gold in your bankbox to reacquire that item.");
                                    break;
                                }

                                if (pm_From.Backpack != null)
                                {
                                    if (pm_From.Backpack.Items.Count >= pm_From.Backpack.MaxItems)
                                    {
                                        pm_From.SendMessage("You have too many items in your backpack to reacquire that item.");
                                        break;
                                    }
                                }

                                PlayerMobile ransomer = ransomEntry.m_Ransomer as PlayerMobile;

                                bool ransomerExists = true;

                                if (ransomer == null)
                                    ransomerExists = false;

                                else
                                {
                                    if (ransomer.Deleted)
                                        ransomerExists = false;
                                }

                                if (ransomerExists)
                                {
                                    BankBox ransomerBank = ransomer.FindBankNoCreate();
                                    bool validRansomerBank = true;

                                    if (ransomerBank == null)
                                        validRansomerBank = false;

                                    else
                                    {
                                        if (ransomerBank.Items.Count == ransomerBank.MaxItems)
                                        {
                                            int bankBalance = Banker.GetBalance(ransomer);

                                            if ((ransomEntry.m_GoldCost + bankBalance) > 60000)
                                                validRansomerBank = false;
                                        }
                                    }

                                    if (validRansomerBank)
                                    {
                                        if (Banker.Withdraw(pm_From, ransomEntry.m_GoldCost) && Banker.Deposit(ransomer, ransomEntry.m_GoldCost))
                                        {
                                            Item item = (Item)Activator.CreateInstance(ransomEntry.m_ItemType);

                                            if (item is BaseWeapon || item is Spellbook)
                                            {
                                                item.PlayerClass = m_Vendor.m_PlayerClass;
                                                item.PlayerClassOwner = pm_From;
                                                item.PlayerClassRestricted = true;

                                                switch (item.PlayerClass)
                                                {
                                                    case PlayerClass.Paladin: item.Hue = PlayerClassPersistance.PaladinItemHue; break;
                                                    case PlayerClass.Murderer: item.Hue = PlayerClassPersistance.MurdererItemHue; break;
                                                    case PlayerClass.Pirate: item.Hue = PlayerClassPersistance.PirateItemHue; break;
                                                }
                                            }

                                            else
                                                item.PlayerClassOwner = pm_From;

                                            pm_From.AddToBackpack(item);

                                            Gold goldPile = new Gold(ransomEntry.m_GoldCost);
                                            int currencySound = goldPile.GetDropSound();
                                            goldPile.Delete();

                                            pm_From.PlaySound(currencySound);

                                            pm_From.m_PlayerClassRansomedItemsAvailable.Remove(ransomEntry);

                                            pm_From.SendMessage("You pay the ransom and reacquire the item.");
                                            ransomer.SendMessage(pm_From.Name + " has paid " + ransomEntry.m_GoldCost.ToString() + " gold in ransom to you for an item. The gold has been placed in your bankbox.");

                                            pm_From.CloseGump(typeof(PlayerClassVendorGump));
                                            pm_From.SendGump(new PlayerClassVendorGump(m_Vendor, 1, m_Vendor.m_GumpBackgroundId, m_Vendor.m_PlayerClass, m_Vendor.m_CurrencyType, m_Vendor.m_CurrencyItemId, m_Vendor.m_CurrencyItemHue, m_Vendor.GetVendorItems(pm_From)));
                                        }
                                    }

                                    else
                                    {
                                        pm_From.SendMessage("The player offering this item does not have enough space in their bankbox to receive the gold offered.");
                                        break;
                                    }
                                }

                                else
                                {                                   
                                    if (Banker.Withdraw(pm_From, ransomEntry.m_GoldCost))
                                    {
                                        Item item = (Item)Activator.CreateInstance(ransomEntry.m_ItemType);

                                        if (item is BaseWeapon || item is Spellbook)
                                        {
                                            item.PlayerClass = m_Vendor.m_PlayerClass;
                                            item.PlayerClassOwner = pm_From;
                                            item.PlayerClassRestricted = true;

                                            switch (item.PlayerClass)
                                            {
                                                case PlayerClass.Paladin: item.Hue = PlayerClassPersistance.PaladinItemHue; break;
                                                case PlayerClass.Murderer: item.Hue = PlayerClassPersistance.MurdererItemHue; break;
                                                case PlayerClass.Pirate: item.Hue = PlayerClassPersistance.PirateItemHue; break;
                                            }
                                        }

                                        else
                                            item.PlayerClassOwner = pm_From;

                                        pm_From.AddToBackpack(item);                                                                                

                                        Gold goldPile = new Gold(ransomEntry.m_GoldCost);
                                        int currencySound = goldPile.GetDropSound();
                                        goldPile.Delete();

                                        pm_From.PlaySound(currencySound);

                                        pm_From.SendMessage("You pay the ransom and reacquire the item.");

                                        pm_From.m_PlayerClassRansomedItemsAvailable.Remove(ransomEntry);

                                        pm_From.CloseGump(typeof(PlayerClassVendorGump));
                                        pm_From.SendGump(new PlayerClassVendorGump(m_Vendor, 1, m_Vendor.m_GumpBackgroundId, m_Vendor.m_PlayerClass, m_Vendor.m_CurrencyType, m_Vendor.m_CurrencyItemId, m_Vendor.m_CurrencyItemHue, m_Vendor.GetVendorItems(pm_From)));
                                    }
                                }

                                break;
                            }
                        }
                    }

                    else
                    {
                        if (TryConsumeResources(sender.Mobile, m_CurrencyType, entry.m_Cost))
                        {
                            Item item = (Item)Activator.CreateInstance(entry.m_Type);

                            item.PlayerClass = m_PlayerClassType;
                            item.PlayerClassOwner = pm_From;

                            pm_From.AddToBackpack(item);
                        }

                        sender.Mobile.SendGump(new PlayerClassVendorGump(m_Vendor, m_PageNo, m_BookBackgroundId, m_PlayerClassType, m_CurrencyType, m_CurrencyItemId, m_CurrencyItemHue, m_VendorItems));
                    }
                break;

                case (int)Buttons.btnPurchaseRightItem:
                    arrayIndex = (m_PageNo * 2) - 1;

                    if (arrayIndex < m_VendorItems.Count && m_VendorItems.Count > 0)
                        entry = m_VendorItems[arrayIndex];
                    else
                        return;

                    if (entry == null)
                        return;

                    if (entry.m_IsRansomItem)
                    {
                        playerClassRansomedItemsAvailable = pm_From.m_PlayerClassRansomedItemsAvailable.Count;

                        for (int a = 0; a < playerClassRansomedItemsAvailable; a++)
                        {
                            if (a >= pm_From.m_PlayerClassRansomedItemsAvailable.Count)
                                continue;

                            PlayerClassItemRansomEntry ransomEntry = pm_From.m_PlayerClassRansomedItemsAvailable[a];

                            if (ransomEntry == null)
                                continue;

                            if (ransomEntry.m_ItemType == entry.m_Type && ransomEntry.m_GoldCost == entry.m_Cost)
                            {
                                BankBox playerBank = pm_From.FindBankNoCreate();
                                bool validPlayerBank = true;

                                if (playerBank == null)
                                        validPlayerBank = false;

                                else
                                {
                                    if (Banker.GetBalance(pm_From) < ransomEntry.m_GoldCost)
                                        validPlayerBank = false;
                                }

                                if (!validPlayerBank)
                                {
                                    pm_From.SendMessage("You do not have the neccessary gold in your bankbox to reacquire that item."); 
                                    break;
                                }

                                if (pm_From.Backpack != null)
                                {
                                    if (pm_From.Backpack.Items.Count >= pm_From.Backpack.MaxItems)
                                    {
                                        pm_From.SendMessage("You have too many items in your backpack to reacquire that item.");
                                        break;
                                    }
                                }

                                PlayerMobile ransomer = ransomEntry.m_Ransomer as PlayerMobile;

                                bool ransomerExists = true;

                                if (ransomer == null)
                                    ransomerExists = false;

                                else
                                {
                                    if (ransomer.Deleted)
                                        ransomerExists = false;
                                }

                                if (ransomerExists)
                                {                                    
                                    BankBox ransomerBank = ransomer.FindBankNoCreate();                                                                       
                                    bool validRansomerBank = true;                                   

                                    if (ransomerBank == null)
                                        validRansomerBank = false;

                                    else
                                    {
                                        if (ransomerBank.Items.Count == ransomerBank.MaxItems)
                                        {
                                            int bankBalance = Banker.GetBalance(ransomer);

                                            if ((ransomEntry.m_GoldCost + bankBalance) > 60000)
                                                validRansomerBank = false;
                                        }
                                    }                                    

                                    if (validRansomerBank)
                                    {
                                        if (Banker.Withdraw(pm_From, ransomEntry.m_GoldCost) && Banker.Deposit(ransomer, ransomEntry.m_GoldCost))
                                        {
                                            Item item = (Item)Activator.CreateInstance(ransomEntry.m_ItemType);

                                            if (item is BaseWeapon || item is Spellbook)
                                            {
                                                item.PlayerClass = m_Vendor.m_PlayerClass;
                                                item.PlayerClassOwner = pm_From;
                                                item.PlayerClassRestricted = true;

                                                switch (item.PlayerClass)
                                                {
                                                    case PlayerClass.Paladin: item.Hue = PlayerClassPersistance.PaladinItemHue; break;
                                                    case PlayerClass.Murderer: item.Hue = PlayerClassPersistance.MurdererItemHue; break;
                                                    case PlayerClass.Pirate: item.Hue = PlayerClassPersistance.PirateItemHue; break;
                                                }
                                            }

                                            else
                                                item.PlayerClassOwner = pm_From;

                                            pm_From.AddToBackpack(item);

                                            pm_From.m_PlayerClassRansomedItemsAvailable.Remove(ransomEntry);

                                            pm_From.SendMessage("You pay the ransom and reacquire the item.");
                                            ransomer.SendMessage(pm_From.Name + " has paid " + ransomEntry.m_GoldCost.ToString() + " gold in ransom to you for an item. The gold has been placed in your bankbox.");

                                            pm_From.CloseGump(typeof(PlayerClassVendorGump));
                                            pm_From.SendGump(new PlayerClassVendorGump(m_Vendor, 1, m_Vendor.m_GumpBackgroundId, m_Vendor.m_PlayerClass, m_Vendor.m_CurrencyType, m_Vendor.m_CurrencyItemId, m_Vendor.m_CurrencyItemHue, m_Vendor.GetVendorItems(pm_From)));
                                        }
                                    }

                                    else
                                    {
                                        pm_From.SendMessage("The player offering this item does not have enough space in their bankbox to receive the gold offered.");
                                        break;
                                    }
                                }

                                else
                                {
                                    if (Banker.Withdraw(pm_From, ransomEntry.m_GoldCost))
                                    {
                                        Item item = (Item)Activator.CreateInstance(ransomEntry.m_ItemType);

                                        if (item is BaseWeapon || item is Spellbook)
                                        {
                                            item.PlayerClass = m_Vendor.m_PlayerClass;
                                            item.PlayerClassOwner = pm_From;
                                            item.PlayerClassRestricted = true;

                                            switch (item.PlayerClass)
                                            {
                                                case PlayerClass.Paladin: item.Hue = PlayerClassPersistance.PaladinItemHue; break;
                                                case PlayerClass.Murderer: item.Hue = PlayerClassPersistance.MurdererItemHue; break;
                                                case PlayerClass.Pirate: item.Hue = PlayerClassPersistance.PirateItemHue; break;
                                            }
                                        }

                                        else
                                            item.PlayerClassOwner = pm_From;

                                        pm_From.AddToBackpack(item);

                                        pm_From.m_PlayerClassRansomedItemsAvailable.Remove(ransomEntry);

                                        pm_From.SendMessage("You pay the ransom and reacquire the item.");

                                        pm_From.CloseGump(typeof(PlayerClassVendorGump));
                                        pm_From.SendGump(new PlayerClassVendorGump(m_Vendor, 1, m_Vendor.m_GumpBackgroundId, m_Vendor.m_PlayerClass, m_Vendor.m_CurrencyType, m_Vendor.m_CurrencyItemId, m_Vendor.m_CurrencyItemHue, m_Vendor.GetVendorItems(pm_From)));
                                    }
                                }

                                break;
                            }
                        }
                    }

                    else
                    {
                        if (TryConsumeResources(sender.Mobile, m_CurrencyType, entry.m_Cost))
                        {
                            Item item = (Item)Activator.CreateInstance(entry.m_Type);

                            item.PlayerClass = m_PlayerClassType;
                            item.PlayerClassOwner = pm_From;

                            pm_From.AddToBackpack(item);
                        }

                        sender.Mobile.SendGump(new PlayerClassVendorGump(m_Vendor, m_PageNo, m_BookBackgroundId, m_PlayerClassType, m_CurrencyType, m_CurrencyItemId, m_CurrencyItemHue, m_VendorItems));
                    }
                break;

                case (int)Buttons.btnDeclineLeftItem:
                    arrayIndex = (m_PageNo * 2) - 2;

                    if (arrayIndex < m_VendorItems.Count && m_VendorItems.Count > 0)
                        entry = m_VendorItems[arrayIndex];
                    else
                        return;

                    playerClassRansomedItemsAvailable = pm_From.m_PlayerClassRansomedItemsAvailable.Count;

                    for (int a = 0; a < playerClassRansomedItemsAvailable; a++)
                    {
                        if (a >= pm_From.m_PlayerClassRansomedItemsAvailable.Count)
                            continue;

                        PlayerClassItemRansomEntry ransomEntry = pm_From.m_PlayerClassRansomedItemsAvailable[a];

                        if (ransomEntry == null)
                            continue;
                                               
                        if (ransomEntry.m_ItemType == entry.m_Type && ransomEntry.m_GoldCost == entry.m_Cost)
                        {
                            PlayerMobile ransomer = ransomEntry.m_Ransomer as PlayerMobile;

                            bool ransomerExists = true;

                            if (ransomer == null)
                                ransomerExists = false;

                            else
                            {
                                if (ransomer.Deleted)
                                    ransomerExists = false;
                            }

                            if (ransomerExists)
                            {
                                BankBox ransomerBank = ransomer.FindBankNoCreate();

                                bool validRansomerBank = true;

                                if (ransomerBank == null)
                                    validRansomerBank = false;

                                else
                                {
                                    if (ransomerBank.Items.Count >= ransomerBank.MaxItems)
                                        validRansomerBank = false;
                                }

                                if (validRansomerBank)
                                {
                                    Item item = (Item)Activator.CreateInstance(ransomEntry.m_ItemType);

                                    if (item is BaseWeapon || item is Spellbook)
                                    {
                                        item.PlayerClass = m_Vendor.m_PlayerClass;
                                        item.PlayerClassOwner = pm_From;
                                        item.PlayerClassRestricted = true;

                                        switch (item.PlayerClass)
                                        {
                                            case PlayerClass.Paladin: item.Hue = PlayerClassPersistance.PaladinItemHue; break;
                                            case PlayerClass.Murderer: item.Hue = PlayerClassPersistance.MurdererItemHue; break;
                                            case PlayerClass.Pirate: item.Hue = PlayerClassPersistance.PirateItemHue; break;
                                        }
                                    }

                                    else
                                        item.PlayerClassOwner = pm_From;

                                    ransomerBank.AddItem(item);

                                    pm_From.m_PlayerClassRansomedItemsAvailable.Remove(ransomEntry);
                                    pm_From.SendMessage("You decline the offer.");

                                    ransomer.SendMessage("A player has declined an offer on one of your ransomed items and the item has been placed in your bankbox.");

                                    pm_From.CloseGump(typeof(PlayerClassVendorGump));
                                    pm_From.SendGump(new PlayerClassVendorGump(m_Vendor, 1, m_Vendor.m_GumpBackgroundId, m_Vendor.m_PlayerClass, m_Vendor.m_CurrencyType, m_Vendor.m_CurrencyItemId, m_Vendor.m_CurrencyItemHue, m_Vendor.GetVendorItems(pm_From)));
                                }

                                else                                
                                    pm_From.SendMessage("You decline the offer, however the player offering the item currently does not have space in their bankbox to receive the item.");                                
                            }

                            else
                            {
                                pm_From.m_PlayerClassRansomedItemsAvailable.Remove(ransomEntry);
                                pm_From.SendMessage("You decline the offer.");

                                pm_From.CloseGump(typeof(PlayerClassVendorGump));
                                pm_From.SendGump(new PlayerClassVendorGump(m_Vendor, 1, m_Vendor.m_GumpBackgroundId, m_Vendor.m_PlayerClass, m_Vendor.m_CurrencyType, m_Vendor.m_CurrencyItemId, m_Vendor.m_CurrencyItemHue, m_Vendor.GetVendorItems(pm_From)));
                            }

                            break;
                        }
                    }                    
                break;

                case (int)Buttons.btnDeclineRightItem:
                    arrayIndex = (m_PageNo * 2) - 1;

                    if (arrayIndex < m_VendorItems.Count && m_VendorItems.Count > 0)
                        entry = m_VendorItems[arrayIndex];
                    else
                        return;

                    playerClassRansomedItemsAvailable = pm_From.m_PlayerClassRansomedItemsAvailable.Count;

                    for (int a = 0; a < playerClassRansomedItemsAvailable; a++)
                    {
                        if (a > pm_From.m_PlayerClassRansomedItemsAvailable.Count)
                            continue;

                        PlayerClassItemRansomEntry ransomEntry = pm_From.m_PlayerClassRansomedItemsAvailable[a];

                        if (ransomEntry == null)
                            continue;

                        if (ransomEntry.m_ItemType == entry.m_Type && ransomEntry.m_GoldCost == entry.m_Cost)
                        {
                            PlayerMobile ransomer = ransomEntry.m_Ransomer as PlayerMobile;

                            bool ransomerExists = true;

                            if (ransomer == null)
                                ransomerExists = false;

                            else
                            {
                                if (ransomer.Deleted)
                                    ransomerExists = false;
                            }

                            if (ransomerExists)
                            {
                                BankBox ransomerBank = ransomer.FindBankNoCreate();

                                bool validRansomerBank = true;

                                if (ransomerBank == null)
                                    validRansomerBank = false;

                                else
                                {
                                    if (ransomerBank.Items.Count >= ransomerBank.MaxItems)
                                        validRansomerBank = false;
                                }

                                if (validRansomerBank)
                                {
                                    Item item = (Item)Activator.CreateInstance(ransomEntry.m_ItemType);

                                    if (item is BaseWeapon || item is Spellbook)
                                    {
                                        item.PlayerClass = m_Vendor.m_PlayerClass;
                                        item.PlayerClassOwner = pm_From;
                                        item.PlayerClassRestricted = true;

                                        switch(item.PlayerClass)
                                        {
                                            case PlayerClass.Paladin: item.Hue = PlayerClassPersistance.PaladinItemHue; break;
                                            case PlayerClass.Murderer: item.Hue = PlayerClassPersistance.MurdererItemHue; break;
                                            case PlayerClass.Pirate: item.Hue = PlayerClassPersistance.PirateItemHue; break;
                                        }
                                    }

                                    else
                                        item.PlayerClassOwner = pm_From;

                                    ransomerBank.AddItem(item);

                                    pm_From.m_PlayerClassRansomedItemsAvailable.Remove(ransomEntry);
                                    pm_From.SendMessage("You decline the offer.");

                                    ransomer.SendMessage("A player has declined an offer on one of your ransomed items and the item has been placed in your bankbox.");

                                    pm_From.CloseGump(typeof(PlayerClassVendorGump));
                                    pm_From.SendGump(new PlayerClassVendorGump(m_Vendor, 1, m_Vendor.m_GumpBackgroundId, m_Vendor.m_PlayerClass, m_Vendor.m_CurrencyType, m_Vendor.m_CurrencyItemId, m_Vendor.m_CurrencyItemHue, m_Vendor.GetVendorItems(pm_From)));
                                }

                                else
                                    pm_From.SendMessage("You decline the offer, however the player offering the item currently does not have space in their bankbox to receive the item.");
                            }

                            else
                            {
                                pm_From.m_PlayerClassRansomedItemsAvailable.Remove(ransomEntry);
                                pm_From.SendMessage("You decline the offer.");

                                pm_From.CloseGump(typeof(PlayerClassVendorGump));
                                pm_From.SendGump(new PlayerClassVendorGump(m_Vendor, 1, m_Vendor.m_GumpBackgroundId, m_Vendor.m_PlayerClass, m_Vendor.m_CurrencyType, m_Vendor.m_CurrencyItemId, m_Vendor.m_CurrencyItemHue, m_Vendor.GetVendorItems(pm_From)));
                            }

                            break;
                        }
                    }
                break;

                case (int)Buttons.btnDeleteLeftItem:
                    arrayIndex = (m_PageNo * 2) - 2;

                    if (arrayIndex < m_VendorItems.Count && m_VendorItems.Count > 0)
                        entry = m_VendorItems[arrayIndex];
                    else
                        return;

                    playerClassRansomedItemsAvailable = pm_From.m_PlayerClassRansomedItemsAvailable.Count;

                    for (int a = 0; a < playerClassRansomedItemsAvailable; a++)
                    {
                        if (a >= pm_From.m_PlayerClassRansomedItemsAvailable.Count)
                            continue;

                        PlayerClassItemRansomEntry ransomEntry = pm_From.m_PlayerClassRansomedItemsAvailable[a];

                        if (ransomEntry == null)
                            continue;

                        if (ransomEntry.m_ItemType == entry.m_Type && ransomEntry.m_GoldCost == entry.m_Cost)
                        {
                            pm_From.m_PlayerClassRansomedItemsAvailable.Remove(ransomEntry);                            
                            pm_From.SendMessage("You decide that you will make no future attempts to repurchase that item.");

                            pm_From.CloseGump(typeof(PlayerClassVendorGump));
                            pm_From.SendGump(new PlayerClassVendorGump(m_Vendor, 1, m_Vendor.m_GumpBackgroundId, m_Vendor.m_PlayerClass, m_Vendor.m_CurrencyType, m_Vendor.m_CurrencyItemId, m_Vendor.m_CurrencyItemHue, m_Vendor.GetVendorItems(pm_From)));

                            break;
                        }
                    }                    
                break;

                case (int)Buttons.btnDeleteRightItem:
                    arrayIndex = (m_PageNo * 2) - 1;

                    if (arrayIndex < m_VendorItems.Count && m_VendorItems.Count > 0)
                        entry = m_VendorItems[arrayIndex];
                    else
                        return;

                    playerClassRansomedItemsAvailable = pm_From.m_PlayerClassRansomedItemsAvailable.Count;

                    for (int a = 0; a < pm_From.m_PlayerClassRansomedItemsAvailable.Count; a++)
                    {
                        if (a >= pm_From.m_PlayerClassRansomedItemsAvailable.Count)
                            continue;

                        PlayerClassItemRansomEntry ransomEntry = pm_From.m_PlayerClassRansomedItemsAvailable[a];

                        if (ransomEntry == null)
                            continue;

                        if (ransomEntry.m_ItemType == entry.m_Type && ransomEntry.m_GoldCost == entry.m_Cost)
                        {
                            pm_From.m_PlayerClassRansomedItemsAvailable.Remove(ransomEntry);
                            pm_From.SendMessage("You decide that you will make no future attempts to repurchase that item.");

                            pm_From.CloseGump(typeof(PlayerClassVendorGump));
                            pm_From.SendGump(new PlayerClassVendorGump(m_Vendor, 1, m_Vendor.m_GumpBackgroundId, m_Vendor.m_PlayerClass, m_Vendor.m_CurrencyType, m_Vendor.m_CurrencyItemId, m_Vendor.m_CurrencyItemHue, m_Vendor.GetVendorItems(pm_From)));

                            break;
                        }
                    }
                break;

                default:
                    pm_From.CloseGump(typeof(PlayerClassVendorGump));
                break;
            }
        }

        public bool TryConsumeResources(Mobile from, Type currencyType, int cost)
        {
            Item currency = (Item)Activator.CreateInstance(m_CurrencyType);
            
            string currencyName = currency.Name;
            int currencySound = currency.GetDropSound();
            currency.Delete();

            if (Banker.WithdrawUniqueCurrency(from, currencyType, cost))
            {
                from.PlaySound(currencySound);
                from.SendMessage("You purchase the item");    
           
                return true;
            }

            else
                from.SendMessage("You do not have the required amount of " + currencyName + " to purchase this item.");            

            return false;
        }
    }
}