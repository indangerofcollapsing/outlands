using System;
using System.Collections.Generic;
using Server.Engines.Plants;
using Server.Targeting;

namespace Server.Items
{
    public class PlantGrowthAccelerant : Item
    {
        private int m_Charges;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges
        {
            get { return m_Charges; }
            set
            { 
                m_Charges = value;                
                Weight = 1 * value;
            }
        }

        [Constructable]
        public PlantGrowthAccelerant() : this(5)
        {
        }

        [Constructable]
        public PlantGrowthAccelerant(int charges) : base(0x103A)
        {
            m_Charges = charges;

            Name = "plant growth accelerant";
            Hue = 2004;

            Weight = 1 * charges;          
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            LabelTo(from, "(charges: " + m_Charges.ToString() + ")");
        }

        public PlantGrowthAccelerant(Serial serial) : base(serial)
        {
        }        

        public override void OnDoubleClick(Mobile from)
        {
            if (!this.IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042664); // You must have the object in your backpack to use it.
                return;
            }

            from.Target = new InternalTarget(this);
            from.SendMessage("Target a plant to accelerate its growth or another bag of plant growth accelerant to combine this with.");
        }
        

        private class InternalTarget : Target
        {
            private readonly PlantGrowthAccelerant m_PlantGrowthAccelerant;

            public InternalTarget(PlantGrowthAccelerant plantgrowthaccelerant): base(3, true, TargetFlags.None)
            {
                this.m_PlantGrowthAccelerant = plantgrowthaccelerant;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_PlantGrowthAccelerant == null || m_PlantGrowthAccelerant.Deleted)
                    return;

                if (!this.m_PlantGrowthAccelerant.IsChildOf(from.Backpack))
                {
                    from.SendLocalizedMessage(1042664); // You must have the object in your backpack to use it.
                    return;
                }

                if (targeted == null)
                    return;

                if (targeted is PlantItem)
                {
                    PlantItem plant = targeted as PlantItem;
                   
                    if (plant.PlantStatus > PlantStatus.BowlOfDirt && plant.PlantStatus < PlantStatus.Stage9)
                    {
                        plant.PlantStatus++;

                        from.SendMessage("You mix some accelerants into the plants soil.");
                        from.SendSound(0x59);

                        m_PlantGrowthAccelerant.ConsumeCharge();
                    }

                    else                    
                        from.SendMessage("You think it would be a waste to pour your accelerants on that...");

                    return;
                }

                else if (targeted is PlantGrowthAccelerant)
                {
                    PlantGrowthAccelerant otherAccelerant = targeted as PlantGrowthAccelerant;

                    if (otherAccelerant == null || otherAccelerant.Charges >= 100)
                    {
                        from.SendMessage("That sack of accelerants is filled to the top.");
                        return;
                    }

                    int maxChargesToAdd = 100 - otherAccelerant.Charges;
                    int chargesAdded = m_PlantGrowthAccelerant.Charges;

                    if (chargesAdded > maxChargesToAdd)
                        chargesAdded = maxChargesToAdd;

                    otherAccelerant.Charges += chargesAdded;

                    if (m_PlantGrowthAccelerant.Charges - chargesAdded <= 0)
                        m_PlantGrowthAccelerant.Delete();

                    else
                        m_PlantGrowthAccelerant.Charges -= chargesAdded;

                    from.SendMessage("You combine the accelerants.");
                    from.SendSound(0x59);

                    return;
                }

                else
                {
                    from.SendMessage("That does not appear to work with that.");
                    return;
                }
            }
        }

        public void ConsumeCharge()
        {
            m_Charges--;

            if (m_Charges < 1)
                Delete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            writer.Write(m_Charges);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Charges = reader.ReadInt();
        }
    }
}