/***************************************************************************
 *                              WandedNote.cs
 *                            ------------------
 *   begin                : February 2010
 *   author               : Sean Stavropoulos
 *   email                : sean.stavro@gmail.com
 *
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;


namespace Server.Custom
{
    public class WantedNote : Item
    {
        private Mobile m_Detective;
        private Mobile m_Killer;
        private DateTime m_TimeCreated;
        private int m_Quality;

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime TimeCreated
        {
            get { return m_TimeCreated; }
            set { m_TimeCreated = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Detective
        {
            get { return m_Detective; }
            set { m_Detective = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Killer
        {
            get { return m_Killer; }
            set { m_Killer = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Quality
        {
            get { return m_Quality; }
            set { m_Quality = value; }
        }

        [Constructable]
        public WantedNote()
            : this(null, null, 50)
        {
        }

        public WantedNote(Mobile killer, Mobile det, int qual) : base( 5359 )
		{
            if (killer != null)
                m_Killer = killer;
            if (det != null)
                m_Detective = det;

            m_Quality = qual;

			TimeCreated = DateTime.UtcNow;

            Name = killer != null ? String.Format("WANTED: {0}", killer.Name) : "WANTED NOTE";
		}
        public WantedNote(Serial serial)
            : base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            if (m_Detective != null)
                LabelTo(from, String.Format("Evidence gathered by Detective {0}", m_Detective.Name));
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.SendMessage("Turn this evidence in to the Detective Guildmaster for action to be taken.");
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);

            writer.WriteMobile(m_Detective);
            writer.WriteMobile(m_Killer);
            writer.Write((DateTime)m_TimeCreated);
            writer.Write((int)m_Quality);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version  = reader.ReadInt();

            m_Detective = (Mobile)reader.ReadMobile();
            m_Killer = (Mobile)reader.ReadMobile();
            m_TimeCreated = (DateTime)reader.ReadDateTime();
            m_Quality = reader.ReadInt();

            if (version < 1)
                Timer.DelayCall(TimeSpan.FromTicks(1), Delete);
        }
    }
}
