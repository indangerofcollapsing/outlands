using System;
using Server;
using System.Collections.Generic;
using Server.Network;
using Server.Prompts;
using Server.Multis;
using Server.Mobiles;
using Server.Targeting;
using Server.Gumps;
using Server.Spells;
using Server.Items;

namespace Server.Custom
{
    public class MysteryLocation : Item
    {
        private AncientMystery.MysteryType m_MysteryType;
        [CommandProperty(AccessLevel.GameMaster)]
        public AncientMystery.MysteryType MysteryType
        {
            get { return m_MysteryType; }
            set {}
        }

        private Mobile m_Owner;
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner
        {
            get { return m_Owner; }
            set { m_Owner = value; }
        }

        private DateTime m_Expiration;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime Expiration
        {
            get { return m_Expiration; }
            set { m_Expiration = value; }
        }

        public List<Item> m_Items = new List<Item>();
        public List<Mobile> m_Mobiles = new List<Mobile>();
        
        public TimeSpan Duration = TimeSpan.FromHours(4);       

        private Timer m_Timer;      

        [Constructable]
        public MysteryLocation(AncientMystery.MysteryType mysteryType, Mobile owner): base(3633)
        {
            m_MysteryType = mysteryType;
            m_Owner = owner;

            Name = "a mystery location spot";

            Visible = false;

            m_Expiration = DateTime.UtcNow + Duration;

            m_Timer = new InternalTimer(this);
            m_Timer.Start();

            Timer.DelayCall(TimeSpan.Zero, new TimerCallback(AddComponents));
        }

        public virtual void AddComponents()
        {
            if (Deleted)
                return;

            Movable = false;      
        }        

        public virtual void GroupItem(Item item, int hue, int xOffset, int yOffset, int zOffset, string name)
        {
            if (item == null)
                return;

            item.Name = name;
            item.Hue = hue;
            item.Movable = false;
                
            m_Items.Add(item);

            item.MoveToWorld(new Point3D(X + xOffset, Y + yOffset, Z + zOffset), Map);            
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);
        }     

        private class InternalTimer : Timer
        {
            private MysteryLocation m_MysteryLocation;

            public InternalTimer(MysteryLocation MysteryLocation): base(TimeSpan.Zero, TimeSpan.FromMinutes(1))
            {
                Priority = TimerPriority.OneMinute;

                m_MysteryLocation = MysteryLocation;
            }

            protected override void OnTick()
            {
                if (m_MysteryLocation == null)
                {
                    Stop();
                    return;
                }

                if (m_MysteryLocation.Deleted)
                {
                    Stop();
                    return;
                }

                if (m_MysteryLocation.m_Expiration < DateTime.UtcNow)
                {
                    Stop();

                    m_MysteryLocation.Delete();
                }
            }
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();
            
            for (int a = 0; a < m_Items.Count; ++a)
            {
                if (m_Items[a] != null)
                {
                    if (!m_Items[a].Deleted)
                        m_Items[a].Delete();
                }
            }

            for (int a = 0; a < m_Mobiles.Count; ++a)
            {
                if (m_Mobiles[a] != null)
                {
                    if (!m_Mobiles[a].Deleted)
                        m_Mobiles[a].Delete();
                }
            }

            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }

            Delete();
        }

        public MysteryLocation(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
            
            //Version 0
            writer.Write((int)m_MysteryType);
            writer.Write(m_Owner);
            writer.Write(m_Expiration);

            writer.Write((int)m_Items.Count);
            foreach (Item item in m_Items)
            {
                writer.Write(item);
            }

            writer.Write((int)m_Mobiles.Count);
            foreach (Mobile mobile in m_Mobiles)
            {
                writer.Write(mobile);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            m_MysteryType = (AncientMystery.MysteryType)reader.ReadInt();
            m_Owner = reader.ReadMobile();

            m_Expiration = reader.ReadDateTime();

            int itemsCount = reader.ReadInt();
            for (int i = 0; i < itemsCount; ++i)
            {
                m_Items.Add(reader.ReadItem());
            }

            int mobileCount = reader.ReadInt();
            for (int i = 0; i < mobileCount; ++i)
            {
                m_Mobiles.Add(reader.ReadMobile());
            }

            //------

            m_Timer = new InternalTimer(this);
            m_Timer.Start();
        }
    }
}