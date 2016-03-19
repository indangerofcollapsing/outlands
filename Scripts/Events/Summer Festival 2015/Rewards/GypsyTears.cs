using System;

namespace Server.Items
{
    public class GypsyTears : Item
    {
        public override bool AlwaysAllowDoubleClick { get { return true; } }

        private bool m_Male = true;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Male
        {
            get { return m_Male; }
            set { m_Male = value; } 
        }

        [Constructable]
        public GypsyTears() : base(0x1836)
        {
            Name = ("a flask of gypsy tears");
            Weight = 1.0;

            if (Utility.RandomDouble() <= .50)
                m_Male = false;            
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.SendMessage("They look surprisingly delicious.");

            if (m_Male)
                from.SendSound(0x422);

            else
                from.SendSound(0x313);
        }

        public GypsyTears(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version

            writer.Write(m_Male);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            m_Male = reader.ReadBool();
        }
    }
}