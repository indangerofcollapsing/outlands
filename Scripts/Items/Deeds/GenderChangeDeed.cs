using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Custom.Townsystem;
using Server.Custom;

namespace Server.Gumps
{
    public class GenderChangeDeed : Item // Create the item class which is derived from the base item class
    {
        public override string DefaultName
        {
            get { return "a gender change deed"; }
        }

        [Constructable]
        public GenderChangeDeed()
            : base(0x14F0)
        {
            Weight = 1.0;
            LootType = LootType.Blessed;
        }

        public GenderChangeDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            LootType = LootType.Blessed;

            int version = reader.ReadInt();
        }

        public override bool DisplayLootType { get { return false; } }

        public override void OnDoubleClick(Mobile from) // Override double click of the deed to call our target
		{
			if ( !IsChildOf( from.Backpack ) ) // Make sure its in their pack
			{
				 from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
			else
			{
				if (!from.HasGump(typeof(GenderChangeConfirmGump)))
                    from.SendGump(new GenderChangeConfirmGump(this));
			 }
		}

        public class GenderChangeConfirmGump : Gump
        {
            GenderChangeDeed m_Deed;

            public GenderChangeConfirmGump(GenderChangeDeed deed)
                : base(150, 200)
            {
                m_Deed = deed;

                AddPage(0);

                AddBackground(0, 0, 220, 170, 5054);
                AddBackground(10, 10, 200, 150, 3000);

                AddHtml(20, 20, 180, 80, "Are you sure you wish to change your gender?", true, false); // Do you wish to dry dock this boat?

                AddHtmlLocalized(55, 100, 140, 25, 1011011, false, false); // CONTINUE
                AddButton(20, 100, 4005, 4007, 2, GumpButtonType.Reply, 0);

                AddHtmlLocalized(55, 125, 140, 25, 1011012, false, false); // CANCEL
                AddButton(20, 125, 4005, 4007, 1, GumpButtonType.Reply, 0);
            }

            public override void OnResponse(NetState state, RelayInfo info)
            {
                Mobile from = state.Mobile;

                if (from == null)
                    return;

                if (info.ButtonID == 2)
                {
                    if (m_Deed == null || m_Deed.Deleted)
                        return;

                    m_Deed.Delete();

                    from.Body = from.Female ? 0x190 : 0x191;
                    from.Female = !from.Female;

                    from.SendMessage("You will henceforth be a {0}.", from.Female ? "female" : "male");
                }
            }


        }
    }
}