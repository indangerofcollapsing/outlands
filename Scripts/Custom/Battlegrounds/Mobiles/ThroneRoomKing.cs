using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Battlegrounds.Mobiles
{
    [CorpseName("the dragon king's corpse")]
    public class ThroneRoomKing : BattlegroundKing
    {

        [Constructable]
        public ThroneRoomKing()
            : base()
        {
            Body = 826;
            Name = "The Dragon King";
        }

        public ThroneRoomKing(Serial serial)
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
