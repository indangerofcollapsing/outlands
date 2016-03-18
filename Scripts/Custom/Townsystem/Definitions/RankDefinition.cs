using System;

namespace Server.Custom.Townsystem
{
	public class RankDefinition
	{
		private int m_Rank;
		private int m_Required;
		private string m_Title;

		public int Rank{ get{ return m_Rank; } }
		public int Required{ get{ return m_Required; } }
		public string Title{ get{ return m_Title; } }

		public RankDefinition( int rank, int required, string title )
		{
			m_Rank = rank;
			m_Required = required;
			m_Title = title;
		}
	}
}