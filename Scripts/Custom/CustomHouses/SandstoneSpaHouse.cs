using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Multis.Deeds;

namespace Server.Multis
{
	public class SandstoneSpaHouse : BaseHouse 
	{
        public static Rectangle2D[] AreaArray = new Rectangle2D[] { new Rectangle2D(-11, -9, 23, 20) };
		public override Rectangle2D[] Area{ get{ return AreaArray; } }
		public override Point3D BaseBanLocation{ get{ return new Point3D( 2, 1, 7 ); } }
		public override int DefaultPrice{ get{ return 100000; } }
        public override HousePlacementEntry ConvertEntry { get { return HousePlacementEntry.ClassicHouses[20]; } }

		public SandstoneSpaHouse( Mobile owner ) : base( 0x145, owner, 1475, 10 )
		{
            uint keyValue = CreateKeys(owner);
            SetSign(-2, 5, 28);
            BaseHouseDoor door = (BaseHouseDoor)AddEastDoor(false, -8, -2, 47, keyValue);
            door.Facing = DoorFacing.EastCCW;
		}

        public SandstoneSpaHouse( Serial serial ) : base( serial )
		{
		}

		public override HouseDeed GetDeed() 
		{
            return new SandstoneSpaHouseDeed();
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
