using System;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;

namespace Server.Custom
{
    public class BreakableStoneWall : BreakableStatic
    {
        [Constructable]
        public BreakableStoneWall(): base()
        {
            Name = "a stone wall";

            Visible = true;

            InteractionRange = 1;
            InteractionDelay = 5;
            MinInteractDamage = 5;
            MaxInteractDamage = 10;

            InteractDamageScalar = 1.0;
            WeaponDamageScalar = 1.0;
            LockpickDamageScalar = 0;
            ObjectBreakingDeviceDamageScalar = 2.0;
            MiningDamageScalar = .33;
            LumberjackingDamageScalar = 0.20;

            HitPoints = 1000;
            MaxHitPoints = 1000;

            AddLOSBlocker = false;

            HitSound = 0x0EE;

            ItemID = 95;
            Hue = 0;             

            NormalItemId = 95;
            NormalHue = 0;

            LightlyDamagedItemId = 630;
            LightlyDamagedHue = 0;

            HeavilyDamagedItemId = 631;
            HeavilyDamagedHue = 0;

            BrokenSound = 0x477;
            BrokenItemId = 631;
            BrokenHue = 0;

            DeleteOnBreak = true;
            CreateTimedStaticAfterBreak = true; 
                        
            TimedStaticOnBreakItemId = 632;
            TimedStaticOnBreakHue = 0;
            TimedStaticDuration = 3;
            TimedStaticOnBreakName = "rubble";

            RevealNearbyHiddenItemsOnBreak = false;
            RevealNearbyHiddenItemsOnBreakRadius = 0;
            RefreshNearbyMovables = false;
            RefreshNearbyMovablesRadius = 0;            
        }

        public BreakableStoneWall(Serial serial): base(serial)
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
