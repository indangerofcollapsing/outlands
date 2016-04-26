using System;
using Server;
using Server.Items;
using System.Collections;
using System.Collections.Generic;

namespace Server.Multis
{
	public class RowBoat : BaseBoat
	{   
        public override int NorthID { get { return 0x3C; } }
        public override int EastID { get { return 0x3D; } }
        public override int SouthID { get { return 0x3E; } }
        public override int WestID { get { return 0x3F; } }

		public override int HoldDistance{ get{ return 2; } }
		public override int TillerManDistance{ get{ return -3; } }

		public override Point2D StarboardOffset{ get{ return new Point2D(  2, 0 ); } }
		public override Point2D PortOffset{ get{ return new Point2D( -2, 0 ); } }

		public override Point3D MarkOffset{ get{ return new Point3D( 0, 1, 3 ); } }

        public override BaseBoatDeed BoatDeed { get { return new RowBoatDeed(); } }

        public override List<Point3D> m_EmbarkLocations()
        {
            List<Point3D> list = new List<Point3D>();

            list.Add(new Point3D(0, -2, 0));
            list.Add(new Point3D(0, -1, 0));
            list.Add(new Point3D(0, 0, 0));
            list.Add(new Point3D(0, 1, 0));
            list.Add(new Point3D(0, 2, 0));

            return list;
        }

        public override List<Point3D> m_MastLocations()
        {
            List<Point3D> list = new List<Point3D>();

            return list;
        }

        public override List<Point3D> m_BoatFireLocations()
        {
            List<Point3D> list = new List<Point3D>();

            list.Add(new Point3D(0, 0, 0));
            list.Add(new Point3D(0, -2, 0));
            list.Add(new Point3D(0, 2, 0));          

            return list;
        }

        public override void ConfigureShip()
        {
            base.ConfigureShip();

            PPlank.Visible = false;
            SPlank.Visible = false;
        }

		[Constructable]
		public RowBoat()
		{
		}

        public RowBoat(Serial serial): base(serial)
		{
		}

        public override void OnSingleClick(Mobile from)
        {           
            LabelTo(from, "a rowboat");
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

	public class RowBoatDeed : BaseBoatDeed
	{		
		public override BaseBoat Boat{ get{ return new RowBoat(); } }
        private static int m_BoatId = 0x3C;

		public override int DoubloonCost { get { return 0; } }

		[Constructable]
		public RowBoatDeed() : base( m_BoatId, Point3D.Zero )
		{
            Name = " a row boat deed";
            PlayerClassRestricted = false;
		}

        public RowBoatDeed(Serial serial): base(serial)
		{
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }
	}
}