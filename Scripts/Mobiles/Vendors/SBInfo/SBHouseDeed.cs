using System;
using System.Collections.Generic;
using Server.Multis.Deeds;

namespace Server.Mobiles
{
	public class SBHouseDeed: SBInfo
	{
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBHouseDeed()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
				Add( new GenericBuyInfo( "deed to a stone-and-plaster house", typeof( StonePlasterHouseDeed ), 72500, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a field stone house", typeof( FieldStoneHouseDeed ), 72500, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a small brick house", typeof( SmallBrickHouseDeed), 72500, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a wooden house", typeof( WoodHouseDeed ), 72500, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a wood-and-plaster house", typeof( WoodPlasterHouseDeed ), 72500, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a thatched-roof cottage", typeof( ThatchedRoofCottageDeed ), 72500, 20, 0x14F0, 0 ) );
				
				Add( new GenericBuyInfo( "deed to a small stone workshop", typeof( StoneWorkshopDeed ), 150000, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a small marble workshop", typeof( MarbleWorkshopDeed ), 150000, 20, 0x14F0, 0 ) );

				Add( new GenericBuyInfo( "deed to a small stone tower", typeof( SmallTowerDeed ), 175500, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a sandstone house with patio", typeof( SandstonePatioDeed ), 195900, 20, 0x14F0, 0 ) );
				
				Add( new GenericBuyInfo( "deed to a two story villa", typeof( VillaDeed ), 276500, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a two story log cabin", typeof( LogCabinDeed ), 295500, 20, 0x14F0, 0 ) );

				Add( new GenericBuyInfo( "deed to a large house with patio", typeof( LargePatioDeed ), 425000, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a brick house", typeof( BrickHouseDeed ), 495000, 20, 0x14F0, 0 ) );

				Add( new GenericBuyInfo( "deed to a two-story wood-and-plaster house", typeof( TwoStoryWoodPlasterHouseDeed ), 650000, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a marble house with patio", typeof( LargeMarbleDeed ), 780000, 20, 0x14F0, 0 ) );

				Add( new GenericBuyInfo( "deed to a tower", typeof( TowerDeed ), 1250000, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a small stone keep", typeof( KeepDeed ), 2550000, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a castle", typeof( CastleDeed ), 7420000, 20, 0x14F0, 0 ) );
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( StonePlasterHouseDeed ), 72500 );
				Add( typeof( FieldStoneHouseDeed ), 72500 );
				Add( typeof( SmallBrickHouseDeed ), 72500 );
				Add( typeof( WoodHouseDeed ), 72500 );
				Add( typeof( WoodPlasterHouseDeed ), 72500 );
				Add( typeof( ThatchedRoofCottageDeed ), 72500 );
				Add( typeof( BrickHouseDeed ), 495000 );
				Add( typeof( TwoStoryWoodPlasterHouseDeed ), 650000 );
				Add( typeof( TowerDeed ), 1250000 );
				Add( typeof( KeepDeed ), 2550000 );
				Add( typeof( CastleDeed ), 7420000 );
				Add( typeof( LargePatioDeed ), 425000 );
				Add( typeof( LargeMarbleDeed ), 780000 );
				Add( typeof( SmallTowerDeed ), 175500 );
				Add( typeof( LogCabinDeed ), 295500 );
				Add( typeof( SandstonePatioDeed ), 195900 );
				Add( typeof( VillaDeed ), 276500 );
				Add( typeof( StoneWorkshopDeed ), 150000 );
				Add( typeof( MarbleWorkshopDeed ), 150000 );
			}
		}
	}
}
