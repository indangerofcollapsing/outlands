using System;

namespace Server.Items
{
    public class TimedStatic : Item
    {
        [Constructable]
        public TimedStatic() : this(0x104B)
        {
        }

        [Constructable]
        public TimedStatic(int itemID) : base(itemID)
        {           
            Movable = false;

            new InternalTimer(this).Start();
        }

        [Constructable]
        public TimedStatic(int itemID, double decayTime): base(itemID)
        {
            Movable = false;

            Timer customTimer = new InternalTimer(this);

            customTimer.Delay = TimeSpan.FromSeconds(decayTime);
            customTimer.Start();
        }

        public TimedStatic(Serial serial) : base(serial)
        {
            new InternalTimer(this).Start();
        }

        private class InternalTimer : Timer
        {
            private readonly Item m_TimedStatic;

            public InternalTimer(Item timedStatic) : base(TimeSpan.FromSeconds(5.0))
            {
                Priority = TimerPriority.OneSecond;
                m_TimedStatic = timedStatic;
            }

            protected override void OnTick()
            {
                m_TimedStatic.Delete();
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}