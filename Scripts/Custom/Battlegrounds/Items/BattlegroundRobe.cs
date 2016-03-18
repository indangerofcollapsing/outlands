using Server.Custom.Battlegrounds.Regions;
using Server.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Battlegrounds.Items
{
    public class BattlegroundRobe : Robe
    {
        [Constructable]
        public BattlegroundRobe(int hue)
        {
            Hue = hue;
        }

        public override bool CheckLift(Mobile from, Item item, ref Network.LRReason reject)
        {
            if (from.Region is BattlegroundRegion)
            {
                reject = Network.LRReason.CannotLift;
                return false;
            }
            else
            {
                Delete();
                return true;
            }
        }

        public BattlegroundRobe(Serial serial)
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
            Delete();
        }
    }
}
