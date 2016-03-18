using System;
using Server;
using Server.Items;

namespace Server.Custom.Townsystem
{
    [FlipableAttribute(5681, 5531)]
	public class TownFlag : Item
	{
        private Town m_Town;
        
        [CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
        public Town Town
        {
            get { return m_Town; }
            set { m_Town = value; Invalidate(); }
        }

        [Constructable]
		public TownFlag() : base(5681)
		{
            Movable = false;
        }

        public TownFlag(Serial serial)
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
            Town.Flags.Add(this);
            Invalidate();
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
            LabelTo(from, TownBuff.GetBuffName(m_Town.PrimaryCitizenshipBuff));
            foreach (var buff in m_Town.SecondaryCitizenshipBuffs)
                LabelTo(from, TownBuff.GetBuffName(buff));
        }

        public override void OnDelete()
        {
            if (m_Town != null && m_Town.Flags.Contains(this))
                m_Town.Flags.Remove(this);

            base.OnDelete();
        }

        public void Invalidate()
        {
            Name = m_Town == null ? "a town flag" : m_Town.Definition.TownFlagName;
			Hue = m_Town == null || m_Town.HomeFaction == null ? 0 : m_Town.HomeFaction.Definition.HueFlag;
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );

            Town.WriteReference(writer, m_Town);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

            m_Town = Town.ReadReference(reader);

            Invalidate();
		}
	}
}