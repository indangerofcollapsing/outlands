﻿using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Misc;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Targeting;
using Server.Achievements;
using Server.Accounting;
using System.Linq;
using Server.Custom;

namespace Server
{
    public static class UOACZMultis
    {
        public static void Initialize()
        {
            CommandSystem.Register("UOACZBuildTowerSouth", AccessLevel.GameMaster, new CommandEventHandler(UOACZBuildTowerSouth));
            CommandSystem.Register("UOACZBuildTowerWest", AccessLevel.GameMaster, new CommandEventHandler(UOACZBuildTowerWest));
            CommandSystem.Register("UOACZBuildTowerEast", AccessLevel.GameMaster, new CommandEventHandler(UOACZBuildTowerEast));

            CommandSystem.Register("UOACZBuildEastWestGate", AccessLevel.GameMaster, new CommandEventHandler(UOACZBuildEastWestGate));
            CommandSystem.Register("UOACZBuildNorthSouthGate", AccessLevel.GameMaster, new CommandEventHandler(UOACZBuildNorthSouthGate));

            CommandSystem.Register("UOACZBuildConstructionPlatformWest", AccessLevel.GameMaster, new CommandEventHandler(UOACZBuildConstructionPlatformWest));
            CommandSystem.Register("UOACZBuildConstructionPlatformNorth", AccessLevel.GameMaster, new CommandEventHandler(UOACZBuildConstructionPlatformNorth));
            CommandSystem.Register("UOACZBuildConstructionPlatformEast", AccessLevel.GameMaster, new CommandEventHandler(UOACZBuildConstructionPlatformEast));
            CommandSystem.Register("UOACZBuildConstructionPlatformSouth", AccessLevel.GameMaster, new CommandEventHandler(UOACZBuildConstructionPlatformSouth));

            Timer.DelayCall(TimeSpan.FromTicks(1), delegate
            {
            });
        }
        
        [Usage("UOACZBuildTowerSouth")]
        [Description("Builds a south entrance sentry tower")]
        public static void UOACZBuildTowerSouth(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            BuildTower(player.Location, player.Map, Direction.South);
        }

        [Usage("UOACZBuildTowerWest")]
        [Description("Builds a west entrance sentry tower")]
        public static void UOACZBuildTowerWest(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            BuildTower(player.Location, player.Map, Direction.West);
        }

        [Usage("UOACZBuildTowerEast")]
        [Description("Builds a east entrance sentry tower")]
        public static void UOACZBuildTowerEast(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            BuildTower(player.Location, player.Map, Direction.East);
        }

