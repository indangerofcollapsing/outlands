using System;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;

namespace Server.Custom
{
    public class UOACZEastWestLogLeftDoor : UOACZBreakableDoor
    {
        public override int OverrideNormalItemId { get { return 1749; } }
        public override int OverrideNormalHue { get { return 0; } }

        public override int OverrideLightlyDamagedItemId { get { return 2105; } }
        public override int OverrideLightlyDamagedHue { get { return 0; } }

        public override int OverrideHeavilyDamagedItemId { get { return 7138; } }
        public override int OverrideHeavilyDamagedHue { get { return 0; } }

        public override int OverrideBrokenItemId { get { return 1290; } }
        public override int OverrideBrokenHue { get { return 2415; } }

        public override DoorFacingType DoorFacing { get { return DoorFacingType.EastWest; } }

        [Constructable]
        public UOACZEastWestLogLeftDoor(): base()
        {
            Name = "door";

            MaxHitPoints = 1500;
            HitPoints = 1500;
        }

        public UOACZEastWestLogLeftDoor(Serial serial): base(serial)
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
