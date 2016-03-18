/***************************************************************************
 *                              YewJailEscapeHelper.cs
 *                            -------------------
 *   begin                : July 2010
 *   author               : Sean Stavropoulos
 *   email                : sean.stavro@gmail.com
 *
 *
 ***************************************************************************/
using System;
using Server;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Engines.Quests;

namespace Server.YewJail
{
    public class YewJailEscapeHelper : BaseVendor
    {
        public YewJailEscapeHelper(YewJailItem m) : base("") {
            jailNo = m.m_JailNo;
        }

        private Timer m_Timer;
        private DateTime m_deleteTime;
        public Point3D bagSpawnLoc;
        public int jailNo;
        public List<Point3D> coordinates = new List<Point3D>();
        public Direction finalDirection = Direction.North;
		private List<SBInfo> m_SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }
        public override void InitSBInfo() {}

        private List<Mobile> m_Hostiles = new List<Mobile>();
        public override bool IsInvulnerable { get { return false; } }
        public override bool IsActiveVendor { get { return false; } }
        public override bool CanTeach { get { return false; } }
        public Point3D BagSpawnLocation { get { return bagSpawnLoc; } set { bagSpawnLoc = value; } }

        public override void InitBody()
        {
            InitStats(40, 125, 25);
            Hue = Utility.RandomSkinHue();
            Female = false;
            Body = 0x190;
            Name = NameList.RandomName("male");
            m_deleteTime = DateTime.UtcNow + TimeSpan.FromMinutes(2);
            m_Timer = new JailEscapeHelperInternalTimer(this);
            m_Timer.Start();

        }

        public override void InitOutfit()
        {
            switch (Utility.Random(3))
            {
                case 0:
                    {
                        AddItem(new PlateChest());
                        AddItem(new PlateArms());
                        AddItem(new PlateGloves());
                        AddItem(new PlateLegs());
                    } break;
                case 1:
                    {
                        AddItem(new LongPants());
                        AddItem(new FancyShirt(690));
                        AddItem(new Bandana());
                        AddItem(new ThighBoots());
                    } break;
                case 2:
                    {
                        AddItem(new LongPants());
                        AddItem(new Doublet(537));
                        AddItem(new SkullCap());
                        AddItem(new ThighBoots());
                    } break;
                case 3:
                    {
                        AddItem(new Robe(672));
                        AddItem(new WizardsHat(997));
                        AddItem(new ThighBoots());
                    } break;
                case 4:
                    {
                        AddItem(new StrawHat());
                        AddItem(new ShortPants(547));
                        AddItem(new Sandals());
                    } break;

            }
            Utility.AssignRandomHair(this);
            Utility.AssignRandomFacialHair(this, HairHue);

        }

        public YewJailEscapeHelper(Serial serial)
            : base(serial)
        {
        }

