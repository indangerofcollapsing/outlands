using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Net;
using System.Diagnostics;
using System.Linq;

using Server.Items;
using Server.Accounting;
using Server.Commands;
using Server.Network;
using Server.Mobiles;

namespace Server.PortalSystem
{
    public static class PortalsSystem
    {
        public static Map s_map = Map.Ilshenar;
        public static int s_minimumZ = 0;
        public static int s_maxBuildingHeight = 50;

        // Do not change
        public static readonly int s_partitionSize = 100;
        public static readonly int s_partitionColumnsMax = (int)(s_map.Width / s_partitionSize);
        public static readonly int s_partitionRowsMax = (int)(s_map.Height / s_partitionSize);
        public static readonly int s_partitionsMax = s_partitionColumnsMax * s_partitionRowsMax;

        // Causes any space not associated with a partition to be wiped on system initialization.
        private static bool m_strictVerify = false;
        private static bool m_confirmVerify = true;

        // Directories
        private static readonly string m_internalDirectory = "PortalSystem";
        private static readonly string m_saveDirectory = "Saves/PortalSystem";

        private static readonly string s_itemizationPath = m_internalDirectory + "/itemization.xml";
        private static readonly string s_configPath = m_internalDirectory + "/config.xml";
        private static readonly string m_savePath = m_saveDirectory + "/portals.dat";

        // Partitions
        public static Dictionary<int, PortalPartition> m_partitionIdToPartitionMap;
        private static Dictionary<string, PortalPartition> m_usernameToPartitionMap;
        private static Dictionary<string, PortalPartition> m_ipToPartitionMap;

        private static string s_config_root = "config";

        // Portal manager
        public static PortalAccessManager AccessManager;

        public static void Initialize()
        {
            RegisterCommands();
           
            EventSink.ServerStarted += new ServerStartedEventHandler(OnWorldLoad);
            EventSink.WorldSave += new WorldSaveEventHandler(OnWorldSave);
            EventSink.Login += new LoginEventHandler(OnLogin);

            m_partitionIdToPartitionMap = new Dictionary<int, PortalPartition>(s_partitionRowsMax);
            m_usernameToPartitionMap = new Dictionary<string, PortalPartition>(s_partitionRowsMax);
            m_ipToPartitionMap = new Dictionary<string, PortalPartition>();

            AccessManager = new PortalAccessManager();

            if (!File.Exists(s_configPath))
            {
                try
                {
                    PortalsSystem.WriteLine("Warning: \"{0}\" could not be found. A template is being created.", s_configPath);
                    // Generate template
                    XmlWriter writer = XmlWriter.Create(s_configPath);
                    writer.WriteStartElement(s_config_root);
                    writer.WriteEndElement();
                    writer.Flush();
                    writer.Close();
                }
                catch (Exception error)
                {
                    PortalsSystem.WriteError("Error. Configuration XML was not generated.", error);
                }
            }

            try
            {
                // General (xml)
                XmlDocument doc = new XmlDocument();
                doc.Load(s_configPath);

                XmlNode partitionsXmlNode = doc.SelectSingleNode(s_config_root);
                if (partitionsXmlNode == null)
                    throw new Exception(String.Format("Node \"{0}\" missing. Verify XML at {1}", s_config_root, s_configPath));

                //XmlAttributeCollection configXmlAttr = partitionsXmlNode.Attributes;
            }
            catch (Exception error)
            {
                PortalsSystem.WriteError("Configuration XML at \"{0}\" not correctly formatted.", error, s_configPath);
            }

            PortalsItemization.LoadItemizationFromXml(s_itemizationPath);

            DungeonPortalsRegion region = new DungeonPortalsRegion();
            region.Register();
        }

        public static PortalPartition GetPartitionById(int partitionId)
        {
            if (m_partitionIdToPartitionMap.ContainsKey(partitionId))
                return m_partitionIdToPartitionMap[partitionId];
            return null;
        }
        public static PortalPartition GetPartitionAtPoint(Point3D point)
        {
            int partitionCol = (int)(point.X / s_partitionSize);
            int partitionRow = (int)(point.Y / s_partitionSize);
            int partitionId = partitionCol + (partitionRow * s_partitionColumnsMax);

            if (m_partitionIdToPartitionMap.ContainsKey(partitionId))
                return m_partitionIdToPartitionMap[partitionId];
            return null;
        }
        public static PortalPartition GetPartitionByUsername(string username)
        {
            if (m_usernameToPartitionMap.ContainsKey(username))
                return m_usernameToPartitionMap[username];
            return null;
        }

