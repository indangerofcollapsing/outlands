using System;
using System.Collections.Generic;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Items
{
    public class MasterSkillScrollSpyglass : Item
    {
        public override string DefaultName
        {
            get
            {
                return "Master Skill Scroll Spyglass";
            }
        }

        private int m_Charges;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges
        {
            get { return m_Charges; }
            set { m_Charges = value; }
        }

        [Constructable]
        public MasterSkillScrollSpyglass()
            : base(5365)
        {
            m_Charges = 10;
            LootType = Server.LootType.Blessed;
        }

        public MasterSkillScrollSpyglass(Serial serial)
            : base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
            LabelTo(from, "Charges: {0}", m_Charges);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1010095); //Must be on your person to use.
                return;
            }

            from.SendMessage("Target the monster you wish to increase the skill scroll drop rate for.");
            from.Target = new InternalTarget(this);

            base.OnDoubleClick(from);
        }

        public void UseCharge(Mobile from, BaseCreature creature)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1010095); //Must be on your person to use.
                return;
            }

            if (creature == null || creature.Deleted)
                return;

            creature.AdeptDropIncrease *= 10;
            m_Charges--;

            from.SendMessage("You have studied this monster well, and the chances of learning from it has increased.");

            if (m_Charges < 1)
            {
                from.SendMessage("You have worn out the spyglass");
                Delete();
                return;
            }
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

            if (version == 0)
                m_Charges = 10;
        }

        public class InternalTarget : Target
        {
            MasterSkillScrollSpyglass m_Spyglass;

            public InternalTarget(MasterSkillScrollSpyglass spyglass)
                : base(12, false, TargetFlags.None)
            {
                m_Spyglass = spyglass;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Spyglass == null || m_Spyglass.Deleted)
                    return;

                if (!(targeted is BaseCreature))
                {
                    from.SendMessage("You must target a creature.");
                    return;
                }

                m_Spyglass.UseCharge(from, targeted as BaseCreature);
            }
        }
    }
}
