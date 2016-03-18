using System;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;

namespace Server.Custom
{
    public class UOACZEastWestGateRightDoor : UOACZBreakableDoor
    {
        public override int OverrideNormalItemId { get { return 13893; } }
        public override int OverrideNormalHue { get { return 2405; } }

        public override int OverrideLightlyDamagedItemId { get { return 13893; } }
        public override int OverrideLightlyDamagedHue { get { return 2405; } }

        public override int OverrideHeavilyDamagedItemId { get { return 13893; } }
        public override int OverrideHeavilyDamagedHue { get { return 2405; } }

        public override int OverrideBrokenItemId { get { return 1291; } }
        public override int OverrideBrokenHue { get { return 2406; } }

        public override DoorFacingType DoorFacing { get { return DoorFacingType.EastWest; } }

        [Constructable]
        public UOACZEastWestGateRightDoor(): base()
        {
            Name = "gate door";
        }

        public UOACZEastWestGateRightDoor(Serial serial): base(serial)
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
