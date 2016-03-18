using System;
using System.Collections.Generic;
using Server.Custom;

namespace Server.Items
{
    public class WindBattlegroundTeleporter : Teleporter
    {
        [Constructable]
        public WindBattlegroundTeleporter() {
        }

        public WindBattlegroundTeleporter(Serial serial)
            : base(serial) {
        }

        public override void DoTeleport(Mobile m) {
            if (WindBattleground.CanEnter(m)) {
                base.DoTeleport(m);
            }
        }

        public override void Serialize(GenericWriter writer) {
            base.Serialize(writer);
            writer.WriteEncodedInt(0);
        }

        public override void Deserialize(GenericReader reader) {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }
}
