using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;
using System.IO;

using Server.Items;
using Server.Targeting;
using Server.Mobiles;
using Server.Gumps;
using Server.Network;
using Server.Commands;
using Server.Spells;

namespace Server.PortalSystem
{
    public class PortalAccessManager
    {
        public enum ERegion
        {
            eD_Covetous,
            eD_Deceit,
            eD_Shame,
            eD_Despise,
            eD_Destard,
            eD_Hythloth,
            eD_Wrong,
            eD_Fire,
            eD_Ice,
        }
        public class RegionDetails
        {
            public string m_region;
            public int m_difficulty;
            public int m_portalBase;
            public int m_maxPortals;

            public RegionDetails(string region, int difficulty, int portalBase, int maxPortals)
            {
                m_region = region;
                m_difficulty = difficulty;
                m_portalBase = portalBase;
                m_maxPortals = maxPortals;
            }
        }
        public class RegionStatistic
        {
            public string m_region;
            public float m_sessionKills;
            public float m_sessionPoints;
            public int m_totalKills;
            public int m_totalPoints;
            public int m_currentChance;

            public RegionStatistic(string region, int totalKills, int totalPoints)
            {
                m_region = region;
                m_sessionKills = 0;
                m_sessionPoints = 0;
                m_totalKills = totalKills;
                m_totalPoints = totalPoints;
                m_currentChance = 0;
            }
        }

        private static readonly string s_internalDirectory = "PortalSystem";
        private static readonly string s_configPath = s_internalDirectory + "/portalAccessManager.xml";

        // See TentDeed.cs for region listing.
        public string[] m_regions = {"Covetous", "Deceit", "Shame",
                                 "Despise", "Destard", "Hythloth",
                                 "Wrong", "Fire", "Ice"};

        // An enumeration is just an optimization for the look up times.
        public Dictionary<string, ERegion> m_regionMap;
        public Dictionary<ERegion, RegionStatistic> m_regionStatisticsMap;
        public Dictionary<ERegion, RegionDetails> m_regionDetailsMap;
        // <creatureType, weight>
        private Dictionary<string, byte> m_creatureValueMap;
        public List<List<PortalPoint>> m_portalPointsLists;

        public int m_portalFrequency;
        private int m_totalSessions;
        public float m_difficultyFactor;

        public List<PortalPartition> m_eligiblePortals;

        // DungeonPortalsRegion
        public List<int> m_activatedPortals;
        public bool IsPortalActive(int partitionId){return m_activatedPortals.Contains(partitionId);}

        // Controls the portal generation.
        public bool m_enabled;

