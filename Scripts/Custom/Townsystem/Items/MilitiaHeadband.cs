using System;
using Server;
using Server.Mobiles;
using Server.Custom.Townsystem;

namespace Server.Items
{

    public class ArmoredMilitiaHeadband : BaseArmoredHat
    {
        public override string DefaultName
        {
            get
            {
                return "an armored militia headband";
            }
        }

        private Town m_Town;

        [CommandProperty(AccessLevel.GameMaster)]
        public Town Town
        {
            get { return m_Town; }
            set
            {
                m_Town = value;
                if (m_Town != null)
                    Hue = m_Town.HomeFaction.Definition.HuePrimary;
            }
        }

        public ArmoredMilitiaHeadband(Town town)
            : base(0x1540)
        {
            Town = town;
        }

        public ArmoredMilitiaHeadband(Serial serial)
            : base(serial)
        {
        }
        public override bool CanEquip(Mobile from)
        {
            return Town.Find(from) == m_Town && base.CanEquip(from);
        }

        public new bool Scissor(Mobile from, Scissors scissors)
        {
            from.SendLocalizedMessage(502440); // Scissors can not be used on that to produce anything.
            return false;
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

            switch (version)
            {
                case 0:
                    Town = Town.ReadReference(reader);
                    break;
            }
        }

        public override bool Nontransferable { get { return true; } }

        public override bool OnDroppedToWorld(Mobile from, Point3D p)
        {
            if (from.AccessLevel == AccessLevel.Player)
            {
                from.SendMessage("Militia items are non-transferrable");
                return false;
            }
            return base.OnDroppedToWorld(from, p);
        }

        public override bool DropToMobile(Mobile from, Mobile target, Point3D p)
        {
            if (from.AccessLevel == AccessLevel.Player && from != target)
            {
                from.SendMessage("Militia items are non-transferrable");
                return false;
            }

            return base.DropToMobile(from, target, p);
        }

        public override bool DropToItem(Mobile from, Item target, Point3D p)
        {
            if (from.AccessLevel == AccessLevel.Player && target != from.Backpack && target != from.BankBox)
            {
                from.SendMessage("Militia items are non-transferrable");
                return false;
            }

            return base.DropToItem(from, target, p);
        }

    }
    public class MilitiaHeadband : Bandana
    {
        public override string DefaultName { get { return "a militia headband"; } }

        private Town m_Town;

        [CommandProperty(AccessLevel.GameMaster)]
        public Town Town
        {
            get { return m_Town; }
            set
            {
                m_Town = value;
                if (m_Town != null)
                    Hue = m_Town.HomeFaction.Definition.HuePrimary;
            }
        }

        [Constructable]
        public MilitiaHeadband(Town town)
        {
            Town = town;
            LootType = Server.LootType.Newbied;
        }

        public MilitiaHeadband(Serial serial)
            : base(serial)
        {
        }

        public override bool CanEquip(Mobile from)
        {
            // town could be null from version 0 items
            if (m_Town == null)
                Town = Town.Find(from);
            return Town.Find(from) == m_Town && base.CanEquip(from);
        }

        public new bool Scissor(Mobile from, Scissors scissors)
        {
            from.SendLocalizedMessage(502440); // Scissors can not be used on that to produce anything.
            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);

            Town.WriteReference(writer, m_Town);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    Town = Town.ReadReference(reader);
                    goto case 0;
                case 0:
                    if (version < 1)
                        Faction.ReadReference(reader);
                    break;
            }
        }

        public override bool Nontransferable { get { return true; } }

        public override bool OnDroppedToWorld(Mobile from, Point3D p)
        {
            if (from.AccessLevel == AccessLevel.Player)
            {
                from.SendMessage("Militia items are non-transferrable");
                return false;
            }
            return base.OnDroppedToWorld(from, p);
        }

        public override bool DropToMobile(Mobile from, Mobile target, Point3D p)
        {
            if (from.AccessLevel == AccessLevel.Player && from != target)
            {
                from.SendMessage("Militia items are non-transferrable");
                return false;
            }

            return base.DropToMobile(from, target, p);
        }

        public override bool DropToItem(Mobile from, Item target, Point3D p)
        {
            if (from.AccessLevel == AccessLevel.Player && target != from.Backpack && target != from.BankBox)
            {
                from.SendMessage("Militia items are non-transferrable");
                return false;
            }

            return base.DropToItem(from, target, p);
        }
    }
}