        public static void WriteLine(string format, params object[] args)
        {
            Console.WriteLine(String.Concat("DMS: ", format), args);
        }
        public static void WriteError(string format, Exception error, params object[] args)
        {
            Utility.PushColor(ConsoleColor.Red);
            PortalsSystem.WriteLine(format, args);
            PortalsSystem.WriteLine("Error: " + error.Message);
            PortalsSystem.WriteLine("Stack: " + error.StackTrace);
            Utility.PopColor();
        }
        public static void VerifyPath(string directory, string fileName)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            string filePath = String.Format("{0}/{1}", directory, fileName);
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }
        }

        private static void RegisterCommands()
        {
            CommandSystem.Register("portals_admin", AccessLevel.GameMaster, new CommandEventHandler(CreatePortalsAdminGump));
            CommandSystem.Register("portals_all_items", AccessLevel.GameMaster, new CommandEventHandler(FillPortalInventoryCompletely));
        }

        public static void OnLogin(LoginEventArgs e)
        {
            Mobile m = e.Mobile;
            if (m.AccessLevel > AccessLevel.Player || m.Map != PortalsSystem.s_map)
                return;

            PortalPartition partition = PortalsSystem.GetPartitionAtPoint(m.Location);
            if (partition != null && partition.m_username == m.Account.Username)
                return;
            else
            {
                // Move is required
                if (m.Criminal)
                    m.MoveToWorld(new Point3D(2736, 2166, 0), Map.Felucca); // Bucs
                else
                    m.MoveToWorld(PortalPartition.s_defaultLastPortalPointLoc, PortalPartition.s_defaultLastPortalPointMap);  // Britain
                m.SendMessage("You were relocated because you logged out in a portal of which you are not the architect.");
            }

        }
        public static void ErasePartitions()
        {
            m_partitionIdToPartitionMap.Clear();
            m_usernameToPartitionMap.Clear();
            m_ipToPartitionMap.Clear();

            Rectangle2D mapArea = new Rectangle2D(0, 0, s_map.Width, s_map.Height);
            PurgeArea(mapArea);

            WriteLine("All partitions have been destroyed.");
        }

        private static void FillPortalInventoryCompletely(CommandEventArgs e)
        {
            PortalPartition partition = GetPartitionByUsername(e.Mobile.Account.Username);
            if (partition == null)
                return;

            PortalsItemization.PartitionPortalInventory inventory = partition.GetInventory();
            // add all entries

            foreach (KeyValuePair<int,PortalContentEntry> entry in PortalsItemization.InventoryLibrary.s_keyToEntryMap)
                inventory.Add(entry.Value);
        }
        private static void CreatePortalsAdminGump(CommandEventArgs e)
        {
            if (e.Mobile.HasGump(typeof(PortalsAdminGump)))
                e.Mobile.CloseGump(typeof(PortalsAdminGump));
            e.Mobile.SendGump(new PortalsAdminGump(e.Mobile));
        }

        private static void OnWorldLoad()
        {
            if (File.Exists(m_savePath))
            {
                WriteLine("Loading partitions at \"{0}\"", m_savePath);
                try
                {
                    FileStream fs = new FileStream(m_savePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    BinaryFileReader reader = new BinaryFileReader(new BinaryReader(fs));

                    int version = reader.ReadInt();
                    int nPartitions = reader.ReadInt();
                    for (int i = 0; i < nPartitions; ++i)
                    {
                        PortalPartition partition = new PortalPartition(reader);

                        Account acc = (Account)Accounts.GetAccount(partition.m_username);
                        if (acc == null)
                            continue;  // Account no longer exists.

                        m_partitionIdToPartitionMap.Add(partition.m_partitionId, partition);
                        m_ipToPartitionMap.Add(partition.m_ip, partition);
                        m_usernameToPartitionMap.Add(partition.m_username, partition);
                    }

                    reader.Close();
                }
                catch (Exception exc)
                {
                    WriteError("Failed to load partitions at \"{0}\"", exc, m_savePath);
                }
            }
            AccessManager.Load();
        }

        private static void OnWorldSave(WorldSaveEventArgs e)
        {
            if (!Directory.Exists(m_saveDirectory))
                Directory.CreateDirectory(m_saveDirectory);

            try
            {
                WriteLine("Saving {0} partitions to file {1}.", m_partitionIdToPartitionMap.Count, m_savePath);

                FileStream fs = new FileStream(m_savePath, FileMode.Create, FileAccess.Write, FileShare.None);
                GenericWriter writer = new BinaryFileWriter(fs, true);
                writer.Write(0); // Version
                writer.Write(m_partitionIdToPartitionMap.Count);

                foreach (KeyValuePair<int, PortalPartition> kvp in m_partitionIdToPartitionMap)
                {
                    PortalPartition partition = kvp.Value;

                    // Discard expired surveys.
                    partition.CheckSurveyExpiration();

                    partition.Serialize(writer);
                }

                writer.Close();
            }
            catch (Exception exc)
            {
                WriteError("Failed to save partition file.", exc);
            }

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(s_configPath);

                //XmlAttributeCollection attr = doc.DocumentElement.Attributes;

                XmlTextWriter xmlWriter = new XmlTextWriter(s_configPath, null);
                xmlWriter.Formatting = Formatting.Indented;
                xmlWriter.IndentChar = '\t';
                xmlWriter.Indentation = 1;
                doc.Save(xmlWriter);
                xmlWriter.Close();
            }
            catch (Exception exc)
            {
                WriteError("Failes to save config file.", exc);
            }
            
            AccessManager.OnWorldSave();
        }

        public static void PurgeArea(Rectangle2D rect)
        {
            IPooledEnumerable pool = PortalsSystem.s_map.GetObjectsInBounds(rect);
            List<IEntity> deleteList = new List<IEntity>();
            foreach (IEntity obj in pool)
            {
                if (obj is PlayerMobile)
                    continue;
                deleteList.Add(obj);
            }
            pool.Free();

            int len = deleteList.Count;
            for (int j = 0; j < len; j++)
                deleteList[j].Delete();
        }
        public static bool VerifyAvailability()
        {
            if (m_partitionIdToPartitionMap.Count <= s_partitionsMax)
                return true;
            return false;
        }

        public static bool VerifyEligibility(Mobile from)
        {
            string username = from.Account.Username;
            string ip = from.NetState.Address.ToString();

            // ip protection needs to be improved
            int lastDotIdx = ip.LastIndexOf(".");
            string parsedIp = ip.Substring(0, lastDotIdx + 1);
            Dictionary<string, PortalPartition>.Enumerator e = m_ipToPartitionMap.GetEnumerator();
            while (e.MoveNext())
            {
                string enumIp = e.Current.Key;
                if (e.Current.Key.Contains(parsedIp))
                    return false;
            }

            if (m_usernameToPartitionMap.ContainsKey(username) || m_ipToPartitionMap.ContainsKey(ip))
                return false;
            return true;
        }

        public static bool IsValidLocationForElement(string username, Point3D point)
        {
            PortalPartition partition = PortalsSystem.GetPartitionByUsername(username);
            if (partition == null || !partition.InteriorContainsPoint(point) || partition.OccupiedByPlatform(point) || point.Z < s_minimumZ || point.Z > s_minimumZ + s_maxBuildingHeight)
                return false;
            return true;
        }
        // Interface to begin partition creation (used on confirmed purchase)
        public static bool RequestPartition(Mobile from, DungeonControl controller)
        {
            // Criteria must be met before assigning a new partition.
            // 1. An account and ip can be registered to only 1 partition.
            // 2. A maximum of 88 partitions are available at any time.
            string username = from.Account.Username;
            string ip = from.NetState.Address.ToString();
            if (!VerifyEligibility(from))
            {
                from.SendMessage("You cannot control more than 1 dungeon.");
                return false;
            }
            if (!VerifyAvailability())
            {
                Console.WriteLine("Account {0} with ip {1} was denied a dungeon due to lack of space.", username, ip);
                from.SendMessage("All available dungeon space has been used; UOAC administrators have been notified.");
                return false;
            }

            return CreatePartition(ip, username, from, controller);
        }
        private static int GetAvailablePartitionId()
        {
            for (int i = 0; i < s_partitionsMax; ++i)
            {
                if (!m_partitionIdToPartitionMap.ContainsKey(i))
                    return i;
            }
            return -1;
        }
        private static bool CreatePartition(string ip, string username, Mobile from, DungeonControl controller)
        {
            // The partitionId is the same as the segmentId.
            int partitionId = GetAvailablePartitionId();
            PortalPartition partition = new PortalPartition(partitionId, ip, username);

            m_partitionIdToPartitionMap.Add(partition.m_partitionId, partition);
            m_ipToPartitionMap.Add(partition.m_ip, partition);
            m_usernameToPartitionMap.Add(partition.m_username, partition);

            controller.BindToAccount(username);

            // Bind the activating controller, and provide a controller for every other character.
            Account account = (Account)Accounts.GetAccount(username);
            foreach (Mobile m in account.accountMobiles)
            {
                if (m == null)
                    continue;

                if (m != from)
                {
                    DungeonControl control = new DungeonControl();
                    control.BindToAccount(username);
                    m.Backpack.AddItem(control);
                }
            }

            return true;
        }

        public static bool DestroyPartition(int partitionId)
        {
            if (!m_partitionIdToPartitionMap.ContainsKey(partitionId))
                return false;

            PortalPartition partition = m_partitionIdToPartitionMap[partitionId];
            PortalUtility.ReturnPlayersToPortalPoint(partition);
            partition.Clean(true);

            foreach (Serial s in partition.m_barrierSerialList)
            {
                IEntity entity = World.FindEntity(s);
                if (entity != null)
                    entity.Delete();
            }
            Account account = (Account)Accounts.GetAccount(partition.m_username);
            foreach (Mobile m in account.accountMobiles)
            {
                if (m == null)
                    continue;

                Item[] items = m.Backpack.FindItemsByType(typeof(DungeonControl));
                foreach (Item item in items)
                {
                    DungeonControl control = item as DungeonControl;
                    if (control.m_isActivated)
                        control.Delete();
                }
            }

            m_partitionIdToPartitionMap.Remove(partition.m_partitionId);
            m_ipToPartitionMap.Remove(partition.m_ip);
            m_usernameToPartitionMap.Remove(partition.m_username);

            AccessManager.SetPortalAccessibility(partition, false);

            return true;
        }
        public static Survey GetPartitionSurveyByAccount(short partitionId, string account)
        {
            if (m_partitionIdToPartitionMap.ContainsKey(partitionId))
            {
                PortalPartition partition = m_partitionIdToPartitionMap[partitionId];
                Survey survey = partition.GetSurveyByAccount(account);
                return survey != null ? survey : null;
            }
            return null;
        }
    }

    public class PortalPartition
    {
        public static Point3D s_defaultLastPortalPointLoc = new Point3D(1495, 1629, 10);
        public static Map s_defaultLastPortalPointMap = Map.Felucca;

        public short m_partitionId;
        public string m_ip;
        public string m_username;

        public Point3D m_position;

        public bool m_isAccessible;
        public string Description { get; set; }

        // owner world -> portal
        public Point3D m_lastTeleportFromLocation;
        public Map m_lastTeleportFromMap;
        // portal -> portal point
        public Point3D m_lastPortalPointLoc;
        public Map m_lastPortalPointMap;
        // portal point -> portal
        public Point3D m_entryPoint;

        public List<Serial> m_barrierSerialList;

        private Rectangle2D m_platformBounds;

        public PortalsItemization.PartitionPortalInventory GetInventory() { return m_inventory; }
        public PortalsItemization.PartitionPortalInventory m_inventory;

        // Manager for undo/redo actions
        public ControlActionManager ControlActionManager { get; private set; }

        private PortalRecycleBin m_bin;

        public List<Survey> m_surveyList;

        public int m_options;
        public enum EPlacementOptions
        {
            ePO_DynamicHeight = 0x01,
        }

        public PortalPartition(int partitionId, string ip, string username)
        {
            CommonInitialization();

            m_lastPortalPointLoc = s_defaultLastPortalPointLoc;
            m_lastPortalPointMap = s_defaultLastPortalPointMap;

            m_lastTeleportFromLocation = s_defaultLastPortalPointLoc;
            m_lastTeleportFromMap = s_defaultLastPortalPointMap;

            m_partitionId = (short)partitionId;
            m_ip = ip;
            m_username = username;
            Description = String.Empty;

            int x = (m_partitionId % PortalsSystem.s_partitionColumnsMax) * PortalsSystem.s_partitionSize;
            int y = ((int)(m_partitionId / PortalsSystem.s_partitionColumnsMax)) * PortalsSystem.s_partitionSize;
            m_position = new Point3D(x, y, PortalsSystem.s_minimumZ);

            CreatePlatform();
            CreateBarrier();
            CreateBin();
        }

        public PortalPartition(BinaryFileReader reader)
        {
            CommonInitialization();

            int version = reader.ReadInt();

            m_partitionId = reader.ReadShort();
            m_ip = reader.ReadString();
            m_username = reader.ReadString();
            Description = reader.ReadString();
            m_isAccessible = reader.ReadBool();

            m_position = reader.ReadPoint3D();

            int barrierCount = reader.ReadInt();
            for (int i = 0; i < barrierCount; ++i)
                m_barrierSerialList.Add(reader.ReadInt());

            int nSurveys = reader.ReadInt();
            for (int i = 0; i < nSurveys; ++i)
            {
                Survey survey = new Survey(reader);
                m_surveyList.Add(survey);
            }

            m_entryPoint = reader.ReadPoint3D();
            m_platformBounds = reader.ReadRect2D();

            if (version >= 1)
            {
                m_lastPortalPointLoc = reader.ReadPoint3D();
                m_lastPortalPointMap = reader.ReadMap();

                m_lastTeleportFromLocation = reader.ReadPoint3D();
                m_lastTeleportFromMap = reader.ReadMap();
            }

            m_bin = (PortalRecycleBin)World.FindItem(reader.ReadInt());

            m_inventory.Deserialize(reader);
        }

        private void CommonInitialization()
        {
            m_barrierSerialList = new List<Serial>();

            m_inventory = new PortalsItemization.PartitionPortalInventory();
            ControlActionManager = new ControlActionManager(this);
            m_surveyList = new List<Survey>();
        }

        private void CreatePlatform()
        {
            const int tlcorner = 0x4367;
            const int top = 0x435F;
            const int trcorner = 0x436A;
            const int right = 0x435E;
            const int brcorner = 0x4368;
            const int bottom = 0x4360;
            const int blcorner = 0x4369;
            const int left = 0x4361;
            const int fill = 0x4317;
            const int platformA = 0x4328;
            const int platformB = 0x4329;
            const int platformC = 0x432A;
            const int platformD = 0x432B;

            int zloc = PortalsSystem.s_minimumZ+1;

            Point2D platformStart = new Point2D(m_position.X + 4, m_position.Y + 4);
            m_entryPoint = new Point3D(platformStart.X + 3, platformStart.Y + 3, zloc+5);

            StaticHelper[,] boundary = new StaticHelper[,] {
                        {new StaticHelper(0,0,tlcorner,zloc), new StaticHelper(1,0,top,zloc), new StaticHelper(2,0,top,zloc), new StaticHelper(3,0,top,zloc), new StaticHelper(4,0,top,zloc), new StaticHelper(5,0,trcorner,zloc)},
                        {new StaticHelper(0,1,left,zloc), new StaticHelper(1,1,fill,zloc), new StaticHelper(2,1,fill,zloc), new StaticHelper(3,1,fill,zloc), new StaticHelper(4,1,fill,zloc), new StaticHelper(5,1,right,zloc)},
                        {new StaticHelper(0,2,left,zloc), new StaticHelper(1,2,fill,zloc), new StaticHelper(2,2,platformA, zloc+5), new StaticHelper(3,2,platformB, zloc+5), new StaticHelper(4,2,fill,zloc), new StaticHelper(5,2,right,zloc)},
                        {new StaticHelper(0,3,left,zloc), new StaticHelper(1,3,fill,zloc), new StaticHelper(2,3,platformC, zloc+5), new StaticHelper(3,3,platformD, zloc+5), new StaticHelper(4,3,fill,zloc), new StaticHelper(5,3,right,zloc)},
                        {new StaticHelper(0,4,left,zloc), new StaticHelper(1,4,fill,zloc), new StaticHelper(2,4,fill,zloc), new StaticHelper(3,4,fill,zloc), new StaticHelper(4,4,fill,zloc), new StaticHelper(5,4,right,zloc)},
                        {new StaticHelper(0,5,blcorner,zloc), new StaticHelper(1,5,bottom,zloc), new StaticHelper(2,5,bottom,zloc), new StaticHelper(3,5,bottom,zloc), new StaticHelper(4,5,bottom,zloc), new StaticHelper(5,5,brcorner,zloc)}
                    };

            int minX = Int32.MaxValue;
            int maxX = Int32.MinValue;
            int minY = Int32.MaxValue;
            int maxY = Int32.MinValue;

            Map map = PortalsSystem.s_map;
            for (int x = 0; x < 6; x++)
            {
                for (int y = 0; y < 6; y++)
                {
                    StaticHelper platformHelper = boundary[x, y];
                    DungeonPlatform platformStatic = new DungeonPlatform(platformHelper.m_id);
                    Point3D p = new Point3D(platformStart.X + platformHelper.m_x, platformStart.Y + platformHelper.m_y, platformHelper.m_z);
                    platformStatic.MoveToWorld(p, map);

                    int resultX = x + platformStart.X;
                    int resultY = y + platformStart.Y;

                    if (resultX < minX)
                        minX = resultX;
                    if (resultX > maxX)
                        maxX = resultX;
                    if (resultY < minY)
                        minY = resultY;
                    if (y > maxY)
                        maxY = resultY;
                }
            }

            Point2D platformMin = new Point2D(minX, minY);
            Point2D platformMax = new Point2D(maxX + 1, maxY + 1);
            m_platformBounds = new Rectangle2D(platformMin, platformMax);
        }

        private void CreateBin()
        {
            m_bin = new PortalRecycleBin();
            // Place the reclaim container on the side of the platform.
            Point3D p = this.m_entryPoint;
            p.Y -= 2;
            m_bin.MoveToWorld(p, PortalsSystem.s_map);
        }

        private void CreateBarrier()
        {
            // Boundary graphics
            const int tlcorner = 0x512B;
            const int topAndBottom = 0x5117;
            const int leftAndRight = 0x510F;
            const int brcorner = 0x512A;

            List<PortalBarrier> barrierList = new List<PortalBarrier>();

            // tl
            PortalBarrier barrier = new PortalBarrier(tlcorner);
            barrier.MoveToWorld(new Point3D(m_position.X, m_position.Y, m_position.Z), PortalsSystem.s_map);
            barrierList.Add(barrier);

            // br
            barrier = new PortalBarrier(brcorner);
            barrier.MoveToWorld(new Point3D(m_position.X + PortalsSystem.s_partitionSize, m_position.Y + PortalsSystem.s_partitionSize, m_position.Z), PortalsSystem.s_map);
            barrierList.Add(barrier);

            // tr
            barrier = new PortalBarrier(topAndBottom);
            barrier.MoveToWorld(new Point3D(m_position.X + PortalsSystem.s_partitionSize, m_position.Y, m_position.Z), PortalsSystem.s_map);
            barrierList.Add(barrier);

            // bl
            barrier = new PortalBarrier(leftAndRight);
            barrier.MoveToWorld(new Point3D(m_position.X, m_position.Y + PortalsSystem.s_partitionSize, m_position.Z), PortalsSystem.s_map);
            barrierList.Add(barrier);

            for (int i = 1; i < PortalsSystem.s_partitionSize; ++i)
            {
                // top
                barrier = new PortalBarrier(topAndBottom);
                barrier.MoveToWorld(new Point3D(m_position.X + i, m_position.Y, m_position.Z), PortalsSystem.s_map);
                barrierList.Add(barrier);

                // bottom
                barrier = new PortalBarrier(topAndBottom);
                barrier.MoveToWorld(new Point3D(m_position.X + i, m_position.Y + PortalsSystem.s_partitionSize, m_position.Z), PortalsSystem.s_map);
                barrierList.Add(barrier);

                // left
                barrier = new PortalBarrier(leftAndRight);
                barrier.MoveToWorld(new Point3D(m_position.X, m_position.Y + i, m_position.Z), PortalsSystem.s_map);
                barrierList.Add(barrier);

                // right
                barrier = new PortalBarrier(leftAndRight);
                barrier.MoveToWorld(new Point3D(m_position.X + PortalsSystem.s_partitionSize, m_position.Y + i, m_position.Z), PortalsSystem.s_map);
                barrierList.Add(barrier);
                
            }

            int barrierListLen = barrierList.Count;
            for (int i = 0; i < barrierListLen; ++i)
                m_barrierSerialList.Add(barrierList[i].Serial);
        }


        public bool IsUsingRelativeHeight()
        {
            return (m_options & (1 << (int)EPlacementOptions.ePO_DynamicHeight)) > 0;
        }
        public void ToggleDynamicHeight()
        {
            m_options ^= (1 << (int)EPlacementOptions.ePO_DynamicHeight);
        }

        public void Clean(bool purge, bool returnToInventory = false)
        {
            if (purge)
                returnToInventory = false;

            List<Item> itemList = new List<Item>();
            List<IEntity> removeList = new List<IEntity>();

            Rectangle2D rect = purge ? GetArea() : GetInterior();
            IPooledEnumerable pool = PortalsSystem.s_map.GetObjectsInBounds(rect);
            foreach (IEntity obj in pool)
            {
                if (obj is PortalBarrier)
                    continue;
                if (obj is PortalStatic || obj is PortalDoor)
                {
                    removeList.Add(obj);
                    if (returnToInventory)
                    {
                        IPortalElement element = obj as IPortalElement;
                        m_inventory.Add(element.GetGid(), element.GetEntryKey(), 1);
                    }
                }
                else if (obj is PortalRecycleBin || obj is DungeonPlatform)
                {
                    if (purge)
                        removeList.Add(obj);
                }
                else if (obj is Item)
                {
                    if (returnToInventory)
                        itemList.Add(obj as Item); // to the bin
                    else
                        removeList.Add(obj);
                }
                else if (obj is BaseCreature || (obj is Corpse) || (obj is Mobile && !(obj is PlayerMobile)))
                    removeList.Add(obj);
            }
            pool.Free();

            if (itemList.Count > 0)
            {
                foreach (Item item in itemList)
                    m_bin.AddItem(item);
            }

            int removeLen = removeList.Count;
            for (int i = 0; i < removeLen; ++i)
                removeList[i].Delete();
        }

        public Point3D GetEntryPosition()
        {
            return m_entryPoint;
        }
        public bool OccupiedByPlatform(Point3D p)
        {
            return m_platformBounds.Contains(p);
        }
        public bool InteriorContainsPoint(Point3D p)
        {
            if (p.Z > PortalsSystem.s_maxBuildingHeight || p.Z < 0)
                return false;

            Rectangle2D interior = GetInterior();
            return interior.Contains(p);
        }
        public Rectangle2D GetArea()
        {
            return new Rectangle2D(m_position.X, m_position.Y, PortalsSystem.s_partitionSize, PortalsSystem.s_partitionSize);
        }
        public Rectangle2D GetInterior()
        {
            return new Rectangle2D(m_position.X + 1, m_position.Y + 1, PortalsSystem.s_partitionSize - 2, PortalsSystem.s_partitionSize - 2);
        }
        public List<NetState> GetClientsInBounds()
        {
            List<NetState> clientList = new List<NetState>();

            Rectangle2D interior = GetInterior();
            IPooledEnumerable enumerable = PortalsSystem.s_map.GetClientsInBounds(interior);
            foreach (NetState ns in enumerable)
                clientList.Add(ns);

            enumerable.Free();
            return clientList;
        }

        #region Portals
        public bool IsAccessible()
        {
            return m_isAccessible;
        }
        public bool ToggleAccessibility()
        {
            m_isAccessible = !m_isAccessible;
            PortalsSystem.AccessManager.SetPortalAccessibility(this, m_isAccessible);
            return m_isAccessible;
        }
        #endregion

        #region Surveys
        public void CheckSurveyExpiration()
        {
            for (int i = m_surveyList.Count-1; i >= 0; --i)
            {
                if (m_surveyList[i].IsExpired())
                    m_surveyList.RemoveAt(i);
            }
        }
        /// <summary>
        public void AddSurvey(Survey survey)
        {
            bool found = false;
            foreach (Survey s in m_surveyList)
            {
                if (s.m_account == survey.m_account)
                {
                    found = true;
                    // Replace the existing survey.
                    s.m_submitter = survey.m_submitter;
                    s.m_rating = survey.m_rating;
                    s.m_comment = survey.m_comment;
                    break;
                }
            }

            if (!found)
            {
                m_surveyList.Add(survey);
            }
        }
        
        public Survey GetSurveyByAccount(string account)
        {
            foreach (Survey survey in m_surveyList)
            {
                if (survey.m_account == account)
                {
                    return survey;
                }
            }
            return null;
        }
        public List<Survey> GetSurveys()
        {
            return m_surveyList;
        }
        public int GetRatingCount()
        {
            return m_surveyList.Count;
        }
        public float GetAverageRating()
        {
            if (m_surveyList.Count == 0)
            {
                // Average rating by default.
                return 3;
            }

            float rates = 0;
            foreach (Survey survey in m_surveyList)
            {
                rates += survey.m_rating;
            }
            rates /= m_surveyList.Count;
            return rates;
        }
        #endregion

        public void Serialize(GenericWriter writer)
        {
            // Version
            writer.Write(1);

            writer.Write(m_partitionId);
            writer.Write(m_ip);
            writer.Write(m_username);
            writer.Write(Description);
            writer.Write(m_isAccessible);

            writer.Write(m_position);

            int barrierCount = m_barrierSerialList.Count;
            writer.Write(barrierCount);
            for (int i = 0; i < barrierCount; ++i)
                writer.Write(m_barrierSerialList[i]);

            writer.Write(m_surveyList.Count);
            foreach (Survey survey in m_surveyList)
            {
                survey.Serialize(writer);
            }

            writer.Write(m_entryPoint);
            writer.Write(m_platformBounds);

            // Version 1
            writer.Write(m_lastPortalPointLoc);
            writer.Write(m_lastPortalPointMap);

            writer.Write(m_lastTeleportFromLocation);
            writer.Write(m_lastTeleportFromMap);

            writer.Write(m_bin.Serial);

            m_inventory.Serialize(writer);
        }
    }

    public struct StaticHelper
    {
        public int m_x, m_y, m_id, m_z;
        public StaticHelper(int x, int y, int id, int z)
        {
            m_x = x;
            m_y = y;
            m_id = id;
            m_z = z;
        }
    }

    /// <summary>
    /// This is a barrier that encloses a dungeon.
    /// </summary>
    /// 
    public class PortalBarrier : Static
    {
        private string m_name = "A portal barrier";
        public PortalBarrier(int gid)
            : base(gid) { }
        public PortalBarrier(Serial serial)
            : base(serial)
        {
        }
        public override string DefaultName
        {
            get
            {
                return m_name;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
        }
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
        }
    }
    /// <summary>
    /// This is the portal platform that accompanies every dungeon.
    /// </summary>
    public class DungeonPlatform : Static
    {
        private string m_name = "A portal platform";
        public DungeonPlatform(int gid)
            : base(gid) { }
        public DungeonPlatform(Serial serial)
            : base(serial)
        {
        }
        public override string DefaultName
        {
            get
            {
                return m_name;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
        }
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
        }
    }
   
    public class Survey
    {
        public string m_account;
        public string m_submitter;
        public byte m_rating;
        public string m_comment;
        public DateTime m_submitTime;

        public Survey(string account, string submitter, byte rating, string comment)
        {
            m_account = account;
            m_submitter = submitter;
            m_rating = rating;
            m_comment = comment;
            m_submitTime = DateTime.UtcNow;
        }

        public Survey(BinaryFileReader reader)
        {
            int version = reader.ReadInt();

            m_account = reader.ReadString();
            m_submitter = reader.ReadString();
            m_rating = reader.ReadByte();
            m_comment = reader.ReadString();
            m_submitTime = reader.ReadDateTime();
        }
        public void Serialize(GenericWriter writer)
        {
            writer.Write(0); // Version

            writer.Write(m_account);
            writer.Write(m_submitter);
            writer.Write(m_rating);
            writer.Write(m_comment);
            writer.Write(m_submitTime);
        }

        public bool IsExpired()
        {
            TimeSpan time = DateTime.UtcNow.Subtract(m_submitTime);
            return time.TotalDays > 10;
        }
    };

}
