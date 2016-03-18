using System;

namespace Server.Items
{
    [FlipableAttribute(0x1BD7, 0x1BDA)]
    public class BaseResourceBoard : Item, ICommodity
    {
        private CraftResource m_Resource;

        [CommandProperty(AccessLevel.GameMaster)]
        public CraftResource Resource
        {
            get { return m_Resource; }
            set { m_Resource = value; InvalidateProperties(); }
        }

        int ICommodity.DescriptionNumber
        {
            get
            {
                if (m_Resource >= CraftResource.OakWood && m_Resource <= CraftResource.YewWood)
                    return 1075052 + ((int)m_Resource - (int)CraftResource.OakWood);

                switch (m_Resource)
                {
                    case CraftResource.Bloodwood: return 1075055;
                    case CraftResource.Frostwood: return 1075056;
                    case CraftResource.Heartwood: return 1075062;	//WHY Osi.  Why?
                }

                return LabelNumber;
            }
        }

        bool ICommodity.IsDeedable { get { return true; } }

        [Constructable]
        public BaseResourceBoard()
            : this(1)
        {
        }

        [Constructable]
        public BaseResourceBoard(int amount)
            : this(CraftResource.RegularWood, amount)
        {
        }

        public BaseResourceBoard(Serial serial)
            : base(serial)
        {
        }

        [Constructable]
        public BaseResourceBoard(CraftResource resource)
            : this(resource, 1)
        {
        }

        [Constructable]
        public BaseResourceBoard(CraftResource resource, int amount)
            : base(0x1BD7)
        {
            Stackable = true;
            Amount = amount;

            m_Resource = resource;
            Hue = CraftResources.GetHue(resource);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (!CraftResources.IsStandard(m_Resource))
            {
                int num = CraftResources.GetLocalizationNumber(m_Resource);

                if (num > 0)
                    list.Add(num);
                else
                    list.Add(CraftResources.GetName(m_Resource));
            }
        }



        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)4);

            writer.Write((int)m_Resource);
        }

        public static bool UpdatingBaseClass;
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            if (version == 3)
                UpdatingBaseClass = true;
            switch (version)
            {
                case 4:
                case 3:
                case 2:
                    {
                        m_Resource = (CraftResource)reader.ReadInt();
                        break;
                    }
            }

            if ((version == 0 && Weight == 0.1) || (version <= 2 && Weight == 2))
                Weight = -1;

            if (version <= 1)
                m_Resource = CraftResource.RegularWood;
        }
    }

    public class Board : BaseResourceBoard
    {
        [Constructable]
        public Board()
            : this(1)
        {
            Name = "board";
        }

        [Constructable]
        public Board(int amount)
            : base(CraftResource.RegularWood, amount)
        {
            Name = "board";
        }

        public Board(Serial serial)
            : base(serial)
        {
            Name = "board";
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            if (BaseResourceBoard.UpdatingBaseClass)
                return;
            int version = reader.ReadInt();
        }
    }

    public class HeartwoodBoard : BaseResourceBoard
    {
        [Constructable]
        public HeartwoodBoard()
            : this(1)
        {
            Name = "heartwood board";
        }

        [Constructable]
        public HeartwoodBoard(int amount)
            : base(CraftResource.Heartwood, amount)
        {
            Name = "heartwood board";
        }

        public HeartwoodBoard(Serial serial)
            : base(serial)
        {
            Name = "heartwood board";
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
    }

    public class BloodwoodBoard : BaseResourceBoard
    {
        [Constructable]
        public BloodwoodBoard()
            : this(1)
        {
            Name = "bloodwood board";
        }

        [Constructable]
        public BloodwoodBoard(int amount)
            : base(CraftResource.Bloodwood, amount)
        {
            Name = "bloodwood board";
        }

        public BloodwoodBoard(Serial serial)
            : base(serial)
        {
            Name = "bloodwood board";
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
    }

    public class FrostwoodBoard : BaseResourceBoard
    {
        [Constructable]
        public FrostwoodBoard()
            : this(1)
        {
            Name = "frostwood board";
        }

        [Constructable]
        public FrostwoodBoard(int amount)
            : base(CraftResource.Frostwood, amount)
        {
            Name = "frostwood board";
        }

        public FrostwoodBoard(Serial serial)
            : base(serial)
        {
            Name = "frostwood board";
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
    }

    public class OakBoard : BaseResourceBoard
    {
        [Constructable]
        public OakBoard()
            : this(1)
        {
            Name = "oak board";
        }

        [Constructable]
        public OakBoard(int amount)
            : base(CraftResource.OakWood, amount)
        {
            Name = "oak board";
        }

        public OakBoard(Serial serial)
            : base(serial)
        {
            Name = "oak board";
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
    }

    public class AshBoard : BaseResourceBoard
    {
        [Constructable]
        public AshBoard()
            : this(1)
        {
            Name = "ash board";
        }

        [Constructable]
        public AshBoard(int amount)
            : base(CraftResource.AshWood, amount)
        {
            Name = "ash board";
        }

        public AshBoard(Serial serial)
            : base(serial)
        {
            Name = "ash board";
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
    }

    public class YewBoard : BaseResourceBoard
    {
        [Constructable]
        public YewBoard()
            : this(1)
        {
            Name = "yew board";
        }

        [Constructable]
        public YewBoard(int amount)
            : base(CraftResource.YewWood, amount)
        {
            Name = "yew board";
        }

        public YewBoard(Serial serial)
            : base(serial)
        {
            Name = "yew board";
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
    }
}