using System; 
using System.Collections.Generic; 
using Server.Items; 

namespace Server.Mobiles 
{ 
	public class SBTinker: SBInfo 
	{ 
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo(); 
		private IShopSellInfo m_SellInfo = new InternalSellInfo(); 

		public SBTinker() 
		{ 
		} 

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } } 
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } } 

		public class InternalBuyInfo : List<GenericBuyInfo> 
		{ 
			public InternalBuyInfo() 
			{ 
				Add( new GenericBuyInfo( typeof( Drums ), 50, 20, 0x0E9C, 0 ) );
				Add( new GenericBuyInfo( typeof( Tambourine ), 60, 20, 0x0E9E, 0 ) );
				Add( new GenericBuyInfo( typeof( StandingHarp ), 30, 20, 0x0EB2, 0 ) );
				Add( new GenericBuyInfo( typeof( Lute ), 40, 20, 0x0EB3, 0 ) );

				Add( new GenericBuyInfo( typeof( Shovel ), 12, 20, 0xF39, 0 ) );
				Add( new GenericBuyInfo( typeof( SewingKit ), 3, 20, 0xF9D, 0 ) );
				Add( new GenericBuyInfo( typeof( Scissors ), 13, 20, 0xF9F, 0 ) );
				Add( new GenericBuyInfo( typeof( Tongs ), 9, 20, 0xFBB, 0 ) );
				Add( new GenericBuyInfo( typeof( Key ), 3, 20, 0x100E, 0 ) );

				Add( new GenericBuyInfo( typeof( DovetailSaw ), 14, 20, 0x1028, 0 ) );
				Add( new GenericBuyInfo( typeof( MouldingPlane ), 13, 20, 0x102C, 0 ) );
				Add( new GenericBuyInfo( typeof( Nails ), 3, 20, 0x102E, 0 ) );
				Add( new GenericBuyInfo( typeof( JointingPlane ), 13, 20, 0x1030, 0 ) );
				Add( new GenericBuyInfo( typeof( SmoothingPlane ), 12, 20, 0x1032, 0 ) );
				Add( new GenericBuyInfo( typeof( Saw ), 18, 20, 0x1034, 0 ) );

				Add( new GenericBuyInfo( typeof( Clock ), 22, 20, 0x104B, 0 ) );
				Add( new GenericBuyInfo( typeof( ClockParts ), 3, 20, 0x104F, 0 ) );
				Add( new GenericBuyInfo( typeof( AxleGears ), 3, 20, 0x1051, 0 ) );
				Add( new GenericBuyInfo( typeof( Gears ), 2, 20, 0x1053, 0 ) );
				Add( new GenericBuyInfo( typeof( Hinge ), 2, 20, 0x1055, 0 ) );
				Add( new GenericBuyInfo( typeof( Sextant ), 25, 20, 0x1057, 0 ) );
				Add( new GenericBuyInfo( typeof( SextantParts ), 5, 20, 0x1059, 0 ) );
				Add( new GenericBuyInfo( typeof( Axle ), 2, 20, 0x105B, 0 ) );
				Add( new GenericBuyInfo( typeof( Springs ), 3, 20, 0x105D, 0 ) );

				Add( new GenericBuyInfo( typeof( DrawKnife ), 12, 20, 0x10E4, 0 ) );
				Add( new GenericBuyInfo( typeof( Froe ), 12, 20, 0x10E5, 0 ) );
				Add( new GenericBuyInfo( typeof( Inshave ), 12, 20, 0x10E6, 0 ) );
				Add( new GenericBuyInfo( typeof( Scorp ), 12, 20, 0x10E7, 0 ) );

				Add( new GenericBuyInfo( typeof( Lockpick ), 12, 20, 0x14FC, 0 ) );
				Add( new GenericBuyInfo( typeof( TinkerTools ), 30, 20, 0x1EB8, 0 ) );

				Add( new GenericBuyInfo( typeof( Pickaxe ), 32, 20, 0xE86, 0 ) );
				// TODO: Sledgehammer
				Add( new GenericBuyInfo( typeof( Hammer ), 28, 20, 0x102A, 0 ) );
				Add( new GenericBuyInfo( typeof( SmithHammer ), 4, 20, 0x13E3, 0 ) );
				Add( new GenericBuyInfo( typeof( ButcherKnife ), 21, 20, 0x13F6, 0 ) );
			} 
		} 

		public class InternalSellInfo : GenericSellInfo 
		{ 
			public InternalSellInfo() 
			{ 
				Add( typeof( Drums ), 17 );
				Add( typeof( Tambourine ), 20 );
				Add( typeof( StandingHarp ), 10 );
				Add( typeof( Lute ), 14 );

				Add( typeof( Shovel ), 4 );
				Add( typeof( SewingKit ), 1 );
				Add( typeof( Scissors ), 4 );
				Add( typeof( Tongs ), 1 );
				Add( typeof( Key ), 1 );

				Add( typeof( DovetailSaw ), 4 );
				Add( typeof( MouldingPlane ), 4 );
				Add( typeof( Nails ), 1 );
				Add( typeof( JointingPlane ), 4 );
				Add( typeof( SmoothingPlane ), 4 );
				Add( typeof( Saw ), 6 );

				Add( typeof( Clock ), 7 );
				Add( typeof( ClockParts ), 1 );
				Add( typeof( AxleGears ), 1 );
				Add( typeof( Gears ), 1 );
				Add( typeof( Hinge ), 1 );
				Add( typeof( Sextant ), 6 );
				Add( typeof( SextantParts ), 1 );
				Add( typeof( Axle ), 1 );
				Add( typeof( Springs ), 1 );

				Add( typeof( DrawKnife ), 4 );
				Add( typeof( Froe ), 4 );
				Add( typeof( Inshave ), 4 );
				Add( typeof( Scorp ), 4 );

				Add( typeof( Lockpick ), 3 );
				Add( typeof( TinkerTools ), 6 );

				Add( typeof( Pickaxe ), 9 );
				Add( typeof( Hammer ), 3 );
				Add( typeof( SmithHammer ), 1 );
				Add( typeof( ButcherKnife ), 7 );
			} 
		} 
	} 
}