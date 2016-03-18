using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Multis.Deeds;

namespace Server.Multis
{
	public class ArbiterEstate : BaseHouse 
	{
        public static Rectangle2D[] AreaArray = new Rectangle2D[] { new Rectangle2D(-12, -12, 25, 26) };
		public override Rectangle2D[] Area{ get{ return AreaArray; } }
        public override Point3D BaseBanLocation { get { return new Point3D(13, -1, 6); } }
		public override int DefaultPrice{ get{ return 1200000; } }

		public ArbiterEstate( Mobile owner ) : base( 0x159, owner, 3975, 20 )
		{
            uint keyValue = CreateKeys(owner);
            SetSign(6, -3, 16);
            AddEastDoor(false, 5, 0, 12, keyValue);
            AddSouthDoor(false, 0, 6, 12);
            AddSouthDoor(false, 0, -7, 12);
            AddEastDoor(false, -4, 3, 12);
            AddEastDoor(false, 3, -9, 12);
            AddSouthDoor(false, -8, -5, 12);
            AddSouthDoor(false, -9, 4, 12);
		}

        public ArbiterEstate(Serial serial)
            : base(serial)
		{
		}

		public override HouseDeed GetDeed() 
		{
            return new ArbiterEstateDeed();
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
