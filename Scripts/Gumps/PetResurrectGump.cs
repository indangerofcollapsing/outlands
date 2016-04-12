using System;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Gumps;

using Server.Custom.Items;

namespace Server.Gumps
{
    public class PetResurrectGump : Gump
    {
        private BaseCreature m_Follower;
        private double m_HitsScalar;
        private Mobile m_Healer;
        private VeterinarySalts m_Salts;

        public PetResurrectGump(Mobile from, BaseCreature follower, VeterinarySalts salts = null): this(from, follower, 0.0, salts)
        {
        }

        public PetResurrectGump(Mobile from, BaseCreature follower, double hitsScalar, VeterinarySalts salts = null)
            : base(50, 50)
        {
            from.CloseGump(typeof(PetResurrectGump));

            m_Follower = follower;
            m_HitsScalar = hitsScalar;
            m_Healer = from;
            m_Salts = salts;

            AddPage(0);

            AddBackground(10, 10, 265, 140, 0x242C);

            AddItem(205, 40, 0x4);
            AddItem(227, 40, 0x5);

            AddItem(180, 78, 0xCAE);
            AddItem(195, 90, 0xCAD);
            AddItem(218, 95, 0xCB0);

            AddHtmlLocalized(30, 30, 150, 75, 1049665, false, false); // <div align=center>Wilt thou sanctify the resurrection of:</div>
            AddHtml(30, 70, 150, 25, String.Format("<div align=CENTER>{0}</div>", follower.Name), true, false);

            AddButton(40, 105, 0x81A, 0x81B, 0x1, GumpButtonType.Reply, 0); // Okay
            AddButton(110, 105, 0x819, 0x818, 0x2, GumpButtonType.Reply, 0); // Cancel
        }

        private void ResurrectPet()
        {
            m_Follower.PlaySound(0x214);
            m_Follower.FixedEffect(0x376A, 10, 16);
            m_Follower.ResurrectPet();
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (m_Follower.Deleted || !m_Follower.IsBonded || !m_Follower.IsDeadPet)
                return;

            Mobile from = state.Mobile;

            if (info.ButtonID == 1)
            {
                if (m_Follower.Map == null || !m_Follower.Map.CanFit(m_Follower.Location, 16, false, false))
                {
                    from.SendLocalizedMessage(503256); // You fail to resurrect the creature.
                    return;
                }

                /*
                if (m_Follower.CanBeResurrectedThroughVeterinary && m_Salts == null)
                {
                    ResurrectPet();
                }
                 * */

                if (m_Salts != null && m_Salts.Charges >= m_Follower.ControlSlots)
                {
                    ResurrectPet();
                    m_Salts.Charges -= m_Follower.ControlSlots;

                    if (m_Follower.ResurrectionsRemaining != -1)
                    {
                        m_Follower.ResurrectionsRemaining--;

                        if (m_Follower.ResurrectionsRemaining > 0)
                            from.SendMessage("You resurrect your target. They may be resurrected " + m_Follower.ResurrectionsRemaining.ToString() + " more time(s) before they fade from creation."); 
                        else
                            from.SendMessage("You resurrect your target, however you are certain the next time they perish they will fade from creation."); 
                    }

                    if (m_Salts.Charges <= 0)
                        m_Salts.Delete();
                }

                else
                    from.SendMessage("There are not enough charges remaining to resurrect this creature.");
            }
        }
    }
}