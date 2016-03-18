using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Custom.YewJail
{
    public class DaemonQuestStarter : Item
    {
        Timer m_Timer;
        string questText = "Hey! Come on! I know a way out!";

        public DaemonQuestStarter()
            : base(0x1b71)
        {
            Movable = false;
            m_Timer = Timer.DelayCall(TimeSpan.FromMinutes(3), Delete);
            Start();
        }

        public void Start()
        {
            Timer.DelayCall(TimeSpan.FromSeconds(1), Slice);
        }

        public void Slice()
        {
            PublicOverheadMessage(Network.MessageType.Regular, 0, false, questText);
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (!(m is PlayerMobile))
                return false;

            DaemonCavern.Begin(m);

            Delete();

            return false;
        }

        public override void OnAfterDelete()
        {
            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }

            base.OnAfterDelete();
        }

        public DaemonQuestStarter(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();

            m_Timer = Timer.DelayCall(TimeSpan.FromMinutes(3), Delete);
        }
    }
}
