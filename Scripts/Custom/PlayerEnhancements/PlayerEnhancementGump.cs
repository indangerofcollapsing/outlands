using System;
using System.Collections;
using System.Collections.Generic;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Custom;

namespace Server.Items
{
	public class PlayerEnhancementGump : Gump
	{
		public PlayerEnhancementGump(Mobile from) : base( 10, 10 )
		{
		    Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;             

            AddImage(0, 44, 202);
            AddImage(44, 0, 201);
            AddImage(0, 0, 206);
            AddImage(0, 468, 204);
            AddImage(590, 1, 207);
            AddImage(590, 468, 205);
            AddImage(44, 468, 233);
            AddImage(590, 45, 203);
            AddImageTiled(44, 44, 546, 424, 200);
            AddImage(0, 152, 202);
            AddImage(163, 0, 201);
            AddImage(166, 468, 233);
            AddImage(590, 152, 203);

            AddImage(10, 212, 50576, 2613);
            AddImage(439, 264, 50575, 2613);
            AddImage(467, 318, 9811);           
            AddImage(109, 151, 9000);
            AddImage(600, 45, 10441);
            AddImage(501, 190, 7041, 2615);
            AddImage(413, 193, 7041, 2641);
            AddImage(264, 147, 1261, 2626);
            AddImage(48, 240, 12);
            AddImage(48, 242, 60552);
            AddImage(49, 239, 60680, 2615);
            AddImage(48, 241, 60477, 2615);
            AddImage(46, 237, 50618, 2615);
            AddImage(400, 253, 13);
            AddImage(400, 256, 50449, 2615);
            AddImage(399, 252, 60914, 2614);
            AddImage(399, 251, 50545, 2613);
            AddImage(400, 253, 50561, 2613);
            AddImage(48, 242, 60531, 2613);
            AddImage(288, 180, 1262, 2514);
            AddImage(323, 141, 1261, 2586);
            AddImage(457, 149, 7041, 2587);
            AddImage(70, 246, 60985, 2615);

            AddLabel(258, 20, 149, @"Player Enhancements");

            AddLabel(80, 65, 2615, "View Customizations");
            AddButton(128, 88, 4008, 4010, 1, GumpButtonType.Reply, 0);

            AddLabel(439, 65, 2606, "View Spell Hues");
            AddButton(475, 88, 4011, 4013, 2, GumpButtonType.Reply, 0);
		}        

		public override void OnResponse( NetState sender, RelayInfo info )
		{
            Mobile from = sender.Mobile;

            if (from == null) return;
            if (from.Deleted) return;          
            
            //Customizations
            if (info.ButtonID == 1)
            {
                from.CloseGump(typeof(PlayerEnhancementGump));
                from.SendGump(new PlayerCustomizationGump(from, 1));
            }

            //Spell Hues
            if (info.ButtonID == 2)
            {
                from.CloseGump(typeof(PlayerEnhancementGump));
                from.SendGump(new PlayerSpellHuesGump(from, 1));
            }

            return;            
		}
	}

    public class PlayerCustomizationGump : Gump
    {
        private PlayerMobile m_Player;
        private int m_PageNumber = 1;
        private int m_TotalPages = 1;
        private List<CustomizationType> m_CustomizationsOnPage = new List<CustomizationType>();

