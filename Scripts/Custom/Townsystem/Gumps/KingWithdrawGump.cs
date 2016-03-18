using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Commands;
using Server.Custom.Townsystem;

namespace Server.Gumps
{
    public class KingWithdrawGump : Gump
    {
        private Town m_Town;

        public KingWithdrawGump(Mobile from, Town town)
            : base(50, 50)
        {
            m_Town = town;

            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            AddPage(0);
            AddImage(0, 48, 202);
            AddImage(0, 4, 206);
            AddImage(44, 4, 201);
            AddImage(471, 4, 207);
            AddImage(471, 47, 203);
            AddImage(0, 363, 204);
            AddImage(471, 363, 205);
            AddImage(44, 363, 233);
            AddImageTiled(44, 44, 427, 323, 200);
            AddHtml(43, 22, 426, 20, String.Format("<BASEFONT COLOR=BLACK><center>{0} KING STONE</center>", town.Definition.TownName), (bool)false, (bool)false);
            AddHtml(26, 59, 457, 90, @"Kings are allowed to withdraw 10% of the treasury per week (200k max). This may be withdrawn in the form of gold coin, dispursements to all active militia members, or dispursements to all active citizens.  The withdraw will be given out 36 hours after the request is made. Withdraw requests will be announced by all town criers immediately following the request.  Citizen and Militia dispursements must be denoted in terms of the total dispursement. Individual dispursements are then calculated by UOAC and given out. All withdraws have a 15% fee imposed on the withdraw from the bankers.", (bool)true, (bool)true);
            AddHtml(32, 227, 250, 25, @"<center>Gold Coin Withdraw:</center>", (bool)true, (bool)false);
            AddHtml(30, 272, 250, 25, @"<center>Citizen Dispursement:</center>", (bool)true, (bool)false);
            AddHtml(31, 317, 250, 25, @"<center>Militia Dispursement:</center>", (bool)true, (bool)false);
            AddHtml(29, 174, 451, 22, String.Format("<center>Available to withdraw: {0} gold</center>", m_Town.WithdrawManager.AvailableToWithdraw), (bool)false, (bool)false);
            AddImage(381, 272, 2445);
            AddTextEntry(387, 273, 95, 20, 0, 0, @"");
            AddGroup(0);
            AddRadio(286, 229, 208, 209, true, 1);
            AddRadio(286, 274, 208, 209, false, 2);
            AddRadio(286, 319, 208, 209, false, 3);
            AddLabel(328, 273, 0, @"Amount:");
            AddButton(399, 298, 247, 248, 1, GumpButtonType.Reply, 0);
            AddButton(236, 365, 242, 241, 0, GumpButtonType.Reply, 0); //cancel
        }



        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (!m_Town.IsKing(from))
                return;

            switch (info.ButtonID)
            {
                case 1:
                    {
                        TextRelay entry0 = info.GetTextEntry(0);
                        string text0 = (entry0 == null ? "" : entry0.Text.Trim());
                        int amount;
                        Int32.TryParse(text0, out amount);

                        if (/*amount == null ||*/ amount < 1)
                            return;

                        Withdraws.WithdrawType type = Withdraws.WithdrawType.Personal;

                        if (info.Switches.Length > 0)
                        {
                            for (int i = 0; i < info.Switches.Length; i++)
                            {
                                if (info.Switches[i] == 2)
                                    type = Withdraws.WithdrawType.CitizenDispursement;
                                else if (info.Switches[i] == 3)
                                    type = Withdraws.WithdrawType.MilitiaDispursement;
                            }
                        }

                        m_Town.TreasuryWithdrawRequest(from, amount, type);

                        from.SendGump(new KingsGump(m_Town));
                        break;
                    }

            }
        }
    }
}