using System;
using Server.Network;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Targeting;
using Server.Spells;
using Server.Gumps;

namespace Server.Custom
{
    public class AncientMystery
    {
        public enum MysteryType
        {
            Fountain,
            Mongbat,
            Medusa,
            Sphinx,
            Daemon,
            Vampire
        }

        public class AncientMysteryScroll : Item
        {
            public static int ResearchRequired = 6;
            public static int DiscoveriesRequired = 6;

            private bool m_Revealed = false;
            [CommandProperty(AccessLevel.GameMaster)]
            public bool Revealed
            {
                get { return m_Revealed; }
                set { m_Revealed = value; }
            }

            private bool m_Completed = false;
            [CommandProperty(AccessLevel.GameMaster)]
            public bool Completed
            {
                get { return m_Completed; }
                set { m_Completed = value; }
            }            

            private int m_ResearchAdded = 0;
            [CommandProperty(AccessLevel.GameMaster)]
            public int ResearchAdded
            {
                get { return m_ResearchAdded; }
                set { m_ResearchAdded = value; }
            }

            public List<MysteryType> m_Discoveries = new List<MysteryType>();

            private MysteryType m_RevealedMysteryType;
            [CommandProperty(AccessLevel.GameMaster)]
            public MysteryType RevealedMysteryType
            {
                get { return m_RevealedMysteryType; }
                set { m_RevealedMysteryType = value; }
            }

            [Constructable]
            public AncientMysteryScroll(): base(8792)
            {
                Name = "an ancient mystery scroll";
                Hue = 2611;

                Weight = 1;
            }

            public AncientMysteryScroll(Serial serial): base(serial)
            {
            }

            public override void OnSingleClick(Mobile from)
            {
                base.OnSingleClick(from);

                if (m_Revealed)
                {
                    if (m_Completed)
                        LabelTo(from, "(completed)");
                    else
                        LabelTo(from, "(revealed)");
                }
            }

            public override void OnDoubleClick(Mobile from)
            {
                base.OnDoubleClick(from);

                from.SendSound(0x249);

                from.CloseGump(typeof(AncientMysteryGump));
                from.SendGump(new AncientMysteryGump(this, from));                
            }            

            public void AttemptDiscovery(Mobile from, Item item)
            {
                bool success = false;

                double userSkill = from.Skills.Cartography.Value;

                double baseChance = 0;
                double finalChance = 0;

                if (userSkill < 95)
                {
                    from.SendMessage("You do not have enough cartography skill to make an appropriate discovery attempt from this.");
                    return;
                }

                if (item is TreasureMap)
                {
                    TreasureMap treasureMap = item as TreasureMap;

                    switch (treasureMap.Level)
                    {
                        case 0: baseChance = 0.025; break;

                        case 1: baseChance = 0.05; break;
                        case 2: baseChance = 0.10; break;
                        case 3: baseChance = 0.15; break;
                        case 4: baseChance = 0.20; break;
                        case 5: baseChance = 0.25; break;
                        case 6: baseChance = 0.30; break;

                        case 7: baseChance = 0.50; break;
                    }
                }

                else if (item is SOS)
                {
                    SOS sos = item as SOS;

                    switch (sos.Level)
                    {
                        case 1: baseChance = 0.10; break;
                        case 2: baseChance = 0.15; break;
                        case 3: baseChance = 0.20; break;
                        case 4: baseChance = 0.25; break;

                        case 5: baseChance = 0.30; break;
                    }
                }

                finalChance = baseChance * userSkill / 120;

                if (Utility.RandomDouble() <= finalChance)
                    success = true;

                if (success)
                {
                    item.Delete();
                    AddDiscovery(from);
                }

                else
                {
                    if (Utility.RandomDouble() <= .25)
                    {
                        from.SendSound(0x5BB);
                        from.SendMessage("You exhaust your expertise on the map's contents, and are certain no discoveries shall ever be found within.");

                        item.Delete();
                    }

                    else
                    {
                        from.SendSound(0x100);
                        from.SendMessage("You take great effort to search the map for hidden discoveries, but alas none reveal themselves to you at the moment.");
                    }
                }
            }

