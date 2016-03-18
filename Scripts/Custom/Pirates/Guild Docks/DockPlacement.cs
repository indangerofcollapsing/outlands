﻿using System;
using System.Collections.Generic;
using Server.Items;
using Server.Custom.Townsystem;
using Server.Multis;

namespace Server.Custom.Pirates
{
    public class DockPlacement
    {
        public static readonly int MaximumBadTiles = 15; //3 layers of 5

        public static bool ValidatePlacement(Mobile from, BaseGuildDock dock, Point3D center)
        {
            // These are storage lists. They hold items and mobiles found in the map for further processing
            List<Item> items = new List<Item>();
            List<Mobile> mobiles = new List<Mobile>();

            Map map = from.Map;

            MultiComponentList mcl = dock.Components;

            Point3D start = new Point3D(center.X + mcl.Min.X, center.Y + mcl.Min.Y, center.Z);
            Point3D end = new Point3D(start.X + mcl.Width, start.Y + mcl.Height, center.Z);

            int badTiles = 0;

            int countX = end.X - start.X;
            int countY = end.Y - start.Y;

            for (int x = 0; x < countX; x++)
            {
                for (int y = 0; y < countY; y++)
                {
                    int tileX = start.X + x;
                    int tileY = start.Y + y;

                    StaticTile[] addTiles = mcl.Tiles[x][y];

                    if (addTiles.Length == 0)
                        continue; // There are no tiles here, continue checking somewhere else

                    Point3D testPoint = new Point3D(tileX, tileY, center.Z);

                    Region reg = Region.Find(testPoint, Map.Felucca);

                    if (!reg.AllowHousing(from, testPoint)) // Cannot place houses in dungeons, towns, treasure map areas etc
                    {
                        if (reg.IsPartOf(typeof(TempNoHousingRegion)))
                            return false;

                        if (reg.IsPartOf(typeof(TreasureRegion)))
                            return false;

                        return false;
                    }

                    LandTile landTile = map.Tiles.GetLandTile(tileX, tileY);
                    StaticTile[] tiles = map.Tiles.GetStaticTiles(tileX, tileY, true);

                    bool hasWater = false;

                    if ((landTile.ID >= 168 && landTile.ID <= 171) || (landTile.ID >= 310 && landTile.ID <= 311))
                        hasWater = true;

                    for (int i = 0; i < tiles.Length; ++i)
                    {
                        StaticTile tile = tiles[i];
                        bool isWater = (tile.ID >= 0x1796 && tile.ID <= 0x17B2);

                        if (isWater)
                            hasWater = true;
                        else if (!isWater)
                            return false;
                    }

                    if (from.AccessLevel > AccessLevel.Player)
                        from.SendMessage(String.Format("DEBUG: Land Tile ID: {0} at {1}: {2} (#{3})", landTile.ID, new Point2D(tileX, tileY), hasWater ? "water" : "no water", badTiles));

                    if (!hasWater && ++badTiles >= MaximumBadTiles)
                    {
                        from.SendMessage("This dock would be overlapping too much land.");
                        return false;
                    }

                    Sector sector = map.GetSector(tileX, tileY);

                    for (int i = 0; i < sector.Items.Count; ++i)
                    {
                        Item item = sector.Items[i];

                        if (item.Visible && item.X == tileX && item.Y == tileY)
                        {
                            if (from.AccessLevel > AccessLevel.Player)
                                from.SendMessage("DEBUG: Error at {0}", new Point2D(tileX, tileY));

                            from.SendMessage("You cannot place a guild dock over existing items.");
                            return false;
                        }
                    }

                    for (int i = 0; i < sector.Mobiles.Count; ++i)
                    {
                        Mobile m = sector.Mobiles[i];

                        if (m.X == tileX && m.Y == tileY)
                        {
                            if (from.AccessLevel > AccessLevel.Player)
                                from.SendMessage("DEBUG: Error at {0}", new Point2D(tileX, tileY));

                            from.SendMessage("There is a person in the way of the placing the dock.");
                            return false;
                        }
                    }
                }
            }

            LandTile playerTile = map.Tiles.GetLandTile(from.X, from.Y);
            StaticTile[] playerTiles = map.Tiles.GetStaticTiles(from.X, from.Y, true);

            bool playerOnWater = false;

            if ((playerTile.ID >= 168 && playerTile.ID <= 171) || (playerTile.ID >= 310 && playerTile.ID <= 311))
                playerOnWater = true;

            for (int i = 0; i < playerTiles.Length; ++i)
            {
                StaticTile tile = playerTiles[i];
                bool isWater = (tile.ID >= 0x1796 && tile.ID <= 0x17B2);

                if (isWater)
                    playerOnWater = true;
            }

            if (playerOnWater)
            {
                from.SendMessage("You must be standing on land to place a guild dock.");
                return false;
            }

            return true;
        }
    }
}
