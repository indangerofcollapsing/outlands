using Server.Mobiles;
using Server.Targeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Items
{
    class PetDye : Item
    {
        protected int m_Charges;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges { get { return m_Charges; } set { m_Charges = value; } }

        public override string DefaultName
        {
            get
            {
                return "Pet Dye";
            }
        }

        [Constructable]
        public PetDye()
            : base(4033)
        {
            m_Charges = 5;
        }

        public PetDye(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
            writer.Write(m_Charges);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_Charges = reader.ReadInt();
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
            LabelTo(from, String.Format("[charges: {0}]", m_Charges));
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1010095); //Must be on your person to use.
                return;
            }

            from.SendMessage("Target the pet you wish to dye.");
            from.Target = new InternalTarget(this);

            base.OnDoubleClick(from);
        }

        public virtual bool Use(Mobile from, BaseCreature pet)
        {
            if (Deleted)
                return false;

            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1010095); //Must be on your person to use.
                return false;
            }

            if (pet == null || pet.Deleted)
                return false;

            if (pet.IsHenchman)
            {
                from.SendMessage("They politely decline the offer.");
                return false;
            }

            if (!pet.Controlled || pet.ControlMaster != from)
            {
                from.SendMessage("The pet must belong to you!");
                return false;
            }

            int chargesNeeded = pet.ControlSlots;

            if (m_Charges - chargesNeeded < 0)
            {
                from.SendMessage("You do not have enough charges to dye that pet.");
                return false;
            }

            pet.Hue = Hue;

            from.SendMessage("You have dyed your pet.");

            m_Charges -= chargesNeeded;
            if (m_Charges <= 0)
            {
                from.SendMessage("You have used up the pet dye.");
                Delete();
            }

            return true;
        }

        public class InternalTarget : Target
        {
            PetDye m_Dye;

            public InternalTarget(PetDye dye)
                : base(12, false, TargetFlags.None)
            {
                m_Dye = dye;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Dye == null || m_Dye.Deleted)
                    return;

                if (!(targeted is BaseCreature))
                {
                    from.SendMessage("You must target a pet you own.");
                    return;
                }

                m_Dye.Use(from, (BaseCreature)targeted);
            }
        }
    }
}
