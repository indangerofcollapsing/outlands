using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Townsystem
{
    public class CaptureBrazier : TownBrazier
    {

        [Constructable]
        public CaptureBrazier()
            : base()
        {
            Name = "a capture brazier";
            Movable = false;
            Captured = false;
            BrazierLocationName = "Capture";
        }

        public CaptureBrazier(Serial serial) : base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            if (Captured && CapTown != null)
            {
                LabelTo(from, "[" + CapTown.Definition.FriendlyName + "]");
            }
            LabelTo(from, "Capture Brazier");
        }

        public override void OnMapChange()
        {
            if (Location == Point3D.Zero) return;
            Town = Town.FromRegion(Region.Find(Location, Map));
            if (Town == null)
            {
                Delete();
                return;
            }
            Town.CaptureBrazier = this;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
