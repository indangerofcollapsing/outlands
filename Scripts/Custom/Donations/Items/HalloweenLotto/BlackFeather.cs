using System;
using System.Collections.Generic;
using Server.Custom;

namespace Server.Items
{
    public class BlackFeather : Item
    {
        public override string DefaultName { get { return "a black feather"; } }

        [Constructable]
        public BlackFeather()
            : base(4129)
        {
            DonationItem = true;
            Hue = 2051;
        }

        public BlackFeather(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Deleted)
                return;

            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("That must be in your backpack to use.");
                return;
            }

            if (CheckLuck()) {
                from.Backpack.DropItem(new HalloweenTicket());
                from.PublicOverheadMessage(Network.MessageType.Regular, 0, false, "You win another ticket!"); 
            }
            else {
                from.SendMessage("The feather crumbles in your hands.");
            }

            Delete();
        }

        private bool CheckLuck()
        {
            return Utility.RandomBool();
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
