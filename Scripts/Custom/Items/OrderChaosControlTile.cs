using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;

namespace Server.Items
{
    public class OrderChaosControlTile : Item
    {
        [Constructable]
        public OrderChaosControlTile(): base(5307)
        {
            Name = "Order Chaos Control Tile";

            //5307 Order
            //5347 Chaos
            //7973 Uncontrolled

            Hue = 2500;
            Movable = false;
        }        

        public OrderChaosControlTile(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            //LabelTo(from, "Wrong Dungeon: Zone A");
            //LabelTo(from, "(Order Controlled)"); 

            LabelTo(from, "Wrong Dungeon: Zone A");
            LabelTo(from, "(Uncontrolled)"); 
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;
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