        public PlayerCustomizationGump(Mobile from, int pageNumber): base(10, 10)
        {
            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            m_Player = player;
            m_PageNumber = pageNumber;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddImage(0, 44, 202);
            AddImage(44, 0, 201);
            AddImage(0, 0, 206);
            AddImage(0, 468, 204);
            AddImage(590, 1, 207);
            AddImage(590, 468, 205);
            AddImage(44, 468, 233);
            AddImage(590, 45, 203);
            AddImageTiled(44, 44, 546, 424, 200);
            AddImage(0, 152, 202);
            AddImage(163, 0, 201);
            AddImage(166, 468, 233);
            AddImage(590, 152, 203);   

            //----------------------
            int textHue = 2036;
            int boldHue = 149;

            AddLabel(277, 20, 2615, "Customizations");

            int totalCustomizations = Enum.GetNames(typeof(CustomizationType)).Length;
            int customizationsPerPage = 5;

            m_TotalPages = (int)(Math.Ceiling((double)totalCustomizations / (double)customizationsPerPage));

            if (m_TotalPages == 0)
                m_TotalPages = 1;

            if (m_PageNumber < 1)
                m_PageNumber = 1;

            if (m_PageNumber > m_TotalPages)
                m_PageNumber = m_TotalPages;

            int indexStart = (m_PageNumber - 1) * customizationsPerPage;
            int indexEnd;

            if (indexStart + customizationsPerPage <= totalCustomizations)
                indexEnd = indexStart + customizationsPerPage;
            else
                indexEnd = totalCustomizations;

            if (indexEnd > totalCustomizations)
                indexEnd = totalCustomizations;

            var iStartY = 50;
            var iSpacingY = 75;

            int iCount = 0;           

            //Entry
            for (int a = indexStart; a < indexEnd; a++)
            {
                CustomizationType customizationType = (CustomizationType)a;
                PlayerCustomizationDetail customizationDetail = PlayerCustomization.GetCustomizationDetail(customizationType);
                PlayerCustomizationEntry customizationEntry = PlayerEnhancementPersistance.GetCustomizationEntry(player, customizationType);

                if (customizationType == null || customizationDetail == null || customizationEntry == null)
                    return;

                m_CustomizationsOnPage.Add(customizationType);

                bool unlocked = false;
                bool activated = false;

                AddBackground(25, iStartY, 459, 66, 3000);
                AddItem(25 + customizationDetail.m_IconOffsetX, iStartY + 13 + customizationDetail.m_IconOffsetY, customizationDetail.m_IconItemId, customizationDetail.m_IconHue); //Icon
                AddLabel(260 - (customizationDetail.m_Name.Length * 3), iStartY + 3, 149, customizationDetail.m_Name);

                if (customizationDetail.m_Description != null)
                {
                    for (int b = 0; b < customizationDetail.m_Description.Length; b++)
                    {
                        AddLabel(260 - (customizationDetail.m_Description[b].Length * 3), iStartY + ((b + 1) * 20), textHue, customizationDetail.m_Description[b]);
                    }
                }

                //AddLabel(260 - (customizationDetail.m_Description[0].Length * 3), iStartY + 20, textHue, customizationDetail.m_Description[0]);
                //AddLabel(260 - (customizationDetail.m_Description[1].Length * 3), iStartY + 40, textHue, customizationDetail.m_Description[1]);

                if (customizationEntry.m_Unlocked)
                {                   
                    if (customizationEntry.m_Active)
                    {
                        AddButton(496, iStartY + 16, 2154, 2151, 3 + iCount, GumpButtonType.Reply, 0);
                        AddLabel(533, iStartY + 20, textHue, "Active");
                    }

                    else
                    {
                        AddButton(496, iStartY + 16, 2151, 2154, 3 + iCount, GumpButtonType.Reply, 0);
                        AddLabel(533, iStartY + 20, textHue, "Disabled");
                    }
                }

                else
                {                    
                    AddItem(488, iStartY + 5, 3823);
                    AddLabel(535, iStartY + 9, textHue, Utility.CreateCurrencyString(customizationDetail.m_Cost));
                    AddButton(496, iStartY + 34, 9720, 9723, 3 + iCount, GumpButtonType.Reply, 0);
                    AddLabel(535, iStartY + 38, textHue, "Unlock");
                }

                iStartY += iSpacingY;
                iCount++;
            }

            //Navigation
            if (m_PageNumber > 1)
            {
                AddButton(50, 465, 4014, 4016, 1, GumpButtonType.Reply, 0);
                AddLabel(85, 465, textHue, @"Previous Page");
            }

            if (m_PageNumber < m_TotalPages)
            {
                AddButton(485, 465, 4005, 4007, 2, GumpButtonType.Reply, 0);
                AddLabel(520, 465, textHue, @"Next Page");
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            PlayerMobile player = sender.Mobile as PlayerMobile;

            if (player == null) return;
            if (player.Deleted) return;

            //Previous
            if (info.ButtonID == 1)
            {
                if (m_PageNumber > 1)
                {
                    m_PageNumber--;

                    player.SendSound(0x055);

                    player.CloseGump(typeof(PlayerCustomizationGump));
                    player.SendGump(new PlayerCustomizationGump(player, m_PageNumber));

                    return;
                }
            }

            //Next
            if (info.ButtonID == 2)
            {
                if (m_PageNumber < m_TotalPages)
                {
                    m_PageNumber++;

                    player.SendSound(0x055);

                    player.CloseGump(typeof(PlayerCustomizationGump));
                    player.SendGump(new PlayerCustomizationGump(player, m_PageNumber));

                    return;
                }
            }

            //Entries
            if (info.ButtonID > 2)
            {
                int index = info.ButtonID - 3;

                if (index >= m_CustomizationsOnPage.Count || index < 0)
                    return;

                CustomizationType customizationType = m_CustomizationsOnPage[index];
                PlayerCustomizationDetail customizationDetail = PlayerCustomization.GetCustomizationDetail(customizationType);
                PlayerCustomizationEntry customizationEntry = PlayerEnhancementPersistance.GetCustomizationEntry(player, customizationType);
                
                if (customizationType == null || customizationEntry == null || customizationDetail == null)
                    return;

                bool selectable = customizationDetail.m_Selectable;

                if (customizationEntry.m_Unlocked)
                {
                    if (customizationEntry.m_Active)
                    {
                        if (selectable)
                        {
                            customizationEntry.m_Active = false;
                            player.SendMessage("You disable the player customization: " + customizationDetail.m_Name);
                        }

                        else
                            player.SendMessage("That customization may not be disabled.");                        
                    }

                    else
                    {
                        customizationEntry.m_Active = true;
                        player.SendMessage("You enable the player customization: " + customizationDetail.m_Name);
                    }

                    player.CloseGump(typeof(PlayerCustomizationGump));
                    player.SendGump(new PlayerCustomizationGump(player, m_PageNumber));

                    return;
                }

                else
                {
                    player.CloseGump(typeof(PlayerCustomizationGump));
                    player.SendGump(new PlayerCustomizationConfirmationGump(player, m_PageNumber, customizationType));

                    return;
                }
            }

            player.CloseGump(typeof(PlayerEnhancementGump));
            player.SendGump(new PlayerEnhancementGump(player));

            return;
        }
    }

