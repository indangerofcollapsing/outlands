using System;
using System.Collections;
using System.Collections.Generic;

namespace Server.Engines.Craft
{
	public enum CraftMarkOption
	{
		MarkItem,
		DoNotMark,
		PromptForMark
	}

    public enum CraftRecycleOption
    {
        RecycleItem,
        RecycleAllNonExceptional,
        RecycleAll
    }

	public class CraftContext
	{
		private List<CraftItem> m_Items;
		private int m_LastResourceIndex;
		private int m_LastResourceIndex2;
		private int m_LastGroupIndex;
		private bool m_DoNotColor;
		private CraftMarkOption m_MarkOption;

        private CraftRecycleOption m_RecycleOption;
        private bool m_HighlightSkillGainItems = true;

		public List<CraftItem> Items { get { return m_Items; } }
		public int LastResourceIndex{ get{ return m_LastResourceIndex; } set{ m_LastResourceIndex = value; } }
		public int LastResourceIndex2{ get{ return m_LastResourceIndex2; } set{ m_LastResourceIndex2 = value; } }
		public int LastGroupIndex{ get{ return m_LastGroupIndex; } set{ m_LastGroupIndex = value; } }
		public bool DoNotColor{ get{ return m_DoNotColor; } set{ m_DoNotColor = value; } }
		public CraftMarkOption MarkOption{ get{ return m_MarkOption; } set{ m_MarkOption = value; } }

        public CraftRecycleOption RecycleOption { get { return m_RecycleOption; } set { m_RecycleOption = value; } }
        public bool HighlightSkillGainItems { get { return m_HighlightSkillGainItems; } set { m_HighlightSkillGainItems = value; } }

		public CraftContext()
		{
			m_Items = new List<CraftItem>();
			m_LastResourceIndex = -1;
			m_LastResourceIndex2 = -1;
			m_LastGroupIndex = -1;
		}

		public CraftItem LastMade
		{
			get
			{
				if ( m_Items.Count > 0 )
					return m_Items[0];

				return null;
			}
		}

		public void OnMade( CraftItem item )
		{
			m_Items.Remove( item );

			if ( m_Items.Count == 10 )
				m_Items.RemoveAt( 9 );

			m_Items.Insert( 0, item );
		}
	}
}