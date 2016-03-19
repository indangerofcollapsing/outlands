/*
 * (Heavily) Modified from zerodowned's code http://www.playuo.org/clone_emu/index.php?resources/ridable-carousel.150/
 */

using System;
using Server;
using Server.Items;
using System.Collections;
using System.Collections.Generic;
using Server.Multis;
using Server.Mobiles;
using Server.Commands;

namespace Server.Items
{
    public class AdjustableCarousel : Item
    {
        private Timer m_CarouselMovement;

        bool mPower = false;
        int lastX = 0;
        int lastY = 0;
        int lastZ = 0;

        int radius;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Power { get { return mPower; } set { mPower = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int xCenter { get { return lastX; } set { lastX = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int yCenter { get { return lastY; } set { lastY = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int zCenter { get { return lastZ; } set { lastZ = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int radiusDist { get { return radius; } set { radius = value; } }

        [Constructable]
        public AdjustableCarousel() : this(1) { }

        [Constructable]
        public AdjustableCarousel(int r)
            : base()
        {
            Movable = false;
            Name = "Carousel";
            ItemID = 0x4121;
            if (r > 12 || r < 0)
                radius = 1;
            else
                radius = r;

        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from == null) { return; }

            if (!from.InRange(this.GetWorldLocation(), 4))
            {
                from.SendMessage("I can't reach that!");
                return;
            }
            if (this != null)
            {
                lastX = this.X;
                lastY = this.Y;
                lastZ = this.Z;

                if (this.Power == false)
                {
                    from.SendMessage("You turn the Carousel on.");
                    this.Power = true;
                    this.m_CarouselMovement = new CarouselMovement(this);
                    this.m_CarouselMovement.Start();
                }

                else if (this.Power == true)
                {
                    from.SendMessage("You turn the Carousel off.");
                    this.Power = false;
                    this.m_CarouselMovement.Stop();
                }
            }
            base.OnDoubleClick(from);
        }


        public AdjustableCarousel(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            writer.Write(lastX);
            writer.Write(lastY);
            writer.Write(lastZ);
            writer.Write(radius);
        }


        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            lastX = reader.ReadInt();
            lastY = reader.ReadInt();
            lastZ = reader.ReadInt();
            radius = reader.ReadInt();
            mPower = false; // reader.ReadBool(); <== crashes the server for some reason if left on, then re-dbl clicked.. use "false" to be safe.
        }

        private class CarouselMovement : Timer
        {
            private AdjustableCarousel m_Carousel;

            public int QuadrantDirection(int originX, int originY, int pointX, int pointY, int radius)
            {
                int resultToken = 0;

                // 0    Invalid
                // 1    South
                // 2    East
                // 3    North
                // 4    West

                if (pointX - originX >= 0) // +X
                {
                    if (originX + radius == pointX) // Right Row
                    {
                        if (pointY < originY + radius) // Not a corner
                        {
                            // South
                            resultToken = 1;
                        }
                        else // (SouthEast Corner Turn)
                        {
                            // West
                            resultToken = 4;
                        }
                    }
                    else if (originY + radius == pointY) // Bottom Row
                    {
                        // West
                        resultToken = 4;
                    }
                    else if (originY - radius == pointY) // Top Row
                    {
                        // East
                        resultToken = 2;
                    }
                    else
                    {
                        resultToken = 2;
                    }
                }
                else // -X
                {
                    if (originX - radius == pointX) // Left Row
                    {
                        if (pointY > originY - radius)
                        {
                            resultToken = 3;
                        }
                        else // (NorthWest Corner Turn)
                        {
                            // East
                            resultToken = 2;
                        }
                    }
                    else if (originY + radius == pointY) // Bottom Row
                    {
                        // West
                        resultToken = 4;
                    }
                    else if (originY - radius == pointY) // Top Row
                    {
                        // East
                        resultToken = 2;
                    }
                }
                return resultToken;
            }

            public CarouselMovement(AdjustableCarousel carousel)
                : base(TimeSpan.FromSeconds(0.5))
            {
                Priority = TimerPriority.FiftyMS; m_Carousel = carousel;
            }

            protected override void OnTick()
            {
                List<Item> list = new List<Item>();
                // Point3D loc = m_Carousel.GetWorldLocation(); // could probably use this rather than lastX, lastY...

                foreach (Item item in m_Carousel.GetItemsInRange(m_Carousel.radius))
                {
                    Point3D itemLoc = item.GetWorldLocation();
                    // Only "Add" items that are on the radius's edge, ignoring everything in-between the middle and the edge.
                    if ((itemLoc.X == (m_Carousel.lastX - m_Carousel.radius) || itemLoc.X == (m_Carousel.lastX + m_Carousel.radius)) ||
                       (itemLoc.Y == (m_Carousel.lastY - m_Carousel.radius) || itemLoc.Y == (m_Carousel.lastY + m_Carousel.radius)))
                    {
                        list.Add(item);
                    }
                }

                foreach (Item item in list)
                {
                    Point3D itemLoc = item.GetWorldLocation();
                    int direction = QuadrantDirection(m_Carousel.lastX, m_Carousel.lastY, itemLoc.X, itemLoc.Y, m_Carousel.radius);
                    switch (direction)
                    {
                        // 0    Invalid
                        // 1    South
                        // 2    East
                        // 3    North
                        // 4    West
                        case 1:
                            item.Y++;
                            break;
                        case 2:
                            item.X++;
                            break;
                        case 3:
                            item.Y--;
                            break;
                        case 4:
                            item.X--;
                            break;
                        case 0:
                        default:
                            break;
                    }
                }

                List<Mobile> mobilelist = new List<Mobile>();

                foreach (Mobile mobile in m_Carousel.GetMobilesInRange(m_Carousel.radius))
                {
                    // Only "Add" mobiles that are on the radius's edge, ignoring everything in-between the middle and the edge.
                    if ((mobile.X == (m_Carousel.lastX - m_Carousel.radius) || mobile.X == (m_Carousel.lastX + m_Carousel.radius)) ||
                       (mobile.Y == (m_Carousel.lastY - m_Carousel.radius) || mobile.Y == (m_Carousel.lastY + m_Carousel.radius)))
                    {
                        mobilelist.Add(mobile);
                    }
                }

                foreach (Mobile mobile in mobilelist)
                {
                    int direction = QuadrantDirection(m_Carousel.lastX, m_Carousel.lastY, mobile.X, mobile.Y, m_Carousel.radius);
                    switch (direction)
                    {
                        // 0    Invalid
                        // 1    South
                        // 2    East
                        // 3    North
                        // 4    West
                        case 1:
                            mobile.Y++;
                            break;
                        case 2:
                            mobile.X++;
                            break;
                        case 3:
                            mobile.Y--;
                            break;
                        case 4:
                            mobile.X--;
                            break;
                        case 0:
                        default:
                            break;
                    }
                }
                if (m_Carousel.Power == false) { Stop(); }
                else { Start(); }
            }
        }
    }
}