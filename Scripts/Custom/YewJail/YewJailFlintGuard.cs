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
    public class YewJailFlintGuard : BaseVendor
    {
        public YewJailFlintGuard(YewJailItem jitem) : base("") 
        {
            jailNo = jitem.m_JailNo;
        }

        private Timer m_Timer;
        private DateTime m_deleteTime;
        public Point3D bagSpawnLoc;
        public int jailNo;
        public List<Point3D> coordinates = new List<Point3D>();
        public Direction finalDirection = Direction.North;
		private List<SBInfo> m_SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }
        public override void InitSBInfo() { }

        private List<Mobile> m_Hostiles = new List<Mobile>();
        public bool IsInvulnerable { get { return false; } }
        public bool IsActiveVendor { get { return false; } }
        public bool CanTeach { get { return false; } }
        public Point3D BagSpawnLocation { get { return bagSpawnLoc; } set { bagSpawnLoc = value; } }

        public override void InitBody()
        {
            InitStats(40, 125, 25);
            Hue = Utility.RandomSkinHue();
            Female = false;
            Body = 0x190;
            Name = NameList.RandomName("male");
            m_deleteTime = DateTime.UtcNow + TimeSpan.FromMinutes(2);
            m_Timer = new JailFlintGuardInternalTimer(this);
            m_Timer.Start();
            Title = "the Jail Guard";
            //flint = new Flint();
            //AddToBackpack(flint);
        }

        public override void InitOutfit()
        {
            AddItem(new PlateChest());
            AddItem(new PlateArms());
            AddItem(new PlateGloves());
            AddItem(new PlateLegs());

            Utility.AssignRandomHair(this);
            Utility.AssignRandomFacialHair(this, HairHue);
        }
        private bool triedsteal = false;
        public void TrySteal(Mobile from)
        {
            if (!triedsteal)
            {
                double probability = 10; //percet
                double skillmod = from.Skills.Stealing.Base - 90;
                if (skillmod > 0)
                {
                    probability += skillmod;
                    if (Utility.Random(1000) < probability * 10)
                        from.AddToBackpack(new Flint());
                    else
                        SayTo(from, true, "Huh? Whaaa? Why I oughta... thief, eh?");
                }
                else
                    SayTo(from, true, "Huh? Whaaa? Why I oughta... thief, eh?");

                triedsteal = true;
            }
            else
            {
                SayTo(from, true, "Get your hands away from me, you thief!");
            }
        }

        private bool triedbeg = false;
        public void TryBeg(Mobile from)
        {
            if (!triedbeg)
            {
                double probability = 20; //percet
                double skillmod = from.Skills.Begging.Base - 80;
                if (skillmod > 0)
                {
                    probability += skillmod;
                    if (Utility.Random(1000) < probability * 10)
                    {
                        from.AddToBackpack(new Flint());
                        SayTo(from, true, "Hmm? The flint? Well, I suppose... what harm could it do?");
                    }
                    else
                        SayTo(from, true, "Hah! I am certainly not that gullible.");
                }
                else
                    SayTo(from, true, "Hah! I am certainly not that gullible.");

                triedbeg = true;
            }
            else
            {
                SayTo(from, true, "You've already tried to beg from me.");
            }
        }

        public YewJailFlintGuard(Serial serial)
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
            writer.Write((int)jailNo);
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
            m_Timer = new JailFlintGuardInternalTimer(this);
            m_Timer.Start();
            jailNo = reader.ReadInt();
            YewJailControl.RegisterFlintGuard(this, jailNo);
        }

        public class JailFlintGuardInternalTimer : Timer
        {
            private YewJailFlintGuard m_Helper;

            private static TimeSpan m_location;

            private int m_CoordinateNumber = 0;

            private bool spoken = false;

            private DateTime EndTime;

            private DateTime waitTime;

            public JailFlintGuardInternalTimer(YewJailFlintGuard guard)
                : base(TimeSpan.Zero, TimeSpan.FromMilliseconds(200))
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
                        waitTime = DateTime.UtcNow + TimeSpan.FromSeconds(60);
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
                        waitTime = DateTime.UtcNow + TimeSpan.FromSeconds(60);
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
                m_Helper.Say(true,"You two! What's going on in here? Better not be causing any trouble...");
            }
        }
    }






}