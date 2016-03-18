using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using System.Collections.Generic;

namespace Server.Custom.Townsystem
{
	public class SheriffGump : Gump
	{
		private PlayerMobile m_From;
		private Faction m_Faction;
		private Town m_Town;

		private void CenterItem( int itemID, int x, int y, int w, int h )
		{
			Rectangle2D rc = ItemBounds.Table[itemID];
			AddItem( x + ((w - rc.Width) / 2) - rc.X, y + ((h - rc.Height) / 2) - rc.Y, itemID );
		}

		public SheriffGump( PlayerMobile from, Faction faction, Town town ) : base( 50, 50 )
		{
			m_From = from;
			m_Faction = faction;
			m_Town = town;


			AddPage( 0 );

			AddBackground( 0, 0, 320, 410, 5054 );
			AddBackground( 10, 10, 300, 390, 3000 );

			#region General
			AddPage( 1 );

			AddHtml( 20, 30, 260, 25, town.IsKing(from) ? "King" : "Guard Menu", false, false ); // Sheriff

			AddHtmlLocalized( 55, 90, 200, 25, 1011494, false, false ); // HIRE GUARDS
			AddButton( 20, 90, 4005, 4007, 0, GumpButtonType.Page, 3 );

			AddHtmlLocalized( 55, 120, 200, 25, 1011495, false, false ); // VIEW FINANCES
			AddButton( 20, 120, 4005, 4007, 0, GumpButtonType.Page, 2 );

			AddHtmlLocalized( 55, 360, 200, 25, 1011441, false, false ); // Exit
			AddButton( 20, 360, 4005, 4007, 0, GumpButtonType.Reply, 0 );
			#endregion

			#region Finances
			AddPage( 2 );

            //int financeUpkeep = 0;// town.FinanceUpkeep;
            int guardUpkeep = town.GuardUpkeep;
            //int dailyIncome = 0;// town.DailyIncome;
            //int netCashFlow = 0;// town.NetCashFlow;

			AddHtmlLocalized( 20, 30, 300, 25, 1011524, false, false ); // FINANCE STATEMENT
			
			AddHtmlLocalized( 20, 80, 300, 25, 1011538, false, false ); // Current total money for town : 
			AddLabel( 20, 100, 0x44, town.Treasury.ToString( "N0" ) ); // NOTE: Added 'N0'

            AddHtml(20, 130, 300, 25, "Guard Upkeep", false, false); // Guard Upkeep : 
            AddLabel(20, 150, 0x44, guardUpkeep.ToString("N0")); // NOTE: Added 'N0'

			/*AddHtmlLocalized( 20, 130, 300, 25, 1011520, false, false ); // Finance Minister Upkeep : 
			AddLabel( 20, 150, 0x44, financeUpkeep.ToString( "N0" ) ); // NOTE: Added 'N0'
	
			AddHtmlLocalized( 20, 180, 300, 25, 1011521, false, false ); // Sheriff Upkeep : 
			AddLabel( 20, 200, 0x44, sheriffUpkeep.ToString( "N0" ) ); // NOTE: Added 'N0'

			AddHtmlLocalized( 20, 230, 300, 25, 1011522, false, false ); // Town Income : 
			AddLabel( 20, 250, 0x44, dailyIncome.ToString( "N0" ) ); // NOTE: Added 'N0'

			AddHtmlLocalized( 20, 280, 300, 25, 1011523, false, false ); // Net Cash flow per day : 
			AddLabel( 20, 300, 0x44, netCashFlow.ToString( "N0" ) ); // NOTE: Added 'N0'*/

			AddHtmlLocalized( 55, 360, 200, 25, 1011067, false, false ); // Previous page
			AddButton( 20, 360, 4005, 4007, 0, GumpButtonType.Page, 1 );
			#endregion

			#region Hire Guards
			AddPage( 3 );

			AddHtmlLocalized( 20, 30, 300, 25, 1011494, false, false ); // HIRE GUARDS

			List<GuardList> guardLists = town.GuardLists;

			for ( int i = 0; i < guardLists.Count; ++i )
			{
				GuardList guardList = guardLists[i];
				int y = 90 + (i * 60);

				AddButton( 20, y, 4005, 4007, 0, GumpButtonType.Page, 4 + i );
				CenterItem( guardList.Definition.ItemID, 50, y - 20, 70, 60 );
				AddHtml( 120, y, 200, 25, guardList.Definition.Header, false, false );
			}

			AddHtmlLocalized( 55, 360, 200, 25, 1011067, false, false ); // Previous page
			AddButton( 20, 360, 4005, 4007, 0, GumpButtonType.Page, 1 );
			#endregion

			#region Guard Pages
			for ( int i = 0; i < guardLists.Count; ++i )
			{
				GuardList guardList = guardLists[i];

				AddPage( 4 + i );

				AddHtml( 90, 30, 300, 25, guardList.Definition.Header, false, false );
				CenterItem( guardList.Definition.ItemID, 10, 10, 80, 80 );

				AddHtmlLocalized( 20, 90, 200, 25, 1011514, false, false ); // You have : 
				AddLabel( 230, 90, 0x26, m_Town.NumberOfGuards.ToString() );

				AddHtmlLocalized( 20, 120, 200, 25, 1011515, false, false ); // Maximum : 
				AddLabel( 230, 120, 0x12A, m_Town.MaximumNumberOfGuards.ToString() );

				AddHtmlLocalized( 20, 150, 200, 25, 1011516, false, false ); // Cost : 
				AddLabel( 230, 150, 0x44, (guardList.Definition.Price * m_Town.Definition.CostFactor / 100).ToString( "N0" ) ); // NOTE: Added 'N0'

				AddHtmlLocalized( 20, 180, 200, 25, 1011517, false, false ); // Daily Pay :
				AddLabel( 230, 180, 0x37, guardList.Definition.Upkeep.ToString( "N0" ) ); // NOTE: Added 'N0'

				AddHtml( 20, 210, 200, 25, "Current Treasury", false, false ); // Current Silver : 
				AddLabel( 230, 210, 0x44, town.Treasury.ToString( "N0" ) ); // NOTE: Added 'N0'

				AddHtmlLocalized( 20, 240, 200, 25, 1011519, false, false ); // Current Payroll : 
				AddLabel( 230, 240, 0x44, guardUpkeep.ToString( "N0" ) ); // NOTE: Added 'N0'

				AddHtml( 55, 300, 200, 25, guardList.Definition.Label, false, false );
				AddButton( 20, 300, 4005, 4007, 1 + i, GumpButtonType.Reply, 0 );

				AddHtmlLocalized( 55, 360, 200, 25, 1011067, false, false ); // Previous page
				AddButton( 20, 360, 4005, 4007, 0, GumpButtonType.Page, 3 );
			}
			#endregion
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			if ( !(m_Town.IsKing( m_From ) || m_Town.IsCommander( m_From)) || m_Town.HomeFaction != m_Faction )
			{
				m_From.SendLocalizedMessage( 1010339 ); // You no longer control this city
				return;
			}

			int index = info.ButtonID - 1;

			if ( index >= 0 && index < m_Town.GuardLists.Count )
			{
				GuardList guardList = m_Town.GuardLists[index];
				Town town = Town.FromRegion( m_From.Region );
                int price = guardList.Definition.Price * m_Town.Definition.CostFactor / 100;

				if ( Town.FromRegion( m_From.Region ) != m_Town )
				{
					m_From.SendLocalizedMessage( 1010305 ); // You must be in your controlled city to buy Items
				}
				else if ( m_Town.NumberOfGuards >= m_Town.MaximumNumberOfGuards )
				{
					m_From.SendLocalizedMessage( 1010306 ); // You currently have too many of this enhancement type to place another
				}
                else if (!m_Town.AllowGuardType(guardList.Definition.Type))
                {
                    m_From.SendMessage("You need to increase the town's guard level to hire this type of guard.");
                }
                else if (m_Town.LastGuardHire + TimeSpan.FromMinutes(30) > DateTime.UtcNow)
                {
                    m_From.SendMessage("You cannot hire another guard so soon!");
                }
                else if (m_Town.Treasury >= price)
                {
                    BaseFactionGuard guard = guardList.Construct();

                    if (guard != null)
                    {
                        guard.Town = m_Town;

                        m_Town.Treasury -= price;
                        m_Town.WithdrawLog.AddLogEntry(m_From, price, TreasuryLogType.GuardPurchase);

                        guard.MoveToWorld(m_From.Location, m_From.Map);
                        guard.Home = guard.Location;
                        m_Town.LastGuardHire = DateTime.UtcNow;
                    }
                }
                else
                {
                    m_From.SendMessage("Your treasury lacks the finances to make this purchase.");
                }
			}
		}
	}
}