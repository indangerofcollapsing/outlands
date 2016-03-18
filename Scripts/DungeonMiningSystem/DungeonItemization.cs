using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Server;
using Server.Network;
using Server.Gumps;
using Server.Items;

namespace Server.PortalSystem
{
    // See descriptions in itemization .xml
    public class PortalContentEntry
    {
        public static string s_behavior_door = "door";

        public int m_key;
        public string m_behavior;
        public string m_category;
        public string m_subcategory;
        public string m_name;
        public short m_rarity;

        public bool m_canVendor;
        public int m_vendorPrice;
        public int m_vendorQuantity;

        public PortalContentEntry(int key, string behavior, string category, string subcategory, string name, short rarity)
        {
            m_key = key;
            m_behavior = behavior;
            m_category = category;
            m_subcategory = subcategory;
            m_name = name;
            m_rarity = rarity;

            m_canVendor = false;
        }
        public void SetAsVendorable(int vendorPrice, short vendorQuantity)
        {
            if (vendorPrice > 0 && vendorQuantity > 0)
            {
                m_canVendor = true;
                m_vendorPrice = vendorPrice;
                m_vendorQuantity = vendorQuantity;
            }
        }
    }
    // deprecated
    public class PortalSingleContentEntry : PortalContentEntry
    {
        public int m_gid;
        public PortalSingleContentEntry(string span, string behavior, string category, string subcategory, string name, short rarity, int gid)
            : base(gid, behavior, category, subcategory, name, rarity)
        {
            m_gid = gid;
        }
    }
    public class PortalRangedContentEntry : PortalContentEntry
    {
        public List<IntervalTree.Interval<int>> m_intervals;
        public PortalRangedContentEntry(string behavior, string category, string subcategory, string name, short rarity, int entryKey, List<IntervalTree.Interval<int>> intervals)
            : base(entryKey, behavior, category, subcategory, name, rarity)
        {
            m_intervals = intervals;
        }
        public IntervalTree.Interval<int> GetIntervalForGid(int gid)
        {
            foreach (IntervalTree.Interval<int> interval in m_intervals)
                if (gid >= interval.Start && gid <= interval.End-1)
                    return interval;
            return new IntervalTree.Interval<int>(0, 1); // bad
        }
    }
    public class PortalInjector : Item
    {
        public static readonly int s_itemId = 0x2831;
        private string m_desc;
        public int m_key;

        public PortalInjector(PortalContentEntry entry)
            : base(s_itemId)
        {
            m_key = entry.m_key;
            m_desc = String.Format("Portal scroll: ({0})", entry.m_name);
        }
        public PortalInjector(Serial serial)
            : base(serial)
        {
        }
        public override string DefaultName
        {
            get { return m_desc; }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            // Version
            writer.Write(0);
            writer.Write(m_key);
            writer.Write(m_desc);
        }
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            switch (version)
            {
                case 0:
                    {
                        m_key = reader.ReadInt();
                        m_desc = reader.ReadString();
                        break;
                    }
            }

        }
        public override void OnDoubleClick(Mobile from)
        {
            // Display contents
            from.SendGump(new PortalInjectorContentsGump(m_key));
        }

