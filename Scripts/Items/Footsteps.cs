using System;
using Server;

namespace Server.Items
{
    public class Footsteps : Item
    {
        [Constructable]
        public Footsteps(Direction d)
            : this(Utility.RandomList(0x1E03, 0x1E04, 0x1E05, 0x1E06)) //Eventually change to match D 
        {
        }
        
        [Constructable]
        public Footsteps(int itemID)
            : base(itemID)
        {
            Movable = false;

            new InternalTimer(this).Start();
        }

        public Footsteps(Serial serial)
            : base(serial)
        {
            new InternalTimer(this).Start();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        private class InternalTimer : Timer
        {
            private Item m_Footsteps;

            public InternalTimer(Item footsteps)
                : base(TimeSpan.FromSeconds(15.0))
            {
                Priority = TimerPriority.OneSecond;

                m_Footsteps = footsteps;
            }

            protected override void OnTick()
            {
                m_Footsteps.Delete();
            }
        }
    }
}