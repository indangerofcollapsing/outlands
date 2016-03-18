using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBAxeWeapon: SBInfo
	{
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBAxeWeapon()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
				Add( new GenericBuyInfo( typeof( BattleAxe ), 38, 20, 0xF47, 0 ) );
				Add( new GenericBuyInfo( typeof( DoubleAxe ), 32, 20, 0xF4B, 0 ) );
				Add( new GenericBuyInfo( typeof( ExecutionersAxe ), 38, 20, 0xF45, 0 ) );
				Add( new GenericBuyInfo( typeof( LargeBattleAxe ), 43, 20, 0x13FB, 0 ) );
				Add( new GenericBuyInfo( typeof( Pickaxe ), 32, 20, 0xE86, 0 ) );
				Add( new GenericBuyInfo( typeof( TwoHandedAxe ), 42, 20, 0x1443, 0 ) );
				Add( new GenericBuyInfo( typeof( WarAxe ), 38, 20, 0x13B0, 0 ) );
				Add( new GenericBuyInfo( typeof( Axe ), 48, 20, 0xF49, 0 ) );
				Add( new GenericBuyInfo( typeof( Hatchet ), 21, 20, 0xF43, 0 ) );
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( BattleAxe ), 25 );
				Add( typeof( DoubleAxe ), 11 );
				Add( typeof( ExecutionersAxe ), 24 );
				Add( typeof( LargeBattleAxe ), 16 );
				Add( typeof( Pickaxe ), 11 );
				Add( typeof( TwoHandedAxe ), 15 );
				Add( typeof( WarAxe ), 12 );
				Add( typeof( Axe ), 16 );
				Add( typeof( Hatchet), 14 );
			}
		}
	}
}
