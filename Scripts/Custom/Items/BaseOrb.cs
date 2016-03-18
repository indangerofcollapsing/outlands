using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Targeting;
using Server.ContextMenus;
using Server.Network;
using Server.Regions;
using System.Text;
using Server.Gumps;
using System.IO;
using Server.Engines.Craft;

namespace Server.Items
{
    public class BaseOrb : MapItem
    {
        public static readonly int MapDistance = 200;
        public static readonly int RandomRadius = 50;
        public static readonly int MapWidth = 300;
        public static readonly int MapHeight = 300;

        public Map targetMap;

        public bool m_Activated = false;

        public static Dictionary<Mobile, BaseOrb> m_Instances = new Dictionary<Mobile, BaseOrb>();
        
        [Constructable]
        public BaseOrb()
        {
            Name = "Orb of Sight";

            ItemID = 0xE2D;
            Weight = 1.0;
            SetDisplay(0, 0, 5119, 4095, 400, 400);
            Protected = true;                        
        }

        public BaseOrb(Serial serial): base(serial)
        {
        }

        public override bool OnDragLift(Mobile from)
        {
            if (m_Instances.ContainsKey(from))
            {
                m_Instances.Remove(from);
                from.LocalOverheadMessage(Server.Network.MessageType.Regular, 0x3B2, false, "The orb deactivates.");
            }

            return true;
        }
        
        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {                
                //Deactivate Any Other Orbs Active
                Item[] m_Orbs = from.Backpack.FindItemsByType(typeof(BaseOrb));

                foreach (Item item in m_Orbs)
                {
                    BaseOrb orb = item as BaseOrb;

                    if (orb != this)                    
                        orb.m_Activated = false;                    
                }

                from.SendGump(new OrbGump(this, from, 0));
            }

