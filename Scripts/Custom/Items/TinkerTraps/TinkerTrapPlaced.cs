using System;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Network;
using Server.Misc;
using Server.Spells;
using Server.Multis;

namespace Server.Custom
{
    public class TinkerTrapPlaced : Item
    {
        public List<TrapTriggerTile> m_TrapTriggerTiles = new List<TrapTriggerTile>();
                
        public TinkerTrap m_TinkerTrap;

        public string TrapName = "a tinker trap";
        public int TrapItemId = 6173;
        public int TrapHue = 2500;
        public int TrapTextHue = 2500;
        public int TriggerRadius = 1;
        public int EffectRadius = 1;
        public bool SingleLine = false;
        public Direction LineFacing = Direction.North;

        public bool trapTriggerTilesPlaced = false;

        public Mobile Owner = null;

        public DateTime TrapReady = DateTime.MaxValue;

        public DateTime Expiration = DateTime.MaxValue;

        public double TriggerDelay = 0; 
        public DateTime TriggerDelayActivation = DateTime.MaxValue;

        public bool IsResolving = false;

        private Timer m_Timer;

        [Constructable]
        public TinkerTrapPlaced(): base(7963)
        {
            Movable = false;
            
            m_Timer = new InternalTimer(this);
            m_Timer.Start();                 
        }

        public TinkerTrapPlaced(Serial serial): base(serial)
        {
        }

        public void AddComponents()
        {
            if (this == null) return;
            if (Deleted) return;

            int triggerRadius = TriggerRadius;

            if (UOACZRegion.ContainsItem(this))
                triggerRadius += 1;

            int rows = (triggerRadius * 2) + 1;
            int columns = (triggerRadius * 2) + 1;
            
            if (SingleLine)
            {
                bool eastWest = true;

                if (LineFacing == Direction.West || LineFacing == Direction.East)
                    eastWest = false;

                if (eastWest)
                {
                    for (int a = 0; a < rows; a++)
                    {
                        GroupItem(new TrapTriggerTile(), (-1 * (triggerRadius)) + a, 0, 0);                        
                    }
                }

                else
                {
                    for (int b = 0; b < columns; b++)
                    {
                        GroupItem(new TrapTriggerTile(), 0, (-1 * (triggerRadius)) + b, 0);
                    }                    
                }
            }

            else
            {
                for (int a = 1; a < rows + 1; a++)
                {
                    for (int b = 1; b < columns + 1; b++)
                    {
                        GroupItem(new TrapTriggerTile(), (-1 * (triggerRadius + 1)) + a, (-1 * (triggerRadius + 1)) + b, 0);
                    }
                }
            }
            
            Effects.PlaySound(Location, Map, 0x3E6);

            PublicOverheadMessage(MessageType.Emote, 0, false, "*trap ready*");
            trapTriggerTilesPlaced = true;
        }

        public virtual void GroupItem(TrapTriggerTile trapTriggerTile, int xOffset, int yOffset, int zOffset)
        {
            Point3D location = new Point3D(Location.X + xOffset, Location.Y + yOffset, Location.Z + zOffset);

            bool canFit = SpellHelper.AdjustField(ref location, Map, 12, false);

            if (Map == null || !canFit)
                return;

            if (SpellHelper.CheckMulti(location, Map))
                return;

            if (BaseBoat.IsWaterTile(location, Map))
                return;

            trapTriggerTile.Name = TrapName;
            trapTriggerTile.ItemID = TrapItemId;
            trapTriggerTile.Hue = TrapHue;

            trapTriggerTile.m_TinkerTrapPlaced = this;

            trapTriggerTile.MoveToWorld(location, Map);

            m_TrapTriggerTiles.Add(trapTriggerTile);
        }

        private class InternalTimer : Timer
        {
            private TinkerTrapPlaced m_TinkerTrapPlaced;

            public InternalTimer(TinkerTrapPlaced tinkerTrapPlaced): base(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1.0))
            {
                Priority = TimerPriority.OneSecond;

                m_TinkerTrapPlaced = tinkerTrapPlaced;
            }

            protected override void OnTick()
            {
                if (m_TinkerTrapPlaced == null)
                {
                    Stop();
                    return;
                }

                if (m_TinkerTrapPlaced.Deleted)
                {
                    Stop();
                    return;
                }

                if (m_TinkerTrapPlaced.TrapReady <= DateTime.UtcNow && !m_TinkerTrapPlaced.trapTriggerTilesPlaced)
                {
                    m_TinkerTrapPlaced.AddComponents();

                    return;
                }

                if (m_TinkerTrapPlaced.Expiration <= DateTime.UtcNow)
                {
                    Stop();

                    m_TinkerTrapPlaced.Owner.SendMessage("Your trap has expired.");
                    m_TinkerTrapPlaced.Delete();                   

                    return;
                }
            }
        }

