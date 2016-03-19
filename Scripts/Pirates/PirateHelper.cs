/***************************************************************************
 *                             PirateHelper.cs
 *                            ------------------
 *   begin                : August 2010
 *   author               : Sean Stavropoulos
 *   email                : sean.stavro@gmail.com
 *
 *
 ***************************************************************************/
using System;
using System.Collections;
using Server;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;
using Server.Commands;
using Server.Targeting;
using Server.Multis;
using Server.Mobiles;

namespace Server.Custom.Pirates
{
    public static class PirateHelper
    {

        #region Commands
        public static void Initialize()
        {
            CommandSystem.Register("KillAtSea", AccessLevel.GameMaster, new CommandEventHandler(KillAtSea_OnCommand));
        }

        [Usage("KillAtSea")]
        [Description("KillAtSea")]
        public static void KillAtSea_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            if (e.Length == 0)
                from.Target = new KillTarget();
            else
                e.Mobile.SendMessage(0x25, "Bad Format: [SpawnDoubloonDocks");
        }

        private class KillTarget : Target
        {
            public KillTarget()
                : base(10, false, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object o)
            {

                if (o != null)
                {
                    if (o is Mobile)
                    {
                        if (((Mobile)o).Alive)
                            PirateHelper.KillAtSea((Mobile)o);
                        else
                            from.SendMessage("That is already dead.");
                    }
                    else
                    {
                        from.SendMessage("You cannot kill that.");
                    }
                }
            }
        }

        #endregion

        public static void KillAtSea(Mobile m)
        {
            if (!(m is PlayerMobile))
            {
                m.Delete();
                return;
            }

            Effects.PlaySound(m.Location, m.Map, 0x021);
                
            m.LastKiller = null;
            m.Kill();
            m.MoveToWorld(FindRandomNearbyLand(m.Location, m.Map), m.Map);
            SendDelayedMessage(m, @"You wash ashore on a unfamiliar land.", TimeSpan.FromSeconds(2));            
        }

        public static void KillEffect(Mobile m)
        {
            m.DisruptiveAction();

            m.Warmode = false;

            m.DropHolding();

            m.Poison = null;
            m.Combatant = null;

            if (m.Paralyzed)
                m.Paralyzed = false;

            if (m.Frozen)
                m.Frozen = false;

            m.Hits = m.HitsMax;
            m.Stam = m.StamMax;
            m.Mana = m.ManaMax;

        }

        public static Point3D FindRandomNearbyLand(Point3D p, Map map)
        {
            Direction dir = (Direction)Utility.Random(7);

            int X = p.X;
            int Y = p.Y;

            int rx = 0, ry = 0;

            if (dir == Direction.East)
                rx = 1;
            else if (dir == Direction.West)
                rx = -1;
            else if (dir == Direction.South)
                ry = 1;
            else if (dir == Direction.North)
                ry = -1;
            else if (dir == Direction.Up)
                ry = rx = -1;
            else if (dir == Direction.Right)
            {
                ry = -1;
                rx = 1;
            }
            else if (dir == Direction.Down)
                ry = rx = 1;
            else//(dir == Direction.Left
            {
                ry = 1;
                rx = -1;
            }

            bool foundLand = false;
            int i = 1;

            int count = 0;
            while (foundLand == false)
            //for (int i = 1; i <= 6; ++i)
            {
                if (count++ > 500)
                    break;

                int x = X + (i * rx);
                int y = Y + (i * ry);
                int z = 0;

                if (x >= 5000)
                {
                    X = 5;
                    i = 1;
                }

                if (y >= 4050)
                {
                    Y = 5;
                    i = 1;
                }

                /*for (int j = -8; j <= 8; ++j)
                {
                    //z = from.Z + j;

                    if (map.CanFit(x, y, z, 16, false, false) && !Server.Spells.SpellHelper.CheckMulti(new Point3D(x, y, z), map) && !Region.Find(new Point3D(x, y, z), map).IsPartOf(typeof(Factions.StrongholdRegion)))
                    {
                       foundLand = true;
                       return new Point3D(x, y, z);
                    }
                }*/

                z = map.GetAverageZ(x, y);

                if (map.CanFit(x, y, z, 16, false, false) && !Server.Spells.SpellHelper.CheckMulti(new Point3D(x, y, z), map))
                {
                    foundLand = true;
                    Console.Write("\n\nKillAtSea: {0} iterations\n\n", i / 5);
                    return new Point3D(x, y, z);
                }

                i = i + 5;
            }

            return new Point3D(0, 0, 0);

        }

        public static void SendDelayedMessage(Mobile m, string str, TimeSpan t)
        {
            Timer timer = Timer.DelayCall(t, new TimerStateCallback(SendMessage), new object[] { m, str });
        }

        private static void SendMessage(object state)
        {
            object[] args = (object[])state;

            Mobile m = args[0] as Mobile;
            string str = args[1] as string;

            m.SendMessage(str);
        }



    }
}
