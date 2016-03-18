using System;

using Server.Network;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Items
{
    public class VictimNote : Item
	{
        private PlayerMobile m_Paladin;
        [CommandProperty(AccessLevel.GameMaster)]
        public PlayerMobile Paladin
        {
            get { return m_Paladin; }
            set { m_Paladin = value; }
        }

        private PlayerMobile m_Murderer;
        [CommandProperty(AccessLevel.GameMaster)]
        public PlayerMobile Murderer
        {
            get { return m_Murderer; }
            set { m_Murderer = value; }
        }

        private PlayerMobile m_Victim;
        [CommandProperty(AccessLevel.GameMaster)]
        public PlayerMobile Victim
        {
            get { return m_Victim; }
            set { m_Victim = value; }
        }        

        private int m_Restitution;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Restitution
        {
            get { return m_Restitution; }
            set { m_Restitution = value; }
        }

        private string m_PaladinName = "";
        private string m_MurdererName = "";
        private string m_VictimName = "";

        [Constructable]
        public VictimNote(PlayerMobile paladin, PlayerMobile murderer, PlayerMobile victim, int restitution): base(0x14ED)
        {
            m_Paladin = paladin;
            m_Murderer = murderer;
            m_Victim = victim;
            m_Restitution = restitution;

            if (m_Paladin != null)
                m_PaladinName = m_Paladin.RawName;

            if (m_Murderer != null)
                m_MurdererName = m_Murderer.RawName;

            if (m_Victim != null)
                m_VictimName = m_Victim.RawName;

            Name = "a note of restitution";                    
        }

        public VictimNote(Serial serial): base(serial)
		{
		}

        public override void OnSingleClick(Mobile from)
        {
            if (m_VictimName != null)
                LabelTo(from, "a note of restitution for " + m_VictimName);

            else
                LabelTo(from, "a note of restitution");
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.CloseAllGumps();
            from.SendGump(new Custom.Paladin.VictimNoteGump(m_PaladinName, m_MurdererName, m_VictimName, m_Restitution));
        }

        public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( 0 ); // version

            writer.Write(m_Paladin);
            writer.Write(m_Murderer);
            writer.Write(m_Victim);
            writer.Write(m_Restitution);

            writer.Write(m_PaladinName);
            writer.Write(m_MurdererName);
            writer.Write(m_VictimName);
		}

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();    
        
            //Version 0
            if (version >= 0)
            {
                m_Paladin = (PlayerMobile)reader.ReadMobile();
                m_Murderer = (PlayerMobile)reader.ReadMobile();
                m_Victim = (PlayerMobile)reader.ReadMobile();
                m_Restitution = reader.ReadInt();

                m_PaladinName = reader.ReadString();
                m_MurdererName = reader.ReadString();
                m_VictimName = reader.ReadString();
            }
        }
    }
}