using System;
using Server;
using Server.Gumps;

namespace Server.Mobiles
{
    public class PricedHealer : BaseHealer
    {
        private int m_Price;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Price
        {
            get { return m_Price; }
            set { m_Price = value; }
        }

        [Constructable]
        public PricedHealer()
            : this(5000)
        {
        }

        [Constructable]
        public PricedHealer(int price)
        {
            m_Price = price;

            if (!Core.AOS)
                NameHue = 0x35;
        }

        public override bool IsInvulnerable { get { return true; } }

        public override void InitSBInfo()
        {
        }

        public override bool HealsYoungPlayers { get { return false; } }

        public override void OfferResurrection(Mobile m)
        {
            Direction = GetDirectionTo(m);

            m.PlaySound(0x214);
            m.FixedEffect(0x376A, 10, 16);

            m.CloseGump(typeof(ResurrectGump));
            m.SendGump(new ResurrectGump(m, this, m_Price));
        }

        public override bool CheckResurrect(Mobile m)
        {
            PlayerMobile player = m as PlayerMobile;

            if (player == null)
                return false;

            if (player.Criminal)
            {
                Say(501222); // Thou art a criminal.  I shall not resurrect thee.
                return false;
            }

            else if (player.ShortTermMurders >= 5)
            {
                Say(501223); // Thou'rt not a decent and good person. I shall not resurrect thee.
                return false;
            }

            else if (player.RestitutionFee > 0 || player.MurdererDeathGumpNeeded)
            {
                Say("Thou has not paid sufficiently for your crimes and I shall not ressurect thee.");
                return false;
            }

            else if (player.Karma < 0)
            {
                Say(501224); // Thou hast strayed from the path of virtue, but thou still deservest a second chance.
                return true;
            }

            return true;
        }

        public PricedHealer(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((int)m_Price);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        m_Price = reader.ReadInt();
                        break;
                    }
            }
        }
    }
}