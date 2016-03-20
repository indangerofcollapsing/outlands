using System;
using System.Collections.Generic;
using Server;
using Server.Multis;
using Server.Targeting;
using Server.ContextMenus;
using Server.Gumps;

namespace Server.Items
{
    public interface IDyable
    {
        bool Dye(Mobile from, DyeTub sender);
    }

    public class DyeTub : Item, ISecurable
    {
        private bool m_Redyable;
        private int m_DyedHue;
        private SecureLevel m_SecureLevel;
        private int m_UsesRemaining; // -1: Infinite

        public virtual CustomHuePicker CustomHuePicker { get { return null; } }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            if (m_UsesRemaining > -1)
                LabelTo(from, String.Format("[Uses Remaining: {0}]", m_UsesRemaining));
        }

        public virtual bool AllowRunebooks
        {
            get { return false; }
        }

        public virtual bool AllowFurniture
        {
            get { return false; }
        }

        public virtual bool AllowStatuettes
        {
            get { return false; }
        }

        public virtual bool AllowLeather
        {
            get { return false; }
        }

        public virtual bool AllowDyables
        {
            get { return true; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get { return m_UsesRemaining; }
            set { m_UsesRemaining = value; }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)2); // version

            //version 2
            writer.Write((int)m_SecureLevel);

            //version 1 (IPY)
            writer.Write(m_UsesRemaining);

