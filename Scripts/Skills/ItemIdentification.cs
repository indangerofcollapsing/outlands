using System;
using Server;
using Server.Targeting;
using Server.Mobiles;
using Server.Gumps;
using System.Globalization;

namespace Server.Items
{
    public class ItemIdentification
    {
        public static void Initialize()
        {
            SkillInfo.Table[(int)SkillName.ItemID].Callback = new SkillUseCallback(OnUse);
        }

        public static TimeSpan OnUse(Mobile from)
        {
            from.SendLocalizedMessage(500343); // What do you wish to appraise and identify?
            from.Target = new InternalTarget();

            return TimeSpan.FromSeconds(1.0);
        }

        [PlayerVendorTarget]
        private class InternalTarget : Target
        {
            public InternalTarget(): base(8, false, TargetFlags.None)
            {
                AllowNonlocal = true;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (player == null)
                    return;

                if (target is Item)
                {
                    Item item = target as Item;

                    from.NextSkillTime = Core.TickCount + (int)(SkillCooldown.ItemIDCooldown * 1000);

                    bool useGump = true;
                    bool success = false;

                    /*
                    if (item is BaseWeapon || item is BaseArmor || item is BaseInstrument)
                    {
                        if (!item.DecorativeEquipment)
                            useGump = false;
                    }
                    */

                    if (from.CheckTargetSkill(SkillName.ItemID, 0, 120, 1.0))
                        success = true;

                    if (success)
                    {
                        if (item is Container)
                        {
                            Container container = item as Container;

                            if (player.Skills[SkillName.ItemID].Value >= 105 || player.AccessLevel > AccessLevel.Player)
                            {
                                foreach (Item containerItem in container.Items)
                                {
                                    containerItem.Identified = true;
                                }
                            }

                            else                            
                                player.SendMessage("An Item Identification skill of 105 or higher is required to identify all items held within a container.");                                                   
                        }
                        
                        item.Identified = true;
                    }

                    else
                    {
                        player.SendMessage("You are not certain.");
                        return;
                    }

                    if (useGump)
                    {
                        player.SendSound(0x055);
                        player.CloseGump(typeof(ItemIdGump));
                        player.SendGump(new ItemIdGump(player, item, success, false, false));
                    }
                }

                else
                {
                    from.SendMessage("That is not an item.");
                    return;
                }
            }
        }
    }

    public class ItemIdGump : Gump
    {
        public PlayerMobile m_Player;
        public Item m_Item;

        public bool m_Success;

        public bool m_ShowRarity;
        public bool m_ShowWorldItemCount;

        public ItemIdGump(PlayerMobile player, Item item, bool success, bool overrideShowRarity, bool overrideShowWorldItemCount): base(200, 25)
        {
            if (player == null || item == null) return;
            if (item.Deleted) return;

            m_Player = player;
            m_Item = item;

            m_Success = success;

            m_ShowRarity = overrideShowRarity;
            m_ShowWorldItemCount = overrideShowWorldItemCount;
            
            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            int worldItemCount = 0;

            bool showRarity = false;
            bool showWorldItemCount = false;

            bool conductWorldItemQuery = true;

            double itemIDSkill = m_Player.Skills[SkillName.ItemID].Value;

            Type itemType = m_Item.GetType();

            if ((success && itemIDSkill >= 110) || m_ShowRarity || m_Player.AccessLevel > AccessLevel.Player)
                showRarity = true;

            if ((success && itemIDSkill >= 120) || m_ShowWorldItemCount || m_Player.AccessLevel > AccessLevel.Player)
            {
                showWorldItemCount = true;

                if (m_Player.AccessLevel == AccessLevel.Player && itemType == m_Player.m_LastItemIdWorldItemCountSearchType && itemType != null)                
                {
                    if (DateTime.UtcNow < m_Player.m_LastItemIdWorldItemCountSearch + TimeSpan.FromSeconds(10))
                        conductWorldItemQuery = false;
                }               

                if (conductWorldItemQuery)
                {
                    foreach (Item worldItem in World.Items.Values)
                    {
                        if (worldItem.GetType() == itemType)
                            worldItemCount++;
                    }

                    m_Player.m_LastItemIdWorldItemCountSearchCount = worldItemCount;
                    m_Player.m_LastItemIdWorldItemCountSearchType = itemType;
                    m_Player.m_LastItemIdWorldItemCountSearch = DateTime.UtcNow;
                }

                else
                    worldItemCount = m_Player.m_LastItemIdWorldItemCountSearchCount;
            }

            string itemName = m_Item.Name;

            if (itemName == null || itemName == "")
            {
                itemName = "Item Details";

                m_Item.OnSingleClick(m_Player);
            }

            if (itemName != "")
                itemName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(itemName);

            string typeText = Item.GetItemGroupTypeName(m_Item.ItemGroup);
            string rarityText = Item.GetItemRarityName(m_Item.ItemRarity);
            int rarityHue = Item.GetItemRarityTextHue(m_Item.ItemRarity);

            string totalCountText = Utility.CreateCurrencyString(worldItemCount);

            if (!showRarity)
                rarityText = "?";
     
            if (!showWorldItemCount)
                totalCountText = "?";

            m_ShowRarity = showRarity;
            m_ShowWorldItemCount = showWorldItemCount;

            int WhiteTextHue = 2655;

            AddPage(0);

            AddImage(135, 12, 103, 2401);
            AddImage(7, 12, 103, 2401);
            AddBackground(19, 21, 246, 78, 9270);

            AddButton(6, 98, 2094, 2095, 1, GumpButtonType.Reply, 0);
            AddLabel(2, 86, 149, "Guide");

            AddLabel(Utility.CenteredTextOffset(145, itemName), 4, 149, itemName);

            AddLabel(56, 39, WhiteTextHue, "Item Type");
            AddLabel(Utility.CenteredTextOffset(90, typeText), 59, 2599, typeText);

            AddLabel(160, 39, WhiteTextHue, "Item Rarity");
            AddLabel(Utility.CenteredTextOffset(197, rarityText), 59, rarityHue, rarityText);
           
            AddLabel(71, 92, WhiteTextHue, "Total in World:");
            AddLabel(171, 92, 149, totalCountText);            
        }

        public override void OnResponse(Network.NetState sender, RelayInfo info)
        {
            if (m_Player == null || m_Item == null) return;
            if (m_Item.Deleted) return;

            bool closeGump = true;

            switch (info.ButtonID)
            {
                // Guide
                case 1:
                    closeGump = false;
                break;
            }

            if (!closeGump)
            {
                m_Player.CloseGump(typeof(ItemIdGump));
                m_Player.SendGump(new ItemIdGump(m_Player, m_Item, m_Success, m_ShowRarity, m_ShowWorldItemCount));
            }

            else            
                m_Player.SendSound(0x058);            
        }
    }
}