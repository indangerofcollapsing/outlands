using Server.Items;
using Server.Mobiles;
using Server.Multis.Deeds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Mobiles
{
    class Homebuilder : BaseVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos{ get { return m_SBInfos; } }

		public override NpcGuild NpcGuild{ get{ return NpcGuild.TinkersGuild; } }

		[Constructable]
		public Homebuilder() : base( "the artisan homebuilder" )
		{
		}

		public override void InitSBInfo()
		{
		    m_SBInfos.Add( new SBHousePlan() );
		}

        public Homebuilder(Serial serial)
            : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
    }

    public class SBHousePlan : SBInfo
    {
        private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private IShopSellInfo m_SellInfo = new InternalSellInfo();

        public SBHousePlan()
        {
        }

        public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
        public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo()
            {
                Add(new GenericBuyInfo("plans for a small stone temple house", typeof(UnfilledSmallStoneTempleHousePlans), 35889, 20, 0x14F0, 0));
                Add(new GenericBuyInfo("plans for the arbiter's estate", typeof(UnfilledArbitersEstateHousePlans), 618000, 20, 0x14F0, 0));
                Add(new GenericBuyInfo("plans for the magistrate house", typeof(UnfilledMagistratesHousePlans), 146250, 20, 0x14F0, 0));
                Add(new GenericBuyInfo("plans for a sandstone spa", typeof(UnfilledSandstoneSpaHousePlans), 146250, 20, 0x14F0, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                Add(typeof(UnfilledSmallStoneTempleHousePlans), 35889);
                Add(typeof(UnfilledArbitersEstateHousePlans), 618000);
                Add(typeof(UnfilledMagistratesHousePlans), 146250);
                Add(typeof(UnfilledSandstoneSpaHousePlans), 146250);
            }
        }
    }
}
