using System;
using Server;
using Server.Items;
using System.Collections;
using System.Collections.Generic;

namespace Server.Multis
{
	public class SmallBoat : BaseBoat
	{
        [Constructable]
        public SmallBoat()
        {
            Name = "a small boat";
        }

        public SmallBoat(Serial serial): base(serial)
        {
        }
        
        public override int NorthID { get { return 0x0; } }
        public override int EastID { get { return 0x1; } }
        public override int SouthID { get { return 0x2; } }
        public override int WestID { get { return 0x3; } }

		public override int HoldDistance{ get{ return 4; } }
		public override int TillerManDistance{ get{ return -4; } }

		public override Point2D StarboardOffset{ get{ return new Point2D(  2, 0 ); } }
		public override Point2D      PortOffset{ get{ return new Point2D( -2, 0 ); } }

		public override Point3D MarkOffset{ get{ return new Point3D( 0, 1, 3 ); } }

        public override BaseBoatDeed BoatDeed { get { return new SmallBoatDeed(); } }

        public override List<Point3D> m_EmbarkLocations()
        {
            List<Point3D> list = new List<Point3D>();

            list.Add(new Point3D(0, -4, 0));
            list.Add(new Point3D(0, -3, 0));
            list.Add(new Point3D(0, -2, 0));
            list.Add(new Point3D(-1, -1, 0)); list.Add(new Point3D(0, -1, 0)); list.Add(new Point3D(1, -1, 0));
            list.Add(new Point3D(-1, 0, 0)); list.Add(new Point3D(1, 0, 0));
            list.Add(new Point3D(-1, 1, 0)); list.Add(new Point3D(0, 1, 0)); list.Add(new Point3D(1, 1, 0));
            list.Add(new Point3D(0, 2, 0));
            list.Add(new Point3D(0, 3, 0));

            return list;
        }

        public override List<Point3D> m_MastLocations()
        {
            List<Point3D> list = new List<Point3D>();

            list.Add(new Point3D(0, 0, 0));

            return list;
        }

        public override void GenerateShipCannons()
        {
            base.GenerateShipCannons();

            ShipCannon.PlaceShipCannon(this, new Point3D(-2, -1, 0), ShipCannon.CannonType.Small, ShipCannon.CannonPosition.Left);
            ShipCannon.PlaceShipCannon(this, new Point3D(-2, 0, 0), ShipCannon.CannonType.Small, ShipCannon.CannonPosition.Left);
            ShipCannon.PlaceShipCannon(this, new Point3D(-2, 1, 0), ShipCannon.CannonType.Small, ShipCannon.CannonPosition.Left);

            ShipCannon.PlaceShipCannon(this, new Point3D(2, -1, 0), ShipCannon.CannonType.Small, ShipCannon.CannonPosition.Right);
            ShipCannon.PlaceShipCannon(this, new Point3D(2, 0, 0), ShipCannon.CannonType.Small, ShipCannon.CannonPosition.Right);
            ShipCannon.PlaceShipCannon(this, new Point3D(2, 1, 0), ShipCannon.CannonType.Small, ShipCannon.CannonPosition.Right);
        }

        public override List<Point3D> m_CannonLocations()
        {
            List<Point3D> list = new List<Point3D>();
            
            list.Add(new Point3D(-2, -2, 0));
            list.Add(new Point3D(-2, -1, 0));
            list.Add(new Point3D(-2, 0, 0));
            list.Add(new Point3D(-2, 1, 0));
            list.Add(new Point3D(-2, 2, 0)); 
           
            list.Add(new Point3D(2, -2, 0));
            list.Add(new Point3D(2, -1, 0));
            list.Add(new Point3D(2, 0, 0));
            list.Add(new Point3D(2, 1, 0));
            list.Add(new Point3D(2, 2, 0));            

            return list;
        }

        public override List<Point3D> m_BoatFireLocations()
        {
            List<Point3D> list = new List<Point3D>();

            list.Add(new Point3D(0, 1, 2));
            list.Add(new Point3D(-2, -2, 2));
            list.Add(new Point3D(1, 3, 2));
            list.Add(new Point3D(0, -3, 2));
            list.Add(new Point3D(2, 0, 2));
            list.Add(new Point3D(-2, 1, 2));
            list.Add(new Point3D(-1, 3, 2));
            list.Add(new Point3D(1, -2, 2));

            return list;
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

	public class SmallBoatDeed : BaseBoatDeed
	{		
		public override BaseBoat Boat{ get{ return new SmallBoat(); } }
		private static int m_BoatId = 0x4000;

        public override int DoubloonCost { get { return 0; } }
        public override double DoubloonMultiplier { get { return 1; } }

		[Constructable]
		public SmallBoatDeed() : base( m_BoatId, Point3D.Zero )
		{
            Name = " a small boat deed";
            PlayerClassRestricted = false;
		}

		public SmallBoatDeed( Serial serial ) : base( serial )
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