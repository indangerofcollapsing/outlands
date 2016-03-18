using System; 
using System.Collections.Generic; 
using Server.Items;
using Server.Custom;

namespace Server.Mobiles 
{ 
	public class SBFisherman : SBInfo 
	{ 
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo(); 
		private IShopSellInfo m_SellInfo = new InternalSellInfo(); 

		public SBFisherman() 
		{ 
		} 

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } } 
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } } 

		public class InternalBuyInfo : List<GenericBuyInfo> 
		{ 
			public InternalBuyInfo() 
			{ 
				Add( new GenericBuyInfo( typeof( RawFishSteak ), 4, 20, 0x97A, 0 ) );
				Add( new GenericBuyInfo( typeof( Fish ), 4, 80, 0x9CC, 0 ) );
				Add( new GenericBuyInfo( typeof( FishingPole ), 15, 20, 0xDC0, 0 ) );
                Add( new GenericBuyInfo( typeof( LobsterTrap), 6, 40, 0x44D0, 0));
			} 
		} 

		public class InternalSellInfo : GenericSellInfo 
		{ 
			public InternalSellInfo() 
			{ 
                //Add( typeof( RawFishSteak ), 1 );
                Add( typeof( FishCommissionCompletedDeed ), 500);
				Add( typeof( FishingPole ), 7 );
			} 
		} 
	} 
}