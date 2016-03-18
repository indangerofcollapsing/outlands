using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Server.Mobiles;
using Server.Items;

namespace Server.ArenaSystem
{
    public class ArenaVendor : BaseVendor
    {
		private List<SBInfo> m_SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

        [Constructable]
        public ArenaVendor()
            : base("an arena vendor")
        {
            AddItem( new PlateChest() );
			AddItem( new PlateArms() );
			AddItem( new PlateGorget() );
			AddItem( new PlateLegs() );

			switch( Utility.Random( 3 ) )
			{
				case 0: AddItem( new Doublet( Utility.RandomNondyedHue() ) ); break;
				case 1: AddItem( new Tunic( Utility.RandomNondyedHue() ) ); break;
				case 2: AddItem( new BodySash( Utility.RandomNondyedHue() ) ); break;
			}

			Halberd weapon = new Halberd();
            AddItem( weapon );
        }
        public ArenaVendor(Serial serial)
            : base(serial)
        {
            CantWalk = true;
            Frozen = true;
            Direction = Direction.East;
        }
        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBArena());
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
        public override void VendorSell(Mobile from)
        {
            Say(true, "I am not purchasing anything at this time.");
        }

        public class SBArena : SBInfo
        {
			private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
            private IShopSellInfo m_SellInfo = new InternalSellInfo();

            public SBArena()
            {
            }

            public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
			public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

			public class InternalBuyInfo : List<GenericBuyInfo>
            {
                public InternalBuyInfo()
                {
                    Add(new GenericBuyInfo("An arena team charter",typeof(ArenaTeamCharter), 1, 100, ArenaTeamCharter.s_gid, 0));
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

}
