using System;
using Server;
using Server.Items;
using System.Collections;
using System.Collections.Generic;

namespace Server.Multis
{
	public class LargeDragonBoat : BaseBoat
	{   
        public override int NorthID { get { return 0x14; } }
        public override int EastID { get { return 0x15; } }
        public override int SouthID { get { return 0x16; } }
        public override int WestID { get { return 0x17; } }

		public override int HoldDistance{ get{ return 5; } }
		public override int TillerManDistance{ get{ return -5; } }

		public override Point2D StarboardOffset{ get{ return new Point2D(  2, -1 ); } }
		public override Point2D      PortOffset{ get{ return new Point2D( -2, -1 ); } }

		public override Point3D MarkOffset{ get{ return new Point3D( 0, 0, 3 ); } }

        public override BaseBoatDeed BoatDeed { get { return new LargeDragonBoatDeed(); } }

        public override List<Point3D> m_EmbarkLocations()
        {
            List<Point3D> list = new List<Point3D>();

            list.Add(new Point3D(0, -4, 0));
            list.Add(new Point3D(0, -3, 0));
            list.Add(new Point3D(-1, -2, 0)); list.Add(new Point3D(0, -2, 0)); list.Add(new Point3D(1, -2, 0));
            list.Add(new Point3D(-1, -1, 0));                                  list.Add(new Point3D(1, -1, 0));
            list.Add(new Point3D(-1, 0, 0));  list.Add(new Point3D(0, 0, 0));  list.Add(new Point3D(1, 0, 0));
            list.Add(new Point3D(-1, 1, 0));  list.Add(new Point3D(0, 1, 0));  list.Add(new Point3D(1, 1, 0));
            list.Add(new Point3D(-1, 2, 0));  list.Add(new Point3D(0, 2, 0));  list.Add(new Point3D(1, 2, 0));
            list.Add(new Point3D(0, 3, 0));
            list.Add(new Point3D(0, 4, 0));

            return list;
        }

        public override List<Point3D> m_MastLocations()
        {
            List<Point3D> list = new List<Point3D>();

            list.Add(new Point3D(0, -1, 0));

            return list;
        }

        public override List<Point3D> m_CannonLocations()
        {
            List<Point3D> list = new List<Point3D>();
            
            list.Add(new Point3D(-2, -2, 0));
            list.Add(new Point3D(-2, -1, 0));
            list.Add(new Point3D(-2, 0, 0));
            list.Add(new Point3D(-2, 1, 0));
            list.Add(new Point3D(-2, 2, 0));
            list.Add(new Point3D(-2, 3, 0));
            list.Add(new Point3D(-2, 4, 0));
          
            list.Add(new Point3D(2, -2, 0));
            list.Add(new Point3D(2, -1, 0));
            list.Add(new Point3D(2, 0, 0));
            list.Add(new Point3D(2, 1, 0));
            list.Add(new Point3D(2, 2, 0));
            list.Add(new Point3D(2, 3, 0));
            list.Add(new Point3D(2, 4, 0));

            return list;
        }

        public override List<Point3D> m_BoatFireLocations()
        {
            List<Point3D> list = new List<Point3D>();

            list.Add(new Point3D(0, 3, 0));
            list.Add(new Point3D(-2, 0, 0));
            list.Add(new Point3D(2, 1, 0));
            list.Add(new Point3D(0, -3, 0));
            list.Add(new Point3D(-1, 1, 0));
            list.Add(new Point3D(1, -1, 0));
            list.Add(new Point3D(-1, -1, 0));
            list.Add(new Point3D(1, 2, 0));
            list.Add(new Point3D(1, 4, 0));
            list.Add(new Point3D(-1, 4, 0));

            list.Add(new Point3D(0, -2, 0));
            list.Add(new Point3D(-2, 2, 0));

            return list;
        }

		[Constructable]
		public LargeDragonBoat()
		{
		}

		public LargeDragonBoat( Serial serial ) : base( serial )
		{
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );
		}
	}

	public class LargeDragonBoatDeed : BaseBoatDeed
	{
		public override BaseBoat Boat{ get{ return new LargeDragonBoat(); } }
		private static int m_BoatId = 0x4014;

        public override int DoubloonCost { get { return 550; } }
        public override double DoubloonMultiplier { get { return 2; } }

		[Constructable]
		public LargeDragonBoatDeed() : base( m_BoatId, new Point3D( 0, -1, 0 ) )
		{
            Name = "a large dragon boat deed";
		}

		public LargeDragonBoatDeed( Serial serial ) : base( serial )
		{
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );
		}
	}
}