        public static void BuildTower(Point3D startPoint, Map startMap, Direction direction)
        {
            Point3D location = startPoint;
            Map map = startMap;

            int columns = 9; 

            Teleporter lowerTeleporter = null;
            Teleporter upperTeleporter = null;

            for (int a = 0; a < columns; a++)
            {
                //South
                Point3D southPoint = location;
                southPoint.Z += (a * 5);

                if (a == 0)
                {   
                    switch (direction)
                    {
                        case Direction.South:
                            new UOACZStairNorth().MoveToWorld(new Point3D(southPoint.X, southPoint.Y + 1, southPoint.Z), map);
                            new UOACZStairWest().MoveToWorld(new Point3D(southPoint.X + 1, southPoint.Y, southPoint.Z), map);
                        break;

                        case Direction.West:
                            new UOACZStairEast().MoveToWorld(new Point3D(southPoint.X - 2, southPoint.Y, southPoint.Z), map);
                            new UOACZStairEast().MoveToWorld(new Point3D(southPoint.X - 2, southPoint.Y - 1, southPoint.Z), map);
                            new UOACZStairSouth().MoveToWorld(new Point3D(southPoint.X - 1, southPoint.Y - 2, southPoint.Z), map);
                        break;

                        case Direction.East:
                            new UOACZStairWest().MoveToWorld(new Point3D(southPoint.X + 1, southPoint.Y - 1, southPoint.Z), map);
                            new UOACZStairSouth().MoveToWorld(new Point3D(southPoint.X, southPoint.Y - 2, southPoint.Z), map);
                        break;
                    }

                    new UOACZBlock().MoveToWorld(new Point3D(southPoint.X, southPoint.Y, southPoint.Z), map);
                    new UOACZBlock().MoveToWorld(new Point3D(southPoint.X - 1, southPoint.Y, southPoint.Z), map);
                    new UOACZBlock().MoveToWorld(new Point3D(southPoint.X, southPoint.Y - 1, southPoint.Z), map);
                    new UOACZBlock().MoveToWorld(new Point3D(southPoint.X - 1, southPoint.Y - 1, southPoint.Z), map);
                }

                if (a == 1)
                {
                    if (direction == Direction.South)
                    {
                        lowerTeleporter = new Teleporter();
                        lowerTeleporter.MoveToWorld(new Point3D(southPoint.X, southPoint.Y, southPoint.Z), map);
                    }

                    if (direction == Direction.West)
                    {
                        lowerTeleporter = new Teleporter();
                        lowerTeleporter.MoveToWorld(new Point3D(southPoint.X - 1, southPoint.Y, southPoint.Z), map);
                    }

                    if (direction == Direction.East)
                    {
                        lowerTeleporter = new Teleporter();
                        lowerTeleporter.MoveToWorld(new Point3D(southPoint.X, southPoint.Y - 1, southPoint.Z), map);
                    } 
                }

                if (a == 1 || a == 2 || a == 3 || a == 4)
                {
                    if (direction != Direction.South)
                        new UOACZBlock().MoveToWorld(new Point3D(southPoint.X, southPoint.Y, southPoint.Z), map);

                    if (direction != Direction.West)
                        new UOACZBlock().MoveToWorld(new Point3D(southPoint.X - 1, southPoint.Y, southPoint.Z), map);

                    if (direction != Direction.East)
                        new UOACZBlock().MoveToWorld(new Point3D(southPoint.X, southPoint.Y - 1, southPoint.Z), map);

                    if (direction != Direction.West)
                        new UOACZBlock().MoveToWorld(new Point3D(southPoint.X - 1, southPoint.Y - 1, southPoint.Z), map);
                }

                else if (a == (columns - 1))
                {
                    new UOACZBlock().MoveToWorld(new Point3D(southPoint.X, southPoint.Y, southPoint.Z), map);
                    new UOACZBlock().MoveToWorld(new Point3D(southPoint.X - 1, southPoint.Y, southPoint.Z), map);
                    new UOACZStairSouth().MoveToWorld(new Point3D(southPoint.X, southPoint.Y - 1, southPoint.Z), map);
                    new UOACZBlock().MoveToWorld(new Point3D(southPoint.X - 1, southPoint.Y - 1, southPoint.Z), map);

                    upperTeleporter = new Teleporter();
                    upperTeleporter.MoveToWorld(new Point3D(southPoint.X, southPoint.Y - 1, southPoint.Z + 2), map);

                    lowerTeleporter.MapDest = map;
                    lowerTeleporter.PointDest = upperTeleporter.Location;

                    upperTeleporter.MapDest = map;
                    upperTeleporter.PointDest = lowerTeleporter.Location;
                }

                else
                {
                    new UOACZBlock().MoveToWorld(new Point3D(southPoint.X, southPoint.Y, southPoint.Z), map);
                    new UOACZBlock().MoveToWorld(new Point3D(southPoint.X - 1, southPoint.Y, southPoint.Z), map);
                    new UOACZBlock().MoveToWorld(new Point3D(southPoint.X, southPoint.Y - 1, southPoint.Z), map);
                    new UOACZBlock().MoveToWorld(new Point3D(southPoint.X - 1, southPoint.Y - 1, southPoint.Z), map);
                }
            }
        }

        [Usage("UOACZBuildEastWestGate")]
        [Description("Builds an east-west gate")]
        public static void UOACZBuildEastWestGate(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            BuildGate(player.Location, player.Map, Direction.East);
        }

        [Usage("UOACZBuildNorthSouthGate")]
        [Description("Builds an north-south gate")]
        public static void UOACZBuildNorthSouthGate(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            BuildGate(player.Location, player.Map, Direction.North);
        }

        public static void BuildGate(Point3D startPoint, Map startMap, Direction direction)
        {
            Point3D location = startPoint;
            Map map = startMap;

            switch (direction)
            {
                case Direction.East:
                    new UOACZEastWestGateLeftWall().MoveToWorld(new Point3D(location.X, location.Y, location.Z), map);
                    new UOACZEastWestGateLeftDoor().MoveToWorld(new Point3D(location.X + 1, location.Y, location.Z), map);
                    new UOACZEastWestGateRightDoor().MoveToWorld(new Point3D(location.X + 2, location.Y, location.Z), map);
                    new UOACZEastWestGateRightWall().MoveToWorld(new Point3D(location.X + 3, location.Y, location.Z), map);
                break;

                case Direction.North:
                    new UOACZNorthSouthGateLeftWall().MoveToWorld(new Point3D(location.X, location.Y, location.Z), map);
                    new UOACZNorthSouthGateLeftDoor().MoveToWorld(new Point3D(location.X, location.Y - 1, location.Z), map);
                    new UOACZNorthSouthGateRightDoor().MoveToWorld(new Point3D(location.X, location.Y - 2, location.Z), map);
                    new UOACZNorthSouthGateRightWall().MoveToWorld(new Point3D(location.X, location.Y - 3, location.Z), map);
                break;
            }
        }

        [Usage("UOACZBuildConstructionPlatformWest")]
        [Description("Builds a west facing construction platform")]
        public static void UOACZBuildConstructionPlatformWest(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            BuildConstructionPlatform(player.Location, player.Map, Direction.West);
        }

        [Usage("UOACZBuildConstructionPlatformNorth")]
        [Description("Builds a north facing construction platform")]
        public static void UOACZBuildConstructionPlatformNorth(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            BuildConstructionPlatform(player.Location, player.Map, Direction.North);
        }

