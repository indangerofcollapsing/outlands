/***************************************************************************
 *                           InfiltrationDust.cs
 *                            ------------------
 *   begin                : march 2011
 *   author               : Sean Stavropoulos
 *   email                : sean.stavro@gmail.com
 *
 *
 ***************************************************************************/
namespace Server.Items
{
    public class InfiltrationDust : Item
    {
        public enum DustTypes
        {
            Alpha,
            Beta
        }

        public override string DefaultName { get { return "Infiltration Dust"; } }
        private DustTypes m_DustType;

        [CommandProperty(AccessLevel.GameMaster)]
        public DustTypes DustType { get { return m_DustType; } set { m_DustType = value; } }

        [Constructable]
        public InfiltrationDust() : this( 1 )
		{
		}

        [Constructable]
        public InfiltrationDust(int amt)
            : this(amt, DustTypes.Alpha)
        {
        }

        [Constructable]
        public InfiltrationDust(int amt, DustTypes type)
            : base(3983)
        {
            Hue = 2103;
            Stackable = true;
            Amount = amt;
            m_DustType = type;
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
            LabelTo(from, m_DustType == DustTypes.Alpha ? "(Alpha)" : "(Beta)");
        }

        public InfiltrationDust(Serial serial)
            : base(serial)
        {
        }

        public override bool StackWith(Mobile from, Item dropped, bool playSound)
        {
            if (!(dropped is InfiltrationDust))
                return false;
            var dust = dropped as InfiltrationDust;

            return m_DustType == dust.DustType && base.StackWith(from, dropped, playSound);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
            writer.Write((byte)m_DustType);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_DustType = (DustTypes)reader.ReadByte();
        }
    }
}