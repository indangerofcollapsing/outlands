using System;

namespace Server.Items
{
    [Flipable(0x1fa1, 0x1fa2)]
    public class AnCorpSupporterTunic : BaseShirt
    {
        [Constructable]
        public AnCorpSupporterTunic(): this(0)
        {
        }

        [Constructable]
        public AnCorpSupporterTunic(int hue): base(0x1FA1, hue)
        {
            Name = "an An Corp Supporter tunic";

            Hue = 1159;
            Weight = 2.0;

            LootType = Server.LootType.Blessed;
            Layer = Layer.MiddleTorso;
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, "an An Corp Supporter tunic");
            LabelTo(from, "(blessed)"); 
        }

        public override bool Scissor(Mobile from, Scissors scissors)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(502437); // Items you wish to cut must be in your backpack.
                return false;
            }

            Cloth cloth = new Cloth();
            cloth.Amount = 1;
            cloth.Hue = Hue;

            Delete();

            from.AddToBackpack(cloth);
            from.SendMessage("You cut the item into cloth.");

            return false;
        }

        public override bool Dye(Mobile from, DyeTub sender)
        {
            return false;
        }

        public AnCorpSupporterTunic(Serial serial): base(serial)
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
            Layer = Layer.MiddleTorso;
        }
    }
}