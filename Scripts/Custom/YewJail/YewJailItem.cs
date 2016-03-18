/***************************************************************************
 *                              YewJailItem.cs
 *                            -------------------
 *   begin                : July 2010
 *   author               : Sean Stavropoulos
 *   email                : sean.stavro@gmail.com
 *
 *
 ***************************************************************************/
using System;
using Server;
using Server.Targeting;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Misc;
using Server.Items;
using Server.Mobiles;
using Server.Commands;
using Server.Custom.YewJail;

namespace Server.YewJail
{
    public class YewJailItem : Item
    {
        private Timer m_Timer;
        public TimeSpan m_End;
        public TimeSpan m_End_orig;
        public DateTime offline_release_date;
        public Mobile m_Jailed;
        public Int32 m_JailNo;
        private YewJailDoor m_JailDoor;
        public Queue m_overheadEmoteQueue = new Queue();
        public DateTime m_overheadEmoteQueueTime;
        public bool spawnedHelper = false;
        private List<Mobile> m_Guards = new List<Mobile>();
        public override bool HandlesOnSpeech { get { return true; } }

        public YewJailItem(Mobile m, TimeSpan duration, Int32 jailNo) : base(0xEED)
        {
            PlayerMobile pm = m as PlayerMobile;
            pm.m_YewJailItem = this;
            pm.YewJailed = true;
            Visible = false;
            m.AddToBackpack(this);
            m_End = pm.GameTime + duration;
            m_End_orig = pm.GameTime + duration;
            m_Jailed = m;
            m_JailNo = jailNo;
            LinkDoor();
            m_JailDoor.Open = false;
            Name = "Jail Tag";
            Movable = false;
            offline_release_date = DateTime.UtcNow + TimeSpan.FromSeconds(m_End.TotalSeconds * 10);

            double skillTinkering = m.Skills.Tinkering.Base;
            double skillBegging = m.Skills.Begging.Base;
            double skillStealing = m.Skills.Stealing.Base;
            double skillLockpicking = m.Skills.Lockpicking.Base;

            if (skillTinkering > 80 || skillBegging > 80 || skillStealing > 80)
            {
                m_Timer = new BombQuestInternalTimer(this);
                m_Timer.Start();
            }
            else if (skillLockpicking > 80)
            {
                m_Timer = new LockpickQuestInternalTimer(this);
                m_Timer.Start();
            }
            else
            {
                m_Timer = new DaemonQuestInternalTimer(this);
                m_Timer.Start();
            }
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            if (e.Mobile == m_Jailed && e.Speech.ToLower().Contains("bail"))
                    m_Jailed.SendGump(new YewJailBailGump(this));
        }

        public void EscapedJail()
        {
            m_End = ((PlayerMobile)m_Jailed).GameTime + TimeSpan.FromSeconds(15);
            m_Jailed.SendMessage("You have just escaped jail! If you can stay alive for long enough to get away, then maybe you'll be gone for good!");
        }

        public void LinkDoor()
        {
            m_JailDoor = YewJailControl.m_YewJailDoorItems[m_JailNo];
            m_JailDoor.YewJailItem = this;
        }

        public void MakeDoorUnlockpickable()
        {
            m_JailDoor.RequiredSkill = 0;
            m_JailDoor.LockLevel = 120;
        }

        public void MakeDoorLockpickable()
        {
            m_JailDoor.RequiredSkill = 0;
            m_JailDoor.LockLevel = 94;
        }

        private void DeleteGuards()
        {
            foreach (YewJailGuard g in m_Guards)
            {
                if (g != null)
                {
                    g.Delete();
                }
            }
            m_Guards.Clear();
        }

        public void DoorPicked()
        {
            //SPAWN GUARDS
            m_Jailed.Criminal = true;
            foreach (Point3D p in YewJailControl.m_GuardSpawnLocs)
            {
                YewJailGuard g = new YewJailGuard();
                g.MoveToWorld(p, Map.Felucca);
                m_Guards.Add(g);
                g.AddHostileTarget(m_Jailed);
            }

            //ADD ITEMS TO CHEST
            YewJailControl.FillChest();
        }

