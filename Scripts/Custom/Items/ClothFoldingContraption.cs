using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Items;
using Server.Targets;

namespace Server.Items
{
    [Flipable(0x2AF9, 0x2AFD)]
    public class ClothFoldingContraption : Item
    {
        public override string DefaultName
        {
            get
            {
                return "a cloth folding contraption";
            }
        }

        [Constructable]
        public ClothFoldingContraption()
            : base(0x2AF9)
        {
            Weight = 8.0;
            Hue = 2502;
        }

        public ClothFoldingContraption(int itemId)
            : base(itemId)
        {
        }

        public ClothFoldingContraption(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(this.GetWorldLocation(), 1))
            {
                from.SendLocalizedMessage(1010018); // What do you want to use this item on?

                from.Target = new FoldClothesTarget(this);
            }
            else
            {
                from.SendLocalizedMessage(500446); // That is too far away.
            }

        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);


        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

        }
    }
}
