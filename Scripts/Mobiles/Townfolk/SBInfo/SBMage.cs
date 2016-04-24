using System;
using System.Collections.Generic;
using Server.Items;
using Server.Engines.Craft;

namespace Server.Mobiles
{
	public class SBMage : SBInfo
	{
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBMage()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
				Type[] types = Loot.RegularScrollTypes;

				//for ( int i = 0; i < types.Length; ++i )
				for ( int i = 0; i < 32; ++i ) 

				{
					int itemID = 0x1F2E + i;

					if ( i == 6 )
						itemID = 0x1F2D;
					else if ( i > 6 )
						--itemID;

					Add( new GenericBuyInfo( types[i], 12 + ((i / 3) * 10), 20, itemID, 0 ) );
				}

                Add(new GenericBuyInfo(typeof(BlackPearl), 6, 100, 0xF7A, 0));
				Add(new GenericBuyInfo(typeof(Bloodmoss), 5, 100, 0xF7B, 0));
				Add(new GenericBuyInfo(typeof(MandrakeRoot), 3, 100, 0xF86, 0));
				Add(new GenericBuyInfo(typeof(Garlic), 3, 100, 0xF84, 0));
				Add(new GenericBuyInfo(typeof(Ginseng), 3, 100, 0xF85, 0));
				Add(new GenericBuyInfo(typeof(Nightshade), 3, 100, 0xF88, 0));
				Add(new GenericBuyInfo(typeof(SpidersSilk), 3, 100, 0xF8D, 0));
				Add(new GenericBuyInfo(typeof(SulfurousAsh), 3, 100, 0xF8C, 0)); 

				if ( Core.AOS )
				{
					Add( new GenericBuyInfo( typeof( BatWing ), 3, 999, 0xF78, 0 ) );
					Add( new GenericBuyInfo( typeof( GraveDust ), 3, 999, 0xF8F, 0 ) );
					Add( new GenericBuyInfo( typeof( DaemonBlood ), 6, 999, 0xF7D, 0 ) );
					Add( new GenericBuyInfo( typeof( NoxCrystal ), 6, 999, 0xF8E, 0 ) );
					Add( new GenericBuyInfo( typeof( PigIron ), 5, 999, 0xF8A, 0 ) );

					Add( new GenericBuyInfo( typeof( NecromancerSpellbook ), 115, 10, 0x2253, 0 ) );
				}

				Add( new GenericBuyInfo( "1041072", typeof( MagicWizardsHat ), 11, 10, 0x1718, 0 ) );

				//Add( new GenericBuyInfo( "1041267", typeof( Runebook ), 2500, 10, 0xEFA, 0x461 ) );

				Add( new GenericBuyInfo( typeof( RecallRune ), 15, 10, 0x1f14, 0 ) );
				Add( new GenericBuyInfo( typeof( Spellbook ), 18, 10, 0xEFA, 0 ) );

				Add( new GenericBuyInfo( typeof( ScribesPen ), 8, 10, 0xFBF, 0 ) );
				Add( new GenericBuyInfo( typeof( BlankScroll ), 6, 20, 0x0E34, 0 ) );
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
			//	Add( typeof( Runebook ), 1250 );
				Add( typeof( BlackPearl ), 3 ); 
				Add( typeof( Bloodmoss ), 3 ); 
				Add( typeof( MandrakeRoot ), 2 ); 
				Add( typeof( Garlic ), 2 ); 
				Add( typeof( Ginseng ), 2 ); 
				Add( typeof( Nightshade ), 2 ); 
				Add( typeof( SpidersSilk ), 2 ); 
				Add( typeof( SulfurousAsh ), 2 ); 
				Add( typeof( RecallRune ), 8 );
				Add( typeof( Spellbook ), 9 );
				Add( typeof( BlankScroll ), 3 );

				if ( Core.AOS )
				{
					Add( typeof( BatWing ), 2 );
					Add( typeof( GraveDust ), 2 );
					Add( typeof( DaemonBlood ), 3 );
					Add( typeof( NoxCrystal ), 3 );
					Add( typeof( PigIron ), 3 );
				}

				Type[] types = Loot.RegularScrollTypes;

                for (int i = 0; i < types.Length; ++i)
                {
                    Type type = types[i];

                    var ci = Server.Engines.Craft.DefInscription.CraftSystem.CraftItems.SearchFor(type);

                    if (ci == null)
                        continue;

                    var res = ci.Resources;

                    int cost = 0;

                    foreach (CraftRes c in ci.Resources)
                        cost += CostOfResource(c.ItemType);

                    int level = ((i + 1) / 8) / 2;

                    cost += LevelBonusFor(level);

                    cost *= 3;

                    int count = res.Count;

                    //old system + limiting factor of (cost of blankscroll + 3gp per reg + level / 2) (Sean)
                    Add(type, Math.Min(6 + ((i / 8) * 5), cost));
                }
			}

            public static int CostOfResource(Type type)
            {
                if (type == typeof(BlankScroll))
                    return 2;
                else if (type == typeof(MandrakeRoot))
                    return 2;
                else if (type == typeof(BlackPearl))
                    return 3;
                else if (type == typeof(Bloodmoss))
                    return 2;
                else
                    return 1;
            }

            public static int LevelBonusFor(int level)
            {
                if (level < 4)
                    return 0;
                else if (level < 7)
                    return 1;
                else
                    return 2;
            }
		}
	}
}