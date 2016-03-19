using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Multis.Deeds;

namespace Server.Multis
{
	public class StableHouse : BaseHouse 
	{
        public static Rectangle2D[] AreaArray = new Rectangle2D[] { new Rectangle2D(-13, -7, 13, 6) };
		public override Rectangle2D[] Area{ get{ return AreaArray; } }
		public override Point3D BaseBanLocation{ get{ return new Point3D( 13, -1, 1 ); } }
        public override int DefaultPrice { get { return 100000; } } 

		public StableHouse( Mobile owner ) : base( 0x151, owner, 1370, 10 )
		{
            uint keyValue = CreateKeys(owner);
            SetSign(12, -4, 28);
            AddEastDoors(false, 11, -1, 8, keyValue);   
		}

        public StableHouse( Serial serial ) : base( serial )
		{
		}

		public override HouseDeed GetDeed() 
		{
			return new StableHouseDeed();
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
