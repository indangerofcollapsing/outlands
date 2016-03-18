using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBSwordWeapon: SBInfo
	{
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBSwordWeapon()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
				Add( new GenericBuyInfo( typeof( Broadsword ), 44, 20, 0xF5E, 0 ) );
				Add( new GenericBuyInfo( typeof( Cutlass ), 32, 20, 0x1441, 0 ) );
				Add( new GenericBuyInfo( typeof( Katana ), 42, 20, 0x13FF, 0 ) );
				Add( new GenericBuyInfo( typeof( Kryss ), 42, 20, 0x1401, 0 ) );
				Add( new GenericBuyInfo( typeof( Longsword ), 60, 20, 0xF61, 0 ) );
				Add( new GenericBuyInfo( typeof( Scimitar ), 43, 20, 0x13B6, 0 ) );
				Add( new GenericBuyInfo( typeof( ThinLongsword ), 60, 20, 0x13B8, 0 ) );
				Add( new GenericBuyInfo( typeof( VikingSword ), 66, 20, 0x13B9, 0 ) );
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( Broadsword ), 15 );
				Add( typeof( Cutlass ), 18 );
				Add( typeof( Katana ), 14 );
				Add( typeof( Kryss ), 15 );
				Add( typeof( Longsword ), 20 );
				Add( typeof( Scimitar ), 16 );
				Add( typeof( ThinLongsword ), 19 );
				Add( typeof( VikingSword ), 21 );
			}
		}
	}
}
