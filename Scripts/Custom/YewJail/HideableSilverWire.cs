using System;
using System.Collections.Generic;
using Server.Items;
using Server;

namespace Server.Items
{
    public class HideableSilverWire : Item, IHideable
    {

        [Constructable]
        public HideableSilverWire()
            : this(1)
        {
        }

        [Constructable]
        public HideableSilverWire(int amount)
            : base(0x1877)
        {
            Stackable = true;
            Weight = 5.0;
            Amount = amount;
            Visible = true;
        }

        public HideableSilverWire(Serial serial)
            : base(serial)
        {
        }

        private int m_HideLevel = 0;
        private bool m_Findable = true;
        private Mobile m_Finder;

        [CommandProperty(AccessLevel.GameMaster)]
        public int HideLevel 
        { 
            get { return m_HideLevel; }
            set { m_HideLevel = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Findable
        {
            get { return m_Findable; }
            set { m_Findable = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Finder { 
            get { return m_Finder; }
            set { m_Finder = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsVisible
        {
            get { return Visible; }
            set { Visible = value; }
        }

        public void Reveal(Mobile from)
        {
            this.Visible = true;
            from.LocalOverheadMessage(Network.MessageType.Regular,from.EmoteHue,true, "You find hidden wire under the bed!");
        }
        public override bool OnDroppedInto(Mobile from, Container target, Point3D p)
        {
            if (from.Player)
            {
                if (((Server.Mobiles.PlayerMobile)from).YewJailed && target.FindItemByType(typeof(Flint)) != null && target.FindItemByType(typeof(Manure)) != null)
                {
                    from.SendGump(new YewJail.YewJailBombGump(((Mobiles.PlayerMobile)from).m_YewJailItem));
                }
            }
            return base.OnDroppedInto(from, target, p);
        }
        public override bool OnDroppedToMobile(Mobile from, Mobile target)
        {
            if (target.Player)
            {
                Container target2 = target.Backpack;

                if (((Server.Mobiles.PlayerMobile)from).YewJailed && target2.FindItemByType(typeof(Flint)) != null && target2.FindItemByType(typeof(Manure)) != null)
                {
                    from.SendGump(new YewJail.YewJailBombGump(((Mobiles.PlayerMobile)from).m_YewJailItem));
                }
            }
            return base.OnDroppedToMobile(from, target);
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            writer.Write((int)m_HideLevel);
            writer.Write((bool)m_Findable);
            writer.WriteMobile((Mobile)m_Finder);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            int m_HideLevel = reader.ReadInt();
            bool m_Findable = reader.ReadBool();
            Mobile m_Finder = reader.ReadMobile();
        }
    }
}
