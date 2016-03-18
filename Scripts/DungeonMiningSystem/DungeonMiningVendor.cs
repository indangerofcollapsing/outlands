using System;
using System.Collections.Generic;
using System.Collections;
using System.Globalization;

using Server;
using Server.Items;
using Server.Mobiles;
using Server.Engines.Craft;

namespace Server.PortalSystem
{
	public class PortalVendor : BaseVendor
	{
        public static int s_controllerPrice = 20000;

		private List<SBInfo> m_SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

		[Constructable]
		public PortalVendor() : base( "a portal architect" )
        {
        }

		public override void InitSBInfo()
		{
			m_SBInfos.Add( new SBPortals() );
		}

        public PortalVendor(Serial serial)
            : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
        public override void VendorBuy(Mobile from)
        {
            // This vendor can only be accessed by accounts without a registered dungeon.
            // Authenticate the request for a controller.
            bool canPurchase = true;
            canPurchase &= PortalsSystem.VerifyEligibility(from);
            canPurchase &= PortalsSystem.VerifyAvailability();
            if (canPurchase)
            {
                base.VendorBuy(from);
            }
            else
            {
                Say(true, "You've already bought a dungeon from me!");
            }
        }
        public override void VendorSell(Mobile from)
        {
            Say(true, "I am not purchasing anything at this time.");
        }
    }
    public class PortalSupplierVendor : BaseVendor
    {
        public static int s_controllerPrice = 1;

		private List<SBInfo> m_SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

        [Constructable]
        public PortalSupplierVendor()
            : base("a portal tradesman")
        {
        }

        public override void InitSBInfo()
        {
        }

        public PortalSupplierVendor(Serial serial)
            : base(serial)
        {
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
        public override void VendorBuy(Mobile from)
        {
            Say(true, "Sure, I've got a few things for you!");
            from.SendGump(new PortalVendorInventoryBrowserGump(PortalsItemization.GetSystemInventory(), String.Empty, String.Empty, 0));
        }

        public override void VendorSell(Mobile from)
        {
            Say(true, "I am not purchasing anything at this time.");
        }
    }

    public class SBPortals : SBInfo
    {
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private IShopSellInfo m_SellInfo = new InternalSellInfo();

        public SBPortals(){}
        public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo()
            {
                Add(new GenericBuyInfo(typeof(DungeonControl), PortalVendor.s_controllerPrice, 1, 0x1C11, 0));
                Add(new GenericBuyInfo(typeof(GhostTrap), GhostTrap.m_cost, 400, GhostTrap.m_gidDeactivated, 0));  
            }
        }
        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
            }
        }
    }
    public class SBPortalSupplies : SBInfo
    {
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private IShopSellInfo m_SellInfo = new InternalSellInfo();

        public SBPortalSupplies(){ }
        public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo()
            {
                Add(new GenericBuyInfo(typeof(GhostTrap), GhostTrap.m_cost, 400, GhostTrap.m_gidDeactivated, 0));
               // Add(new GenericBuyInfo(typeof(InjectorInspector), 100, 300, InjectorInspector.s_gid, 0));
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