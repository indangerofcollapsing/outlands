using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using Server.Network;
using Server.Gumps;
using Server.ContextMenus;

namespace Server.Items
{
    public class Aquarium : BaseAddonContainer
    {
        public override bool AlwaysAllowDoubleClick { get { return true; } }
        
        public static TimeSpan EventInterval = TimeSpan.FromMinutes(15);

        public static TimeSpan WaterInterval = TimeSpan.FromHours(24);
        public DateTime NextWaterNeeded = DateTime.UtcNow + WaterInterval;

        private Timer m_Timer;

        private int m_WaterLevel = 5;
        [CommandProperty(AccessLevel.GameMaster)]
        public int WaterLevel
        {
            get { return m_WaterLevel; }
            set { m_WaterLevel = value; }
        } 

        public static int MinWaterLevel = 0;
        public static int MaxWaterLevel = 5;         

        public static int MaxFish = 20;
        public static int MaxDecorations = 10;

        public List<AquariumItem> m_FishItems = new List<AquariumItem>();
        public List<AquariumItem> m_DecorationItems = new List<AquariumItem>();

        private bool m_HouseSecureInEffect = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool HouseSecureInEffect
        {
            get { return m_HouseSecureInEffect; }
            set { m_HouseSecureInEffect = value; }
        }
        
        public Aquarium(int itemID): base(itemID)
        {
            Name = "an aquarium";
            Movable = false;

            if (itemID == 0x3060)
                AddComponent(new AddonContainerComponent(0x3061), -1, 0, 0);

            if (itemID == 0x3062)
                AddComponent(new AddonContainerComponent(0x3063), 0, -1, 0);

            MaxItems = 0;

            m_Timer = new InternalTimer(this);
            m_Timer.Start();
        }

        public override BaseAddonContainerDeed Deed
        {
            get
            {
                if (ItemID == 0x3062)
                    return new AquariumEastDeed();
                else
                    return new AquariumNorthDeed();
            }
        }

        public override void OnSingleClick(Mobile from)
        {
            NetState ns = from.NetState;

            if (ns != null)
            {
                int waterLevelPercent = (int)((double)WaterLevel / (double)MaxWaterLevel * 100);

                ns.Send(new UnicodeMessage(Serial, ItemID, MessageType.Label, 0, 3, "ENU", "", Name));
                ns.Send(new UnicodeMessage(Serial, ItemID, MessageType.Label, 0, 3, "ENU", "", "water level: " + waterLevelPercent.ToString() + "%"));
            }  
        }

        public override void Open(Mobile from)
        {           
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            from.PlaySound(0x027);

            from.CloseGump(typeof(AquariumGump));
            from.SendGump(new AquariumGump(this, from, 0));
        }

