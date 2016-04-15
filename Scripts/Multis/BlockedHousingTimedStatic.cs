using System;

namespace Server.Items
{
    public class BlockedHousingTimedStatic : TimedStatic
    {
        public Mobile m_Owner;

        [Constructable]
        public BlockedHousingTimedStatic() : base()
        {
        }

        [Constructable]
        public BlockedHousingTimedStatic(int itemID) : base(itemID)
        {           
            Movable = false;

            new InternalTimer(this).Start();
        }

        [Constructable]
        public BlockedHousingTimedStatic(int itemID, double decayTime): base(itemID)
        {
            Movable = false;

            Timer customTimer = new InternalTimer(this);

            customTimer.Delay = TimeSpan.FromSeconds(decayTime);
            customTimer.Start();
        }

        public override bool CanBeSeenBy(Mobile from)
        {
            if (from == m_Owner || from.AccessLevel > AccessLevel.Player)
                return true;

            return false;
        }

        public BlockedHousingTimedStatic(Serial serial): base(serial)
        {
            new InternalTimer(this).Start();
        }

        private class InternalTimer : Timer
        {
            private readonly Item m_TimedStatic;

            public InternalTimer(Item timedStatic) : base(TimeSpan.FromSeconds(3.0))
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