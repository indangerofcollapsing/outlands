using System;
using Server;

namespace Server.Custom.Townsystem
{
	public class TownCrystal : Item
	{
        private Town m_Town;
        //private bool m_Capturing;
        private Item m_EffectItem1;
        public int EventRange { get { return 5; } }
        public override bool HandlesOnMovement { get { return false; } }
        
        [CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public Town Town
        {
            get { return m_Town; }
            set { m_Town = value; Invalidate(); }
        }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public Town ControllingTown
        {
			get { return m_Town.ControllingTown; }
            set 
            {
				if (value == m_Town.ControllingTown) return;
                m_Town.Capture(value);
            }
        }
		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public Town ControllingTownNoMsg
        {
			get { return m_Town.ControllingTown; }
            set
            {
				if (value == m_Town.ControllingTown) return;
                m_Town.Capture(value, false);
            }
        }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public bool MainTownCrystal
        {
            get { return (m_Town.Crystals.IndexOf(this) == 0); }
            set 
            {
                if (value)
                {
                    TownCrystal prev = m_Town.Crystals[0];
                    if (prev == this) return;
                    m_Town.Crystals.Remove(this);
                    m_Town.Crystals.Insert(0, this);
                    Invalidate();
                    prev.Invalidate();
                }
            }
        }

        [Constructable]
		public TownCrystal() : base(8740)
		{
            Movable = false;
            //m_Capturing = false;
        }

        public TownCrystal(Serial serial)
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
            Town.Crystals.Add(this);
            Invalidate();
        }

        /*public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            bool inOldRange = Utility.InRange(oldLocation, Location, EventRange);
            bool inNewRange = Utility.InRange(m.Location, Location, EventRange);

            if (inNewRange && !inOldRange)
                OnEnter(m);
            else if (inOldRange && !inNewRange)
                OnExit(m);
        }

        public void OnEnter(Mobile m)
        {
            if (Capturing & Faction.Find(m) != m_Town.Owner)
                m.SendMessage("You are now in the control zone.");
        }

        public void OnExit(Mobile m)
        {
            if (Capturing & Faction.Find(m) != m_Town.Owner)
                m.SendMessage("You have left the control zone.");
        }*/

        public override void OnDelete()
        {
            if (m_Town != null && m_Town.Crystals.Contains(this))
                m_Town.Crystals.Remove(this);

            base.OnDelete();
        }

        public void Invalidate()
        {
            Name = m_Town == null || (m_Town.Crystals.IndexOf(this) != 0) ? "a town crystal" : m_Town.Definition.TownCrystalName;
			ItemID = m_Town == null || m_Town.ControllingTown == null ? 8740 : m_Town.ControllingTown.HomeFaction.Definition.CrystalID;
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );

            Town.WriteReference(writer, m_Town);
            //writer.Write(m_Capturing);
            writer.WriteItem(m_EffectItem1);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

            m_Town = Town.ReadReference(reader);
            //m_Capturing = reader.ReadBool();
            m_EffectItem1 = reader.ReadItem();

           // if (m_Capturing == true)
           //     Capturing = false;
		}
	}
}