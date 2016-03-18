using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Battlegrounds.Mobiles
{
    [CorpseName("the sanctuary king's corpse")]
    class SanctuaryKing : BattlegroundKing
    {

        [Constructable]
        public SanctuaryKing()
            : base()
        {
            Body = 175;
            Name = "The Sanctuary King";
        }

        public SanctuaryKing(Serial serial)
            : base(serial)
        {

        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
