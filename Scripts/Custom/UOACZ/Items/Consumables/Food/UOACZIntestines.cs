using System;
using Server;

namespace Server.Items
{
    public class UOACZIntestines : Item
    {
        [Constructable]
        public UOACZIntestines(): base(22311)
        { 
            Name = "intestines";
            Hue = 2970;

            Weight = .1;
            Stackable = true;
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);            
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            from.SendMessage("Intensines can be used to make several items with the Tailoring skill.");
        }

        public UOACZIntestines(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}