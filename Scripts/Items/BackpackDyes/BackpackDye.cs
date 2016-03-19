using Server.Mobiles;
using Server.Targeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Items
{
    class BackpackDye : Item
    {
        private int m_DyeHue = 2500;
        [CommandProperty(AccessLevel.GameMaster)]
        public int DyeHue
        {
            get { return m_DyeHue; }
            set
            { 
                m_DyeHue = value;
                Hue = m_DyeHue;
            }
        }

        private int m_Charges = 3;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges
        {
            get { return m_Charges; }
            set { m_Charges = value; } 
        }

        [Constructable]
        public BackpackDye(): base(3622) //4033
        {
            Name = "backpack dye";
            Hue = m_DyeHue;
        }

        public BackpackDye(Serial serial): base(serial)
        {
        }        

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            LabelTo(from, "(hue " + m_DyeHue.ToString() + ")");
            LabelTo(from, String.Format("[charges: {0}]", m_Charges));
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1010095); //Must be on your person to use.
                return;
            }

            from.SendMessage("Target your character's backpack on your paper doll.");
            from.Target = new InternalTarget(this);            
        }

        public class InternalTarget : Target
        {
            BackpackDye m_Dye;

            public InternalTarget(BackpackDye dye): base(12, false, TargetFlags.None)
            {
                m_Dye = dye;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (from == null || m_Dye == null) return;
                if (!from.Alive || from.Deleted || m_Dye.Deleted) return;

                if (!m_Dye.IsChildOf(from.Backpack))
                {
                    from.SendLocalizedMessage(1010095); //Must be on your person to use.
                    return;
                }

                Backpack backpack = null;

                if (!(targeted is Backpack))
                {
                    from.SendMessage("You must target a backpack.");
                    return;
                }

                backpack = targeted as Backpack;

                if (from.FindItemOnLayer(Layer.Backpack) != backpack)
                {
                    from.SendMessage("You must target a backpack in your paper doll.");
                    return;
                }

                if (backpack.Hue == m_Dye.DyeHue)
                {
                    from.SendMessage("Your backpack is already that color.");
                    return;
                }

                backpack.Hue = m_Dye.DyeHue;

                from.SendMessage("You dye your backpack.");
                from.PlaySound(0x23E);

                m_Dye.Charges--;

                if (m_Dye.Charges <= 0)                
                    m_Dye.Delete();                
            }            
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            //Version 0
            writer.Write(m_DyeHue);
            writer.Write(m_Charges);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version >= 0)
            {
                m_DyeHue = reader.ReadInt();
                m_Charges = reader.ReadInt();
            }
        }
    }
}
