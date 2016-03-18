using Server.Items;
using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Battlegrounds.Items
{
    [Flipable(5020, 4647)]
    class BattlegroundTrophy : Item
    {
        public BattlegroundTrophy(int hue, string name)
            : base(5020)
        {
            Name = name;
            Hue = hue;
            Weight = 0.01;
        }

        public BattlegroundTrophy(Serial serial)
            : base(serial)
        {

        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
