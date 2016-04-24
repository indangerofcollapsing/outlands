using System; 
using System.Collections.Generic; 
using Server.Items; 

namespace Server.Mobiles 
{ 
	public class SBButcher : SBInfo 
	{ 
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo(); 
		private IShopSellInfo m_SellInfo = new InternalSellInfo(); 

		public SBButcher() 
		{ 
		} 

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } } 
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } } 

		public class InternalBuyInfo : List<GenericBuyInfo> 
		{ 
			public InternalBuyInfo() 
			{
                Add( new GenericBuyInfo("Skillet", typeof(Skillet), 10, 50, 0x97F, 0));
				Add( new GenericBuyInfo( typeof( ButcherKnife ), 21, 20, 0x13F6, 0 ) );             
				Add( new GenericBuyInfo( typeof( Cleaver ), 24, 20, 0xEC3, 0 ) ); 
				Add( new GenericBuyInfo( typeof( SkinningKnife ), 26, 20, 0xEC4, 0 ) ); 
			} 
		} 

		public class InternalSellInfo : GenericSellInfo 
		{ 
			public InternalSellInfo() 
			{
                Add( typeof( Skillet ), 2);
				Add( typeof( ButcherKnife ), 13 ); 
				Add( typeof( Cleaver ), 12 ); 
				Add( typeof( SkinningKnife ), 10 ); 
			} 
		} 
	} 
}