using System;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;

namespace Server.Custom
{
    public class UOACZNorthSouthPalisade : UOACZBreakableStatic
    {
        public override int OverrideNormalItemId { get { return 545; } }
        public override int OverrideNormalHue { get { return 0; } }

        public override int OverrideLightlyDamagedItemId { get { return 542; } }
        public override int OverrideLightlyDamagedHue { get { return 0; } }

        public override int OverrideHeavilyDamagedItemId { get { return 7137; } }
        public override int OverrideHeavilyDamagedHue { get { return 0; } }

        public override int OverrideBrokenItemId { get { return 3118; } }
        public override int OverrideBrokenHue { get { return 0; } }

        [Constructable]
        public UOACZNorthSouthPalisade(): base()
        {
            Name = "palisade";

            MaxHitPoints = 1000;
            HitPoints = 1000;
        }

        public UOACZNorthSouthPalisade(Serial serial): base(serial)
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
