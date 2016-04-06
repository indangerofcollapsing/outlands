using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBInnKeeper : SBInfo
	{
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBInnKeeper()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
				Add( new BeverageBuyInfo( typeof( BeverageBottle ), BeverageType.Ale, 7, 20, 0x99F, 0 ) );
				Add( new BeverageBuyInfo( typeof( BeverageBottle ), BeverageType.Wine, 7, 20, 0x9C7, 0 ) );
				Add( new BeverageBuyInfo( typeof( BeverageBottle ), BeverageType.Liquor, 7, 20, 0x99B, 0 ) );
				Add( new BeverageBuyInfo( typeof( Jug ), BeverageType.Cider, 13, 20, 0x9C8, 0 ) );
				Add( new BeverageBuyInfo( typeof( Pitcher ), BeverageType.Milk, 7, 20, 0x9F0, 0 ) );
				Add( new BeverageBuyInfo( typeof( Pitcher ), BeverageType.Ale, 11, 20, 0x1F95, 0 ) );
				Add( new BeverageBuyInfo( typeof( Pitcher ), BeverageType.Cider, 11, 20, 0x1F97, 0 ) );
				Add( new BeverageBuyInfo( typeof( Pitcher ), BeverageType.Liquor, 11, 20, 0x1F99, 0 ) );
				Add( new BeverageBuyInfo( typeof( Pitcher ), BeverageType.Wine, 11, 20, 0x1F9B, 0 ) );
				Add( new BeverageBuyInfo( typeof( Pitcher ), BeverageType.Water, 11, 20, 0x1F9D, 0 ) );			
				Add( new GenericBuyInfo( typeof( Torch ), 7, 20, 0xF6B, 0 ) );
				Add( new GenericBuyInfo( typeof( Candle ), 6, 20, 0xA28, 0 ) );
				Add( new GenericBuyInfo( typeof( Beeswax ), 1, 20, 0x1422, 0 ) );
				Add( new GenericBuyInfo( typeof( Backpack ), 15, 20, 0x9B2, 0 ) );
				Add( new GenericBuyInfo( "1016450", typeof( Chessboard ), 2, 20, 0xFA6, 0 ) );
				Add( new GenericBuyInfo( "1016449", typeof( CheckerBoard ), 2, 20, 0xFA6, 0 ) );
				Add( new GenericBuyInfo( typeof( Backgammon ), 2, 20, 0xE1C, 0 ) );
				Add( new GenericBuyInfo( typeof( Dices ), 2, 20, 0xFA7, 0 ) );
                Add(new GenericBuyInfo("1062332", typeof(VendorRentalContract), 2575, 20, 0x14F0, 0));
				Add( new GenericBuyInfo( "a barkeep contract", typeof( BarkeepContract ), 9250, 20, 0x14F0, 0 ) );
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( BeverageBottle ), 3 );
				Add( typeof( Jug ), 6 );
				Add( typeof( Pitcher ), 5 );
				Add( typeof( GlassMug ), 1 );				
				Add( typeof( Torch ), 3 );
				Add( typeof( Candle ), 3 );
				Add( typeof( Chessboard ), 1 );
				Add( typeof( CheckerBoard ), 1 );
				Add( typeof( Backgammon ), 1 );
				Add( typeof( Dices ), 1 );
				Add( typeof( ContractOfEmployment ), 512 );
				Add( typeof( Beeswax ), 1 );
			}
		}
	}
}