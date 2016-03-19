using System;
using Server;
using Server.Mobiles;
using Server.Multis;
using Server.Targeting;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Spells;
using Server.Custom;
using Server.Network;

namespace Server
{
    public class PlayerLandCannon : LandCannon
    {
        public override int PlayerClassCurrencyValue { get { return 10000; } }

        public override bool AlwaysAllowDoubleClick { get { return true; } }
        public override bool Decays { get { return true; } }

        [Constructable]
        public PlayerLandCannon(): base()
        {
            Name = "a salvaged cannon";

            UsageAccessLevel = AccessLevel.Player;
            MoveAccessLevel = AccessLevel.Player;

            CreatureDamageScalar = 0;
            TamedCreatureDamageScalar = 0;
            PlayerDamageScalar = 0;
            BoatDamageScalar = 0;
            BreakableObjectDamageScalar = 0;
        }

        public PlayerLandCannon(Serial serial): base(serial)
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