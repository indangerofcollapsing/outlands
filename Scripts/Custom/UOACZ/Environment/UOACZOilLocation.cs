using System;
using Server;
using Server.Mobiles;
using System.Collections;
using Server.Achievements;

namespace Server.Items
{
    public class UOACZOilLocation : Item
    {
        public DateTime Expiration = DateTime.MaxValue;
        public TimeSpan Duration = TimeSpan.FromMinutes(30);

        public bool Burning = false;

        private Timer m_Timer;

        [Constructable]
        public UOACZOilLocation(): base(Utility.RandomList(4650, 4651, 4653, 4654, 4655))		
		{
            Name = "oil";

            Hue = 2515;
            Movable = false;           

            Expiration = DateTime.UtcNow + Duration;

            m_Timer = new InternalTimer(this);
            m_Timer.Start();
		}

        public UOACZOilLocation(Serial serial): base(serial)
        {   
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            LabelTo(from, "(can be ignited with a torch)");
        }

        public void Ignite(Mobile from)
        {
            if (Burning)
                return;

            Burning = true;

            Effects.PlaySound(Location, Map, 0x5CF);
            Effects.SendLocationParticles(EffectItem.Create(Location, Map, TimeSpan.FromSeconds(1.0)), 0x3709, 10, 30, 0, 0, 5052, 0);

            Queue m_Queue = new Queue();  
           
            IPooledEnumerable nearbyItems = Map.GetItemsInRange(Location, 1);

            foreach (Item item in nearbyItems)
            {
                if (item is UOACZOilLocation)
                    m_Queue.Enqueue(item);
            }

            nearbyItems.Free();

            while (m_Queue.Count > 0)
            {
                Item item = (Item)m_Queue.Dequeue();

                if (item is UOACZOilLocation)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(.2), delegate
                    {
                        if (item == null) return;
                        if (item.Deleted) return;

                        UOACZOilLocation oilLocation = item as UOACZOilLocation;

                        oilLocation.Ignite(from);
                    });
                }
            }

            Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
            {
                if (Deleted)
                    return;

                UOACZFirefield firefield = new UOACZFirefield(from);
                firefield.MoveToWorld(Location, Map);

                Delete();
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
        }
        
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); //version

            //Version 0
            writer.Write(Expiration);

            //Version 1
            writer.Write(Burning);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();  
          
            //Version 0
            if (version >= 0)
            {
                Expiration = reader.ReadDateTime();
            }

            //Version 1
            if (version >= 1)
            {
                Burning = reader.ReadBool();
            }

            //------

            m_Timer = new InternalTimer(this);
            m_Timer.Start();
        }

        private class InternalTimer : Timer
        {
            private UOACZOilLocation m_OilLocation;
            private static Queue m_Queue = new Queue();

            public InternalTimer(UOACZOilLocation oilLocation): base(TimeSpan.Zero, TimeSpan.FromMinutes(1))
            {
                m_OilLocation = oilLocation;
                Priority = TimerPriority.OneMinute;               
            }

            protected override void OnTick()
            {
                if (m_OilLocation == null)
                    Stop();

                if (m_OilLocation.Deleted || DateTime.UtcNow >= m_OilLocation.Expiration)
                {
                    m_OilLocation.Delete();
                    Stop();
                }
            }
        }
    }
}