            public void AddResearchMaterials(Mobile from, ResearchMaterials researchMaterial)
            {
                from.SendMessage("You apply your findings from the research materials towards the mystery held within the scroll.");
                from.SendSound(0x655);

                m_ResearchAdded++;
                researchMaterial.Delete();

                CheckRevealMystery(from);
            }

            public void AddResearchedMasterScroll(Mobile from, ResearchedMasterScroll researchedMasterScroll)
            {
                from.SendMessage("You apply your findings from the researched master scroll towards the mystery held within the scroll.");
                from.SendSound(0x655);

                m_ResearchAdded++;
                researchedMasterScroll.Delete();

                CheckRevealMystery(from);
            }

            public void AddDiscovery(Mobile from)
            {
                int mysteryTypeCount = Enum.GetNames(typeof(MysteryType)).Length;
                MysteryType mysteryType = (MysteryType)Utility.RandomMinMax(0, mysteryTypeCount - 1);

                MysteryTypeDetail mysteryTypeDetail = AncientMystery.GetMysteryDetails(mysteryType);

                m_Discoveries.Add(mysteryType);
                from.SendMessage("You have made a discovery towards the mystery!");

                from.SendSound(0x102);

                CheckRevealMystery(from);
            }

            public void CheckRevealMystery(Mobile from)
            {
                if (m_Discoveries.Count >= DiscoveriesRequired && m_ResearchAdded >= ResearchRequired)
                {
                    int TotalValues = 0;   

                    Dictionary<MysteryType, int> DictMysteryTypes = new Dictionary<MysteryType, int>();

                    for (int a = 0; a < DiscoveriesRequired; a++)
                    {
                        MysteryType mysteryType = m_Discoveries[a];

                        if (DictMysteryTypes.ContainsKey(mysteryType))
                            DictMysteryTypes[mysteryType]++;
                        else
                            DictMysteryTypes.Add(mysteryType, 1);  
                    }

                    foreach (KeyValuePair<MysteryType, int> pair in DictMysteryTypes)
                    {
                        TotalValues += pair.Value;
                    }

                    double mysteryCheck = Utility.RandomDouble();

                    double CumulativeAmount = 0.0;
                    double AdditionalAmount = 0.0;

                    //Determine Mystery                      
                    foreach (KeyValuePair<MysteryType, int> pair in DictMysteryTypes)
                    {
                        AdditionalAmount = (double)pair.Value / (double)TotalValues;

                        if (mysteryCheck >= CumulativeAmount && mysteryCheck < (CumulativeAmount + AdditionalAmount))
                        {
                            m_RevealedMysteryType = pair.Key;                            
                        }

                        CumulativeAmount += AdditionalAmount;
                    }

                    from.SendSound(0x0F8);
                    m_Revealed = true;

                    MysteryTypeDetail mysteryTypeDetail = AncientMystery.GetMysteryDetails(m_RevealedMysteryType);

                    from.SendMessage("You have revealed the mystery! It is " + mysteryTypeDetail.m_Name + ".");                    
                }
            }

            public void AttemptStartMystery(Mobile from)
            {
                if (from == null)
                    return;

                MysteryTypeDetail mysteryTypeDetail = AncientMystery.GetMysteryDetails(m_RevealedMysteryType);

                if (mysteryTypeDetail == null)
                    return;

                if (m_Completed)
                {
                    from.SendMessage("The mystery held within the scroll has already been uncovered.");
                    return;
                }

                Point3D mysteryLocation = mysteryTypeDetail.m_MysteryLocation;

                if (from.InRange(mysteryLocation, 10))
                {
                    bool anotherMysteryActive = false;

                    IPooledEnumerable searchArea = Map.GetObjectsInRange(mysteryLocation, 20);
                    
                    foreach (Object targetObject in searchArea)
                    {
                        if (targetObject is MysteryLocation)
                        {
                            anotherMysteryActive = true;
                            break;
                        }
                    }

                    searchArea.Free();

                    if (anotherMysteryActive)
                    {
                        from.SendMessage("There appears to be some sort of mysterious activity already underway here. Perhaps it would be best to return later.");
                        return;
                    }

                    from.RevealingAction();
                    from.SendMessage("The mystery unfolds...");
                    StartMystery(from);
                }

                else
                    from.SendMessage("From your research, you believe a search should be conducted at " + mysteryLocation.X.ToString() + "," + mysteryLocation.Y.ToString() + ".");
            }

