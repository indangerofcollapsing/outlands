using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Server.Mobiles;
using Server.Items;

namespace Server.Items
{
    public class BritainTownSquare : Item
    {
        public override string DefaultName { get { return "Town Square"; } }
        private static readonly int MaximumRange = 20;
        private int m_ActiveRange;
		private Region m_Region;
        private static TimeSpan m_TimeBetweenNotifications = TimeSpan.FromMinutes(3);

        [CommandProperty(AccessLevel.GameMaster)]
        public int ActiveRange { get { return m_ActiveRange; } set { m_ActiveRange = Math.Min(value, MaximumRange); } }

        [Constructable]
        public BritainTownSquare()
            : base(0x23F)
        {
            m_ActiveRange = 20;
            Movable = Visible = false;
        }

        public static void BritainTownSquareModifier(Mobile from, Skill skill, ref double gc)
        {
            IPooledEnumerable eable = from.GetItemsInRange(MaximumRange);
            foreach (Item item in eable)
            {
                if (item is BritainTownSquare && from.InRange(item.Location, ((BritainTownSquare)item).ActiveRange))
                {
                    if (from is PlayerMobile)
                    {
                        var pm = from as PlayerMobile;
                        if (pm.LastTownSquareNotification + m_TimeBetweenNotifications < DateTime.Now)
                        {
                            from.SendMessage("You feel your skills are quickly improving!");
                            pm.LastTownSquareNotification = DateTime.Now;
                        }
                    }
                    gc *= 1.5;
                    eable.Free();
                    return;
                }
            }
            eable.Free();
        }

        public BritainTownSquare(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(2); // version
            writer.Write((int)0); // LEGACY (number of activated skills)
            writer.Write(m_ActiveRange);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
			
			// LEGACY (number of activated skills)
			int activated_count = reader.ReadInt(); 
			byte dummy = 0;
			for (int i = 0; i < activated_count; i++)
				dummy = reader.ReadByte();

			m_ActiveRange = reader.ReadInt();
			Point3D min = new Point3D(this.Location.X - m_ActiveRange, this.Location.Y - m_ActiveRange, -150);
			Point3D max = new Point3D(this.Location.X + m_ActiveRange, this.Location.Y + m_ActiveRange, 150);

			string BritainTownSquare_name = Serial.ToString(); //dummy name to get away from the "duplicate region" warnings
			m_Region = new BritainTownSquareRegion(new Rectangle3D(min, max), Region.Find(Location, Map.Felucca), BritainTownSquare_name);
			m_Region.Register();
        }
    }

	public class BritainTownSquareRegion : Region
	{
		public BritainTownSquareRegion(Rectangle3D area, Region parent, string name)
			: base(name, Map.Felucca, parent, area)
		{
		}
		public override bool AllowHousing(Mobile from, Point3D p)
		{
			return false;
		}
		public override void OnEnter(Mobile m)
		{
			base.OnEnter(m);
		}
		public override void OnExit(Mobile m)
		{
			base.OnExit(m);
		}
	}
}




namespace Server.ArenaSystem
{
    public class BritainTownSquareVendor : BaseVendor
    {
		private List<SBInfo> m_SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

        [Constructable]
        public BritainTownSquareVendor()
            : base("the master craftsman")
        {
			CantWalk = true;
			Frozen = true;
        }
		public BritainTownSquareVendor(Serial serial)
            : base(serial)
        {
            CantWalk = true;
            Frozen = true;
            Direction = Direction.East;
        }
        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBBritainTownSquare());
        }
		public override void InitOutfit()
		{
			base.InitOutfit();

			AddItem(new Server.Items.HalfApron(Utility.RandomOrangeHue()));
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
			//Say(true, "I am not purchasing anything at this time.");
			Say(1071140);
        }

        public class SBBritainTownSquare : SBInfo
        {
			private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
            private IShopSellInfo m_SellInfo = new InternalSellInfo();

			public SBBritainTownSquare()
            {
            }

            public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
			public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

			public class InternalBuyInfo : List<GenericBuyInfo>
            {
                public InternalBuyInfo()
                {
                    Add(new GenericBuyInfo(typeof(Pickaxe), 25, 10, 0xE86, 0));
                    Add(new GenericBuyInfo(typeof(Shovel), 12, 10, 0xF39, 0));
                    Add(new GenericBuyInfo(typeof(Hatchet), 25, 20, 0xF44, 0));
                    Add(new GenericBuyInfo(typeof(Froe), 12, 20, 0x10E5, 0));
                    Add(new GenericBuyInfo(typeof(Saw), 18, 20, 0x1034, 0));
                    Add(new GenericBuyInfo(typeof(TinkerTools), 30, 20, 0x1EB8, 0));
                    Add(new GenericBuyInfo(typeof(Hammer), 28, 20, 0x102A, 0));
                    Add(new GenericBuyInfo(typeof(SewingKit), 3, 20, 0xF9D, 0));
                    Add(new GenericBuyInfo(typeof(Tongs), 9, 20, 0xFBB, 0));
                    Add(new GenericBuyInfo(typeof(MortarPestle), 8, 10, 0xE9B, 0));
                    Add(new GenericBuyInfo(typeof(SewingKit), 3, 20, 0xF9D, 0));
                    Add(new GenericBuyInfo(typeof(FletcherTools), 20, 20, 0x1022, 0));
                    Add(new GenericBuyInfo(typeof(MapmakersPen), 8, 20, 0x0FBF, 0));
                    Add(new GenericBuyInfo(typeof(ScribesPen), 8, Utility.Random(8, 15), 0xFBF, 0));
                    Add(new GenericBuyInfo(typeof(BlankMap), 5, 40, 0x14EC, 0));
                    Add(new GenericBuyInfo(typeof(BlankScroll), 6, 40, 0xEF3, 0));
                    Add(new GenericBuyInfo(typeof(Bottle), 5, 200, 0xF0E, 0)); 
                    Add(new GenericBuyInfo("1044567", typeof(Skillet), 6, 20, 0x97F, 0));
                    Add(new GenericBuyInfo(typeof(Scissors), 13, 20, 0xF9F, 0));

                    Add(new GenericBuyInfo(typeof(TransformationDust), 50000, 1, 0x5745, 0)); // This will be like 16k from hometown mages
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
