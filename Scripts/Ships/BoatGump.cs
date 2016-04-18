using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Server.Network;
using Server.Prompts;
using Server.Multis;
using Server.Multis.Deeds;
using Server.Items;
using Server.Mobiles;

namespace Server.Prompts
{
    public class BoatRenamePrompt : Prompt
    {
        private BaseBoat m_Boat;

        public BoatRenamePrompt(BaseBoat boat)
        {
            m_Boat = boat;
        }

        public override void OnResponse(Mobile from, string text)
        {
            if (m_Boat.IsOwner(from))
            {
                m_Boat.ShipName = text;

                from.SendMessage("Boat name changed.");
            }
        }
    }
}

namespace Server.Gumps
{
    public class BoatListGump : Gump
    {
        private BaseBoat m_Boat;

        public BoatListGump(int number, ArrayList list, BaseBoat boat, bool accountOf)
            : base(20, 30)
        {
            if (boat.Deleted)
                return;

            m_Boat = boat;

            AddPage(0);

            AddBackground(0, 0, 420, 430, 5054);
            AddBackground(10, 10, 400, 410, 3000);

            AddButton(20, 388, 4005, 4007, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(55, 388, 300, 20, 1011104, false, false); // Return to previous menu

            AddHtmlLocalized(20, 20, 350, 20, number, false, false);

            if (list != null)
            {
                for (int i = 0; i < list.Count; ++i)
                {
                    if ((i % 16) == 0)
                    {
                        if (i != 0)
                        {
                            // Next button
                            AddButton(370, 20, 4005, 4007, 0, GumpButtonType.Page, (i / 16) + 1);
                        }

                        AddPage((i / 16) + 1);

                        if (i != 0)
                        {
                            // Previous button
                            AddButton(340, 20, 4014, 4016, 0, GumpButtonType.Page, i / 16);
                        }
                    }

                    Mobile m = (Mobile)list[i];

                    string name;

                    if (m == null || (name = m.Name) == null || (name = name.Trim()).Length <= 0)
                        continue;

                    AddLabel(55, 55 + ((i % 16) * 20), 0, accountOf && m.Player && m.Account != null ? String.Format("Account of {0}", name) : name);
                }
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (m_Boat.Deleted)
                return;

            Mobile from = state.Mobile;

            from.SendGump(new BoatGump(from, m_Boat));
        }
    }

    public class BoatRemoveGump : Gump
    {
        private BaseBoat m_Boat;
        private ArrayList m_List, m_Copy;
        private int m_Number;
        private bool m_AccountOf;

        public BoatRemoveGump(int number, ArrayList list, BaseBoat boat, bool accountOf)
            : base(20, 30)
        {
            if (boat.Deleted)
                return;

            m_Boat = boat;
            m_List = list;
            m_Number = number;
            m_AccountOf = accountOf;

            AddPage(0);

            AddBackground(0, 0, 420, 430, 5054);
            AddBackground(10, 10, 400, 410, 3000);

            AddButton(20, 388, 4005, 4007, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(55, 388, 300, 20, 1011104, false, false); // Return to previous menu

            AddButton(20, 365, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(55, 365, 300, 20, 1011270, false, false); // Remove now!

            AddHtmlLocalized(20, 20, 350, 20, number, false, false);

            if (list != null)
            {
                m_Copy = new ArrayList(list);

                for (int i = 0; i < list.Count; ++i)
                {
                    if ((i % 15) == 0)
                    {
                        if (i != 0)
                        {
                            // Next button
                            AddButton(370, 20, 4005, 4007, 0, GumpButtonType.Page, (i / 15) + 1);
                        }

                        AddPage((i / 15) + 1);

                        if (i != 0)
                        {
                            // Previous button
                            AddButton(340, 20, 4014, 4016, 0, GumpButtonType.Page, i / 15);
                        }
                    }

                    Mobile m = (Mobile)list[i];

                    string name;

                    if (m == null || (name = m.Name) == null || (name = name.Trim()).Length <= 0)
                        continue;

                    AddCheck(34, 52 + ((i % 15) * 20), 0xD2, 0xD3, false, i);
                    AddLabel(55, 52 + ((i % 15) * 20), 0, accountOf && m.Player && m.Account != null ? String.Format("Account of {0}", name) : name);
                }
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (m_Boat.Deleted)
                return;

            Mobile from = state.Mobile;

            if (m_List != null && info.ButtonID == 1) // Remove now
            {
                int[] switches = info.Switches;

                if (switches.Length > 0)
                {
                    for (int i = 0; i < switches.Length; ++i)
                    {
                        int index = switches[i];

                        if (index >= 0 && index < m_Copy.Count)
                            m_List.Remove(m_Copy[index]);
                    }

                    if (m_List.Count > 0)
                    {
                        from.CloseGump(typeof(BoatGump));
                        from.CloseGump(typeof(BoatListGump));
                        from.CloseGump(typeof(BoatRemoveGump));
                        from.SendGump(new BoatRemoveGump(m_Number, m_List, m_Boat, m_AccountOf));
                        return;
                    }
                }
            }

            from.SendGump(new BoatGump(from, m_Boat));
        }
    }

    public class BoatGump : Gump
    {
        private BaseBoat m_Boat;

        private ArrayList Wrap(string value)
        {
            if (value == null || (value = value.Trim()).Length <= 0)
                return null;

            string[] values = value.Split(' ');
            ArrayList list = new ArrayList();
            string current = "";

            for (int i = 0; i < values.Length; ++i)
            {
                string val = values[i];

                string v = current.Length == 0 ? val : current + ' ' + val;

                if (v.Length < 10)
                {
                    current = v;
                }

                else if (v.Length == 10)
                {
                    list.Add(v);

                    if (list.Count == 6)
                        return list;

                    current = "";
                }

                else if (val.Length <= 10)
                {
                    list.Add(current);

                    if (list.Count == 6)
                        return list;

                    current = val;
                }

                else
                {
                    while (v.Length >= 10)
                    {
                        list.Add(v.Substring(0, 10));

                        if (list.Count == 6)
                            return list;

                        v = v.Substring(10);
                    }

                    current = v;
                }
            }

            if (current.Length > 0)
                list.Add(current);

            return list;
        }

        public class ShipUpgradesGump : Gump
        {   
            BaseBoat m_Boat;
            Mobile m_From;
            int m_UpgradePage = 0;

            public ShipUpgradesGump(BaseBoat boat, Mobile from, int upgradePage): base(0, 0)
            {
                if (boat == null || from == null)
                    return;
                
                m_Boat = boat;
                m_From = from;
                m_UpgradePage = upgradePage;

                Closable = true;
                Disposable = true;
                Dragable = true;
                Resizable = false;

                AddImage(397, 462, 103);
                AddImage(266, 462, 103);
                AddImage(133, 462, 103);
                AddImage(3, 463, 103);
                AddImage(2, 0, 103);
                AddImage(131, 0, 103);
                AddImage(264, 0, 103);
                AddImage(397, 0, 103);
                AddImage(397, 88, 103);
                AddImage(397, 181, 103);
                AddImage(397, 276, 103);
                AddImage(2, 86, 103);
                AddImage(2, 180, 103);
                AddImage(2, 277, 103);
                AddImage(3, 372, 103);
                AddImage(133, 384, 103);
                AddImage(266, 384, 103);
                AddImage(397, 371, 103);

                AddBackground(14, 13, 515, 537, 3000);

                AddItem(427, 215, 5365);
                AddItem(54, 57, 5363);
                AddItem(413, 202, 5367);
                AddItem(39, 210, 5368);
                AddItem(53, 103, 4033);
                AddItem(327, 110, 7141);
                AddItem(218, 180, 710);
                AddItem(54, 130, 4009);               
                AddItem(8, 12, 6657);
                AddItem(325, 57, 6921);
                AddItem(-2, 507, 3703);
                AddItem(265, 215, 7866);
                AddItem(78, 211, 3700);
                AddItem(316, 47, 2462);
                AddItem(15, 482, 7723);

                int textHue = 2036;
                int boldHue = 149;

                BaseBoatOutfittingUpgradeDeed outfitting = m_Boat.GetOutfittingUpgrade();
                BaseBoatThemeUpgradeDeed theme = m_Boat.GetThemeUpgrade();
                BaseBoatPaintUpgradeDeed paint = m_Boat.GetPaintUpgrade();
                BaseBoatCannonMetalUpgradeDeed cannonMetal = m_Boat.GetCannonMetalUpgrade();

                string sOutfitting = "None";
                if (outfitting != null)
                    sOutfitting = outfitting.DisplayName;

                string sTheme = "None";
                if (theme != null)
                    sTheme = theme.DisplayName;

                string sPaint = "None";
                if (paint != null)
                    sPaint = paint.DisplayName;

                string sCannonMetal = "None";
                if (cannonMetal != null)
                    sCannonMetal = cannonMetal.DisplayName;

                //Upgrades
                AddLabel(207, 17, boldHue, "Installed Upgrades");

                AddLabel(96, 50, boldHue, "Outfitting");
                AddLabel(96, 70, textHue, sOutfitting);

                AddLabel(374, 50, boldHue, "Theme");
                AddLabel(374, 69, textHue, sTheme);

                AddLabel(96, 105, boldHue, "Paint");
                AddLabel(96, 125, textHue, sPaint);

                AddLabel(374, 105, boldHue, "Cannon Metal");
                AddLabel(374, 125, textHue, sCannonMetal);

                int abilityhueText = textHue;

                string sViewing = "";

                int textX = 0;

                //Ability Types
                if (m_UpgradePage == 0)
                {
                    abilityhueText = boldHue;
                    sViewing = "Displaying";
                    textX = 48;
                }

                else
                {
                    abilityhueText = textHue;
                    sViewing = "View";
                    textX = 63;
                }

                AddLabel(36, 179, abilityhueText, "Active Abilities");
                AddLabel(textX, 279, abilityhueText, sViewing);
                AddButton(63, 254, 4017, 4019, 11, GumpButtonType.Reply, 0);

                if (m_UpgradePage == 1)
                {
                    abilityhueText = boldHue;
                    sViewing = "Displaying";
                    textX = 231;
                }

                else
                {
                    abilityhueText = textHue;
                    sViewing = "View";
                    textX = 248;
                }

                AddLabel(221, 179, abilityhueText, "Epic Abilities");
                AddLabel(textX, 279, abilityhueText, sViewing);
                AddButton(247, 254, 4017, 4019, 12, GumpButtonType.Reply, 0);

                if (m_UpgradePage == 2)
                {
                    abilityhueText = boldHue;
                    sViewing = "Displaying";
                    textX = 404;
                }

                else
                {
                    abilityhueText = textHue;
                    sViewing = "View";
                    textX = 420;
                }

                AddLabel(387, 179, abilityhueText, "Passive Abilities");
                AddLabel(textX, 279, abilityhueText, sViewing);
                AddButton(419, 254, 4017, 4019, 13, GumpButtonType.Reply, 0);

                //Current Abilities
                string sInstalled = "";

                switch (m_UpgradePage)
                {
                    case 0: sInstalled = "Installed Active Abilities"; break;
                    case 1: sInstalled = "Installed Epic Abilities"; break;
                    case 2: sInstalled = "Installed Passive Abilities"; break;
                }

                AddLabel(265 - (sInstalled.Length * 3), 310, boldHue, sInstalled);

                List<BaseBoatUpgradeDeed> m_Upgrades = new List<BaseBoatUpgradeDeed>();

                switch (m_UpgradePage)
                {
                    case 0:
                        List<BaseBoatActiveAbilityUpgradeDeed> m_ActiveUpgrades = m_Boat.GetActiveAbilityUpgrades();
                        
                        foreach (BaseBoatActiveAbilityUpgradeDeed activeUpgrade in m_ActiveUpgrades)
                        {
                            m_Upgrades.Add(activeUpgrade as BaseBoatUpgradeDeed);
                        }
                    break;

                    case 1:
                        List<BaseBoatEpicAbilityUpgradeDeed> m_EpicUpgrades = m_Boat.GetEpicAbilityUpgrades();

                        foreach (BaseBoatEpicAbilityUpgradeDeed epicUpgrade in m_EpicUpgrades)
                        {
                            m_Upgrades.Add(epicUpgrade as BaseBoatUpgradeDeed);
                        }
                    break;

                    case 2:
                        List<BaseBoatPassiveAbilityUpgradeDeed> m_PassiveUpgrades = m_Boat.GetPassiveAbilityUpgrades();

                        foreach (BaseBoatPassiveAbilityUpgradeDeed PassiveUpgrade in m_PassiveUpgrades)
                        {
                            m_Upgrades.Add(PassiveUpgrade as BaseBoatUpgradeDeed);
                        }
                    break;
                }

                string abilityTimeRemaining = "";
                bool abilityReady = false;

                switch (m_UpgradePage)
                {
                    case 0:
                        if (m_Boat.m_NextActiveAbilityAllowed > DateTime.UtcNow)
                            abilityTimeRemaining = "Next Active Ability Usable in " + Utility.CreateTimeRemainingString(DateTime.UtcNow, m_Boat.m_NextActiveAbilityAllowed, false, false, false, true, true);
                        else
                        {
                            abilityTimeRemaining = "Active Ability Ready";
                            abilityReady = true;
                        }
                        break;

                    case 1:
                        if (m_Boat.m_NextEpicAbilityAllowed > DateTime.UtcNow)
                            abilityTimeRemaining = "Next Epic Ability Usable in " + Utility.CreateTimeRemainingString(DateTime.UtcNow, m_Boat.m_NextEpicAbilityAllowed, false, false, false, true, true);
                        else
                        {
                            abilityTimeRemaining = "Epic Ability Ready";
                            abilityReady = true;
                        }
                        break;

                    case 2:
                        abilityTimeRemaining = "Passive Abilities Are Always Active";
                        abilityReady = true;
                        break;
                }

                var iStartY = 340;
                
                for (int a = 0; a < m_Upgrades.Count; a++)
                {
                    if (a <= 4)
                    {
                        if (m_UpgradePage != 2)
                        {
                            if (abilityReady)
                                AddButton(55, iStartY, 2151, 2154, a + 1, GumpButtonType.Reply, 0);
                            else
                                AddButton(55, iStartY, 9721, 9724, a + 1, GumpButtonType.Reply, 0);
                        }

                        AddLabel(90, iStartY + 2, textHue, m_Upgrades[a].DisplayName);
                    }

                    else
                    {
                        if (m_UpgradePage != 2)
                        {
                            if (abilityReady)
                                AddButton(325, iStartY, 2151, 2154, a + 1, GumpButtonType.Reply, 0);
                            else
                                AddButton(325, iStartY, 9721, 9724, a + 1, GumpButtonType.Reply, 0);
                        }

                        AddLabel(360, iStartY, textHue, m_Upgrades[a].DisplayName);
                    }

                    iStartY += 35;

                    if (a == 4)
                        iStartY = 340;
                }                           

                int timeHue = textHue;

                if (abilityReady)
                    timeHue = boldHue;

                AddItem(230 - (abilityTimeRemaining.Length * 3), 516, 6160); //Hourglass
                AddLabel(265 - (abilityTimeRemaining.Length * 3), 519, timeHue, abilityTimeRemaining);
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                Mobile from = sender.Mobile;

                if (from == null || m_Boat == null) return;
                if (!from.Alive || m_Boat.Deleted) return;

                if (m_Boat.m_SinkTimer != null)
                {
                    if (m_Boat.m_SinkTimer.Running)
                        return;
                }

                if (info.ButtonID == 0)
                    return;

                if (info.ButtonID > 0 && info.ButtonID < 11)
                {
                    if (m_UpgradePage == 0)
                    {
                        if (DateTime.UtcNow < m_Boat.m_NextActiveAbilityAllowed && from.AccessLevel == AccessLevel.Player)
                        {
                            from.SendMessage("You must wait another " + Utility.CreateTimeRemainingString(DateTime.UtcNow, m_Boat.m_NextActiveAbilityAllowed, false, false, false, true, true) + " before using another active ability on the ship.");
                            
                            from.CloseGump(typeof(BoatGump.ShipUpgradesGump));
                            from.SendGump(new BoatGump.ShipUpgradesGump(m_Boat, from, m_UpgradePage));
                        }

                        else
                        {
                            BaseBoatActiveAbilityUpgradeDeed upgradeDeed = m_Boat.GetActiveAbilityUpgrades()[info.ButtonID - 1];
                            ActiveAbilityType activeAbility = upgradeDeed.ActiveAbility;

                            m_Boat.m_ActiveAbility = activeAbility;
                            from.SendMessage("You activate the " + upgradeDeed.DisplayName + " active ability on the ship.");

                            if (m_Boat.TillerMan != null)
                                m_Boat.TillerMan.Say(upgradeDeed.DisplayName + " ready sir!");

                            m_Boat.m_ActiveAbilityExpiration = DateTime.UtcNow + TimeSpan.FromSeconds(BaseBoat.ActiveAbilityDuration);

                            if (from.AccessLevel == AccessLevel.Player)
                                m_Boat.m_NextActiveAbilityAllowed = DateTime.UtcNow + TimeSpan.FromSeconds(BaseBoat.ActiveAbilityCooldown);
                        }

                        from.CloseGump(typeof(BoatGump.ShipUpgradesGump));
                        from.SendGump(new BoatGump.ShipUpgradesGump(m_Boat, from, m_UpgradePage));

                        return;
                    }

                    if (m_UpgradePage == 1)
                    {
                        if (DateTime.UtcNow < m_Boat.m_NextEpicAbilityAllowed && from.AccessLevel == AccessLevel.Player)
                            from.SendMessage("You must wait another " + Utility.CreateTimeRemainingString(DateTime.UtcNow, m_Boat.m_NextEpicAbilityAllowed, false, false, false, true, true) + " before using another epic ability on the ship.");
                        
                        else
                        {
                            BaseBoatEpicAbilityUpgradeDeed upgradeDeed = m_Boat.GetEpicAbilityUpgrades()[info.ButtonID - 1];
                            EpicAbilityType epicAbility = upgradeDeed.EpicAbility;

                            m_Boat.m_EpicAbility = epicAbility;
                            from.SendMessage("You activate the " + upgradeDeed.DisplayName + " epic ability on the ship.");
                            
                            if (m_Boat.TillerMan != null)
                                m_Boat.TillerMan.Say(upgradeDeed.DisplayName + " ready sir!");
                            
                                m_Boat.m_EpicAbilityExpiration = DateTime.UtcNow + TimeSpan.FromSeconds(BaseBoat.EpicAbilityDuration);

                                if (from.AccessLevel == AccessLevel.Player)
                                {
                                    double cooldown = BaseBoat.EpicAbilityCooldown;

                                    if (m_Boat.GuildDock != null)
                                    {
                                        if (GuildDockPersistance.BoatHasGuildDockUpgrade(m_Boat, GuildDockUpgradeType.MunitionsSpecialist))
                                            cooldown *= BaseBoat.MunitionsSpecialistcEpicAbilityCooldownScalar;
                                    }

                                    m_Boat.m_NextEpicAbilityAllowed = DateTime.UtcNow + TimeSpan.FromSeconds(cooldown);
                                }
                        }

                        from.CloseGump(typeof(BoatGump.ShipUpgradesGump));
                        from.SendGump(new BoatGump.ShipUpgradesGump(m_Boat, from, m_UpgradePage));

                        return;
                    }
                }

                if (info.ButtonID == 11)
                {
                    m_UpgradePage = 0;

                    from.CloseGump(typeof(BoatGump.ShipUpgradesGump));
                    from.SendGump(new BoatGump.ShipUpgradesGump(m_Boat, from, m_UpgradePage));

                    return;
                }

                if (info.ButtonID == 12)
                {
                    m_UpgradePage = 1;

                    from.CloseGump(typeof(BoatGump.ShipUpgradesGump));
                    from.SendGump(new BoatGump.ShipUpgradesGump(m_Boat, from, m_UpgradePage));

                    return;
                }

                if (info.ButtonID == 13)
                {
                    m_UpgradePage = 2;

                    from.CloseGump(typeof(BoatGump.ShipUpgradesGump));
                    from.SendGump(new BoatGump.ShipUpgradesGump(m_Boat, from, m_UpgradePage));

                    return;
                }
            }
        }

        public BoatGump(Mobile from, BaseBoat boat): base(20, 30)
        {
            if (boat.Deleted || boat.m_ScuttleInProgress)
                return;

            m_Boat = boat;

            from.CloseGump(typeof(BoatGump));
            from.CloseGump(typeof(BoatListGump));
            from.CloseGump(typeof(BoatRemoveGump));

            //Refresh Boat
            if (m_Boat.IsOwner(from) || m_Boat.IsCoOwner(from) || m_Boat.IsFriend(from))            
                m_Boat.Refresh();            

            AddPage(0);

            if (m_Boat.IsFriend(from) || m_Boat.IsCoOwner(from) || m_Boat.IsOwner(from))
            {
                AddBackground(0, 0, 420, 430, 5054);
                AddBackground(10, 10, 400, 410, 3000);
            }

            AddImage(130, 0, 100);

            if (m_Boat.ShipName != null)
            {
                ArrayList lines = Wrap(m_Boat.ShipName);

                if (lines != null)
                {
                    for (int i = 0, y = (101 - (lines.Count * 14)) / 2; i < lines.Count; ++i, y += 14)
                    {
                        string s = (string)lines[i];

                        AddLabel(130 + ((143 - (s.Length * 8)) / 2), y, 0, s);
                    }
                }
            }

            if (!m_Boat.IsFriend(from) && !m_Boat.IsCoOwner(from) && !m_Boat.IsOwner(from))
                return;

            AddHtml(55, 103, 75, 20, "Ship Info", false, false);
            AddButton(20, 103, 4005, 4007, 0, GumpButtonType.Page, 1);

            AddHtml(170, 103, 75, 20, "Ownership", false, false);
            AddButton(135, 103, 4005, 4007, 0, GumpButtonType.Page, 2);

            AddHtml(295, 103, 75, 20, "Options", false, false);
            AddButton(260, 103, 4005, 4007, 0, GumpButtonType.Page, 3);
            
            AddButton(20, 390, 4005, 4007, 18, GumpButtonType.Reply, 0);
            AddHtml(55, 390, 200, 20, "Upgrades", false, false);            

            AddButton(115, 390, 4005, 4007, 16, GumpButtonType.Reply, 0);
            AddHtml(150, 390, 200, 20, "Dock", false, false);

            AddButton(190, 390, 4005, 4007, 21, GumpButtonType.Reply, 0);
            AddHtml(225, 390, 200, 20, "Divide Plunder", false, false);

            AddButton(320, 390, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtml(355, 390, 75, 20, "Rename", false, false);
           
            // Info page
            AddPage(1);

            AddHtml(150, 130, 100, 20, "Owned by: ", false, false);
            AddHtml(225, 130, 100, 20, GetOwnerName(), false, false);

            AddHtml(25, 150, 200, 20, "Items in Hold:", false, false);
            AddHtml(145, 150, 50, 20, m_Boat.Hold.TotalItems.ToString(), false, false);

            /*
            //Left: Combat Stats
            AddHtml(25, 175, 200, 20, "Player Ships Sunk:", false, false);
            AddHtml(145, 175, 150, 20, m_Boat.playerShipsSunk.ToString(), false, false);

            AddHtml(25, 200, 200, 20, "Other Ships Sunk:", false, false);
            AddHtml(145, 200, 150, 20, m_Boat.NPCShipsSunk.ToString(), false, false);

            AddHtml(25, 225, 250, 20, "Doubloons Earned:", false, false);
            AddHtml(145, 225, 150, 20, m_Boat.doubloonsEarned.ToString(), false, false);

            //Right: Adventure Stats
            AddHtml(220, 175, 200, 20, "Nets Cast:", false, false);
            AddHtml(325, 175, 150, 20, m_Boat.netsCast.ToString(), false, false);

            AddHtml(220, 200, 275, 20, "MIBs Recovered:", false, false);
            AddHtml(325, 200, 150, 20, m_Boat.MIBsRecovered.ToString(), false, false);

            AddHtml(220, 225, 275, 20, "Fish Caught:", false, false);
            AddHtml(325, 225, 150, 20, m_Boat.fishCaught.ToString(), false, false);
            */

            //Ship Stats
            int hullPercent = (int)(Math.Floor(100 * (float)boat.HitPoints / (float)boat.MaxHitPoints));
            AddHtml(25, 250, 200, 20, "Ship's Hull: ", false, false);
            AddHtml(100, 250, 300, 20, boat.HitPoints.ToString() + "/" + boat.MaxHitPoints.ToString() + " (" + hullPercent.ToString() + "%)", false, false);

            int sailPercent = (int)(Math.Floor(100 * (float)boat.SailPoints / (float)boat.MaxSailPoints));
            AddHtml(25, 275, 200, 20, "Ship's Sails: ", false, false);
            AddHtml(100, 275, 300, 20, boat.SailPoints.ToString() + "/" + boat.MaxSailPoints.ToString() + " (" + sailPercent.ToString() + "%)", false, false);

            int gunPercent = (int)(Math.Floor(100 * (float)boat.GunPoints / (float)boat.MaxGunPoints));
            AddHtml(25, 300, 200, 20, "Ship's Guns: ", false, false);
            AddHtml(100, 300, 300, 20, boat.GunPoints.ToString() + "/" + boat.MaxGunPoints.ToString() + " (" + gunPercent.ToString() + "%)", false, false);

            AddButton(20, 325, 4005, 4007, 12, GumpButtonType.Reply, 0);
            AddHtml(55, 325, 200, 20, "Embark", false, false);            

            AddButton(20, 350, 4005, 4007, 13, GumpButtonType.Reply, 0);
            AddHtml(55, 350, 200, 20, "Embark All Followers", false, false);            

            AddButton(225, 325, 4005, 4007, 14, GumpButtonType.Reply, 0);
            AddHtml(260, 325, 200, 20, "Disembark", false, false);           

            AddButton(225, 350, 4005, 4007, 15, GumpButtonType.Reply, 0);
            AddHtml(260, 350, 200, 20, "Disembark All Followers", false, false);           

            // Friends page
            AddPage(2);

            AddHtmlLocalized(45, 130, 150, 20, 1011266, false, false); // List of co-owners
            AddButton(20, 130, 2714, 2715, 2, GumpButtonType.Reply, 0);

            AddHtmlLocalized(45, 150, 150, 20, 1011267, false, false); // Add a co-owner
            AddButton(20, 150, 2714, 2715, 3, GumpButtonType.Reply, 0);

            AddHtmlLocalized(45, 170, 150, 20, 1018036, false, false); // Remove a co-owner
            AddButton(20, 170, 2714, 2715, 4, GumpButtonType.Reply, 0);

            AddHtmlLocalized(45, 190, 150, 20, 1011268, false, false); // Clear co-owner list
            AddButton(20, 190, 2714, 2715, 5, GumpButtonType.Reply, 0);

            AddHtmlLocalized(225, 130, 155, 20, 1011243, false, false); // List of Friends
            AddButton(200, 130, 2714, 2715, 6, GumpButtonType.Reply, 0);

            AddHtmlLocalized(225, 150, 155, 20, 1011244, false, false); // Add a Friend
            AddButton(200, 150, 2714, 2715, 7, GumpButtonType.Reply, 0);

            AddHtmlLocalized(225, 170, 155, 20, 1018037, false, false); // Remove a Friend
            AddButton(200, 170, 2714, 2715, 8, GumpButtonType.Reply, 0);

            AddHtmlLocalized(225, 190, 155, 20, 1011245, false, false); // Clear Friends list
            AddButton(200, 190, 2714, 2715, 9, GumpButtonType.Reply, 0);

            // Options page
            AddPage(3);
            
            AddHtml(45, 130, 355, 30, "Scuttle The Ship", false, false);
            AddButton(20, 130, 2714, 2715, 11, GumpButtonType.Reply, 0);

            AddHtml(45, 150, 355, 30, "Grant Temporary Access", false, false);
            AddButton(20, 150, 2714, 2715, 17, GumpButtonType.Reply, 0);

            if (m_Boat.GuildAsFriends)
                AddHtml(45, 170, 355, 30, "Auto-Friend Guildmates: Enabled", false, false);
            else
                AddHtml(45, 170, 355, 30, "Auto-Friend Guildmates: Disabled", false, false);
            AddButton(20, 170, 2714, 2715, 19, GumpButtonType.Reply, 0);

            AddHtml(45, 190, 355, 30, "Wash Ashore", false, false);
            AddButton(20, 190, 2714, 2715, 20, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Boat.Deleted)
                return;

            Mobile from = sender.Mobile;

            if (!(m_Boat.IsFriend(from) || m_Boat.IsCoOwner(from) || m_Boat.IsOwner(from)))
                return;

            switch (info.ButtonID)
            {
                case 1: // Rename Boat
                    {
                        if (m_Boat.IsOwner(from))
                        {
                            from.Prompt = new BoatRenamePrompt(m_Boat);
                            from.SendMessage("What do you wish to rename this boat to?");
                        }

                        from.CloseGump(typeof(BoatGump));
                        from.SendGump(new BoatGump(from, m_Boat));

                        break;
                    }

                case 2: // List of co-owners
                    {
                        from.CloseGump(typeof(BoatGump));
                        from.CloseGump(typeof(BoatListGump));
                        from.CloseGump(typeof(BoatRemoveGump));
                        from.SendGump(new BoatListGump(1011275, m_Boat.CoOwners, m_Boat, false));

                        break;
                    }

                case 3: // Add co-owner
                    {
                        if (m_Boat.IsOwner(from))
                        {
                            from.SendMessage("Target the person you wish to name a co-owner of this boat");
                            from.Target = new Server.Multis.BaseBoat.BoatCoOwnerTarget(true, m_Boat);
                        }

                        else
                        {
                            from.SendMessage("Only the boat owner may add co-owners");
                        }

                        break;
                    }

                case 4: // Remove co-owner
                    {
                        if (m_Boat.IsOwner(from))
                        {
                            from.CloseGump(typeof(BoatGump));
                            from.CloseGump(typeof(BoatListGump));
                            from.CloseGump(typeof(BoatRemoveGump));
                            from.SendGump(new BoatRemoveGump(1011274, m_Boat.CoOwners, m_Boat, false));
                        }

                        else
                        {
                            from.SendMessage("Only the boat owwner may remove co-owners");
                        }

                        break;
                    }

                case 5: // Clear co-owners
                    {
                        if (m_Boat.IsOwner(from))
                        {
                            if (m_Boat.CoOwners != null)
                                m_Boat.CoOwners.Clear();

                            from.SendMessage("All co-owners have been removed from this boat");
                        }

                        else
                        {
                            from.SendMessage("Only the boat owner may remove co-owners");
                        }

                        break;
                    }

                case 6: // List friends
                    {
                        from.CloseGump(typeof(BoatGump));
                        from.CloseGump(typeof(BoatListGump));
                        from.CloseGump(typeof(BoatRemoveGump));
                        from.SendGump(new BoatListGump(1011273, m_Boat.Friends, m_Boat, false));

                        break;
                    }

                case 7: // Add friend
                    {
                        if (m_Boat.IsOwner(from) || m_Boat.IsCoOwner(from))
                        {
                            from.SendMessage("Target the person you wish to name a friend of this boat");
                            from.Target = new Server.Multis.BaseBoat.BoatFriendTarget(true, m_Boat);
                        }

                        else
                        {
                            from.SendMessage("Only the boat owner may add friends");
                        }

                        break;
                    }

                case 8: // Remove friend
                    {
                        if (m_Boat.IsOwner(from) || m_Boat.IsCoOwner(from))
                        {
                            from.CloseGump(typeof(BoatGump));
                            from.CloseGump(typeof(BoatListGump));
                            from.CloseGump(typeof(BoatRemoveGump));
                            from.SendGump(new BoatRemoveGump(1011272, m_Boat.Friends, m_Boat, false));
                        }

                        else
                        {
                            from.SendMessage("Only the boat owner or co-owners may remove friends");
                        }

                        break;
                    }

                case 9: // Clear friends
                    {
                        if (m_Boat.IsOwner(from) || m_Boat.IsCoOwner(from))
                        {
                            if (m_Boat.Friends != null)
                                m_Boat.Friends.Clear();

                            from.SendMessage("All friends have been removed from this boat");
                        }

                        else
                        {
                            from.SendMessage("Only the boat owner or co-owners may remove friends");
                        }

                        break;
                    }

                case 10: // Transfer ownership
                    {
                        break;
                    }

                case 11: // Scuttle boat
                    {
                        if (!from.Alive)                        
                            from.SendMessage("You must be alive to do this.");
                                                   
                        else if (m_Boat.IsOwner(from))
                        {
                            from.CloseGump(typeof(BoatDemolishGump));
                            from.SendGump(new BoatDemolishGump(from, m_Boat));
                        }

                        else
                        {
                            from.SendMessage("Only the boat owner may do this.");
                        }

                        break;
                    }

                case 12: // Embark
                    {
                        if (!from.Alive)
                            from.SendMessage("You must be alive to do this.");

                        else if (m_Boat.IsOwner(from) || m_Boat.IsCoOwner(from) || m_Boat.IsFriend(from))
                            m_Boat.Embark(from, false);                        

                        else                        
                            from.SendMessage("Only friends of this boat may embark upon it");

                        from.CloseGump(typeof(BoatGump));
                        //from.SendGump(new BoatGump(from, m_Boat));

                        break;
                    }

                case 13: // Embark Followers
                    {
                        if (!from.Alive)
                            from.SendMessage("You must be alive to do this.");

                        else if (m_Boat.IsOwner(from) || m_Boat.IsCoOwner(from) || m_Boat.IsFriend(from))                        
                            m_Boat.EmbarkFollowers(from);                        

                        else                        
                            from.SendMessage("Only friends of this boat may embark their followers upon it");

                        from.CloseGump(typeof(BoatGump));
                        //from.SendGump(new BoatGump(from, m_Boat));

                        break;
                    }

                case 14: // Disembark
                    {
                        m_Boat.Disembark(from);

                        from.CloseGump(typeof(BoatGump));
                        //from.SendGump(new BoatGump(from, m_Boat));

                        break;
                    }

                case 15: // Disembark Followers
                    {
                        m_Boat.DisembarkFollowers(from);

                        from.CloseGump(typeof(BoatGump));
                        //from.SendGump(new BoatGump(from, m_Boat));

                        break;
                    }

                case 16: // Dry Dock Ship
                    {
                        if (!from.Alive)
                            from.SendMessage("You must be alive to do this.");

                        else if (m_Boat.IsOwner(from))
                        {
                            from.CloseGump(typeof(BoatGump));
                            from.CloseGump(typeof(BoatListGump));
                            from.CloseGump(typeof(BoatRemoveGump));

                            m_Boat.BeginDryDock(from);
                        }

                        else                        
                            from.SendMessage("Only the owner of this boat may dry dock it");                        

                        break;
                    }

                case 18: // View Upgrades
                {
                    if (!from.Alive)
                        from.SendMessage("You must be alive to do this.");

                    else if (m_Boat.IsOwner(from) || m_Boat.IsCoOwner(from))
                    {
                        from.CloseGump(typeof(ShipUpgradesGump));
                        from.SendGump(new ShipUpgradesGump(m_Boat, from, 0));
                    }

                    break;
                }

                case 19: // Auto-Guild Toggle
                {
                    if (m_Boat.IsOwner(from) || m_Boat.IsCoOwner(from))
                    {
                        if (m_Boat.GuildAsFriends)
                        {
                            from.CloseGump(typeof(BoatGump));
                            from.SendGump(new BoatGump(from, m_Boat));

                            m_Boat.GuildAsFriends = false;
                            from.SendMessage("Auto-Friend Guildmates disabled.");
                        }

                        else
                        {
                            from.CloseGump(typeof(BoatGump));
                            from.SendGump(new BoatGump(from, m_Boat));

                            m_Boat.GuildAsFriends = true;
                            from.SendMessage("Auto-Friend Guildmates enabled.");
                        }
                    }

                    break;
                }

                case 20: // Wash Ashore
                {
                    if (!m_Boat.Contains(from.Location))
                        from.SendMessage("You must be on board this ship to do that.");

                    else
                    {
                        from.CloseAllGumps();
                        from.SendGump(new WashAshoreGump(from, m_Boat));
                    }

                    break;
                }

                case 21: // Divide The Plunder
                {
                    if (!m_Boat.IsOwner(from))
                        from.SendMessage("Only the owner of this ship may divide the plunder.");

                    else
                        m_Boat.BeginDivideThePlunder(from);

                    break;
                }
            }
        }

        private string GetOwnerName()
        {
            Mobile m = m_Boat.Owner;

            if (m == null)
                return "(unowned)";

            string name;

            if ((name = m.Name) == null || (name = name.Trim()).Length <= 0)
                name = "(no name)";

            return name;
        }
    }

    public class WashAshoreGump : Gump
    {
        private Mobile m_Mobile;
        private BaseBoat m_Boat;

        public WashAshoreGump(Mobile mobile, BaseBoat boat): base(110, 100)
        {
            m_Mobile = mobile;
            m_Boat = boat;            

            Closable = false;

            AddPage(0);

            AddBackground(0, 0, 420, 280, 5054);

            AddImageTiled(10, 10, 400, 20, 2624);
            AddAlphaRegion(10, 10, 400, 20);

            AddHtmlLocalized(10, 10, 400, 20, 1060635, 30720, false, false); // <CENTER>WARNING</CENTER>

            AddImageTiled(10, 40, 400, 200, 2624);
            AddAlphaRegion(10, 40, 400, 200);

            AddHtml(10, 40, 400, 200, "You are about to jump off the ship, killing yourself and your tamed creatures onboard, and washing ashore to a nearby land location as a ghost. Are you sure you wish to proceed?", true, false);

            AddImageTiled(10, 250, 400, 20, 2624);
            AddAlphaRegion(10, 250, 400, 20);

            AddButton(10, 250, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(40, 250, 170, 20, 1011036, 32767, false, false); // OKAY

            AddButton(210, 250, 4005, 4007, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(240, 250, 170, 20, 1011012, 32767, false, false); // CANCEL
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (m_Boat == null || m_Mobile == null) return;
            if (m_Boat.Deleted || m_Mobile.Deleted) return;

            if (info.ButtonID == 1)
            {
                List<Mobile> m_Mobiles = m_Boat.GetMobilesOnBoat(true, true);
                List<Mobile> creaturesToKill = new List<Mobile>();

                foreach (Mobile mobile in m_Mobiles)
                {
                    BaseCreature bc_Creature = mobile as BaseCreature;

                    if (bc_Creature != null)
                    {
                        if (bc_Creature.Controlled && bc_Creature.ControlMaster == m_Mobile)
                            creaturesToKill.Add(bc_Creature);
                    }
                }

                Custom.Pirates.PirateHelper.KillAtSea(m_Mobile);

                foreach (BaseCreature creature in creaturesToKill)
                {
                    creature.Kill();

                    if (creature.IsBonded)
                        creature.MoveToWorld(creature.ControlMaster.Location, creature.ControlMaster.Map);
                }   
            }
        }
    }

    public enum DivideMode
    {
        CaptainOnly,
        OwnerCoOwnerEqual,
        RankedShares,
        Equal
    }

    public class DivideThePlunderGump : Gump
    {
        private Mobile m_Mobile;
        private BaseBoat m_Boat;

        private DivideMode m_DivideMode = DivideMode.CaptainOnly;

        int doubloonTotal = 0;
        double totalShares = 3;

        double captainShareValue = 3;
        double coownerShareValue = 2;
        double friendShareValue = 1;

        double totalParticipants = 1;

        double participatingCaptains = 1;
        double participatingCoowners = 0;
        double participatingFriends = 0;

        int doubloonsPerCaptain = 0;
        int doubloonsPerCoowner = 0;
        int doubloonsPerFriend = 0;   

        public DivideThePlunderGump(Mobile mobile, BaseBoat boat, DivideMode divideMode): base(110, 100)
        {
            m_Mobile = mobile;
            m_Boat = boat;

            m_DivideMode = divideMode;

            if (m_Mobile == null) return;
            if (m_Mobile.Deleted) return;
            if (m_Boat == null) return;
            if (m_Boat.Deleted || m_Boat.m_SinkTimer != null) return;
            if (m_Boat.Owner == null) return;
            if (m_Boat.Owner.Deleted) return;
            
            Closable = false;

            AddPage(0);

            AddBackground(0, 0, 400, 500, 5054);
            AddBackground(10, 10, 380, 480, 3000);

            AddItem(247, 168, 3226);
            AddItem(261, 217, 2321);
            AddItem(227, 213, 4967);
            AddItem(238, 237, 2321);
            AddItem(255, 78, 2539);           
            AddItem(259, 255, 2322);
            AddItem(285, 247, 2321);
            AddItem(210, 198, 3897);
            AddItem(278, 226, 4970);
            AddItem(266, 230, 2475);
            AddItem(263, 267, 4973);
            AddItem(304, 233, 4963);
            AddItem(310, 226, 2586);
            AddItem(322, 224, 3227);
            AddItem(225, 236, 3228);
            AddItem(281, 153, 3229);
            AddItem(292, 271, 6009);
            AddItem(262, 266, 2321);
            AddItem(288, 271, 4093);
            AddItem(291, 272, 5185);
            AddItem(44, 67, 4654, 2579);
            AddItem(53, 44, 4655, 2579);
            AddItem(41, 35, 4653, 2579);
            AddItem(15, 74, 4650, 2579);
            AddItem(52, 81, 15998);
            AddItem(88, 86, 15994);
            AddItem(68, 67, 16003);
            AddItem(104, 52, 15999);
            AddItem(66, 90, 15974);
            AddItem(105, 79, 15976);
            AddItem(85, 34, 16013);
            AddItem(72, 28, 3120);
            AddItem(22, 87, 3119);
            AddItem(48, 53, 3118);
            AddItem(28, 51, 3117);
            AddItem(66, 74, 4651, 2579);
            AddItem(108, 69, 5368);
            AddItem(41, 100, 5369);
            AddItem(162, 110, 6005);
            AddItem(154, 118, 5366);
            AddItem(162, 100, 5355);
            AddItem(176, 105, 3921);
            AddItem(202, 48, 16645);
            AddItem(208, 92, 2539);
            AddItem(196, 88, 2539);
            AddItem(198, 80, 2538);
            AddItem(220, 377, 2539);
            AddItem(220, 403, 2539);
            AddItem(220, 428, 2539);

            doubloonTotal = m_Boat.GetHoldDoubloonTotal(m_Boat);

            if (doubloonTotal == 0)
                return;

            //Make Sure Owner is Always In Participating Mobiles
            if (!m_Boat.ParticipatingMobiles.Contains(m_Boat.Owner))
                m_Boat.ParticipatingMobiles.Add(m_Boat.Owner);

            switch (m_DivideMode)
            {
                case DivideMode.CaptainOnly:
                    doubloonsPerCaptain = doubloonTotal;
                break;

                case DivideMode.OwnerCoOwnerEqual:
                    foreach (Mobile crewMember in m_Boat.ParticipatingMobiles)
                    {
                        if (crewMember == null) continue;
                        if (crewMember.Deleted) continue;
                        if (crewMember == m_Boat.Owner) continue;

                        if (m_Boat.IsCoOwner(crewMember))
                        {
                            participatingCoowners++;
                            totalParticipants++;                            
                        }
                    }
                    
                    doubloonsPerCaptain = (int)(Math.Ceiling(doubloonTotal * (1 / totalParticipants)));

                    if (participatingCoowners > 0)
                        doubloonsPerCoowner = (int)(Math.Ceiling(doubloonTotal * (1 / totalParticipants)));
                break;

                case DivideMode.RankedShares:
                    foreach (Mobile crewMember in m_Boat.ParticipatingMobiles)
                    {
                        if (crewMember == null) continue;
                        if (crewMember.Deleted) continue;
                        if (crewMember == m_Boat.Owner) continue;

                        if (m_Boat.IsCoOwner(crewMember))
                        {
                            participatingCoowners++;
                            totalParticipants++;
                            totalShares += coownerShareValue;
                        }

                        if (m_Boat.IsFriend(crewMember))
                        {
                            participatingFriends++;
                            totalParticipants++;
                            totalShares += friendShareValue;
                        }
                    }

                    doubloonsPerCaptain = (int)(Math.Ceiling(doubloonTotal * (captainShareValue / totalShares)));

                    if (participatingCoowners > 0)
                        doubloonsPerCoowner = (int)(Math.Ceiling(doubloonTotal * (coownerShareValue / totalShares)));

                    if (participatingFriends > 0)
                        doubloonsPerFriend = (int)(Math.Ceiling(doubloonTotal * (friendShareValue / totalShares)));
                break;

                case DivideMode.Equal:
                    foreach (Mobile crewMember in m_Boat.ParticipatingMobiles)
                    {
                        if (crewMember == null) continue;
                        if (crewMember.Deleted) continue;
                        if (crewMember == m_Boat.Owner) continue;

                        if (m_Boat.IsCoOwner(crewMember))
                        {
                            participatingCoowners++;
                            totalParticipants++; 
                        }

                        if (m_Boat.IsFriend(crewMember))
                        {
                            participatingFriends++;
                            totalParticipants++;    
                        }
                    }

                    doubloonsPerCaptain = (int)(Math.Ceiling(doubloonTotal * (1 / totalParticipants)));

                    if (participatingCoowners > 0)
                        doubloonsPerCoowner = (int)(Math.Ceiling(doubloonTotal * (1 / totalParticipants)));

                    if (participatingFriends > 0)
                        doubloonsPerFriend = (int)(Math.Ceiling(doubloonTotal * (1 / totalParticipants)));
                break;
            }

            int textHue = 2036;
            int goldHue = 149;

            AddLabel(150, 19, goldHue, "Divide the Plunder!");

            AddLabel(291, 76, goldHue, doubloonTotal.ToString());
            AddLabel(270, 97, goldHue, "Doubloons");

            AddLabel(33, 170, goldHue, "How shall we divide the loot?");

            AddLabel(88, 203, textHue, "Owner Only");
            if (m_DivideMode == DivideMode.CaptainOnly)
                AddButton(162, 200, 2154, 2152, 3, GumpButtonType.Reply, 0);
            else
                AddButton(162, 200, 2152, 2154, 3, GumpButtonType.Reply, 0);

            AddLabel(25, 239, textHue, "Owner and Co-Owners");
            if (m_DivideMode == DivideMode.OwnerCoOwnerEqual)
                AddButton(162, 235, 2154, 2152, 4, GumpButtonType.Reply, 0);
            else
                AddButton(162, 235, 2152, 2154, 4, GumpButtonType.Reply, 0);

            AddLabel(62, 273, textHue, "Shares By Rank");
            if (m_DivideMode == DivideMode.RankedShares)
                AddButton(162, 270, 2154, 2152, 5, GumpButtonType.Reply, 0);
            else
                AddButton(162, 270, 2152, 2154, 5, GumpButtonType.Reply, 0);

            AddLabel(81, 308, textHue, "Equal Shares");
            if (m_DivideMode == DivideMode.Equal)
                AddButton(162, 305, 2154, 2152, 6, GumpButtonType.Reply, 0);
            else
                AddButton(162, 305, 2152, 2154, 6, GumpButtonType.Reply, 0);

            AddLabel(120, 350, goldHue, "Participants");
            AddLabel(224, 350, goldHue, "Size of Share");

            AddLabel(105, 375, textHue, "Owner");
            AddLabel(81, 400, textHue, "Co-Owners");
            AddLabel(100, 425, textHue, "Friends");

            AddLabel(155, 375, textHue, participatingCaptains.ToString()); //Owner
            AddLabel(155, 400, textHue, participatingCoowners.ToString()); //Co-Owners
            AddLabel(155, 425, textHue, participatingFriends.ToString()); //Friends

            AddLabel(255, 375, textHue, doubloonsPerCaptain.ToString());
            AddLabel(255, 400, textHue, doubloonsPerCoowner.ToString());
            AddLabel(255, 425, textHue, doubloonsPerFriend.ToString());

            AddButton(50, 455, 247, 248, 1, GumpButtonType.Reply, 0);
            AddLabel(119, 455, goldHue, "Yarrr!");

            AddButton(233, 454, 242, 241, 2, GumpButtonType.Reply, 0);
            AddLabel(301, 454, goldHue, "Nay!");
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile sender = state.Mobile;

            if (sender == null) return;
            if (sender.Deleted) return;
            if (m_Boat == null) return;
            if (m_Boat.Deleted || m_Boat.m_SinkTimer != null) return;
            if (m_Boat.Owner == null) return;
            if (m_Boat.Owner.Deleted) return;
 
            bool reloadGump = false;            

            int doubloonsInHold = m_Boat.GetHoldDoubloonTotal(m_Boat);

            int doubloonDropSound = 0;

            Doubloon doubloonPile = new Doubloon(doubloonsInHold);
            doubloonDropSound = doubloonPile.GetDropSound();
            doubloonPile.Delete();

            if ((doubloonsInHold != doubloonTotal) || doubloonsInHold < 0)
            {
                sender.SendMessage("The number of doubloons in the ship's hold have changed. Shares will be recalculated...");              
                return;
            }

            if (info.ButtonID == 1)
            {
                double newTotalShares = 3;

                double newCaptainShareValue = 3;
                double newCoownerShareValue = 2;
                double newFriendShareValue = 1;

                double newTotalParticipants = 1;

                double newParticipatingCaptains = 1;
                double newParticipatingCoowners = 0;
                double newParticipatingFriends = 0;

                int newDoubloonsPerCaptain = 0;
                int newDoubloonsPerCoowner = 0;
                int newDoubloonsPerFriend = 0;

                PlayerMobile player;

                switch (m_DivideMode)
                {
                    case DivideMode.CaptainOnly:
                        newDoubloonsPerCaptain = (int)doubloonTotal;

                        if (newDoubloonsPerCaptain < 1)
                            newDoubloonsPerCaptain = 1;

                        player = m_Boat.Owner as PlayerMobile;

                        if (player != null)
                        {
                            player.PirateScore += newDoubloonsPerCaptain;
                            Banker.DepositUniqueCurrency(player, typeof(Doubloon), newDoubloonsPerCaptain);
                        }

                        player.SendSound(doubloonDropSound);
                        player.SendMessage("As the captain of the ship, you claim " + newDoubloonsPerCaptain.ToString() + " doubloons as your own. The doubloons have been placed in your bankbox.");
                        
                        m_Boat.DeleteDoubloonsInHold();
                    break;

                    case DivideMode.OwnerCoOwnerEqual:
                        foreach (Mobile crewMember in m_Boat.ParticipatingMobiles)
                        {
                            if (crewMember == null) continue;
                            if (crewMember.Deleted) continue;
                            if (crewMember == m_Boat.Owner) continue;

                            if (m_Boat.IsCoOwner(crewMember))
                            {
                                newParticipatingCoowners++;
                                newTotalParticipants++;
                            }
                        }

                        newDoubloonsPerCaptain = (int)(Math.Ceiling(doubloonTotal * (1 / newTotalParticipants)));

                        if (newParticipatingCoowners > 0)
                            newDoubloonsPerCoowner = (int)(Math.Ceiling(doubloonTotal * (1 / newTotalParticipants)));

                        if ((newDoubloonsPerCaptain != doubloonsPerCaptain) || (newDoubloonsPerCoowner != doubloonsPerCoowner))
                        {
                            sender.SendMessage("The number of ship participants have changed. Shares will be recalculated...");
                            sender.SendGump(new DivideThePlunderGump(m_Mobile, m_Boat, m_DivideMode));

                            return;
                        }

                        if ((newParticipatingCoowners != participatingCoowners))
                        {
                            sender.SendMessage("The number of ship participants have changed. Shares will be recalculated...");
                            sender.SendGump(new DivideThePlunderGump(m_Mobile, m_Boat, m_DivideMode));

                            return;
                        }

                        foreach (Mobile crewMember in m_Boat.ParticipatingMobiles)
                        {
                            if (crewMember == null) continue;
                            if (crewMember.Deleted) continue;
                            
                            if (crewMember == m_Boat.Owner)
                            {
                                if (newDoubloonsPerCaptain < 1)
                                    newDoubloonsPerCaptain = 1;

                                player = crewMember as PlayerMobile;

                                if (player != null)
                                {
                                    player.PirateScore += newDoubloonsPerCaptain;
                                    Banker.DepositUniqueCurrency(player, typeof(Doubloon), newDoubloonsPerCaptain);

                                    player.SendSound(doubloonDropSound);
                                    player.SendMessage("As the captain of the ship, you claim " + newDoubloonsPerCaptain.ToString() + " doubloons as your own. The doubloons have been placed in your bankbox.");
                                }
                            }

                            if (m_Boat.IsCoOwner(crewMember))
                            {
                                if (newDoubloonsPerCoowner < 1)
                                    newDoubloonsPerCoowner = 1;

                                player = crewMember as PlayerMobile;

                                if (player != null)
                                {
                                    player.PirateScore += newDoubloonsPerCoowner;
                                    Banker.DepositUniqueCurrency(player, typeof(Doubloon), newDoubloonsPerCoowner);

                                    player.SendSound(doubloonDropSound);
                                    player.SendMessage("As a co-owner of the ship, you claim " + newDoubloonsPerCoowner.ToString() + " doubloons as your own. The doubloons have been placed in your bankbox.");
                                }
                            }
                        }

                        m_Boat.DeleteDoubloonsInHold();
                    break;

                    case DivideMode.RankedShares:
                        foreach (Mobile crewMember in m_Boat.ParticipatingMobiles)
                        {
                            if (crewMember == null) continue;
                            if (crewMember.Deleted) continue;
                            if (crewMember == m_Boat.Owner) continue;

                            if (m_Boat.IsCoOwner(crewMember))
                            {
                                newParticipatingCoowners++;
                                newTotalParticipants++;
                                newTotalShares += newCoownerShareValue;
                            }

                            if (m_Boat.IsFriend(crewMember))
                            {
                                newParticipatingFriends++;
                                newTotalParticipants++;
                                newTotalShares += newFriendShareValue;
                            }
                        }

                        newDoubloonsPerCaptain = (int)(Math.Ceiling(doubloonTotal * (newCaptainShareValue / newTotalShares)));
                        
                        if (newParticipatingCoowners > 0)
                            newDoubloonsPerCoowner = (int)(Math.Ceiling(doubloonTotal * (newCoownerShareValue / newTotalShares)));

                        if (newParticipatingFriends > 0)
                            newDoubloonsPerFriend = (int)(Math.Ceiling(doubloonTotal * (newFriendShareValue / newTotalShares)));

                        if ((newDoubloonsPerCaptain != doubloonsPerCaptain) || (newDoubloonsPerCoowner != doubloonsPerCoowner) || (newDoubloonsPerFriend != doubloonsPerFriend))
                        {
                            sender.SendMessage("The number of ship participants have changed. Shares will be recalculated...");
                            sender.SendGump(new DivideThePlunderGump(m_Mobile, m_Boat, m_DivideMode));
                            
                            return;
                        }

                        if ((newParticipatingCoowners != participatingCoowners) || (newParticipatingFriends != participatingFriends))
                        {
                            sender.SendMessage("The number of ship participants have changed. Shares will be recalculated...");
                            sender.SendGump(new DivideThePlunderGump(m_Mobile, m_Boat, m_DivideMode));

                            return;
                        }

                        foreach (Mobile crewMember in m_Boat.ParticipatingMobiles)
                        {
                            if (crewMember == null) continue;
                            if (crewMember.Deleted) continue;
                            
                            if (crewMember == m_Boat.Owner)
                            {
                                if (newDoubloonsPerCaptain < 1)
                                    newDoubloonsPerCaptain = 1;

                                player = crewMember as PlayerMobile;

                                if (player != null)
                                {
                                    player.PirateScore += newDoubloonsPerCaptain;
                                    Banker.DepositUniqueCurrency(player, typeof(Doubloon), newDoubloonsPerCaptain);

                                    player.SendSound(doubloonDropSound);
                                    player.SendMessage("As the captain of the ship, you claim " + newDoubloonsPerCaptain.ToString() + " doubloons as your own. The doubloons have been placed in your bankbox.");
                                }
                            }

                            if (m_Boat.IsCoOwner(crewMember))
                            {
                                if (newDoubloonsPerCoowner < 1)
                                    newDoubloonsPerCoowner = 1;

                                player = crewMember as PlayerMobile;

                                if (player != null)
                                {
                                    player.PirateScore += newDoubloonsPerCoowner;
                                    Banker.DepositUniqueCurrency(player, typeof(Doubloon), newDoubloonsPerCoowner);

                                    player.SendSound(doubloonDropSound);
                                    player.SendMessage("As a co-owner of the ship, you claim " + newDoubloonsPerCoowner.ToString() + " doubloons as your own. The doubloons have been placed in your bankbox.");
                                }
                            }

                            if (m_Boat.IsFriend(crewMember))
                            {
                                if (newDoubloonsPerFriend < 1)
                                    newDoubloonsPerFriend = 1;

                                player = crewMember as PlayerMobile;

                                if (player != null)
                                {
                                    player.PirateScore += newDoubloonsPerFriend;
                                    Banker.DepositUniqueCurrency(player, typeof(Doubloon), newDoubloonsPerFriend);

                                    player.SendSound(doubloonDropSound);
                                    player.SendMessage("As a friend of the ship, you claim " + newDoubloonsPerFriend.ToString() + " doubloons as your own. The doubloons have been placed in your bankbox.");
                                }
                            }
                        }

                        m_Boat.DeleteDoubloonsInHold();
                    break;

                    case DivideMode.Equal:
                        foreach (Mobile crewMember in m_Boat.ParticipatingMobiles)
                        {
                            if (crewMember == null) continue;
                            if (crewMember.Deleted) continue;
                            if (crewMember == m_Boat.Owner) continue;

                            if (m_Boat.IsCoOwner(crewMember))
                            {
                                newParticipatingCoowners++;
                                newTotalParticipants++;
                            }

                            if (m_Boat.IsFriend(crewMember))
                            {
                                newParticipatingFriends++;
                                newTotalParticipants++;
                            }
                        }

                        newDoubloonsPerCaptain = (int)(Math.Ceiling(doubloonTotal * (1 / newTotalParticipants)));

                        if (newParticipatingCoowners > 0)
                            newDoubloonsPerCoowner = (int)(Math.Ceiling(doubloonTotal * (1 / newTotalParticipants)));

                        if (newParticipatingFriends > 0)
                            newDoubloonsPerFriend = (int)(Math.Ceiling(doubloonTotal * (1 / newTotalParticipants)));

                        if ((newDoubloonsPerCaptain != doubloonsPerCaptain) || (newDoubloonsPerCoowner != doubloonsPerCoowner) || (newDoubloonsPerFriend != doubloonsPerFriend))
                        {
                            sender.SendMessage("The number of ship participants have changed. Shares will be recalculated...");
                            sender.SendGump(new DivideThePlunderGump(m_Mobile, m_Boat, m_DivideMode));
                            
                            return;
                        }

                        if ((newParticipatingCoowners != participatingCoowners) || (newParticipatingFriends != participatingFriends))
                        {
                            sender.SendMessage("The number of ship participants have changed. Shares will be recalculated...");
                            sender.SendGump(new DivideThePlunderGump(m_Mobile, m_Boat, m_DivideMode));

                            return;
                        }

                        foreach (Mobile crewMember in m_Boat.ParticipatingMobiles)
                        {
                            if (crewMember == null) continue;
                            if (crewMember.Deleted) continue;
                            
                            if (crewMember == m_Boat.Owner)
                            {
                                if (newDoubloonsPerCaptain < 1)
                                    newDoubloonsPerCaptain = 1;

                                player = crewMember as PlayerMobile;

                                if (player != null)
                                {
                                    player.PirateScore += newDoubloonsPerCaptain;
                                    Banker.DepositUniqueCurrency(player, typeof(Doubloon), newDoubloonsPerCaptain);

                                    player.SendSound(doubloonDropSound);
                                    player.SendMessage("As the captain of the ship, you claim " + newDoubloonsPerCaptain.ToString() + " doubloons as your own. The doubloons have been placed in your bankbox.");
                                }
                            }

                            if (m_Boat.IsCoOwner(crewMember))
                            {
                                if (newDoubloonsPerCoowner < 1)
                                    newDoubloonsPerCoowner = 1;

                                player = crewMember as PlayerMobile;

                                if (player != null)
                                {
                                    player.PirateScore += newDoubloonsPerCoowner;
                                    Banker.DepositUniqueCurrency(player, typeof(Doubloon), newDoubloonsPerCoowner);

                                    player.SendSound(doubloonDropSound);
                                    player.SendMessage("As a co-owner of the ship, you claim " + newDoubloonsPerCoowner.ToString() + " doubloons as your own. The doubloons have been placed in your bankbox.");
                                }
                            }

                            if (m_Boat.IsFriend(crewMember))
                            {
                                if (newDoubloonsPerFriend < 1)
                                    newDoubloonsPerFriend = 1;

                                player = crewMember as PlayerMobile;

                                if (player != null)
                                {
                                    player.PirateScore += newDoubloonsPerFriend;
                                    Banker.DepositUniqueCurrency(player, typeof(Doubloon), newDoubloonsPerFriend);

                                    player.SendSound(doubloonDropSound);
                                    player.SendMessage("As a friend of the ship, you claim " + newDoubloonsPerFriend.ToString() + " doubloons as your own. The doubloons have been placed in your bankbox.");
                                }
                            }
                        }

                        m_Boat.DeleteDoubloonsInHold();
                    break;
                }
            }            

            if (info.ButtonID == 2)
            {
                reloadGump = false;
                sender.CloseAllGumps();
            }

            if (info.ButtonID == 3)
            {
                reloadGump = true;
                m_DivideMode = DivideMode.CaptainOnly;
            }

            if (info.ButtonID == 4)
            {
                reloadGump = true;
                m_DivideMode = DivideMode.OwnerCoOwnerEqual;
            }

            if (info.ButtonID == 5)
            {
                reloadGump = true;
                m_DivideMode = DivideMode.RankedShares;
            }

            if (info.ButtonID == 6)
            {
                reloadGump = true;
                m_DivideMode = DivideMode.Equal;
            }           

            if (reloadGump)
                sender.SendGump(new DivideThePlunderGump(m_Mobile, m_Boat, m_DivideMode));

            else
                sender.CloseGump(typeof(DivideThePlunderGump));
        }
    }
}



