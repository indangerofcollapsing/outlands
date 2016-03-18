using System;
using Server;
using Server.Regions;
using Server.Targeting;
using Server.Engines.CannedEvil;
using System.Collections;
using System.Collections.Generic;
using Server.Custom.Pirates;

namespace Server.Multis
{
	public abstract class BaseDockedBoat : Item
	{
		private int m_MultiID;
		private Point3D m_Offset;
        private string m_ShipName;
        private int m_Hits;
        private int m_Doubloons;

        public List<Point3D> m_LightCannonList = new List<Point3D>();
        public List<Point3D> m_HeavyCannonList = new List<Point3D>();

		[CommandProperty( AccessLevel.GameMaster )]
		public int MultiID{ get{ return m_MultiID; } set{ m_MultiID = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public Point3D Offset{ get{ return m_Offset; } set{ m_Offset = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public string ShipName{ get{ return m_ShipName; } set{ m_ShipName = value; InvalidateProperties(); } }

		public BaseDockedBoat( int id, Point3D offset, BaseBoat boat ) : base( 0x14F4 )
		{
			Weight = 1.0;
			//LootType = LootType.Blessed; // IPY : Commented out

			m_MultiID = id;
			m_Offset = offset;
			m_ShipName = boat.ShipName;
            m_Hits = boat.HitPoints;
            Items.Doubloon d = boat.Hold.FindItemByType(typeof(Items.Doubloon)) as Items.Doubloon;
            if (d != null)
            {
                m_Doubloons = d.Amount;
                d.Delete();
            }
		}

		public BaseDockedBoat( Serial serial ) : base( serial )
		{
		}

        private void PlaceCannons(Point3D center, Map map, BaseBoat b)
        {
            foreach (Point3D p in m_LightCannonList)
            {
                LightCannon c = new LightCannon(b);
                c.MoveToWorld(new Point3D(center.X + p.X, center.Y + p.Y, center.Z + p.Z), map);
                c.SetFacing(Direction.North);
                b.AddCannon(c);
            }
            m_LightCannonList.Clear();

            foreach (Point3D p in m_HeavyCannonList)
            {
                HeavyCannon c = new HeavyCannon(b);
                c.MoveToWorld(new Point3D(center.X + p.X, center.Y + p.Y, center.Z + p.Z), map);
                c.SetFacing(Direction.North);
                b.AddCannon(c);
            }
            m_HeavyCannonList.Clear();
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version

            writer.WriteEncodedInt(m_HeavyCannonList.Count);
            foreach (Point3D p in m_HeavyCannonList)
                writer.Write((Point3D)p);

            writer.WriteEncodedInt(m_LightCannonList.Count);
            foreach (Point3D p in m_LightCannonList)
                writer.Write((Point3D)p);

            writer.Write(m_Doubloons);

			writer.Write( m_MultiID );
			writer.Write( m_Offset );
			writer.Write( m_ShipName );
            writer.Write( m_Hits );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
                case 1:
                    {
                        int count = reader.ReadEncodedInt();
                        for (int i = 0; i < count; i++)
                            m_HeavyCannonList.Add(reader.ReadPoint3D());

                        count = reader.ReadEncodedInt();
                        for (int i = 0; i < count; i++)
                            m_LightCannonList.Add(reader.ReadPoint3D());

                        m_Doubloons = reader.ReadInt();

                        goto case 0;
                    }
				case 0:
				{

                    m_MultiID = reader.ReadInt();
					m_Offset = reader.ReadPoint3D();
					m_ShipName = reader.ReadString();
                    m_Hits = reader.ReadInt();

					break;
				}
			}

			if ( LootType == LootType.Newbied )
				LootType = LootType.Regular; // IPY : Changed from LootType = LootType.Blessed;

			if ( Weight == 0.0 )
				Weight = 1.0;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
			else
			{
				from.SendLocalizedMessage( 502482 ); // Where do you wish to place the ship?

				from.Target = new InternalTarget( this );
			}
		}

		public abstract BaseBoat Boat{ get; }

		public override void AddNameProperty( ObjectPropertyList list )
		{
			if ( m_ShipName != null )
				list.Add( m_ShipName );
			else
				base.AddNameProperty( list );
		}

		public override void OnSingleClick( Mobile from )
		{
			if ( m_ShipName != null )
				LabelTo( from, m_ShipName );
			else
				base.OnSingleClick( from );
		}

		public void OnPlacement( Mobile from, Point3D p )
		{
			if ( Deleted )
			{
				return;
			}
			else if ( !IsChildOf( from.Backpack ) && Map != Map.Internal) //Internal map for dockmasters
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
			else
			{
				Map map = from.Map;

				if ( map == null )
					return;

				BaseBoat boat = Boat;

				if ( boat == null )
					return;

				p = new Point3D( p.X - m_Offset.X, p.Y - m_Offset.Y, p.Z - m_Offset.Z );

				if ( BaseBoat.IsValidLocation( p, map ) && boat.CanFit( p, map, boat.ItemID ) && map != Map.Ilshenar && map != Map.Malas )
				{
                    if (BaseBoat.TryAddBoat(from, boat))
                    {
                        Delete();

                        boat.Owner = from;
                        boat.Anchored = true;
                        boat.ShipName = m_ShipName;
                        boat.HitPoints = m_Hits;
                       
                        if (m_Doubloons > 0)
                            boat.Hold.TryDropItem(boat.Owner,new Items.Doubloon(m_Doubloons),false);
                        
                        boat.MoveToWorld(p, map);

                        PlaceCannons(p, map, boat);

                        boat.Hue = Hue;
                    }

                    else
                    {
                        boat.Delete();
                        from.SendMessage("This account already has a boat.");
                    }
				}
				else
				{
					boat.Delete();
					from.SendLocalizedMessage( 1043284 ); // A ship can not be created here.
				}
			}
		}

		private class InternalTarget : MultiTarget
		{
			private BaseDockedBoat m_Model;

			public InternalTarget( BaseDockedBoat model ) : base( model.MultiID, model.Offset )
			{
				m_Model = model;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				IPoint3D ip = o as IPoint3D;

				if ( ip != null )
				{
					if ( ip is Item )
						ip = ((Item)ip).GetWorldTop();

					Point3D p = new Point3D( ip );

					Region region = Region.Find( p, from.Map );

					if ( region.IsPartOf( typeof( DungeonRegion ) ) )
						from.SendLocalizedMessage( 502488 ); // You can not place a ship inside a dungeon.
					else if ( region.IsPartOf( typeof( HouseRegion ) ) || region.IsPartOf( typeof( ChampionSpawnRegion ) ) )
						from.SendLocalizedMessage( 1042549 ); // A boat may not be placed in this area.
					else
						m_Model.OnPlacement( from, p );
				}
			}
		}
	}
}