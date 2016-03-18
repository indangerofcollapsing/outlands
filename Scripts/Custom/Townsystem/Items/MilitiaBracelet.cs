using System;
using Server.Custom.Townsystem;

namespace Server.Items
{
    public class MilitiaBracelet : GoldBracelet
    {
        public override string DefaultName { get { return "a militia bracelet"; } }
        private Town m_Town;

        [CommandProperty(AccessLevel.GameMaster)]
        public Town Town
        {
            get { return m_Town; }
            set
            {
                m_Town = value;
            }
        }

        [Constructable]
        public MilitiaBracelet(Town town)
        {
            m_Town = town;
            LootType = Server.LootType.Newbied;
        }

        public override void OnSingleClick(Mobile from)
        {
            if (m_Town != null)
                LabelTo(from, "Bracelet of {0}", m_Town.Definition.FriendlyName);
            else
                base.OnSingleClick(from);
        }


        public MilitiaBracelet(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            Town.WriteReference(writer, m_Town);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Town = Town.ReadReference(reader);
        }
    }
}
