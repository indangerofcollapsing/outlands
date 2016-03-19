/***************************************************************************
 *                            HeavyCannon.cs
 *                            ------------------
 *   begin                : August 2010
 *   author               : Sean Stavropoulos
 *   email                : sean.stavro@gmail.com
 *
 *
 ***************************************************************************/
using System;
using Server;
using Server.Items;
using Server.Multis;

namespace Server.Custom.Pirates
{
    public class HeavyCannon : BaseCannon
    {
        public override BaseCannonDeed GetDeed { get { return new HeavyCannonDeed(); } }

        public override int NorthID { get { return 0x2c1; } }
        public override int EastID { get { return 0x2c2; } }
        public override int SouthID { get { return 0x2c3; } }
        public override int WestID { get { return 0x2c4; } }       

        public HeavyCannon(BaseBoat b)
            : base(0x2c1, b)
        {
        }

        public HeavyCannon(Serial serial)
            : base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, "Heavy Cannon");
            LabelTo(from, "[Charged: {0}/{1}]", m_CurrentCharges, MaxCharges);

            base.OnSingleClick(from);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class HeavyCannonDeed : BaseCannonDeed
    {
        public override CannonTypes CannonType { get { return CannonTypes.Heavy; } }

        public override BaseCannon Addon { get { return new HeavyCannon(null); } }

        [Constructable]
        public HeavyCannonDeed()
        {
            Name = "Heavy Cannon Deed";
        }

        public HeavyCannonDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
    public class HeavyCannonPlans : BaseCannonPlans
    {
        [Constructable]
        public HeavyCannonPlans()
        {
            Name = "Heavy Cannon Plans";
        }

        public HeavyCannonPlans(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}