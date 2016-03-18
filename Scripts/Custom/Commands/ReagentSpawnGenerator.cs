using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Commands;
using System.Collections;
using System.Collections.Generic;

namespace Server
{
	public class ReagentSpawnGenerator
	{
		private static Rectangle2D[] m_BritRegions = new Rectangle2D[]
			{
				new Rectangle2D( new Point2D( 0, 0 ), new Point2D( 6142, 4094 ) ),
			};
			
		private static XmlSpawner.SpawnObject spawnGarlic;
		private static XmlSpawner.SpawnObject spawnGinseng;
		private static XmlSpawner.SpawnObject spawnMandrakeRoot;
		private static XmlSpawner.SpawnObject spawnSulfurousAsh;
		private static XmlSpawner.SpawnObject spawnSpidersSilk;
		private static XmlSpawner.SpawnObject spawnBlackPearl;
		private static XmlSpawner.SpawnObject spawnBloodMoss;
		private static XmlSpawner.SpawnObject[] spawnRegs = new XmlSpawner.SpawnObject[7];

		private static int[] m_Trees = new int[]
			{
				0xC9E,
				0xCA8,
				0xCAA,
				0xCCA,
				0xCCB,
				0xCCC,
				0xCCD,
				0xCD0,
				0xCD3,
				0xCD6,
				0xCD8,
				0xCDA,
				0xCDD,
				0xCE0,
				0xCE3,
				0xCE6,
				0xCF8,
				0xCFB,
				0xCFE,
				0xD01,
				0xD43,
				0xD59,
				0xD70,
				0xD85,
				0xD94,
				0xD98,
				0xD9C,
				0xDA0,
				0xDA4,
				0xDA8
				//0x12B9,
			};

		private const int MaxRegNodeSpawns = 2;    // How many stacks of reagents can be spawned per node.
		private const int MaxSpawnsPerReagent = 2; // How many of each reagent type can be spawned per node.
		private const int RegSpawnsPerTick = 1;    // Respawn this amount of reagents per tick per node.
		private const int MaxRegStackAmount = 4;   // Maximum amount per reagent.
		private const int MinRegStackAmount = 1;   // Minimum amount per reagent.

		public static void Initialize()
		{
			CommandSystem.Register( "RegSpawnGen", AccessLevel.Administrator, new CommandEventHandler( RegSpawnGen_OnCommand ) );

			spawnGarlic = new XmlSpawner.SpawnObject("Garlic", MaxSpawnsPerReagent);
			spawnGarlic.SpawnsPerTick = RegSpawnsPerTick;
			spawnRegs[0] = spawnGarlic;

			spawnGinseng = new XmlSpawner.SpawnObject("Ginseng", MaxSpawnsPerReagent);
			spawnGinseng.SpawnsPerTick = RegSpawnsPerTick;
			spawnRegs[1] = spawnGinseng;

			spawnMandrakeRoot = new XmlSpawner.SpawnObject("MandrakeRoot", MaxSpawnsPerReagent);
			spawnMandrakeRoot.SpawnsPerTick = RegSpawnsPerTick;
			spawnRegs[2] = spawnMandrakeRoot;

			spawnSulfurousAsh = new XmlSpawner.SpawnObject("SulfurousAsh", MaxSpawnsPerReagent);
			spawnSulfurousAsh.SpawnsPerTick = RegSpawnsPerTick;
			spawnRegs[3] = spawnSulfurousAsh;

			spawnSpidersSilk = new XmlSpawner.SpawnObject("SpidersSilk", MaxSpawnsPerReagent);
			spawnSpidersSilk.SpawnsPerTick = RegSpawnsPerTick;
			spawnRegs[4] = spawnSpidersSilk;

			spawnBlackPearl = new XmlSpawner.SpawnObject("BlackPearl", MaxSpawnsPerReagent);
			spawnBlackPearl.SpawnsPerTick = RegSpawnsPerTick;
			spawnRegs[5] = spawnBlackPearl;

			spawnBloodMoss = new XmlSpawner.SpawnObject("BloodMoss", MaxSpawnsPerReagent);
			spawnBloodMoss.SpawnsPerTick = RegSpawnsPerTick;
			spawnRegs[6] = spawnBloodMoss;
		}

		[Usage( "RegSpawnGen" )]
		[Description( "Generates reagent spawners by analyzing the map. Slow." )]
		public static void RegSpawnGen_OnCommand( CommandEventArgs e )
		{
			Generate();
		}

		private static Map m_Map;
		private static int m_Count;

		public static void Generate()
		{
			World.Broadcast( 0x35, true, "Generating reagent spawners, please wait." );

			Network.NetState.FlushAll();
			Network.NetState.Pause();

			m_Map = Map.Felucca;
			m_Count = 0;

			for ( int i = 0; i < m_BritRegions.Length; ++i )
				Generate( m_BritRegions[i] );

			int feluccaCount = m_Count;

			Network.NetState.Resume();

			World.Broadcast( 0x35, true, "Reagent spawner generation complete. {0}", feluccaCount );
		}

		public static bool IsTree( int id )
		{
			if ( id > m_Trees[m_Trees.Length - 1] )
				return false;

			for ( int i = 0; i < m_Trees.Length; ++i )
			{
				int delta = id - m_Trees[i];

				if ( delta < 0 )
					return false;
				else if ( delta == 0 )
					return true;
			}

			return false;
		}

		private static void AddRegSpawner( int x, int y, int z )
		{
			if ( 0.00174 < Utility.RandomDouble() )
				return;
		
			if ( !m_Map.CanFit( x, y, z, 1, false, false ) )
				return;

			XmlSpawner spawner = new XmlSpawner( MaxSpawnsPerReagent, 3, 7, 0, 0, 50, "Nightshade" );
			spawner.SpawnObjects[0].SpawnsPerTick = RegSpawnsPerTick;

			spawner.StackAmount = Utility.Random( MinRegStackAmount, MaxRegStackAmount );
			spawner.SpawnObjects = spawnRegs;
			spawner.Name = "Eni's Reagent Spawner #" + m_Count;
			spawner.MoveToWorld( new Point3D( x, y, z), Map.Felucca);
			spawner.MaxCount = MaxRegNodeSpawns;

			++m_Count;
		}

		public static void Generate( Rectangle2D region )
		{
			int OakTree = 0x12B9;
			
			for ( int rx = 0; rx < region.Width; ++rx )
			{
				for ( int ry = 0; ry < region.Height; ++ry )
				{
					int vx = rx + region.X;
					int vy = ry + region.Y;

					StaticTile[] tiles = m_Map.Tiles.GetStaticTiles( vx, vy );

					for ( int i = 0; i < tiles.Length; ++i )
					{
						StaticTile tile = tiles[i];

						int id = tile.ID;
						id &= 0x3FFF;
						int z = tile.Z;

						if ( id == OakTree )
						{
							AddRegSpawner( vx + 1, vy, z );
						}
						else if ( IsTree( id ) )
						{
							AddRegSpawner( vx + 1, vy, z );
						}
					}
				}
			}
		}
	}
}
