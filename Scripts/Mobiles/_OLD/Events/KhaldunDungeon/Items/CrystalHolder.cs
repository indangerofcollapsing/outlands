using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Items;

namespace Server.Custom.Events.KhaldunDungeon.Items
{
    public class CrystalHolder : Item
    {
        public int Amount { get; set; }
        public CrystalType CrystalType { get; set; }

        public CrystalHolder(CrystalType type, int amount = 0)
            : base(7964)
        {
            Amount = amount;
            CrystalType = type;
        }

        public bool CanHold(object item)
        {
            if (item is Crystal)
            {
                Crystal crystal = item as Crystal;
                return crystal.CrystalType == CrystalType;
            }
            else
                return false;
        }

        public CrystalHolder(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
            writer.Write(Amount);
            writer.Write((int)CrystalType);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Amount = reader.ReadInt();
            CrystalType = (CrystalType)reader.ReadInt();
        }
    }

    public class GraveDustCrystalHolder : CrystalHolder
    {
        public override string DefaultName { get { return string.Format("A Grave Dust Crystal Container: {0}", Amount); } }

    [Constructable]
        public GraveDustCrystalHolder()
            : base(CrystalType.GraveDust)
        {
            Hue = 2500;
        }
        public GraveDustCrystalHolder(Serial serial) : base(serial) { }

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

    public class NoxiousCrystalHolder : CrystalHolder
    {
        public override string DefaultName { get { return string.Format("A Noxious Crystal Container: {0}", Amount); } }

    [Constructable]

        public NoxiousCrystalHolder()
            : base(CrystalType.Noxious)
        {
            Hue = 2127;
        }
        public NoxiousCrystalHolder(Serial serial) : base(serial) { }

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

    public class BloodyCrystalHolder : CrystalHolder
    {
        public override string DefaultName { get { return string.Format("A Bloody Crystal Container: {0}", Amount); } }

    [Constructable]

        public BloodyCrystalHolder()
            : base(CrystalType.Bloody)
        {
            Hue = 1779;
        }
        public BloodyCrystalHolder(Serial serial) : base(serial) { }

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

    public class MurkyCrystalHolder : CrystalHolder
    {
        public override string DefaultName { get { return string.Format("A Murky Crystal Container: {0}", Amount); } }
    
        [Constructable]
        public MurkyCrystalHolder()
            : base(CrystalType.Murky)
        {
            Hue = 1768;
        }
        public MurkyCrystalHolder(Serial serial) : base(serial) { }

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
