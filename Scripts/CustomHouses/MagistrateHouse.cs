using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Multis.Deeds;

namespace Server.Multis
{
	public class MagistrateHouse : BaseHouse 
	{
        public static Rectangle2D[] AreaArray = new Rectangle2D[] { new Rectangle2D(-6, -8, 12, 15) };
		public override Rectangle2D[] Area{ get{ return AreaArray; } }
		public override Point3D BaseBanLocation{ get{ return new Point3D( 6, 0, 8 ); } }
		public override int DefaultPrice{ get{ return 450000; } }

		public MagistrateHouse( Mobile owner ) : base( 0x14B, owner, 1475, 10 )
		{
            uint keyValue = CreateKeys(owner);
            SetSign(6, 0, 28);
            BaseHouseDoor door = (BaseHouseDoor)AddEastDoor(false, 3, -4, 13, keyValue);
            door.Facing = DoorFacing.NorthCCW;
            AddSouthDoor(false, -1, -1, 13);
		}

        public MagistrateHouse(Serial serial)
            : base(serial)
		{
		}

		public override HouseDeed GetDeed() 
		{
			return new MagistrateHouseDeed();
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
