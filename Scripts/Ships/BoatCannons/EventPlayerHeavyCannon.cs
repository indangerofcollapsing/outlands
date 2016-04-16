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
    public class EventPlayerHeavyCannon : LandCannon
    {
        public override bool AlwaysAllowDoubleClick { get { return true; } }
        public override bool Decays { get { return true; } }

        [Constructable]
        public EventPlayerHeavyCannon(): base()
        {
            Name = "a salvaged heavy cannon";

            CannonType = CannonSize.Heavy;

            UsageAccessLevel = AccessLevel.Player;
            MoveAccessLevel = AccessLevel.Seer;

            DamageMin = 20;
            DamageMax = 30;

            Range = 18;
            ExplosionRadius = 1;

            CreatureDamageScalar = 6;
            TamedCreatureDamageScalar = 0;
            PlayerDamageScalar = 0;
            BoatDamageScalar = 8;
            BreakableObjectDamageScalar = 5;

            CannonballItemId = 22337;
        }

        public EventPlayerHeavyCannon(Serial serial): base(serial)
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