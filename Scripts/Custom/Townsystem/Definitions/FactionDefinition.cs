using System;

namespace Server.Custom.Townsystem
{
	public class FactionDefinition
	{
		private int m_Sort;

        private bool m_Primary;

		private int m_HuePrimary;
		private int m_HueFlag;
		private int m_HueJoin;
		private int m_HueBroadcast;

		private string m_FriendlyName;
		private string m_Keyword;
		private string m_Abbreviation;

		private string m_Name;
		private string m_PropName;
		private string m_Header;
		private string m_About;
        private int m_CrystalID;
		private string m_CityControl;
		private string m_SigilControl;
		private string m_SignupName;
		private string m_FactionStoneName;
		private string m_OwnerLabel;
        private int m_CapFlagID1;
        private int m_CapFlagID2;

		private string m_GuardIgnore, m_GuardWarn, m_GuardAttack;

		private RankDefinition[] m_Ranks;
		private GuardDefinition[] m_Guards;

		public int Sort{ get{ return m_Sort; } }

        public bool Primary { get { return m_Primary; } }

		public int HuePrimary{ get{ return m_HuePrimary; } }
		public int HueFlag{ get{ return m_HueFlag; } }
		public int HueJoin{ get{ return m_HueJoin; } }
		public int HueBroadcast{ get{ return m_HueBroadcast; } }

		public string FriendlyName{ get{ return m_FriendlyName; } }
		public string Keyword{ get{ return m_Keyword; } }
		public string Abbreviation{ get { return m_Abbreviation; } }

		public string Name{ get{ return m_Name; } }
		public string PropName{ get{ return m_PropName; } }
		public string Header{ get{ return m_Header; } }
		public string About{ get{ return m_About; } }
        public int CrystalID { get { return m_CrystalID; } }
		public string CityControl{ get{ return m_CityControl; } }
		public string SigilControl{ get{ return m_SigilControl; } }
		public string SignupName{ get{ return m_SignupName; } }
		public string FactionStoneName{ get{ return m_FactionStoneName; } }
		public string OwnerLabel{ get{ return m_OwnerLabel; } }

		public string GuardIgnore{ get{ return m_GuardIgnore; } }
		public string GuardWarn{ get{ return m_GuardWarn; } }
		public string GuardAttack{ get{ return m_GuardAttack; } }

		public RankDefinition[] Ranks{ get{ return m_Ranks; } }
		public GuardDefinition[] Guards{ get{ return m_Guards; } }

        public int CapFlagID1 { get { return m_CapFlagID1; } }
        public int CapFlagID2 { get { return m_CapFlagID2; } }

        public FactionDefinition(int sort, bool primary, int huePrimary, int HueFlag, int hueJoin, int hueBroadcast, int capFlagID1, int capFlagID2, string friendlyName, string keyword, string abbreviation, string name, string propName, string header, string about, int crystalID, string cityControl, string sigilControl, string signupName, string factionStoneName, string ownerLabel, string guardIgnore, string guardWarn, string guardAttack, RankDefinition[] ranks, GuardDefinition[] guards)
		{
			m_Sort = sort;
            m_Primary = primary;
			m_HuePrimary = huePrimary;
			m_HueFlag = HueFlag;
			m_HueJoin = hueJoin;
			m_HueBroadcast = hueBroadcast;
            m_CapFlagID1 = capFlagID1;
            m_CapFlagID2 = capFlagID2;
			m_FriendlyName = friendlyName;
			m_Keyword = keyword;
			m_Abbreviation = abbreviation;
			m_Name = name;
			m_PropName = propName;
			m_Header = header;
			m_About = about;
            m_CrystalID = crystalID;
			m_CityControl = cityControl;
			m_SigilControl = sigilControl;
			m_SignupName = signupName;
			m_FactionStoneName = factionStoneName;
			m_OwnerLabel = ownerLabel;
			m_GuardIgnore = guardIgnore;
			m_GuardWarn = guardWarn;
			m_GuardAttack = guardAttack;
			m_Ranks = ranks;
			m_Guards = guards;
		}
	}
}