using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Guilds;
using Server.Items;
using Server.Misc;
using Server.Regions;
using Server.Spells;

namespace Server.Multis
{
	public enum HousePlacementResult
	{
		Valid,
		BadRegion,
		BadLand,
		BadStatic,
		BadItem,
		NoSurface,
		BadRegionHidden,
		BadRegionTemp,
		InvalidCastleKeep,
		BadRegionRaffle
	}

	public class HousePlacement
	{
		private const int YardSize = 5;
        private const int SideyardSize = 3;
		
		private static int[] m_RoadIDs = new int[]
		{
			0x0071, 0x0078,
			0x00E8, 0x00EB,
			0x07AE, 0x07B1,
			0x3FF4, 0x3FF4,
			0x3FF8, 0x3FFB,
			0x0442, 0x0479,
			0x0501, 0x0510,
			0x0009, 0x0015,
			0x0150, 0x015C
		};

        public static void ShowBlockingTile(Mobile from, int x, int y, int z, Map map)
        {
            from.SendSound(0x64B);

            Effects.SendLocationParticles(EffectItem.Create(new Point3D(x, y, z), map, TimeSpan.FromSeconds(3.00)), 0x3709, 10, 60, 2954, 0, 5029, 0);
        }

		public static HousePlacementResult Check(Mobile from, int multiID, Point3D center, out ArrayList toMove, bool east_facing_door)
		{			
			toMove = new ArrayList();

			Map map = from.Map;

			if ( map == null || map == Map.Internal )
				return HousePlacementResult.BadLand; // A house cannot go here

			if ( from.AccessLevel >= AccessLevel.GameMaster )
				return HousePlacementResult.Valid; // Staff can place anywhere

			if ( map == Map.Ilshenar || SpellHelper.IsFeluccaT2A( map, center ) )
				return HousePlacementResult.BadRegion; // No houses in Ilshenar/T2A

			if ( map == Map.Malas && ( multiID == 0x007C || multiID == 0x007E ) )
				return HousePlacementResult.InvalidCastleKeep;

			NoHousingRegion noHousingRegion = (NoHousingRegion) Region.Find( center, map ).GetRegion( typeof( NoHousingRegion ) );

			if ( noHousingRegion != null )
				return HousePlacementResult.BadRegion;

			MultiComponentList mcl = MultiData.GetComponents( multiID );

            //AOS House With Stairs
			if ( multiID >= 0x13EC && multiID < 0x1D00 )
				HouseFoundation.AddStairsTo( ref mcl );

			//Northwest Corner of House
			Point3D start = new Point3D( center.X + mcl.Min.X, center.Y + mcl.Min.Y, center.Z );

			List<Item> items = new List<Item>();
			List<Mobile> mobiles = new List<Mobile>();
			List<Point2D> yard = new List<Point2D>(), borders = new List<Point2D>();

			 /* Placement Rules:			  
			 1) All tiles which are around the -outside- of the foundation must not have anything impassable.
			 2) No impassable object or land tile may come in direct contact with any part of the house.
			 3) Five tiles from the front and back of the house must be completely clear of all house tiles.
			 4) The foundation must rest flatly on a surface. Any bumps around the foundation are not allowed.
			 5) No foundation tile may reside over terrain which is viewed as a road.
			 */

			for ( int x = 0; x < mcl.Width; ++x )
			{
				for ( int y = 0; y < mcl.Height; ++y )
				{
					int tileX = start.X + x;
					int tileY = start.Y + y;

                    StaticTile[] addTiles = mcl.Tiles[x][y];

					if ( addTiles.Length == 0 )
						continue;

					Point3D testPoint = new Point3D( tileX, tileY, center.Z );
					Region reg = Region.Find( testPoint, map );

					if ( !reg.AllowHousing( from, testPoint ) ) // Cannot place houses in dungeons, towns, treasure map areas etc
					{
						if ( reg.IsPartOf( typeof( TempNoHousingRegion ) ) )
							return HousePlacementResult.BadRegionTemp;

						if ( reg.IsPartOf( typeof( TreasureRegion ) ) || reg.IsPartOf( typeof( HouseRegion ) ) )
							return HousePlacementResult.BadRegionHidden;

						if ( reg.IsPartOf( typeof( HouseRaffleRegion ) ) )
							return HousePlacementResult.BadRegionRaffle;

						return HousePlacementResult.BadRegion;
					}

                    LandTile landTile = map.Tiles.GetLandTile(tileX, tileY);
                    int landID = landTile.ID & TileData.MaxLandValue;
                    
                    StaticTile[] oldTiles = map.Tiles.GetStaticTiles(tileX, tileY, true);

					Sector sector = map.GetSector( tileX, tileY );

					items.Clear();

					for ( int i = 0; i < sector.Items.Count; ++i )
					{
						Item item = sector.Items[i];

						if ( item.Visible && item.X == tileX && item.Y == tileY )
							items.Add( item );
					}

					mobiles.Clear();

					for ( int i = 0; i < sector.Mobiles.Count; ++i )
					{
						Mobile m = sector.Mobiles[i];

						if ( m.X == tileX && m.Y == tileY )
							mobiles.Add( m );
					}

					int landStartZ = 0, landAvgZ = 0, landTopZ = 0;

					map.GetAverageZ( tileX, tileY, ref landStartZ, ref landAvgZ, ref landTopZ );

					bool hasFoundation = false;

					for ( int i = 0; i < addTiles.Length; ++i )
					{
                        StaticTile addTile = addTiles[i];

						if ( addTile.ID == 0x1 ) //Nodraw
							continue;

						TileFlag addTileFlags = TileData.ItemTable[addTile.ID & TileData.MaxItemValue].Flags;

						bool isFoundation = ( addTile.Z == 0 && (addTileFlags & TileFlag.Wall) != 0 );
						bool hasSurface = false;

						if ( isFoundation )
							hasFoundation = true;

						int addTileZ = center.Z + addTile.Z;
						int addTileTop = addTileZ + addTile.Height;

						if ( (addTileFlags & TileFlag.Surface) != 0 )
							addTileTop += 16;

                        //Broke Rule 2
                        if (addTileTop > landStartZ && landAvgZ > addTileZ)
                        {
                            ShowBlockingTile(from, tileX, tileY, landAvgZ, from.Map);

                            return HousePlacementResult.BadLand;
                        }

						if ( isFoundation && ((TileData.LandTable[landTile.ID & TileData.MaxLandValue].Flags & TileFlag.Impassable) == 0) && landAvgZ == center.Z )						
							hasSurface = true;

						for ( int j = 0; j < oldTiles.Length; ++j )
						{
                            StaticTile oldTile = oldTiles[j];
							ItemData id = TileData.ItemTable[oldTile.ID & TileData.MaxItemValue];

                            //Rules 2 Broken
                            if ((id.Impassable || (id.Surface && (id.Flags & TileFlag.Background) == 0)) && addTileTop > oldTile.Z && (oldTile.Z + id.CalcHeight) > addTileZ)
                            {
                                ShowBlockingTile(from, tileX, tileY, landAvgZ, from.Map);

                                return HousePlacementResult.BadStatic;
                            }
						}

						for ( int j = 0; j < items.Count; ++j )
						{
							Item item = items[j];
							ItemData id = item.ItemData;

							if ( addTileTop > item.Z && (item.Z + id.CalcHeight) > addTileZ )
							{
								if ( item.Movable )
									toMove.Add( item );

                                //Broke Rule 2
                                else if ((id.Impassable || (id.Surface && (id.Flags & TileFlag.Background) == 0)))
                                {
                                    ShowBlockingTile(from, tileX, tileY, item.Z + id.CalcHeight, from.Map);

                                    return HousePlacementResult.BadItem;
                                }
							}
						}

                        //Broke Rule 4
                        if (isFoundation && !hasSurface)
                        {
                            ShowBlockingTile(from, tileX, tileY, addTileTop, from.Map);

                            return HousePlacementResult.NoSurface;
                        }

						for ( int j = 0; j < mobiles.Count; ++j )
						{
							Mobile m = mobiles[j];

							if ( addTileTop > m.Z && (m.Z + 16) > addTileZ )
								toMove.Add( m );
						}
					}

					for ( int i = 0; i < m_RoadIDs.Length; i += 2 )
					{
                        //Broke Rule 5                        
                        if (landID >= m_RoadIDs[i] && landID <= m_RoadIDs[i + 1])
                        {
                            ShowBlockingTile(from, tileX, tileY, landAvgZ, from.Map);

                            return HousePlacementResult.BadLand;
                        }
					}

					if ( hasFoundation || east_facing_door)
					{						
						int x_expanse = east_facing_door ? YardSize : SideyardSize;
						int y_expanse = east_facing_door ? YardSize : YardSize;
                        						
						for (int xOffset = -x_expanse; xOffset <= x_expanse; ++xOffset)
						{
							for (int yOffset = -y_expanse; yOffset <= y_expanse; ++yOffset)
							{
								Point2D yardPoint = new Point2D( tileX + xOffset, tileY + yOffset );

								if ( !yard.Contains( yardPoint ) )
									yard.Add( yardPoint );
							}
						}

						for ( int xOffset = -1; xOffset <= 1; ++xOffset )
						{
							for ( int yOffset = -1; yOffset <= 1; ++yOffset )
							{
								if ( xOffset == 0 && yOffset == 0 )
									continue;

								int vx = x + xOffset;
								int vy = y + yOffset;

								if ( vx >= 0 && vx < mcl.Width && vy >= 0 && vy < mcl.Height )
								{
									StaticTile[] breakTiles = mcl.Tiles[vx][vy];
									bool shouldBreak = false;

									for ( int i = 0; !shouldBreak && i < breakTiles.Length; ++i )
									{
										StaticTile breakTile = breakTiles[i];

										if ( breakTile.Height == 0 && breakTile.Z <= 8 && TileData.ItemTable[breakTile.ID & TileData.MaxItemValue].Surface )
											shouldBreak = true;
									}

									if ( shouldBreak )
										continue;
								}

								Point2D borderPoint = new Point2D( tileX + xOffset, tileY + yOffset );

								if ( !borders.Contains( borderPoint ) )
									borders.Add( borderPoint );
							}
						}
					}
				}
			}

			for ( int i = 0; i < borders.Count; ++i )
			{
				Point2D borderPoint = borders[i];

				LandTile landTile = map.Tiles.GetLandTile( borderPoint.X, borderPoint.Y );
				int landID = landTile.ID & TileData.MaxLandValue;

                //Broke Rule
                if ((TileData.LandTable[landID].Flags & TileFlag.Impassable) != 0)
                {
                    ShowBlockingTile(from, borderPoint.X, borderPoint.Y, landTile.Z, from.Map);

                    return HousePlacementResult.BadLand;
                }

				for ( int j = 0; j < m_RoadIDs.Length; j += 2 )
				{
                    //Broke Rule 5                    
                    if (landID >= m_RoadIDs[j] && landID <= m_RoadIDs[j + 1])
                    {
                        ShowBlockingTile(from, borderPoint.X, borderPoint.Y, landTile.Z, from.Map);

                        return HousePlacementResult.BadLand;
                    }
				}

                StaticTile[] tiles = map.Tiles.GetStaticTiles(borderPoint.X, borderPoint.Y, true);

				for ( int j = 0; j < tiles.Length; ++j )
				{
                    StaticTile tile = tiles[j];
					ItemData id = TileData.ItemTable[tile.ID & TileData.MaxItemValue];

                    //Broke Rule 1
                    if (id.Impassable || (id.Surface && (id.Flags & TileFlag.Background) == 0 && (tile.Z + id.CalcHeight) > (center.Z + 2)))
                    {
                        ShowBlockingTile(from, borderPoint.X, borderPoint.Y, tile.Z + id.CalcHeight, from.Map);

                        return HousePlacementResult.BadStatic;
                    }
				}

				Sector sector = map.GetSector( borderPoint.X, borderPoint.Y );
				List<Item> sectorItems = sector.Items;

				for ( int j = 0; j < sectorItems.Count; ++j )
				{
					Item item = sectorItems[j];

					if ( item.X != borderPoint.X || item.Y != borderPoint.Y || item.Movable )
						continue;

					ItemData id = item.ItemData;

                    //Broke Rule 1
                    if (id.Impassable || (id.Surface && (id.Flags & TileFlag.Background) == 0 && (item.Z + id.CalcHeight) > (center.Z + 2)))
                    {
                        ShowBlockingTile(from, borderPoint.X, borderPoint.Y, item.Z + id.CalcHeight, from.Map);

                        return HousePlacementResult.BadItem;
                    }
				}
			}

			List<Sector> sectors = new List<Sector>();
			List<BaseHouse> houses = new List<BaseHouse>();

			for ( int i = 0; i < yard.Count; i++ ) 
            {
				Sector sector = map.GetSector( yard[i] );
				
				if ( !sectors.Contains( sector ) ) 
                {
					sectors.Add( sector );
					
					if ( sector.Multis != null ) 
                    {
						for ( int j = 0; j < sector.Multis.Count; j++ ) 
                        {
							if ( sector.Multis[j] is BaseHouse ) 
                            {
								BaseHouse house = (BaseHouse)sector.Multis[j];

								if ( !houses.Contains( house ) )                                 
									houses.Add( house );								
							}
						}
					}
				}
			}

			for ( int i = 0; i < yard.Count; ++i )
			{
				foreach ( BaseHouse b in houses )
                {
                    //Broke Rule 3
                    if (b.Contains(yard[i]))
                    {
                        //TEST: Determine Location to Display Overlap in Yard

                        return HousePlacementResult.BadStatic;
                    }
				}
			}

			return HousePlacementResult.Valid;
		}
	}
}