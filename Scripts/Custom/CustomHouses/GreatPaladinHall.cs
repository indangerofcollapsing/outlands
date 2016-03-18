using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Multis.Deeds;

namespace Server.Multis
{
	public class GreatPaladinHall : BaseHouse 
	{
        public static Rectangle2D[] AreaArray = new Rectangle2D[] { new Rectangle2D(-17, -14, 17, 14) };
		public override Rectangle2D[] Area{ get{ return AreaArray; } }
		public override Point3D BaseBanLocation{ get{ return new Point3D( 18, 2, 7 ); } }
		public override int DefaultPrice{ get{ return 220000; } }

		public GreatPaladinHall( Mobile owner ) : base( 0x142, owner, 2200, 16 )
		{
            uint keyValue = CreateKeys(owner);
            SetSign(16, -3, 28);
            AddEastDoors(false, 15, -1, 7, keyValue);   
		}

        public GreatPaladinHall(Serial serial)
            : base(serial)
		{
		}

		public override HouseDeed GetDeed() 
		{
            return new GreatPaladinHallDeed();
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
