using System;
using System.Collections.Generic;
using Server.Items;
using Server.Guilds;

namespace Server.Mobiles
{
	public class SBProvisioner : SBInfo
	{
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBProvisioner()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
                Add(new GenericBuyInfo(typeof(Arrow), 6, 20, 0xF3F, 0));
                Add(new GenericBuyInfo(typeof(Bolt), 6, 20, 0x1BFB, 0));
                Add(new GenericBuyInfo(typeof(Backpack), 15, 20, 0x9B2, 0));
                Add(new GenericBuyInfo(typeof(Pouch), 6, 20, 0xE79, 0));
                Add(new GenericBuyInfo(typeof(Bag), 6, 20, 0xE76, 0));
                Add(new GenericBuyInfo(typeof(WoodenBox), 14, 20, 0xE7D, 0));
                Add(new GenericBuyInfo(typeof(Lockpick), 12, 20, 0x14FC, 0));
                Add(new GenericBuyInfo("Skillet", typeof(Skillet), 10, 20, 0x97F, 0));
                Add(new GenericBuyInfo(typeof(Bedroll), 12, 20, 0xA57, 0));
                Add(new GenericBuyInfo(typeof(Kindling), 2, 20, 0xDE1, 0));
                Add(new GenericBuyInfo(typeof(Torch), 7, 20, 0xF6B, 0));
                Add(new GenericBuyInfo(typeof(Lantern), 2, 20, 0xA25, 0));
                Add(new GenericBuyInfo(typeof(Candle), 6, 20, 0xA28, 0));
                Add( new GenericBuyInfo( typeof(ParagonDevolveWand), 500, 50, 0x26BC, 2408));				
				Add( new GenericBuyInfo( "1060834", typeof( Engines.Plants.PlantBowl ), 2, 20, 0x15FD, 0 ) );
				Add( new BeverageBuyInfo( typeof( BeverageBottle ), BeverageType.Ale, 7, 20, 0x99F, 0 ) );
				Add( new BeverageBuyInfo( typeof( BeverageBottle ), BeverageType.Wine, 7, 20, 0x9C7, 0 ) );
				Add( new BeverageBuyInfo( typeof( BeverageBottle ), BeverageType.Liquor, 7, 20, 0x99B, 0 ) );
				Add( new BeverageBuyInfo( typeof( Jug ), BeverageType.Cider, 13, 20, 0x9C8, 0 ) );				
				Add( new GenericBuyInfo( typeof( Beeswax ), 1, 20, 0x1422, 0 ) );
				Add( new GenericBuyInfo( typeof( Bottle ), 5, 20, 0xF0E, 0 ) );
				Add( new GenericBuyInfo( typeof( RedBook ), 15, 20, 0xFF1, 0 ) );
				Add( new GenericBuyInfo( typeof( TanBook ), 15, 20, 0xFF0, 0 ) );
				Add( new GenericBuyInfo( "1016450", typeof( Chessboard ), 2, 20, 0xFA6, 0 ) );
				Add( new GenericBuyInfo( "1016449", typeof( CheckerBoard ), 2, 20, 0xFA6, 0 ) );
				Add( new GenericBuyInfo( typeof( Backgammon ), 2, 20, 0xE1C, 0 ) );
				Add( new GenericBuyInfo( typeof( Dices ), 2, 20, 0xFA7, 0 ) );
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( Backpack ), 7 );
				Add( typeof( Pouch ), 3 );
				Add( typeof( Bag ), 3 );
				Add( typeof( Candle ), 3 );
				Add( typeof( Torch ), 3 );
				Add( typeof( Lantern ), 1 );
				Add( typeof( Lockpick ), 3 );
				Add( typeof( Bottle ), 3 );
				Add( typeof( RedBook ), 7 );
				Add( typeof( BlueBook ), 7 );
				Add( typeof( TanBook ), 7 );
				Add( typeof( WoodenBox ), 7 );
				Add( typeof( Kindling ), 1 );
				Add( typeof( HairDye ), 30 );
				Add( typeof( Chessboard ), 1 );
				Add( typeof( CheckerBoard ), 1 );
				Add( typeof( Backgammon ), 1 );
				Add( typeof( Dices ), 1 );
				Add( typeof( Beeswax ), 1 );
			}
		}
	}
}
