using System;
using System.Collections.Generic;
using System.Diagnostics;

using Server.Regions;

namespace Server.Custom.Townsystem
{
	public enum ETownId
	{
		Britain = 0,
		Cove,
		Jhelom,
		Magincia,
		Minoc,
		Moonglow,
		Nujelm,
		Ocllo,
		SerpentsHold,
		SkaraBrae,
		Trinsic,
		Vesper,
		Yew,

		NumTowns,
	};

	public class TownDefinition
	{
		private int m_Sort;
		private string m_Region;
		private string m_TownName;
        private string m_FriendlyName;
		private ETownId m_TownId;

        private string m_TownCrystalName;
        private string m_TownBrazierName;
        private string m_TownFlagName;

        private Point3D m_ElectionStoneLocation;
        private Point3D[] m_TreasuryWallEastLocations;
        private Point3D[] m_TreasuryWallNorthLocations;
        private Point3D m_TreasuryLocation;
        private Point3D m_TreasuryGoLocation;
        private Rectangle3D[] m_TreasuryRects;
        private int m_TreasuryID;
        private int m_TownBuffSpellID;
        private string m_TownBuffDescription;
        private int m_CostFactor;

		public int Sort{ get{ return m_Sort; } }
		public string Region{ get{ return m_Region; } }
		public ETownId TownId { get { return m_TownId; } }
		public string TownName { get { return m_TownName; } }
        public string FriendlyName { get { return m_FriendlyName; } }
        public string TownCrystalName { get { return m_TownCrystalName; } }
        public string TownBrazierName { get { return m_TownBrazierName; } }
        public string TownFlagName { get { return m_TownFlagName; } }
        public Point3D ElectionStoneLocation { get { return m_ElectionStoneLocation; } }
        public Point3D[] TreasuryWallEastLocations { get { return m_TreasuryWallEastLocations; } }
        public Point3D[] TreasuryWallNorthLocations { get { return m_TreasuryWallNorthLocations; } }
        public Point3D TreasuryLocation { get { return m_TreasuryLocation; } }
        public Point3D TreasuryGoLocation { get { return m_TreasuryGoLocation; } }
        public Rectangle3D[] TreasuryRects { get { return m_TreasuryRects; } }
        public int TreasuryID { get { return m_TreasuryID; } }
        public int CostFactor { get { return m_CostFactor; } }
		
		private List<TreasuryRegion> m_TreasuryRegions;

        public TownDefinition(int sort, string region, string friendlyName, string townName, string townCrystalName, string townBrazierName, string townFlagName, Point3D electionStoneLocation, Point3D[] eastWallLocations, Point3D[] northWallLocations, Point3D treasuryLocation, int treasuryID, Point3D goLocation, Rectangle3D[] treasuryRects, int costFactor)
		{
			m_Sort = sort;
			m_Region = region;
			m_TownName = townName;
            m_FriendlyName = friendlyName;
            m_TownCrystalName = townCrystalName;
            m_TownBrazierName = townBrazierName;
            m_TownFlagName = townFlagName;
            m_ElectionStoneLocation = electionStoneLocation;
            m_TreasuryWallEastLocations = eastWallLocations;
            m_TreasuryWallNorthLocations = northWallLocations;
            m_TreasuryLocation = treasuryLocation;
            m_TreasuryID = treasuryID;
            m_TreasuryGoLocation = goLocation;
            m_TreasuryRects = treasuryRects;
            m_CostFactor = costFactor;

			// Create treasury region(s)
			m_TreasuryRegions = new List<Server.Regions.TreasuryRegion>();
			if (m_TreasuryRects != null && m_TreasuryRects.Length > 0)
			{
				Server.Region parent_region = Server.Region.Find(m_TreasuryLocation, Map.Felucca);
				for(int i = 0; i < m_TreasuryRects.Length; ++i)
				{
					TreasuryRegion t = new TreasuryRegion(String.Format("{0} treasury region {1}", townName, i), m_TreasuryRects[i], parent_region, Map.Felucca);
					t.Register();
					m_TreasuryRegions.Add(t);
				}
			}

			m_TownId = ETownId.NumTowns;
			if (String.Compare(m_TownName, "Britain", true) == 0)
				m_TownId = ETownId.Britain;
			else if (String.Compare(m_TownName, "Cove", true) == 0)
				m_TownId = ETownId.Cove;
			else if (String.Compare(m_TownName, "Jhelom", true) == 0)
				m_TownId = ETownId.Jhelom;
			else if (String.Compare(m_TownName, "Magincia", true) == 0)
				m_TownId = ETownId.Magincia;
			else if (String.Compare(m_TownName, "Minoc", true) == 0)
				m_TownId = ETownId.Minoc;
			else if (String.Compare(m_TownName, "Moonglow", true) == 0)
				m_TownId = ETownId.Moonglow;
			else if (String.Compare(m_TownName, "Nujel'm", true) == 0)
				m_TownId = ETownId.Nujelm;
			else if (String.Compare(m_TownName, "Ocllo", true) == 0)
				m_TownId = ETownId.Ocllo;
			else if (String.Compare(m_TownName, "Serpent's Hold", true) == 0)
				m_TownId = ETownId.SerpentsHold;
			else if (String.Compare(m_TownName, "Skara Brae", true) == 0)
				m_TownId = ETownId.SkaraBrae;
			else if (String.Compare(m_TownName, "Trinsic", true) == 0)
				m_TownId = ETownId.Trinsic;
			else if (String.Compare(m_TownName, "Vesper", true) == 0)
				m_TownId = ETownId.Vesper;
			else if (String.Compare(m_TownName, "Yew", true) == 0)
				m_TownId = ETownId.Yew;

			Debug.Assert(m_TownId != ETownId.NumTowns);
		}
	}
}