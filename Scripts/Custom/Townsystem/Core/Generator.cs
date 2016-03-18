using System;
using Server.Commands;
using System.Collections.Generic;

namespace Server.Custom.Townsystem
{
	public class Generator
	{
		public static void Initialize()
		{
						CommandSystem.Register( "GenerateTownsystem", AccessLevel.Administrator, new CommandEventHandler( GenerateTownsystem_OnCommand ) );
            CommandSystem.Register("RespawnTreasuries", AccessLevel.Administrator, new CommandEventHandler(RespawnTreasuries_OnCommand));
        
        }

        public static void RespawnTreasuries_OnCommand(CommandEventArgs e)
        {
			while (TreasuryChest.s_AllTreasuryChests.Count > 0)
				TreasuryChest.s_AllTreasuryChests[0].Delete();

            foreach (Town town in Town.Towns)
				new TreasuryChest(town).MoveToWorld(town.Definition.TreasuryLocation, Map.Felucca);
        }

		public static void GenerateTownsystem_OnCommand( CommandEventArgs e )
		{
			new TownsystemPersistance();

            List<Faction> factions = Faction.Factions;

			foreach ( Faction faction in factions )
				Generate( faction );

			List<Town> towns = Town.Towns;

			foreach ( Town town in towns )
				Generate( town );
		}

		public static void Generate( Town town )
		{
			Map facet = Faction.Facet;

			TownDefinition def = town.Definition;
		}

        public static void Generate(Faction faction)
		{
			Map facet = Faction.Facet;

			List<Town> towns = Town.Towns;

            
			//StrongholdDefinition stronghold = faction.Definition.Stronghold;

			/*if ( !CheckExistance( stronghold.JoinStone, facet, typeof( JoinStone ) ) )
				new JoinStone( faction ).MoveToWorld( stronghold.JoinStone, facet );

			if ( !CheckExistance( stronghold.FactionStone, facet, typeof( FactionStone ) ) )
				new FactionStone( faction ).MoveToWorld( stronghold.FactionStone, facet );

			for ( int i = 0; i < stronghold.Monoliths.Length; ++i )
			{
				Point3D monolith = stronghold.Monoliths[i];

				if ( !CheckExistance( monolith, facet, typeof( StrongholdMonolith ) ) )
					new StrongholdMonolith( towns[i], faction ).MoveToWorld( monolith, facet );
			}*/
		}

		private static bool CheckExistance( Point3D loc, Map facet, Type type )
		{
			foreach ( Item item in facet.GetItemsInRange( loc, 0 ) )
			{
				if ( type.IsAssignableFrom( item.GetType() ) )
					return true;
			}

			return false;
		}
	}
}