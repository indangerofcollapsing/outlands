using System;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;

namespace Server.Custom
{
    public class BreakableMimic : BreakableContainer
    {
        private Timer m_Timer;

        public int animationStep = 1;

        bool IsActive = false;

        public DateTime m_NextActivityAllowed;
        public TimeSpan NextActivityDelay = TimeSpan.FromSeconds(Utility.RandomMinMax(2, 10));

        public DateTime m_ActivityExpiration;
        public TimeSpan ActivityDuration = TimeSpan.FromSeconds(Utility.RandomMinMax(1, 6)); 

        [Constructable]
        public BreakableMimic(): base()
        {
            Name = "a mimic";

            ItemID = 16646;

            Visible = true;

            HitSound = 0x5EA;

            m_Timer = new InternalTimer(this);
            m_Timer.Start();
        }

        private class InternalTimer : Timer
        {
            private BreakableMimic m_BreakableMimic;

            public InternalTimer(BreakableMimic breakableMimic): base(TimeSpan.Zero, TimeSpan.FromMilliseconds(50))
            {
                Priority = TimerPriority.FiftyMS;

                m_BreakableMimic = breakableMimic;
            }

            protected override void OnTick()
            {
                if (m_BreakableMimic == null)
                {
                    Stop();
                    return;
                }

                if (m_BreakableMimic.Deleted)
                {
                    Stop();
                    return;
                }

                if (m_BreakableMimic.m_NextActivityAllowed > DateTime.UtcNow)
                    return;

                if (!m_BreakableMimic.IsActive)
                {
                    m_BreakableMimic.IsActive = true;
                    m_BreakableMimic.m_ActivityExpiration = DateTime.UtcNow + m_BreakableMimic.ActivityDuration;
                }

                if (m_BreakableMimic.animationStep > 7)
                {
                    m_BreakableMimic.animationStep = 1;

                    if (m_BreakableMimic.m_ActivityExpiration < DateTime.UtcNow)
                    {
                        m_BreakableMimic.m_NextActivityAllowed = DateTime.UtcNow + m_BreakableMimic.NextActivityDelay;
                        m_BreakableMimic.animationStep = 1;
                        m_BreakableMimic.IsActive = false;

                        return;
                    }
                }

                switch (m_BreakableMimic.animationStep)
                {
                    case 1:                        
                        m_BreakableMimic.ItemID = 16646;

                        Effects.PlaySound(m_BreakableMimic.Location, m_BreakableMimic.Map, 0x5EB);                       
                    break;

                    case 2: m_BreakableMimic.ItemID = 16647; break;
                    case 3: m_BreakableMimic.ItemID = 16648; break;
                    case 4: m_BreakableMimic.ItemID = 16649; break;
                    case 5: m_BreakableMimic.ItemID = 16648; break;
                    case 6: m_BreakableMimic.ItemID = 16647; break;
                    case 7:
                        m_BreakableMimic.ItemID = 16646;

                        IPooledEnumerable mobilesOnTile = m_BreakableMimic.Map.GetMobilesInRange(m_BreakableMimic.Location, 1);

                        Queue m_Queue = new Queue();

                        foreach (Mobile mobile in mobilesOnTile)
                        {
                            if (!mobile.CanBeDamaged() || !mobile.Alive || mobile.AccessLevel > AccessLevel.Player)
                                continue;

                            bool validTarget = true;
                           
                            BaseCreature bc_Target = mobile as BaseCreature;                            

                            if (bc_Target != null)
                            {
                                if (!(bc_Target.Controlled && bc_Target.ControlMaster is PlayerMobile))
                                    validTarget = false;
                            }

                            if (validTarget)
                                m_Queue.Enqueue(mobile);
                        }

                        mobilesOnTile.Free();

                        while (m_Queue.Count > 0)
                        {
                            Mobile mobile = (Mobile)m_Queue.Dequeue();

                            new Blood().MoveToWorld(mobile.Location, mobile.Map);

                            Blood blood = new Blood();
                            Point3D bloodLocation = new Point3D(mobile.Location.X + Utility.RandomList(-1, 1), mobile.Location.Y + Utility.RandomList(-1, 1), mobile.Location.Z);
                            blood.MoveToWorld(bloodLocation, m_BreakableMimic.Map);

                            double damage = Utility.RandomMinMax(20, 40);

                            if (mobile is BaseCreature)
                                damage *= 1.5;

                            Effects.PlaySound(m_BreakableMimic.Location, m_BreakableMimic.Map, 0x5E8); //0x63B //0x5D6
                            AOS.Damage(mobile, null, (int)Math.Round(damage), 100, 0, 0, 0, 0);                            
                        }
                    break;
                }

                m_BreakableMimic.animationStep++;
            }
        }

        public BreakableMimic(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            //writer.Write(HitSound);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            /*
            if (version >= 0)
            {
                HitSound = reader.ReadInt();
            }
            */

            m_Timer = new InternalTimer(this);
            m_Timer.Start();
        }
    }
}