    public class PlayerCustomizationConfirmationGump : Gump
    {
        private PlayerMobile m_Player;
        private int m_PageNumber = 1;
        CustomizationType m_CustomizationType;

        public PlayerCustomizationConfirmationGump(PlayerMobile player, int pageNumber, CustomizationType customizationType): base(10, 10)
        {
            if (player == null || customizationType == null)
                return;

            m_Player = player;
            m_PageNumber = pageNumber;
            m_CustomizationType = customizationType;

            PlayerCustomizationDetail customizationDetail = PlayerCustomization.GetCustomizationDetail(customizationType);
            PlayerCustomizationEntry customizationEntry = PlayerEnhancementPersistance.GetCustomizationEntry(m_Player, customizationType);

            if (customizationEntry == null || customizationDetail == null)
                return;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddBackground(8, 8, 484, 228, 9200);
            AddBackground(19, 43, 459, 66, 3000);            

            int textHue = 2036;
            int boldTextHue = 149;

            int iStartY = 43;

            AddLabel(167, 17, boldTextHue, "Unlock Player Customization");

            AddItem(25 + customizationDetail.m_IconOffsetX, iStartY + 13 + customizationDetail.m_IconOffsetY, customizationDetail.m_IconItemId, customizationDetail.m_IconHue); //Icon
            AddLabel(260 - (customizationDetail.m_Name.Length * 3), iStartY + 3, 149, customizationDetail.m_Name);

            if (customizationDetail.m_Description != null)
            {
                for (int a = 0; a < customizationDetail.m_Description.Length; a++)
                {
                    AddLabel(260 - (customizationDetail.m_Description[a].Length * 3), iStartY + ((a + 1) * 20), textHue, customizationDetail.m_Description[a]);
                }
            }

            //AddLabel(260 - (customizationDetail.m_Description[0].Length * 3), iStartY + 20, textHue, customizationDetail.m_Description[0]);
            //AddLabel(260 - (customizationDetail.m_Description[1].Length * 3), iStartY + 40, textHue, customizationDetail.m_Description[1]);

            AddLabel(37, 119, textHue, "This will unlock this Player Customization for all characters on this ");
            AddLabel(29, 139, textHue, "account and will withdraw the following amount of gold from your bank");

            AddItem(185, 163, 3823);
            AddLabel(230, 167, textHue, Utility.CreateCurrencyString(customizationDetail.m_Cost));

            AddLabel(112, 199, textHue, "Confirm");
            AddButton(73, 195, 2152, 2154, 1, GumpButtonType.Reply, 0);

            AddLabel(376, 197, textHue, "Cancel");
            AddButton(337, 193, 2474, 2472, 2, GumpButtonType.Reply, 0);            
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            PlayerMobile player = sender.Mobile as PlayerMobile;

            if (player == null) return;
            if (player.Deleted) return;            
            if (m_CustomizationType == null) return;

            PlayerCustomizationDetail customizationDetail = PlayerCustomization.GetCustomizationDetail(m_CustomizationType);
            PlayerCustomizationEntry customizationEntry = PlayerEnhancementPersistance.GetCustomizationEntry(player, m_CustomizationType);

            if (customizationEntry == null || customizationDetail == null)
                return;

            //Confirm
            if (info.ButtonID == 1)
            {
                int bankBalance = Banker.GetBalance(player);

                if (bankBalance >= customizationDetail.m_Cost)
                {
                    Banker.Withdraw(player, customizationDetail.m_Cost);

                    player.SendMessage("You unlock the player customization: " + customizationDetail.m_Name + ".");

                    PlayerCustomization.OnUnlockCustomization(player, m_CustomizationType);

                    customizationEntry.m_Unlocked = true;
                    customizationEntry.m_Active = true;

                    player.PlaySound(0x5C9);
                    player.FixedParticles(0x375A, 10, 15, 5012, 0, 0, EffectLayer.Waist);

                    player.CloseGump(typeof(PlayerCustomizationConfirmationGump));
                    player.SendGump(new PlayerCustomizationGump(player, m_PageNumber));

                    return;
                }

                else
                    player.SendMessage("You do not have the neccesary funds in your bank account to purchase that player customization.");

                player.CloseGump(typeof(PlayerCustomizationConfirmationGump));
                player.SendGump(new PlayerCustomizationConfirmationGump(player, m_PageNumber, m_CustomizationType));

                return;                
            }

            //Cancel
            if (info.ButtonID == 2)
            {

                player.CloseGump(typeof(PlayerCustomizationGump));
                player.SendGump(new PlayerCustomizationGump(player, m_PageNumber));

                return;
            }

            player.CloseGump(typeof(PlayerCustomizationGump));
            player.SendGump(new PlayerCustomizationGump(player, m_PageNumber));
        }
    }

