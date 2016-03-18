/***************************************************************************
 *                              YewBombHelper.cs
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
    public class YewJailBombHelper : BaseVendor
    {
        public YewJailItem JailItem;
        public Mobile Target;
        public JailBombHelperInternalTimer m_Timer;
        public DateTime m_deleteTime;
        public int noBreadEaten = 0;

        public override bool AlwaysMurderer { get { return true; } }

        public YewJailBombHelper(YewJailItem m) : base("") 
        { 
            JailItem = m;
            Target = m.m_Jailed;
            m_deleteTime = DateTime.UtcNow + TimeSpan.FromMinutes(5);
            m_Timer = new JailBombHelperInternalTimer(this);
            m_Timer.Start();
            JailItem.RegisterBombHelper(this);
            jailNo = m.m_JailNo;

        }

        public Point3D bagSpawnLoc;
        public int jailNo;
        public List<Point3D> coordinates = new List<Point3D>();
        public Direction finalDirection = Direction.North;
        private List<Mobile> m_Hostiles = new List<Mobile>();
        public Point3D BagSpawnLocation { get { return bagSpawnLoc; } set { bagSpawnLoc = value; } }

        //VENDOR STUFF
		private List<SBInfo> m_SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }
        public override void InitSBInfo() { }
        public override bool IsInvulnerable { get { return false; } }
        public override bool IsActiveVendor { get { return false; } }
        public override bool CanTeach { get { return false; } }
        //END VENDOR STUFF


        public override void InitBody()
        {
            InitStats(40, 125, 25);
            Hue = Utility.RandomSkinHue();
            Female = false;
            Body = 0x190;
            Name = "a prisoner";//NameList.RandomName("male");
        }

        public override void InitOutfit()
        {
            AddItem(new Robe());
            Utility.AssignRandomHair(this);
            Utility.AssignRandomFacialHair(this, HairHue);
        }

        public YewJailBombHelper(Serial serial) : base(serial) { }

        public override void OnAfterDelete()
        {
            if (m_Timer != null)
                m_Timer.Stop();
        }

        public void EatBread() 
        { 
            noBreadEaten++;
            if (noBreadEaten == 3)
            {
                Manure m = new Manure();
                m.MoveToWorld(Target.Location, Map.Felucca);
                Target.LocalOverheadMessage(Network.MessageType.Regular, Target.EmoteHue, true, "*After all that bread, you create some manure by your feet*");
            }
        
        }
        private bool bFlint, bManure, bWire = false;
        public override bool OnDragDrop(Mobile from, Item item)
        {
            if (item is Manure)
            {
                if (from.Skills.Tinkering.Base > 80)
                {
                    SayTo(from, true, "I don't want that. You're making the bomb!");
                    return false;
                }
                else
                {
                    item.Delete();
                    SayTo(from, true, "Great! We have the manure now...");
                    bManure = true;
                    MakeBomb();
                    return true;
                }
            }
            if (item is HideableSilverWire)
            {
                if (from.Skills.Tinkering.Base > 80)
                {
                    SayTo(from, true, "I don't want that. You're making the bomb!");
                    return false;
                }
                else
                {
                    item.Delete();
                    SayTo(from, true, "Hmm... yes this wire should do just fine.");
                    bWire = true;
                    MakeBomb();
                    return true;
                }
            }
            if (item is Flint)
            {
                if (from.Skills.Tinkering.Base > 80)
                {
                    SayTo(from, true, "I don't want that. You're making the bomb!");
                    return false;
                }
                else
                {
                    item.Delete();
                    SayTo(from, true, "We have the flint now! Yes!");
                    bFlint = true;
                    MakeBomb();
                    return true;
                }
            }
            return false;
        }

        private void MakeBomb()
        {
            if (bFlint && bWire && bManure)
            {
                SayTo(Target, true, "We have all three items now.  Stand back and let me handle this door.");
                m_Timer.Step = 7;
            }
            else
            {
                if ((bFlint && bWire) || (bFlint && bManure) || (bManure && bWire))
                    SayTo(Target, true, "All we need is one more item");
                else
                    SayTo(Target, true, "Just a couple more items..");
            }
        }

        //public override bool HandlesOnSpeech { get { return true; } }
        private bool GuardSpoken = false;
        public override void OnSpeech(SpeechEventArgs e)
        {
            if (e.Mobile is YewJailFlintGuard && !GuardSpoken)
            {
                GuardSpoken = true;
                Target.LocalOverheadMessage(Network.MessageType.Emote, Target.EmoteHue, true, "*You notice the guard has some flint in his pack*");
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((DateTime)m_deleteTime);
            writer.Write((Point3D)bagSpawnLoc);

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
            writer.WriteItem((YewJailItem)JailItem);
            writer.Write((int)m_Timer.Step);
            writer.Write((DateTime)m_deleteTime);
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
            JailItem = reader.ReadItem() as YewJailItem;
            m_Timer.Step = reader.ReadInt();
            m_deleteTime = reader.ReadDateTime();

            JailItem.RegisterBombHelper(this);

            m_Timer = new JailBombHelperInternalTimer(this);
            m_Timer.Start();
        }

    }
    public class JailBombHelperInternalTimer : Timer
    {
        private YewJailBombHelper m_Helper;

        public int Step = 0;

        private DateTime nextStep;

        private DateTime EndTime;

        private bool tinkerer = false;

        private Mobile prisoner;

        public JailBombHelperInternalTimer(YewJailBombHelper guard)
            : base(TimeSpan.Zero, TimeSpan.FromMilliseconds(500))
        {
            m_Helper = guard;
            Priority = TimerPriority.FiftyMS;
            EndTime = guard.m_deleteTime;
            nextStep = DateTime.UtcNow + TimeSpan.FromSeconds(10);
            prisoner = m_Helper.JailItem.m_Jailed;
            tinkerer = (prisoner.Skills.Tinkering.Base > 80);
            Step = 0;

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


            if (DateTime.UtcNow > nextStep)
            {
                switch (Step)
                {
                    case 0:
                        {
                            m_Helper.SayTo(prisoner, true, "*grumble* Damn bounty hunters...");
                            nextStep += TimeSpan.FromSeconds(10);
                            Step++;
                        } break;
                    case 1:
                        {
                            m_Helper.SayTo(prisoner, true, "I need to get out of here...");
                            nextStep += TimeSpan.FromSeconds(15);
                            Step++;
                        } break;
                    case 2:
                        {
                            m_Helper.SayTo(prisoner, true, "*looks around*");
                            nextStep += TimeSpan.FromSeconds(3);
                            Step++;
                        } break;
                    case 3:
                        {
                            if (tinkerer)
                                m_Helper.SayTo(prisoner, true, "You want to get out of here? I've been thinking... If you find some wire and manure...");
                            else
                                m_Helper.SayTo(prisoner, true, "You want to get out of here? I've been thinking... If you find some wire, flint, and manure...");
                            nextStep += TimeSpan.FromSeconds(8);
                            Step++;
                        } break;
                    case 4:
                        {
                            if (tinkerer)
                            {
                                m_Helper.SayTo(prisoner, true, "Perhaps you could make a bomb to blow the door down. I have some flint...");
                                prisoner.LocalOverheadMessage(Network.MessageType.Regular, prisoner.EmoteHue, true, @"The prisoner hands you some flint.");
                                prisoner.AddToBackpack(new Flint());
                            }
                            else
                            {
                                m_Helper.SayTo(prisoner, true, "Perhaps I could make a bomb to blow the door down.");
                            }
                            JailBreadLoaf bread = new JailBreadLoaf(m_Helper.jailNo, 3);
                            bread.MoveToWorld(YewJailControl.m_bagSpawnLoc[m_Helper.jailNo - 1], Map.Felucca);

                            nextStep += TimeSpan.FromSeconds(20);
                            Step++;

                        } break;
                    case 5:
                        {
                            if (!tinkerer)
                            {
                                YewJailFlintGuard helper = new YewJailFlintGuard(m_Helper.JailItem);
                                if (m_Helper.jailNo < 10)
                                {
                                    helper.MoveToWorld(YewJailControl.m_HelperSpawnLoc, Map.Felucca);
                                    helper.coordinates.Add(new Point3D(295, 765, 0));
                                    helper.coordinates.Add(new Point3D(295, 768, 0));
                                    helper.coordinates.Add(new Point3D(289, 771, 0));
                                    helper.coordinates.Add(YewJailControl.m_HelperFinalLoc[helper.jailNo - 1]);
                                    helper.bagSpawnLoc = YewJailControl.m_bagSpawnLoc[helper.jailNo - 1];
                                    if (helper.jailNo < 6 || (helper.jailNo > 10 && helper.jailNo < 16))
                                        helper.finalDirection = Direction.North;
                                    else
                                        helper.finalDirection = Direction.South;
                                    helper.coordinates.Add(new Point3D(289, 771, 0));
                                    helper.coordinates.Add(new Point3D(295, 768, 0));
                                    helper.coordinates.Add(new Point3D(295, 765, 0));
                                    helper.coordinates.Add(new Point3D(291, 764, 20));
                                }
                                else
                                {
                                    helper.MoveToWorld(YewJailControl.m_HelperSpawnLoc2, Map.Felucca);
                                    helper.coordinates.Add(new Point3D(289, 771, 20));
                                    helper.jailNo = m_Helper.jailNo;
                                    helper.coordinates.Add(YewJailControl.m_HelperFinalLoc[helper.jailNo - 1]);
                                    if (helper.jailNo < 6 || (helper.jailNo > 10 && helper.jailNo < 16))
                                        helper.finalDirection = Direction.North;
                                    else
                                        helper.finalDirection = Direction.South;
                                    helper.coordinates.Add(new Point3D(289, 771, 20));
                                    helper.coordinates.Add(YewJailControl.m_HelperSpawnLoc2);
                                    helper.coordinates.Add(YewJailControl.m_HelperSpawnLoc2);
                                }
                            }
                            Step++;
                        } break;
                    case 6:
                        {
                            //Nothing. Waiting for items.
                        }break;
                    case 7:
                        {
                            m_Helper.Frozen = false;
                            if (!m_Helper.InRange(YewJailControl.m_doorBombLocation[m_Helper.jailNo - 1], 0))
                            {
                                m_Helper.Move(m_Helper.GetDirectionTo(YewJailControl.m_doorBombLocation[m_Helper.jailNo - 1]));
                                m_Helper.Frozen = true;
                            }
                            else
                            {
                                if (m_Helper.jailNo < 6 || (m_Helper.jailNo > 10 && m_Helper.jailNo < 16))
                                    m_Helper.Direction = Direction.South;
                                else
                                    m_Helper.Direction = Direction.North;
                                m_Helper.Frozen = true;
                                nextStep = DateTime.UtcNow + TimeSpan.FromSeconds(5);
                                Step++;
                            }

                        }break;
                    case 8:
                        {
                            m_Helper.Frozen = false;
                            if (!m_Helper.InRange(YewJailControl.m_BombHelperLocations[m_Helper.jailNo - 1], 0))
                            {
                                m_Helper.Move(m_Helper.GetDirectionTo(YewJailControl.m_BombHelperLocations[m_Helper.jailNo - 1]));
                                m_Helper.Frozen = true;
                            }
                            else
                            {
                                m_Helper.SayTo(prisoner, true, "Any second now...");
                                m_Helper.Direction = Direction.Down;
                                m_Helper.Frozen = true;
                                Step++;
                                nextStep = DateTime.UtcNow + TimeSpan.FromSeconds(5);
                            }
                                
                        }break;
                    case 9:
                        {

                            Effects.SendLocationEffect(YewJailControl.m_doorSpawnLocs[m_Helper.jailNo - 1], Map.Felucca, 0x36BD, 15, 10);
                            YewJailControl.m_YewJailDoorItems[m_Helper.jailNo].Delete();
                            YewJailControl.UnRegisterJailDoor(m_Helper.jailNo);
                            m_Helper.SayTo(prisoner, true, "Yes! We're free! Get out quick!");
                            Step++;
                            nextStep = DateTime.UtcNow + TimeSpan.FromSeconds(6);
                        } break;
                    case 10:
                        {
                            YewJailDoor jailDoor;
                            Point3D loc = YewJailControl.m_doorSpawnLocs[m_Helper.jailNo - 1];
                            if (loc.Y == 769)
                                jailDoor = new YewJailDoor(DoorFacing.WestCW, m_Helper.jailNo);
                            else
                                jailDoor = new YewJailDoor(DoorFacing.WestCCW, m_Helper.jailNo);

                            jailDoor.Locked = true;
                            jailDoor.MoveToWorld(loc, Map.Felucca);

                            YewJailControl.m_YewJailDoorItems.Add(m_Helper.jailNo, jailDoor);
                            m_Helper.Delete();
                            this.Stop();

                        } break;
                    case 11:
                        {
                            m_Helper.SayTo(prisoner, true, "No! You ruined it! Now we're stuck in here...");
                            Step = 12;
                            nextStep = DateTime.UtcNow + TimeSpan.FromSeconds(6);

                        }break;
                    case 12:
                        {
                            m_Helper.Delete();
                            this.Stop();

                        } break;
                }
            }
        }
    }

    public class JailBreadLoaf : Food
    {
        private int m_JailNo;

        public JailBreadLoaf(int JailNo)
            : this(JailNo, 1)
        {
        }

        public JailBreadLoaf(int JailNo, int amount)
            : base(amount, 0x103B)
        {
            this.Weight = 1.0;
            this.FillFactor = 1;
            m_JailNo = JailNo;
        }

        public JailBreadLoaf(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (YewJailControl.m_YewJailItems.ContainsKey(m_JailNo) && from.InRange(this.Location,1))
            {
                YewJailControl.m_YewJailItems[m_JailNo].EatBread();
            }

            base.OnDoubleClick(from);
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
            writer.Write((int)m_JailNo);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            m_JailNo = reader.ReadInt();
        }
    }



}