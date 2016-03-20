
using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBAnimalTrainer : SBInfo
	{
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBAnimalTrainer()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
                Add(new AnimalBuyInfo(1, typeof(Eagle), 402, 10, 0x5, 0));
                Add(new AnimalBuyInfo(1, typeof(Cat), 138, 10, 0xC9, 0));
                Add(new AnimalBuyInfo(1, typeof(Horse), 602, 10, 0xC8, 0));
                Add(new AnimalBuyInfo(1, typeof(Rabbit), 78, 10, 0xCD, 0));
                Add(new AnimalBuyInfo(1, typeof(BrownBear), 855, 10, 0xA7, 0));
                Add(new AnimalBuyInfo(1, typeof(GrizzlyBear), 1767, 10, 0xD4, 0));
                Add(new AnimalBuyInfo(1, typeof(Panther), 1271, 10, 0xD6, 0));
                Add(new AnimalBuyInfo(1, typeof(Dog), 181, 10, 0xD9, 0));
                Add(new AnimalBuyInfo(1, typeof(TimberWolf), 768, 10, 0xE1, 0));
                Add(new AnimalBuyInfo(1, typeof(PackHorse), 606, 10, 0x123, 0));
                Add(new AnimalBuyInfo(1, typeof(PackLlama), 491, 10, 0x124, 0));
                Add(new AnimalBuyInfo(1, typeof(Rat), 107, 10, 0xEE, 0));
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
