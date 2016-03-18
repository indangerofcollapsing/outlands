using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Multis.Deeds;

namespace Server.Multis
{
	public class SmallStoneTempleHouse : BaseHouse 
	{
        public static Rectangle2D[] AreaArray = new Rectangle2D[] { new Rectangle2D(-4, -4, 10, 10) };
		public override Rectangle2D[] Area{ get{ return AreaArray; } }
		public override Point3D BaseBanLocation{ get{ return new Point3D( 5, 0, 1 ); } }
		public override int DefaultPrice{ get{ return 250000; } }

		public SmallStoneTempleHouse( Mobile owner ) : base( 0x149, owner, 846, 4 )
		{
            uint keyValue = CreateKeys(owner);
            SetSign(3, 0, 40);
            BaseHouseDoor door = (BaseHouseDoor)AddEastDoor(false, 2, 0, 12, keyValue);
            door.Facing = DoorFacing.NorthCCW;
		}

        public SmallStoneTempleHouse(Serial serial)
            : base(serial)
		{
		}

		public override HouseDeed GetDeed() 
		{
            return new SmallStoneTempleHouseDeed();
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
