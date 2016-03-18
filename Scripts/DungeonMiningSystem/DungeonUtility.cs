using System;
using System.Collections.Generic;
using System.Text;

using Server.Targeting;
using Server.Spells;
using Server.Items;

namespace Server.PortalSystem
{
    class PortalUtility
    {
        public static Point3D s_point_invalid = new Point3D(-1, -1, -1);
        public static Point3D GetPointFromTarget(Mobile from, object target)
        {
            if (target == null)
                return s_point_invalid;
            else if (target == from)
                return from.Location;
            else if (target is Item)
                return ((Item)target).Location;
            else if (target is LandTarget)
                return ((LandTarget)target).Location;

            // Out of range
            return s_point_invalid;
        }
        public static int GetTargetHeight(Mobile from, object target)
        {
            if (target == null)
                return -1;
            else if (target == from || target is LandTarget)
                return 0;
            else if (target is Item)
                return Math.Max(((Item)target).ItemData.Height, 1);

            return -1;
        }
        public static Point3D CalculateElementLocation(Mobile from, Item element, object target)
        {
            if (element == null)
                return s_point_invalid;

            Point3D targetLoc = GetPointFromTarget(from, target);

            if (((int)element.ItemData.Flags & (int)TileFlag.Wall) > 0)
            {
                // element is a wall
                if (target is Item)
                {
                    Item targetAsItem = target as Item;
                    if (((int)targetAsItem.ItemData.Flags & (int)TileFlag.Wall) > 0)
                    {
                        // stack
                        int height = GetTargetHeight(from, target);
                        targetLoc.Z += height;
                        return targetLoc;
                    }
                }

                // Zero height adjustment (wall alignment)
                return targetLoc;
            }
            else
            {
                int height = GetTargetHeight(from, target);
                targetLoc.Z += height;
                return targetLoc;
            }

            return s_point_invalid;
        }
        public static IPortalElement CreateIPortalElement(int gid)
        {
            PortalContentEntry entry = PortalsItemization.InventoryLibrary.GetPortalContentEntryWithGid(gid);
            
            if (entry == null) return null;

            if (entry.m_behavior == PortalContentEntry.s_behavior_door)
            {
                PortalDoor.EDoorType type = PortalDoor.GetTypeFromGid(gid);
                DoorFacing facing = PortalDoor.GetFacingFromGid(gid);
                PortalDoor portalDoor = new PortalDoor(gid, entry.m_key, entry.m_name, entry.m_category, type, facing);
                return portalDoor;
            }
            else
            {
                PortalStatic portalStatic = new PortalStatic(gid, entry.m_key, entry.m_name, entry.m_category);
                return portalStatic;
            }
            return null;
        }
        public static Rectangle2D Inflate(Rectangle2D rect, int amount)
        {
            Point2D start = rect.Start;
            Point2D end = rect.End;
            start.X -= amount;
            start.Y -= amount;
            end.X += amount;
            end.Y += amount;
            return new Rectangle2D(start.X, start.Y, end.X - start.X, end.Y - start.Y);
        }

        public static float GetItemTransitTime(Point3D p, Mobile from)
        {
            Point2D offset = new Point2D(Math.Abs(p.X - from.Location.X), Math.Abs(p.Y - from.Location.Y));
            int magSq = offset.X * offset.X + offset.Y * offset.Y;
            float magSqrt = (float)Math.Sqrt((double)magSq);
            return magSqrt / 6.6f;
        }
        public static void CreateItemTransitFromSourceEffect(Point3D p, Mobile from, int gid, float time)
        {
            IEntity to = new Entity(Serial.Zero, new Point3D(p), PortalsSystem.s_map);
            Effects.SendMovingEffect(from, to, gid, 1, 3, true, false, 0, 0);
        }
        public static void CreateItemTransitToSourceEffect(Point3D p, Mobile from, int gid, float time)
        {
            IEntity to = new Entity(Serial.Zero, new Point3D(p), PortalsSystem.s_map);
            Effects.SendMovingEffect(to, from, gid, 1, 3, true, false, 0, 0);
        }
        public static void ReturnPlayersToPlatform(PortalPartition partition)
        {
            if (partition == null)
            {
                return;
            }

            // Any players in the dungeon must be moved back to the platform for safety.
            List<Mobile> playerList = new List<Mobile>();
            Rectangle2D interior = partition.GetInterior();
            IPooledEnumerable penum = PortalsSystem.s_map.GetMobilesInBounds(interior);
            foreach (Mobile m in penum)
            {
                playerList.Add(m);
            }
            penum.Free();

            Point3D entryPoint = partition.GetEntryPosition();
            foreach (Mobile m in playerList)
            {
                m.SendMessage("You've been relocated to the dungeon platform.");
                m.MoveToWorld(entryPoint, PortalsSystem.s_map);
            }
        }
        public static void ReturnPlayersToPortalPoint(PortalPartition partition)
        {
            if (partition == null)
                return;

            Point3D entryPoint = partition.m_lastPortalPointLoc;
            Map entryMap = partition.m_lastPortalPointMap;

            List<Network.NetState> netStates = partition.GetClientsInBounds();
            foreach (Network.NetState ns in netStates)
            {
                if (ns.Mobile.Criminal)
                {
                    ns.Mobile.SendMessage("The portal has unexpectedly closed...");
                    Point3D bucsDock = new Point3D(2736, 2166, 0);
                    ns.Mobile.MoveToWorld(bucsDock, Map.Felucca);
                }
                else
                {
                    ns.Mobile.SendMessage("The portal has unexpectedly closed...");
                    ns.Mobile.MoveToWorld(entryPoint, entryMap);
                }
            }
        }
    }
    public class PortalTransportSpell : MagerySpell
    {
        private Mobile m_caster;
        private PortalPartition m_partition;
        private bool m_entering;
        private static float s_castTime = 8.0f;

