/***************************************************************************
 *                               StickyBar.cs
 *                            ------------------
 *   begin                : November 2010
 *   author               : Sean Stavropoulos
 *   email                : sean.stavro@gmail.com
 *
 *
 ***************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Network;

namespace Server
{
    public class StickyBar
    {
        private static readonly bool _enabled = true;
        public static bool Enabled { get { return _enabled; } }

        private static List<StickyBar> _list = new List<StickyBar>();
        private static InternalTimer _timer = new InternalTimer();
        private static readonly TimeSpan _stickyBarDelay = TimeSpan.FromSeconds(2); //sticky bar buffer time

        private DateTime _time;
        private Mobile _m1;
        private Mobile _m2;

        public StickyBar(Mobile m1, Mobile m2)
        {
            _m1 = m1;
            _m2 = m2;
        }

        public static void Queue(Mobile m1, Mobile m2)
        {
            if (!_enabled || m1 == null || m2 == null)
                return;

            StickyBar s = Find(m1, m2);
            s._time = DateTime.UtcNow + _stickyBarDelay;
            _list.Add(s);

            if (_list.Count == 1)
                _timer.Start();
        }

        private static void EraseStickyBar(Mobile target, Mobile toRemove)
        {
            if (target == null || toRemove == null || target.NetState == null || (Utility.InUpdateRange( target, toRemove)  && target.CanSee(toRemove)))
                return;

            NetState ns = target.NetState;
            Point3D loc = target.Location;

            ns.Send(new MobileIncomingHAX(toRemove, loc));

            Timer.DelayCall(TimeSpan.FromMilliseconds(50),
                delegate 
                {
                    if (target == null || toRemove == null || ns == null || (Utility.InUpdateRange(target, toRemove) && target.CanSee(toRemove)))
                        return;

                    ns.Send(new DeathAnimation(toRemove, null));
                    ns.Send(toRemove.RemovePacket);
                });
        }

        private static StickyBar Find(Mobile m1, Mobile m2)
        {
            foreach (StickyBar s in _list)
                if ((s._m1 == m1 || s._m1 == m2) && (s._m2 == m1 || s._m2 == m2))
                {
                    _list.Remove(s);
                    return s;
                }

            return new StickyBar(m1, m2);
        }

        private class InternalTimer : Timer
        {
            public InternalTimer()
                : base(TimeSpan.Zero, TimeSpan.FromMilliseconds(500))
            {
                Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                if (_list.Count == 0)
                {
                    Stop();
                    return;
                }

                while (_list.Count > 0 && DateTime.UtcNow > _list[0]._time)
                {
                    StickyBar s = _list[0];
                    _list.RemoveAt(0);
                    EraseStickyBar(s._m1, s._m2);
                    EraseStickyBar(s._m2, s._m1);
                }
            }
        }
    }

    public sealed class MobileIncomingHAX : Packet
    {
        public MobileIncomingHAX(Mobile beheld, Point3D loc)
            : base(0x78)
        {
            this.EnsureCapacity(23);
            m_Stream.Write((int)beheld.Serial);
            m_Stream.Write((short)0);
            m_Stream.Write((short)loc.X);
            m_Stream.Write((short)loc.Y);
            m_Stream.Write((sbyte)loc.Z);
            m_Stream.Write((byte)0);//1
            m_Stream.Write((short)0);//1
            m_Stream.Write((byte)0);
            m_Stream.Write((byte)0);//4;
            m_Stream.Write((int)0); // terminate
        }
    }
}