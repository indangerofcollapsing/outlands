using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using System.Collections.Generic;

namespace Server.Custom.Pirates.Battleground
{
	public class DefenseGump : Gump
	{
        public DefenseGump()
            : base(50, 50)
		{
			AddPage( 0 );

			AddBackground( 0, 0, 320, 410, 5054 );
			AddBackground( 10, 10, 300, 390, 3000 );

			#region General
			AddPage( 1 );

			AddHtml( 20, 30, 260, 25, "Bucc's Battleground Defenses", false, false );

			AddHtml( 55, 90, 200, 25,"HIRE CANNONEERS", false, false );
			AddButton( 20, 90, 4005, 4007, 0, GumpButtonType.Page, 3 );

			AddHtml( 55, 120, 200, 25, "PURCHASE DEPTH CHARGE", false, false );
			AddButton( 20, 120, 4005, 4007, 0, GumpButtonType.Page, 2 );

			AddHtmlLocalized( 55, 360, 200, 25, 1011441, false, false ); // Exit
			AddButton( 20, 360, 4005, 4007, 0, GumpButtonType.Reply, 0 );
			#endregion

			#region Depth Charge
			AddPage( 2 );

			AddHtml( 20, 30, 300, 25, "DEPTH CHARGES", false, false );
			
			AddHtml( 20, 80, 300, 25, "Existing Depth Charges :", false, false );
            AddLabel(20, 100, 0x44, BattlegroundDefense.DepthChargeCount.ToString("N0"));

            AddHtml(20, 130, 300, 25, "Maximum Depth Charges :", false, false);
            AddLabel(20, 150, 0x44, BattlegroundDefense.MaximumDepthCharges.ToString("N0"));

			AddHtml( 20, 180, 300, 25, "Depth Charge Doubloon Cost :", false, false );
			AddLabel( 20, 200, 0x44, BattlegroundDefense.DepthChargeCost.ToString("N0") );
	
			//AddHtml( 20, 230, 300, 25, "Available Doubloons :", false, false );
			//AddLabel( 20, 250, 0x44, BattlegroundDefense.GetAvailableDoubloons(from).ToString("N0") );

            AddHtml(55, 330, 200, 25, "PURCHASE DEPTH CHARGE", false, false);
            AddButton(20, 330, 4005, 4007, (int)Buttons.PurchaseDepthCharge, GumpButtonType.Reply, 0);

			AddHtmlLocalized( 55, 360, 200, 25, 1011067, false, false ); // Previous page
			AddButton( 20, 360, 4005, 4007, 0, GumpButtonType.Page, 1 );
			#endregion

			#region Hire Guards
            AddPage(3);

            AddHtml(20, 30, 300, 25, "CANNONEERS", false, false);

            AddHtml(20, 80, 300, 25, "Existing Cannoneers :", false, false);
            AddLabel(20, 100, 0x44, BattlegroundDefense.CannoneerCount.ToString("N0"));

            AddHtml(20, 130, 300, 25, "Maximum Cannoneers :", false, false);
            AddLabel(20, 150, 0x44, BattlegroundDefense.MaximumHiredCannoneers.ToString("N0"));

            AddHtml(20, 180, 300, 25, "Cannoneer Purchase Doubloon Cost :", false, false);
            AddLabel(20, 200, 0x44, BattlegroundDefense.CannoneerPurchaseCost.ToString("N0"));

            AddHtml(20, 230, 300, 25, "Cannoneer Upkeep :", false, false);
            AddLabel(20, 250, 0x44, (2*BattlegroundDefense.CannoneerCount).ToString("N0"));

            //AddHtml(20, 280, 300, 25, "Available Doubloons :", false, false);
            //AddLabel(20, 300, 0x44, BattlegroundDefense.GetAvailableDoubloons(from).ToString("N0"));

            AddHtml(55, 330, 200, 25, "PURCHASE CANNONEER", false, false);
            AddButton(20, 330, 4005, 4007, (int)Buttons.PurchaseCannoneer, GumpButtonType.Reply, 0);

            AddHtmlLocalized(55, 360, 200, 25, 1011067, false, false); // Previous page
            AddButton(20, 360, 4005, 4007, 0, GumpButtonType.Page, 1);
            #endregion
			
		}

        private enum Buttons
        {
            Cancel,
            PurchaseDepthCharge,
            PurchaseCannoneer
        }

		public override void OnResponse( NetState sender, RelayInfo info )
		{
            switch (info.ButtonID)
            {
                case ((int)Buttons.PurchaseCannoneer):
                    {
                        BattlegroundDefense.PurchaseCannoneerRequest(sender.Mobile);
                        break;
                    }
                case ((int)Buttons.PurchaseDepthCharge):
                    {
                        BattlegroundDefense.PurchaseDepthChargeRequest(sender.Mobile);
                        break;
                    }
            }
		}
	}
}