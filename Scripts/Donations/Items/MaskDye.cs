using Server.Custom.Items;
using Server.Items;
using Server.Targeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Donations.Items
{
    class MaskDye : Item
    {
        private static List<Type> m_PossibleTypes = new List<Type>()
        {
            typeof(HornedTribalMask),
            typeof(TribalMask),
            typeof(BearMask),
            typeof(DeerMask),
            typeof(OrcMask),
            typeof(OrcishKinMask),
            typeof(OgreLordMask),
        };

        private static int[] m_RareHues = new int[] { 1281, 2052, 1150, 1266 };

        public static MaskDye RandomDye()
        {
            return new MaskDye() { Hue = Utility.RandomList() };
        }

        public override string DefaultName
        {
            get { return string.Format("Mask Dye: {0}", Hue); }
        }

        [Constructable]
        public MaskDye()
            : base(0xE26)
        {
        }

        public MaskDye(Serial serial) : base(serial) {}

        public override void Serialize(GenericWriter writer)
        {
 	         base.Serialize(writer);
             writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1010095); //Must be on your person to use.
                return;
            }

            from.SendMessage("Target the mask you wish to dye.");
            from.Target = new InternalTarget(this);
        }

        public void Use(Mobile from, Item target)
        {
            if (!MaskDye.m_PossibleTypes.Contains(target.GetType()))
            {
                from.SendMessage("That is not a valid mask type.");
                return;
            }

            target.Hue = Hue;
            Delete();
        }

        private class InternalTarget : Target
        {
            public MaskDye m_Dye;

            public InternalTarget(MaskDye dye)
                : base(12, false, TargetFlags.None)
            {
                m_Dye = dye;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Dye == null || m_Dye.Deleted)
                    return;
                if (!(targeted is Item))
                {
                    from.SendMessage("That is not a valid mask type.");
                    return;
                }

                m_Dye.Use(from, targeted as Item);
            }
        }
    }
}
