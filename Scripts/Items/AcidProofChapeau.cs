using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Items
{
    public class AcidProofChapeau : WizardsHat
    {
        [Constructable]
        public AcidProofChapeau()
        {
            Name = "acid proof chapeau";
            Hue = 2006;
            DonationItem = true;
        }

        public AcidProofChapeau(Serial serial)
            : base(serial)
        {
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
