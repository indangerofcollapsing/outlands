using System;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using System.Collections;
using System.Collections.Generic;

namespace Server.Items
{
	public class RaidLavaTile : LavaTile
	{
        DateTime m_End;
        Timer m_Timer;

        [Constructable]
        public RaidLavaTile()
            : this(TimeSpan.FromMinutes(2))
        {
            Light = LightType.Circle300;
        }

        [Constructable]
        public RaidLavaTile(TimeSpan lifeTime)
            : base()
		{
            m_End = DateTime.Now + lifeTime;
            m_Timer = new TileTimer(this);
            m_Timer.Start();
		}

        public RaidLavaTile(Serial serial)
            : base(serial)
		{
		}

        public override bool OnMoveOver(Mobile m)
        {
            if (m == null || !m.Alive || m.IsDeadBondedPet)
                return true;

            if (m is ChaosDragon)
                m.Heal(100);
            else if (m is RaidFireElemental || m is OrderDragon) // || m is RaidDrake
            {
                //immune
            }
            else
            {
                AOS.Damage(m, 50, true, 0, 0, 50, 0, 0);
            }

            return base.OnMoveOver(m);
        }

        private void Damage(Mobile m)
        {


        }
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

            TimeSpan timeLeft = TimeSpan.Zero;

            if (m_End != null)
                timeLeft = m_End - DateTime.Now;

            writer.Write(timeLeft);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

            TimeSpan timeLeft = reader.ReadTimeSpan();
            if (timeLeft != null && timeLeft != TimeSpan.Zero)
            {
                m_End = DateTime.Now + timeLeft;
                m_Timer = new TileTimer(this);
                m_Timer.Start();
            }
		}

        private class TileTimer : Timer
        {
            private RaidLavaTile m_Tile;
            private Queue m_Queue = new Queue();

            public TileTimer(RaidLavaTile tile)
                : base(TimeSpan.Zero, TimeSpan.FromSeconds(1))
            {
                m_Tile = tile;
            }

            protected override void OnTick()
            {
                if (DateTime.Now > m_Tile.m_End)
                {
                    m_Tile.Delete();
                    Stop();
                    return;
                }

                IPooledEnumerable eable = m_Tile.GetMobilesInRange(0);

                foreach (Mobile m in eable)
				{
                    m_Queue.Enqueue( m );
				}

                eable.Free();

                while (m_Queue.Count > 0)
                {
                    m_Tile.OnMoveOver((Mobile)m_Queue.Dequeue());
                }
            }
        }
	}
}