        public YewJailBombHelper BombHelper;
        public void RegisterBombHelper(YewJailBombHelper from) { BombHelper = from; }

        public void EatBread() { 
            BombHelper.EatBread(); 
        }

        public void OnDeath()
        {
            if (!m_Jailed.Alive)
                m_Jailed.Resurrect();
            m_JailDoor.Locked = true;
            m_JailDoor.Open = false;

            DeleteGuards();
            if (m_Jailed.LastKiller is JailBountyHunter)
                Timer.DelayCall(TimeSpan.FromSeconds(2), delegate { m_overheadEmoteQueue.Enqueue(@"You wake up to find yourself in the King's Jail."); });
            else
                Timer.DelayCall(TimeSpan.FromSeconds(2), delegate { m_overheadEmoteQueue.Enqueue(@"You wake up to the familiar walls of your jail cell."); });


            MakeDoorUnlockpickable();

            m_End = m_End_orig;

            m_Jailed.MoveToWorld(YewJailControl.m_JailLocations[m_JailNo - 1], Map.Felucca);

            m_overheadEmoteQueueTime = DateTime.UtcNow + TimeSpan.FromSeconds(3);
        }

        public void PostBail(TimeSpan hours)
        {
            m_End -= hours;
        }
        
        public override void OnAfterDelete()
        {
            if (YewJailControl.m_YewJailDoorItems.ContainsKey(m_JailNo))
                m_JailDoor = YewJailControl.m_YewJailDoorItems[m_JailNo];

            if (m_JailDoor == null)
            {
                YewJailDoor jailDoor;
                Point3D loc = YewJailControl.m_doorSpawnLocs[m_JailNo - 1];
                if (loc.Y == 769)
                    jailDoor = new YewJailDoor(DoorFacing.WestCW, m_JailNo);
                else
                    jailDoor = new YewJailDoor(DoorFacing.WestCCW, m_JailNo);

                jailDoor.Locked = true;
                jailDoor.MoveToWorld(loc, Map.Felucca);

                YewJailControl.m_YewJailDoorItems.Add(m_JailNo, jailDoor);
            }
            
            //Delete Guards
            DeleteGuards();
            if (m_JailDoor != null)
            {
                m_JailDoor.Locked = false;
                m_JailDoor.Open = true;
            }

            //Unregister the jail cell
            YewJailControl.UnregisterJailCell(m_JailNo);

            //Set PlayerFlag
            ((PlayerMobile)m_Jailed).YewJailed = false;
            

            if (m_Timer != null)
                m_Timer.Stop();
            if (m_End == m_End_orig)
                m_Jailed.LocalOverheadMessage(MessageType.Regular, m_Jailed.EmoteHue, true, @"Your jail sentence has expired. You are now free to leave.");
            else
                m_Jailed.LocalOverheadMessage(MessageType.Regular, m_Jailed.EmoteHue, true, @"The guards have given up. Consider yourself lucky, this time!");

            base.OnAfterDelete();
        }

        public YewJailItem(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((Mobile)m_Jailed);
            writer.Write((TimeSpan)m_End);
            writer.Write((int)m_JailNo);
            writer.Write((YewJailDoor)m_JailDoor);
            writer.Write((bool)spawnedHelper);
            writer.Write((DateTime)offline_release_date);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_Jailed = reader.ReadMobile();
            m_End = reader.ReadTimeSpan();
            m_JailNo = reader.ReadInt();
            m_JailDoor = reader.ReadItem() as YewJailDoor;
            spawnedHelper = reader.ReadBool();
            offline_release_date = reader.ReadDateTime();

            double skillTinkering = m_Jailed.Skills.Tinkering.Base;
            double skillBegging = m_Jailed.Skills.Begging.Base;
            double skillStealing = m_Jailed.Skills.Stealing.Base;

            if (skillTinkering > 80 || skillBegging > 80 || skillStealing > 80)
            {
                m_Timer = new BombQuestInternalTimer(this);
                m_Timer.Start();
            }
            else
            {
                m_Timer = new LockpickQuestInternalTimer(this);
                m_Timer.Start();
            }
        }
        private class BombQuestInternalTimer : Timer
        {
            private YewJailItem m_Item;
            private bool spawned = false;
            private TimeSpan m_questSpawnTime;