        public override void OnSingleClick(Mobile from)
        {
            int minutes;
            int seconds;

            string sTimeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, Expiration, true, true, true, true, true);

            bool trapReady = false;

            if (TrapReady <= DateTime.UtcNow)            
                trapReady = true;

            if (from.NetState != null)
            {
                LabelTo(from, Name);

                if (Owner != null)
                    LabelTo(from, "(placed by " + Owner.RawName + ")");

                if (trapReady)
                    LabelTo(from, "[expires in " + sTimeRemaining + "]");

                else
                    LabelTo(from, "[ready in " + sTimeRemaining + "]");
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            if (IsResolving)
                return;

            if (m_TinkerTrap != null)
            {
                if (Owner != from)
                {
                    from.SendMessage("You may only recover traps that you have placed yourself.");
                    return;
                }

                if (TrapReady > DateTime.UtcNow)
                {
                    from.SendMessage("You must wait to finish placing that trap before attempting to recover it.");
                    return;
                }

                Item item = (Item)Activator.CreateInstance(m_TinkerTrap.GetType());

                if (!from.AddToBackpack(item))
                {
                    from.SendMessage("You don't have enough room in your backpack to recover this trap. Please make room and try again.");
                    return;
                }

                from.SendMessage("You recover the trap.");
                Delete();
            }                
        }

        public virtual void Activate()
        {
            if (IsResolving)
                return;

            IsResolving = true;
            
            TriggerDelayActivation = DateTime.UtcNow + TimeSpan.FromSeconds(TriggerDelay);
            Expiration = TriggerDelayActivation + TimeSpan.FromSeconds(20);

            PublicOverheadMessage(MessageType.Emote, 0, false, "*trap triggered*");

            Timer.DelayCall(TimeSpan.FromSeconds(TriggerDelay), delegate
            {
                if (this == null) return;
                if (Deleted) return;

                IsResolving = false;

                if (m_TinkerTrap != null)
                    m_TinkerTrap.Resolve(this);
            });            
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }

            for (int a = 0; a < m_TrapTriggerTiles.Count; ++a)
                m_TrapTriggerTiles[a].Delete();

            if (m_TinkerTrap != null)
            {
                if (!m_TinkerTrap.Deleted)
                    m_TinkerTrap.Delete();
            }

            if (!Deleted)
                Delete();
        }
        
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version

            //Version 0
            writer.Write(TrapName);
            writer.Write(TrapItemId);
            writer.Write(TrapHue);
            writer.Write(TrapTextHue);
            writer.Write(TriggerRadius);
            writer.Write(EffectRadius);

            writer.Write(trapTriggerTilesPlaced);

            writer.Write(Owner);

            writer.Write(TrapReady);

            writer.Write(Expiration);

            writer.Write(TriggerDelay);
            writer.Write(TriggerDelayActivation);

            writer.Write(IsResolving);            

            writer.Write(m_TrapTriggerTiles.Count);
            foreach (Item item in m_TrapTriggerTiles)
            {
                writer.Write(item);
            }

            //Version 1
            writer.Write(SingleLine);
            writer.Write((int)LineFacing);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_TrapTriggerTiles = new List<TrapTriggerTile>();

            //Version 0
            if (version >= 0)
            {
                TrapName = reader.ReadString();
                TrapItemId = reader.ReadInt();
                TrapHue = reader.ReadInt();
                TrapTextHue = reader.ReadInt();
                TriggerRadius = reader.ReadInt();
                EffectRadius = reader.ReadInt();

                trapTriggerTilesPlaced = reader.ReadBool();

                Owner = reader.ReadMobile();

                TrapReady = reader.ReadDateTime();

                Expiration = reader.ReadDateTime();

                TriggerDelay = reader.ReadDouble();

                TriggerDelayActivation = reader.ReadDateTime();

                IsResolving = reader.ReadBool();                

                int itemsCount = reader.ReadInt();
                for (int i = 0; i < itemsCount; ++i)
                {
                    TrapTriggerTile trapTriggerTile = reader.ReadItem() as TrapTriggerTile;

                    if (trapTriggerTile != null)
                        m_TrapTriggerTiles.Add(trapTriggerTile);
                }
            }

            //Version 1
            if (version >= 1)
            {
                SingleLine = reader.ReadBool();
                LineFacing = (Direction)(reader.ReadInt());
            }

            m_Timer = new InternalTimer(this);
            m_Timer.Start();
        }
    }
}
