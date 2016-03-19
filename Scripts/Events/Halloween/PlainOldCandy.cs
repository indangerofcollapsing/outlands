
using System;
using Server.Items;

namespace Server.Custom
{
    class PlainOldCandy1 : Item
    { 
        [Constructable]
        public PlainOldCandy1(): base(2485)
        {
            Name = "plain old candy";

            Hue = 2501;
            Stackable = true;
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            from.PlaySound(0x5A9);
            from.Animate(34, 5, 1, true, false, 0);

            from.SendMessage("You eat some plain old candy.");

            if (Amount == 1)
                Delete();

            else
                Amount--;
        }

        public PlainOldCandy1(Serial serial): base(serial)
        {
        }

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

    class PlainOldCandy2 : Item
    {
        [Constructable]
        public PlainOldCandy2(): base(2485)
        {
            Name = "plain old candy";

            Hue = 2947;
            Stackable = true;
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            from.PlaySound(0x5A9);
            from.Animate(34, 5, 1, true, false, 0);

            from.SendMessage("You eat some plain old candy.");

            if (Amount == 1)
                Delete();

            else
                Amount--;
        }

        public PlainOldCandy2(Serial serial): base(serial)
        {
        }

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

    class PlainOldCandy3 : Item
    {
        [Constructable]
        public PlainOldCandy3(): base(2485)
        {
            Name = "plain old candy";

            Hue = 2213;
            Stackable = true;
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            from.PlaySound(0x5A9);
            from.Animate(34, 5, 1, true, false, 0);

            from.SendMessage("You eat some plain old candy.");

            if (Amount == 1)
                Delete();

            else
                Amount--;
        }

        public PlainOldCandy3(Serial serial): base(serial)
        {
        }

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