    public class PlayerSpellHuesGump : Gump
    {
        private PlayerMobile m_Player;
        private int m_PageNumber = 1;
        private int m_TotalPages = 1;
        private List<HueableSpell> m_SpellsOnPage = new List<HueableSpell>();

        public PlayerSpellHuesGump(Mobile from, int pageNumber): base(10, 10)
        {
            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            m_Player = player;
            m_PageNumber = pageNumber;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddImage(0, 44, 202);
            AddImage(44, 0, 201);
            AddImage(0, 0, 206);
            AddImage(0, 468, 204);
            AddImage(590, 1, 207);
            AddImage(590, 468, 205);
            AddImage(44, 468, 233);
            AddImage(590, 45, 203);
            AddImageTiled(44, 44, 546, 424, 200);
            AddImage(0, 152, 202);
            AddImage(163, 0, 201);
            AddImage(166, 468, 233);
            AddImage(590, 152, 203);

            //-------------
            int textHue = 2036;
            int boldHue = 149;
            
            int totalSpells = Enum.GetNames(typeof(HueableSpell)).Length;
            int totalHues = Enum.GetNames(typeof(SpellHueType)).Length;

            int spellsPerPage = 3;           

            m_TotalPages = (int)(Math.Ceiling((double)totalSpells / (double)spellsPerPage));

            int indexStart = (m_PageNumber - 1) * spellsPerPage;
            int indexEnd;

            if (indexStart + spellsPerPage <= totalSpells)
                indexEnd = indexStart + spellsPerPage;
            else
                indexEnd = totalSpells;

            var iStartY = 50;
            var iSpacingY = 135;

            AddLabel(292, 20, 2606, "Spell Hues");

            int iRow = 0;

            //Entry
            for (int a = indexStart; a < indexEnd; a++)
            {
                HueableSpell hueableSpell = (HueableSpell)a;
                HueableSpellDetail spellDetail = SpellHue.GetHueableSpellDetail(hueableSpell);
                SpellHueEntry spellEntry = PlayerEnhancementPersistance.GetSpellHueEntry(player, hueableSpell);

                if (hueableSpell == null || spellDetail == null || spellEntry == null)
                    return;

                m_SpellsOnPage.Add(hueableSpell);

                AddLabel(320 - (spellDetail.m_SpellName.Length * 3), iStartY, boldHue, spellDetail.m_SpellName);

                for (int b = 0; b < totalHues; b++)
                {
                    SpellHueType spellHueType = (SpellHueType)b;
                    SpellHueTypeDetail spellHueTypeDetail = SpellHue.GetSpellHueTypeDetail(spellHueType);

                    AddItem((100 + (75 * b)), iStartY + 25, spellDetail.m_IconItemId, spellHueTypeDetail.m_DisplayHue);
                    AddLabel((120 + (75 * b)) - (spellHueTypeDetail.m_Name.Length * 3), iStartY + 75, textHue, spellHueTypeDetail.m_Name);

                    bool unlocked = false;
                    bool active = false;

                    if (spellEntry.m_UnlockedHues.Contains(spellHueType))
                        unlocked = true;

                    if (spellEntry.m_ActiveHue == spellHueType)
                        active = true;

                    int buttonIndex = 3 + (iRow * totalHues) + b;

                    if (unlocked)
                    {
                        if (active)
                            AddButton(105 + (75 * b), iStartY + 100, 2154, 2151, buttonIndex, GumpButtonType.Reply, 0);  
                        else
                            AddButton(105 + (75 * b), iStartY + 100, 2151, 2154, buttonIndex, GumpButtonType.Reply, 0);  
                    }

                    else
                        AddButton(105 + (75 * b), iStartY + 100, 9727, 9727, buttonIndex, GumpButtonType.Reply, 0);                   
                }

                iStartY += iSpacingY;
                iRow++;
            }

            //Navigation
            if (m_PageNumber > 1)
            {
                AddButton(50, 465, 4014, 4016, 1, GumpButtonType.Reply, 0);
                AddLabel(85, 465, textHue, @"Previous Page");
            }

            if (m_PageNumber < m_TotalPages)
            {
                AddButton(485, 465, 4005, 4007, 2, GumpButtonType.Reply, 0);
                AddLabel(520, 465, textHue, @"Next Page");
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            PlayerMobile player = sender.Mobile as PlayerMobile;

            if (player == null) return;
            if (player.Deleted) return;           

            //Previous
            if (info.ButtonID == 1)
            {
                if (m_PageNumber > 1)
                {
                    m_PageNumber--;

                    player.SendSound(0x055);

                    player.CloseGump(typeof(PlayerSpellHuesGump));
                    player.SendGump(new PlayerSpellHuesGump(player, m_PageNumber));

                    return;
                }
            }

            //Next
            if (info.ButtonID == 2)
            {
                if (m_PageNumber < m_TotalPages)
                {
                    m_PageNumber++;

                    player.SendSound(0x055);

                    player.CloseGump(typeof(PlayerSpellHuesGump));
                    player.SendGump(new PlayerSpellHuesGump(player, m_PageNumber));

                    return;
                }
            }

            //Entries
            if (info.ButtonID > 2)
            {
                if (m_SpellsOnPage.Count == 0)
                    return;
              
                int totalHues = Enum.GetNames(typeof(SpellHueType)).Length;
                
                int baseIndex = info.ButtonID - 3;
                int spellIndex = (int)(Math.Floor((double)baseIndex / (double)totalHues));
                int hueTypeIndex = baseIndex - (spellIndex * totalHues);
                
                if (spellIndex > m_SpellsOnPage.Count || spellIndex < 0)
                {
                    player.SendMessage("Invalid selection.");
                    return;
                }

                if (spellIndex >= m_SpellsOnPage.Count)
                    return;

                HueableSpell hueableSpell = m_SpellsOnPage[spellIndex];

                if (hueTypeIndex >= totalHues || hueableSpell == null)
                    return;

                SpellHueType spellHueType = (SpellHueType)hueTypeIndex;
                HueableSpellDetail spellDetail = SpellHue.GetHueableSpellDetail(hueableSpell);
                SpellHueEntry spellEntry = PlayerEnhancementPersistance.GetSpellHueEntry(player, hueableSpell);                

                if (spellDetail == null || spellEntry == null)
                    return;

                bool unlocked = false;
                bool active = false;
                
                if (spellEntry.m_UnlockedHues.Contains(spellHueType))
                    unlocked = true;

                if (spellEntry.m_ActiveHue == spellHueType)
                    active = true;
                
                if (unlocked)
                {
                    if (active)
                    {
                        if (spellHueType == SpellHueType.Basic)
                            player.SendMessage("The standard hue for a spell cannot be deactivated.");                        

                        else
                        {
                            player.SendMessage("You disable the custom spell hue and activate the basic hue.");
                            spellEntry.m_ActiveHue = SpellHueType.Basic;

                            player.SendSound(0x1EC);
                        }
                    }

                    else
                    {
                        player.SendMessage("You activate the custom spell hue.");
                        spellEntry.m_ActiveHue = spellHueType;

                        player.SendSound(0x1EC);
                    }

                    player.CloseGump(typeof(PlayerSpellHuesGump));
                    player.SendGump(new PlayerSpellHuesGump(player, m_PageNumber));

                    return;
                }

                else
                {
                    player.SendMessage("You have not unlocked that hue for that spell. Spell Hue Deeds may be obtained from the Donation Shop or may be discovered out in the world.");

                    player.CloseGump(typeof(PlayerSpellHuesGump));
                    player.SendGump(new PlayerSpellHuesGump(player, m_PageNumber));

                    return;
                }                
            }

            player.CloseGump(typeof(PlayerSpellHuesGump));
            player.SendGump(new PlayerEnhancementGump(player));

            return;
        }
    }
}
