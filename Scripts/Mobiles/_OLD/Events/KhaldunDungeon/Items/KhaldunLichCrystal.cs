using System;
using Server.Items;
using System.Collections;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;
using Server.Mobiles;
using Server.Targeting;

namespace Server
{
    public class KhaldunLichCrystal : Item
    {
        public InternalTimer m_Timer;
        
        public virtual int MainHue { get { return 2614; } }
        public virtual int AltHue { get { return 2500; } }

        public TimeSpan HueChangeDelay = TimeSpan.FromSeconds(1);

        [Constructable]
        public KhaldunLichCrystal(): base(730)
        {
            Name = "khaldun crystal";

            Hue = MainHue;

            m_Timer = new InternalTimer(this);
            m_Timer.Start();
        }

        public class InternalTimer : Timer
        {
            public KhaldunLichCrystal m_KhaldunLichCrystal;

            public InternalTimer(KhaldunLichCrystal khaldunLichCrystal): base(khaldunLichCrystal.HueChangeDelay, khaldunLichCrystal.HueChangeDelay)
            {
                m_KhaldunLichCrystal = khaldunLichCrystal;

                Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                if (m_KhaldunLichCrystal == null)
                {
                    Stop();
                    return;
                }

                if (m_KhaldunLichCrystal.Deleted)
                {
                    Stop();
                    return;
                }

                if (m_KhaldunLichCrystal.Hue == m_KhaldunLichCrystal.MainHue)
                    m_KhaldunLichCrystal.Hue = m_KhaldunLichCrystal.AltHue;
                else
                    m_KhaldunLichCrystal.Hue = m_KhaldunLichCrystal.MainHue;
            }
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }

            if (!Deleted)
                Delete();
        }
        
        public KhaldunLichCrystal(Serial serial): base(serial)
        {
        }
        
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //---------

            m_Timer = new InternalTimer(this);
            m_Timer.Start();
        }
    }
}