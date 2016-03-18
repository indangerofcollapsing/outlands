using System;
using System.Collections.Generic;
using Server.Items;
using Server.Multis;
using Server.Custom.Pirates;

namespace Server.Mobiles
{
	public class SBShipwright : SBInfo
	{
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBShipwright()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{				
				Add( typeof( SmallBoatDeed ), 1000 );
                Add( typeof( SmallDragonBoatDeed ), 1025);
                Add( typeof( MediumBoatDeed ), 1250);
                Add( typeof( MediumDragonBoatDeed ), 1300);
                Add( typeof( LargeBoatDeed ), 1500);
                Add( typeof( LargeDragonBoatDeed ), 1600);
                Add( typeof( CarrackBoatDeed ), 2000);
                Add( typeof( GalleonBoatDeed ), 3000);
			}
		}
	}
}