        public virtual bool HasAccess(Mobile from)
        {
            if (from == null || from.Deleted)
                return false;

            else if (from.AccessLevel >= AccessLevel.GameMaster)
                return true;

            BaseHouse house = BaseHouse.FindHouseAt(this);

            return (house != null && house.IsCoOwner(from));
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            bool allowItem = false;

            if (!HasAccess(from))
            {
                from.SendLocalizedMessage(1073821); // You do not have access to that item for use with the aquarium.
                return false;
            }

            if (dropped is Pitcher)
            {
                Pitcher item = dropped as Pitcher;

                if (item.Content != BeverageType.Water)
                {
                    from.SendMessage("You cannot pour that into the aquarium.");
                    return false;
                }

                else
                {
                    if (WaterLevel < MaxWaterLevel)
                    {
                        int waterNeeded = Aquarium.MaxWaterLevel - WaterLevel;
                        int waterUsed = waterNeeded;

                        if (item.Quantity < waterUsed)
                            waterUsed = item.Quantity;

                        WaterLevel += waterUsed;
                        item.Quantity -= waterUsed;

                        from.PlaySound(0x4E);

                        from.CloseGump(typeof(AquariumGump));
                        from.SendMessage("You add water to the aquarium.");

                        return false;
                    }

                    else
                    {
                        from.SendMessage("That aquarium is already full of water.");
                        return false;
                    }
                }
            }

            if (dropped is BaseBeverage)
            {
                BaseBeverage item = dropped as BaseBeverage;

                if (item.IsEmpty || !item.Pourable || item.Content != BeverageType.Water)
                {
                    from.SendLocalizedMessage(500840); // Can't pour that in there.
                    return false;
                }

                else
                {
                    if (WaterLevel < MaxWaterLevel)
                    {
                        int waterNeeded = Aquarium.MaxWaterLevel - WaterLevel;
                        int waterUsed = waterNeeded;

                        if (item.Quantity < waterUsed)
                            waterUsed = item.Quantity;

                        WaterLevel += waterUsed;
                        item.Quantity -= waterUsed;

                        from.PlaySound(0x4E);

                        from.CloseGump(typeof(AquariumGump));
                        from.SendMessage("You add water to the aquarium.");

                        return false;
                    }

                    else
                    {
                        from.SendMessage("That aquarium is already full of water.");
                        return false;
                    }                    
                }
            }

            if (dropped is AquariumItem)
            {
                AquariumItem item = dropped as AquariumItem;                

                switch (item.ItemType)
                {
                    case AquariumItem.Type.Fish:
                        if (m_FishItems.Count >= MaxFish)
                        {
                            from.SendMessage("That aquarium cannot hold any more fish.");
                            return false;
                        }

                        else
                        {
                            from.CloseGump(typeof(AquariumGump));
                            from.SendMessage("You add the fish to the aquarium.");

                            m_FishItems.Add(item);
                            item.Internalize();

                            Splash();
                            Splash();

                            from.CloseGump(typeof(AquariumGump));
                            from.SendGump(new AquariumGump(this, from, 0));

                            return true;
                        }
                    break;

                    case AquariumItem.Type.Decoration:
                        if (m_DecorationItems.Count >= MaxDecorations)
                        {
                            from.SendMessage("That aquarium cannot hold any more decorative items.");
                            return false;
                        }

                        else
                        {
                            from.CloseGump(typeof(AquariumGump));
                            from.SendMessage("You add the decoration to the aquarium.");

                            m_DecorationItems.Add(item);
                            item.Internalize();

                            Splash();
                            Splash();

                            from.CloseGump(typeof(AquariumGump));
                            from.SendGump(new AquariumGump(this, from, 0));

                            return true;
                        }
                    break;
                }
            }

            return allowItem;
        }

        public void Splash()
        {
            Effects.PlaySound(Location, Map, 0x025);

            int splashes = Utility.RandomMinMax(1, 2);

            for (int a = 0; a < splashes; a++)
            {
                Blood water = new Blood();
                water.Hue = 2222;
                water.Name = "water";
                water.ItemID = Utility.RandomList(4650, 4651, 4652, 4653, 4654, 4655);

                Point3D splashLocation = new Point3D(Location.X + Utility.RandomMinMax(-1, 1), Location.Y + Utility.RandomMinMax(-1, 1), Z);

                water.MoveToWorld(splashLocation, Map);
            }
        }

        private class InternalTimer : Timer
        {
            private Aquarium m_Aquarium;

            public InternalTimer(Aquarium aquarium): base(EventInterval, EventInterval)
            {
                Priority = TimerPriority.OneMinute;

                m_Aquarium = aquarium;
            }

