using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Items;
using Server.Targeting;

namespace Server.Custom.Events.KhaldunDungeon.Items
{
    public enum CrystalType
    {
        GraveDust,
        Noxious,
        Bloody,
        Murky
    }

    public class Crystal : Item
    {
        public CrystalType CrystalType { get; set; }
        public DateTime DecaysAt { get; set; }

        private DecayTimer Timer;

        public Crystal(CrystalType type)
            : base(7964)
        {
            CrystalType = type;
            DecaysAt = DateTime.UtcNow + TimeSpan.FromDays(7);
            Timer = new DecayTimer(this);
            Timer.Start();
        }

        public Crystal(Serial serial) : base(serial) { }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
            int days = (DecaysAt - DateTime.UtcNow).Days;
            int hours = (DecaysAt - DateTime.UtcNow).Hours;
            int minutes = (DecaysAt - DateTime.UtcNow).Minutes;

            from.SendMessage(string.Format("This crystal decays in {0} days, {1} hours and {2} minutes.", days, hours, minutes));
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);
            if (!IsChildOf(from.Backpack))
                from.SendMessage("You have to use it in your backpack");
            else
            {
                from.Target = new CombineTarget(this);
            }
        }

        private class CombineTarget : Target
        {
            private Crystal m_Crystal;

            public CombineTarget(Crystal item) : base(12, false, TargetFlags.None)
            {
                m_Crystal = item;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Crystal == null || m_Crystal.Deleted)
                    return;

                if (targeted is CrystalHolder)
                {
                    CrystalHolder holder = targeted as CrystalHolder;
                    if (holder.CanHold(m_Crystal))
                    {
                        holder.Amount++;
                        m_Crystal.Delete();
                    }
                    else
                        from.SendMessage("You cannot combine these objects.");
                }
                else
                    from.SendMessage("You cannot combine those objects.");

            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
            writer.Write((int)CrystalType);
            writer.Write(DecaysAt);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            CrystalType = (CrystalType)reader.ReadInt();
            DecaysAt = reader.ReadDateTime();

            Timer = new DecayTimer(this);
            Timer.Start();
        }

        private class DecayTimer : Timer
        {
            private Crystal m_Crystal;

            public DecayTimer(Crystal crystal)
                : base(TimeSpan.Zero, TimeSpan.FromMinutes(1))
            {
                Priority = TimerPriority.OneMinute;
                m_Crystal = crystal;
            }

            protected override void OnTick()
            {
                base.OnTick();
                if (m_Crystal == null || m_Crystal.Deleted)
                {
                    Stop();
                    return;
                }

                if (DateTime.UtcNow > m_Crystal.DecaysAt)
                {
                    m_Crystal.Delete();
                    Stop();
                }
            }
        }
    }

    public class GraveDustCrystal : Crystal
    {
        public override string DefaultName { get { return "a grave dust crystal"; } }

        [Constructable]
        public GraveDustCrystal()
            : base(CrystalType.GraveDust)
        {
            Hue = 2500;
        }
        public GraveDustCrystal(Serial serial) : base(serial) { }

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

    public class NoxiousCrystal : Crystal
    {
        public override string DefaultName { get { return "a noxious crystal"; } }

        [Constructable]
        public NoxiousCrystal()
            : base(CrystalType.Noxious)
        {
            Hue = 2127;
        }
        public NoxiousCrystal(Serial serial) : base(serial) { }

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


    public class BloodyCrystal : Crystal
    {
        public override string DefaultName { get { return "a bloody crystal"; } }

        [Constructable]
        public BloodyCrystal()
            : base(CrystalType.Bloody)
        {
            Hue = 1779;
        }
        public BloodyCrystal(Serial serial) : base(serial) { }

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


    public class MurkyCrystal : Crystal
    {
        public override string DefaultName { get { return "a murky crystal"; } }

        [Constructable]
        public MurkyCrystal()
            : base(CrystalType.Murky)
        {
            Hue = 1768;
        }

        public MurkyCrystal(Serial serial) : base(serial) { }

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
