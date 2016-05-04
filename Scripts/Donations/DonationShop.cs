using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Server;
using Server.Items;
using Server.Commands;
using Server.Mobiles;
using Server.Gumps;

namespace Server
{
    public static class DonationShop
    {
        public static string DonationCurrencyName = "Platinum";
        public static Type DonationCurrencyType = typeof(DragonCoin);        

        public static List<DonationCategory> DonationCategories = new List<DonationCategory>();
        public static Dictionary<DonationCategory, List<DonationItem>> DonationShopList = new Dictionary<DonationCategory,List<DonationItem>>()
        {              
        };

        public static void Initialize()
        {
            CommandSystem.Register("DonationShop", AccessLevel.Player, new CommandEventHandler(DonationShop_OnCommand));
            
            //Masks
            DonationShopList.Add(new DonationCategory("Masks", 0x1545, 0, 0, 0), 
            new List<DonationItem>()
            {
                new DonationItem(typeof(BearMask), "Bear Mask", new List<string>{"Bear Mask", "(Non-Blessed)"}, 1000, 0x1545, 0, 0, 0),
                new DonationItem(typeof(DeerMask), "Deer Mask", new List<string>{"Deer Mask", "(Non-Blessed)"}, 1000, 0x1547, 0, 0, 0),
                new DonationItem(typeof(OrcMask), "Orc Mask", new List<string>{"Orc Mask", "(Non-Blessed)"}, 1000, 0x141B, 0, 0, 0),
                new DonationItem(typeof(SavageMask), "Savage Mask", new List<string>{"Savage Mask", "(Non-Blessed)"}, 1000, 0x154B, 0, 0, 0),
                new DonationItem(typeof(HornedTribalMask), "Tribal Mask", new List<string>{"Tribal Mask", "(Non-Blessed)"}, 1000, 0x1549, 0, 0, 0)
            });  
        }

        [Usage("DonationShop")]
        [Aliases("DonationStore", "Store", "Shop", "Donation")]
        [Description("Accesses the Donation Shop")]
        public static void DonationShop_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            player.CloseGump(typeof(DonationShop));
            player.SendGump(new DonationShopGump(player, 0, 0, 0));
        }
    }        

    public class DonationCategory
    {
        public string CategoryName;
        public int CategoryIconItemId;
        public int CategoryIconHue;
        public int CategoryIconOffsetX;
        public int CategoryIconOffsetY;

        public DonationCategory(string categoryName, int categoryIconItemId, int categoryIconHue, int categoryIconOffsetX, int categoryOffsetY)
        {
            CategoryName = categoryName;
            CategoryIconItemId = categoryIconItemId;
            CategoryIconHue = categoryIconHue;
            CategoryIconOffsetX = categoryIconOffsetX;
            CategoryIconOffsetY = categoryOffsetY;

            DonationShop.DonationCategories.Add(this);
        }
    }

    public class DonationItem
    {
        public Type ItemType;
        public string ItemName;
        public List<string> ItemDescription = new List<string>() { };
        public int ItemCost;
        public int ItemIconItemId;
        public int ItemIconHue;
        public int ItemIconOffsetX;
        public int ItemIconOffsetY;

        public DonationItem(Type itemType, string itemName, List<string> itemDescription, int itemCost, int itemIconItemId, int itemIconHue, int itemIconOffsetX, int itemIconOffsetY)
        {
            ItemType = itemType;
            ItemName = itemName;
            ItemDescription = itemDescription;
            ItemCost = itemCost;
            ItemIconItemId = itemIconItemId;
            ItemIconHue = itemIconHue;
            ItemIconOffsetX = itemIconOffsetY;
            ItemIconOffsetY = itemIconOffsetY;
        }
    }
}
