﻿using System;
using Server.Network;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Targeting;
using Server.Spells;

namespace Server.Custom
{
    public class IrresistibleLure : Lure
    {
        public override int AggroBonus { get { return 25; } }
        public override TimeSpan AggroExpiration { get { return TimeSpan.FromMinutes(2); } }
        public override int LureHue { get { return 2619; } }

        [Constructable]
        public IrresistibleLure(): base()
        {
            Name = "an irresistible lure";
        }

        public IrresistibleLure(Serial serial): base(serial)
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