            public void StartMystery(Mobile from)
            {
                if (from == null)
                    return;

                MysteryTypeDetail mysteryTypeDetail = AncientMystery.GetMysteryDetails(m_RevealedMysteryType);

                if (mysteryTypeDetail == null)
                    return;

                m_Completed = true;

                from.SendSound(0x0F8);

                switch (m_RevealedMysteryType)
                {
                    case MysteryType.Daemon: Mysteries.Daemon(from); break;
                    case MysteryType.Fountain: Mysteries.Fountain(from); break;
                    case MysteryType.Medusa: Mysteries.Medusa(from); break;
                    case MysteryType.Mongbat: Mysteries.Mongbat(from); break;
                    case MysteryType.Sphinx: Mysteries.SphinxMystery(from); break;
                    case MysteryType.Vampire: Mysteries.Vampire(from); break;
                }
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);
                writer.Write((int)0); //version

                //Version 0
                writer.Write(m_Revealed);
                writer.Write(m_Completed);

                writer.Write(m_ResearchAdded);

                writer.Write(m_Discoveries.Count);
                foreach (MysteryType discovery in m_Discoveries)
                {
                    writer.Write((int)discovery);
                }

                writer.Write((int)m_RevealedMysteryType);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);
                int version = reader.ReadInt();

                m_Discoveries = new List<MysteryType>();

