using System;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;

namespace Server.Custom
{
    public class UOACZNorthSouthLogWall : UOACZBreakableStatic
    {
        public override int OverrideNormalItemId { get { return 145; } }
        public override int OverrideNormalHue { get { return 0; } }

        public override int OverrideLightlyDamagedItemId { get { return 155; } }
        public override int OverrideLightlyDamagedHue { get { return 0; } }

        public override int OverrideHeavilyDamagedItemId { get { return 7138; } }
        public override int OverrideHeavilyDamagedHue { get { return 0; } }

        public override int OverrideBrokenItemId { get { return 1289; } }
        public override int OverrideBrokenHue { get { return 2415; } }

        [Constructable]
        public UOACZNorthSouthLogWall(): base()
        {
            Name = "wall";

            MaxHitPoints = 1500;
            HitPoints = 1500;
        }

        public UOACZNorthSouthLogWall(Serial serial): base(serial)
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