            //version 0
            writer.Write((bool)m_Redyable);
            writer.Write((int)m_DyedHue);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 2:
                    {
                        m_SecureLevel = (SecureLevel)reader.ReadInt();
                        goto case 1;
                    }
                case 1:
                    {
                        m_UsesRemaining = reader.ReadInt();
                        goto case 0;
                    }
                case 0:
                    {
                        m_Redyable = reader.ReadBool();
                        m_DyedHue = reader.ReadInt();

                        if (version == 0)
                            m_UsesRemaining = -1;

                        break;
                    }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool Redyable
        {
            get
            {
                return m_Redyable;
            }
            set
            {
                m_Redyable = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int DyedHue
        {
            get
            {
                return m_DyedHue;
            }
            set
            {
                if (m_Redyable)
                {
                    m_DyedHue = value;
                    Hue = value;
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level
        {
            get
            {
                return m_SecureLevel;
            }
            set
            {
                m_SecureLevel = value;
            }
        }

        [Constructable]
        public DyeTub() : base(0xFAB)
        {
            m_UsesRemaining = -1;
            Weight = 10.0;
            m_Redyable = true;
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);
            SetSecureLevelEntry.AddTo(from, this, list);
        }

        public DyeTub(Serial serial) : base(serial)
        {
        }

        // Three metallic tubs now.
        public virtual bool MetallicHues { get { return false; } }

        // Select the clothing to dye.
        public virtual int TargetMessage { get { return 500859; } }

        // You can not dye that.
        public virtual int FailMessage { get { return 1042083; } }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(this.GetWorldLocation(), 1))
            {
                from.SendLocalizedMessage(TargetMessage);
                from.Target = new InternalTarget(this);
            }
            else
            {
                from.SendLocalizedMessage(500446); // That is too far away.
            }
        }

        public void UseCharge(Mobile from)
        {
            if (UsesRemaining == -1)
            {
                return;
            }
            else if (--UsesRemaining <= 0)
            {
                Delete();

                if (from != null)
                    from.SendMessage("You use up all the remaining dye in the dyetub.");
            }
        }

        private class InternalTarget : Target
        {
            private DyeTub m_Tub;

            public InternalTarget(DyeTub tub) : base(1, false, TargetFlags.None)
            {
                m_Tub = tub;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Tub == null || m_Tub.Deleted)
                    return;

                if (targeted is Item)
                {
                    Item item = (Item)targeted;

                    if (item is IDyable && m_Tub.AllowDyables)
                    {
                        if (!from.InRange(m_Tub.GetWorldLocation(), 1) || !from.InRange(item.GetWorldLocation(), 1))
                            from.SendLocalizedMessage(500446); // That is too far away.
                        else if (item.Parent is Mobile)
                            from.SendLocalizedMessage(500861); // Can't Dye clothing that is being worn.
                        else if (!item.Movable)
                            from.SendMessage("You cannot dye items that are locked down.");
                        else if (((IDyable)item).Dye(from, m_Tub))
                        {
                            m_Tub.UseCharge(from);

                            from.PlaySound(0x23E);
                        }
                    }
                    else if ((FurnitureAttribute.Check(item) || (item is PotionKeg)) && m_Tub.AllowFurniture)
                    {
                        if (!from.InRange(m_Tub.GetWorldLocation(), 1) || !from.InRange(item.GetWorldLocation(), 1))
                        {
                            from.SendLocalizedMessage(500446); // That is too far away.
                        }
                        else
                        {
                            bool okay = (item.IsChildOf(from.Backpack));

                            if (!okay)
                            {
                                if (item.Parent == null)
                                {
                                    BaseHouse house = BaseHouse.FindHouseAt(item);

                                    if (house == null || (!house.IsLockedDown(item) && !house.IsSecure(item)))
                                        from.SendLocalizedMessage(501022); // Furniture must be locked down to paint it.

                                    else if (!house.IsCoOwner(from))
                                        from.SendLocalizedMessage(501023); // You must be the owner to use this item.

                                    else
                                        okay = true;
                                }
                                else
                                {
                                    from.SendLocalizedMessage(1048135); // The furniture must be in your backpack to be painted.
                                }
                            }

                            if (okay)
                            {
                                m_Tub.UseCharge(from);

                                item.Hue = m_Tub.DyedHue;
                                from.PlaySound(0x23E);
                            }
                        }
                    }
                    else if ((item is Runebook || item is RecallRune) && m_Tub.AllowRunebooks)
                    {
                        if (!from.InRange(m_Tub.GetWorldLocation(), 1) || !from.InRange(item.GetWorldLocation(), 1))
                        {
                            from.SendLocalizedMessage(500446); // That is too far away.
                        }
                        else if (!item.Movable)
                        {
                            from.SendLocalizedMessage(1049776); // You cannot dye runes or runebooks that are locked down.
                        }
                        else
                        {
                            m_Tub.UseCharge(from);

                            item.Hue = m_Tub.DyedHue;
                            from.PlaySound(0x23E);
                        }
                    }
                    else if (item is MonsterStatuette && m_Tub.AllowStatuettes)
                    {
                        if (!from.InRange(m_Tub.GetWorldLocation(), 1) || !from.InRange(item.GetWorldLocation(), 1))
                        {
                            from.SendLocalizedMessage(500446); // That is too far away.
                        }
                        else if (!item.Movable)
                        {
                            from.SendLocalizedMessage(1049779); // You cannot dye statuettes that are locked down.
                        }
                        else
                        {
                            m_Tub.UseCharge(from);

                            item.Hue = m_Tub.DyedHue;
                            from.PlaySound(0x23E);
                        }
                    }
                    else if ((item is BaseArmor && (((BaseArmor)item).MaterialType == ArmorMaterialType.Leather || ((BaseArmor)item).MaterialType == ArmorMaterialType.Studded) || item is ElvenBoots || item is WoodlandBelt) && m_Tub.AllowLeather)
                    {
                        if (!from.InRange(m_Tub.GetWorldLocation(), 1) || !from.InRange(item.GetWorldLocation(), 1))
                        {
                            from.SendLocalizedMessage(500446); // That is too far away.
                        }
                        else if (!item.Movable)
                        {
                            from.SendLocalizedMessage(1042419); // You may not dye leather items which are locked down.
                        }
                        else if (item.Parent is Mobile)
                        {
                            from.SendLocalizedMessage(500861); // Can't Dye clothing that is being worn.
                        }
                        else
                        {
                            m_Tub.UseCharge(from);
                            item.Hue = m_Tub.DyedHue;
                            from.PlaySound(0x23E);
                            
                        }
                    }
                    else
                    {
                        from.SendLocalizedMessage(m_Tub.FailMessage);
                    }
                }
                else
                {
                    from.SendLocalizedMessage(m_Tub.FailMessage);
                }
            }
        }
    }
}