        public PortalAccessManager()
        {
            int nRegions = m_regions.Length;
            m_regionStatisticsMap = new Dictionary<ERegion, RegionStatistic>(nRegions);
            m_regionDetailsMap = new Dictionary<ERegion, RegionDetails>(nRegions);
            m_regionMap = new Dictionary<string, ERegion>(nRegions);
            m_creatureValueMap = new Dictionary<string, byte>();
            m_eligiblePortals = new List<PortalPartition>();

            // Setup the foundation for region lookups.
            int enumIdx = 0;
            foreach (string regionName in m_regions)
            {
                ERegion region = (ERegion)enumIdx;
                m_regionMap.Add(regionName, region);
                ++enumIdx;
            }

            m_portalPointsLists = new List<List<PortalPoint>>(nRegions);
            for (int i = 0; i < nRegions; ++i)
            {
                m_portalPointsLists.Add(new List<PortalPoint>());
            }

            // This is slow, but it's a one time operation on server init.
            foreach (Map map in Map.AllMaps)
            {
                Rectangle2D bounds = new Rectangle2D(0, 0, map.Width, map.Height);
                IPooledEnumerable pool = map.GetItemsInBounds(bounds);

                List<PortalPoint> points = new List<PortalPoint>();

                foreach (IEntity entity in pool)
                {
                    if (entity is PortalPoint)
                    {
                        points.Add(entity as PortalPoint);
                    }
                }
                pool.Free();

                foreach (PortalPoint portalPoint in points)
                {
                    AddPortalPoint(portalPoint);
                }
            }

            m_activatedPortals = new List<int>();
        }
        private static void WriteLine(string format, params object[] args)
        {
			bool do_write = false;
			if( do_write )
				Console.WriteLine(String.Concat("DMS: ", format), args);
        }
        public static void Initialize()
        {
        }
        public bool OnWorldSave()
        {
            // Incremental save to XML. File needs to exist.
            if (File.Exists(s_configPath))
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(s_configPath);

                    XmlElement newConfigXmlNode = doc.CreateElement("config");
                    newConfigXmlNode.SetAttribute("frequency", m_portalFrequency.ToString());
                    newConfigXmlNode.SetAttribute("enabled", m_enabled.ToString());

                    XmlNode configParentXmlNode = doc.SelectSingleNode("portalAccessManager");
                    XmlNode oldConfigXmlNode = doc.SelectSingleNode("portalAccessManager/config");
                    configParentXmlNode.ReplaceChild(newConfigXmlNode, oldConfigXmlNode);

                    XmlElement newStatisticsXmlNode = doc.CreateElement("statistics");
                    newStatisticsXmlNode.SetAttribute("totalSessions", m_totalSessions.ToString());

                    foreach (KeyValuePair<ERegion, RegionStatistic> kvp in m_regionStatisticsMap)
                    {
                        RegionStatistic statistic = kvp.Value;
                        XmlElement statisticXmlNode = doc.CreateElement("statistic");
                        statisticXmlNode.SetAttribute("region", statistic.m_region);
                        statisticXmlNode.SetAttribute("totalKills", statistic.m_totalKills.ToString());
                        statisticXmlNode.SetAttribute("totalPoints", statistic.m_totalPoints.ToString());

                        newStatisticsXmlNode.AppendChild(statisticXmlNode);
                    }

                    XmlNode statisticsParentXmlNode = doc.SelectSingleNode("portalAccessManager");
                    XmlNode oldStatisticsXmlNode = doc.SelectSingleNode("portalAccessManager/statistics");
                    statisticsParentXmlNode.ReplaceChild(newStatisticsXmlNode, oldStatisticsXmlNode);

                    XmlElement newRegionsXmlNode = doc.CreateElement("regions");
                    newRegionsXmlNode.SetAttribute("difficultyFactor", m_difficultyFactor.ToString());

                    foreach (KeyValuePair<ERegion, RegionDetails> kvp in m_regionDetailsMap)
                    {
                        RegionDetails details = kvp.Value;
                        XmlElement regionXmlNode = doc.CreateElement("region");
                        regionXmlNode.SetAttribute("name", details.m_region);
                        regionXmlNode.SetAttribute("difficulty", details.m_difficulty.ToString());
                        regionXmlNode.SetAttribute("portalBase", details.m_portalBase.ToString());
                        regionXmlNode.SetAttribute("maxPortals", details.m_maxPortals.ToString());
                        newRegionsXmlNode.AppendChild(regionXmlNode);
                    }

                    XmlNode regionsParentXmlNode = doc.SelectSingleNode("portalAccessManager");
                    XmlNode oldRegionsXmlNode = doc.SelectSingleNode("portalAccessManager/regions");
                    regionsParentXmlNode.ReplaceChild(newRegionsXmlNode, oldRegionsXmlNode);

                    XmlTextWriter writer = new XmlTextWriter(s_configPath, null);
                    writer.Formatting = Formatting.Indented;
                    writer.IndentChar = '\t';
                    writer.Indentation = 1;
                    doc.Save(writer);
                    writer.Close();
                }
                catch (Exception exc)
                {
                    PortalsSystem.WriteError("PortalAccessManager save failed!", exc);
                }
            }
            else
            {
                PortalsSystem.WriteLine("Failed to perform incremental save of {0} for PortalAccessManager because it could not be found.", s_configPath);
            }
            
