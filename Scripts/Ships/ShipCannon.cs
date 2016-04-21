using System;
using Server;
using Server.Mobiles;
using Server.Multis;
using Server.Targeting;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Spells;
using Server.Network;

namespace Server
{
    public class ShipCannon : Item
    {
        public enum CannonType
        {
            Small
        }

        public enum CannonPosition
        {
            Left,
            Right,
            Front,
            Rear
        }

        public BaseBoat m_Boat;

        public CannonType m_CannonType = CannonType.Small;
        public CannonPosition m_CannonPosition = CannonPosition.Left;

        public int m_xOffset;
        public int m_yOffset;
        public int m_zOffset;

        private int m_Ammunition;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Ammunition
        {
            get { return m_Ammunition; }
            set { m_Ammunition = value; }
        }

        private Direction m_Facing;
        [CommandProperty(AccessLevel.GameMaster)]
        public Direction Facing
        {
            get { return m_Facing; }
            set { m_Facing = value; }
        }       

        [Constructable]
        public ShipCannon(): base()
        {
            Movable = false;
        }

        public ShipCannon(Serial serial): base(serial)
        {
        }

        #region Place Ship Cannon

        public static void PlaceShipCannon(BaseBoat boat, Point3D point, CannonType cannonType, CannonPosition cannonPosition)
        {
            if (boat == null)
                return;

            ShipCannon shipCannon = new ShipCannon();

            shipCannon.Visible = false;

            shipCannon.m_Boat = boat;
            shipCannon.m_CannonType = cannonType;
            shipCannon.m_CannonPosition = cannonPosition;
            shipCannon.m_xOffset = point.X;
            shipCannon.m_yOffset = point.Y;
            shipCannon.m_zOffset = point.Z;

            Point3D cannonLocation = boat.GetRotatedLocation(point.X, point.Y, 0);

            shipCannon.MoveToWorld(new Point3D(boat.Location.X + cannonLocation.X, boat.Location.Y + cannonLocation.Y, boat.Location.Z + cannonLocation.Z), boat.Map);
            shipCannon.BoatFacingChange(boat.Facing);
            shipCannon.Z = boat.Location.Z + cannonLocation.Z + shipCannon.GetAdjustedCannonZOffset();

            shipCannon.Hue = boat.CannonHue;

            if (boat.MobileControlType != MobileControlType.Player)
                shipCannon.Ammunition = shipCannon.GetMaxAmmunition();

            shipCannon.Visible = true;

            boat.m_Cannons.Add(shipCannon);

            switch (cannonPosition)
            {
                case CannonPosition.Left: boat.m_LeftCannons.Add(shipCannon); break;
                case CannonPosition.Right: boat.m_RightCannons.Add(shipCannon); break;
                case CannonPosition.Front: boat.m_FrontCannons.Add(shipCannon); break;
                case CannonPosition.Rear: boat.m_RearCannons.Add(shipCannon); break;
            }
        }

        #endregion

        #region Boat Facing Change

