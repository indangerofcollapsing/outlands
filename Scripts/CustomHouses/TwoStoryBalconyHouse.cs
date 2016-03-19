using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Multis.Deeds;

namespace Server.Multis
{
	public class TwoStoryBalconyHouse : BaseHouse 
	{
        public static Rectangle2D[] AreaArray = new Rectangle2D[] { new Rectangle2D(-9, -5, 8, 5) };
		public override Rectangle2D[] Area{ get{ return AreaArray; } }
		public override Point3D BaseBanLocation{ get{ return new Point3D( -1, 5, -7 ); } }
		public override int DefaultPrice{ get{ return 100000; } }

		public TwoStoryBalconyHouse( Mobile owner ) : base( 0x163, owner, 1100, 8 )
		{
            uint keyValue = CreateKeys(owner);
            SetSign(0, 5, 20);
            AddSouthDoors(false, -1, 2, 0, keyValue);
		}

        public TwoStoryBalconyHouse(Serial serial)
            : base(serial)
		{
		}

		public override HouseDeed GetDeed() 
		{
            return new TwoStoryBalconyDeed();
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
