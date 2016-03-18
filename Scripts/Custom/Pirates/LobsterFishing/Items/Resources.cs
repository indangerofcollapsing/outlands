/***************************************************************************
 *                            Resources.cs
 *                            ------------------
 *   begin                : February 2011
 *   author               : Sean Stavropoulos
 *   email                : sean.stavro@gmail.com
 *
 *
 ***************************************************************************/
using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class RawLobster : CookableFood
    {
        public override string DefaultName { get { return "raw lobster"; } }

        [Constructable]
        public RawLobster()
            : base(0x44D3, 8 + Utility.Random(3))
        {
            Stackable = true;
        }

        public RawLobster(Serial serial)
            : base(serial)
        {
        }

        public override Food Cook()
        {
            return new Lobster();
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
        }
    }

    public class Lobster : Food
    {
        public override string DefaultName { get { return "cooked lobster"; } }

        [Constructable]
        public Lobster()
            : base(0x44D3)
        {
            FillFactor = 1;
            Weight = 1;
            Stackable = true;
        }

        public Lobster(Serial serial)
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
        }
    }

    public class RawCrab : CookableFood
    {
        public override string DefaultName { get { return "raw crab"; } }

        [Constructable]
        public RawCrab()
            : base(0x44D1, 6 + Utility.Random(3))
        {
            Stackable = true;
        }

        public RawCrab(Serial serial)
            : base(serial)
        {
        }

        public override Food Cook()
        {
            return new Crab();
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
        }
    }

    public class Crab : Food
    {
        public override string DefaultName { get { return "cooked crab"; } }

        [Constructable]
        public Crab()
            : base(0x44D1)
        {
            FillFactor = 1;
            Weight = 1;
            Stackable = true;
        }

        public Crab(Serial serial)
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
        }
    }
}