        public void BoatFacingChange(Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    switch (m_CannonPosition)
                    {
                        case CannonPosition.Left:
                            Facing = Direction.West;

                            switch (m_CannonType)
                            {
                                case CannonType.Small: ItemID = 0x2dd; break;
                            }
                        break;

                        case CannonPosition.Right:
                            Facing = Direction.East;

                            switch (m_CannonType)
                            {
                                case CannonType.Small: ItemID = 0x2c6; break;
                            }
                        break;

                        case CannonPosition.Front:
                            Facing = Direction.North;

                            switch (m_CannonType)
                            {
                                case CannonType.Small: ItemID = 0x2c5; break;
                            }
                        break;

                        case CannonPosition.Rear:
                            Facing = Direction.South;

                            switch (m_CannonType)
                            {
                                case CannonType.Small: ItemID = 0x2dc; break;
                            }
                        break;
                    }
                break;

                case Direction.South:
                    switch (m_CannonPosition)
                    {
                        case CannonPosition.Left:
                            Facing = Direction.East;

                            switch (m_CannonType)
                            {
                                case CannonType.Small: ItemID = 0x2c6; break;
                            }
                        break;

                        case CannonPosition.Right:
                            Facing = Direction.West;

                            switch (m_CannonType)
                            {
                                case CannonType.Small: ItemID = 0x2dd; break;
                            }
                        break;

                        case CannonPosition.Front:
                            Facing = Direction.South;

                            switch (m_CannonType)
                            {
                                case CannonType.Small: ItemID = 0x2dc; break;
                            }
                        break;

                        case CannonPosition.Rear:
                            Facing = Direction.North;

                            switch (m_CannonType)
                            {
                                case CannonType.Small: ItemID = 0x2c5; break;
                            }
                        break;
                    }
                break;

                case Direction.East:
                    switch (m_CannonPosition)
                    {
                        case CannonPosition.Left:
                            Facing = Direction.North;

                            switch (m_CannonType)
                            {
                                case CannonType.Small: ItemID = 0x2c5; break;
                            }
                        break;

                        case CannonPosition.Right:
                            Facing = Direction.South;

                            switch (m_CannonType)
                            {
                                case CannonType.Small: ItemID = 0x2dc; break;
                            }
                        break;

                        case CannonPosition.Front:
                            Facing = Direction.East;

                            switch (m_CannonType)
                            {
                                case CannonType.Small: ItemID = 0x2c6; break;
                            }
                        break;

                        case CannonPosition.Rear:
                            Facing = Direction.West;

                            switch (m_CannonType)
                            {
                                case CannonType.Small: ItemID = 0x2dd; break;
                            }
                        break;
                    }
                break;

                case Direction.West:
                    switch (m_CannonPosition)
                    {
                        case CannonPosition.Left:
                            Facing = Direction.South;

                            switch (m_CannonType)
                            {
                                case CannonType.Small: ItemID = 0x2dc; break;
                            }
                        break;

                        case CannonPosition.Right:
                            Facing = Direction.North;

                            switch (m_CannonType)
                            {
                                case CannonType.Small: ItemID = 0x2c5; break;
                            }
                        break;

                        case CannonPosition.Front:
                            Facing = Direction.West;

                            switch (m_CannonType)
                            {
                                case CannonType.Small: ItemID = 0x2dd; break;
                            }
                        break;

                        case CannonPosition.Rear:
                            Facing = Direction.East;

                            switch (m_CannonType)
                            {
                                case CannonType.Small: ItemID = 0x2c6; break;
                            }
                        break;
                    }
                break;
            }
        }

        #endregion

        #region GetAdjustedCannonZOffset()

        public int GetAdjustedCannonZOffset()
        {
            if (m_Boat == null)
                return 0;

            int adjustZ = 0;

            switch (m_Boat.Facing)
            {
                case Direction.North:
                    if (Facing == Direction.West)
                        adjustZ = -1;

                    else
                        adjustZ = 1;
                break;

                case Direction.East:
                    if (Facing == Direction.North)
                        adjustZ = -2;

                    else
                        adjustZ = 2;
                break;

                case Direction.South:
                    if (Facing == Direction.West)
                        adjustZ = -1;

                    else
                        adjustZ = 1;
                break;

                case Direction.West:
                    if (Facing == Direction.North)
                        adjustZ = -2;

                    else
                        adjustZ = 2;
                break;
            }
          
            return adjustZ;
        }

        #endregion

        public int GetMaxAmmunition()
        {
            int maxAmmunition = 10;

            return maxAmmunition;
        }

        public override void OnSingleClick(Mobile from)
        {    
            base.OnSingleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);
        }

        public override void OnDelete()
        {
            if (m_Boat != null)
            {
                if (m_Boat.m_Cannons.Contains(this))
                    m_Boat.m_Cannons.Remove(this);
            }

            base.OnDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            //Version 0
            writer.Write(m_Boat);
            writer.Write((int)m_CannonType);
            writer.Write((int)m_CannonPosition);
            writer.Write(m_Ammunition);
            writer.Write(m_xOffset);
            writer.Write(m_yOffset);
            writer.Write(m_zOffset);
            writer.Write((int)m_Facing);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_Boat = (BaseBoat)reader.ReadItem();
                m_CannonType = (CannonType)reader.ReadInt();
                m_CannonPosition = (CannonPosition)reader.ReadInt();
                m_Ammunition = reader.ReadInt();
                m_xOffset = reader.ReadInt();
                m_yOffset = reader.ReadInt();
                m_zOffset = reader.ReadInt();
                Facing = (Direction)reader.ReadInt();
            }

            //-----

            if (m_Boat != null)
            {
                m_Boat.m_Cannons.Add(this);

                switch (m_CannonPosition)
                {
                    case CannonPosition.Left: m_Boat.m_LeftCannons.Add(this); break;
                    case CannonPosition.Right: m_Boat.m_RightCannons.Add(this); break;
                    case CannonPosition.Front: m_Boat.m_FrontCannons.Add(this); break;
                    case CannonPosition.Rear: m_Boat.m_RearCannons.Add(this); break;
                }
            }
        }
    }    
}