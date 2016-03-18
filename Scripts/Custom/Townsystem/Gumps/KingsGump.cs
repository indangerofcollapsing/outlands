using System;
using System.Collections.Generic;
using Server;
using Server.Custom.Townsystem;
using System.Collections;
using Server.Prompts;
using Server.Custom.Townsystem.Gumps;

namespace Server.Gumps
{
    public class KingsGump : Gump
    {
        private Town m_Town;

        public KingsGump(Town town)
            : base(50, 50)
        {
            m_Town = town;

            #region static content
            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;
            this.AddPage(0);
            this.AddImage(0, 4, 206);
            this.AddImageTiled(44, 45, 427, 338, 200);
            this.AddImage(44, 4, 201);
            this.AddImage(471, 4, 207);
            this.AddImage(0, 48, 202);
            this.AddImage(471, 48, 203);
            this.AddImage(0, 68, 202);
            this.AddImage(471, 68, 203);
            this.AddImage(0, 383, 204);
            this.AddImage(471, 383, 205);
            this.AddImage(44, 383, 233);
            this.AddButton(227, 383, 238, 240, (int)Buttons.btnApply, GumpButtonType.Reply, 0);
            #endregion

			if (town.AllowNewCitizenshipBuffs)
            {
                this.AddHtml(26, 20, 460, 20, String.Format("<center><BASEFONT COLOR=BLACK>{0} CHOOSES FOR ITS CITIZENS</CENTER>", town.Definition.TownName), (bool)false, (bool)false);

                int yMod = 0;
                int xMod = 0;
                this.AddGroup(0);
                for (int i = 1; i < TownBuff.NumberOfPrimaryBuffs+1; i++)
                {
                    CitizenshipBuffs pb = (CitizenshipBuffs)i;
                    string buffName = TownBuff.GetBuffName(pb);

                    if (buffName.Length == 0) 
                        continue;

                    yMod += 20;

                    if (yMod == 300)
                    {
                        xMod += 100;
                        yMod = 0;
                    }

                    this.AddRadio(60, 40 + yMod, 208, 209, false, 40+i);
                    this.AddLabel(80, 40 + yMod, 0x0, buffName);
                }
            }
            else
            {
                this.AddHtml(43, 22, 426, 14, String.Format("<BASEFONT COLOR=BLACK><center>{0} KING STONE</center>", town.Definition.TownName), (bool)false, (bool)false);

                this.AddLabel(35, 46, 0x0, @"Manage Town Alliances");
                this.AddButton(180, 44, 4005, 4007, (int)Buttons.btnAlliances, GumpButtonType.Reply, 0);

                this.AddLabel(305, 46, 0x0, @"Controlling Town:");
				this.AddLabel(410, 46, town.ControllingTown == town ? 0x5 : 0x0, town.ControllingTown == null ? "None" : town.ControllingTown.Definition.FriendlyName);

                this.AddImageTiled(35, 95, 171, 19, 214);
                this.AddLabel(35, 80, 0x0, @"Treasury:");
                this.AddLabel(120, 80, 0x44, ((int)town.Treasury).ToString());
                this.AddButton(210, 78, 4005, 4007, (int)Buttons.btnWithdraw, GumpButtonType.Reply, 0);

                this.AddImageTiled(35, 125, 171, 19, 214);
                this.AddLabel(35, 110, 0x0, @"Sales Tax:");
                this.AddTextEntry(152, 110, 42, 20, 0x5, (int)Buttons.SalesTaxEntry, (town.SalesTax * 100).ToString());
                this.AddLabel(196, 110, 0x0, @"%");

                this.AddImageTiled(35, 155, 171, 19, 214);
                this.AddLabel(35, 140, 0x0, string.Format("Secondary Buffs: {0}", m_Town.SecondaryCitizenshipBuffs.Count));
                this.AddButton(210, 140, 4005, 4007, (int)Buttons.SecondaryBuffs, GumpButtonType.Reply, 0);

                this.AddImageTiled(306, 95, 171, 19, 214);
                this.AddLabel(305, 80, 0x0, @"Active Citizens:");
                this.AddLabel(441, 80, 0x0, town.ActiveCitizens.ToString());

                this.AddImageTiled(306, 125, 171, 19, 214);
                this.AddLabel(305, 110, 0x0, @"Active Militia:");
                this.AddLabel(441, 110, 0x0, town.ActiveMilitia.ToString());

                this.AddImageTiled(306, 155, 171, 19, 214);
                this.AddLabel(305, 140, 0x0, @"Exiles:");
                this.AddLabel(441, 140, 0x0, town.Exiles.Count.ToString());

                this.AddImageTiled(306, 185, 171, 19, 214);
                this.AddLabel(305, 170, 0x0, @"Guards:");
                this.AddLabel(441, 170, 0x0, town.NumberOfGuards.ToString());


                this.AddGroup(1); //GUARD GROUP
                this.AddImageTiled(35, 218, 171, 19, 214);
                this.AddLabel(35, 203, 0x0, @"Passive Guard Protection:");
                this.AddRadio(35, 230, 208, 209, town.GuardState == GuardStates.None ? true : false, 20);
                this.AddLabel(57, 230, 0x0, @"None ( 0gp/day )");
                this.AddRadio(35, 255, 208, 209, town.GuardState == GuardStates.Lax ? true : false, 21);
                this.AddLabel(57, 255, 0x0, @"Lax ( 3gp/day/citizen )");
                this.AddRadio(35, 280, 208, 209, town.GuardState == GuardStates.Standard ? true : false, 22);
                this.AddLabel(57, 280, 0x0, @"Standard ( 7gp/day/citizen )");
                this.AddRadio(35, 305, 208, 209, town.GuardState == GuardStates.Strong ? true : false, 23);
                this.AddLabel(56, 305, 0x0, @"Strong ( 10gp/day/citizen )");
                this.AddRadio(35, 330, 208, 209, town.GuardState == GuardStates.Exceptional ? true : false, 24);
                this.AddLabel(56, 330, 0x0, @"Exceptional ( 17gp/day/citizen )");
               
            }
        }