            public BombQuestInternalTimer(YewJailItem item)
                : base(TimeSpan.Zero, TimeSpan.FromSeconds(1))
            {
                spawned = item.spawnedHelper;
                m_Item = item;
                Priority = TimerPriority.FiftyMS;
                m_questSpawnTime = (((PlayerMobile)m_Item.m_Jailed).GameTime + TimeSpan.FromMinutes(Utility.RandomMinMax(1, 20))); //TIME TO SPAWN QUEST
            }

            protected override void OnTick()
            {
                
                if (m_Item.Deleted)
                    return;

                if (m_Item.m_overheadEmoteQueue.Count > 0 && DateTime.UtcNow > m_Item.m_overheadEmoteQueueTime)
                    m_Item.m_Jailed.LocalOverheadMessage(MessageType.Emote, m_Item.m_Jailed.EmoteHue, true, m_Item.m_overheadEmoteQueue.Dequeue() as String);
                
                if (DateTime.UtcNow > m_Item.offline_release_date)
                {
                    //PLAYER LOGGED OUT FOR 5 DAYS WHILE IN JAIL
                    ((PlayerMobile)m_Item.m_Jailed).MoveToWorld(new Point3D(291, 771, 20), Map.Felucca);
                    m_Item.Delete();
                    Stop();
                }

                if (((PlayerMobile)m_Item.m_Jailed).GameTime > m_Item.m_End)
                {
                    //JAIL SENTENCE IS UP
                    m_Item.Delete();
                    Stop();
                }
                else
                {

                    if (((PlayerMobile)m_Item.m_Jailed).GameTime > m_questSpawnTime && !spawned)
                    {
                        spawned = true;
                        m_Item.spawnedHelper = true;
                        YewJailBombHelper helper = new YewJailBombHelper(m_Item);
                        helper.MoveToWorld(YewJailControl.m_BombHelperLocations[m_Item.m_JailNo-1], Map.Felucca);
                        helper.Direction = Direction.Down;
                        helper.Frozen = true;
                        Item hideableWire = new HideableSilverWire();
                        hideableWire.MoveToWorld(YewJailControl.m_JailLocations[m_Item.m_JailNo - 1], Map.Felucca);
                        ((IHideable)hideableWire).Findable = true;
                        ((IHideable)hideableWire).HideLevel = 0;
                        ((IHideable)hideableWire).IsVisible = false;
                    }
                }
            }
        }

        private class LockpickQuestInternalTimer : Timer
        {
            private YewJailItem m_Item;

            private bool spawned = false;

            //private static int JailState;

            private TimeSpan m_questSpawnTime;

            public LockpickQuestInternalTimer(YewJailItem item) : base(TimeSpan.Zero, TimeSpan.FromSeconds(1))
            {
                spawned = item.spawnedHelper;
                m_Item = item;
                Priority = TimerPriority.FiftyMS;
                m_questSpawnTime = (((PlayerMobile)m_Item.m_Jailed).GameTime + TimeSpan.FromMinutes(Utility.RandomMinMax(1, 20))); //TIME TO SPAWN QUEST
            }

