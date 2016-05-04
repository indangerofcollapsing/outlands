using System;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Gumps
{
    public class DonationShopGump : Gump
    {
        public PlayerMobile m_Player;

        public int m_CategorySelected = 0;
        public int m_CategoryPage = 0;        
        public int m_ItemPage = 0;

        public static int CategoriesPerPage = 5;
        public static int ItemsPerPage = 3;        

        public DonationShopGump(PlayerMobile player, int categoryPage, int categorySelected, int itemPage): base(10, 10)
        {
            if (player == null)
                return;

            m_Player = player;
            m_CategoryPage = categoryPage;
            m_CategorySelected = categorySelected;
            m_ItemPage = itemPage;

            if (DonationShop.DonationCategories == null) return;
            if (DonationShop.DonationCategories.Count == 0) return;

            int totalCategories = DonationShop.DonationCategories.Count;
            int totalCategoryPages = (int)(Math.Ceiling((double)totalCategories / (double)CategoriesPerPage));  
          
            if (m_CategoryPage >= totalCategoryPages)
                m_CategoryPage = 0;

            if (m_CategoryPage < 0)
                m_CategoryPage = 0;

            int categoryStartIndex = m_CategoryPage * CategoriesPerPage;
            int categoryEndIndex = (m_CategoryPage * CategoriesPerPage) + (CategoriesPerPage - 1);

            if (categoryEndIndex >= totalCategories)
                categoryEndIndex = totalCategories - 1;

            if (m_CategorySelected >= totalCategories)
                m_CategorySelected = 0;

            if (m_CategorySelected < 0)
                m_CategorySelected = 0;  
          
            DonationCategory donationCategory = DonationShop.DonationCategories[m_CategorySelected];

            if (donationCategory == null)
                return;

            List<DonationItem> itemList = new List<DonationItem>();

            foreach (DonationItem itemListing in DonationShop.DonationShopList[donationCategory])
            {
                itemList.Add(itemListing);
            }
                        
            int totalItems = itemList.Count;            
            int totalItemPages = (int)(Math.Ceiling((double)totalItems / (double)ItemsPerPage));

            if (m_ItemPage >= totalItemPages)
                m_ItemPage = 0;

            if (m_ItemPage < 0)
                m_ItemPage = 0;

            int itemStartIndex = m_ItemPage * ItemsPerPage;
            int itemEndIndex = (m_ItemPage * ItemsPerPage) + (ItemsPerPage - 1);
            
            if (itemStartIndex >= totalItems)
                itemStartIndex = totalItems - 1;

            if (itemEndIndex >= totalItems)
                itemEndIndex = totalItems - 1;

            int donationCurrencyInBank = Banker.GetUniqueCurrencyBalance(m_Player, DonationShop.DonationCurrencyType);

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            int textHue = 2036;

            AddPage(0);

            #region Images

            AddImage(13, 5, 206, 2419);
            AddImage(645, 5, 207, 2419);
            AddImage(13, 169, 202, 2419);
            AddImage(517, 361, 200, 2419);
            AddImage(389, 361, 200, 2419);
            AddImage(312, 361, 200, 2419);
            AddImage(184, 361, 200, 2419);
            AddImage(56, 361, 200, 2419);
            AddImage(518, 240, 200, 2419);
            AddImage(519, 170, 200, 2419);
            AddImage(518, 49, 200, 2419);
            AddImage(395, 240, 200, 2419);
            AddImage(312, 240, 200, 2419);
            AddImage(184, 240, 200, 2419);
            AddImage(56, 240, 200, 2419);
            AddImage(56, 170, 200, 2419);
            AddImage(184, 170, 200, 2419);
            AddImage(312, 170, 200, 2419);
            AddImage(398, 170, 200, 2419);
            AddImage(56, 49, 200, 2419);
            AddImage(184, 49, 200, 2419);
            AddImage(312, 49, 200, 2419);
            AddImage(390, 49, 200, 2419);
            AddImage(57, 485, 233, 2419);
            AddImage(56, 5, 201, 2419);
            AddImage(13, 49, 202, 2419);
            AddImage(646, 49, 203, 2419);
            AddImage(645, 485, 205, 2419);
            AddImage(13, 485, 204, 2419);
            AddImage(218, 5, 201, 2419);
            AddImage(218, 485, 233, 2419);
            AddImage(645, 169, 203, 2419);

            AddImage(31, 22, 9002, 2412);
            AddImage(223, 27, 1143, 2499);

            AddItem(546, 251, 2760);
            AddItem(524, 229, 2760);
            AddItem(545, 208, 2760);
            AddItem(502, 251, 2768);
            AddItem(523, 272, 2768);
            AddItem(566, 198, 2766);
            AddItem(525, 194, 2765);
            AddItem(630, 254, 2764);
            AddItem(590, 252, 2760);
            AddItem(568, 273, 2760);
            AddItem(568, 229, 2760);
            AddItem(589, 218, 2766);
            AddItem(544, 186, 2762);
            AddItem(502, 216, 2765);
            AddItem(608, 237, 2766);
            AddItem(544, 292, 2768);
            AddItem(481, 233, 2763);
            AddItem(611, 271, 2767);
            AddItem(590, 293, 2767);
            AddItem(566, 314, 2761);
            AddItem(505, 149, 3225);
            AddItem(618, 175, 3229);
            AddItem(482, 175, 3228);
            AddItem(475, 218, 555);
            AddItem(484, 166, 9);
            AddItem(605, 241, 3651);
            AddItem(491, 237, 554);
            AddItem(508, 254, 554);
            AddItem(556, 300, 554);
            AddItem(570, 313, 555, 2415);
            AddItem(586, 295, 555);
            AddItem(496, 196, 555);
            AddItem(517, 173, 555);
            AddItem(604, 274, 555);
            AddItem(525, 166, 555);
            AddItem(567, 168, 554);
            AddItem(584, 184, 554);
            AddItem(599, 198, 554);
            AddItem(616, 214, 554);
            AddItem(640, 237, 554);
            AddItem(484, 140, 9);
            AddItem(543, 111, 9);
            AddItem(543, 54, 9);
            AddItem(630, 186, 9);
            AddItem(630, 129, 9);
            AddItem(618, 259, 555);
            AddItem(538, 186, 3644);
            AddItem(540, 178, 3647);
            AddItem(506, 32, 1539, 149);
            AddItem(486, 50, 1561, 149);
            AddItem(467, 80, 1561, 149);
            AddItem(526, 23, 1536, 149);
            AddItem(530, 54, 1539, 2500);
            AddItem(510, 72, 1561, 2500);
            AddItem(491, 102, 1561, 2500);
            AddItem(550, 45, 1536, 2500);
            AddItem(552, 77, 1539, 149);
            AddItem(532, 95, 1561, 149);
            AddItem(513, 125, 1561, 149);
            AddItem(572, 68, 1536, 149);
            AddItem(573, 99, 1539, 2500);
            AddItem(553, 117, 1561, 2500);
            AddItem(534, 147, 1561, 2500);
            AddItem(593, 90, 1536, 2500);
            AddItem(594, 122, 1539, 149);
            AddItem(575, 139, 1561, 149);
            AddItem(614, 113, 1536, 149);
            AddItem(615, 143, 1539, 2500);
            AddItem(595, 161, 1561, 2500);
            AddItem(635, 134, 1536, 2500);
            AddItem(524, 215, 2818);
            AddItem(547, 236, 2817);
            AddItem(568, 258, 2816);
            AddItem(532, 215, 3656);
            AddItem(512, 226, 3628);
            AddItem(557, 279, 5452, 2500);
            AddItem(552, 222, 4644);
            AddItem(538, 238, 5357);
            AddItem(522, 234, 3838);
            AddItem(553, 256, 2886, 2606);
            AddItem(574, 261, 3629, 2500);
            AddItem(552, 244, 2586, 2587);
            AddItem(564, 245, 9);
            AddItem(564, 210, 9);
            AddItem(573, 314, 554);
            AddItem(582, 288, 3228);
            AddItem(555, 170, 1561, 149);
            AddItem(576, 190, 1561, 2500);

            #endregion

            //Header           
            AddLabel(314, 29, 149, "Donation Shop");  

            //Guide
            AddButton(29, 10, 2094, 2095, 1, GumpButtonType.Reply, 0);
            AddLabel(56, 19, textHue, "Guide");

            //Bank Account
            AddItem(467, 337, 3823, 2500);
            AddLabel(Utility.CenteredTextOffset(490, Utility.CreateCurrencyString(donationCurrencyInBank)), 366, textHue, Utility.CreateCurrencyString(donationCurrencyInBank));
            AddLabel(440, 386, 149, DonationShop.DonationCurrencyName + " in Bank");

            //Make Donation
            AddLabel(570, 386, 169, "Make Donation");
            AddButton(598, 409, 2152, 2151, 2, GumpButtonType.Reply, 0);            

            //Scroll Items
            if (m_ItemPage > 0)
                AddButton(436, 224, 9900, 9900, 3, GumpButtonType.Reply, 0);

            if (m_ItemPage < totalItemPages - 1)
                AddButton(436, 265, 9906, 9906, 4, GumpButtonType.Reply, 0);

            //Item
            int startY = 75;
            int itemSpacing = 124;

            int itemCount = itemEndIndex - itemStartIndex;

            for (int a = 0; a < itemCount + 1; a++)
            {
                int itemIndex = itemStartIndex + a;
                int itemButtonIndex = 20 + a;

                if (itemIndex >= itemList.Count)
                    continue;
                
                DonationItem item = itemList[itemIndex];

                if (item == null)
                    continue;
                
                AddImage(75, startY, 103, 2412);
                AddImage(205, startY, 103, 2412);
                AddImage(275, startY, 103, 2412);
                AddBackground(85, startY + 10, 323, 81, 3000);

                AddLabel(Utility.CenteredTextOffset(250, item.ItemName), startY - 10, 149, item.ItemName);
                AddItem(85 + item.ItemIconOffsetX, startY + 35 + item.ItemIconOffsetY, item.ItemIconItemId, item.ItemIconHue);

                if (item.ItemDescription != null)
                {
                    for (int b = 0; b < item.ItemDescription.Count; b++)
                    {
                        //AddLabel(140, startY + 18 + (b * 20), textHue, item.ItemDescription[b]);
                        AddLabel(Utility.CenteredTextOffset(250, item.ItemDescription[b]), startY + 18 + (b * 20), textHue, item.ItemDescription[b]);
                    }
                }
                
                AddItem(150, startY + 80, 3823, 2500);
                AddLabel(190, startY + 83, 2519, Utility.CreateCurrencyString(item.ItemCost));
                AddButton(270, startY + 75, 2152, 2151, itemButtonIndex, GumpButtonType.Reply, 0);
                AddLabel(305, startY + 83, 169, "Purchase");

                startY += itemSpacing;
            }

            //Categories
            int startX = 120;
            int categorySpacing = 100;

            int categoryCount = categoryEndIndex - categoryStartIndex;

            for (int a = 0; a < categoryCount + 1; a++)
            {
                int categoryIndex = categoryStartIndex + a;
                int categoryButtonIndex = 40 + categoryIndex;

                if (categoryStartIndex >= totalCategories)
                    continue;

                DonationCategory category = DonationShop.DonationCategories[categoryIndex];

                if (donationCategory == null)
                    continue;

                AddLabel(Utility.CenteredTextOffset(startX + 45, category.CategoryName), 445, 149, category.CategoryName);
                AddItem(startX + category.CategoryIconOffsetX, 470 + category.CategoryIconOffsetY, category.CategoryIconItemId, category.CategoryIconHue);

                if (categoryIndex == m_CategorySelected)
                    AddButton(startX + 45, 475, 9724, 9721, 0, GumpButtonType.Reply, 0);
                else
                    AddButton(startX + 45, 475, 9721, 9724, 0, GumpButtonType.Reply, 0);

                startX += categorySpacing;
            }
            
            //Previous Category
            if (categoryPage > 0)
                AddButton(77, 475, 4014, 4016, 5, GumpButtonType.Reply, 0);

            //Next Category
            if (categoryPage < totalCategoryPages - 1)
                AddButton(609, 475, 4005, 4007, 6, GumpButtonType.Reply, 0); 
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null) return;
            if (!m_Player.Alive) return;
            if (m_Player.Backpack == null) return;

            bool closeGump = true;

            int totalCategories = DonationShop.DonationCategories.Count;
            int totalCategoryPages = (int)(Math.Ceiling((double)totalCategories / (double)CategoriesPerPage));

            if (m_CategoryPage >= totalCategoryPages)
                m_CategoryPage = 0;

            if (m_CategoryPage < 0)
                m_CategoryPage = 0;

            int categoryStartIndex = m_CategoryPage * CategoriesPerPage;
            int categoryEndIndex = (m_CategoryPage * CategoriesPerPage) + (CategoriesPerPage - 1);

            if (categoryEndIndex >= totalCategories)
                categoryEndIndex = totalCategories - 1;

            if (m_CategorySelected >= totalCategories)
                m_CategorySelected = 0;

            if (m_CategorySelected < 0)
                m_CategorySelected = 0;

            DonationCategory donationCategory = DonationShop.DonationCategories[m_CategorySelected];

            if (donationCategory == null)
                return;

            List<DonationItem> itemList = DonationShop.DonationShopList[donationCategory];

            int totalItems = itemList.Count;
            int totalItemPages = (int)(Math.Ceiling((double)totalItems / (double)ItemsPerPage));

            if (m_ItemPage >= totalItemPages)
                m_ItemPage = 0;

            if (m_ItemPage < 0)
                m_ItemPage = 0;

            int itemStartIndex = m_ItemPage * ItemsPerPage;
            int itemEndIndex = (m_ItemPage * ItemsPerPage) + (ItemsPerPage - 1);

            if (itemEndIndex >= totalItems)
                itemEndIndex = totalItems - 1;

            int donationCurrencyInBank = Banker.GetUniqueCurrencyBalance(m_Player, DonationShop.DonationCurrencyType);

            switch (info.ButtonID)
            {
                //Guide
                case 1:
                    closeGump = false;
                break;

                //Make Donation
                case 2:
                    closeGump = false;
                break;

                //Previous Item Page
                case 3:
                    if (m_ItemPage > 0)
                        m_ItemPage--;

                    closeGump = false;
                break;

                //Next Item Page
                case 4:
                    if (m_ItemPage < totalItemPages - 1)
                        m_ItemPage++;

                    closeGump = false;
                break;

                //Previous Category Page
                case 5:
                    if (m_CategoryPage > 0)
                        m_CategoryPage++;

                    closeGump = false;
                break;

                //Next Category Page
                case 6:
                    if (m_CategoryPage < totalCategoryPages - 1)
                        m_CategoryPage++;

                    closeGump = false;
                break;
            }

            //Item Selection
            if (info.ButtonID >= 20 && info.ButtonID < 40)
            {
                int itemSelectionIndex = info.ButtonID - 20;
                int itemSelected = (m_ItemPage * ItemsPerPage) + itemSelectionIndex;

                if (itemSelected >= totalItems)
                    itemSelected = 0;

                //Purchase Item
                DonationItem item = itemList[itemSelected];

                if (item == null)
                    return;

                bool purchaseAllowed = true;

                if (!purchaseAllowed)
                {
                    m_Player.SendMessage("Donation shop purchases are not allowed in this area.");

                    m_Player.CloseGump(typeof(DonationShopGump));
                    m_Player.SendGump(new DonationShopGump(m_Player, m_CategoryPage, m_CategorySelected, m_ItemPage));

                    return;
                }

                //TEST
                m_Player.Say("Cost: " + item.ItemCost.ToString() + " vs Bank: " + donationCurrencyInBank.ToString());

                if (item.ItemCost > donationCurrencyInBank)
                {
                    m_Player.SendMessage("You do not have enough " + DonationShop.DonationCurrencyName + " in your bank to purchase this item.");

                    m_Player.CloseGump(typeof(DonationShopGump));
                    m_Player.SendGump(new DonationShopGump(m_Player, m_CategoryPage, m_CategorySelected, m_ItemPage));

                    return;
                }

                Item donationItem = (Item)Activator.CreateInstance(item.ItemType);

                if (donationItem == null)
                {
                    m_Player.CloseGump(typeof(DonationShopGump));
                    m_Player.SendGump(new DonationShopGump(m_Player, m_CategoryPage, m_CategorySelected, m_ItemPage));

                    return;
                }
                
                if (m_Player.Backpack.TotalItems + donationItem.TotalItems > m_Player.Backpack.MaxItems)
                {
                    donationItem.Delete();

                    m_Player.SendMessage("Your backpack contains too many items to purchase this item. Please remove some items and try again.");

                    m_Player.CloseGump(typeof(DonationShopGump));
                    m_Player.SendGump(new DonationShopGump(m_Player, m_CategoryPage, m_CategorySelected, m_ItemPage));

                    return;
                }

                if (m_Player.Backpack.TotalWeight + donationItem.TotalWeight > m_Player.MaxWeight)
                {
                    donationItem.Delete();

                    m_Player.SendMessage("Your backpack is too heavy to purchase this item. Please remove some items and try again.");

                    m_Player.CloseGump(typeof(DonationShopGump));
                    m_Player.SendGump(new DonationShopGump(m_Player, m_CategoryPage, m_CategorySelected, m_ItemPage));

                    return;
                }
                
                m_Player.SendSound(0x2E6);
                m_Player.SendMessage("You purchase the donation item.");

                m_Player.Backpack.DropItem(donationItem);

                Banker.WithdrawUniqueCurrency(m_Player, DonationShop.DonationCurrencyType, item.ItemCost);

                closeGump = false;
            }

            //Category Selection
            if (info.ButtonID >= 40 && info.ButtonID < 60)
            {
                int categorySelectionIndex = info.ButtonID - 40;
                int categorySelected = (m_CategoryPage * CategoriesPerPage) + categorySelectionIndex;

                if (categorySelected >= totalCategories)
                    categorySelected = 0;

                m_CategorySelected = categorySelected;
                m_ItemPage = 0;

                closeGump = false;
            }

            if (!closeGump)
            {
                m_Player.CloseGump(typeof(DonationShopGump));
                m_Player.SendGump(new DonationShopGump(m_Player, m_CategoryPage, m_CategorySelected, m_ItemPage));
            }
        }
    }
}