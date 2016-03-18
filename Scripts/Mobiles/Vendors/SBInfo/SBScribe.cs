using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBScribe: SBInfo
	{
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBScribe()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
				//Add( new GenericBuyInfo( typeof( BladeSpiritsScroll ), 350, 5, 0x1F4D, 0 ) );
				Add( new GenericBuyInfo( typeof( IncognitoScroll ), 450, 5, 0x1F4F, 0 ) );
				Add( new GenericBuyInfo( typeof( MagicReflectScroll ), 500, 5, 0x1F50, 0 ) );
				Add( new GenericBuyInfo( typeof( MindBlastScroll ), 350, 5, 0x1F51, 0 ) );
				Add( new GenericBuyInfo( typeof( ParalyzeScroll ), 550, 5, 0x1F52, 0 ) );
				Add( new GenericBuyInfo( typeof( PoisonFieldScroll ), 450, 5, 0x1F53, 0 ) );
				Add( new GenericBuyInfo( typeof( SummonCreatureScroll ), 450, 5, 0x1F54, 0 ) );
				Add( new GenericBuyInfo( typeof( DispelScroll ), 800, 4, 0x1F55, 0 ) );
				Add( new GenericBuyInfo( typeof( EnergyBoltScroll ), 900, 4, 0x1F56, 0 ) );
				Add( new GenericBuyInfo( typeof( ExplosionScroll ), 1050, 4, 0x1F57, 0 ) );
				Add( new GenericBuyInfo( typeof( InvisibilityScroll ), 900, 4, 0x1F58, 0 ) );
				Add( new GenericBuyInfo( typeof( MarkScroll ), 600, 4, 0x1F59, 0 ) );
				Add( new GenericBuyInfo( typeof( MassCurseScroll ), 700, 4, 0x1F5A, 0 ) );
				Add( new GenericBuyInfo( typeof( ParalyzeFieldScroll ), 700, 4, 0x1F5B, 0 ) );
				Add( new GenericBuyInfo( typeof( RevealScroll ), 800, 4, 0x1F5C, 0 ) );
				//Add( new GenericBuyInfo( typeof( ChainLightningScroll ), 1500, 3, 0x1F5D, 0 ) );
				//Add( new GenericBuyInfo( typeof( EnergyFieldScroll ),1500, 3, 0x1F5E, 0 ) );
				//Add( new GenericBuyInfo( typeof( FlamestrikeScroll ), 1750, 3, 0x1F5F, 0 ) );
				//Add( new GenericBuyInfo( typeof( GateTravelScroll ), 1950, 3, 0x1F60, 0 ) );
				Add( new GenericBuyInfo( typeof( ManaVampireScroll ), 1550, 3, 0x1F61, 0 ) );
				//Add( new GenericBuyInfo( typeof( MassDispelScroll ), 1500, 3, 0x1F62, 0 ) );
				//Add( new GenericBuyInfo( typeof( MeteorSwarmScroll ), 1050, 3, 0x1F63, 0 ) );
				//Add( new GenericBuyInfo( typeof( PolymorphScroll ), 1000, 3, 0x1F64, 0 ) );
				//Add( new GenericBuyInfo( typeof( EarthquakeScroll ), 5500, 2, 0x1F65, 0 ) );
				//Add( new GenericBuyInfo( typeof( EnergyVortexScroll ), 2500, 2, 0x1F66, 0 ) );
				Add( new GenericBuyInfo( typeof( ResurrectionScroll ), 6575, 2, 0x1F67, 0 ) );
				//Add( new GenericBuyInfo( typeof( SummonAirElementalScroll ), 2000, 2, 0x1F68, 0 ) );
				//Add( new GenericBuyInfo( typeof( SummonDaemonScroll ), 2500, 2, 0x1F69, 0 ) );
				//Add( new GenericBuyInfo( typeof( SummonEarthElementalScroll ), 2000, 2, 0x1F6A, 0 ) );
				//Add( new GenericBuyInfo( typeof( SummonFireElementalScroll ), 2000, 2, 0x1F6B, 0 ) );
				//Add( new GenericBuyInfo( typeof( SummonWaterElementalScroll ), 2000, 2, 0x1F6C, 0 ) );
				//Add( new GenericBuyInfo( typeof( DispelFieldScroll ), 500, 5, 0x1F4E, 0 ) );
				Add( new GenericBuyInfo( typeof( ScribesPen ), 8,  Utility.Random( 8, 15 ), 0xFBF, 0 ) );
				Add( new GenericBuyInfo( typeof( BrownBook ), 15, 10, 0xFEF, 0 ) );
				Add( new GenericBuyInfo( typeof( TanBook ), 15, 10, 0xFF0, 0 ) );
				Add( new GenericBuyInfo( typeof( BlueBook ), 15, 10, 0xFF2, 0 ) );
				Add( new GenericBuyInfo( typeof( BlankScroll ), 6, 999, 0x0E34, 0 ) );
				Add( new GenericBuyInfo( typeof( Spellbook ), 18, 10, 0xEFA, 0 ) );
				Add( new GenericBuyInfo( typeof( RecallRune ), 25, 10, 0x1F14, 0 ) );
				//Add( new GenericBuyInfo( "1041267", typeof( Runebook ), 3500, 10, 0xEFA, 0x461 ) );
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( ScribesPen ), 4 );
				Add( typeof( BrownBook ), 7 );
				Add( typeof( TanBook ), 7 );
				Add( typeof( BlueBook ), 7 );
				Add( typeof( BlankScroll ), 3 );
				Add( typeof( Spellbook ), 9 );
				Add( typeof( RecallRune ), 8 );
			}
		}
	}
}