        public enum Buttons
        {
            CANCEL,
            btnApply,
            SalesTaxEntry,
            PropertyTaxEntry,
            btnPurchaseGuardSpawner,
            btnWithdraw,
            btnAlliances,
            btnPurchaseBomb,
            SecondaryBuffs
        }

        public override void OnResponse(Network.NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (from.AccessLevel == AccessLevel.Player && !m_Town.IsKing(from))
            {
                from.SendMessage("Only the King is allowed to make changes to the King's Stone.");
                return;
            }

            if (info.ButtonID == (int)Buttons.SecondaryBuffs) 
            {
                if (m_Town.CanUpdateSecondaryBuffs)
                    from.SendGump(new SecondaryBuffGump(from, m_Town));
                else
                {
                    from.SendGump(new KingsGump(m_Town));
                    from.SendMessage("You cannot update secondary bonuses yet.");
                }
            }

            if (info.ButtonID == (int)Buttons.btnAlliances)
            {
                if (!from.HasGump(typeof(AllianceGump)))
                    from.SendGump(new AllianceGump(from, m_Town));

                return;
            }

            if (info.ButtonID == (int)Buttons.btnPurchaseBomb)
            {
            }

            if (info.ButtonID == (int)Buttons.btnApply)
            {
                //TAXES
                TextRelay salesTaxEntry = info.GetTextEntry((int)Buttons.SalesTaxEntry);
                string salesTaxValue = (salesTaxEntry == null ? null : salesTaxEntry.Text.Trim().ToLower());
                if (salesTaxValue != null)
                {
                        double salesTax = -1;
                        Double.TryParse(salesTaxValue, out salesTax );
                        if (salesTax == -1)
                        {
                            from.SendMessage("You have entered an invalid value for the sales tax.");
                            return;
                        }

                        salesTax /= 100;

                        if (m_Town == Town.Parse("Britain") && salesTax >= 0.50) {
                            from.SendMessage(@"The maximum tax rate is currently 50%.");
                            salesTax = 0.50;
                        }

                        if (salesTax != m_Town.SalesTax)
                            m_Town.SalesTaxChangeRequest(from, salesTax);
                }

                TextRelay propertyTaxEntry = info.GetTextEntry((int)Buttons.PropertyTaxEntry);
                string propertyTaxValue = (propertyTaxEntry == null ? null : propertyTaxEntry.Text.Trim().ToLower());
                if (propertyTaxValue != null)
                {
                    double propertyTax;
                    Double.TryParse(propertyTaxValue, out propertyTax);
                    if (propertyTax == -1)
                    {
                        from.SendMessage("You have entered an invalid value for the property tax.");
                        return;
                    }

                    propertyTax /= 100;

                    if (propertyTax != m_Town.PropertyTax)
                        m_Town.PropertyTaxChangeRequest(from, propertyTax);
                }

                //OTHER
                int chaosOrderID = -1;
                int guardID = -1;
                int wallID = -1;
                int primaryBuffID = -1;

                if (info.Switches.Length > 0)
                {
                    for (int i = 0; i < info.Switches.Length; i++)
                    {
                        if (info.Switches[i] > 10 && info.Switches[i] < 13)
                            chaosOrderID = info.Switches[i];
                        else if (info.Switches[i] > 19 && info.Switches[i] < 30)
                            guardID = info.Switches[i] - 20;
                        else if (info.Switches[i] > 29 && info.Switches[i] < 40)
                            wallID = info.Switches[i];
                        else if (info.Switches[i] > 40)
                            primaryBuffID = info.Switches[i] - 40;
                    }
                }

				if (m_Town.AllowNewCitizenshipBuffs)
                {
                    if (primaryBuffID == -1)
                        from.SendMessage(String.Format("You must select the citizenship benefits for {0}.", m_Town.Definition.FriendlyName));
                    else
                    {
                        m_Town.PrimaryCitizenshipBuff = (CitizenshipBuffs)primaryBuffID;
                        m_Town.AllowNewCitizenshipBuffs = false;
                    }

                    from.SendGump(new KingsGump(m_Town));

                    return;
                }

                GuardStates newState = (GuardStates)guardID;

                if (m_Town.GuardState != newState)
                {
                    if (m_Town.LastGuardStateChange + TimeSpan.FromDays(3) > DateTime.Now && from.AccessLevel == AccessLevel.Player) {
                        from.SendMessage("You cannot change the guard state again so soon.");
                    } else if (m_Town == Town.Parse("Britain") && newState < GuardStates.Lax) {
                        from.SendMessage("This town must maintain at least Lax guardstate.");
                    } else {
                        m_Town.GuardState = newState;
                        string s = "";
                        switch (newState) {
                            case GuardStates.None: s = "The King has dismissed the town guards!"; break;
                            case GuardStates.Lax: s = "The King has lowered payment of guards! Watch out for thieves!"; break;
                            case GuardStates.Standard: s = "The King has returned the guard protection to standard levels!"; break;
                            case GuardStates.Strong: s = "The King has has hired additional guards for the town!"; break;
                            case GuardStates.Exceptional: s = "The King has hired exceptional guards for the town!"; break;
                        }

                        if (s.Length > 0)
                            m_Town.AddTownCrierEntry(new string[] { s }, TimeSpan.FromHours(4));

                        m_Town.LastGuardStateChange = DateTime.Now;
                    }
                }

            }
            else if (info.ButtonID == (int)Buttons.btnWithdraw)
            {
                if (m_Town.IsKing(from))
                    from.SendGump(new KingWithdrawGump(from, m_Town));
            }
        }
    }
}
