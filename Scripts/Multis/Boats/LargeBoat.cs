using System;
using Server;
using Server.Items;
using System.Collections;
using System.Collections.Generic;

namespace Server.Multis
{
	public class LargeBoat : BaseBoat
	{   
        public override int NorthID { get { return 0x10; } }
        public override int EastID { get { return 0x11; } }
        public override int SouthID { get { return 0x12; } }
        public override int WestID { get { return 0x13; } }

		public override int HoldDistance{ get{ return 5; } }
		public override int TillerManDistance{ get{ return -5; } }

		public override Point2D StarboardOffset{ get{ return new Point2D(  2, -1 ); } }
		public override Point2D      PortOffset{ get{ return new Point2D( -2, -1 ); } }

		public override Point3D MarkOffset{ get{ return new Point3D( 0, 0, 3 ); } }

        public override BaseBoatDeed BoatDeed { get { return new LargeBoatDeed(); } }

        public override List<Point3D> m_EmbarkLocations()
        {
            List<Point3D> list = new List<Point3D>();

            list.Add(new Point3D(0, -5, 0));
            list.Add(new Point3D(0, -4, 0));
            list.Add(new Point3D(0, -3, 0));
            list.Add(new Point3D(-1, -2, 0)); list.Add(new Point3D(0, -2, 0)); list.Add(new Point3D(1, -2, 0));
            list.Add(new Point3D(-1, -1, 0)); list.Add(new Point3D(1, -1, 0));
            list.Add(new Point3D(-1, 0, 0)); list.Add(new Point3D(0, 0, 0)); list.Add(new Point3D(1, 0, 0));
            list.Add(new Point3D(-1, 1, 0)); list.Add(new Point3D(0, 1, 0)); list.Add(new Point3D(1, 1, 0));
            list.Add(new Point3D(-1, 2, 0)); list.Add(new Point3D(0, 2, 0)); list.Add(new Point3D(1, 2, 0));
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
		public LargeBoat()
		{
            Name = "a large boat";
		}

		public LargeBoat( Serial serial ) : base( serial )
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

	public class LargeBoatDeed : BaseBoatDeed
	{
		public override BaseBoat Boat{ get{ return new LargeBoat(); } }
		private static int m_BoatId = 0x4010;

        public override int DoubloonCost { get { return 500; } }
        public override double DoubloonMultiplier { get { return 2; } }

		[Constructable]
		public LargeBoatDeed() : base( m_BoatId, new Point3D( 0, -1, 0 ) )
		{
            Name = "a large boat deed";
		}

		public LargeBoatDeed( Serial serial ) : base( serial )
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