using System; 
using System.Collections.Generic; 
using Server.Items; 

namespace Server.Mobiles 
{ 
	public class SBMiller : SBInfo 
	{ 
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo(); 
		private IShopSellInfo m_SellInfo = new InternalSellInfo(); 

		public SBMiller() 
		{ 
		} 

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } } 
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } } 

		public class InternalBuyInfo : List<GenericBuyInfo> 
		{ 
			public InternalBuyInfo() 
			{
                Add(new GenericBuyInfo("Skillet", typeof(Skillet), 10, 20, 0x97F, 0));
                Add(new GenericBuyInfo(typeof(RollingPin), 2, 20, 0x1043, 0));
				Add(new GenericBuyInfo(typeof(SackOfFlour), 3, 20, 0x1039, 0 ) );
				Add(new GenericBuyInfo(typeof(SheafOfHay), 2, 20, 0xF36, 0 ) );
			} 
		} 

		public class InternalSellInfo : GenericSellInfo 
		{ 
			public InternalSellInfo() 
			{
                Add(typeof( Skillet), 2);
                Add(typeof( RollingPin), 1);
                Add(typeof( SackOfFlour ), 1);
				Add(typeof( SheafOfHay ), 1 );
			} 
		} 
	} 
}