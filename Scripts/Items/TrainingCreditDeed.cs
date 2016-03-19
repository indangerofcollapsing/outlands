using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;

namespace Server.Items
{
    public class TrainingCreditDeed : Item
    {
        private int m_Value = 1000;
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.Administrator)]
        public int Value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }

        [Constructable]
        public TrainingCreditDeed(): base(5359)
        {
            Name = "a training credit deed";

            Hue = 2213;
            Weight = .1;

            LootType = LootType.Newbied;
        }

        public TrainingCreditDeed(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, Name);
            LabelTo(from, "(" + m_Value.ToString() + " credits remaining)");
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.SendMessage("Training credit deeds may be given to NPC trainers in place of gold for training skills.");
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version     
     
            //Version 0
            writer.Write(m_Value);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_Value = reader.ReadInt();
            }
        }
    }    
}