            protected override void OnTick()
            {
                if (m_Aquarium == null)
                {
                    Stop();
                    return;
                }

                if (m_Aquarium.Deleted)
                {
                    Stop();
                    return;
                }

                if (m_Aquarium.m_FishItems.Count > 0)
                {
                    AquariumItem item = m_Aquarium.m_FishItems[Utility.RandomMinMax(0, m_Aquarium.m_FishItems.Count - 1)];

                    if (Utility.RandomDouble() < .1)
                    {  
                        Blood creature = new Blood();
                        creature.ItemID = item.ItemID;
                        creature.Name = item.Name;
                        creature.MoveToWorld(m_Aquarium.Location, m_Aquarium.Map);
                        creature.Z += 10;

                        m_Aquarium.PublicOverheadMessage(MessageType.Emote, 0, false, item.Name + " comes to the surface and takes a look around");
                        m_Aquarium.Splash();
                    }

                    else if (Utility.RandomDouble() < .9)
                    {                        
                        m_Aquarium.PublicOverheadMessage(MessageType.Emote, 0, false, item.Name + " splashes around");

                        int splashes = Utility.RandomMinMax(2, 4);

                        for (int a = 0; a < splashes; a++)
                        {
                            m_Aquarium.Splash();
                        }
                    }

                    else
                    {
                        m_Aquarium.PublicOverheadMessage(MessageType.Emote, 0, false, item.Name + " thrashes about wildly");

                        int splashes = Utility.RandomMinMax(6, 8);

                        for (int a = 0; a < splashes; a++)
                        {
                            m_Aquarium.Splash();
                        }
                    }
                }                
            }
        }

        public override void OnDelete()
        {
            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (house != null)
            {
                if (m_HouseSecureInEffect)
                    house.MaxSecures--;
            }

            foreach (AquariumItem item in m_FishItems)
            {
                item.MoveToWorld(Location, Map);
            }

            foreach (AquariumItem item in m_DecorationItems)
            {
                item.MoveToWorld(Location, Map);
            }

            m_FishItems.Clear();
            m_DecorationItems.Clear();

            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }
        }

        public Aquarium(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1); // Version

            writer.Write(m_FishItems.Count);
            foreach (AquariumItem item in m_FishItems)
            {
                writer.Write(item);
            }

            writer.Write(m_DecorationItems.Count);
            foreach (AquariumItem item in m_DecorationItems)
            {
                writer.Write(item);
            }

            writer.Write(m_HouseSecureInEffect);

            //Version 1
            writer.Write(NextWaterNeeded);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_FishItems = new List<AquariumItem>();
            m_DecorationItems = new List<AquariumItem>();

            //Version 0
            if (version >= 0)
            {
                int fishItems = reader.ReadInt();
                for (int a = 0; a < fishItems; a++)
                {
                    AquariumItem item = reader.ReadItem() as AquariumItem;

                    if (item != null)
                        m_FishItems.Add(item);
                }

                int decorationItems = reader.ReadInt();
                for (int a = 0; a < decorationItems; a++)
                {
                    AquariumItem item = reader.ReadItem() as AquariumItem;

                    if (item != null)
                        m_DecorationItems.Add(item);
                }

                m_HouseSecureInEffect = reader.ReadBool();
            }

            //Version 1
            if (version >= 1)
            {
                NextWaterNeeded = reader.ReadDateTime();
            }

            //--------

            m_Timer = new InternalTimer(this);
            m_Timer.Start();
        }
    }
    
    public class AquariumItemPlaced : Item
    {
        [Constructable]
        public AquariumItemPlaced(): base(0x0)
        {
        }

        public AquariumItemPlaced(Serial serial): base(serial)
        {
        }        

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version >= 0)
            {
            }
        }    
    }

    public class AquariumEastDeed : BaseAddonContainerDeed
    {
        public override BaseAddonContainer Addon { get { return new Aquarium(0x3060); } }
        public override int LabelNumber { get { return 1074501; } } // Large Aquarium (east)

        [Constructable]
        public AquariumEastDeed(): base()
        {
        }

        public AquariumEastDeed(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class AquariumNorthDeed : BaseAddonContainerDeed
    {
        public override BaseAddonContainer Addon { get { return new Aquarium(0x3062); } }
        public override int LabelNumber { get { return 1074497; } } // Large Aquarium (north)

		[Constructable]
		public AquariumNorthDeed() : base()
		{
		}

		public AquariumNorthDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}
