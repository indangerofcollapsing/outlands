using Server.Custom.Townsystem;
using Server.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Items.Totem
{
    public class MilitiaTotem : FinisherTotem
    {
        [Constructable]
        public MilitiaTotem() : base()
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Town Town
        {
            set
            {
                Hue = value.HomeFaction.Definition.HuePrimary;
                Name = string.Format("{0} Finisher Totem", value.Definition.FriendlyName);
                m_DisplayName = value.Definition.FriendlyName;
            }
        }

        public MilitiaTotem(Serial serial) : base(serial) { }
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

    public class DreadTotem : FinisherTotem
    {
        public override int PlayerClassCurrencyValue { get { return 5000; } }

        [Constructable]
        public DreadTotem(): base("Dread Lord", PlayerClassPersistance.MurdererItemHue)
        {
            PlayerClass = PlayerClass.Murderer;
            PlayerClassRestricted = true;
        }

        public DreadTotem(Serial serial) : base(serial) { }
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

    public class PaladinTotem : FinisherTotem
    {
        public override int PlayerClassCurrencyValue { get { return 5000; } }

        [Constructable]
        public PaladinTotem(): base("Paladin", PlayerClassPersistance.PaladinItemHue)
        {
            PlayerClass = PlayerClass.Paladin;
            PlayerClassRestricted = true;
        }

        public PaladinTotem(Serial serial) : base(serial) { }
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

    public class PirateTotem : FinisherTotem
    {
        public override int PlayerClassCurrencyValue { get { return 5000; } }

        [Constructable]
        public PirateTotem(): base("Pirate", PlayerClassPersistance.PirateItemHue)
        {
            PlayerClass = PlayerClass.Pirate;
            PlayerClassRestricted = true;
        }

        public PirateTotem(Serial serial) : base(serial) { }
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

    public class FinisherTotem : Item
    {
        protected string m_DisplayName;

        [Constructable]
        public FinisherTotem(string name, int hue)
            : base(0x2F5B)
        {
            LootType = LootType.Blessed;
            Weight = 1.0;
            Layer = Layer.Talisman;
            Hue = hue;
            Name = string.Format("{0} Finisher Totem", name);
            m_DisplayName = name;
        }

        [Constructable]
        public FinisherTotem() : this("", 0) { }

        public FinisherTotem(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
            writer.Write(m_DisplayName);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_DisplayName = reader.ReadString();
        }

        public void PerformFinisher(Point3D location, Map map)
        {
            if (location == Point3D.Zero) return;

            var owner = Parent as Mobile;
            if (owner == null) return;

            string flagName = string.Format("{0}'s {1} Flag", owner.Name, m_DisplayName);

            FinisherFlag flag = new FinisherFlag(Hue, flagName);
            flag.MoveToWorld(location, map);
            flag.PlayEffects();

            Timer.DelayCall(TimeSpan.FromMinutes(1), () => { flag.Delete(); });
        }
    }

    public class FinisherFlag : Item
    {
        [Constructable]
        public FinisherFlag(int hue, string name)
            : base(5525)
        {
            Hue = hue;
            Name = name;
            Movable = false;
        }

        public FinisherFlag(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            this.Delete();
        }

        public void PlayEffects()
        {
            var location = new Point3D(Location.X, Location.Y, Location.Z + 10);
            int renderMode = Utility.RandomList(0, 2, 3, 4, 5, 7);

            Effects.PlaySound(location, Map, Utility.Random(0x11B, 4));
            Effects.SendLocationEffect(location, Map, 0x373A + (0x10 * Utility.Random(4)), 16, 10, Hue, renderMode);
        }
    }
}
