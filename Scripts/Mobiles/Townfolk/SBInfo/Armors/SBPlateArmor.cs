using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBPlateArmor: SBInfo
	{
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBPlateArmor()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
				Add( new GenericBuyInfo( typeof( PlateArms ), 181, 20, 0x1410, 0 ) );
				Add( new GenericBuyInfo( typeof( PlateChest ), 273, 20, 0x1415, 0 ) );
				Add( new GenericBuyInfo( typeof( PlateGloves ), 145, 20, 0x1414, 0 ) );
				Add( new GenericBuyInfo( typeof( PlateGorget ), 124, 20, 0x1413, 0 ) );
				Add( new GenericBuyInfo( typeof( PlateLegs ), 218, 20, 0x1411, 0 ) );

				Add( new GenericBuyInfo( typeof( PlateHelm ), 170, 20, 0x1412, 0 ) );
				Add( new GenericBuyInfo( typeof( FemalePlateChest ), 245, 20, 0x1C04, 0 ) );
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{	
			public InternalSellInfo()
			{
				Add( typeof( PlateArms ), 54 );
				Add( typeof( PlateChest ), 75 );
				Add( typeof( PlateGloves ), 36 );
				Add( typeof( PlateGorget ), 30 );
				Add( typeof( PlateLegs ), 60 );

				Add( typeof( PlateHelm ), 45 );
				Add( typeof( FemalePlateChest ), 60 );
			}
		}
	}
}
