using System;
using System.Collections;
using Server;
using Server.Targeting;
using Server.Items;
using Server.Regions;

using Server.Mobiles;

namespace Server.Multis
{
    public class HousePlacementTarget : MultiTarget
    {
        private HouseDeed m_Deed;

        public HousePlacementTarget(HouseDeed deed): base(deed.MultiID, deed.Offset)
        {
            m_Deed = deed;
        }

        protected override void OnTarget(Mobile from, object o)
        {
            IPoint3D ip = o as IPoint3D;

            if (ip != null)
            {
                if (ip is Item)
                    ip = ((Item)ip).GetWorldTop();

                Point3D p = new Point3D(ip);

                Region reg = Region.Find(new Point3D(p), from.Map);

                if (from.AccessLevel >= AccessLevel.GameMaster || reg.AllowHousing(from, p))
                    m_Deed.OnPlacement(from, p);
                
                else if (reg.IsPartOf(typeof(TempNoHousingRegion)))
                    from.SendLocalizedMessage(501270); // Lord British has decreed a 'no build' period, thus you cannot build this house at this time.
                
                else if (reg.IsPartOf(typeof(TreasureRegion)) || reg.IsPartOf(typeof(HouseRegion)))
                    from.SendLocalizedMessage(1043287); // The house could not be created here.  Either something is blocking the house, or the house would not be on valid terrain.
                
                else if (reg.IsPartOf(typeof(HouseRaffleRegion)))
                    from.SendLocalizedMessage(1150493); // You must have a deed for this plot of land in order to build here.
                
                else
                    from.SendLocalizedMessage(501265); // Housing can not be created in this area.
            }
        }
    }

    public abstract class HouseDeed : Item
    {
        private int m_MultiID;
        private Point3D m_Offset;

        public virtual bool HasEastFacingDoor()
        {
            return false;
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MultiID
        {
            get
            {
                return m_MultiID;
            }

            set
            {
                m_MultiID = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D Offset
        {
            get
            {
                return m_Offset;
            }

            set
            {
                m_Offset = value;
            }
        }

        public HouseDeed(int id, Point3D offset): base(0x14F0)
        {
            Weight = 1.0;

            m_MultiID = id;
            m_Offset = offset;
        }

        public HouseDeed(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            //Version 0
            writer.Write(m_Offset);
            writer.Write(m_MultiID);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            //Veresion 0
            if (version >= 0)
            {
                m_Offset = reader.ReadPoint3D();
                m_MultiID = reader.ReadInt();
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))            
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.            

            else if (((PlayerMobile)from).Young)            
                from.SendMessage("Young players may not place houses. Renounce your young players status first");
            
            else if (from.AccessLevel < AccessLevel.GameMaster && BaseHouse.HasAccountHouse(from))            
                from.SendLocalizedMessage(501271); // You already own a house, you may not place another!
            
            else
            {
                from.SendLocalizedMessage(1010433); /* House placement cancellation could result in a
													   * 60 second delay in the return of your deed.
													   */

                from.Target = new HousePlacementTarget(this);
            }
        }

        public abstract BaseHouse GetHouse(Mobile owner);
        public abstract Rectangle2D[] Area { get; }

        public override bool DropToMobile(Mobile from, Mobile target, Point3D p)
        {
            if (target is PlayerMobile && ((PlayerMobile)target).Young)
            {
                from.SendMessage("Young players cannot be given house deeds.");
                return false;
            }

            return base.DropToMobile(from, target, p);
        }

        public override bool DropToItem(Mobile from, Item target, Point3D p)
        {
            if (from is PlayerMobile && ((PlayerMobile)from).Young)
            {
                from.SendMessage("Young players cannot pick up house deeds.");
                return false;
            }

            return base.DropToItem(from, target, p);
        }

        public override bool OnDragLift(Mobile from)
        {
            if (from is PlayerMobile && ((PlayerMobile)from).Young)
            {
                from.SendMessage("Young players cannot pick up house deeds.");
                return false;
            }

            return base.OnDragLift(from);
        }

        public void OnPlacement(Mobile from, Point3D p)
        {
            if (Deleted)
                return;

            if (!IsChildOf(from.Backpack))            
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            
            else if (from.AccessLevel < AccessLevel.GameMaster && BaseHouse.HasAccountHouse(from))            
                from.SendLocalizedMessage(501271); // You already own a house, you may not place another!
            
            else
            {
                ArrayList toMove;
                Point3D center = new Point3D(p.X - m_Offset.X, p.Y - m_Offset.Y, p.Z - m_Offset.Z);
                HousePlacementResult res = HousePlacement.Check(from, m_MultiID, center, out toMove, HasEastFacingDoor());

                switch (res)
                {
                    case HousePlacementResult.Valid:
                        {
                            BaseHouse house = GetHouse(from);
                            
                            //For Custom house, drop z-level by -8 <--- May need to revise after we add more custom houses
                            if (house is SmallStoneTempleHouse || house is MagistrateHouse || house is SandstoneSpaHouse || house is ArbiterEstate)
                                center.Z -= 8;

                            house.MoveToWorld(center, from.Map);
                            Delete();

                            for (int i = 0; i < toMove.Count; ++i)
                            {
                                object o = toMove[i];

                                if (o is Mobile)
                                    ((Mobile)o).Location = house.BanLocation;

                                else if (o is Item)
                                    ((Item)o).Location = house.BanLocation;
                            }

                            break;
                        }
                    case HousePlacementResult.BadItem:
                    case HousePlacementResult.BadLand:
                    case HousePlacementResult.BadStatic:
                    case HousePlacementResult.BadRegionHidden:
                    {
                        from.SendLocalizedMessage(1043287); // The house could not be created here.  Either something is blocking the house, or the house would not be on valid terrain.
                        break;
                    }

                    case HousePlacementResult.NoSurface:
                    {
                        from.SendMessage("The house could not be created here.  Part of the foundation would not be on any surface.");
                        break;
                    }

                    case HousePlacementResult.BadRegion:
                    {
                        from.SendLocalizedMessage(501265); // Housing cannot be created in this area.
                        break;
                    }

                    case HousePlacementResult.BadRegionTemp:
                    {
                        from.SendLocalizedMessage(501270); //Lord British has decreed a 'no build' period, thus you cannot build this house at this time.
                        break;
                    }

                    case HousePlacementResult.BadRegionRaffle:
                    {
                        from.SendLocalizedMessage(1150493); // You must have a deed for this plot of land in order to build here.
                        break;
                    }
                }
            }
        }
    }
}