using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Engines.Craft;
using Server.Mobiles;
using Server.Regions;
using Server.Targets;

namespace Server.Items
{
    [FlipableAttribute(0x1439, 0x1438)]
    public class LargeArcaneHammer : ArcaneHammer
    {
        //Move this to feature list in the future
        public static readonly int DefaultUsesRemaining = 100;

        public override string DefaultName
        {
            get
            {
                return "a large arcane hammer";
            }
        }

        [Constructable]
        public LargeArcaneHammer()
            : base(0x1439)
        {
            Weight = 10.0;
            Layer = Layer.TwoHanded;
            UsesRemaining = DefaultUsesRemaining;
            ShowUsesRemaining = true;
            Hue = 2410;
        }

        public LargeArcaneHammer(Serial serial)
            : base(serial)
        {
        }

        public override bool CheckConflictingLayer(Mobile m, Item item, Layer layer)
        {
            if (base.CheckConflictingLayer(m, item, layer))
                return true;

            if (this.Layer == Layer.TwoHanded && layer == Layer.OneHanded)
            {
                m.SendLocalizedMessage(500214); // You already have something in both hands.
                return true;
            }
            else if (this.Layer == Layer.OneHanded && layer == Layer.TwoHanded && !(item is BaseShield) && !(item is BaseEquipableLight))
            {
                m.SendLocalizedMessage(500215); // You can only wield one weapon at a time.
                return true;
            }

            return false;
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
