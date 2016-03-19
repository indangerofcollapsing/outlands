using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
    public class UOACZFish : Item
    {
        [Constructable]
        public UOACZFish(): base(15102)
        {
            Name = "a fish";
            Weight = 1;
        }

        public UOACZFish(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            LabelTo(from, "(double click to clean fish)");
        }
        
        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("That item must be in your pack in order to use it.");
                return;
            }

            from.SendMessage("You clean the fish and place a raw fish steak in your pack.");

            int x = X;
            int y = Y;

            Item item = new UOACZRawFishsteak();

            bool dropToGround = false;    

            Delete();

            if (!from.Backpack.TryDropItem(from, item, false))
            {
                item.MoveToWorld(from.Location, from.Map);
                dropToGround = true;
            }

            else
            {
                item.X = X;
                item.Y = Y;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version          
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}