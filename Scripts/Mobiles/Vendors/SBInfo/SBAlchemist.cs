using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBAlchemist : SBInfo
	{
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBAlchemist()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
				Add(new GenericBuyInfo(typeof(BlackPearl), 6, 100, 0xF7A, 0));
				Add(new GenericBuyInfo(typeof(Bloodmoss), 5, 100, 0xF7B, 0));
				Add(new GenericBuyInfo(typeof(MandrakeRoot), 3, 100, 0xF86, 0));
				Add(new GenericBuyInfo(typeof(Garlic), 3, 100, 0xF84, 0));
				Add(new GenericBuyInfo(typeof(Ginseng), 3, 100, 0xF85, 0));
				Add(new GenericBuyInfo(typeof(Nightshade), 3, 100, 0xF88, 0));
				Add(new GenericBuyInfo(typeof(SpidersSilk), 3, 100, 0xF8D, 0));
				Add(new GenericBuyInfo(typeof(SulfurousAsh), 3, 100, 0xF8C, 0)); 

				Add( new GenericBuyInfo( typeof( Bottle ), 5, 200, 0xF0E, 0 ) ); 

				Add( new GenericBuyInfo( "1041060", typeof( HairDye ), 2500, 10, 0xEFF, 0 ) );

				Add( new GenericBuyInfo( typeof( MortarPestle ), 8, 10, 0xE9B, 0 ) );

                Add( new GenericBuyInfo( typeof( LesserMagicResistPotion), 15, 10, 0xF06, 0)); 
				Add( new GenericBuyInfo( typeof( AgilityPotion ), 15, 10, 0xF08, 0 ) );
				Add( new GenericBuyInfo( typeof( StrengthPotion ), 15, 10, 0xF09, 0 ) );
				Add( new GenericBuyInfo( typeof( RefreshPotion ), 15, 10, 0xF0B, 0 ) );
				Add( new GenericBuyInfo( typeof( LesserCurePotion ), 15, 10, 0xF07, 0 ) );
				Add( new GenericBuyInfo( typeof( LesserHealPotion ), 15, 10, 0xF0C, 0 ) );
				Add( new GenericBuyInfo( typeof( LesserPoisonPotion ), 15, 10, 0xF0A, 0 ) );
				Add( new GenericBuyInfo( typeof( LesserExplosionPotion ), 41, 10, 0xF0D, 0 ) );
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( BlackPearl ), 3 ); 
				Add( typeof( Bloodmoss ), 3 ); 
				Add( typeof( MandrakeRoot ), 2 ); 
				Add( typeof( Garlic ), 2 ); 
				Add( typeof( Ginseng ), 2 ); 
				Add( typeof( Nightshade ), 2 ); 
				Add( typeof( SpidersSilk ), 2 ); 
				Add( typeof( SulfurousAsh ), 2 ); 
				Add( typeof( Bottle ), 3 );
				Add( typeof( MortarPestle ), 4 );
				Add( typeof( HairDye ), 19 );

                Add( typeof( LesserMagicResistPotion), 3);
				Add( typeof( AgilityPotion ), 3 );
				Add( typeof( StrengthPotion ), 3 );
				Add( typeof( RefreshPotion ), 3 );
				Add( typeof( LesserCurePotion ), 3 );
				Add( typeof( LesserHealPotion ), 3 );
				Add( typeof( LesserPoisonPotion ), 3 );
				Add( typeof( LesserExplosionPotion ), 10 );
			}
		}
	}
}
