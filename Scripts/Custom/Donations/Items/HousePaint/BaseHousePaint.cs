using System;
using System.Collections.Generic;
using Server.Targeting;
using Server.Mobiles;
using Server.Multis;

namespace Server.Items
{
    public class BaseHousePaint : Item
    {
        protected int m_Charges;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges { get { return m_Charges; } set { m_Charges = value; } }

        public BaseHousePaint(int itemID)
            : base(itemID)
        {
            LootType = Server.LootType.Blessed;
            m_Charges = 1;
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
            LabelTo(from, String.Format("[charges: {0}]", m_Charges));
        }

        public BaseHousePaint(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1010095); //Must be on your person to use.
                return;
            }

            from.SendMessage("Target the house you wish to apply this to.");
            from.Target = new InternalTarget(this);

            base.OnDoubleClick(from);
        }

        public virtual bool CanPaint(BaseHouse house) 
        {
            return true;
        }

        public virtual bool Use(Mobile from, BaseHouse house)
        {
            if (Deleted)
                return false;

            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1010095); //Must be on your person to use.
                return false;
            }

            if (house == null || house.Deleted)
                return false;

            house.Hue = Hue;

            from.SendMessage("You have painted the house.");

            if (--m_Charges <= 0)
            {
                from.SendMessage("You have used up the house paint.");
                Delete();
            }

            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(1);

            writer.Write(m_Charges);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();

            switch (version)
            {
                case 1:
                    {
                        m_Charges = reader.ReadInt();
                        break;
                    }
            }
        }

        public class InternalTarget : Target
        {
            BaseHousePaint m_Paint;

            public InternalTarget(BaseHousePaint paint)
                : base(12, false, TargetFlags.None)
            {
                m_Paint = paint;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Paint == null || m_Paint.Deleted)
                    return;

                if (!(targeted is BaseHouse))
                {
                    from.SendMessage("This house decoration will not work on that.");
                    return;
                }

                m_Paint.Use(from, (BaseHouse)targeted);
            }
        }
    }
}
