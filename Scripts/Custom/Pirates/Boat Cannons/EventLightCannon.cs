using System;
using Server;
using Server.Mobiles;
using Server.Multis;
using Server.Targeting;
using System.Collections;
using Server.Items;
using Server.Spells;
using System.Collections.Generic;
using Server.Network;

namespace Server.Custom.Pirates
{
    public class EventLightCannon : LandCannon
    {
        public override bool AlwaysAllowDoubleClick { get { return true; } }
        public override bool Decays { get { return true; } }

        [Constructable]
        public EventLightCannon(): base()
        {
            Name = "a salvaged light cannon";

            CannonType = CannonSize.Heavy;

            UsageAccessLevel = AccessLevel.Seer;
            MoveAccessLevel = AccessLevel.Seer;

            DamageMin = 20;
            DamageMax = 30;

            Range = 12;
            ExplosionRadius = 1;

            CreatureDamageScalar = 0;
            TamedCreatureDamageScalar = 2.5;
            PlayerDamageScalar = 1;
            BoatDamageScalar = 3;
            BreakableObjectDamageScalar = 2.5;

            CannonballItemId = 22337;
        }

        public EventLightCannon(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}