            return true;
        }
        public bool Load()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(s_configPath);

                XmlNode configXmlNode = doc.SelectSingleNode("portalAccessManager/config");
                XmlAttributeCollection configAttr = configXmlNode.Attributes;
                XmlNode frequencyAttrXmlNode = configAttr.GetNamedItem("frequency");
                m_portalFrequency = Convert.ToInt32(frequencyAttrXmlNode.Value);
                XmlNode enabledAttrXmlNode = configAttr.GetNamedItem("enabled");
                m_enabled = Convert.ToBoolean(enabledAttrXmlNode.Value);

                XmlNode statisticsXmlNode = doc.SelectSingleNode("portalAccessManager/statistics");
                XmlAttributeCollection statisticsAttr = statisticsXmlNode.Attributes;
                XmlNode totalSessionsAttrXmlNode = statisticsAttr.GetNamedItem("totalSessions");
                m_totalSessions = Convert.ToInt32(totalSessionsAttrXmlNode.Value);

                XmlNodeList statisticsXmlList = doc.GetElementsByTagName("statistic");
                foreach (XmlNode statisticXmlNode in statisticsXmlList)
                {
                    XmlAttributeCollection statisticAttr = statisticXmlNode.Attributes;
                    XmlNode regionAttrXmlNode = statisticAttr.GetNamedItem("region");
                    string region = regionAttrXmlNode.Value;
                    XmlNode tpAttrXmlNode = statisticAttr.GetNamedItem("totalPoints");
                    int totalPoints = Convert.ToInt32(tpAttrXmlNode.Value);
                    XmlNode tkAttrXmlNode = statisticAttr.GetNamedItem("totalKills");
                    int totalKills = Convert.ToInt32(tkAttrXmlNode.Value);

                    ERegion eRegion = m_regionMap[region];
                    m_regionStatisticsMap.Add(eRegion, new RegionStatistic(region, totalKills, totalPoints));
                }

                XmlNode regionsXmlNode = doc.SelectSingleNode("portalAccessManager/regions");
                XmlAttributeCollection regionsAttr = regionsXmlNode.Attributes;
                XmlNode diffFactorAttrXmlNode = regionsAttr.GetNamedItem("difficultyFactor");
                m_difficultyFactor = (float)Convert.ToDecimal(diffFactorAttrXmlNode.Value);

                XmlNodeList regionXmlList = doc.GetElementsByTagName("region");
                foreach (XmlNode regionXmlNode in regionXmlList)
                {
                    XmlAttributeCollection regionAttr = regionXmlNode.Attributes;
                    XmlNode nameAttrXmlNode = regionAttr.GetNamedItem("name");
                    string name = nameAttrXmlNode.Value;
                    XmlNode diffAttrXmlNode = regionAttr.GetNamedItem("difficulty");
                    int difficulty = Convert.ToInt32(diffAttrXmlNode.Value);
                    XmlNode portalBaseAttrXmlNode = regionAttr.GetNamedItem("portalBase");
                    int portalBase = Convert.ToInt32(portalBaseAttrXmlNode.Value);
                    XmlNode maxPortalsAttrXmlNode = regionAttr.GetNamedItem("maxPortals");
                    int maxPortals = Convert.ToInt32(maxPortalsAttrXmlNode.Value);

                    ERegion eRegion = m_regionMap[name];
                    m_regionDetailsMap.Add(eRegion, new RegionDetails(name, difficulty, portalBase, maxPortals));
                }


                XmlNode weightsXmlNode = doc.SelectSingleNode("portalAccessManager/weights");
                XmlNodeList weightsXmlList = doc.GetElementsByTagName("weight");
                foreach (XmlNode weightXmlNode in weightsXmlList)
                {
                    XmlAttributeCollection weightAttr = weightXmlNode.Attributes;
                    XmlNode weightAttrXmlNode = weightAttr.GetNamedItem("val");
                    int weight = Convert.ToInt32(weightAttrXmlNode.Value);

                    XmlNodeList creatureList = weightXmlNode.SelectNodes("creature");
                    foreach (XmlNode creatureXmlNode in creatureList)
                    {
                        XmlAttributeCollection creatureAttr = creatureXmlNode.Attributes;
                        XmlNode nameAttrXmlNode = creatureAttr.GetNamedItem("name");
                        string name = nameAttrXmlNode.Value;

                        if (!m_creatureValueMap.ContainsKey(name))
                            m_creatureValueMap.Add(name, (byte)weight);
                        else
                            PortalsSystem.WriteLine("{0} is a duplicate entry. Can't add as weight {1}.", name, weight);
                    }
                }
            }
            catch (Exception exc)
            {
                PortalsSystem.WriteError("Failed to load AccessControlManager from \"{0}\"", exc, s_configPath);
            }

            Startup();

            return true;
        }

        private void Startup()
        {
            if (m_portalFrequency <= 0)
                return;

            CreatePortalTimer timer = new CreatePortalTimer(this);
            timer.Start();

            // Add any accessible partitions to the portal queue.
            foreach (KeyValuePair<int, PortalPartition> kvp in PortalsSystem.m_partitionIdToPartitionMap)
            {
                PortalPartition partition = kvp.Value;
                if (partition.m_isAccessible)
                    m_eligiblePortals.Add(partition);
            }
        }
        public void SetPortalAccessibility(PortalPartition partition, bool isAccessible)
        {
            bool found = m_eligiblePortals.Contains(partition);
            if (isAccessible)
            {
                if (!found)
                    m_eligiblePortals.Insert(0, partition);
                else
                    Console.WriteLine("DPM: Warning! Dungeon {0} tried to enter queue, but was already in it.", partition.m_partitionId);
            }
            else
            {
                if (found)
                    m_eligiblePortals.Remove(partition);
                else
                    Console.WriteLine("DPM: Warning! Dungeon {0} tried to exit queue, but wasn't in it.", partition.m_partitionId);
            }
        }
        public void Shuffle<T>(IList<T> list)
        {
            Random rnd = new Random();
            int n = list.Count;
            while (n > 1)
            {
                int k = (rnd.Next(0, n) % n);
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        public void EvaluateSession()
        {
            WriteLine("EvaluateSession called.");

            if (m_eligiblePortals.Count < 1)
            {
                WriteLine("No eligible portals to be evaluated this session.");
                return;
            }

            string directory = s_internalDirectory + "/PortalAccessManagerLogs";
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            string date = DateTime.Today.ToString("d");
            date = date.Replace("/", "_");
            string file = directory + "\\portalLog_" + date + ".txt";

            StreamWriter writer = null;
            if (!File.Exists(file))
            {
                writer = File.CreateText(file);
            }
            else
            {
                writer = new StreamWriter(file, true);
            }

            writer.WriteLine("Portal report for {0}", DateTime.Now.ToString());
            writer.WriteLine();

            m_activatedPortals.Clear();

            // The target portals are ordered based on a pseudo-random score generated from their rating.
            Random r = new Random(DateTime.UtcNow.Millisecond);

            List<DungeonScore> scoreList = new List<DungeonScore>();
            int len = m_eligiblePortals.Count;
            WriteLine("{0} portals are marked as accessible.", len);
            writer.WriteLine("{0} portals are marked as accessible.", len);
            for (int i = 0; i < len; ++i)
            {
                PortalPartition partition = m_eligiblePortals[i];
                float averageRating = partition.GetAverageRating();

                // Every 1.0 of avg rating yields 10 points.
                float guaranteedPoints = averageRating * 10;
                // A random number of points [0,20] are added to the guaranteed yield. This means that
                // a portal with a 1.0 rating could get a score as high as 30 (10 from base, and 20 from random).
                // The same portal would never score higher than a 3.0 dungeon, but may beat out a 2.0 portal.
                // By the same logic, a 5.0 portal can never be out-scored by a portal 3.0 or lower.
                float randomPoints = r.Next(20) + guaranteedPoints;

                scoreList.Add(new DungeonScore(partition.m_partitionId, randomPoints));

                WriteLine("{0}'s portal (id {1}) has a avg. rating of {2}. Score awarded is {3}.",
                    partition.m_username, partition.m_partitionId, averageRating, randomPoints);
                writer.WriteLine("{0}'s portal (id {1}) has a avg. rating of {2}. Score awarded is {3}.",
                    partition.m_username, partition.m_partitionId, averageRating, randomPoints);
            }

            WriteLine("Portals sorted by score:");
            writer.WriteLine("Portals sorted by score:");

            // Sort the list, then convert the enumeration back into list form, so it can be modified during iteration.
            IOrderedEnumerable<DungeonScore> orderedScores = scoreList.OrderByDescending(x => x.m_score);
            List<DungeonScore> sortedList = orderedScores.ToList<DungeonScore>();
            foreach (DungeonScore score in sortedList)
            {
                WriteLine("Portal id: {0}, score: {1}", score.m_id, score.m_score);
                writer.WriteLine("Portal id: {0}, score: {1}", score.m_id, score.m_score);
            }

            WriteLine("Evaluating regions to create portals.");
            writer.WriteLine("Evaluating regions to create portals.");

            // Connect a portal with a portal point. 
            List<RegionStatistic> statisticList = m_regionStatisticsMap.Values.ToList<RegionStatistic>();
            // Randomize the order of the regions so that the dungeons are distributed.
            Shuffle<RegionStatistic>(statisticList);

            foreach (RegionStatistic statistic in statisticList)
            {
                // Previous iteration may have emptied available dungeons.
                if (sortedList.Count < 1)
                {
                    break;
                }

                WriteLine("Region \"{0}\", chance: {1}, s. points: {2}, s. kill: {3}, t. points: {4}, t. kills: {5}",
                    statistic.m_region, statistic.m_currentChance, statistic.m_sessionPoints,
                    statistic.m_sessionKills, statistic.m_totalPoints, statistic.m_totalKills);
                writer.WriteLine("Region \"{0}\", chance: {1}, s. points: {2}, s. kill: {3}, t. points: {4}, t. kills: {5}",
                    statistic.m_region, statistic.m_currentChance, statistic.m_sessionPoints,
                    statistic.m_sessionKills, statistic.m_totalPoints, statistic.m_totalKills);

                // Assemble a chance to create a portal using base and any applicable modifiers.
                ERegion eRegion = m_regionMap[statistic.m_region];

                if (!m_regionDetailsMap.ContainsKey(eRegion))
                {
                    m_regionDetailsMap.Add(eRegion, new RegionDetails(statistic.m_region, 1, 1, 1));
                }

                RegionDetails details = m_regionDetailsMap[eRegion];
                WriteLine("Details \"{0}\", difficulty: {1}, maxPortals: {2}, portalBase: {3}",
                    details.m_region, details.m_difficulty, details.m_maxPortals, details.m_portalBase);
                writer.WriteLine("Details \"{0}\", difficulty: {1}, maxPortals: {2}, portalBase: {3}",
                    details.m_region, details.m_difficulty, details.m_maxPortals, details.m_portalBase);

                if (m_enabled && m_totalSessions > 0 && statistic.m_totalPoints > 0)
                {
                    float comparisonPointsRatio = statistic.m_sessionPoints / (statistic.m_totalPoints / m_totalSessions);
                    float performanceBonus = comparisonPointsRatio > 1.0 ? (comparisonPointsRatio - 1.0f) * 100 : 0.0f;
                    
                    // Clamp the performance bonus to 50%.
                    performanceBonus = performanceBonus > 50 ? 50 : performanceBonus;
                    float difficultyBonus = details.m_difficulty * m_difficultyFactor;
                    float chanceToSucceed = details.m_portalBase + difficultyBonus + performanceBonus;
                    statistic.m_currentChance = (int)chanceToSucceed;

                    int chance = r.Next(0, 100);
                    bool succeeded = chance < chanceToSucceed;
                    WriteLine("Requires a roll below: {0}, roll: {1}/100, {2}", chanceToSucceed, chance, succeeded ? "Success!" :"Failed.");
                    writer.WriteLine("Chance this session: {0}", chanceToSucceed);
                    if (succeeded)
                    {
                        // Swapping elements rather than copy every time.
                        List<PortalPoint> portalPointList = m_portalPointsLists[(int)eRegion];
                        int portalCount = portalPointList.Count;

                        WriteLine("{0} portal points located in the world.", portalCount);
                        writer.WriteLine("{0} portal points located in the world.", portalCount);

                        if (portalCount > 0)
                        {
                            // Portals are available in this region, clamp the quantity to create to what is available.
                            int portalsToCreate = portalCount >= details.m_maxPortals ? details.m_maxPortals : portalCount;
                            int poolIdx = portalCount - 1;

                            while (portalsToCreate > 0 && sortedList.Count > 0)
                            {
                                // The initial range is as big as the pool size; as points are used, the pool size
                                // is reduced by 1, and the used points moved outside the range.
                                int randomIdx = r.Next(0, poolIdx + 1);
                                PortalPoint portalPointToUse = portalPointList[randomIdx];
                                // Swap the positions, and trim that position off the range.
                                portalPointList[randomIdx] = portalPointList[poolIdx];
                                portalPointList[poolIdx] = portalPointToUse;

                                --poolIdx;
                                --portalsToCreate;

                                // 2/15/14 - To prevent lesser rated portals from being alienated, we've agreed to reserve
                                // 1/3rd of the portal points in the region as pure-random-picks. The other 2/3rds of portal
                                // points utilize the rating system to provide the rating-picks.
                                int dungeonTargetId = -1;
                                int pureRandomChance = r.Next(0, 100);
                                bool useRandom = pureRandomChance <= 33;
                                if (useRandom)
                                {
                                    // Discard this portal destination from the list, it is about to be used.
                                    int randomIndex = r.Next(0, sortedList.Count - 1);
                                    dungeonTargetId = sortedList[randomIndex].m_id;
                                    sortedList.RemoveAt(randomIndex);
                                }
                                else
                                {
                                    // Discard this portal destination from the list, it is about to be used.
                                    dungeonTargetId = sortedList[0].m_id;
                                    sortedList.RemoveAt(0);
                                }

                                WriteLine("Portal point selection using [{0}]", useRandom ? "random" : "rating");
                                writer.WriteLine("Portal point selection by [{0}]", useRandom ? "random" : "rating");

                                WriteLine("Connecting user portal {0} to portal point at {1},{2},{3}.", dungeonTargetId,
                                    portalPointToUse.Location.X, portalPointToUse.Location.Y, portalPointToUse.Location.Z);
                                writer.WriteLine("Connecting user portal {0} to portal point at {1},{2},{3}.", dungeonTargetId,
                                    portalPointToUse.Location.X, portalPointToUse.Location.Y, portalPointToUse.Location.Z);

                                m_activatedPortals.Add(dungeonTargetId);

                                CreatePortal(portalPointToUse, dungeonTargetId);
                            }
                        }
                    }
                }
                
                // Clear the session.
                statistic.m_sessionKills = 0;
                statistic.m_sessionPoints = 0;
            }

            WriteLine("EvaluateSession() finished.");
            writer.WriteLine("EvaluateSession() finished.");
            writer.WriteLine();
            writer.Close();

            m_totalSessions++;
        }
        struct DungeonScore
        {
            public int m_id;
            public float m_score;
            public DungeonScore(int id, float score)
            {
                m_id = id;
                m_score = score;
            }
        }
        /// <summary>
        /// Determines what destination portal to open and creates the gates between it and the portal point.
        /// </summary>
        /// <param name="portalPoint"></param>
        public void CreatePortal(PortalPoint portalPoint, int partitionId)
        {
            PortalPartition partition = PortalsSystem.GetPartitionById((short)partitionId);
            if (partition == null)
            {
                return;
            }

            Point3D originPoint = portalPoint.Location;
            Map originMap = portalPoint.Map;

            // Create gate at the portal point, targeted at a player dungeon.
            PortalMoongate toDungeonMoongate = new PortalMoongate(partition.m_entryPoint, PortalsSystem.s_map, m_portalFrequency);
            toDungeonMoongate.MoveToWorld(originPoint, originMap);
            Effects.PlaySound(originPoint, originMap, 0x20E);
            Effects.SendLocationParticles(EffectItem.Create(new Point3D(originPoint.X, originPoint.Y, originPoint.Z), originMap, EffectItem.DefaultDuration), 0x376A, 9, 10, 9502);

            // Create gate inside the player dungeon, targeted at the portal point.
            PortalMoongate toPublicMoongate = new PortalMoongate(originPoint, originMap, m_portalFrequency);
            toPublicMoongate.MoveToWorld(partition.m_entryPoint, PortalsSystem.s_map);
            Effects.PlaySound(partition.m_entryPoint, PortalsSystem.s_map, 0x20E);
            Effects.SendLocationParticles(EffectItem.Create(new Point3D(originPoint.X, originPoint.Y, originPoint.Z), originMap, EffectItem.DefaultDuration), 0x376A, 9, 10, 9502);

            // Set the portal point as the return point.
            partition.m_lastPortalPointLoc = portalPoint.Location;
            partition.m_lastPortalPointMap = portalPoint.Map;
        }
        public void AddPortalPoint(PortalPoint portalPoint)
        {
            Region region = Region.Find(portalPoint.Location, portalPoint.Map);
            int nRegions = m_regions.Length;
            for (int i = 0; i < nRegions; ++i)
            {
                if (m_regions[i] == region.Name)
                {
                    // The portal point is added to the list by index of the
                    // region in which it exists.
                    m_portalPointsLists[i].Add(portalPoint);
                }
            }
        }
        public void RemovePortalPoint(PortalPoint portalPoint)
        {
            foreach (List<PortalPoint> portalPointList in m_portalPointsLists)
            {
                if (portalPointList.Contains(portalPoint))
                {
                    portalPointList.Remove(portalPoint);
                    break;
                }
            }
        }
        
        public void OnCreatureKilled(string creatureType, string regionName)
        {
            // 9/22/13 - Hotfix, [kill command is piping down some bad data.
            if (creatureType == null || regionName == null)
                return;

            if (m_regionMap.ContainsKey(regionName) && m_creatureValueMap.ContainsKey(creatureType))
            {
                ERegion region = m_regionMap[regionName];
                if (!m_regionStatisticsMap.ContainsKey(region))
                {
                    m_regionStatisticsMap.Add(region, new RegionStatistic(regionName, 0, 0));
                }
                RegionStatistic statistic = m_regionStatisticsMap[region];

                byte weight = m_creatureValueMap[creatureType];
                ++statistic.m_totalKills;
                statistic.m_totalPoints += weight;

                ++statistic.m_sessionKills;
                statistic.m_sessionPoints += weight;
            }
        }
    }

    class CreatePortalTimer : Timer
    {
        private PortalAccessManager m_rpm;

        public CreatePortalTimer(PortalAccessManager rpm)
            : base(TimeSpan.FromMinutes(rpm.m_portalFrequency))
        {
            m_rpm = rpm;
        }

        protected override void OnTick()
        {
            m_rpm.EvaluateSession();

            CreatePortalTimer timer = new CreatePortalTimer(m_rpm);
            timer.Start();
        }
    }

    public class PortalPointGarnish : Static
    {
        public PortalPointGarnish(int gid)
            : base(gid) { }
        public PortalPointGarnish(Serial serial)
            : base(serial)
        {
        }
        public override string DefaultName
        {
            get { return ""; }
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
    public class PortalPoint : Item
    {
        [Constructable]
        public PortalPoint(Point3D p, Map m)
            : base(0x361E)
        {
            Visible = true;
            Movable = false;

            // Create some garnish for this point.
            StaticHelper[] helpers = new StaticHelper[]
            {
                new StaticHelper(p.X - 1, p.Y - 1, 0x0791, p.Z),
                new StaticHelper(p.X + 1, p.Y + 1, 0x0792, p.Z),
                new StaticHelper(p.X - 1, p.Y + 1, 0x0793, p.Z),
                new StaticHelper(p.X + 1, p.Y - 1, 0x0794, p.Z),
                new StaticHelper(p.X + 1, p.Y, 0x078A, p.Z),
                new StaticHelper(p.X, p.Y-1, 0x07A6, p.Z),
                new StaticHelper(p.X -1, p.Y, 0x078C,p.Z),
                new StaticHelper(p.X, p.Y+1,0x07A4, p.Z),
                new StaticHelper( p.X+2, p.Y-2, 0x1B0F, p.Z + 5),
                new StaticHelper(p.X - 2, p.Y, 0x1B0E, p.Z + 5)
            };

            int nHelpers = helpers.Length;
            for (int i = 0; i < nHelpers; i++)
            {
                PortalPointGarnish g = new PortalPointGarnish(helpers[i].m_id);
                g.MoveToWorld(new Point3D(helpers[i].m_x, helpers[i].m_y, helpers[i].m_z), m);
            }
        }
        public PortalPoint(Serial serial)
            : base(serial)
        {
            Visible = true;
            Movable = false;
        }
        public override void OnDelete()
        {
            IPooledEnumerable penum = Map.GetItemsInRange(Location, 3);
            List<Item> removeList = new List<Item>();
            foreach (Item item in penum)
            {
                if (item is PortalPointGarnish)
                {
                    removeList.Add(item);
                }
            }
            penum.Free();

            foreach (Item item in removeList)
            {
                item.Delete();
            }

            base.OnDelete();
        }
        public override string DefaultName
        {
            get { return "A portal platform"; }
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

    class AddPortalPointTarget : Target
    {
        public AddPortalPointTarget()
            : base(-1, true, TargetFlags.None)
        {
        }
        protected override void OnTarget(Mobile from, object targeted)
        {
            Point3D p = default(Point3D);
            if (targeted is LandTarget)
            {
                LandTarget landTarget = targeted as LandTarget;
                p = landTarget.Location;
            }
            else if (targeted is Static)
            {
                Static s = targeted as Static;
                p = s.Location;
            }
            else
            {
                from.SendMessage("That is an invalid target.");
                from.SendGump(new PortalsAdminGump(from));
                return;
            }

            PortalPoint portalPoint = new PortalPoint(p, from.Map);
            p.Z += 5;
            portalPoint.MoveToWorld(p, from.Map);

            PortalsSystem.AccessManager.AddPortalPoint(portalPoint);

            from.SendMessage("Portal point added at {0},{1},{2} in {3}.", p.X, p.Y, p.Z, from.Map);
            from.SendGump(new PortalsAdminGump(from));

        }
    }
    class RemovePortalPointTarget : Target
    {
        public RemovePortalPointTarget()
            : base(-1, true, TargetFlags.None)
        {
        }
        protected override void OnTarget(Mobile from, object targeted)
        {
            if (targeted is PortalPoint)
            {
                PortalPoint portalPoint = targeted as PortalPoint;
                PortalsSystem.AccessManager.RemovePortalPoint(portalPoint);

                portalPoint.Delete();

                from.SendMessage("Portal point deleted at {0},{1},{2} in {3}.", portalPoint.X, portalPoint.Y, portalPoint.Z, portalPoint.Map);
                from.SendGump(new PortalsAdminGump(from));
            }
        }
    }

    public class PortalMoongate : Moongate
    {
        private string m_description;

        public PortalMoongate(Point3D target, Map map, float timeMinutes)
            : base(target, map)
        {
            Map = map;
            Dispellable = false;

            this.Hue = 0x501;

            PortalMoongateTimer t = new PortalMoongateTimer(this, timeMinutes);
            t.Start();

            if (map == PortalsSystem.s_map)
            {
                // Description
                PortalPartition partition = PortalsSystem.GetPartitionAtPoint(target);
                if (partition == null)
                    return;

                int ratingCount = partition.GetRatingCount();
                float rating = partition.GetAverageRating();

                string affix = String.Empty;
                if (ratingCount < 10)
                    affix = "unrevealed";
                else if (rating == 5.0)
                    affix = "prodigal";
                else if (rating > 4.5)
                    affix = "legendary";
                else if (rating > 4.0)
                    affix = "epic";
                else if (rating > 3.5)
                    affix = "stunning";
                else if (rating > 3)
                    affix = "inviting";
                else if (rating > 2.5)
                    affix = "curious";
                else if (rating > 2.0)
                    affix = "ordinary";
                else if (rating > 1.5)
                    affix = "drab";
                else if (rating > 1.0)
                    affix = "dull";
                else if (rating > .5)
                    affix = "terrible";
                else
                    affix = "attrocious";

                m_description = String.Format("{0} {1} portal", (affix[0] == 'a' || affix[0] == 'e' || 
                    affix[0] == 'i' || affix[0] == 'o' || affix[0] == 'u' ? "An" : "A"), affix);
            }
            else
            {
                m_description = "An exit portal";
            }
        }

        public PortalMoongate(Serial serial)
            : base(serial)
        {
            m_description = String.Empty;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            Delete();
        }
        public override string DefaultName
        {
            get
            {
                return m_description;
            }
        }

        public override void UseGate(Mobile m)
        {
            if (m.Holding != null)
            {
                m.SendMessage("You can not enter the portal with holding an item.");
                return;
            }

            if (SpellHelper.CheckCombat(m, true))
			{
				m.SendLocalizedMessage(1005564, "", 0x22); // Wouldst thou flee during the heat of battle??
				return;
			}

            // OnGateUsed is fired from the destination location.
            if (m.Map == PortalsSystem.s_map && m.AccessLevel == AccessLevel.Player)
            {
                // The player has entered a dungeon partition.
                PortalPartition partition = PortalsSystem.GetPartitionAtPoint(m.Location);
                if (partition == null)
                {
                    return;
                }

                if (m is PlayerMobile)
                {
                    if (partition.m_username != m.Account.Username)
                    {
                        m.ClearTarget();

                        // Close their dungeon controller, they can't have it open while in someone
                        // else's dungeon. Also cancel any target cursors.
                        if (m.HasGump(typeof(PortalControlGump)))
                        {
                            m.CloseGump(typeof(PortalControlGump));
                        }

                        if (m.HasGump(typeof(DungeonEnteredGump)))
                        {
                            m.CloseGump(typeof(DungeonEnteredGump));
                        }

                        DungeonEnteredGump.SDungeonEnteredParams param = new DungeonEnteredGump.SDungeonEnteredParams(m,
                            partition.m_username, this.Location, this.Map);
                        DungeonEnteredGump gump = new DungeonEnteredGump(param);
                        m.SendGump(gump);
                        OnGateUsed(m);
                    }
                }
            }
            else
            {
                if (m.AccessLevel == AccessLevel.Player)
                {
                    PortalPartition partition = PortalsSystem.GetPartitionAtPoint(this.Location);
                    if (partition == null)
                    {
                        return;
                    }

                    if (partition.m_username != m.Account.Username)
                    {
                        // The player has left a dungeon partition
                        if (m.HasGump(typeof(DungeonEnteredGump)))
                        {
                            m.CloseGump(typeof(DungeonEnteredGump));
                        }
                        if (partition != null)
                        {
                            DungeonExitedGump gump = new DungeonExitedGump(m, partition.m_username);
                            m.SendGump(gump);
                            OnGateUsed(m);
                        }
                    }
                    
                }
            }
        }

        public class PortalMoongateTimer : Timer
        {
            private Item m_Item;

            public PortalMoongateTimer(Item item, float timeMinutes)
                : base(TimeSpan.FromMinutes(timeMinutes))
            {
                Priority = TimerPriority.OneSecond;
                m_Item = item;
            }

            protected override void OnTick()
            {
                m_Item.Delete();
            }
        }
    }
}
