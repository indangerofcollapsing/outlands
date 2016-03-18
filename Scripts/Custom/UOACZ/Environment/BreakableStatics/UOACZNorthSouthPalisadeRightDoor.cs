using System;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;

namespace Server.Custom
{
    public class UOACZNorthSouthPalisadeRightDoor : UOACZBreakableDoor
    {
        public override int OverrideNormalItemId { get { return 1695; } }
        public override int OverrideNormalHue { get { return 1848; } }

        public override int OverrideLightlyDamagedItemId { get { return 554; } }
        public override int OverrideLightlyDamagedHue { get { return 0; } }

        public override int OverrideHeavilyDamagedItemId { get { return 7134; } }
        public override int OverrideHeavilyDamagedHue { get { return 0; } }

        public override int OverrideBrokenItemId { get { return 3120; } }
        public override int OverrideBrokenHue { get { return 0; } }

        public override DoorFacingType DoorFacing { get { return DoorFacingType.NorthSouth; } }

        [Constructable]
        public UOACZNorthSouthPalisadeRightDoor(): base()
        {
            Name = "door";

            MaxHitPoints = 1000;
            HitPoints = 1000;
        }

        public UOACZNorthSouthPalisadeRightDoor(Serial serial): base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);
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
