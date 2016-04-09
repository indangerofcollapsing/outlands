using System;
using Server;
using Server.Items;
using Server.Targeting;
using Server.Network;

namespace Server.Items
{
    public enum SoilEnhancerQualityType
    {
        Low,
        Average,
        High,
        Premium,
        Perfect
    }

    public class SoilEnhancer : Item
    {
        public virtual double MaxSoilQuality { get { return 10.0; } }
        public virtual double SoilQualityIncrease { get { return 10.0; } }
        public virtual SoilEnhancerQualityType QualityType { get { return SoilEnhancerQualityType.Low; } }

        public virtual string SoilItemName { get { return "soil enhancer"; } }
        public virtual int SoilItemId { get { return 4154; } }
        public virtual int SoilItemHue { get { return 0; } }

        public virtual int MaxCharges { get { return 1; } }
        public virtual int WeightPerCharge { get { return 1; } }

        public int m_Charges = 1;
        [CommandProperty(AccessLevel.Administrator)]
        public int Charges
        {
            get { return m_Charges; }
            set 
            {
                m_Charges = value;

                Weight = m_Charges * WeightPerCharge;
            }
        }

        [Constructable]
        public SoilEnhancer(): base(4154)
        {
            Name = SoilItemName;
            ItemID = SoilItemId;
            Hue = SoilItemHue;

            Charges = MaxCharges;
        }

        public SoilEnhancer(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, SoilItemName);
            LabelTo(from, "(" + QualityType.ToString().ToLower() +  " quality soil enhancer)");

            if (MaxCharges > 1)
                LabelTo(from, "[" + Charges.ToString() + " charges remaining]");
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            //Version 0
            writer.Write(m_Charges);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_Charges = reader.ReadInt();
            }
        }
    }
}