using System;
using Server;
using System.Collections;
using System.Collections.Generic;
using Server.Network;

namespace Server.Items
{
    public class MongbatMead : BeverageBottle
    {
        public override string DefaultName { get { return "Mongbat Mead"; } }
        public static List<MongbatMeadDrunkard> Drunks = new List<MongbatMeadDrunkard>();
        private static InternalTimer m_Timer;
        public static Mobile m_MongbatPlaceholder;

        public class MongbatMeadDrunkard
        {
            public Mobile Mobile { get; set; }
            public DateTime End { get; set; }

            public MongbatMeadDrunkard(Mobile from, DateTime end)
            {
                Mobile = from;
                End = end;
            }

            public static MongbatMeadDrunkard Find(Mobile from)
            {
                foreach (MongbatMeadDrunkard drunk in Drunks)
                    if (from == drunk.Mobile)
                        return drunk;

				var newDrunk = new MongbatMeadDrunkard(from, DateTime.UtcNow);

                 Drunks.Add(newDrunk);

                if (Drunks.Count == 1)
                {
                    if (m_Timer != null) {
                        m_Timer.Stop();
                    } else {
                        m_Timer = new InternalTimer();
                        m_Timer.Start();
                    }

                }

                return newDrunk;
            }

            public static void Remove(MongbatMeadDrunkard drunk)
            {
                if (Drunks.Contains(drunk)) {
                    Drunks.Remove(drunk);

                    if (Drunks.Count == 0) {
                        if (m_Timer != null) {
                            m_Timer.Stop();
                            m_Timer = null;
                        }
                    }
                }
            }

            public void SendMongbat()
            {
                if (Mobile == null || Mobile.NetState == null)
                    return;

                if (m_MongbatPlaceholder == null)
                    m_MongbatPlaceholder = new MongbatIllusionPlaceholder();

                Mobile.Send(new MongbatIllusionPacket(Mobile));
            }
        }

        [Constructable]
        public MongbatMead()
            : base(BeverageType.Ale)
        {
            Hue = 1164;
        }

        public MongbatMead(Serial serial)
            : base(serial)
        {
        }

        public override void OnDrink(Mobile from)
        {
            var state = MongbatMeadDrunkard.Find(from);
            state.End += TimeSpan.FromMinutes(2);
        }
        
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public class InternalTimer : Timer
        {
            public InternalTimer()
                : base(TimeSpan.Zero, TimeSpan.FromSeconds(10))
            {
            }

            protected override void OnTick()
            {
				DateTime now = DateTime.UtcNow;

                for (int i = 0; i < Drunks.Count; i++) {
                    var drunk = Drunks[i];
                    if (drunk.End < now) {
                        MongbatMeadDrunkard.Remove(drunk);
                        --i; continue;
                    }
                    else if (Utility.RandomBool()) {
                        drunk.SendMongbat();
                    }
                }
            }
        }

        public sealed class MongbatIllusionPacket : Packet
        {
            public MongbatIllusionPacket(Mobile drunk)
                : base(0x78)
            {
                Console.WriteLine("sent");
                Point3D illusionLoc = new Point3D(drunk.X + Utility.RandomMinMax(-5, 5), drunk.Y + Utility.Random(-5, 5), drunk.Z);
                this.EnsureCapacity(23);
                m_Stream.Write((int)m_MongbatPlaceholder.Serial);
                m_Stream.Write((short)39);
                m_Stream.Write((short)illusionLoc.X);
                m_Stream.Write((short)illusionLoc.Y);
                m_Stream.Write((sbyte)illusionLoc.Z);
                m_Stream.Write((byte)drunk.Direction);
                m_Stream.Write((short)Utility.RandomNondyedHue());//1
                m_Stream.Write((byte)0);
                m_Stream.Write((byte)0);//4;
                m_Stream.Write((int)0); // terminate
            }
        }

        public class MongbatIllusionPlaceholder : Mobile
        {
            public MongbatIllusionPlaceholder()
            {
            }

            public MongbatIllusionPlaceholder(Serial serial)
                : base(serial)
            {
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);

                if (MongbatMead.m_MongbatPlaceholder != null)
                {
                    Timer.DelayCall(TimeSpan.FromTicks(1), Delete);
                    return;
                }

                MongbatMead.m_MongbatPlaceholder = this;
            }
        }
    }
}