        [Usage("UOACZBuildConstructionPlatformEast")]
        [Description("Builds a east facing construction platform")]
        public static void UOACZBuildConstructionPlatformEast(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            BuildConstructionPlatform(player.Location, player.Map, Direction.East);
        }

        [Usage("UOACZBuildConstructionPlatformSouth")]
        [Description("Builds a south facing construction platform")]
        public static void UOACZBuildConstructionPlatformSouth(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            BuildConstructionPlatform(player.Location, player.Map, Direction.South);
        }

        public static void BuildConstructionPlatform(Point3D startPoint, Map startMap, Direction direction)
        {
            Point3D location = startPoint;
            Map map = startMap;

            UOACZConstructionTile constructionTile;
            UOACZConstructionObjectEffectTargeter targeter;

            switch (direction)
            {
                case Direction.West:
                    new UOACZBlock().MoveToWorld(new Point3D(location.X, location.Y, location.Z), map);
                    new UOACZBlock().MoveToWorld(new Point3D(location.X, location.Y, location.Z + 5), map);
                    new UOACZBlock().MoveToWorld(new Point3D(location.X, location.Y, location.Z + 10), map);
                    new UOACZBlock().MoveToWorld(new Point3D(location.X, location.Y, location.Z + 15), map);                    

                    new UOACZBlock().MoveToWorld(new Point3D(location.X + 1, location.Y, location.Z), map);
                    new UOACZBlock().MoveToWorld(new Point3D(location.X + 1, location.Y, location.Z + 5), map);                    

                    new UOACZBlock().MoveToWorld(new Point3D(location.X + 2, location.Y, location.Z), map);
                    new UOACZStairWest().MoveToWorld(new Point3D(location.X + 2, location.Y, location.Z + 5), map);

                    new UOACZStairWest().MoveToWorld(new Point3D(location.X + 3, location.Y, location.Z), map);
                break;

                case Direction.North:
                    new UOACZBlock().MoveToWorld(new Point3D(location.X, location.Y, location.Z), map);
                    new UOACZBlock().MoveToWorld(new Point3D(location.X, location.Y, location.Z + 5), map);
                    new UOACZBlock().MoveToWorld(new Point3D(location.X, location.Y, location.Z + 10), map);
                    new UOACZBlock().MoveToWorld(new Point3D(location.X, location.Y, location.Z + 15), map);                    

                    new UOACZBlock().MoveToWorld(new Point3D(location.X, location.Y + 1, location.Z), map);
                    new UOACZBlock().MoveToWorld(new Point3D(location.X, location.Y + 1, location.Z + 5), map);                   

                    new UOACZBlock().MoveToWorld(new Point3D(location.X, location.Y + 2, location.Z), map);
                    new UOACZStairNorth().MoveToWorld(new Point3D(location.X, location.Y + 2, location.Z + 5), map);

                    new UOACZStairNorth().MoveToWorld(new Point3D(location.X, location.Y + 3, location.Z), map);
                break;

                case Direction.East:
                    new UOACZBlock().MoveToWorld(new Point3D(location.X, location.Y, location.Z), map);
                    new UOACZBlock().MoveToWorld(new Point3D(location.X, location.Y, location.Z + 5), map);
                    new UOACZBlock().MoveToWorld(new Point3D(location.X, location.Y, location.Z + 10), map);
                    new UOACZBlock().MoveToWorld(new Point3D(location.X, location.Y, location.Z + 15), map);
                    
                    new UOACZBlock().MoveToWorld(new Point3D(location.X - 1, location.Y, location.Z), map);
                    new UOACZBlock().MoveToWorld(new Point3D(location.X - 1, location.Y, location.Z + 5), map);                    

                    new UOACZBlock().MoveToWorld(new Point3D(location.X - 2, location.Y, location.Z), map);
                    new UOACZStairEast().MoveToWorld(new Point3D(location.X - 2, location.Y, location.Z + 5), map);

                    new UOACZStairEast().MoveToWorld(new Point3D(location.X - 3, location.Y, location.Z), map);
                break;

                case Direction.South:
                    new UOACZBlock().MoveToWorld(new Point3D(location.X, location.Y, location.Z), map);
                    new UOACZBlock().MoveToWorld(new Point3D(location.X, location.Y, location.Z + 5), map);
                    new UOACZBlock().MoveToWorld(new Point3D(location.X, location.Y, location.Z + 10), map);
                    new UOACZBlock().MoveToWorld(new Point3D(location.X, location.Y, location.Z + 15), map);
                   
                    new UOACZBlock().MoveToWorld(new Point3D(location.X, location.Y - 1, location.Z), map);
                    new UOACZBlock().MoveToWorld(new Point3D(location.X, location.Y - 1, location.Z + 5), map);                    

                    new UOACZBlock().MoveToWorld(new Point3D(location.X, location.Y - 2, location.Z), map);
                    new UOACZStairSouth().MoveToWorld(new Point3D(location.X, location.Y - 2, location.Z + 5), map);

                    new UOACZStairSouth().MoveToWorld(new Point3D(location.X, location.Y - 3, location.Z), map);
                break;
            }
        }
    }
}