        public static PortalInjector CreateRandom()
        {
            var entries = Server.PortalSystem.PortalsItemization.InventoryLibrary.GetPortalContentEntryList();
            return new PortalInjector(entries[Utility.RandomMinMax(0, entries.Count - 1)]);
        }
    }

    public class PortalInjectorContentsGump : Gump
    {
        public PortalInjectorContentsGump(int entryId)
            : base(100, 100)
        {
            PortalContentEntry entry = PortalsItemization.InventoryLibrary.GetPortalContentEntry(entryId);
            if (entry == null)
                   return;

            string message = "Here are the contents of the collection.";
            AddPage(0);
            AddBackground(44, 47, 537, 612, 83);
            AddBackground(64, 180, 39, 458, 2620); // Patch
            AddBackground(64, 170, 486, 468, 2620);
            AddHtml(64, 64, 486, 62, message, true, true);

            int relativePage = 1;
            AddPage(relativePage);

            int entriesPerRow = 8;
            int entrySpace = 50;
            int startingX = 100;
            int startingY = 200;
            int columnOffset = 130;
            int xMax = startingX + (entriesPerRow - 1) * entrySpace;
            int x = startingX;
            int y = startingY;
            int entriesMax = 24;
            int nextButtonX = 517;
            int nextButtonLabelX = nextButtonX - 32;
            int backButtonX = 70;
            int backButtonLabelX = backButtonX + 35;
            int footerY = 609;
            int pathX = 70;
            int pathY = 150;

            string category = entry.m_category;
            string subcategory = entry.m_subcategory;

            AddLabel(pathX, pathY, 0xFF, "Categories -> " + category + " -> " + subcategory + " -> " + entry.m_name + " pg.1");

            List<int> entryContents = new List<int>();
            if (entry is PortalRangedContentEntry)
            {
                PortalRangedContentEntry rangedEntry = entry as PortalRangedContentEntry;
                foreach (IntervalTree.Interval<int> interval in rangedEntry.m_intervals)
                    for (int i = interval.Start; i <= interval.End-1; ++i)
                        entryContents.Add(i);
            }
            else if (entry is PortalSingleContentEntry)
            {
                PortalSingleContentEntry singleEntry = entry as PortalSingleContentEntry;
                entryContents.Add(singleEntry.m_gid);
            }

            int nEntries = 1;
            foreach (int id in entryContents)
            {
                AddItem(x, y + 20, id);
                AddLabel(x, y - 20, 0xFF, "x" + entry.m_vendorQuantity);

                x += entrySpace;
                if (x > xMax)
                {
                    x = startingX;
                    y += columnOffset;
                }
                if (nEntries >= entriesMax)
                {
                    nEntries = 0;
                    x = startingX;
                    y = startingY;

                    AddButton(nextButtonX, footerY, 4005, 4007, 0, GumpButtonType.Page, relativePage + 1);
                    AddLabel(nextButtonLabelX, footerY, 0xFF, "Next");

                    AddPage(++relativePage);

                    AddButton(backButtonX, footerY, 4014, 4016, 0, GumpButtonType.Page, relativePage - 1);
                    AddLabel(backButtonLabelX, footerY, 0xFF, "Back");
                    AddLabel(pathX, pathY, 0xFF, "Categories -> " + category + " -> " + subcategory + " -> " + entry.m_name + " pg." + relativePage.ToString());
                }
                ++nEntries;
            }
        }
    }

    /// <summary>
    /// PortalInventory defines the itemization schema.
    /// </summary>
    public class PortalsItemization
    {
        // Xml constants
        public static readonly string s_tag_root = "itemization";
        public static readonly string s_tag_entry = "entry";
        public static readonly string s_tag_entry_attr_gid = "gid";
        public static readonly string s_tag_entry_attrval_span_single = "single";
        public static readonly string s_tag_entry_attrval_span_intervals = "intervals";
        public static readonly string s_tag_entry_attr_span_ranged_min = "start";
        public static readonly string s_tag_entry_attr_span_ranged_max = "end";
        public static readonly string s_tag_entry_attr_rarity = "rarity";
        public static readonly string s_tag_entry_attr_behavior = "behavior";
        public static readonly string s_tag_entry_attr_category = "category";
        public static readonly string s_tag_entry_attr_subcategory = "subcategory";
        public static readonly string s_tag_entry_attr_name = "name";
        public static readonly string s_tag_entry_vendor_attr_cost = "cost";
        public static readonly string s_tag_entry_vendor_attr_quantity = "quantity";
        public static PortalInventory s_inventory;

        public static void LoadItemizationFromXml(string filepath)
        {
            InventoryLibrary.LoadFromXml(filepath);

            // Load every entry into the system inventory
            foreach (PortalContentEntry entry in InventoryLibrary.GetPortalContentEntryList())
                GetSystemInventory().Add(entry);
        }

        public static PortalInventory GetSystemInventory()
        {
            if (s_inventory == null)
                s_inventory = new PortalInventory();
            return s_inventory;
        }
        public class InventoryLibrary
        {
            // System references
            /// <summary>
            /// key - gid of starting element within the entry group
            /// </summary>
            public static Dictionary<int, PortalContentEntry> s_keyToEntryMap { get; private set; }
            ///// <summary>
            ///// key - gid of an element
            ///// val - key of an entry
            ///// </summary>
            //public static Dictionary<int, int> s_gidToKeyMap {get; private set;}
            public static IntervalTree.IntervalTree<int, int> s_gidIntervalToKeyTree { get; private set; }

            public Dictionary<string, HashSet<string>> m_categoryToSubcategoryMap { get; private set; }
            // key:category, value:hashset<itemId> (ex. Floors -> 0x346E, 0x343B, 0x320D)
            public Dictionary<string, HashSet<int>> m_categoryToSubcategoryIdMap { get; private set; }

            // Subcategories
            // key:subcategory, value:hashset<itemId> (ex. Acid Floors -> 0x2E02-0x2E3B)
            public Dictionary<string, HashSet<int>> m_subcategoryToIdMap { get; private set; }
            // key:entry gid, value:PortalContentEntry

            public InventoryLibrary()
            {
                m_categoryToSubcategoryMap = new Dictionary<string, HashSet<string>>();
                m_categoryToSubcategoryIdMap = new Dictionary<string, HashSet<int>>();
                m_subcategoryToIdMap = new Dictionary<string, HashSet<int>>();
            }
            public void Serialize(GenericWriter writer)
            {
                // Version
                writer.Write(0);

                writer.Write(m_categoryToSubcategoryMap.Count);
                foreach (KeyValuePair<string,HashSet<string>> kvp in m_categoryToSubcategoryMap)
                {
                    writer.Write(kvp.Key);
                    writer.Write(kvp.Value.Count);
                    foreach (string subcategory in kvp.Value)
                        writer.Write(subcategory);
                }

                writer.Write(m_categoryToSubcategoryIdMap.Count);
                foreach (KeyValuePair<string,HashSet<int>> kvp in m_categoryToSubcategoryIdMap)
                {
                    writer.Write(kvp.Key);
                    writer.Write(kvp.Value.Count);
                    foreach (int subcategoryId in kvp.Value)
                        writer.Write(subcategoryId);
                }

                writer.Write(m_subcategoryToIdMap.Count);
                foreach (KeyValuePair<string, HashSet<int>> kvp in m_subcategoryToIdMap)
                {
                    writer.Write(kvp.Key);
                    writer.Write(kvp.Value.Count);
                    foreach (int id in kvp.Value)
                        writer.Write(id);
                }
            }
            public void Deserialize(GenericReader reader)
            {
                int version = reader.ReadInt();

                if (version >= 0)
                {
                    int catToSubcatCount = reader.ReadInt();
                    for (int i = 0; i < catToSubcatCount; ++i)
                    {
                        string key = reader.ReadString();
                        int valueCount = reader.ReadInt();
                        string[] values = new string[valueCount];
                        for (int j = 0; j < valueCount; ++j)
                            values[j] = reader.ReadString();

                        m_categoryToSubcategoryMap.Add(key, new HashSet<string>(values));
                    }

                    int catToSubcatIdCount = reader.ReadInt();
                    for (int i = 0; i < catToSubcatIdCount; ++i)
                    {
                        string key = reader.ReadString();
                        int valueCount = reader.ReadInt();
                        int[] values = new int[valueCount];
                        for (int j = 0; j < valueCount; ++j)
                            values[j] = reader.ReadInt();

                        m_categoryToSubcategoryIdMap.Add(key, new HashSet<int>(values));
                    }

                    int subcatToIdCount = reader.ReadInt();
                    for (int i = 0; i < subcatToIdCount; ++i)
                    {
                        string key = reader.ReadString();
                        int valueCount = reader.ReadInt();
                        int[] values = new int[valueCount];
                        for (int j = 0; j < valueCount; ++j)
                            values[j] = reader.ReadInt();

                        m_subcategoryToIdMap.Add(key, new HashSet<int>(values));
                    }
                }
            }

            public static void SystemInit()
            {
                if (s_keyToEntryMap == null)
                    s_keyToEntryMap = new Dictionary<int, PortalContentEntry>();
                s_gidIntervalToKeyTree = new IntervalTree.IntervalTree<int,int>();

            }
            public static void LoadFromXml(string filepath)
            {
                SystemInit();

                try
                {
                    // General (xml)
                    XmlDocument doc = new XmlDocument();
                    doc.Load(filepath);

                    // Content
                    XmlNodeList entryXmlList = doc.GetElementsByTagName(s_tag_entry);
                    foreach (XmlNode entryXmlNode in entryXmlList)
                    {
                        PortalContentEntry entry = null;

                        XmlAttributeCollection entryAttr = entryXmlNode.Attributes;

                        XmlNode nameAttrXmlNode = entryAttr.GetNamedItem(s_tag_entry_attr_name);
                        string name = nameAttrXmlNode != null ? nameAttrXmlNode.Value : String.Empty;

                        XmlNode rarityAttrXmlNode = entryAttr.GetNamedItem(s_tag_entry_attr_rarity);
                        short rarity = rarityAttrXmlNode != null ? Convert.ToInt16(rarityAttrXmlNode.Value, 10) : (short)0;
                        XmlNode behaviorAttrXmlNode = entryAttr.GetNamedItem(s_tag_entry_attr_behavior);
                        if (behaviorAttrXmlNode == null)
                            throw new Exception(String.Format("Attribute \"{0}\" missing in entry \"{1}\". Verify XML at {2}", s_tag_entry_attr_behavior, name, filepath));

                        string behavior = behaviorAttrXmlNode.Value;
                        XmlNode categoryAttrXmlNode = entryAttr.GetNamedItem(s_tag_entry_attr_category);
                        if (categoryAttrXmlNode == null)
                            throw new Exception(String.Format("Attribute \"{0}\" missing in entry \"{1}\". Verify XML at {2}", s_tag_entry_attr_category, name, filepath));

                        string category = categoryAttrXmlNode.Value;
                        XmlNode subcategoryAttrXmlNode = entryAttr.GetNamedItem(s_tag_entry_attr_subcategory);
                        if (subcategoryAttrXmlNode == null)
                            throw new Exception(String.Format("Attribute \"{0}\" missing in entry \"{1}\". Verify XML at {2}", s_tag_entry_attr_subcategory, name, filepath));
                        string subcategory = subcategoryAttrXmlNode.Value;

                        int key = 0;
                        List<IntervalTree.Interval<int>> intervalList = new List<IntervalTree.Interval<int>>();
                        XmlNode intervalsNode = entryXmlNode.SelectSingleNode("intervals");
                        if (intervalsNode != null)
                        {
                            foreach (XmlNode intervalNode in intervalsNode.ChildNodes)
                            {
                                XmlAttributeCollection entryIntervalAttrXmlNode = intervalNode.Attributes;
                                XmlNode startAttrXmlNode = entryIntervalAttrXmlNode.GetNamedItem(s_tag_entry_attr_span_ranged_min);
                                if (startAttrXmlNode == null)
                                    throw new Exception(String.Format("Attribute \"{0}\" missing in entry interval \"{1}\". Verify XML at {2}", s_tag_entry_attr_span_ranged_min, name, filepath));
                                int start = Convert.ToInt32(startAttrXmlNode.Value, 16);
                                if (start == 0)
                                    throw new Exception(String.Format("Value {0} is zero in entry interval \"{1}\". Verify XML at {2}", s_tag_entry_attr_span_ranged_min, name, filepath));

                                XmlNode endAttrXmlNode = entryIntervalAttrXmlNode.GetNamedItem(s_tag_entry_attr_span_ranged_max);
                                if (endAttrXmlNode == null)
                                    throw new Exception(String.Format("Attribute \"{0}\" missing in entry interval \"{1}\". Verify XML at {2}", s_tag_entry_attr_span_ranged_max, name, filepath));
                                int end = Convert.ToInt32(endAttrXmlNode.Value, 16);
                                if (end == 0)
                                    throw new Exception(String.Format("Value {0} is zero in entry interval \"{1}\". Verify XML at {2}", s_tag_entry_attr_span_ranged_max, name, filepath));

                                if (start > end)
                                    throw new Exception(String.Format("Span \"{0}\"-\"{1}\" in entry interval \"{2}\" is invalid. Verify XML at {1}", start.ToString("X"), end.ToString("X"), name));

                                intervalList.Add(new IntervalTree.Interval<int>(start, end + 1));
                            }
                        }

                        XmlNode singlesNode = entryXmlNode.SelectSingleNode("singles");
                        if (singlesNode != null)
                        {
                            foreach (XmlNode singleNode in singlesNode.ChildNodes)
                            {
                                XmlAttributeCollection entryIntervalAttrXmlNode = singleNode.Attributes;
                                XmlNode gidAttrXmlNode = entryIntervalAttrXmlNode.GetNamedItem(s_tag_entry_attr_gid);
                                if (gidAttrXmlNode == null)
                                    throw new Exception(String.Format("Attribute \"{0}\" missing in entry single \"{1}\". Verify XML at {2}", s_tag_entry_attr_gid, name, filepath));
                                int gid = Convert.ToInt32(gidAttrXmlNode.Value, 16);
                                if (gid == 0)
                                    throw new Exception(String.Format("Value {0} is zero in entry single \"{1}\". Verify XML at {2}", s_tag_entry_attr_gid, name, filepath));

                                intervalList.Add(new IntervalTree.Interval<int>(gid, gid + 1)); // problem here +1
                            }
                        }
                        
                        key = intervalList[0].Start;
                        entry = new PortalRangedContentEntry(behavior, category, subcategory, name, rarity, key, intervalList);

                        // Vendor
                        XmlNode vendorXmlNode = entryXmlNode.SelectSingleNode("vendor");
                        if (vendorXmlNode == null || entry == null)
                            continue;

                        XmlAttributeCollection vendorAttr = vendorXmlNode.Attributes;
                        XmlNode costAttrXmlNode = vendorAttr.GetNamedItem(PortalsItemization.s_tag_entry_vendor_attr_cost);
                        int cost = costAttrXmlNode != null ? Convert.ToInt32(costAttrXmlNode.Value, 10) : 0;

                        XmlNode quantityAttrXmlNode = vendorAttr.GetNamedItem(PortalsItemization.s_tag_entry_vendor_attr_quantity);
                        short quantity = quantityAttrXmlNode != null ? Convert.ToInt16(quantityAttrXmlNode.Value, 10) : (short)1;
                        entry.SetAsVendorable(cost, quantity);

                        // System reference
                        AddEntryToSystem(entry);
                    }
                }
                catch (Exception error)
                {
                    PortalsSystem.WriteError("An exception occured processing portals itemization xml.", error);
                }
            }
            public static void AddEntryToSystem(PortalContentEntry entry)
            {
                if (!s_keyToEntryMap.ContainsKey(entry.m_key))
                    s_keyToEntryMap.Add(entry.m_key, entry);

                PortalRangedContentEntry rangedEntry = entry as PortalRangedContentEntry;
                foreach (IntervalTree.Interval<int> interval in rangedEntry.m_intervals)
                    s_gidIntervalToKeyTree.Add(interval, entry.m_key);

            //    if (entry is PortalSingleContentEntry)
            //    {
            //        IntervalTree.Interval<int> interval = new IntervalTree.Interval<int>(entry.m_key, entry.m_key + 1);
            //        s_gidIntervalToKeyTree.Add(interval, entry.m_key);
            //    }
            //    else if (entry is PortalRangedContentEntry)
            //    {
            //        PortalRangedContentEntry rangedEntry = entry as PortalRangedContentEntry;
            //        foreach (IntervalTree.Interval<int> interval in rangedEntry.m_intervals)
            //            s_gidIntervalToKeyTree.Add(interval, entry.m_key);
            //    }
            //
            }

            public static List<PortalContentEntry> GetPortalContentEntryList()
            {
                return s_keyToEntryMap.Values.ToList();
            }
            public static PortalContentEntry GetPortalContentEntry(int entryKey)
            {
                return s_keyToEntryMap.ContainsKey(entryKey) ? s_keyToEntryMap[entryKey] : null;   
            }
            public static PortalContentEntry GetPortalContentEntryWithGid(int gid)
            {
                IntervalTree.Interval<int> interval = new IntervalTree.Interval<int>(gid, gid+1);

                IEnumerable<KeyValuePair<IntervalTree.Interval<int>, int>> penum = s_gidIntervalToKeyTree.GetIntervalsOverlappingWith(interval);
                foreach (KeyValuePair<IntervalTree.Interval<int>, int> kvp in penum)
                    return s_keyToEntryMap[kvp.Value];// Expecting only 1.

                List<KeyValuePair<IntervalTree.Interval<int>, int>> startingList = s_gidIntervalToKeyTree.GetIntervalsStartingAt(gid);
                if (startingList != null && startingList.Count > 0)
                    return s_keyToEntryMap[startingList[0].Value]; // never hits

                List<KeyValuePair<IntervalTree.Interval<int>, int>> endingList = s_gidIntervalToKeyTree.GetIntervalsEndingAt(gid);
                if (endingList != null && endingList.Count > 0)
                    return s_keyToEntryMap[endingList[0].Value]; // always hits? will break with multiple different item sets... probably.

                //IEnumerable<KeyValuePair<IntervalTree.Interval<int>, int>> penum = s_gidIntervalToKeyTree.GetIntervalsOverlappingWith(interval);
                //foreach (KeyValuePair<IntervalTree.Interval<int>, int> kvp in penum)
                //{
                //    return s_keyToEntryMap[kvp.Value];// Expecting only 1.
                //}
                // return here
                return null; // bad
            }

            public Dictionary<string, HashSet<string>> GetSubcategoriesMap()
            {
                return m_categoryToSubcategoryMap;
            }
            public Dictionary<string, HashSet<int>> GetSubcategoryGidsMap()
            {
                return m_categoryToSubcategoryIdMap;
            }
            public Dictionary<string, HashSet<int>> GetentryKeysMap()
            {
                return m_subcategoryToIdMap;
            }
            public HashSet<string> GetSubcategories(string category)
            {
                return m_categoryToSubcategoryMap.ContainsKey(category) ? m_categoryToSubcategoryMap[category] : new HashSet<string>();
            }
            public HashSet<int> GetSubcategoryGids(string category)
            {
                return m_categoryToSubcategoryIdMap.ContainsKey(category) ? m_categoryToSubcategoryIdMap[category] : new HashSet<int>();
            }
            public HashSet<int> GetentryKeys(string subcategory)
            {
                return m_subcategoryToIdMap.ContainsKey(subcategory) ? m_subcategoryToIdMap[subcategory] : new HashSet<int>();
            }
            public bool Add(PortalContentEntry entry)
            {
                int key = entry.m_key;
                string category = entry.m_category;
                string subcategory = entry.m_subcategory;

                // Categories
                if (m_categoryToSubcategoryIdMap.ContainsKey(category))
                {
                    m_categoryToSubcategoryIdMap[category].Add(key);
                    m_categoryToSubcategoryMap[category].Add(subcategory);
                }
                else
                {
                    HashSet<int> idSet = new HashSet<int>();
                    idSet.Add(key);
                    m_categoryToSubcategoryIdMap.Add(category, idSet);
                    HashSet<string> subcategorySet = new HashSet<string>();
                    subcategorySet.Add(subcategory);
                    m_categoryToSubcategoryMap.Add(category, subcategorySet);
                }

                // Subcategories
                if (m_subcategoryToIdMap.ContainsKey(subcategory))
                    m_subcategoryToIdMap[subcategory].Add(key);
                else
                {
                    HashSet<int> idSet = new HashSet<int>();
                    idSet.Add(key);
                    m_subcategoryToIdMap.Add(subcategory, idSet);
                }

                return true;
            }
            public bool Remove(PortalContentEntry entry)
            {
                int key = entry.m_key;
                string category = entry.m_category;
                string subcategory = entry.m_subcategory;

                // Categories are removed only if a subcategory is removed.
                if (m_subcategoryToIdMap.ContainsKey(subcategory))
                {
                    HashSet<int> entryIds = m_subcategoryToIdMap[subcategory];
                    if (entryIds.Contains(key) && entryIds.Remove(key) && entryIds.Count == 0)
                    {
                        // Subcategory is now empty
                        if (m_subcategoryToIdMap.ContainsKey(subcategory))
                            m_subcategoryToIdMap.Remove(subcategory);

                        // Now category
                        HashSet<int> subcategoryIds = m_categoryToSubcategoryIdMap[category];
                        if (subcategoryIds.Contains(key) && subcategoryIds.Remove(key) && subcategoryIds.Count == 0)
                        {
                            // Category is now empty
                            if (m_categoryToSubcategoryIdMap.ContainsKey(category))
                                m_categoryToSubcategoryIdMap.Remove(category);

                            if (m_categoryToSubcategoryMap.ContainsKey(category))
                                m_categoryToSubcategoryMap.Remove(category);
                        }
                    }
                }

                return true;
            }
            public bool IsEmpty()
            {
                return m_categoryToSubcategoryMap.Count == 0;
            }
            public void Clear()
            {
                m_categoryToSubcategoryMap.Clear();
                m_categoryToSubcategoryIdMap.Clear();
                m_subcategoryToIdMap.Clear();
            }
            public void Clone(InventoryLibrary library)
            {
                m_categoryToSubcategoryMap = new Dictionary<string, HashSet<string>>(library.m_categoryToSubcategoryMap);
                m_categoryToSubcategoryIdMap = new Dictionary<string, HashSet<int>>(library.m_categoryToSubcategoryIdMap);
                m_subcategoryToIdMap = new Dictionary<string, HashSet<int>>(library.m_subcategoryToIdMap);
            }
            public void FillAtRandom()
            {
                Random r = new Random();
                foreach (KeyValuePair<int,PortalContentEntry> kvp in s_keyToEntryMap)
                {
                    if (r.Next(1, 5) == 1) // 25% of complete list
                        Add(kvp.Value);
                }
            }
        }
        public class PortalInventory
        {
            public InventoryLibrary m_library;
            public PortalInventory()
            {
                m_library = new InventoryLibrary();
            }
            public InventoryLibrary GetLibrary()
            {
                return m_library;
            }
            public virtual bool Add(PortalContentEntry entry)
            {
                return m_library.Add(entry);
            }
            public virtual bool Remove(PortalContentEntry entry)
            {
                return m_library.Remove(entry);
            }
        }
        /// <summary>
        /// SystemPortalInventory considers inventory counts of entry contents.
        /// </summary>
        public class PartitionPortalInventory : PortalInventory
        {
            // History
            public List<int> m_selectionHistory;
            public static int s_maxSelectionHistory = 30;

            // Used in conjunction with s_gidToEntryMap to determine quantities available.
            // key:entry gid, value:itemId quantity array
            public Dictionary<int, int[]> m_intervalToEntryQuantitiesMap;

            public PartitionPortalInventory()
            {
                m_selectionHistory = new List<int>();
                m_selectionHistory.Capacity = 30;
                m_intervalToEntryQuantitiesMap = new Dictionary<int, int[]>();
            }
            /// <summary>
            /// Cleans up the history before returning.
            /// </summary>
            /// <returns></returns>
            public List<Pair<int,int>> GetSelectionHistory()
            {
                List<Pair<int, int>> elementsInventoried = new List<Pair<int, int>>();
                foreach (int gid in m_selectionHistory)
                {
                    PortalContentEntry entry = PortalsItemization.InventoryLibrary.GetPortalContentEntryWithGid(gid);
                    // Elements present in the inventory are displayed for quick-access.
                    if (GetCount(gid, entry.m_key) > 0)
                        elementsInventoried.Add(new Pair<int, int>(gid, entry.m_key));
                }

                // Keep the list clean of elements without count.
                m_selectionHistory.Clear();
                foreach (Pair<int, int> pair in elementsInventoried)
                   m_selectionHistory.Add(pair.First);

                return elementsInventoried;
            }
            public void AddSelectionHistory(int gid)
            {
                // An existing entry is moved to the end of the list.
                int historyCount = m_selectionHistory.Count;
                for (int i = 0; i < historyCount; ++i)
                {
                    if (m_selectionHistory[i] == gid)
                    {
                        int value = m_selectionHistory[i];
                        m_selectionHistory.RemoveAt(i);
                        m_selectionHistory.Add(value);
                        return;
                    }
                }

                if (m_selectionHistory.Count >= 30)
                    m_selectionHistory.RemoveAt(0);

                m_selectionHistory.Add(gid);
            }
            public bool Inject(PortalInjector injector)
            {
                PortalContentEntry entry = InventoryLibrary.GetPortalContentEntry(injector.m_key);
                return Add(entry);
            }
            public bool Add(PortalContentEntry entry)
            {
                bool result = true;

                // Quantities
                PortalRangedContentEntry rangedEntry = entry as PortalRangedContentEntry;
                // Allocate if necessary
                if (!m_intervalToEntryQuantitiesMap.ContainsKey(entry.m_key))
                {
                    foreach (IntervalTree.Interval<int> interval in rangedEntry.m_intervals)
                        m_intervalToEntryQuantitiesMap.Add(interval.Start, new int[1 + interval.End-1 - interval.Start]);
                }

                foreach (IntervalTree.Interval<int> interval in rangedEntry.m_intervals)
                {
                    for (int gid = interval.Start; gid <= interval.End-1; ++gid)
                        result &= Add(gid, entry.m_key, entry.m_vendorQuantity);
                }


                //if (entry is PortalSingleContentEntry)
                //{
                //    PortalSingleContentEntry singleEntry = entry as PortalSingleContentEntry;
                //    // Allocate if necessary
                //    if (!m_intervalToEntryQuantitiesMap.ContainsKey(entry.m_key))
                //        m_intervalToEntryQuantitiesMap.Add(entry.m_key, new int[1]);

                //    result &= Add(singleEntry.m_gid, entry.m_key, entry.m_vendorQuantity);
                //}
                //else if (entry is PortalRangedContentEntry)
                //{
                //    PortalRangedContentEntry rangedEntry = entry as PortalRangedContentEntry;
                //    // Allocate if necessary
                //    if (!m_intervalToEntryQuantitiesMap.ContainsKey(entry.m_key))
                //    {
                //        foreach (IntervalTree.Interval<int> interval in rangedEntry.m_intervals)
                //            m_intervalToEntryQuantitiesMap.Add(interval.Start, new int[1 + interval.End - interval.Start]);
                //    }

                //    foreach (IntervalTree.Interval<int> interval in rangedEntry.m_intervals)
                //    {
                //        for (int gid = interval.Start; gid <= interval.End; ++gid)
                //            result &= Add(gid, entry.m_key, entry.m_vendorQuantity);
                //    }
                //}

                return result && m_library.Add(entry);
            }
            public bool Add(int gid, int entryKey, int count)
            {
                if (count == 0)
                    return false;

                PortalRangedContentEntry rangedEntry = InventoryLibrary.GetPortalContentEntry(entryKey) as PortalRangedContentEntry;
                IntervalTree.Interval<int> entryInterval = rangedEntry.GetIntervalForGid(gid);
                int intervalKey = entryInterval.Start;

                // Allocation may required
                if (!m_intervalToEntryQuantitiesMap.ContainsKey(intervalKey))
                    m_intervalToEntryQuantitiesMap.Add(entryInterval.Start, new int[1 + entryInterval.End-1 - entryInterval.Start]);

                int index = gid - intervalKey;
                if (index < m_intervalToEntryQuantitiesMap[intervalKey].Length - 1)
                    m_intervalToEntryQuantitiesMap[intervalKey][index] += count;

                // Library
                bool existsInLibrary = false;
                foreach (IntervalTree.Interval<int> interval in rangedEntry.m_intervals)
                {
                    if (m_intervalToEntryQuantitiesMap.ContainsKey(intervalKey))
                    {
                        existsInLibrary = true;
                        break;
                    }
                }
                if (!existsInLibrary)
                    m_library.Add(rangedEntry);

                return true;
            }
            public bool Remove(int gid, int entryKey, int count)
            {
                PortalContentEntry entry = InventoryLibrary.GetPortalContentEntry(entryKey);
                if (entry is PortalRangedContentEntry)
                {
                    PortalRangedContentEntry rangedEntry = entry as PortalRangedContentEntry;
                    IntervalTree.Interval<int> entryInterval = rangedEntry.GetIntervalForGid(gid);
                    int intervalKey = entryInterval.Start;

                    if (!m_intervalToEntryQuantitiesMap.ContainsKey(intervalKey) || count == 0)
                        return false;

                    int[] quantities = m_intervalToEntryQuantitiesMap[intervalKey];
                    quantities[gid - intervalKey] -= count;

                    // Entry is removed if quantities across all intervals are 0.
                    foreach (IntervalTree.Interval<int> interval in rangedEntry.m_intervals)
                    {
                        int[] intervalQuantities = m_intervalToEntryQuantitiesMap[interval.Start];
                        for (int i = 0; i < intervalQuantities.Length; ++i)
                            if (intervalQuantities[i] > 0)
                                return false;
                    }

                    // Clean
                    if (m_intervalToEntryQuantitiesMap.ContainsKey(entryKey))
                        m_intervalToEntryQuantitiesMap.Remove(entryKey);

                    m_library.Remove(entry);
                }
                return true;
            }
            public int GetCount(int gid)
            {
                return GetCount(gid, gid);
            }
            public int GetCount(int gid, int entryKey)
            {
                PortalContentEntry entry = InventoryLibrary.GetPortalContentEntry(entryKey);
                if (entry is PortalRangedContentEntry)
                {
                    PortalRangedContentEntry rangedEntry = entry as PortalRangedContentEntry;
                    IntervalTree.Interval<int> entryInterval = rangedEntry.GetIntervalForGid(gid);
                    int intervalKey = entryInterval.Start;

                    if (!m_intervalToEntryQuantitiesMap.ContainsKey(intervalKey))
                        return 0;

                    int[] quantities = m_intervalToEntryQuantitiesMap[intervalKey];
                    int idx = gid - intervalKey;
                    if (idx < 0 || idx >= quantities.Length)
                        return 0;
                    else
                        return quantities[idx];
                }

                return 0;
            }
            public void Serialize(GenericWriter writer)
            {
                // Version
                int version = 1;
                writer.Write(version);

                writer.Write(m_intervalToEntryQuantitiesMap.Count);
                foreach (KeyValuePair<int,int[]> kvp in m_intervalToEntryQuantitiesMap)
                {
                    writer.Write(kvp.Key);
                    int valueCount = kvp.Value.Length;
                    writer.Write(valueCount);
                    for (int i = 0; i < valueCount; ++i)
                        writer.Write(kvp.Value[i]);
                }

                int historyCount = m_selectionHistory.Count;
                writer.Write(historyCount);
                for (int i = 0; i < historyCount; ++i)
                    writer.Write(m_selectionHistory[i]);

                m_library.Serialize(writer);
            }
            public void Deserialize(GenericReader reader)
            {
                int version = reader.ReadInt();

                if (version >= 0)
                {
                    int keyCount = reader.ReadInt();
                    for (int i = 0; i < keyCount; ++i)
                    {
                        int key = reader.ReadInt();
                        int valueCount = reader.ReadInt();
                        int[] values = new int[valueCount];
                        for (int j = 0; j < valueCount; ++j)
                            values[j] = reader.ReadInt();

                        m_intervalToEntryQuantitiesMap.Add(key, values);
                    }
                }
                if (version >= 1)
                {
                    int count = reader.ReadInt();
                    for (int i = 0; i < count; ++i)
                        m_selectionHistory.Add(reader.ReadInt());
                }

                m_library.Deserialize(reader);
            }
        }
        /// <summary>
        /// VendorPortalInventory does not consider quantities.
        /// </summary>
        //public class VendorPortalInventory : PortalInventory
        //{
        //    // key: entry gid, value: entry quantity
        //    public Dictionary<int, int> m_entryIdToQuantityMap;
        //    public VendorPortalInventory()
        //    {
        //        m_entryIdToQuantityMap = new Dictionary<int, int>();
        //    }
        //    public bool Add(PortalContentEntry entry, int count)
        //    {
        //        if (!m_entryIdToQuantityMap.ContainsKey(entry.m_key))
        //            return false;
        //        else
        //            m_entryIdToQuantityMap[entry.m_key] += count;

        //       return base.Add(entry);
        //    }
        //    public bool Remove(PortalContentEntry entry, int count)
        //    {
        //        if (!m_entryIdToQuantityMap.ContainsKey(entry.m_key) || m_entryIdToQuantityMap[entry.m_key] < count)
        //            return false;
        //        else
        //            m_entryIdToQuantityMap[entry.m_key] -= count;
        //        return true;
        //    }
        //    public int GetCount(PortalContentEntry entry)
        //    {
        //        if (!m_entryIdToQuantityMap.ContainsKey(entry.m_key))
        //            return 0;
        //        else
        //            return m_entryIdToQuantityMap[entry.m_key];
        //    }
        //    public bool IsEmpty()
        //    {
        //        return m_library.IsEmpty();
        //    }
        //    public void CloneLibrary(InventoryLibrary library)
        //    {
        //        m_library.Clone(library);
        //    }
        //}
    }

    public static class PortalInventoryGumpUtility
    {
        public static readonly int s_entryStartX = 20;
        public static readonly int s_entryStartY = 80;
        public static readonly int s_entryRowSeparation = 130;
        public static readonly int s_entryColumnSeparation = 50;
        public static readonly int s_backgroundStyle1 = 83;
        public static readonly int s_backgroundStyle2 = 2620;
        public static readonly int s_backgroundStyle3 = 3000;
        public static readonly int s_fontColor = 0xFF;
        public static readonly int s_buttonSelectUp = 0x985;
        public static readonly int s_buttonSelectDown = 0x986;
        public static readonly int s_buttonNextUp = 4005;
        public static readonly int s_buttonNextDown = 4007;
        public static readonly int s_buttonBackUp = 4014;
        public static readonly int s_buttonBackDown = 4016;

        // 500-600 to avoid potential conflicts with button responses from a parent gump.
        public const int s_actionMin = 500;
        public const int s_actionMax = 600;
        public enum GumpActions
        {
            GA_Cancel = s_actionMin,
            GA_Confirm,
            GA_GotoCategory,
            GA_GotoSubcategory,
            GA_GotoEntry,
            GA_Back,
            GA_Purchase = s_actionMax, // Vendor
        };

        public static void CreateBoundary(Rectangle2D windowBounds, Gump parentGump)
        {
            // 2620 has an artifact that can be patched
            // Inventory patch
            parentGump.AddBackground(windowBounds.X, windowBounds.Y + 10, 40, windowBounds.Width - 20, s_backgroundStyle2);
            parentGump.AddBackground(windowBounds.X, windowBounds.Y, windowBounds.Width, windowBounds.Height, s_backgroundStyle2);
            parentGump.AddBackground(windowBounds.X, windowBounds.Y, windowBounds.Width, 33, s_backgroundStyle2);
        }
        public static void CreateCategoryPage(Rectangle2D windowBounds, Gump parentGump, PortalsItemization.PortalInventory inventory)
        {
            int relativePage = 1;
            parentGump.AddPage(relativePage);

            string currentNavPath = "Categories";
            BuildNavigationPath(windowBounds, parentGump, String.Format("{0} (pg.1)", currentNavPath));

            int entryStartX = windowBounds.X + s_entryStartX;
            int entryStartY = windowBounds.Y + s_entryStartY;
            int x = entryStartX;
            int y = entryStartY;

            PortalsItemization.InventoryLibrary schema = inventory.GetLibrary();
            foreach (KeyValuePair<string, HashSet<int>> categoryKvp in schema.GetSubcategoryGidsMap())
            {
                HashSet<int> subcategorySet = categoryKvp.Value;
                if (subcategorySet.Count == 0)
                    continue;
                int subcategoryId = subcategorySet.First();

                int buttonId = (subcategoryId << 16) + (int)GumpActions.GA_GotoCategory;
                parentGump.AddButton(x, y, s_buttonSelectUp, s_buttonSelectDown, buttonId, GumpButtonType.Reply, 0);
                parentGump.AddItem(x, y + 20, subcategoryId);

                x += s_entryColumnSeparation;
                if (x + s_entryColumnSeparation > windowBounds.X + windowBounds.Width)
                {
                    x = entryStartX;
                    y += s_entryRowSeparation;
                }
                if (y + s_entryRowSeparation >= windowBounds.Y + windowBounds.Height)
                {
                    x = entryStartX;
                    y = entryStartY;

                    // Complete page, and create the following page's return button and navigation path.
                    BuildNextPage(windowBounds, parentGump, ref relativePage, currentNavPath);
                }
            }
        }
        public static void CreateSubcategoryPage(Rectangle2D windowBounds, Gump parentGump, PortalsItemization.PortalInventory inventory, string category)
        {
            int relativePage = 1;
            parentGump.AddPage(relativePage);

            string currentNavPath = category;
            BuildNavigationPath(windowBounds, parentGump, String.Format("{0} (pg.1)", currentNavPath));

            BuildInitialReturnButton(windowBounds, parentGump);

            int entryStartX = windowBounds.X + s_entryStartX;
            int entryStartY = windowBounds.Y + s_entryStartY;
            int x = entryStartX;
            int y = entryStartY;

            PortalsItemization.InventoryLibrary schema = inventory.GetLibrary();
            HashSet<string> subcategoriesSet = schema.GetSubcategories(category);
            foreach (string subcat in subcategoriesSet)
            {
                HashSet<int> subcategoryContentIds = schema.GetentryKeys(subcat);
                if (subcategoryContentIds.Count == 0)
                    continue;
                int contentId = subcategoryContentIds.First();

                int buttonId = (contentId << 16) + (int)GumpActions.GA_GotoSubcategory;
                parentGump.AddButton(x, y, s_buttonSelectUp, s_buttonSelectDown, buttonId, GumpButtonType.Reply, 0);
                parentGump.AddItem(x, y + 20, contentId);

                x += s_entryColumnSeparation;
                if (x + s_entryColumnSeparation > windowBounds.X + windowBounds.Width)
                {
                    x = entryStartX;
                    y += s_entryRowSeparation;
                }
                if (y + s_entryRowSeparation >= windowBounds.Y + windowBounds.Height)
                {
                    x = entryStartX;
                    y = entryStartY;

                    // Complete page, and create the following page's return button and navigation path.
                    BuildNextPage(windowBounds, parentGump, ref relativePage, currentNavPath);
                }
            }
        }
        public static void CreateSubcategoryEntriesPage(Rectangle2D windowBounds, Gump parentGump,
            PortalsItemization.PortalInventory inventory, string category, string subcategory)
        {
            int relativePage = 1;
            parentGump.AddPage(relativePage);

            string currentNavPath = String.Format("{0}->{1}", category, subcategory);
            BuildNavigationPath(windowBounds, parentGump, String.Format("{0} (pg.1)", currentNavPath));

            BuildInitialReturnButton(windowBounds, parentGump);

            int entryStartX = windowBounds.X + s_entryStartX;
            int entryStartY = windowBounds.Y + s_entryStartY;
            int x = entryStartX;
            int y = entryStartY;

            PortalsItemization.InventoryLibrary schema = inventory.GetLibrary();
            HashSet<int> subcategoryContentIds = schema.GetentryKeys(subcategory);
            foreach (int id in subcategoryContentIds)
            {
                int buttonId = (id << 16) + (int)GumpActions.GA_GotoEntry;
                parentGump.AddButton(x, y, s_buttonSelectUp, s_buttonSelectDown, buttonId, GumpButtonType.Reply, 0);
                parentGump.AddItem(x, y + 20, id);

                PortalContentEntry entry = PortalsItemization.InventoryLibrary.GetPortalContentEntry(id);
                // deprecated
                //Boolean isSingle = entry.m_span == PortalsItemization.s_tag_entry_attrval_span_single;
                //parentGump.AddLabel(x, y - 20, isSingle ? 0x1F : 0xF1, isSingle ? "(Single)" : "(Set)");

                x += s_entryColumnSeparation;
                if (x + s_entryColumnSeparation > windowBounds.X + windowBounds.Width)
                {
                    x = entryStartX;
                    y += s_entryRowSeparation;
                }
                if (y + s_entryRowSeparation >= windowBounds.Y + windowBounds.Height)
                {
                    x = entryStartX;
                    y = entryStartY;

                    // Complete page, and create the following page's return button and navigation path.
                    BuildNextPage(windowBounds, parentGump, ref relativePage, currentNavPath);
                }
            }
        }
        public static void CreatePurchasingPage(Rectangle2D windowBounds, Gump parentGump,
            PortalsItemization.PortalInventory inventory, string category, string subcategory, int entryId)
        {
            PortalContentEntry entry = PortalsItemization.InventoryLibrary.GetPortalContentEntry(entryId);
            List<int> entryContents = new List<int>();

            PortalRangedContentEntry rangedEntry = entry as PortalRangedContentEntry;
            foreach (IntervalTree.Interval<int> interval in rangedEntry.m_intervals)
                for (int i = interval.Start; i <= interval.End-1; ++i)
                    entryContents.Add(i);

            //if (entry is PortalRangedContentEntry)
            //{
            //    PortalRangedContentEntry rangedEntry = entry as PortalRangedContentEntry;
            //    foreach (IntervalTree.Interval<int> interval in rangedEntry.m_intervals)
            //        for (int i = interval.Start; i <= interval.End; ++i)
            //            entryContents.Add(i);
            //}
            //else if (entry is PortalSingleContentEntry)
            //{
            //    PortalSingleContentEntry singleEntry = entry as PortalSingleContentEntry;
            //    entryContents.Add(entry.m_key);
            //}

            string currentNavPath = String.Format("{0}->{1}->{2}", category, subcategory, entry.m_name);
            BuildNavigationPath(windowBounds, parentGump, String.Format("{0} (pg.1)", currentNavPath));

            int uniquesPerEntry = entryContents.Count;
            int quantityPerUnique = entry.m_vendorQuantity;
            int price = entry.m_vendorPrice;

            BuildPurchasePanel(windowBounds, parentGump, price);

            // Changing content
            int relativePage = 1;
            parentGump.AddPage(relativePage);

            BuildInitialReturnButton(windowBounds, parentGump);

            int entryStartX = windowBounds.X + s_entryStartX;
            int entryStartY = windowBounds.Y + s_entryStartY;
            int x = entryStartX;
            int y = entryStartY;

            foreach (int id in entryContents)
            {
                int buttonId = (id << 16) + (int)GumpActions.GA_GotoEntry;
                parentGump.AddItem(x, y + 20, id);
                parentGump.AddLabel(x, y - 20, 0xFF, "x" + entry.m_vendorQuantity);

                x += s_entryColumnSeparation;
                if (x + s_entryColumnSeparation > windowBounds.X + windowBounds.Width)
                {
                    x = entryStartX;
                    y += s_entryRowSeparation;
                }
                if (y + s_entryRowSeparation >= windowBounds.Y + windowBounds.Height)
                {
                    x = entryStartX;
                    y = entryStartY;

                    // Complete page, and create the following page's return button and navigation path.
                    BuildNextPage(windowBounds, parentGump, ref relativePage, currentNavPath);
                }
            }
        }
        public static void BuildNextPage(Rectangle2D windowBounds, Gump parentGump, ref int relativePage, string navigationPath)
        {
            int footerY = GetActionFooterY(windowBounds);

            // Next page
            parentGump.AddButton(windowBounds.X + windowBounds.Width - 35, footerY,
                s_buttonNextUp, s_buttonNextDown, 0, GumpButtonType.Page, relativePage + 1);
            parentGump.AddLabel(windowBounds.X + windowBounds.Width - 85, footerY, s_fontColor, "Next->");

            parentGump.AddPage(++relativePage);

            // Previous page
            parentGump.AddButton(windowBounds.X + 5, footerY,
                s_buttonBackUp, s_buttonBackDown, 0, GumpButtonType.Page, relativePage - 1);
            parentGump.AddLabel(windowBounds.X + 40, footerY, s_fontColor, "<-Back");

            BuildNavigationPath(windowBounds, parentGump, String.Format("{0} (pg.{1})", navigationPath, relativePage));
        }

        public static void BuildNavigationPath(Rectangle2D windowBounds, Gump parentGump, string label)
        {
            int navigationX = windowBounds.X + 10;
            parentGump.AddLabel(navigationX, GetActionHeaderY(windowBounds), s_fontColor, label);
        }
        public static void BuildInitialReturnButton(Rectangle2D windowBounds, Gump parentGump)
        {
            int footerY = GetActionFooterY(windowBounds);
            // Previous page
            parentGump.AddButton(windowBounds.X + 5, footerY,
                s_buttonBackUp, s_buttonBackDown, (int)GumpActions.GA_Back, GumpButtonType.Reply, 0);
            parentGump.AddLabel(windowBounds.X + 40, footerY, s_fontColor, "<-Back");
        }
        private static void BuildPurchasePanel(Rectangle2D windowBounds, Gump parentGump, int price)
        {
            int y = GetActionHeader2Y(windowBounds);
            int offsetX = windowBounds.X + windowBounds.Width;

            parentGump.AddButton(offsetX - 35, y, 4011, 4013, (int)GumpActions.GA_Purchase, GumpButtonType.Reply, 0);
            parentGump.AddLabel(offsetX - 76, y, s_fontColor, "Buy->");
            parentGump.AddBackground(offsetX -120, y, 40, 20, 3000);
            parentGump.AddTextEntry(offsetX - 118, y, 35, 20, 0xFF, 0, "1");
            parentGump.AddLabel(offsetX - 157, y, s_fontColor, "Count:");
            parentGump.AddLabel(offsetX - 280, y, s_fontColor, "Cost/Count: " + price.ToString());
        }
        private static int GetActionFooterY(Rectangle2D windowBounds)
        {
           return windowBounds.Y + windowBounds.Height - 30;
        }
        private static int GetActionHeaderY(Rectangle2D windowBounds)
        {
            return windowBounds.Y + 7;
        }
        private static int GetActionHeader2Y(Rectangle2D windowBounds)
        {
            return windowBounds.Y + 35;
        }
    }
    public class PortalVendorInventoryBrowserGump : Gump
    {
        protected string m_category;
        protected string m_subcategory;
        protected int m_id;
        protected PortalsItemization.PortalInventory m_inventory;

        public PortalVendorInventoryBrowserGump(PortalsItemization.PortalInventory inventory, string category = "", string subcategory = "", int entryId = 0)
            : base(0,0)
        {
            m_inventory = inventory;
            m_category = category;
            m_subcategory = subcategory;
            m_id = entryId;
           
            AddPage(0);
            AddBackground(0, 0, 450, 570, 83);
            //AddBackground(0, 0, 700, 800, 83);

            string header = "Have a look around, purchases will come out of your bank box!";
            AddHtml(10, 10, 430, 70, header, true, false);
            //AddHtml(10, 10, 680, 70, header, true, false);

            Rectangle2D windowBounds = new Rectangle2D(10, 80, 430, 480);
            //Rectangle2D windowBounds = new Rectangle2D(10, 80, 680, 710);
            PortalInventoryGumpUtility.CreateBoundary(windowBounds, this);
            if (category == "")
                PortalInventoryGumpUtility.CreateCategoryPage(windowBounds, this, inventory);
            else if (subcategory == "")
                PortalInventoryGumpUtility.CreateSubcategoryPage(windowBounds, this, inventory, category);
            else if (entryId == 0)
                PortalInventoryGumpUtility.CreateSubcategoryEntriesPage(windowBounds, this, inventory, category, subcategory);
            else
                PortalInventoryGumpUtility.CreatePurchasingPage(windowBounds, this, inventory, m_category, m_subcategory, m_id);
        }
        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID > 0)
            {
                int buttonId = (info.ButtonID << 16) >> 16;
                int gid = info.ButtonID >> 16;
                switch (buttonId)
                {
                    case (int)PortalInventoryGumpUtility.GumpActions.GA_GotoCategory:
                        {
                            PortalContentEntry entry = PortalsItemization.InventoryLibrary.GetPortalContentEntry(gid);
                            sender.Mobile.SendGump(new PortalVendorInventoryBrowserGump(m_inventory, entry.m_category, "", 0));
                            break;
                        }
                    case (int)PortalInventoryGumpUtility.GumpActions.GA_GotoSubcategory:
                        {
                            PortalContentEntry entry = PortalsItemization.InventoryLibrary.GetPortalContentEntry(gid);
                            sender.Mobile.SendGump(new PortalVendorInventoryBrowserGump(m_inventory, entry.m_category, entry.m_subcategory, 0));
                            break;
                        }
                    case (int)PortalInventoryGumpUtility.GumpActions.GA_GotoEntry:
                        {
                            PortalContentEntry entry = PortalsItemization.InventoryLibrary.GetPortalContentEntry(gid);
                            sender.Mobile.SendGump(new PortalVendorInventoryBrowserGump(m_inventory, entry.m_category, entry.m_subcategory, gid));
                            break;
                        }
                    case (int)PortalInventoryGumpUtility.GumpActions.GA_Back:
                        {
                            sender.Mobile.SendGump(new PortalVendorInventoryBrowserGump(m_inventory, m_subcategory == "" ? "" : m_category, "", 0));
                            break;
                        }
                    case (int)PortalInventoryGumpUtility.GumpActions.GA_Purchase:
                        {
                            // Return to previous screen
                            sender.Mobile.SendGump(new PortalVendorInventoryBrowserGump(m_inventory, m_category, m_subcategory, m_id));

                            TextRelay textEntry = info.GetTextEntry(0);
                            string countStr = textEntry == null ? "" : textEntry.Text.Trim();
                            int count = 0;
                            if (!Int32.TryParse(countStr, out count))
                            {
                                string result = "An invalid quantity was entered.";
                                sender.Mobile.SendMessage(result);
                                break;
                            }
                            if (count <= 0)
                            {
                                string result = "The quantity must be at least 1.";
                                sender.Mobile.SendMessage(result);
                                break;
                            }

                            Container backpack = sender.Mobile.Backpack;
                            if (backpack == null || backpack.TotalItems + count > backpack.MaxItems)
                            {

                                string result = String.Format("Your backpack can not accomodate {0} items.", count);
                                sender.Mobile.SendMessage(result);
                                break;
                            }

                            PortalContentEntry entry = PortalsItemization.InventoryLibrary.GetPortalContentEntry(m_id);

                            int pricePer = entry.m_vendorPrice;
                            int totalPrice = pricePer * count;
                            var bank = sender.Mobile.BankBox;

                            if (sender.Mobile.AccessLevel >= AccessLevel.GameMaster ||
                                bank.ConsumeTotal(typeof(Gold), totalPrice))
                            {
                                Random r = new Random();
                                for (int i = 0; i < count; ++i)
                                {
                                    Item injector = new PortalInjector(entry);
                                    injector.X = r.Next(0, 175);
                                    injector.Y = r.Next(0, 175);
                                    backpack.AddItem(injector);
                                }
                                
                                string result = String.Format("{0} portal scroll{1} been placed into your backpack.",count, count > 1 ? "s have" : " has");
                                sender.Mobile.SendMessage(result);
                                sender.Mobile.PlaySound(0x32);

                                break;
                            }
                            else
                            {
                                string result = String.Format("You need {0} gold in your bank to buy that!", totalPrice);
                                sender.Mobile.SendMessage(result);
                                break;
                            }
                            break;
                        }
                }
            }
        }
    }
}
