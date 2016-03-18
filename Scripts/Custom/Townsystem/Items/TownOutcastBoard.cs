using System;
using Server.Network;
using Server.Items;
using Server.Gumps;

namespace Server.Custom.Townsystem
{
    [Flipable(0x1E5E, 0x1E5F)]
	public class TownOutcastBoard : Item
	{
        private Town m_Town;
        [CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
        public Town Town
        {
            get { return m_Town; }
            set { m_Town = value; } //Invalidate(); }
        }
        
        [Constructable]
        public TownOutcastBoard()
            : base(0x1E5E)
		{
            Movable = false;
		}

        public TownOutcastBoard(Serial serial)
            : base(serial)
		{
		}

        public override void OnMapChange()
        {
            base.OnMapChange();
            if (Location == Point3D.Zero) return;
            Town = Town.FromRegion(Region.Find(Location, Map));
            if (Town == null)
            {
                Delete();
                return;
            }
            Town.OutcastBoard = this; //Invalidate();
            Name = String.Format("The {0} Outcast Board", Town.Definition.FriendlyName);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (m_Town == null)
                return;

            if (from.HasGump(typeof(OutcastGump)) || from.HasGump(typeof(OutcastEntryGump)))
                return;

            from.SendGump(new OutcastGump(m_Town));

            base.OnDoubleClick(from);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);

            Town.WriteReference(writer, m_Town);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            Town = Town.ReadReference(reader);
        }
	}
}
