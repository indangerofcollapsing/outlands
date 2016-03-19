using Server.Custom.Battlegrounds.Regions;
using Server.Items;
using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Battlegrounds.Items
{
    public class SpectatorTeleporter : Teleporter
    {

        [CommandProperty(AccessLevel.Administrator)]
        public SiegeDoor AssociatedDoor { get; set; }

        [Constructable]
        public SpectatorTeleporter()
            : base()
        {
            Hue = 1157;
            Name = "A spectator teleporter";
        }

        private SiegeBattleground m_Battleground;

        public override void OnMapChange()
        {
            base.OnMapChange();
            if (Location == Point3D.Zero) return;

            var region = Region.Find(Location, Map);
            if (!(region is BattlegroundRegion))
            {
                Delete();
                return;
            }

            m_Battleground = ((BattlegroundRegion)region).Battleground as SiegeBattleground;

            if (m_Battleground == null)
            {
                Delete();
                return;
            }
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (AssociatedDoor != null && AssociatedDoor.Hits == 0)
                return true;

            if (m.Spectating)
                return base.OnMoveOver(m);
            else if (m is PlayerMobile)
            {
                var player = m as PlayerMobile;
                if (m_Battleground.Defense.Contains(player))
                {
                    if (m.Spell is Spells.Spell)
                        ((Spells.Spell)m.Spell).Disturb(Spells.DisturbType.Kill);

                    return base.OnMoveOver(m);
                }
            }
            return true;
        }

        public SpectatorTeleporter(Serial serial)
            : base(serial)
        {

        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);

            writer.Write(AssociatedDoor);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    AssociatedDoor = reader.ReadItem() as SiegeDoor;
                    goto case 0;
                case 0:
                    break;
            }


            OnMapChange();
        }
    }
}
