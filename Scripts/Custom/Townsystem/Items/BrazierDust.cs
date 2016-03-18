using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Items
{
    public class BrazierDust : Item
    {

        public override string DefaultName { get { return "Brazier Dust"; } }

        [Constructable]
        public BrazierDust()
            : this(1)
        {
        }

        [Constructable]
        public BrazierDust(int amt)
            : base(16954)
        {
            Hue = 973;
            Stackable = true;
            Amount = amt;
        }

        public BrazierDust(Serial serial)
            : base(serial)
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
        }
    }
}
