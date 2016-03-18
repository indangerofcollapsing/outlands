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
				Add( new GenericBuyInfo( typeof( RawRibs ), 6, 20, 0x9F1, 0 ) ); 
				Add( new GenericBuyInfo( typeof( RawLambLeg ), 6, 20, 0x1609, 0 ) ); 
				Add( new GenericBuyInfo( typeof( RawChickenLeg ), 6, 20, 0x1607, 0 ) ); 
				Add( new GenericBuyInfo( typeof( RawBird ), 6, 20, 0x9B9, 0 ) ); 
				Add( new GenericBuyInfo( typeof( Bacon ), 3, 20, 0x979, 0 ) ); 
				Add( new GenericBuyInfo( typeof( Sausage ), 17, 20, 0x9C0, 0 ) ); 
				Add( new GenericBuyInfo( typeof( Ham ), 22, 20, 0x9C9, 0 ) ); 
				Add( new GenericBuyInfo( typeof( ButcherKnife ), 21, 20, 0x13F6, 0 ) );             
				Add( new GenericBuyInfo( typeof( Cleaver ), 24, 20, 0xEC3, 0 ) ); 
				Add( new GenericBuyInfo( typeof( SkinningKnife ), 26, 20, 0xEC4, 0 ) ); 
			} 
		} 

		public class InternalSellInfo : GenericSellInfo 
		{ 
			public InternalSellInfo() 
			{ 
				Add( typeof( RawRibs ), 3 ); 
				Add( typeof( RawLambLeg ), 3 ); 
				Add( typeof( RawChickenLeg ), 3 ); 
				Add( typeof( RawBird ), 3 );
				Add( typeof( CookedBird ), 6 ); 
				Add( typeof( Ribs ), 5 ); 
				Add( typeof( Bacon ), 1 ); 
				Add( typeof( Sausage ), 8 ); 
				Add( typeof( Ham ), 10 ); 
				Add( typeof( ButcherKnife ), 13 ); 
				Add( typeof( Cleaver ), 12 ); 
				Add( typeof( SkinningKnife ), 10 ); 
			} 
		} 
	} 
}