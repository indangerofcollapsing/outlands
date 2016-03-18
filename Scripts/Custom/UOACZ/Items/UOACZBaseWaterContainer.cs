using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
    public class UOACZBaseWaterContainer : Item
    {
        private int m_Charges = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges
        {
            get { return m_Charges; }
            set
            {
                m_Charges = value;

                if (m_Charges > MaxCharges)
                    m_Charges = MaxCharges;

                if (m_Charges < 0)
                    m_Charges = 0;

                Weight = m_Charges * WeightPerCharge;
            }
        }

        public virtual int MaxCharges { get { return 5; } }

        public virtual int EmptyItemID { get { return 3715; } }
        public virtual int FullItemID { get { return 3715; } }

        public virtual int WeightPerCharge { get { return 1; } }   

        [Constructable]
        public UOACZBaseWaterContainer(): base(3715)
        {
            Name = "a water container";
            Weight = 1;
        }

        public UOACZBaseWaterContainer(Serial serial): base(serial)
        {
        }
        
        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("That item must be in your pack in order to use it.");
                return;
            }

            if (Charges == 0)
            {
                from.SendMessage("Where will you fill this container from?");
            }

            else if (Charges > 0 && Charges < MaxCharges)
            {
                from.SendMessage("Where will you fill this container from or what will you pour this into?");
            }

            else
            {
                from.SendMessage("What will you pour this into?");
            }

            //TARGET
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version          
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}