                if (version >= 0)
                {
                    m_Revealed = reader.ReadBool();
                    m_Completed = reader.ReadBool();

                    m_ResearchAdded = reader.ReadInt();

                    int discoveryCount = reader.ReadInt();
                    for (int i = 0; i < discoveryCount; ++i)
                    {
                        MysteryType mystery = (MysteryType)reader.ReadInt();

                        if (mystery != null)
                            m_Discoveries.Add(mystery);
                    }

                    m_RevealedMysteryType = (MysteryType)reader.ReadInt();
                }
            }
        }

        public class AncientMysteryGump : Gump
        {
            public AncientMysteryScroll m_AncientMysteryScroll;

            public AncientMysteryGump(AncientMysteryScroll ancientMysteryScroll, Mobile from): base(15, 15)
            {
                m_AncientMysteryScroll = ancientMysteryScroll;

                Closable = true;
                Disposable = true;
                Dragable = true;
                Resizable = false;

                AddPage(0);

                AddImage(15, 15, 30500); //Scroll Background

                int textHue = 2036;

                List<Point2D> m_LargeIconPoints = new List<Point2D>();

                m_LargeIconPoints.Add(new Point2D(355, 280));
                m_LargeIconPoints.Add(new Point2D(210, 355));
                m_LargeIconPoints.Add(new Point2D(75, 280));
                m_LargeIconPoints.Add(new Point2D(75, 130));
                m_LargeIconPoints.Add(new Point2D(355, 130));
                m_LargeIconPoints.Add(new Point2D(210, 39));

                List<Point2D> m_SmallIconPoints = new List<Point2D>();

                m_SmallIconPoints.Add(new Point2D(302, 354)); //AddItem(302, 358, 8792, 2611);
                m_SmallIconPoints.Add(new Point2D(147, 355)); //AddItem(146, 359, 8792, 2611);
                m_SmallIconPoints.Add(new Point2D(70, 220)); //  AddItem(69, 224, 8792, 2611);
                m_SmallIconPoints.Add(new Point2D(147, 80)); //AddItem(145, 83, 8792, 2611);
                m_SmallIconPoints.Add(new Point2D(303, 80)); //AddItem(303, 85, 8792, 2611);
                m_SmallIconPoints.Add(new Point2D(380, 220)); //AddItem(379, 225, 8792, 2611);
                
                int discoveriesCount = m_AncientMysteryScroll.m_Discoveries.Count;

                if (discoveriesCount > AncientMysteryScroll.DiscoveriesRequired)
                    discoveriesCount = AncientMysteryScroll.DiscoveriesRequired;

                //Research
                for (int a = 0; a < AncientMysteryScroll.ResearchRequired; a++)
                {
                    Point2D point = m_SmallIconPoints[a];

                    if (a < m_AncientMysteryScroll.ResearchAdded)
                        AddItem(point.X, point.Y, 8792, 2611);

                    else
                        AddImage(point.X, point.Y, 2279);
                }

                //Discoveries
                for (int a = 0; a < AncientMysteryScroll.DiscoveriesRequired; a++)
                {
                    Point2D point = m_LargeIconPoints[a];

                    if (a < discoveriesCount)
                    {
                        MysteryTypeDetail mysteryTypeDetail = GetMysteryDetails(m_AncientMysteryScroll.m_Discoveries[a]);
                        AddItem(point.X + mysteryTypeDetail.m_StatueOffsetX, point.Y + mysteryTypeDetail.m_StatueOffsetY, mysteryTypeDetail.m_StatueItemId, mysteryTypeDetail.m_StatueHue);
                    }

                    else                    
                        AddImage(point.X, point.Y, 7039);
                }

                if (!m_AncientMysteryScroll.Revealed)                
                    AddLabel(190, 175, textHue, "An Ancient Mystery");
                
                string line1 = "";
                string line2 = "";

                if (m_AncientMysteryScroll.Revealed)
                {
                    MysteryTypeDetail mysteryTypeDetail = GetMysteryDetails(m_AncientMysteryScroll.RevealedMysteryType);

                    AddItem(210 + mysteryTypeDetail.m_StatueOffsetX, 170 + mysteryTypeDetail.m_StatueOffsetY, mysteryTypeDetail.m_StatueItemId, 2501);
                    
                    line1 = "MYSTERY REVEALED";

                    if (m_AncientMysteryScroll.Completed)
                        line1 = "MYSTERY COMPLETED";

                    line2 = "\"" + mysteryTypeDetail.m_Name + "\"";

                    AddLabel(230 - (line1.Length * 3), 220, textHue, line1);
                    AddLabel(240 - (line2.Length * 3), 240, textHue, line2);
                }

                else     
                {
                    line1 = "Click to Contribute Items";
                    AddLabel(240 - (line1.Length * 3), 240, textHue, line1);    
                }

                if (!m_AncientMysteryScroll.Completed)
                    AddButton(230, 260, 9721, 9725, 1, GumpButtonType.Reply, 0);

                int researchRemaining = AncientMysteryScroll.ResearchRequired - m_AncientMysteryScroll.ResearchAdded;

                AddItem(25, 400, 8002, 2550);
                AddItem(50, 395, 7187, 2550);
                AddLabel(87, 400, textHue, "->");
                AddItem(104, 390, 8792, 2611);
                AddLabel(37, 425, textHue, "Research Needed: " + researchRemaining.ToString());

                int discoveriesRemaining = AncientMysteryScroll.DiscoveriesRequired - m_AncientMysteryScroll.m_Discoveries.Count;

                if (discoveriesRemaining < 0)
                    discoveriesRemaining = 0;

                AddItem(323, 403, 5357, 0);
                AddItem(350, 395, 5355, 2635);
                AddLabel(390, 400, textHue, "->");
                AddItem(400, 390, 4810, 2500);
                AddLabel(312, 425, textHue, "Discoveries Needed: " + discoveriesRemaining.ToString());
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                Mobile from = sender.Mobile;

                if (from == null) return;
                if (from.Deleted) return;
                if (m_AncientMysteryScroll == null) return;
                if (m_AncientMysteryScroll.Deleted) return;

                if (m_AncientMysteryScroll.RootParent != from)
                {
                    from.SendMessage("That ancient mystery scroll is no longer in your backpack.");
                    return;
                }

                if (info.ButtonID == 1)
                {
                    if (m_AncientMysteryScroll.Revealed)                    
                        m_AncientMysteryScroll.AttemptStartMystery(from);                    

                    else
                    {
                        from.SendMessage("To add Research: target a completed research material or researched master scroll. To attempt to make Discoveries: target a completed treasure map or SOS.");
                        from.Target = new AncientMysteryScrollTarget(m_AncientMysteryScroll);
                    }
                }
            }
        }

        public class AncientMysteryScrollTarget : Target
        {
            private AncientMysteryScroll m_AncientMysteryScroll;

            public AncientMysteryScrollTarget(AncientMysteryScroll ancientMysteryScroll): base(3, true, TargetFlags.None)
            {
                m_AncientMysteryScroll = ancientMysteryScroll;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                if (from == null) return;
                if (from.Deleted) return;
                if (m_AncientMysteryScroll == null) return;
                if (m_AncientMysteryScroll.Deleted || m_AncientMysteryScroll.RootParent != from)
                    return;

                PlayerMobile pm = from as PlayerMobile;

                if (pm == null)
                    return;

                if (target is ResearchMaterials)
                {
                    ResearchMaterials researchMaterials = target as ResearchMaterials;

                    if (m_AncientMysteryScroll.ResearchAdded >= AncientMysteryScroll.ResearchRequired)                    
                        from.SendMessage("This mystery already has enough research materials contributed.");                    

                    else if (!researchMaterials.Researched)                    
                        from.SendMessage("Those research materials have not yet been evaluated.");                    

                    else                    
                        m_AncientMysteryScroll.AddResearchMaterials(from, researchMaterials);                    
                }

                else if (target is ResearchedMasterScroll)
                {
                    ResearchedMasterScroll researchedMasterScroll = target as ResearchedMasterScroll;

                    if (m_AncientMysteryScroll.ResearchAdded >= AncientMysteryScroll.ResearchRequired)
                        from.SendMessage("This mystery already has enough research materials contributed.");

                    else
                        m_AncientMysteryScroll.AddResearchedMasterScroll(from, researchedMasterScroll);  
                }

                else if (target is TreasureMap)
                {
                    TreasureMap treasureMap = target as TreasureMap;

                    if (m_AncientMysteryScroll.m_Discoveries.Count >= AncientMysteryScroll.DiscoveriesRequired)                    
                        from.SendMessage("This mystery already has a sufficient number of discoveries.");                    

                    else if (!treasureMap.Completed)
                        from.SendMessage("That treasure map has not been completed yet.");

                    else
                        m_AncientMysteryScroll.AttemptDiscovery(from, treasureMap);
                }

                else if (target is SOS)
                {
                    SOS sos = target as SOS;

                    if (m_AncientMysteryScroll.m_Discoveries.Count >= AncientMysteryScroll.DiscoveriesRequired)
                        from.SendMessage("This mystery already has a sufficient number of discoveries.");  

                    else if (!sos.Completed)
                    {
                        from.SendMessage("That SOS map has not been completed yet.");

                        from.CloseGump(typeof(AncientMysteryGump));
                        from.SendGump(new AncientMysteryGump(m_AncientMysteryScroll, from));
                    }

                    else
                        m_AncientMysteryScroll.AttemptDiscovery(from, sos);
                }

                else
                    from.SendMessage("That is not research materials, a treasure map, or an SOS.");

                from.CloseGump(typeof(AncientMysteryGump));
                from.SendGump(new AncientMysteryGump(m_AncientMysteryScroll, from));
            }
        }

        public static MysteryTypeDetail GetMysteryDetails(MysteryType mysteryType)
        {
            MysteryTypeDetail mysteryTypeDetail = new MysteryTypeDetail();
                       
            switch (mysteryType)
            {
                case MysteryType.Daemon:
                    mysteryTypeDetail.m_MysteryType = MysteryType.Daemon;
                    mysteryTypeDetail.m_Name = "The Pact";
                    mysteryTypeDetail.m_StatueItemId = 13899;
                    mysteryTypeDetail.m_StatueHue = 2500;
                    mysteryTypeDetail.m_StatueOffsetX = 14;
                    mysteryTypeDetail.m_StatueOffsetY = -62;
                    mysteryTypeDetail.m_MysteryLocation = new Point3D(4644, 3654, 100);
                break;

                case MysteryType.Fountain:
                    mysteryTypeDetail.m_MysteryType = MysteryType.Fountain;
                    mysteryTypeDetail.m_Name = "The Collapse";
                    mysteryTypeDetail.m_StatueItemId = 17090;
                    mysteryTypeDetail.m_StatueHue = 2500;
                    mysteryTypeDetail.m_StatueOffsetX = 14;
                    mysteryTypeDetail.m_StatueOffsetY = -40;
                    mysteryTypeDetail.m_MysteryLocation = new Point3D(1170, 2916, 0);
                break;

                case MysteryType.Medusa:
                    mysteryTypeDetail.m_MysteryType = MysteryType.Medusa;
                    mysteryTypeDetail.m_Name = "The Seduction";
                    mysteryTypeDetail.m_StatueItemId = 16572;
                    mysteryTypeDetail.m_StatueHue = 2500;
                    mysteryTypeDetail.m_StatueOffsetX = 14;
                    mysteryTypeDetail.m_StatueOffsetY = -40;
                    mysteryTypeDetail.m_MysteryLocation = new Point3D(1128, 3452, 0);
                break;

                case MysteryType.Mongbat:
                    mysteryTypeDetail.m_MysteryType = MysteryType.Mongbat;
                    mysteryTypeDetail.m_Name = "The Jest";
                    mysteryTypeDetail.m_StatueItemId = 6483;
                    mysteryTypeDetail.m_StatueHue = 2500;
                    mysteryTypeDetail.m_StatueOffsetX = -7;
                    mysteryTypeDetail.m_StatueOffsetY = -18;
                    mysteryTypeDetail.m_MysteryLocation = new Point3D(1370, 505, 1);
                break;

                case MysteryType.Sphinx:
                    mysteryTypeDetail.m_MysteryType = MysteryType.Sphinx;
                    mysteryTypeDetail.m_Name = "The Enigma";
                    mysteryTypeDetail.m_StatueItemId = 17091;
                    mysteryTypeDetail.m_StatueHue = 2500;
                    mysteryTypeDetail.m_StatueOffsetX = 15;
                    mysteryTypeDetail.m_StatueOffsetY = -35;
                    mysteryTypeDetail.m_MysteryLocation = new Point3D(1857, 962, -1);
                break;

                case MysteryType.Vampire:
                    mysteryTypeDetail.m_MysteryType = MysteryType.Vampire;
                    mysteryTypeDetail.m_Name = "The Betrayal";
                    mysteryTypeDetail.m_StatueItemId = 17093;
                    mysteryTypeDetail.m_StatueHue = 2500;
                    mysteryTypeDetail.m_StatueOffsetX = 14;
                    mysteryTypeDetail.m_StatueOffsetY = -63;
                    mysteryTypeDetail.m_MysteryLocation = new Point3D(1467, 2511, 7);
                break;
            }

            return mysteryTypeDetail;
        }

        public class MysteryTypeDetail
        {
            public MysteryType m_MysteryType;
            public string m_Name = "";
            public int m_StatueItemId = 0;
            public int m_StatueHue = 2500;
            public int m_StatueOffsetX = 0;
            public int m_StatueOffsetY = 0;
            public Point3D m_MysteryLocation = Point3D.Zero;

            public MysteryTypeDetail()
            {
            }
        }       
    }    
}