            else
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1042001); // That must be in your pack for you to use it.
                return;
            }
        }

        public static void HandleDeath(DeathEventType deathEntryType)
        {
            Queue q = new Queue();

            foreach (KeyValuePair<Mobile, BaseOrb> entry in m_Instances)
            {
                Mobile orbHolder = entry.Key;
                BaseOrb orb = entry.Value;

                bool correctOrb = false;
                
                switch (deathEntryType)
                {
                    case DeathEventType.Player:
                        OrbOfHolySight orbOfHolySight = orb as OrbOfHolySight;

                        if (orbOfHolySight != null)
                            correctOrb = true;
                    break;

                    case DeathEventType.Murderer:
                        OrbOfDreadSight orbOfDreadSight = orb as OrbOfDreadSight;

                        if (orbOfDreadSight != null)
                            correctOrb = true;
                    break;

                    case DeathEventType.Paladin:                        
                    break;
                }

                if (!correctOrb || !orb.m_Activated)
                    continue;

                if (orb == null || orbHolder == null || !orb.IsChildOf(orbHolder.Backpack))
                {
                    q.Enqueue(orbHolder);
                    continue;
                }

                orbHolder.CloseGump(typeof(OrbGump));

                orbHolder.LocalOverheadMessage(Server.Network.MessageType.Regular, 0x3B2, false, "Your orb begins to illuminate.");
                orbHolder.FixedParticles(0x37B9, 1, 19, 0x251D, EffectLayer.Waist);
                orbHolder.PlaySound(0xF7);
            }

            while (q.Count > 0)
                m_Instances.Remove((Mobile)q.Dequeue());
        }

        private bool IsVisibleRegion()
        {
            return (targetMap == Map.Felucca && (Bounds.Start.X < (5119 + MapDistance) && (Bounds.Start.Y < 4095 + MapDistance)));
        }

        public static Point3D GetOrbWildernessRandomLocation(Point3D location, Map map)
        {
            Region region = Region.Find(location, map);
            DungeonRegion dungeonRegion = region as DungeonRegion;

            Point3D newLocation = location;

            //Keep Location if Death is in Dungeon & Use Random Spot Near Area for Wilderness Death
            if (dungeonRegion == null)
            {
                int x = newLocation.X;

                int xOffset = Utility.RandomMinMax(0, BaseOrb.RandomRadius);
                if (Utility.RandomDouble() >= .5)
                    xOffset *= -1;

                x += xOffset;

                int y = newLocation.Y;

                int yOffset = Utility.RandomMinMax(0, BaseOrb.RandomRadius);
                if (Utility.RandomDouble() >= .5)
                    yOffset *= -1;

                y += yOffset;

                newLocation.X = x;
                newLocation.Y = y;
            }

            return newLocation;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }

    public class OrbGump : Server.Gumps.Gump
    {
        public BaseOrb m_Orb;
        public Mobile m_From;
        public DeathEventType m_DeathEventType;
        public List<DeathEventEntry> m_FilteredDeathEventEntries = new List<DeathEventEntry>();

        public int m_Page = 0;

        public const int m_EntriesPerPage = 10;

        public OrbGump(BaseOrb orb, Mobile from, int page): base(0, 0)
        {
            if (orb == null || from == null || page == null)
                return;
            
            m_Orb = orb;
            m_From = from;            
            m_Page = page;

            PlayerMobile pm_From = m_From as PlayerMobile;

            if (pm_From == null)
                return;

            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            this.AddPage(0);

            this.AddImage(23, 44, 206);

            this.AddImageTiled(127, 147, 21, 21, 200);
            this.AddImageTiled(67, 85, 539, 424, 200);

            this.AddImage(66, 44, 201);
            this.AddImage(178, 44, 201);
            this.AddImage(605, 82, 203);
            this.AddImage(605, 465, 205);
            this.AddImage(605, 149, 203);
            this.AddImage(22, 465, 204);
            this.AddImage(604, 44, 207);
            this.AddImage(178, 465, 233);
            this.AddImage(66, 465, 233);
            this.AddImage(23, 88, 202);
            this.AddImage(23, 149, 202);
            this.AddImage(615, 77, 10441);   
        
            this.AddBackground(65, 100, 550, 350, 9200);

            int baseTextHue = 2036;
            int textHue = 2036;            

            OrbOfHolySight orbOfHolySight = m_Orb as OrbOfHolySight;
            OrbOfDreadSight orbOfDreadSight = m_Orb as OrbOfDreadSight;

            foreach (DeathEventEntry entry in PaladinEvents.m_DeathEventEntries)
            {
                bool correctOrb = false;

                switch (entry.m_DeathEventType)
                {
                    case DeathEventType.Player:
                        if (orbOfHolySight != null)
                            correctOrb = true;
                        break;

                    case DeathEventType.Murderer:
                        if (orbOfDreadSight != null)
                            correctOrb = true;
                        break;

                    case DeathEventType.Paladin:
                        break;
                }

                if (correctOrb && entry.m_EventTime + PaladinEvents.DeathEntryExpiration > DateTime.UtcNow)
                    m_FilteredDeathEventEntries.Add(entry);
            }

            //Show Most Recent First
            m_FilteredDeathEventEntries.Reverse();
            
            if (orbOfHolySight != null)
            {
                textHue = 0x59;
                AddLabel(300, 75, textHue, @"Recent Murders");
            }

            if (orbOfDreadSight != null)
            {
                textHue = 0x22;
                AddLabel(290, 75, textHue, @"Recent Paladin Justice");
            }

            //Not Built
            //AddLabel(300, 75, textHue, @"Recent Paladin Slayings");                       

            AddLabel(125, 115, 149, @"Player Name");
            AddLabel(250, 115, 149, @"Location");
            AddLabel(415, 115, 149, @"Time Ago");
            AddLabel(500, 115, 149, @"Players Involved");

            AddPage(1);

            int totalEntries = m_FilteredDeathEventEntries.Count;

            if (totalEntries > 0)
            {
                //Decrease Page if 
                if ((m_Page * m_EntriesPerPage) > totalEntries)
                    m_Page = (int)(Math.Floor((double)totalEntries / 10));

                if (m_Page >= totalEntries)
                    m_Page = totalEntries - 1;

                int startingEntryIndex = m_Page * m_EntriesPerPage;
                int endingEntryIndex;

                if (totalEntries > startingEntryIndex + m_EntriesPerPage)
                    endingEntryIndex = startingEntryIndex + m_EntriesPerPage;
                else
                    endingEntryIndex = totalEntries;

                int yStart = 145;

                //Death Event Entries
                if (m_Orb.m_Activated)
                {
                    for (int a = startingEntryIndex; a < endingEntryIndex; a++)
                    {
                        DeathEventEntry entry = m_FilteredDeathEventEntries[a];  

                        Region region = Region.Find(entry.m_Location, entry.m_Map);
                        DungeonRegion dungeonRegion = region as DungeonRegion;

                        string sLocation = "";

                        if (dungeonRegion != null)                        
                            sLocation = dungeonRegion.Name;
                        
                        else
                            sLocation = entry.m_RandomLocation.X.ToString() + ", " + entry.m_RandomLocation.Y.ToString();

                        int minutes = Math.Abs((entry.m_EventTime - DateTime.UtcNow).Minutes);
                        int seconds = Math.Abs((entry.m_EventTime - DateTime.UtcNow).Seconds);

                        string sTime = "";
                        string sMinutes = minutes.ToString() + " min";
                        string sSeconds = "";

                        if (seconds >= 0 && seconds < 10)
                            sSeconds = "0" + seconds.ToString() + " sec";
                        else
                            sSeconds = seconds.ToString() + " sec";

                        sTime = sMinutes + " " + sSeconds;                        

                        AddButton(80, yStart, 4008, 4010, 10 + a, GumpButtonType.Reply, 0); //Launch Map Button

                        AddLabel(125, yStart, textHue, entry.m_Victim.RawName); //Player Name                
                        AddLabel(250, yStart, textHue, sLocation); //Location        
                        AddLabel(400, yStart, textHue, sTime); //Time
                        AddLabel(540, yStart, textHue, entry.m_Killers.Count.ToString()); //Players Involved

                        yStart += 28;
                    }
                }

                if (m_Page > 0)
                {
                    //Previous Page
                    AddButton(325, 462, 9909, 9909, 2, GumpButtonType.Reply, 0);
                    AddLabel(355, 462, baseTextHue, @"Previous Page");
                }

                if (totalEntries > endingEntryIndex)
                {
                    //Next Page                
                    AddLabel(480, 462, baseTextHue, @"Next Page");
                    AddButton(550, 462, 9903, 9903, 3, GumpButtonType.Reply, 0);
                }
            }

            //Currently Active: Click to Deactivate
            if (m_Orb.m_Activated)
            {
                AddButton(80, 459, 2154, 2154, 1, GumpButtonType.Reply, 0);
                AddLabel(120, 462, baseTextHue, @"Orb is activated");
            }

            else
            {
                AddButton(80, 459, 2151, 2151, 1, GumpButtonType.Reply, 0);
                AddLabel(120, 462, baseTextHue, @"Orb is deactivated");
            }            
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (sender.Mobile == null || info == null || m_Orb == null || m_Page == null)
                return;
            
            PlayerMobile pm_From = sender.Mobile as PlayerMobile;

            if (pm_From == null)
                return;

            if (info.ButtonID == 1)
            {
                //Activate
                if (!m_Orb.m_Activated)
                {
                    m_Orb.m_Activated = true;

                    if (!BaseOrb.m_Instances.ContainsKey(pm_From))
                        BaseOrb.m_Instances.Add(pm_From, m_Orb);

                    pm_From.SendMessage("You activate the orb.");
                }

                //Deactivate
                else
                {
                    m_Orb.m_Activated = false;

                    if (BaseOrb.m_Instances.ContainsKey(pm_From))
                        BaseOrb.m_Instances.Remove(pm_From);

                    pm_From.SendMessage("You deactivate the orb.");
                }

                pm_From.CloseGump(typeof(OrbGump));
                pm_From.SendGump(new OrbGump(m_Orb, pm_From, m_Page));
            }

            if (info.ButtonID == 2)
            {
                //Previous Page  
                if (m_Page > 0)
                    m_Page--;

                pm_From.CloseGump(typeof(OrbGump));
                pm_From.SendGump(new OrbGump(m_Orb, pm_From, m_Page));
            }

            if (info.ButtonID == 3)
            {
                //Next Page                        
                m_Page++;

                pm_From.CloseGump(typeof(OrbGump));
                pm_From.SendGump(new OrbGump(m_Orb, pm_From, m_Page));
            }

            if (info.ButtonID >= 10)
            {
                pm_From.CloseGump(typeof(OrbGump));
                pm_From.SendGump(new OrbGump(m_Orb, pm_From, m_Page));

                int arrayIndex = info.ButtonID - 10;

                if (m_FilteredDeathEventEntries.Count <= arrayIndex)
                    return;

                DeathEventEntry entry = m_FilteredDeathEventEntries[arrayIndex];

                if (entry == null)
                    return;

                m_Orb.Map = entry.m_Map;
                m_Orb.ClearPins();

                Region region = Region.Find(entry.m_Location, entry.m_Map);
                DungeonRegion dungeonRegion = region as DungeonRegion;

                string sLocation = "";

                Point3D pinPoint = entry.m_Location;

                if (dungeonRegion != null)
                    pinPoint = dungeonRegion.EntranceLocation;
                else
                    pinPoint = entry.m_RandomLocation;

                m_Orb.SetDisplay(pinPoint.X - BaseOrb.MapDistance, pinPoint.Y - BaseOrb.MapDistance, pinPoint.X + BaseOrb.MapDistance, pinPoint.Y + BaseOrb.MapDistance, BaseOrb.MapWidth, BaseOrb.MapHeight);
                m_Orb.AddWorldPin(pinPoint.X, pinPoint.Y);

                pm_From.Send(new MapItem.MapDetails(m_Orb));
                pm_From.Send(new MapItem.MapDisplay(m_Orb));

                for (int i = 0; i < m_Orb.m_Pins.Count; i++)
                    pm_From.Send(new MapItem.MapAddPin(m_Orb, m_Orb.m_Pins[i]));

                pm_From.Send(new MapItem.MapSetEditable(m_Orb, m_Orb.ValidateEdit(pm_From)));                
            }
        }
    }
}