        public override void OnAfterDelete()
        {
            if (m_Timer != null)
                m_Timer.Stop();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

        writer.Write((DateTime) m_deleteTime);
        writer.Write((Point3D) bagSpawnLoc);

            if (coordinates == null)
            {
                writer.WriteEncodedInt((int)0);
            }
            else
            {
                writer.WriteEncodedInt((int)coordinates.Count);

                for (int i = 0; i < coordinates.Count; ++i)
                {
                    Point3D c = coordinates[i];
                    writer.Write((Point3D)c);
                }
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            //private Timer m_Timer = reader.ReadT;
            
            m_deleteTime = reader.ReadDateTime();
            bagSpawnLoc = reader.ReadPoint3D();

            int count = reader.ReadEncodedInt();
            if (count > 0)
            {
                coordinates = new List<Point3D>();

                for (int i = 0; i < count; ++i)
                {
                   coordinates.Add(reader.ReadPoint3D());
                }
            }

            m_deleteTime = DateTime.UtcNow + TimeSpan.FromMinutes(2);
            m_Timer = new JailEscapeHelperInternalTimer(this);
            m_Timer.Start();
        }

        public class JailEscapeHelperInternalTimer : Timer
        {
            private YewJailEscapeHelper m_Helper;

            private int m_CoordinateNumber = 0;

            private bool spoken = false;

            private DateTime EndTime;

            private DateTime waitTime;

            public JailEscapeHelperInternalTimer(YewJailEscapeHelper guard)
                : base(TimeSpan.Zero, TimeSpan.FromMilliseconds(300))
            {
                m_Helper = guard;
                Priority = TimerPriority.FiftyMS;
                EndTime = guard.m_deleteTime;
            }

            protected override void OnTick()
            {
                if (m_Helper.Deleted)
                    return;

                if (DateTime.UtcNow > EndTime)
                {
                    m_Helper.Delete();
                    return;
                }
                if (m_Helper.jailNo < 10)
                {
                    YewJailControl.OpenEntranceDoor();

                    if ((m_CoordinateNumber < 4) && !m_Helper.InRange(m_Helper.coordinates[m_CoordinateNumber], 0))
                    {
                        m_Helper.Move(m_Helper.GetDirectionTo(m_Helper.coordinates[m_CoordinateNumber]));
                    }
                    else if ((m_CoordinateNumber < 4) && m_Helper.InRange(m_Helper.coordinates[m_CoordinateNumber], 0))
                    {
                        m_CoordinateNumber++;
                    }
                    else if (!spoken)
                    {
                        spoken = true;
                        m_Helper.Direction = m_Helper.finalDirection;
                        Speak();
                        waitTime = DateTime.UtcNow + TimeSpan.FromSeconds(10);
                        SpawnLockpickBag();
                        m_Helper.Frozen = true;
                    }
                    else if ((m_CoordinateNumber >= 4) && !m_Helper.InRange(m_Helper.coordinates[m_CoordinateNumber], 0) && (DateTime.UtcNow > waitTime))
                    {
                        m_Helper.Frozen = false;
                        m_Helper.Move(m_Helper.GetDirectionTo(m_Helper.coordinates[m_CoordinateNumber]));
                    }
                    else if (m_CoordinateNumber == 7 && (DateTime.UtcNow > waitTime))
                    {
                        this.Stop();
                        m_Helper.Delete();
                    }
                    else if ((m_CoordinateNumber >= 4) && m_Helper.InRange(m_Helper.coordinates[m_CoordinateNumber], 0) && (DateTime.UtcNow > waitTime))
                    {
                        m_CoordinateNumber++;
                    }
                }
                else
                {
                    YewJailControl.OpenEntranceDoor2();

                    if ((m_CoordinateNumber < 2) && !m_Helper.InRange(m_Helper.coordinates[m_CoordinateNumber], 0))
                    {
                        m_Helper.Move(m_Helper.GetDirectionTo(m_Helper.coordinates[m_CoordinateNumber]));
                    }
                    else if ((m_CoordinateNumber < 2) && m_Helper.InRange(m_Helper.coordinates[m_CoordinateNumber], 0))
                    {
                        m_CoordinateNumber++;
                    }
                    else if (!spoken)
                    {
                        spoken = true;
                        m_Helper.Direction = m_Helper.finalDirection;
                        Speak();
                        waitTime = DateTime.UtcNow + TimeSpan.FromSeconds(10);
                        SpawnLockpickBag();
                        m_Helper.Frozen = true;
                    }
                    else if ((m_CoordinateNumber >= 2) && !m_Helper.InRange(m_Helper.coordinates[m_CoordinateNumber], 0) && (DateTime.UtcNow > waitTime))
                    {
                        m_Helper.Frozen = false;
                        m_Helper.Move(m_Helper.GetDirectionTo(m_Helper.coordinates[m_CoordinateNumber]));
                    }
                    else if (m_CoordinateNumber == 3 && (DateTime.UtcNow > waitTime))
                    {
                        this.Stop();
                        m_Helper.Delete();
                    }
                    else if ((m_CoordinateNumber >= 2) && m_Helper.InRange(m_Helper.coordinates[m_CoordinateNumber], 0) && (DateTime.UtcNow > waitTime))
                    {
                        m_CoordinateNumber++;
                    }


                }
            }


            private void Speak()
            {
                string first = @"";
                string second = @"";

                switch (Utility.Random(3))
                {
                    case 0:
                        {
                            first = @"my brother Quarketh";
                            second = @"bar";
                        } break;
                    case 1:
                        {
                            first = @"my business partner";
                            second = @"gold";
                        } break;
                    case 2:
                        {
                            first = @"my competition";
                            second = @"treasure";
                        } break;
                    case 3:
                        {
                            first = @"the roaches";
                            second = @"kitchen";
                        } break;
                }

                m_Helper.Yell(String.Format("Here... I owe you one. You killed {0}. The {1} is mine now.", first, second));
            }

            private void SpawnLockpickBag()
            {
                Container bag = new Bag();
                bag.AddItem(new Lockpick(3));
                bag.MoveToWorld(YewJailControl.m_bagSpawnLoc[m_Helper.jailNo-1], Map.Felucca);
            }
        }
    }






}