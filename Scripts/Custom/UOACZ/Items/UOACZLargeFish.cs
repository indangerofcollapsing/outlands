using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
    public class UOACZLargeFish : Item
    {
        [Constructable]
        public UOACZLargeFish(): base(Utility.RandomList(2508, 2509, 2510, 2511))
        {
            Name = "a large fish";
            Weight = 3;
        }

        public UOACZLargeFish(Serial serial): base(serial)
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

            from.SendMessage("You clean the fish and place several raw fish steaks in your pack.");

            int x = X;
            int y = Y;

            bool dropToGround = false;

            int fishCount = 3;

            Delete();

            for (int a = 0; a < fishCount; a++)
            {
                Item item = new UOACZRawFishsteak();

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