        private static SpellInfo m_Info = new SpellInfo(
                "Recall To Dungeon", "Kal Ort Por Tal",
                239,
                9031
            );

        public PortalTransportSpell(Mobile caster, PortalPartition partition, bool entering)
            : base(caster, null, m_Info)
        {
            m_caster = caster;
            m_partition = partition;
            m_entering = entering;
            m_caster.SendMessage("You will {0} in {1} seconds.", m_entering ? "arrive" : "depart", s_castTime);
        }

        public override SpellCircle Circle { get { return SpellCircle.First; } }

        public override TimeSpan GetCastDelay()
        {
            if (m_caster.AccessLevel > AccessLevel.Player)
                return TimeSpan.FromSeconds(1);
            return TimeSpan.FromSeconds(s_castTime);
        }
        public override void GetCastSkills(out double min, out double max)
        {
            min = 0;
            max = 0;
        }
        public override int GetMana()
        {
            return 0;
        }
        public override bool CheckCast()
        {
            if (WindFragment.ExistsOn(Caster))
            {
                Caster.SendMessage("You cannot do that while carrying the Wind Fragment."); // You can't do that while carrying the sigil.
                return false;
            }
            if (Factions.Sigil.ExistsOn(Caster))
            {
                Caster.SendLocalizedMessage(1061632); // You can't do that while carrying the sigil.
                return false;
            }

            /*
            else if (Caster.Criminal)
            {
                Caster.SendLocalizedMessage(1005561, "", 0x22); // Thou'rt a criminal and cannot escape so easily.
                return false;
            }
            else if (SpellHelper.CheckCombat(Caster, true))
            {
                Caster.SendLocalizedMessage(1005564, "", 0x22); // Wouldst thou flee during the heat of battle??
                return false;
            }
            else if (Caster is Server.Mobiles.PlayerMobile && ((Server.Mobiles.PlayerMobile)Caster).LastMurder + TimeSpan.FromMinutes(2.5) > DateTime.UtcNow)
            {
                Caster.SendMessage(0x22, "You cannot flee so easily after committing murder.");
                return false;
            }
            */

            return base.CheckCast();
        }
        public override void OnCast()
        {
            if (m_entering)
            {
                if (m_caster.HasGump(typeof(DungeonEnteredGump)))
                {
                    m_caster.CloseGump(typeof(DungeonEnteredGump));
                }
                m_partition.m_lastTeleportFromLocation = m_caster.Location;
                m_partition.m_lastTeleportFromMap = m_caster.Map;

                DungeonEnteredGump.SDungeonEnteredParams param = new DungeonEnteredGump.SDungeonEnteredParams(
                            m_caster, m_partition.m_username, m_caster.Location, m_caster.Map);

                // Send to platform.
                Point3D entryPoint = m_partition.m_entryPoint;
                entryPoint.X -= 1;
                entryPoint.Y -= 1;
                m_caster.MoveToWorld(entryPoint, PortalsSystem.s_map);

                m_caster.SendGump(new PortalControlGump(m_caster));
            }
            else
            {
                if (m_caster.HasGump(typeof(DungeonEnteredGump)))
                {
                    m_caster.CloseGump(typeof(DungeonEnteredGump));
                }

                m_caster.MoveToWorld(m_partition.m_lastTeleportFromLocation, m_partition.m_lastTeleportFromMap);
            }
            FinishSequence();
        }
    }
}