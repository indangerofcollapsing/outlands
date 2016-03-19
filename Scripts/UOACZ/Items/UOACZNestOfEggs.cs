using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
    public class UOACZNestOfEggs : Item
    {
        [Constructable]
        public UOACZNestOfEggs(): base(6868)
        {
            Name = "nest of eggs";
            Weight = 2;
        }

        public UOACZNestOfEggs(Serial serial): base(serial)
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

            from.SendMessage("You carefully remove the eggs from the nest.");

            int x = X;
            int y = Y;

            bool dropToGround = false;

            int itemCount = Utility.RandomMinMax(3, 5);

            Delete();

            for (int a = 0; a < itemCount; a++)
            {
                Item item = new UOACZEggs();

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