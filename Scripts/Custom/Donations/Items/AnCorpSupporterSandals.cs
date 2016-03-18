using System;

namespace Server.Items
{
    [Flipable(0x1fa1, 0x1fa2)]
    public class AnCorpSupporterSandals : Sandals
    {
        [Constructable]
        public AnCorpSupporterSandals(): this(0)
        {
        }

        [Constructable]
        public AnCorpSupporterSandals(int hue): base(hue)
        {
            Name = "An Corp Supporter sandals";

            Hue = 1159;

            LootType = Server.LootType.Blessed;
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, "An Corp Supporter sandals");
            LabelTo(from, "(blessed)"); 
        }

        //public override bool Scissor(Mobile from, Scissors scissors)
        //{
        //    if (!IsChildOf(from.Backpack))
        //    {
        //        from.SendLocalizedMessage(502437); // Items you wish to cut must be in your backpack.
        //        return false;
        //    }

        //    Cloth cloth = new Cloth();
        //    cloth.Amount = 1;
        //    cloth.Hue = Hue;

        //    Delete();

        //    from.AddToBackpack(cloth);
        //    from.SendMessage("You cut the item into cloth.");

        //    return false;
        //}

        public override bool Dye(Mobile from, DyeTub sender)
        {
            return false;
        }

        public AnCorpSupporterSandals(Serial serial): base(serial)
        {
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

            //Override
            Hue = 1159;

        }
    }
}