            protected override void OnTick()
            {
                if (m_Item.Deleted)
                    return;

                if (m_Item.m_overheadEmoteQueue.Count > 0 && DateTime.UtcNow > m_Item.m_overheadEmoteQueueTime)
                    m_Item.m_Jailed.LocalOverheadMessage(MessageType.Emote, m_Item.m_Jailed.EmoteHue, true, m_Item.m_overheadEmoteQueue.Dequeue() as String);

                if (DateTime.UtcNow > m_Item.offline_release_date)
                {
                    //PLAYER LOGGED OUT FOR 5 DAYS WHILE IN JAIL
                    ((PlayerMobile)m_Item.m_Jailed).MoveToWorld(new Point3D(291, 771, 20), Map.Felucca);
                    m_Item.Delete();
                    Stop();
                }

                if (((PlayerMobile)m_Item.m_Jailed).GameTime > m_Item.m_End)
                {
                    //JAIL SENTENCE IS UP
                    m_Item.Delete();
                    Stop();
                }
                else
                {
                    if (((PlayerMobile)m_Item.m_Jailed).GameTime > m_questSpawnTime && !spawned)
                    {
                        m_Item.MakeDoorLockpickable();
                        spawned = true;
                        m_Item.spawnedHelper = true;
                        YewJailEscapeHelper helper = new YewJailEscapeHelper(m_Item);
                        if (m_Item.m_JailNo < 10)
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
                            helper.jailNo = m_Item.m_JailNo;
                            helper.coordinates.Add(YewJailControl.m_HelperFinalLoc[helper.jailNo-1]);
                            if (helper.jailNo < 6 || (helper.jailNo > 10 && helper.jailNo < 16))
                                helper.finalDirection = Direction.North;
                            else
                                helper.finalDirection = Direction.South;
                            helper.coordinates.Add(new Point3D(289, 771, 20));
                            helper.coordinates.Add(YewJailControl.m_HelperSpawnLoc2);
                            helper.coordinates.Add(YewJailControl.m_HelperSpawnLoc2);
                        }
                    }
                }
            }
        }

        private class DaemonQuestInternalTimer : Timer
        {
            private YewJailItem m_Item;
            private bool spawned = false;
            private TimeSpan m_questSpawnTime;

            public DaemonQuestInternalTimer(YewJailItem item)
                : base(TimeSpan.Zero, TimeSpan.FromSeconds(1))
            {
                spawned = item.spawnedHelper;
                m_Item = item;
                Priority = TimerPriority.FiftyMS;
                m_questSpawnTime = (((PlayerMobile)m_Item.m_Jailed).GameTime + TimeSpan.FromMinutes(Utility.RandomMinMax(1, 20))); //TIME TO SPAWN QUEST
            }

            protected override void OnTick()
            {

                if (m_Item.Deleted)
                    return;

                if (m_Item.m_overheadEmoteQueue.Count > 0 && DateTime.UtcNow > m_Item.m_overheadEmoteQueueTime)
                    m_Item.m_Jailed.LocalOverheadMessage(MessageType.Emote, m_Item.m_Jailed.EmoteHue, true, m_Item.m_overheadEmoteQueue.Dequeue() as String);

                if (DateTime.UtcNow > m_Item.offline_release_date)
                {
                    //PLAYER LOGGED OUT FOR 5 DAYS WHILE IN JAIL
                    ((PlayerMobile)m_Item.m_Jailed).MoveToWorld(new Point3D(291, 771, 20), Map.Felucca);
                    m_Item.Delete();
                    Stop();
                }

                if (((PlayerMobile)m_Item.m_Jailed).GameTime > m_Item.m_End)
                {
                    //JAIL SENTENCE IS UP
                    m_Item.Delete();
                    Stop();
                }
                else
                {

                    if (((PlayerMobile)m_Item.m_Jailed).GameTime > m_questSpawnTime && !spawned)
                    {
                        spawned = true;
                        m_Item.spawnedHelper = true;
                        DaemonQuestStarter qs = new DaemonQuestStarter();
                        qs.MoveToWorld(YewJailControl.m_BombHelperLocations[m_Item.m_JailNo-1], Map.Felucca);
                    }
                }
            }
        }
    }
}