using Server.Items;
using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Misc
{
    class YoungTeleporter : Teleporter
    {

        [Constructable]
        public YoungTeleporter() : base() {}

        public YoungTeleporter(Serial serial) : base(serial) { }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
        }
        public override void DoTeleport(Mobile m)
        {

            if (m.Criminal)
            {
                m.SendLocalizedMessage(1005561, "", 0x22); // Thou'rt a criminal and cannot escape so easily.
            }
            else if (m.Spell != null)
            {
                m.SendLocalizedMessage(1049616); // You are too busy to do that at the moment.
            }
            else if (m is PlayerMobile && !((PlayerMobile)m).Young)
            {
                m.SendMessage("You must be a young player to use this teleporter.");
            }
            else
            {
                base.DoTeleport(m);
            }
        }
    }
}
