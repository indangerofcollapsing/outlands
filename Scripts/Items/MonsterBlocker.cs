using Server.Items;
using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Items.Misc
{
    public class MonsterBlocker : LOSBlocker
    {
        [Constructable]
        public MonsterBlocker()
            : base()
        {
        }

        public override string DefaultName
        {
            get
            {
                return "monsters shall not pass; no line of sight";
            }
        }

        public MonsterBlocker(Serial serial)
            : base(serial)
        {

        }

        public override bool OnMoveOver(Mobile m)
        {
            var creature = m as BaseCreature;
            if (creature != null && !(creature.Controlled || creature.Summoned))
                return false;

            return base.OnMoveOver(m);
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
