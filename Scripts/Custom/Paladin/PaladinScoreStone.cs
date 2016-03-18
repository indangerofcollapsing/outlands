using System;
using Server;
using Server.Mobiles;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Gumps;

namespace Server.Custom
{
    public class PaladinScoreStone : PlayerClassScoreStone
    {
        public override PlayerClass PlayerClassType { get { return PlayerClass.Paladin; } }

        [Constructable]
        public PaladinScoreStone(): base()
        {
            Name = "a paladin score stone";

            Hue = 1154;
        }

        public PaladinScoreStone(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version          
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}