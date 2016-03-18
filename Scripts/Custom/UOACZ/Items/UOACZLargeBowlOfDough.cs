using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
    public class UOACZBowlOfDough : Item
    {
        [Constructable]
        public UOACZBowlOfDough(): base(2590)
        {
            Name = "large bowl of dough";
            Weight = 3;
        }

        public UOACZBowlOfDough(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            LabelTo(from, "(double click to separate)");
        }
        
        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("That item must be in your pack in order to use it.");
                return;
            }

            from.SendMessage("You fashion several balls of dough.");

            bool dropToGround = false;

            int itemCount = 5;

            Item item = null;

            int x = X;
            int y = Y;

            Delete();

            for (int a = 0; a < itemCount; a++)
            {
                item = new UOACZDough();

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

            item = new UOACZBowl();

            if (!from.Backpack.TryDropItem(from, item, false))
            {
                item.MoveToWorld(from.Location, from.Map);
                dropToGround = true;
            }

            if (dropToGround)            
                from.SendMessage("There was not enough room in your backpack and some of the items were placed on the ground.");     
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