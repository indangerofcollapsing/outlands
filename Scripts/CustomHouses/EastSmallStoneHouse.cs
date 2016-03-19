using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Multis.Deeds;

namespace Server.Multis
{
    public class EastSmallStoneHouse : BaseHouse
    {
        public static Rectangle2D[] AreaArray = new Rectangle2D[] { new Rectangle2D(-3, -3, 7, 7) };
        public override Rectangle2D[] Area { get { return AreaArray; } }
        public override Point3D BaseBanLocation { get { return new Point3D(4, 2, 0); } }
        public override int DefaultPrice { get { return 43800; } }

        public EastSmallStoneHouse(Mobile owner): base(0x15F, owner, 425, 3)
        {
            uint keyValue = CreateKeys(owner);
            AddEastDoor(2, -1, 7, keyValue);
            SetSign(4, -2, 11);
            this.ChangeSignType(0x0BD1);
        }

        public EastSmallStoneHouse(Serial serial)
            : base(serial)
        {
        }

        public override HouseDeed GetDeed()
        {
            return new EastSmallStoneHouseDeed();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
