using System;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;

namespace Server.Custom
{
    public class UOACZEastWestPalisade : UOACZBreakableStatic
    {
        public override int OverrideNormalItemId { get { return 546; } }
        public override int OverrideNormalHue { get { return 0; } }

        public override int OverrideLightlyDamagedItemId { get { return 541; } }
        public override int OverrideLightlyDamagedHue { get { return 0; } }

        public override int OverrideHeavilyDamagedItemId { get { return 7134; } }
        public override int OverrideHeavilyDamagedHue { get { return 0; } }

        public override int OverrideBrokenItemId { get { return 3120; } }
        public override int OverrideBrokenHue { get { return 0; } }

        [Constructable]
        public UOACZEastWestPalisade(): base()
        {
            Name = "palisade";

            MaxHitPoints = 1000;
            HitPoints = 1000;
        }

        public UOACZEastWestPalisade(Serial serial): base(serial)
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
