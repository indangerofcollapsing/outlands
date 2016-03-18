
using System;
using Server;
using Server.Mobiles;
using Server.Engines.Quests;
using Server.Gumps;
using Server.Custom.Townsystem;

namespace Server.Items
{
	public class DispursementNote : Item
	{
        private Town m_Town;

        public override string DefaultName
        {
            get
            {
                return "Monetary Disbursement Note";
            }
        }

        [Constructable]
        public DispursementNote(Town town) : base(5360)
		{
            m_Town = town;
		}

        public DispursementNote(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.CloseGump(typeof(DispursementNoteGump));
            from.SendGump(new DispursementNoteGump(from, m_Town));
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

            Town.WriteReference(writer, m_Town);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

            m_Town = Town.ReadReference(reader);
		}
	}
}