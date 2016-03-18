using System;
using System.Collections.Generic;
using Server.Items;
using Server.Custom;

namespace Server.Mobiles
{
	public class SBPetBattle : SBInfo
	{
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

        public SBPetBattle()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
                Add(new GenericBuyInfo(typeof(PetBattleGrimoire), 2500, 50, 8787, 966));
                Add(new GenericBuyInfo(typeof(PetBattleDesertOstardToken), 1000, 50, 8501, 0));
                Add(new GenericBuyInfo(typeof(PetBattleGiantSpiderToken), 1000, 50, 8445, 0));
                Add(new GenericBuyInfo(typeof(PetBattleGorillaToken), 1000, 50, 8437, 0));
                Add(new GenericBuyInfo(typeof(PetBattlePantherToken), 1000, 50, 8450, 0));
                Add(new GenericBuyInfo(typeof(PetBattleGrizzlyBearToken), 10000, 50, 8478, 0));
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{				
			}
		}
	}
}