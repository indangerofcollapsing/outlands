using System; 
using System.Collections.Generic; 
using Server.Items; 

namespace Server.Mobiles 
{ 
	public class SBSpecialVendor : SBInfo 
	{
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo(); 
		private IShopSellInfo m_SellInfo = new InternalSellInfo(); 

		public SBSpecialVendor() 
		{ 
		} 

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } } 

		public class InternalBuyInfo : List<GenericBuyInfo> 
		{ 
			public InternalBuyInfo() 
			{ 
				//Add( new GenericBuyInfo( "clothing bless deed", typeof( ClothingBlessDeed ), 350000, 5, 0x14F0, 0 ) ); 
				//Add( new GenericBuyInfo( "holiday tree deed", typeof( HolidayTreeDeed ), 250000, 10, 0x14F0, 0 ) ); 
				//Add( new GenericBuyInfo( "hair restyle deed", typeof( HairRestylingDeed ), 100000, 5, 0x14F0, 0 ) );
				//Add( new GenericBuyInfo( "name change deed", typeof( NameChangeDeed ), 500000, 5, 0x14F0, 0 ) ); 
				//Add( new GenericBuyInfo( "polar bear rug east", typeof( PolarBearRugEastDeed ), 250000, 5, 0x14F0, 0 ) ); 
				//Add( new GenericBuyInfo( "polar bear rug south", typeof( PolarBearRugSouthDeed ), 250000, 5, 0x14F0, 0 ) );
				//Add( new GenericBuyInfo( "dark flower tapestry east", typeof( DarkFlowerTapestryEastDeed ), 150000, 5, 0x14F0, 0 ) );
				//Add( new GenericBuyInfo( "dark flower tapestry south", typeof( DarkFlowerTapestrySouthDeed ), 150000, 5, 0x14F0, 0 ) ); 
				//Add( new GenericBuyInfo( "light flower tapestry east", typeof( LightFlowerTapestryEastDeed ), 125000, 5, 0x14F0, 0 ) ); 
				//Add( new GenericBuyInfo( "light flower tapestry south", typeof( LightFlowerTapestrySouthDeed ), 125000, 5, 0x14F0, 0 ) );
				//Add( new GenericBuyInfo( "large stone table east", typeof( LargeStoneTableEastDeed ), 50000, 5, 0x14F0, 0 ) ); 
				//Add( new GenericBuyInfo( "large stone table south", typeof( LargeStoneTableEastDeed ), 50000, 5, 0x14F0, 0 ) );
			} 
		} 

		public class InternalSellInfo : GenericSellInfo 
		{ 
			public InternalSellInfo() 
			{ 
				//Add( typeof( HairDye ), 30 ); 
				//Add( typeof( SpecialBeardDye ), 1000000 ); 
				//Add( typeof( SpecialHairDye ), 1000000 ); 
			} 
		} 
	} 
}